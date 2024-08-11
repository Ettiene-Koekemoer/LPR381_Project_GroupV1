using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class PrimalSimplex
    {
        public static void Solve(LinearProgrammingModel model)
        {
            StringBuilder outputString = new StringBuilder();
            Console.WriteLine("Solving using Primal Simplex Algorithm...");

            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);
            outputString.Append(BuildTable(tableau));
            // Perform relaxed simplex iterations
            if (model.ConstraintOperators.Contains(">=") || model.ConstraintOperators.Contains("="))
            {
                Console.WriteLine("Primal Simplex not possible");
            }
            else
            {
                while (true)
                {
                    int pivotColumn = SelectPivotColumn(tableau, model);
                    if (pivotColumn == -1)
                        break; // Optimal solution found

                    int pivotRow = SelectPivotRow(tableau, pivotColumn);
                    if (pivotRow == -1)
                    {
                        Console.WriteLine("Unbounded solution.");
                        return;
                    }

                    Pivot(tableau, pivotRow, pivotColumn);

                    DisplayTableau(tableau);
                    outputString.Append(BuildTable(tableau));
                }
                outputString.Append($"Optimal Solution: {tableau[0, tableau.GetLength(1) - 1]}");
                WriteOutput(outputString.ToString());
                SaveSolution(tableau, model);
            }                        
        }

        private static void DisplayTableau(double[,] tableau)
        {
            for (int i = 0; i < tableau.GetLength(0); i++)
            {
                for (int j = 0; j < tableau.GetLength(1); j++)
                {
                    Console.Write($"{tableau[i, j],10:F3}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void WriteOutput(string output)
        {
            string outputFilePath = "Output.txt";
            using (var writer = new System.IO.StreamWriter(outputFilePath))
            {
                writer.WriteLine("Canonical Form and Simplex Iterations:");
                writer.WriteLine(output);
            }
            Console.WriteLine($"Results written to {outputFilePath}");
        }
        private static double[,] ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int rows = model.Constraints.Count + 1;
            int columns = model.ObjectiveCoefficients.Count + model.Constraints.Count + 1;
            double[,] tableau = new double[rows, columns];

            // Objective function
            for (int j = 0; j < model.ObjectiveCoefficients.Count; j++)
                tableau[0, j] = -model.ObjectiveCoefficients[j];

            // Constraints
            for (int i = 0; i < model.Constraints.Count; i++)
            {
                for (int j = 0; j < model.Constraints[i].Count; j++)
                    if (model.ConstraintOperators[i] == "<=")
                    {
                        tableau[i + 1, j] = model.Constraints[i][j];
                        tableau[i + 1, model.ObjectiveCoefficients.Count + i] = 1; // Slack variable
                        tableau[i + 1, columns - 1] = model.RightHandSides[i];
                    }
                    else
                    {
                        tableau[i + 1, j] = -model.Constraints[i][j];
                        tableau[i + 1, model.ObjectiveCoefficients.Count + i] = 1; // Excess variable
                        tableau[i + 1, columns - 1] = -model.RightHandSides[i];
                    }
            }

            return tableau;
        }

        private static int SelectPivotColumn(double[,] tableau, LinearProgrammingModel model)
        {
            int pivotColumn = -1;
            if (model.IsMaximization)
            {
                double minValue = 0;
                for (int j = 0; j < tableau.GetLength(1) - 1; j++)
                {
                    if (tableau[0, j] < minValue)
                    {
                        minValue = tableau[0, j];
                        pivotColumn = j;
                    }
                }
            }
            else
            {
                double maxValue = 0;
                for (int j = 0; j < tableau.GetLength(1) - 1; j++)
                {
                    if (tableau[0, j] > maxValue)
                    {
                        maxValue = tableau[0, j];
                        pivotColumn = j;
                    }
                }
            }
            return pivotColumn;
        }

        private static int SelectPivotRow(double[,] tableau, int pivotColumn)
        {
            int pivotRow = -1;
            double minRatio = double.PositiveInfinity;
            for (int i = 1; i < tableau.GetLength(0); i++)
            {
                if (tableau[i, pivotColumn] > 0)
                {
                    double ratio = tableau[i, tableau.GetLength(1) - 1] / tableau[i, pivotColumn];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }
            return pivotRow;
        }

        private static void Pivot(double[,] tableau, int pivotRow, int pivotColumn)
        {
            double pivotValue = tableau[pivotRow, pivotColumn];
            for (int j = 0; j < tableau.GetLength(1); j++)
                tableau[pivotRow, j] /= pivotValue;

            for (int i = 0; i < tableau.GetLength(0); i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotColumn];
                    for (int j = 0; j < tableau.GetLength(1); j++)
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                }
            }
        }

        static string BuildTable(double[,] table)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("");

            int rows = table.GetLength(0);
            int cols = table.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tableBuilder.Append($"{table[i, j],10:F3}");
                }
                tableBuilder.AppendLine();
            }
            tableBuilder.AppendLine();
            return tableBuilder.ToString();
        }

        private static void SaveSolution(double[,] optimalTableau, LinearProgrammingModel model)
        {
            int rows = optimalTableau.GetLength(0);
            int columns = optimalTableau.GetLength(1);

            model.SolutionRows = rows;
            model.SolutionColumns = columns;
            model.Solution = optimalTableau;
        }
    }
}
