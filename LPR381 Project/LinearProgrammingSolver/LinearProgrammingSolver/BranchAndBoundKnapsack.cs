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
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");
            }

            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);
            if (tableau == null)
            {
                throw new InvalidOperationException("Tableau conversion failed.");
            }

            string canonicalFormOutput = BuildTable(tableau);

            // Write canonical form to a text file
            WriteOutput(canonicalFormOutput, "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt");

            // Create a table for variables, ratios, and ranks
            var variableTable = CreateVariableTable(tableau, model);

            // Display the variable table
            DisplayVariableTable(variableTable);

            // Write variable table to a text file
            WriteOutput(BuildVariableTable(variableTable), "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt");

            // Branch and Bound logic to find the optimal solution
            PerformBranchAndBound(variableTable, model, tableau);
        }


        private static double[,] ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            // Check for null values to avoid NullReferenceException
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (model.A == null)
                throw new ArgumentNullException(nameof(model.A));
            if (model.b == null)
                throw new ArgumentNullException(nameof(model.b));
            if (model.cB == null)
                throw new ArgumentNullException(nameof(model.cB));
            if (model.cN == null)
                throw new ArgumentNullException(nameof(model.cN));

            // Dimensions of the tableau
            int numConstraints = model.A.GetLength(0); // Number of constraints
            int numVariables = model.A.GetLength(1); // Number of variables
            int numSlackVariables = numConstraints; // Number of slack variables
            int totalColumns = numVariables + numSlackVariables + 1; // Extra column for RHS
            int totalRows = numConstraints + 1; // Extra row for the objective function

            double[,] tableau = new double[totalRows, totalColumns];

            // Populate the objective function row (first row)
            for (int j = 0; j < numVariables; j++)
            {
                tableau[0, j] = -model.cN[j]; // Convert to canonical form by negating the objective coefficients
            }

            // Populate the constraints rows
            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numVariables; j++)
                {
                    tableau[i + 1, j] = model.A[i, j]; // Constraint coefficients
                }
                tableau[i + 1, numVariables + i] = 1; // Slack variable
                tableau[i + 1, totalColumns - 1] = model.b[i]; // Right-hand side value
            }

            return tableau;
        }


        private static List<(string Variable, double Ratio, int Rank)> CreateVariableTable(double[,] tableau, LinearProgrammingModel model)
        {
            var variableTable = new List<(string Variable, double Ratio, int Rank)>();

            if (tableau == null || model == null)
            {
                throw new ArgumentNullException("Tableau or model is null.");
            }

            int numVariables = model.cB.Length;

            // Ensure tableau dimensions are correct
            if (tableau.GetLength(1) < numVariables)
            {
                throw new ArgumentException("Tableau dimensions are incorrect.");
            }

            // Calculate ratios and ranks
            for (int j = 0; j < numVariables; j++)
            {
                // Check if the column index is valid
                if (tableau.GetLength(0) > 1)
                {
                    double ratio = tableau[0, j] / tableau[1, j]; // Adjust as necessary
                    variableTable.Add(($"x{j + 1}", ratio, 0));
                }
                else
                {
                    throw new ArgumentException("Tableau does not have sufficient rows for calculations.");
                }
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



        private static void DisplayVariableTable(List<(string Variable, double Ratio, int Rank)> variableTable)
        {
            Console.WriteLine($"{"Variable",10} {"Ratio",10} {"Rank",10}");
            foreach (var entry in variableTable)
            {
                Console.WriteLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }
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

        private static void WriteOutput(string output, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(output);
            }
            Console.WriteLine($"Results written to {fileName}");
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

        private static void PerformBranchAndBound(List<(string Variable, double Ratio, int Rank)> variableTable, LinearProgrammingModel model, double[,] tableau)
        {
            // Implement the branch and bound logic here based on the input format
            // This method should iterate over the variable table and perform the necessary calculations
            // and branching to find the optimal solution.

            // This is just an example. You will need to implement the actual logic here.
            Console.WriteLine("Performing Branch and Bound...");
            
            // Add the rest of the branch and bound logic here as per your requirements.
        }
    }
}
