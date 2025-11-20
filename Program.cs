using System;

namespace SmartThermostatSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var thermostat = new Thermostat();
            thermostat.InitializeDatabase();

            Console.WriteLine("=== Smart Thermostat Simulator ===");
            while (true)
            {
                Console.WriteLine("\n1. Get Current Temperature");
                Console.WriteLine("2. Set Target Temperature");
                Console.WriteLine("3. Run Simulation Step");
                Console.WriteLine("4. View Temperature Log");
                Console.WriteLine("5. Exit");

                Console.Write("> ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine($"Current: {thermostat.CurrentTemperature:F1}°F | Target: {thermostat.TargetTemperature:F1}°F");
                        break;

                    case "2":
                        Console.Write("Enter new target: ");
                        if (double.TryParse(Console.ReadLine(), out double target))
                        {
                            thermostat.SetTargetTemperature(target);
                            Console.WriteLine($"Target set to {target:F1}°F");
                        }
                        else
                        {
                            Console.WriteLine("Invalid number.");
                        }
                        break;

                    case "3":
                        thermostat.SimulateStep();
                        Console.WriteLine("Updated temperature and saved log.");
                        break;

                    case "4":
                        thermostat.PrintLogs();
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
