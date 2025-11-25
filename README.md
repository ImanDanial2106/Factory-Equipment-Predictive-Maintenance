# Factory Equipment Predictive Maintenance (PdM)

A Smart Factory solution that uses **Machine Learning (ML.NET)** to predict equipment failures before they happen. This project simulates IoT sensor data and visualizes real-time health status.

##  Machine Learning Model
* **Algorithm:** SDCA (Stochastic Dual Coordinate Ascent) / FastTree
* **Input Features:** Temperature, Vibration, Voltage, Rotation Speed
* **Target:** Binary Classification (Fail / No Fail) or RUL (Remaining Useful Life)
* **Training Data:** `sensor_data.csv` (Simulated industrial dataset)

##  Tech Stack
* **Language:** C# (.NET 8.0)
* **AI/ML:** ML.NET (Microsoft Machine Learning)
* **Dashboard:** ASP.NET Core MVC / Blazor
* **Database:** SQL Server (Production) / LocalDB (Dev)

## Setup & Run
1.  **Clone the repo:**
    ```bash
    git clone [https://github.com/ImanDanial2106/Factory-Equipment-Predictive-Maintenance.git](https://github.com/ImanDanial2106/Factory-Equipment-Predictive-Maintenance.git)
    ```
2.  **Database:**
    Run the SQL script `CREATE TABLE PredictiveMaintenanceLog.sql` to set up your local database.
3.  **App Settings:**
    Update `appsettings.json` with your local connection string.
4.  **Train Model:**
    Run the `ModelBuilder` console app (if included) to regenerate the `.zip` model file.
5.  **Run Dashboard:**
    ```bash
    dotnet run --project FactoryEquipmentPdM
    ```
