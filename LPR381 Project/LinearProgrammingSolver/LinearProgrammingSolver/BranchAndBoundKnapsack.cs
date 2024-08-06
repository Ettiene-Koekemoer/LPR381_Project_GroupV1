using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundKnapsack
    {
        public static void Solve(LinearProgrammingModel model)
        {
            // Read the input file and convert it to a string
            // string inputFilePath = "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Models/YourInputFile.txt";
            // string inputFileContent = File.ReadAllText(inputFilePath);

            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);
            string canonicalFormOutput = BuildTable(tableau);

            // Create a table for variables, ratios, and ranks
            var variableTable = CreateVariableTable(tableau, model);

            // Display the variable table
            string variableTableOutput = BuildVariableTable(variableTable);

            // Write all results to a text file
            string outputPath = "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt";
            using (var writer = new StreamWriter(outputPath))
            {
                // Write the input problem
                // writer.WriteLine("Problem Input:");
                // writer.WriteLine(inputFileContent);
                writer.WriteLine();

                // Write the canonical form
                writer.WriteLine("Canonical Form:");
                writer.WriteLine(canonicalFormOutput);
                writer.WriteLine();

                // Write the variable table
                writer.WriteLine("Variable Table:");
                writer.WriteLine(variableTableOutput);

                // You can add iterations or additional results here if needed
                writer.WriteLine("Iterations and Additional Results:");
                // Example placeholder for additional results
                // writer.WriteLine("Iteration 1: ...");
                // writer.WriteLine("Iteration 2: ...");
            }

            Console.WriteLine($"Results written to {outputPath}");
        }

        private static double[,] ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int rows = model.A.GetLength(0) + 1;
            int columns = model.A.GetLength(1) + model.B.Count + 1;
            double[,] tableau = new double[rows, columns];

            // Objective function
            for (int j = 0; j < model.cN.Length; j++)
                tableau[0, j] = -model.cN[j];

            // Constraints
            for (int i = 0; i < model.A.GetLength(0); i++)
            {
                for (int j = 0; j < model.A.GetLength(1); j++)
                {
                    tableau[i + 1, j] = model.A[i, j];
                }
                tableau[i + 1, model.A.GetLength(1) + i] = 1; // Slack variable
                tableau[i + 1, columns - 1] = model.b[i];
            }

            return tableau;
        }

        private static List<(string Variable, double Ratio, int Rank)> CreateVariableTable(double[,] tableau, LinearProgrammingModel model)
        {
            var variableTable = new List<(string Variable, double Ratio, int Rank)>();

            // Calculate ratios and ranks
            for (int j = 0; j < model.cN.Length; j++)
            {
                double ratio = tableau[0, j] / tableau[1, j];
                ratio *= -1; //Added to ensure that the values are positive.
                variableTable.Add(($"x{j + 1}", ratio, 0));
            }

            // Rank variables based on ratio
            if (model.IsMaximization)
            {
                variableTable = variableTable.OrderByDescending(v => v.Ratio).ToList();
            }
            else
            {
                variableTable = variableTable.OrderBy(v => v.Ratio).ToList();
            }

            // Assign ranks
            for (int i = 0; i < variableTable.Count; i++)
            {
                variableTable[i] = (variableTable[i].Variable, variableTable[i].Ratio, i + 1);
            }

            return variableTable;
        }

        private static string BuildTable(double[,] table)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("");

            int rows = table.GetLength(0);
            int cols = table.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tableBuilder.Append($"{table[i, j],10:F3}");
                }
                tableBuilder.AppendLine();
            }
            tableBuilder.AppendLine();
            return tableBuilder.ToString();
        }

        private static string BuildVariableTable(List<(string Variable, double Ratio, int Rank)> variableTable)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine($"{"Variable",10} {"Ratio",10} {"Rank",10}");

            foreach (var entry in variableTable)
            {
                tableBuilder.AppendLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }

            return tableBuilder.ToString();
        }
    }
}
