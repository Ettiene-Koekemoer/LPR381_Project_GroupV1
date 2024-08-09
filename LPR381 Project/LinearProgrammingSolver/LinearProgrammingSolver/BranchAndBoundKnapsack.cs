using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundKnapsack
    {
        private static double[,] tableau;
        private static int branchCount;
        private static List<List<(string Variable, int InOut, double Remainder)>> iterationResults = new List<List<(string Variable, int InOut, double Remainder)>>();
        private const int MaxIterations = 30; // Maximum number of iterations

        private static HashSet<string> processedBranches = new HashSet<string>(); // To track processed branches

        public static void Solve(LinearProgrammingModel model)
        {
            int rows = 3; // We will store Variable, InOut, and Remainder for each variable
            int columns = model.cN.Length; // Columns for each variable
            tableau = new double[rows, columns];

            Console.WriteLine("Starting conversion to canonical form...");
            // Convert the model to the canonical form
            ConvertToCanonicalForm(model);
            Console.WriteLine("Canonical form conversion complete.");

            // Initialize best solution variables
            double bestObjectiveValue = double.NegativeInfinity;
            List<(string Variable, int InOut, double Remainder)> bestSolution = null;

            var branches = new Queue<int>();
            branches.Enqueue(0); // Start with branch 0

            Console.WriteLine("Starting branch processing...");
            int iterationCount = 0; // Counter for iterations

            // Process branches
            while (branches.Count > 0 && iterationCount < MaxIterations)
            {
                int currentBranch = branches.Dequeue();
                Console.WriteLine($"Processing branch: {currentBranch}");

                var currentTable = ConvertTableToList(model);
                Console.WriteLine("Current table:");
                PrintTable(currentTable);

                // Save the current iteration
                iterationResults.Add(new List<(string Variable, int InOut, double Remainder)>(currentTable));
                iterationCount++;

                if (IsOptimalSolution(currentTable))
                {
                    double currentObjectiveValue = CalculateObjectiveValue(currentTable, model);
                    Console.WriteLine($"Optimal solution found with objective value: {currentObjectiveValue}");

                    if (currentObjectiveValue > bestObjectiveValue)
                    {
                        bestObjectiveValue = currentObjectiveValue;
                        bestSolution = new List<(string Variable, int InOut, double Remainder)>(currentTable);
                        Console.WriteLine("Best solution updated.");
                    }
                }
                else
                {
                    var branchVariable = GetBranchVariable(currentTable);
                    if (branchVariable != null)
                    {
                        Console.WriteLine($"Branching on variable: {branchVariable}");
                        var newBranches = CreateBranches(branchVariable, model);
                        foreach (var newBranch in newBranches)
                        {
                            branches.Enqueue(newBranch);
                        }
                    }
                }
            }

            Console.WriteLine("Branch processing complete or iteration limit reached.");
            WriteResults(CreateVariableTable(model), bestSolution, bestObjectiveValue);
        }

        private static void ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int numVariables = model.cN.Length;

            // Objective function (first row)
            for (int j = 0; j < numVariables; j++)
                tableau[0, j] = model.cN[j];

            // RHS value
            tableau[0, numVariables - 1] = 0;

            // Constraints (second and third rows)
            for (int j = 0; j < numVariables; j++)
            {
                tableau[1, j] = model.A[0, j];
                if (j == 0)
                {
                    tableau[2, j] = model.b[0]; // Initial Remainder is the RHS value
                }
                else
                {
                    tableau[2, j] = tableau[2, j - 1] - tableau[1, j]; // Subsequent remainders are calculated
                }
            }

            Console.WriteLine("Canonical form:");
            PrintTable(ConvertTableToList(model));
        }

        private static List<(string Variable, double Ratio, int Rank)> CreateVariableTable(LinearProgrammingModel model)
        {
            var variableTable = new List<(string Variable, double Ratio, int Rank)>();

            // Calculate ratios and ranks
            int numVariables = model.cN.Length;
            for (int j = 0; j < numVariables; j++)
            {
                double ratio = tableau[0, j] / tableau[1, j];
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

            Console.WriteLine("Variable Table:");
            PrintVariableTable(variableTable);

            return variableTable;
        }

        private static List<(string Variable, int InOut, double Remainder)> ConvertTableToList(LinearProgrammingModel model)
        {
            int numVariables = model.cN.Length;
            var tableList = new List<(string Variable, int InOut, double Remainder)>();

            for (int i = 0; i < model.A.GetLength(0); i++)
            {
                double remainder = model.b[i];
                for (int j = 0; j < numVariables; j++)
                {
                    var variable = $"x{j + 1}";
                    int inOutValue = (int)tableau[1, j];
                    remainder -= model.A[i, j] * inOutValue;
                    tableList.Add((variable, inOutValue, remainder));
                    if (remainder <= 0)
                    {
                        break;
                    }
                }
            }

            return tableList;
        }

        private static bool IsOptimalSolution(List<(string Variable, int InOut, double Remainder)> table)
        {
            return table.All(v => v.Remainder >= 0);
        }

        private static double CalculateObjectiveValue(List<(string Variable, int InOut, double Remainder)> table, LinearProgrammingModel model)
        {
            return table.Where(v => v.InOut == 1).Sum(v => model.cN[GetVariableIndex(v.Variable)]);
        }

        private static string GetBranchVariable(List<(string Variable, int InOut, double Remainder)> table)
        {
            var branchCandidate = table.FirstOrDefault(v => v.Remainder <= 0);
            return branchCandidate.Equals(default((string Variable, int InOut, double Remainder))) ? null : branchCandidate.Variable;
        }

        private static List<int> CreateBranches(string branchVariable, LinearProgrammingModel model)
        {
            var newBranches = new List<int>();

            // Create branches for In/Out = 0 and 1
            UpdateTableForBranch(branchVariable, 0, model);
            newBranches.Add(branchCount++);
            UpdateTableForBranch(branchVariable, 1, model);
            newBranches.Add(branchCount++);

            return newBranches;
        }

        private static void UpdateTableForBranch(string branchVariable, int inOutValue, LinearProgrammingModel model)
        {
            int variableIndex = GetVariableIndex(branchVariable);
            int numVariables = model.cN.Length;

            // Update the In/Out column
            tableau[1, variableIndex] = inOutValue;

            // Update the Remainder column iteratively
            double remainder = model.b[0];

            for (int j = 0; j < numVariables; j++)
            {
                if (inOutValue == 0)
                {
                    // When In/Out is 0, the coefficient of this variable should not be subtracted
                    tableau[2, j] = remainder;
                }
                else
                {
                    // Subtract the coefficient of the variable from the remainder
                    remainder -= model.A[0, j] * inOutValue;
                    tableau[2, j] = remainder;

                    // Stop branching if remainder becomes zero or negative
                    if (remainder <= 0)
                    {
                        break;
                    }
                }
            }

            Console.WriteLine($"Updated table for branch variable {branchVariable} with In/Out value {inOutValue}:");
            PrintTable(ConvertTableToList(model));
        }

        private static void WriteResults(List<(string Variable, double Ratio, int Rank)> variableTable, List<(string Variable, int InOut, double Remainder)> bestSolution, double bestObjectiveValue)
        {
            string outputPath = "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt";
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("Variable Table:");
                writer.WriteLine(BuildVariableTableFromList(variableTable));

                writer.WriteLine("Iterations:");
                foreach (var iteration in iterationResults)
                {
                    writer.WriteLine("Iteration:");
                    writer.WriteLine(BuildTableFromList(iteration));
                }

                writer.WriteLine("Best Solution:");
                writer.WriteLine(BuildTableFromList(bestSolution ?? new List<(string Variable, int InOut, double Remainder)>()));
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
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "The table cannot be null.");
            }

            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine($"{"Variable",10} {"In/Out",10} {"Remainder",10}");

            foreach (var entry in table)
            {
                tableBuilder.AppendLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
            }

            return tableBuilder.ToString();
        }

        private static int GetVariableIndex(string variableName)
        {
            return int.Parse(variableName.Substring(1)) - 1;
        }

        private static void PrintTable(List<(string Variable, int InOut, double Remainder)> table)
        {
            Console.WriteLine($"{"Variable",10} {"In/Out",10} {"Remainder",10}");
            foreach (var entry in table)
            {
                Console.WriteLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
            }
        }

        private static void PrintVariableTable(List<(string Variable, double Ratio, int Rank)> table)
        {
            Console.WriteLine($"{"Variable",10} {"Ratio",10} {"Rank",10}");
            foreach (var entry in table)
            {
                Console.WriteLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }
        }
    }
}
