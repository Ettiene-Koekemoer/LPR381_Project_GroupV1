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
            Solution = new double[SolutionRows,SolutionColumns];
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
