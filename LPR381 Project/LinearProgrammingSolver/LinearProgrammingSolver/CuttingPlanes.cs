﻿using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class CuttingPlane
    {
        public static void Solve(LinearProgrammingModel model)
        {
            StringBuilder outputString = new StringBuilder();
            Console.WriteLine("Solving using Cutting Plane Algorithm...");

            // Initial LP relaxation
            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);
            outputString.Append(BuildTable(tableau));
            // Perform relaxed simplex iterations
            if (model.ConstraintOperators.Contains(">=") || model.ConstraintOperators.Contains("="))
            {
                while (true)
                {
                    int pivotDualRow = SelectDualPivotRow(tableau);
                    if (pivotDualRow == -1)
                        break; // Optimal solution found
                    int pivotDualColumn = SelectDualPivotColumn(tableau, pivotDualRow);
                    if (pivotDualColumn == -1)
                    {
                        Console.WriteLine("Infeasible solution.");
                        return;
                    }

                    Pivot(tableau, pivotDualRow, pivotDualColumn);
                }
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
                }
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
                }
            }

            DisplayTableau(tableau);
            outputString.Append(BuildTable(tableau));
            
            //Cutting Plane Algorithm
            bool solvedProblem = false;
            while (solvedProblem == false) //loop until a solution is found
            {
                bool noAddedConstraint = true;
                while (noAddedConstraint == true) //loop until a constraint isn't added (stop cutting)
                {
                    for (int i = 1; i < tableau.GetLength(0); i++) //checking each constraint RHS value
                    {
                        if (HasDecimal(tableau[i, tableau.GetLength(1) - 1])) //checking whether a RHS value has a decimal part
                        {
                            tableau = AddZeroColumn(tableau); //adds column for new slack variable
                            double[] newConstraint = new double[tableau.GetLength(1)];
                            for (int j = 0; j < tableau.GetLength(1); j++) //creating new cut constraint
                            {
                                if (tableau[i, j] < 0)
                                {
                                    newConstraint[j] = Math.Round(-1 - tableau[i, j] % 1,3); //for negative values
                                }
                                else
                                {
                                    newConstraint[j] = Math.Round(-(tableau[i, j] % 1),3); //for positive values
                                }
                            }
                            tableau = AddConstraint(tableau, newConstraint);
                            tableau[tableau.GetLength(0) - 1, tableau.GetLength(1) - 2] = 1; //makes added constraint's slack a BV
                            noAddedConstraint = false;
                            DisplayTableau(tableau);
                            outputString.Append(BuildTable(tableau));
                            break; //break ensures cutting occurs only once when a cut is needed 
                        }
                    }
                    if (noAddedConstraint == true) //indicates cutting has stopped
                    {
                        int pivotColumn = SelectPivotColumn(tableau, model);
                        if (pivotColumn == -1) //ensure if primal simplex should follow
                        {
                            Console.WriteLine("Cutting plane algorithm completed");
                            solvedProblem = true;
                            break;
                        }
                        else
                        {
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
                    }
                }
                while (true) //perform dual simplex after cutting and added constraint
                {
                    int pivotDualRow = SelectDualPivotRow(tableau);
                    if (pivotDualRow == -1)
                        break; // Optimal solution found
                    int pivotDualColumn = SelectDualPivotColumn(tableau, pivotDualRow);
                    if (pivotDualColumn == -1)
                    {
                        Console.WriteLine("Infeasible solution.");
                        solvedProblem = true;
                        return;
                    }

                    Pivot(tableau, pivotDualRow, pivotDualColumn);
                    DisplayTableau(tableau);                    
                    outputString.Append(BuildTable(tableau));
                }
            }            
            outputString.Append($"Optimal Solution: {tableau[0, tableau.GetLength(1) - 1]}");
            WriteOutput(outputString.ToString());
            SaveSolution(tableau, model);
        }
        
        private static bool HasDecimal(double value)
        {
            return value % 1 != 0;
        }

        static double[,] AddZeroColumn(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] newMatrix = new double[rows, cols + 1];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols + 1; j++)
                {
                    if (j == cols - 1)
                    {
                        newMatrix[i, j] = 0;
                    }
                    else if (j < cols - 1)
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix[i, j] = matrix[i, j - 1];
                    }
                }
            }

            return newMatrix;
        }
        static double[,] AddConstraint(double[,] originalArray, double[] newRow)
        {
            int originalRows = originalArray.GetLength(0);
            int originalCols = originalArray.GetLength(1);

            // Create a new array with an additional row
            double[,] newArray = new double[originalRows + 1, originalCols];

            // Copy the original array values to the new array
            for (int i = 0; i < originalRows; i++)
            {
                for (int j = 0; j < originalCols; j++)
                {
                    newArray[i, j] = originalArray[i, j];
                }
            }

            // Add the new row to the new array
            for (int j = 0; j < originalCols; j++)
            {
                newArray[originalRows, j] = newRow[j];
            }

            return newArray;
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

        private static int SelectDualPivotRow(double[,] tableau)
        {
            int pivotRow = -1;
            double minValue = 0;
            for (int j = 1; j < tableau.GetLength(0); j++)
            {
                if (tableau[j, tableau.GetLength(1)-1] < minValue)
                {
                    minValue = tableau[j, tableau.GetLength(1) - 1];
                    pivotRow = j;
                }
            }
            return pivotRow;
        }

        private static int SelectDualPivotColumn(double[,] tableau, int pivotRow)
        {
            int pivotColumn = -1;
            double minRatio = double.PositiveInfinity;
            for (int i = 0; i < tableau.GetLength(1)-1; i++)
            {
                if (tableau[pivotRow, i] < 0)
                {
                    double ratio = Math.Abs(tableau[0, i] / tableau[pivotRow, i]);
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotColumn = i;
                    }
                }
            }
            return pivotColumn;
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
