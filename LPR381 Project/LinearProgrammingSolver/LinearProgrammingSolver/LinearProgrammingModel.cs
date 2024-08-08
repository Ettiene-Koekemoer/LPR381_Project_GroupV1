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
            var lines = File.ReadAllLines(filePath);

            // Read the objective function
            var objectiveLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var problemType = objectiveLine[0]; // "max" or "min"
            var objectiveCoefficients = objectiveLine.Skip(1).Select(double.Parse).ToArray();
            
            bool isMaximization = problemType == "max";

            // Read the constraints
            var constraints = new List<double[]>();
            var rhs = new List<double>();

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                // If the line starts with "x", it indicates the end of constraints
                if (parts.Length > 0 && parts[0].StartsWith("x"))
                {
                    // Last line with variable types
                    var variables = parts.Take(parts.Length - 1).ToList();
                    var variableType = parts.Last(); // Should be "bin"
                    
                    // Ensure binary constraints
                    if (variableType != "bin")
                    {
                        throw new ArgumentException("Variable type must be binary for knapsack problems.");
                    }
                    
                    // Create the variable list (not used directly here, but may be used later)
                    var variableNames = variables;
                }
                else
                {
                    // Parse constraints
                    var constraint = parts.Take(parts.Length - 2).Select(double.Parse).ToArray();
                    var sign = parts[parts.Length - 2];
                    var rhsValue = double.Parse(parts.Last());

                    constraints.Add(constraint);
                    rhs.Add(rhsValue);
                }
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
            var N = new List<string>(); // This should be populated with variable names

            // Create the LinearProgrammingModel
            return new LinearProgrammingModel(
                B: B, // List<int> for indices
                N: N, // List<string> for names (variables)
                A: A,
                cB: objectiveCoefficients, // Assuming cB is the same as the objective coefficients
                cN: objectiveCoefficients, // Assuming cN is the same as the objective coefficients
                b: b
            );
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
