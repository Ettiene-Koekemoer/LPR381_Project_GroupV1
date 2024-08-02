﻿using System;
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

        public LinearProgrammingModel()
        {
            ObjectiveCoefficients = new List<double>();
            Constraints = new List<List<double>>();
            ConstraintOperators = new List<string>();
            RightHandSides = new List<double>();
            SignRestrictions = new List<string>();
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
