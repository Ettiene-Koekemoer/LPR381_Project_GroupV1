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
            int rows = 3;
            int columns = model.cN.Length;
            tableau = new double[rows, columns];

            Console.WriteLine("Starting conversion to canonical form...");
            ConvertToCanonicalForm(model);
            Console.WriteLine("Canonical form conversion complete.");

            double bestObjectiveValue = double.NegativeInfinity;
            List<(string Variable, int InOut, double Remainder)> bestSolution = null;

            var branches = new Queue<int>();
            branches.Enqueue(0);

            Console.WriteLine("Starting branch processing...");
            int iterationCount = 0;

            while (branches.Count > 0 && iterationCount < MaxIterations)
            {
                int currentBranch = branches.Dequeue();
                Console.WriteLine($"Processing branch: {currentBranch}");

                var currentTable = ConvertTableToList(model);
                Console.WriteLine("Current table:");
                PrintTable(currentTable);

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
                            if (!processedBranches.Contains(newBranch.ToString()))
                            {
                                branches.Enqueue(newBranch);
                                processedBranches.Add(newBranch.ToString());
                            }
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
                double ratio = tableau[0, j] / (tableau[1, j] == 0 ? 1 : tableau[1, j]); // Avoid division by zero
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
            double objectiveValue = 0.0;
            // Compute objective value
            foreach (var entry in table)
            {
                int variableIndex = GetVariableIndex(entry.Variable);
                objectiveValue += model.cN[variableIndex] * entry.InOut;
            }

            return objectiveValue;
        }

        private static string GetBranchVariable(List<(string Variable, int InOut, double Remainder)> table)
        {
            var branchCandidate = table.FirstOrDefault(v => v.Remainder <= 0);
            return branchCandidate.Equals(default((string Variable, int InOut, double Remainder))) ? null : branchCandidate.Variable;
        }

        private static List<int> CreateBranches(string branchVariable, LinearProgrammingModel model)
        {
            var newBranches = new List<int>();

            // Example for branching on x3
            UpdateTableForBranch(branchVariable, 0, model); // Create a branch with x3 = 0
            newBranches.Add(branchCount++);

            UpdateTableForBranch(branchVariable, 1, model); // Create a branch with x3 = 1
            newBranches.Add(branchCount++);

            return newBranches;
        }

        private static void UpdateTableForBranch(string branchVariable, int inOutValue, LinearProgrammingModel model)
        {
            int variableIndex = GetVariableIndex(branchVariable);

            // Update table logic
            tableau[1, variableIndex] = inOutValue;

            double remainder = model.b[0];
            for (int j = 0; j < tableau.GetLength(1); j++)
            {
                remainder -= model.A[0, j] * inOutValue;
                tableau[2, j] = remainder;
            }
        }

        private static void WriteResults(List<(string Variable, double Ratio, int Rank)> variableTable, List<(string Variable, int InOut, double Remainder)> bestSolution, double bestObjectiveValue)
        {
            var output = new StringBuilder();
            output.AppendLine("Variable Table:");
            foreach (var entry in variableTable)
            {
                output.AppendLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }

            output.AppendLine("\nIterations:");
            foreach (var iteration in iterationResults)
            {
                output.AppendLine("Iteration:");
                foreach (var entry in iteration)
                {
                    output.AppendLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
                }
            }

            output.AppendLine($"\nBest Objective Value: {bestObjectiveValue:F3}");

            File.WriteAllText("output.txt", output.ToString());
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
