using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundKnapsack
    {
        private static double[,,] tableau;
        private static int branchCount;

        public static void Solve(LinearProgrammingModel model)
        {
            int initialBranches = 1; // Start with one branch
            int rows = model.A.GetLength(0) + 1;
            int columns = model.A.GetLength(1) + model.B.Count + 1;
            tableau = new double[initialBranches, rows, columns];
            branchCount = initialBranches;

            // Convert the model to the canonical form
            ConvertToCanonicalForm(model, 0);

            // Create a table for variables, ratios, and ranks
            var variableTable = CreateVariableTable(0, model);

            // Initialize the branches
            var branches = new Queue<int>();
            branches.Enqueue(0); // Start with branch 0

            // Initialize best solution variables
            double bestObjectiveValue = double.NegativeInfinity;
            List<(string Variable, int InOut, double Remainder)> bestSolution = null;

            // Process branches
            while (branches.Count > 0)
            {
                int currentBranch = branches.Dequeue();

                // Convert the branch's tableau to a list format for processing
                var currentTable = ConvertTableToList(currentBranch, model);

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
                        var newBranches = CreateBranches(currentBranch, branchVariable, model);
                        foreach (var newBranch in newBranches)
                        {
                            branches.Enqueue(newBranch);
                        }
                    }
                }
                
            }

            // Output results
            WriteResults(variableTable, bestSolution, bestObjectiveValue);
        }

        private static void ConvertToCanonicalForm(LinearProgrammingModel model, int branchIndex)
        {
            int rows = model.A.GetLength(0) + 1;
            int columns = model.A.GetLength(1) + model.B.Count + 1;

            // Objective function
            for (int j = 0; j < model.cN.Length; j++)
                tableau[branchIndex, 0, j] = model.cN[j];

            // Constraints
            for (int i = 0; i < model.A.GetLength(0); i++)
            {
                for (int j = 0; j < model.A.GetLength(1); j++)
                {
                    tableau[branchIndex, i + 1, j] = model.A[i, j];
                }
                tableau[branchIndex, i + 1, model.A.GetLength(1) + i] = 1; // Slack variable
                tableau[branchIndex, i + 1, columns - 1] = model.b[i];
            }
        }

        private static List<(string Variable, double Ratio, int Rank)> CreateVariableTable(int branchIndex, LinearProgrammingModel model)
        {
            var variableTable = new List<(string Variable, double Ratio, int Rank)>();

            // Calculate ratios and ranks for the first branch
            int numVariables = model.cN.Length;
            for (int j = 0; j < numVariables; j++)
            {
                double ratio = tableau[branchIndex, 0, j] / tableau[branchIndex, 1, j];
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

        private static List<(string Variable, int InOut, double Remainder)> ConvertTableToList(int branchIndex, LinearProgrammingModel model)
        {
            int rows = model.A.GetLength(0);
            var tableList = new List<(string Variable, int InOut, double Remainder)>();

            for (int i = 0; i < rows; i++)
            {
                var variable = $"x{i + 1}";
                tableList.Add((variable, 0, tableau[branchIndex, i + 1, tableau.GetLength(2) - 1]));
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
            return table.FirstOrDefault(v => v.Remainder <= 0).Variable;
        }

        private static List<int> CreateBranches(int currentBranch, string branchVariable, LinearProgrammingModel model)
        {
            var newBranches = new List<int>();

            // Create new branches with updated tableau
            int rows = model.A.GetLength(0) + 1;
            int columns = model.A.GetLength(1) + model.B.Count + 1;

            // Increase the number of branches
            int newBranchIndex1 = branchCount;
            int newBranchIndex2 = branchCount + 1;

            // Create a new larger tableau array
            double[,,] newTableau = new double[branchCount + 2, rows, columns];
            Array.Copy(tableau, newTableau, branchCount * rows * columns);
            tableau = newTableau;

            // Initialize new branches
            Array.Copy(tableau[currentBranch], tableau[newBranchIndex1], rows * columns);

            // Update tableau for the new branches
            UpdateTableForBranch(newBranchIndex1, branchVariable, 0, model);
            UpdateTableForBranch(newBranchIndex2, branchVariable, 1, model);

            newBranches.Add(newBranchIndex1);
            newBranches.Add(newBranchIndex2);

            branchCount += 2;

            return newBranches;
        }

        private static void UpdateTableForBranch(int branchIndex, string branchVariable, int inOutValue, LinearProgrammingModel model)
        {
            int variableIndex = GetVariableIndex(branchVariable);

            for (int i = 1; i < tableau.GetLength(1); i++) // Start from row 1 (excluding the objective function)
            {
                tableau[branchIndex, i, tableau.GetLength(2) - 1] -= (inOutValue * model.A[i - 1, variableIndex]);
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

        private static int GetVariableIndex(string variableName)
        {
            return int.Parse(variableName.Substring(1)) - 1;
        }
    }
}
