/*
    This is the Computer Class

    Gives ability to create a simple computer object and several methods dealing with computers
*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Final_Project {

    public class Computer {

        // Computer attributes
        public string Computer_Name;
        public string Building;
        public bool Physical_Machine;
        public bool Active;

        // Basic computer constructor
        public Computer computer { get; set; }

        // Returns a list of computers that will be displayed to user after selecting what they would like to see
        public static List<Computer> SelectComputersFromBuilding(List<Computer> computerList) {

            // Generates the building menu to show them what is available and their options
            var selectedBuilding = Menu.DisplayBuildingMenu(computerList);

            if (selectedBuilding == "ALL") {
                return computerList;
            }
            else {
                var selectedComputers = computerList.FindAll(computer => computer.Building == selectedBuilding);
                return selectedComputers;
            }
        }

        // Displays a list of the selected computers after choosing a building or showing them all
        public static void DisplayListOfComputers(List<Computer> computerList, Database database) {

            // Collects the list of computers that will be shown
            var selectedComputers = SelectComputersFromBuilding(computerList);
            // Gets the column headers for displaying the results for easy identification
            var computerHeaders = Database.GetComputerTableHeaders(database);

            Console.Clear();

            // This is the way the computers are displayed as long as there is atleast 1 result
            if (selectedComputers.Count > 0) {
                ConsoleView.SetColors(ConsoleColor.Yellow);
                Console.WriteLine(string.Format("{0} {1} {2,-20} {3,-10}",
                    computerHeaders[0].PadRight(20), computerHeaders[1].PadRight(10), computerHeaders[2], computerHeaders[3]));
                ConsoleView.ResetColor();

                foreach (Computer computer in selectedComputers) {

                    Console.WriteLine(string.Format("{0} {1} {2,-20} {3,-10}",
                        computer.Computer_Name.PadRight(20), computer.Building.PadRight(10), computer.Physical_Machine, computer.Active
                    ));
                }
                ConsoleView.SetColors(ConsoleColor.Yellow);
                Console.WriteLine();
                Console.Write("Total Computers Diplayed: ");
                ConsoleView.ResetColor();
                Console.WriteLine(selectedComputers.Count);
            }
            // If there are no results possible to show, it will display no results
            else {
                ConsoleView.SetColors(ConsoleColor.Yellow);
                Console.WriteLine("No Results to display");
                ConsoleView.ResetColor();
            }
        }

        // Identifies computers in CSV
        public static List<Computer> ImportComputersFromCSV(string fileName, Database database) {

            var importedComputers = new List<Computer>();

            if (File.Exists(database.FileName)) {

                Console.Clear();
                int currentProgress = 0;
                int totalComputers = 0;

                using(var reader = new StreamReader(File.OpenRead(fileName))) {
                    reader.ReadLine();
                    while (reader.ReadLine() != null) {
                        totalComputers = totalComputers + 1;
                    }
                }

                Console.WriteLine("Identifying computers in CSV...");

                using(var reader = new StreamReader(File.OpenRead(fileName))) {

                    reader.ReadLine();

                    while (!reader.EndOfStream) {
                        var computer = new Computer();
                        var line = reader.ReadLine().Split(",");

                        computer.Computer_Name = line[0].ToUpper();
                        computer.Building = line[1];
                        computer.Physical_Machine = Convert.ToBoolean(line[2]);
                        computer.Active = Convert.ToBoolean(line[3]);

                        importedComputers.Add(computer);

                        ProgressBar.ShowProgressBar(currentProgress, totalComputers);
                        currentProgress = currentProgress + 1;

                    }
                }

                Console.WriteLine();
                Console.WriteLine("Identification complete");
                Console.WriteLine();

                return importedComputers;

            }
            else {
                return importedComputers;
            }

        }

        // Displays number of computers per building
        public static void ShowComputerCountPerBuilding(Database database) {

            var computers = new List<Computer>();
            var buildingList = new List<string>();

            Console.Clear();

            ConsoleView.SetColors(ConsoleColor.Yellow);
            Console.WriteLine("Computers per Building");
            ConsoleView.ResetColor();

            using(var command = new SQLiteCommand(database.SelectDataQuery, database.DBConnection)) {

                using(var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var computer = new Computer();
                        computer.Computer_Name = reader.GetString(0);
                        computer.Building = reader.GetString(1);
                        computer.Physical_Machine = reader.GetBoolean(2);
                        computer.Active = reader.GetBoolean(3);

                        computers.Add(computer);
                    }
                }
            }

            var uniqueBuildings = computers.GroupBy(computer => computer.Building).ToList();

            foreach (var building in uniqueBuildings) {
                Console.Write(" " + building.Key + " - ");
                Console.WriteLine(computers.Where(computer => computer.Building == building.Key).Count());
            }

            ConsoleView.SetColors(ConsoleColor.Yellow);
            Console.Write("Total Computers: ");
            ConsoleView.ResetColor();
            Console.WriteLine(computers.Count);
            Console.WriteLine();
            Console.WriteLine();
        }

        // Used to change the computer name when importing CSV
        public static string ChangeComputerName(string oldComputerName) {

            string newComputerName = "";
            int selectionError = 0;

            while (newComputerName == "") {

                if (selectionError != 0) {
                    Console.Clear();
                    Console.WriteLine();
                    ConsoleView.SetColors(ConsoleColor.Magenta);
                    Console.WriteLine("Incorrect selection, please try again...");
                    ConsoleView.ResetColor();
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Please choose a new name for your computers.");
                Console.Write("Currently your computers are named like - ");
                ConsoleView.SetColors(ConsoleColor.Yellow);
                Console.WriteLine(oldComputerName);
                ConsoleView.ResetColor();
                Console.WriteLine();
                Console.Write("Enter new name to replace ");
                ConsoleView.SetColors(ConsoleColor.Yellow);
                Console.Write(oldComputerName.Split("-") [0]);
                ConsoleView.ResetColor();
                Console.Write(": ");
                newComputerName = Console.ReadLine();

                if (newComputerName.IndexOf(" ") >= 0 || newComputerName == "") {
                    newComputerName = "";
                    selectionError = 1;
                }
            }

            return newComputerName;
        }
    }
}
