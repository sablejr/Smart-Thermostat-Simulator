using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SmartThermostatSimulator
{
    public class Thermostat
    {
        public double CurrentTemperature { get; private set; } = 70.0;
        public double TargetTemperature { get; private set; } = 72.0;

        private string _connectionString = "Data Source=Data/TemperatureLogs.db";

        public void InitializeDatabase()
        {
            Directory.CreateDirectory("Data");
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT,
                    CurrentTemp REAL,
                    TargetTemp REAL
                );";

            cmd.ExecuteNonQuery();
        }

        public void SetTargetTemperature(double value)
        {
            TargetTemperature = value;
        }

        public void SimulateStep()
        {
            if (CurrentTemperature < TargetTemperature)
                CurrentTemperature = Math.Min(TargetTemperature, CurrentTemperature + 0.3);
            else if (CurrentTemperature > TargetTemperature)
                CurrentTemperature = Math.Max(TargetTemperature, CurrentTemperature - 0.3);

            SaveLog();
        }

        private void SaveLog()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO Logs (Timestamp, CurrentTemp, TargetTemp)
                  VALUES ($ts, $cur, $tar);";

            cmd.Parameters.AddWithValue("$ts", DateTime.UtcNow.ToString("s"));
            cmd.Parameters.AddWithValue("$cur", CurrentTemperature);
            cmd.Parameters.AddWithValue("$tar", TargetTemperature);
            cmd.ExecuteNonQuery();
        }

        public void PrintLogs()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Timestamp, CurrentTemp, TargetTemp FROM Logs ORDER BY Id DESC LIMIT 100";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetString(0)} | {reader.GetDouble(1):F1}°F → {reader.GetDouble(2):F1}°F");
            }
        }
    }
}
