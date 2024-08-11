using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinearProgrammingSolver
{
    public class LinearProgrammingModel
    {
        // Global variable to store the optimal model
        public static LinearProgrammingModel OptimalModel { get; set; }

        public bool IsMaximization { get; set; }
        public List<double> ObjectiveCoefficients { get; set; }
        public List<List<double>> Constraints { get; set; }
        public List<string> ConstraintOperators { get; set; }
        public List<double> RightHandSides { get; set; }
        public List<string> SignRestrictions { get; set; }

        public List<int> B { get; }
        public List<string> N { get; }
        public double[,] A { get; }
        public double[] cB { get; }
        public double[] cN { get; }
        public double[] b { get; }
        public int SolutionRows { get; set; }
        public int SolutionColumns { get; set; }
        public double[,] Solution { get; set; }

        public LinearProgrammingModel()
        {
            ObjectiveCoefficients = new List<double>();
            Constraints = new List<List<double>>();
            ConstraintOperators = new List<string>();
            RightHandSides = new List<double>();
            SignRestrictions = new List<string>();
            SolutionRows = 0;
            SolutionColumns = 0;
            Solution = new double[SolutionRows, SolutionColumns];
        }

        public LinearProgrammingModel(List<int> B, List<string> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            this.B = B;
            this.N = N;
            this.A = A;
            this.cB = cB;
            this.cN = cN;
            this.b = b;
        }

        // Constructor for the knapsack problem
        public LinearProgrammingModel(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            ObjectiveCoefficients = new List<double>(cN);
            Constraints = new List<List<double>>();
            for (int i = 0; i < A.GetLength(0); i++)
            {
                var row = new List<double>();
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    row.Add(A[i, j]);
                }
                Constraints.Add(row);
            }
            ConstraintOperators = new List<string>(); 
            RightHandSides = new List<double>(b);
            SignRestrictions = new List<string>(); 
        }

        // Static method to read a knapsack problem from a file
        public static LinearProgrammingModel ReadKnapsackInput(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    throw new InvalidOperationException("File is empty.");
                }

                // Read the objective function
                var objectiveLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (objectiveLine.Length < 2)
                {
                    throw new InvalidOperationException("Invalid objective function line.");
                }

                bool isMaximization = objectiveLine[0].ToLower() == "max";
                var objectiveCoefficients = objectiveLine.Skip(1).Select(double.Parse).ToArray();

                // Read the constraints
                var constraints = new List<double[]>();
                var rhs = new List<double>();

                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Handle variable type line
                    if (parts.Length > 1 && parts.Last() == "bin")
                    {
                        // Extract variable names
                        var variables = parts.Take(parts.Length - 1).ToList();
                        // Optionally, store the variables if needed
                        continue;
                    }

                    // Parse constraints
                    if (parts.Length < 2)
                    {
                        throw new InvalidOperationException("Invalid constraint line.");
                    }

                    var rhsIndex = parts.Length - 2;
                    var rhsValue = double.Parse(parts.Last());
                    var constraint = parts.Take(rhsIndex).Select(double.Parse).ToArray();
                    constraints.Add(constraint);
                    rhs.Add(rhsValue);
                }

                // Create the A matrix and b vector
                int numConstraints = constraints.Count;
                int numVariables = objectiveCoefficients.Length;
                var A = new double[numConstraints, numVariables];
                var b = rhs.ToArray();

                for (int i = 0; i < numConstraints; i++)
                {
                    for (int j = 0; j < numVariables; j++)
                    {
                        A[i, j] = constraints[i][j];
                    }
                }

                // Create List<int> for indices
                var B = Enumerable.Range(0, numConstraints).ToList(); // Placeholder for indices of basic variables

                // Create List<string> for N (variable names)
                var N = new List<string>(); // This should be populated with variable names if needed

                return new LinearProgrammingModel(
                    B: B, // List<int> for indices
                    N: N, // List<string> for names (variables)
                    A: A,
                    cB: objectiveCoefficients, // Assuming cB is the same as the objective coefficients
                    cN: objectiveCoefficients, // Assuming cN is the same as the objective coefficients
                    b: b
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }

        public void ParseInputFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            ParseObjectiveFunction(lines[0]);
            ParseConstraints(lines.Skip(1).Take(lines.Length - 2).ToArray());
            ParseSignRestrictions(lines.Last());
        }

        private void ParseObjectiveFunction(string line)
        {
            var parts = line.Split(' ');
            IsMaximization = parts[0].ToLower() == "max";

            for (int i = 1; i < parts.Length; i++)
            {
                ObjectiveCoefficients.Add(double.Parse(parts[i]));
            }
        }

        private void ParseConstraints(string[] lines)
        {
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                var coefficients = new List<double>();

                for (int i = 0; i < parts.Length - 2; i++)
                {
                    coefficients.Add(double.Parse(parts[i]));
                }

                Constraints.Add(coefficients);
                ConstraintOperators.Add(parts[parts.Length - 2]);
                RightHandSides.Add(double.Parse(parts.Last()));
            }
        }

        private void ParseSignRestrictions(string line)
        {
            var parts = line.Split(' ');

            for (int i = 0; i < parts.Length; i += 2)
            {
                SignRestrictions.Add(parts[i]);
            }
        }
    }
}
