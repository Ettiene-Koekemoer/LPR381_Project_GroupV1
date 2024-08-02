using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinearProgrammingSolver
{
    public class LinearProgrammingModel
    {
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

        public LinearProgrammingModel()
        {
            ObjectiveCoefficients = new List<double>();
            Constraints = new List<List<double>>();
            ConstraintOperators = new List<string>();
            RightHandSides = new List<double>();
            SignRestrictions = new List<string>();
            
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


        // ->LO 2024/08/05 Added new LPRModel
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
        // ->LO 2024/08/05 Added new LPRModel
        public static LinearProgrammingModel ReadKnapsackInput(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var objectiveLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var coefficients = objectiveLine.Skip(1).Select(double.Parse).ToArray();

            var constraints = new List<double[]>();
            var rhs = new List<double>();

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && parts[0].StartsWith("x")) // End of constraints
                {
                    break;
                }
                else
                {
                    var constraint = parts.Take(parts.Length - 2).Select(double.Parse).ToArray();
                    constraints.Add(constraint);
                    rhs.Add(double.Parse(parts.Last()));
                }
            }

            var lastLine = lines.Last().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var variables = lastLine.Take(lastLine.Length - 1).ToList();
            var variableTypes = lastLine.Last(); // Should be "bin"

            // Ensure binary constraints
            if (variableTypes != "bin")
            {
                throw new ArgumentException("Variable type must be binary for knapsack problems.");
            }

            var A = new double[constraints.Count, coefficients.Length];
            var b = rhs.ToArray();

            for (int i = 0; i < constraints.Count; i++)
            {
                for (int j = 0; j < constraints[i].Length; j++)
                {
                    A[i, j] = constraints[i][j];
                }
            }

            // Create List<int> for indices
            var variableIndices = Enumerable.Range(0, coefficients.Length).ToList();

            // Create List<int> for B if required by the model
            var B = variableIndices; // If B needs to be indices

            // Create List<string> for N (variable names)
            var N = variables; // Use List<string> for variable names

            // Assuming that the second constructor is appropriate
            return new LinearProgrammingModel(
                B: B, // List<int> for indices
                N: variables, // List<string> for names
                A: A,
                cB: coefficients,
                cN: coefficients,
                b: b
            );
        }
        //<-

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
