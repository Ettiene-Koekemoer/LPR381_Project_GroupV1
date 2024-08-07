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
            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);

            // Create a table for variables, ratios, and ranks
            var variableTable = CreateVariableTable(tableau, model);

            // Initialize the branches
            var branches = InitializeBranches(variableTable, model);

            // Initialize best solution variables
            double bestObjectiveValue = double.NegativeInfinity;
            List<(string Variable, int InOut, double Remainder)> bestSolution = null;

            // Process branches
            while (branches.Count > 0)
            {
                var (branchName, currentTable) = branches.Dequeue();

                // Check if the current table is an optimal solution
                if (IsOptimalSolution(currentTable))
                {
                    double currentObjectiveValue = CalculateObjectiveValue(currentTable, model);
                    if (currentObjectiveValue > bestObjectiveValue)
                    {
                        bestObjectiveValue = currentObjectiveValue;
                        bestSolution = new List<(string Variable, int InOut, double Remainder)>(currentTable);
                    }
                }
                else
                {
                    // Determine the next branching variable
                    var branchVariable = GetBranchVariable(currentTable);
                    if (branchVariable != null)
                    {
                        // Create new branches
                        var newTables = CreateBranches(currentTable, branchVariable, model);
                        foreach (var (newBranchName, newTable) in newTables)
                        {
                            branches.Enqueue((newBranchName, newTable));
                        }
                    }
                }
            }

            // Output results
            WriteResults(variableTable, bestSolution, bestObjectiveValue);
        }

        private static double[,] ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int rows = model.A.GetLength(0) + 1;
            int columns = model.A.GetLength(1) + model.B.Count + 1;
            double[,] tableau = new double[rows, columns];

            // Objective function
            for (int j = 0; j < model.cN.Length; j++)
                tableau[0, j] = model.cN[j];

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
                // Calculate the ratio
                double ratio = tableau[0, j] / tableau[1, j];
                // ratio *= -1; // Adjust for positive values

                variableTable.Add(($"x{j + 1}", ratio, 0));
            }

            // Rank variables based on ratio
            if (model.IsMaximization)
            {
                // Sort in descending order for maximization
                variableTable = variableTable.OrderBy(v => v.Ratio).ToList();
            }
            else
            {
                // Sort in ascending order for minimization
                variableTable = variableTable.OrderByDescending(v => v.Ratio).ToList();
            }

            // Assign ranks
            for (int i = 0; i < variableTable.Count; i++)
            {
                variableTable[i] = (variableTable[i].Variable, variableTable[i].Ratio, i + 1);
            }

            return variableTable;
        }


        private static Queue<(string BranchName, List<(string Variable, int InOut, double Remainder)> Table)> InitializeBranches(List<(string Variable, double Ratio, int Rank)> variableTable, LinearProgrammingModel model)
        {
            var branches = new Queue<(string BranchName, List<(string Variable, int InOut, double Remainder)> Table)>();
            branches.Enqueue(("P0", CreateInitialTable(variableTable, model)));
            return branches;
        }

        private static List<(string Variable, int InOut, double Remainder)> CreateInitialTable(List<(string Variable, double Ratio, int Rank)> variableTable, LinearProgrammingModel model)
        {
            var initialTable = variableTable.Select(v => (v.Variable, InOut: 0, Remainder: model.b.Max() /* Placeholder for remainder */)).ToList();
            return initialTable;
        }

        private static bool IsOptimalSolution(List<(string Variable, int InOut, double Remainder)> table)
        {
            // Check if all variables are included and there is no negative remainder
            return table.All(v => v.Remainder >= 0);
        }

        private static double CalculateObjectiveValue(List<(string Variable, int InOut, double Remainder)> table, LinearProgrammingModel model)
        {
            return table.Where(v => v.InOut == 1).Sum(v => model.cN[GetVariableIndex(v.Variable)]);
        }

        private static string GetBranchVariable(List<(string Variable, int InOut, double Remainder)> table)
        {
            return table.FirstOrDefault(v => v.Remainder <= 0).Variable;
        }

        private static List<(string BranchName, List<(string Variable, int InOut, double Remainder)> Table)> CreateBranches(List<(string Variable, int InOut, double Remainder)> table, string branchVariable, LinearProgrammingModel model)
        {
            var newTables = new List<(string BranchName, List<(string Variable, int InOut, double Remainder)> Table)>();

            // Branch with the variable set to 0
            var tableZero = UpdateTableForBranch(table, branchVariable, 0, model);
            newTables.Add(("P0.1", tableZero));

            // Branch with the variable set to 1
            var tableOne = UpdateTableForBranch(table, branchVariable, 1, model);
            newTables.Add(("P0.2", tableOne));

            return newTables;
        }

        private static List<(string Variable, int InOut, double Remainder)> UpdateTableForBranch(List<(string Variable, int InOut, double Remainder)> table, string branchVariable, int inOutValue, LinearProgrammingModel model)
        {
            return table.Select(v =>
                (v.Variable, InOut: v.Variable == branchVariable ? inOutValue : v.InOut, Remainder: CalculateRemainder(v, branchVariable, inOutValue, model))
            ).ToList();
        }

        private static double CalculateRemainder((string Variable, int InOut, double Remainder) entry, string branchVariable, int inOutValue, LinearProgrammingModel model)
        {
            if (entry.Variable == branchVariable)
            {
                return entry.Remainder; // Adjust based on problem constraints
            }
            else
            {
                // Adjust remainder based on constraint coefficients
                return entry.Remainder; // Adjust as needed based on the problem constraints
            }
        }

        private static void WriteResults(List<(string Variable, double Ratio, int Rank)> variableTable, List<(string Variable, int InOut, double Remainder)> bestSolution, double bestObjectiveValue)
        {
            string outputPath = "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt";
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("Variable Table:");
                writer.WriteLine(BuildVariableTableFromList(variableTable));
                writer.WriteLine("Best Solution:");
                writer.WriteLine(BuildTableFromList(bestSolution));
                writer.WriteLine($"Best Objective Value: {bestObjectiveValue:F3}");
            }

            Console.WriteLine($"Results written to {outputPath}");
        }

        private static string BuildVariableTableFromList(List<(string Variable, double Ratio, int Rank)> table)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine($"{"Variable",10} {"Ratio",10} {"Rank",10}");

            foreach (var entry in table)
            {
                tableBuilder.AppendLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }

            return tableBuilder.ToString();
        }

        private static string BuildTableFromList(List<(string Variable, int InOut, double Remainder)> table)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine($"{"Variable",10} {"In/Out",10} {"Remainder",10}");

            foreach (var entry in table)
            {
                tableBuilder.AppendLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
            }

            return tableBuilder.ToString();
        }

        private static int GetVariableIndex(string variable)
        {
            // Assume that variable names are in the form "x1", "x2", etc.
            return int.Parse(variable.TrimStart('x')) - 1;
        }
    }
}