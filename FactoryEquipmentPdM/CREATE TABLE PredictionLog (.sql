CREATE TABLE PredictionLog (
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    MachineID VARCHAR2(50),
    Temperature_Input NUMBER,
    Vibration_Input NUMBER,
    HoursRun_Input NUMBER,
    Pressure_Input NUMBER,
    Prediction_Result NUMBER
);