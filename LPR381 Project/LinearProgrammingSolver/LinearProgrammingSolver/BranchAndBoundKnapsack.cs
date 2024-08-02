using System;
using System.Collections.Generic;

namespace LinearProgrammingSolver
{
    public static class BranchAndBoundKnapsack
    {
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
            var nodes = new List<Node> { new Node(B, N, A, cB, cN, b) };
            double bestValue = double.NegativeInfinity;
            Node bestNode = null;

            while (nodes.Count > 0)
            {
                var currentNode = nodes[0];
                nodes.RemoveAt(0);

                var currentResult = RevisedSimplex.Solve(new LinearProgrammingModel(currentNode.B, currentNode.N, currentNode.A, currentNode.cB, currentNode.cN, currentNode.b)); // Pass current node's data to solve
                if (currentResult == null)
                    continue;

                var (curB, curN, curA, curCB, curCN, curBVal) = currentResult.Value;

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
                    nodes.Add(leftNode);
                    nodes.Add(rightNode);
                }
            }

            WriteOutput(bestNode, bestValue);
        }

        private static bool IsIntegerSolution(double[] solution)
        {
            foreach (var value in solution)
            {
                if (Math.Abs(value - Math.Round(value)) > 1e-6)
                    return false;
            }
            return true;
        }

        private static (Node leftNode, Node rightNode) Branch(Node node)
        {
            // Implement the branching logic to create left and right nodes
            int branchIndex = -1;
            double fractionalValue = 0;

            for (int i = 0; i < node.b.Length; i++)
            {
                double value = node.b[i];
                if (Math.Abs(value - Math.Round(value)) > 1e-6)
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

        private static void DisplayTableau(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            if (N.Count != cN.Length || B.Count != cB.Length)
            {
                throw new ArgumentException("Dimension mismatch between lists and arrays.");
            }

            Console.WriteLine("Objective Function:");
            Console.Write("Max Z\t");
            for (int i = 0; i < cN.Length; i++)
            {
                Console.Write($"{cN[i]}X{N[i]}\t");
            }
            Console.WriteLine();

            Console.WriteLine("Constraints:");
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    Console.Write($"{A[i, j]}X{N[j]}\t");
                }
                Console.WriteLine($" <= {b[i]}");
            }

            Console.WriteLine();

            Console.WriteLine("Tableau:");
            Console.Write("Item\tIn/Out\tRemainder\t");
            for (int i = 0; i < N.Count; i++)
            {
                Console.Write($"X{N[i]}\t");
            }
            Console.WriteLine();

            for (int i = 0; i < N.Count; i++)
            {
                if (i >= B.Count || i >= cB.Length)
                {
                    Console.WriteLine("Error: Index out of range while displaying tableau.");
                    return;
                }

                Console.Write($"X{N[i]}\t");
                Console.Write($"{(B.Contains(N[i]) ? "1" : "0")}\t");
                Console.Write($"{b[i]}\t");
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    if (i >= A.GetLength(0) || j >= A.GetLength(1))
                    {
                        Console.WriteLine("Error: Index out of range while displaying tableau.");
                        return;
                    }
                    Console.Write($"{A[i, j]}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Branching Information:");
            Console.WriteLine($"Branch on {N[0]}");
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
            string outputFilePath = "output_knapsack.txt";
            using (var writer = new System.IO.StreamWriter(outputFilePath))
            {
                // Write the best node's tableau and objective value
                writer.WriteLine($"Best Objective Value: {bestValue}");
                writer.WriteLine("Best Node Tableau:");
                DisplayTableau(bestNode.B, bestNode.N, bestNode.A, bestNode.cB, bestNode.cN, bestNode.b);
            }
            Console.WriteLine($"Results written to {outputFilePath}");
        }

        private class Node
        {
            public List<int> B { get; }
            public List<int> N { get; }
            public double[,] A { get; set; }
            public double[] cB { get; set; }
            public double[] cN { get; set; }
            public double[] b { get; set; }

            public Node(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
            {
                this.B = new List<int>(B);
                this.N = new List<int>(N);
                this.A = A.Clone() as double[,];
                this.cB = (double[])cB.Clone();
                this.cN = (double[])cN.Clone();
                this.b = (double[])b.Clone();
            }
        }
    }
}
