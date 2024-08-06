using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundKnapsack
    {
        private static List<Node> allNodes = new List<Node>();

        public static void Solve(LinearProgrammingModel model)
        {
            Console.WriteLine("Solving using Branch and Bound Knapsack Algorithm...");

            // Initial LP relaxation
            var result = RevisedSimplex.Solve(model);
            if (result == null)
            {
                Console.WriteLine("Infeasible initial relaxation.");
                return;
            }

            var (B, N, A, cB, cN, b) = result.Value;
            var nodes = new Queue<Node>();
            nodes.Enqueue(new Node(B, N, A, cB, cN, b));
            double bestValue = double.NegativeInfinity;
            Node bestNode = null;

            allNodes.Clear(); // Clear all nodes list
            int iterationCount = 0;

            while (nodes.Count > 0)
            {
                var currentNode = nodes.Dequeue();

                var currentResult = RevisedSimplex.Solve(new LinearProgrammingModel(currentNode.B, currentNode.N, currentNode.A, currentNode.cB, currentNode.cN, currentNode.b));
                if (currentResult == null)
                    continue;

                var (curB, curN, curA, curCB, curCN, curBVal) = currentResult.Value;

                // Store the tableau for this iteration
                allNodes.Add(new Node(curB, curN, curA, curCB, curCN, curBVal, iterationCount++));

                if (IsIntegerSolution(curBVal))
                {
                    double currentObjectiveValue = CalculateObjectiveValue(curB, curCB);
                    if (currentObjectiveValue > bestValue)
                    {
                        bestValue = currentObjectiveValue;
                        bestNode = new Node(curB, curN, curA, curCB, curCN, curBVal);
                    }
                }
                else
                {
                    var (leftNode, rightNode) = Branch(new Node(curB, curN, curA, curCB, curCN, curBVal));
                    nodes.Enqueue(leftNode);
                    nodes.Enqueue(rightNode);
                }
            }

            WriteOutput(bestNode, bestValue);
        }


        private static bool IsIntegerSolution(double[] solution)
        {
            foreach (var value in solution)
            {
                if (Math.Abs(value - Math.Round(value)) > 1e-3)
                    return false;
            }
            return true;
        }

        private static (Node leftNode, Node rightNode) Branch(Node node)
        {
            int branchIndex = -1;
            double fractionalValue = 0;

            for (int i = 0; i < node.b.Length; i++)
            {
                double value = node.b[i];
                if (Math.Abs(value - Math.Round(value)) > 1e-3)
                {
                    branchIndex = i;
                    fractionalValue = value;
                    break;
                }
            }

            if (branchIndex == -1)
                throw new Exception("No fractional value found for branching.");

            var leftNode = new Node(node.B, node.N, node.A, node.cB, node.cN, node.b);
            var rightNode = new Node(node.B, node.N, node.A, node.cB, node.cN, node.b);

            // Add constraints to left and right nodes
            AddBranchingConstraint(leftNode, branchIndex, Math.Floor(fractionalValue), "<=");
            AddBranchingConstraint(rightNode, branchIndex, Math.Ceiling(fractionalValue), ">=");

            return (leftNode, rightNode);
        }

        private static void AddBranchingConstraint(Node node, int index, double value, string operation)
        {
            int newConstraintIndex = node.b.Length;
            double[,] newA = new double[node.A.GetLength(0) + 1, node.A.GetLength(1)];
            double[] newb = new double[node.b.Length + 1];

            for (int i = 0; i < node.A.GetLength(0); i++)
            {
                for (int j = 0; j < node.A.GetLength(1); j++)
                {
                    newA[i, j] = node.A[i, j];
                }
            }

            for (int i = 0; i < node.b.Length; i++)
            {
                newb[i] = node.b[i];
            }

            if (operation == "<=")
            {
                newA[newConstraintIndex, index] = 1;
                newb[newConstraintIndex] = value;
            }
            else if (operation == ">=")
            {
                newA[newConstraintIndex, index] = -1;
                newb[newConstraintIndex] = -value;
            }

            node.A = newA;
            node.b = newb;
        }

        private static double CalculateObjectiveValue(List<int> B, double[] cB)
        {
            double value = 0;
            for (int i = 0; i < B.Count; i++)
            {
                value += cB[i];
            }
            return value;
        }

        private static void WriteOutput(Node bestNode, double bestValue)
        {
            string outputFilePath = "./Output.txt";
            try
            {
                using (var writer = new StreamWriter(outputFilePath))
                {
                    writer.WriteLine("Canonical Form:");
                    writer.WriteLine($"Objective Function: {string.Join(", ", bestNode.cB)}");
                    writer.WriteLine("Constraints:");
                    for (int i = 0; i < bestNode.A.GetLength(0); i++)
                    {
                        string row = string.Join(" ", Enumerable.Range(0, bestNode.A.GetLength(1)).Select(j => Math.Round(bestNode.A[i, j], 3)));
                        writer.WriteLine($"{row} <= {Math.Round(bestNode.b[i], 3)}");
                    }

                    writer.WriteLine();
                    writer.WriteLine("Tableau Iterations:");
                    foreach (var node in allNodes)
                    {
                        writer.WriteLine($"Iteration {node.Iteration}:");
                        writer.WriteLine($"B: {string.Join(", ", node.B)}");
                        writer.WriteLine($"N: {string.Join(", ", node.N)}");
                        writer.WriteLine("A:");
                        for (int i = 0; i < node.A.GetLength(0); i++)
                        {
                            for (int j = 0; j < node.A.GetLength(1); j++)
                            {
                                writer.Write($"{Math.Round(node.A[i, j], 3)} ");
                            }
                            writer.WriteLine();
                        }
                        writer.WriteLine($"cB: {string.Join(", ", node.cB.Select(v => Math.Round(v, 3)))}");
                        writer.WriteLine($"cN: {string.Join(", ", node.cN.Select(v => Math.Round(v, 3)))}");
                        writer.WriteLine($"b: {string.Join(", ", node.b.Select(v => Math.Round(v, 3)))}");
                        writer.WriteLine();
                    }

                    writer.WriteLine($"Best Objective Value: {Math.Round(bestValue, 3)}");
                    writer.WriteLine("Best Node Details:");
                    writer.WriteLine($"B: {string.Join(", ", bestNode.B)}");
                    writer.WriteLine($"N: {string.Join(", ", bestNode.N)}");
                    writer.WriteLine("A:");
                    for (int i = 0; i < bestNode.A.GetLength(0); i++)
                    {
                        for (int j = 0; j < bestNode.A.GetLength(1); j++)
                        {
                            writer.Write($"{Math.Round(bestNode.A[i, j], 3)} ");
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine($"cB: {string.Join(", ", bestNode.cB.Select(v => Math.Round(v, 3)))}");
                    writer.WriteLine($"cN: {string.Join(", ", bestNode.cN.Select(v => Math.Round(v, 3)))}");
                    writer.WriteLine($"b: {string.Join(", ", bestNode.b.Select(v => Math.Round(v, 3)))}");
                }
                Console.WriteLine($"Results written to {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }

        private class Node
        {
            public List<int> B { get; }
            public List<int> N { get; }
            public double[,] A { get; set; }
            public double[] cB { get; set; }
            public double[] cN { get; set; }
            public double[] b { get; set; }
            public int Iteration { get; }

            public Node(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b, int iteration = 0)
            {
                this.B = new List<int>(B);
                this.N = new List<int>(N);
                this.A = A.Clone() as double[,];
                this.cB = (double[])cB.Clone();
                this.cN = (double[])cN.Clone();
                this.b = (double[])b.Clone();
                this.Iteration = iteration;
            }
        }
    }
}
