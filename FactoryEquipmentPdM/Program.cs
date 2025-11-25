// Add these using statements at the top if they aren't already there
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.IO;
using SampleClassification.ConsoleApp; // Your model namespace
using System; // Needed for Console.ReadLine and float.TryParse

// --- Build Configuration and Get Connection String FIRST ---
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string? connectionString = configuration.GetConnectionString("OracleDbConnection");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: OracleDbConnection connection string not found in appsettings.json");
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
    return; // Exit
}
// --- END of Configuration Loading ---


// --- Get Sensor Data from User ---
Console.WriteLine("Enter sensor data:");

// Get Machine ID
Console.Write("Machine ID (e.g., M1, M2): ");
string? machineId = Console.ReadLine(); // Read machine ID as string

// Get Temperature
float temperature;
while (true) // Loop until valid input is given
{
    Console.Write("Temperature (e.g., 85.5): ");
    string? tempInput = Console.ReadLine();
    if (float.TryParse(tempInput, out temperature))
    {
        break; // Exit loop if conversion is successful
    }
    Console.WriteLine("Invalid input. Please enter a number.");
}

// Get Vibration
float vibration;
while (true)
{
    Console.Write("Vibration (e.g., 4.2): ");
    string? vibInput = Console.ReadLine();
    if (float.TryParse(vibInput, out vibration))
    {
        break;
    }
    Console.WriteLine("Invalid input. Please enter a number.");
}

// Get Hours Run
float hoursRun;
while (true)
{
    Console.Write("Hours Run (e.g., 3450.0): ");
    string? hoursInput = Console.ReadLine();
    if (float.TryParse(hoursInput, out hoursRun))
    {
        break;
    }
    Console.WriteLine("Invalid input. Please enter a number.");
}

// Get Pressure
float pressure;
while (true)
{
    Console.Write("Pressure (e.g., 115.8): ");
    string? pressureInput = Console.ReadLine();
    if (float.TryParse(pressureInput, out pressure))
    {
        break;
    }
    Console.WriteLine("Invalid input. Please enter a number.");
}

Console.WriteLine("\nChecking status...");
// --- End of Getting User Input ---


// --- Prepare Input for the Model ---
// Use the variables read from the user
var modelInput = new SampleClassification.ConsoleApp.SampleClassification.ModelInput()
{
    MachineID = machineId ?? "Unknown", // Handle potential null input for MachineID
    Temperature = temperature,
    Vibration = vibration,
    HoursRun = hoursRun,
    Pressure = pressure
};

// --- Make the Prediction ---
var predictionResult = SampleClassification.ConsoleApp.SampleClassification.Predict(modelInput);

// --- Display the Result ---
var prediction = (int)predictionResult.PredictedLabel;
Console.WriteLine($"\nPrediction Result: {prediction}");

if (prediction == 1)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"WARNING: Machine {machineId} is likely to fail!");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Status OK: Machine {machineId} is operating normally.");
    Console.ResetColor();
}

// --- Log Prediction to Oracle Database ---
// (The database logging code remains exactly the same as before)
Console.WriteLine("\nLogging prediction to database...");
OracleTransaction? transaction = null;
try
{
    using (OracleConnection connection = new OracleConnection(connectionString))
    {
        // ... (rest of database code: Open, BeginTransaction, INSERT, Commit/Rollback) ...
        connection.Open();
        transaction = connection.BeginTransaction();

        string sql = @"INSERT INTO PredictionLog
                       (MachineID, Temperature_Input, Vibration_Input, HoursRun_Input, Pressure_Input, Prediction_Result)
                       VALUES
                       (:MachineID, :Temperature, :Vibration, :HoursRun, :Pressure, :Prediction)";

        using (OracleCommand command = new OracleCommand(sql, connection))
        {
            command.Transaction = transaction;

            // Use the user-provided variables here too
            command.Parameters.Add(new OracleParameter("MachineID", machineId ?? "Unknown"));
            command.Parameters.Add(new OracleParameter("Temperature", temperature));
            command.Parameters.Add(new OracleParameter("Vibration", vibration));
            command.Parameters.Add(new OracleParameter("HoursRun", hoursRun));
            command.Parameters.Add(new OracleParameter("Pressure", pressure));
            command.Parameters.Add(new OracleParameter("Prediction", prediction));

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                transaction.Commit();
                Console.WriteLine("Log successfully inserted and committed to Oracle database.");
            }
            else
            {
                transaction.Rollback();
                Console.WriteLine("Failed to insert log into database.");
            }
        }
    }
}
catch (OracleException ex)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine($"Oracle Database Error: {ex.Message}");
    transaction?.Rollback();
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    transaction?.Rollback();
    Console.ResetColor();
}
// --- End of Database Logging ---

// (Keep Console.ReadKey commented out or remove)