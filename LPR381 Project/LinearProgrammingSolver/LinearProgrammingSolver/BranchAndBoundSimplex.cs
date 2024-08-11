using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundSimplex
    {
       public static void RunBranchAndBoundAlgorithm()
        {
            Console.WriteLine("Solving using Branch and Bound Simplex Algorithm...");
            // Read input file
            string[] lines = File.ReadAllLines("C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/LPR381 Project/LinearProgrammingSolver/LinearProgrammingSolver/Models/branch&bound.txt");

            // Parse the input file
            string objectiveFunction = lines[0].Trim();
            string[] constraints = new string[lines.Length - 2];
            for (int i = 1; i < lines.Length - 1; i++)
            {
                constraints[i - 1] = lines[i].Trim();
            }
            string[] variableTypes = lines[lines.Length - 1].Trim().Split(' ');

            // Process the linear programming problem
            List<string> output = new List<string>();
            output.Add("Objective Function:");
            output.Add(objectiveFunction);
            output.Add("Constraints:");
            foreach (var constraint in constraints)
            {
                output.Add(constraint);
            }
            output.Add("Variable Types:");
            foreach (var varType in variableTypes)
            {
                output.Add(varType);
            }

            // Branch and Bound Algorithm (simplified example)
            output.Add("\nBranch and Bound Iterations:");

            PerformBranchAndBound(objectiveFunction, constraints, variableTypes, output);

            // Write output to file
            File.WriteAllLines("C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/Output.txt", output);
            Console.WriteLine("Branch and Bound iterations have been written to output.txt");
        }

        static void PerformBranchAndBound(string objectiveFunction, string[] constraints, string[] variableTypes, List<string> output)
        {
            // Initialize the problem (simplified example)
            int[] bestSolution = null;
            int bestValue = int.MinValue;

            // Initial node
            Queue<Node> nodes = new Queue<Node>();
            nodes.Enqueue(new Node { Level = 0, Solution = new int[variableTypes.Length], Value = 0, Path = "0" });

            Dictionary<int, List<Node>> levelNodes = new Dictionary<int, List<Node>>();

            while (nodes.Count > 0)
            {
                Node currentNode = nodes.Dequeue();
                bool isFeasible = CheckFeasibility(currentNode.Solution, constraints);

                if (!levelNodes.ContainsKey(currentNode.Level))
                {
                    levelNodes[currentNode.Level] = new List<Node>();
                }

                levelNodes[currentNode.Level].Add(currentNode);

                if (currentNode.Level == variableTypes.Length)
                {
                    // All variables assigned, check solution
                    if (isFeasible)
                    {
                        int currentValue = EvaluateSolution(currentNode.Solution, objectiveFunction);
                        if (currentValue > bestValue)
                        {
                            bestValue = currentValue;
                            bestSolution = currentNode.Solution;
                        }
                    }
                }
                else
                {
                    // Branch on the next variable
                    int level = currentNode.Level;
                    int[] solution0 = (int[])currentNode.Solution.Clone();
                    int[] solution1 = (int[])currentNode.Solution.Clone();
                    solution0[level] = 0;
                    solution1[level] = 1;

                    nodes.Enqueue(new Node { Level = level + 1, Solution = solution0, Value = EvaluateSolution(solution0, objectiveFunction), Path = $"{currentNode.Path}.1" });
                    nodes.Enqueue(new Node { Level = level + 1, Solution = solution1, Value = EvaluateSolution(solution1, objectiveFunction), Path = $"{currentNode.Path}.2" });
                }
            }

            foreach (var level in levelNodes.Keys)
            {
                output.Add($"\nLevel {level}:");
                var nodeList = levelNodes[level];
                var subProblemDescriptions = new List<string>();

                foreach (var node in nodeList)
                {
                    string subProblem = CreateSubProblemDescription(node.Solution, node.Value, node.Path);
                    subProblemDescriptions.Add(subProblem);
                }

                // Merge descriptions horizontally with proper spacing
                MergeAndAddDescriptions(output, subProblemDescriptions);
            }

            output.Add($"\nBest Solution: {string.Join(" ", bestSolution)}, Value: {bestValue}");
        }

        static bool CheckFeasibility(int[] solution, string[] constraints)
        {
            foreach (var constraint in constraints)
            {
                string[] parts = constraint.Split(' ');
                int sum = 0;
                int index = 0;
                for (int i = 0; i < parts.Length; i++)
                {
                    if (int.TryParse(parts[i], out int coefficient))
                    {
                        sum += coefficient * solution[index++];
                    }
                    else if (parts[i] == "<=")
                    {
                        int rhs = int.Parse(parts[i + 1]);
                        if (sum > rhs) return false;
                        break;
                    }
                }
            }
            return true;
        }

        static int EvaluateSolution(int[] solution, string objectiveFunction)
        {
            // Simplified evaluation based on the objective function
            int value = 0;
            string[] parts = objectiveFunction.Split(' ');
            for (int i = 1; i < parts.Length; i++)
            {
                int coefficient = int.Parse(parts[i]);
                value += coefficient * solution[i - 1];
            }
            return value;
        }

        static string CreateSubProblemDescription(int[] solution, int value, string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Subproblem {(path.Length > 2 ? path.Substring(2) : path)}\n");
            sb.Append("| x1 | x2 | x3 | x4 | x5 | x6 |  z  |\n");
            sb.Append("| " + string.Join(" | ", solution) + $" | {value} |\n");
            return sb.ToString();
        }

        static void MergeAndAddDescriptions(List<string> output, List<string> descriptions)
        {
            var lines = new List<string[]>();
            int maxLines = 0;

            // Calculate the maximum width for each column
            int[] columnWidths = new int[descriptions.Count];
            for (int i = 0; i < descriptions.Count; i++)
            {
                var splitDesc = descriptions[i].Split('\n');
                lines.Add(splitDesc);
                for (int j = 0; j < splitDesc.Length; j++)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], splitDesc[j].Length);
                }
                maxLines = Math.Max(maxLines, splitDesc.Length);
            }

            // Adjust output with proper spacing
            for (int i = 0; i < maxLines; i++)
            {
                var lineParts = new List<string>();
                for (int j = 0; j < lines.Count; j++)
                {
                    if (i < lines[j].Length)
                    {
                        lineParts.Add(lines[j][i].PadRight(columnWidths[j] + 4)); // Adding 4 spaces for padding between columns
                    }
                    else
                    {
                        lineParts.Add(new string(' ', columnWidths[j] + 4));
                    }
                }
                output.Add(string.Join("", lineParts));
            }
        }
    }

    class Node
    {
        public int Level { get; set; }
        public int[] Solution { get; set; }
        public int Value { get; set; }
        public string Path { get; set; }
    }
    
}
