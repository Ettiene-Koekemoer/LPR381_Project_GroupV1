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
        private static List<List<(string Variable, int InOut, double Remainder)>> iterationResults = new List<List<(string Variable, int InOut, double Remainder)>>();
        private const int MaxIterations = 30; // Maximum number of iterations

        private static HashSet<string> processedBranches = new HashSet<string>(); // To track processed branches

        public static void Solve(LinearProgrammingModel model)
        {
            int rows = 3;
            int columns = model.cN.Length;
            tableau = new double[rows, columns];

            double bestObjectiveValue = double.NegativeInfinity;
            List<(string Variable, int InOut, double Remainder)> bestSolution = null;

            var variableTable = CreateVariableTable(model);
            var branches = new Queue<List<(string Variable, int InOut, double Remainder)>>();
            branches.Enqueue(SolveBranch(variableTable, model));

            Console.WriteLine("Starting branch processing...");
            int iterationCount = 0;

            while (branches.Count > 0 && iterationCount < MaxIterations)
            {
                var currentTable = branches.Dequeue();
                Console.WriteLine($"Processing branch with current table:");

                PrintTable(currentTable);

                iterationResults.Add(new List<(string Variable, int InOut, double Remainder)>(currentTable));
                iterationCount++;

                double currentObjectiveValue = CalculateObjectiveValue(currentTable, model);
                if (currentObjectiveValue > bestObjectiveValue)
                {
                    bestObjectiveValue = currentObjectiveValue;
                    bestSolution = new List<(string Variable, int InOut, double Remainder)>(currentTable);
                    Console.WriteLine($"Best solution updated with objective value: {bestObjectiveValue}");
                }

                if (!IsOptimalSolution(currentTable))
                {
                    var branchVariable = GetBranchVariable(currentTable);
                    if (branchVariable != null)
                    {
                        Console.WriteLine($"Branching on variable: {branchVariable}");
                        var newBranches = CreateBranches(branchVariable, variableTable, model);
                        foreach (var newBranch in newBranches)
                        {
                            var branchKey = string.Join(",", newBranch.Select(x => $"{x.Variable},{x.InOut},{x.Remainder:F3}"));
                            if (!processedBranches.Contains(branchKey))
                            {
                                branches.Enqueue(newBranch);
                                processedBranches.Add(branchKey);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Branch processing complete or iteration limit reached.");
            WriteResults(variableTable, bestSolution, bestObjectiveValue);
        }

        private static List<(string Variable, double Ratio, int Rank)> CreateVariableTable(LinearProgrammingModel model)
        {
            var variableTable = new List<(string Variable, double Ratio, int Rank)>();

            int numVariables = model.cN.Length;
            for (int j = 0; j < numVariables; j++)
            {
                double ratio = model.cN[j] / (model.A[0, j] == 0 ? 1 : model.A[0, j]); // Avoid division by zero
                variableTable.Add(($"x{j + 1}", ratio, 0));
            }

            // Rank variables based on ratio
            if (model.IsMaximization)
            {
                // Sort in ascending order for maximization (highest ratio first)
                variableTable = variableTable.OrderByDescending(v => v.Ratio).ToList();
            }
            else
            {
                // Sort in descending order for minimization (lowest ratio first)
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

        private static List<(string Variable, int InOut, double Remainder)> SolveBranch(List<(string Variable, double Ratio, int Rank)> variableTable, LinearProgrammingModel model)
        {
            var tableList = new List<(string Variable, int InOut, double Remainder)>();
            double remainder = model.b[0];

            foreach (var variable in variableTable)
            {
                int variableIndex = GetVariableIndex(variable.Variable);
                int inOutValue = 1; // Initially, try to add the variable to the knapsack

                remainder -= model.A[0, variableIndex] * inOutValue;
                if (remainder >= 0)
                {
                    tableList.Add((variable.Variable, inOutValue, remainder));
                }
                else
                {
                    // This is the branching point
                    tableList.Add((variable.Variable, 0, remainder));
                    break;
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
            var branchCandidate = table.FirstOrDefault(v => v.Remainder < 0);
            return branchCandidate.Equals(default((string Variable, int InOut, double Remainder))) ? null : branchCandidate.Variable;
        }

        private static List<List<(string Variable, int InOut, double Remainder)>> CreateBranches(string branchVariable, List<(string Variable, double Ratio, int Rank)> variableTable, LinearProgrammingModel model)
        {
            var newBranches = new List<List<(string Variable, int InOut, double Remainder)>>();

            // Branch 1: Exclude the variable (In/Out = 0)
            var branch1 = variableTable.Where(v => v.Variable != branchVariable).ToList();
            branch1.Insert(0, (branchVariable, 0, 0));
            newBranches.Add(SolveBranch(branch1, model));

            // Branch 2: Include the variable (In/Out = 1)
            var branch2 = variableTable.Where(v => v.Variable != branchVariable).ToList();
            branch2.Insert(0, (branchVariable, 1, 0));
            newBranches.Add(SolveBranch(branch2, model));

            return newBranches;
        }

        private static void WriteResults(
            List<(string Variable, double Ratio, int Rank)> variableTable, 
            List<(string Variable, int InOut, double Remainder)> bestSolution, 
            double bestObjectiveValue)
        {
            var output = new StringBuilder();
            
            // Variable Table
            output.AppendLine("Variable Table:");
            foreach (var entry in variableTable)
            {
                output.AppendLine($"{entry.Variable,10} {entry.Ratio,10:F3} {entry.Rank,10}");
            }

            // Iterations
            output.AppendLine("\nIterations:");
            foreach (var iteration in iterationResults)
            {
                output.AppendLine("Iteration:");
                foreach (var entry in iteration)
                {
                    output.AppendLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
                }
            }

            // Optimal Solution
            output.AppendLine("\nOptimal Solution:");
            if (bestSolution != null)
            {
                output.AppendLine("Variable Table for Optimal Solution:");
                foreach (var entry in bestSolution)
                {
                    output.AppendLine($"{entry.Variable,10} {entry.InOut,10} {entry.Remainder,10:F3}");
                }
            }
            else
            {
                output.AppendLine("No optimal solution found.");
            }

            // Best Objective Value
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
