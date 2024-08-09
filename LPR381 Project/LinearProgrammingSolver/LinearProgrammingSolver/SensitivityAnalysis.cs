using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinearProgrammingSolver
{
    public static class SensitivityAnalysis
    {
        public static void Perform(LinearProgrammingModel model)
        {
            double[,] optimalSolution = model.Solution;
            if (optimalSolution != null)
            {
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Select Sensitivity Analysis to perform:");
                    Console.WriteLine("1.  Display the range of a selected Non-Basic Variable");
                    Console.WriteLine("2.  Apply and display a change of a selected Non-Basic Variable");
                    Console.WriteLine("3.  Display the range of a selected Basic Variable");
                    Console.WriteLine("4.  Apply and display a change of a selected Basic Variable");
                    Console.WriteLine("5.  Display the range of a selected constraint right-hand-side value");
                    Console.WriteLine("6.  Apply and display a change of a selected constraint right-hand-side value");
                    Console.WriteLine("7.  Display the range of a selected variable in a Non-Basic Variable column");
                    Console.WriteLine("8.  Apply and display a change of a selected variable in a Non-Basic Variable column");
                    Console.WriteLine("9.  Add a new activity to an optimal solution");
                    Console.WriteLine("10. Add a new constraint to an optimal solution");
                    Console.WriteLine("11. Display the shadow prices");
                    Console.WriteLine("12. Duality");
                    Console.WriteLine("13. Exit");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            DisplayNonBasicVariableRange(optimalSolution, model);
                            Console.ReadLine();
                            break;
                        case "2":
                            ApplyNonBasicVariableChange(optimalSolution, model);
                            Console.ReadLine();
                            break;
                        case "3":
                            DisplayBasicVariableRange(optimalSolution, model);
                            Console.ReadLine();
                            break;
                        case "4":
                            ApplyBasicVariableChange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "5":
                            DisplayConstraintRHSRange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "6":
                            ApplyConstraintRHSChange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "7":
                            DisplayNonBasicVariableColumnRange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "8":
                            ApplyNonBasicVariableColumnChange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "9":
                            AddNewActivity(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "10":
                            AddNewConstraint(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "11":
                            DisplayShadowPrices(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "12":
                            PerformDuality(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "13":
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            Console.ReadLine();
                            break;
                    }
                }            
            }
            else
            {
                Console.WriteLine("An optimal solution was not present or there was an error");
                Console.WriteLine();
            }
        }

        private static void DisplayNonBasicVariableRange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to display the range of a selected Non-Basic Variable
            var example = Init(model, solutionTableau, "cbvb-");
            DisplayTableau(example);
        }

        private static void ApplyNonBasicVariableChange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to apply and display a change of a selected Non-Basic Variable
            var example = Init(model, solutionTableau, "b-");
            DisplayTableau(example);
        }

        private static void DisplayBasicVariableRange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to display the range of a selected Basic Variable
            var example = Init(model, solutionTableau, "b");
            DisplayTableau(example);
        }

        private static void ApplyBasicVariableChange(double[,] solutionTableau)
        {
            // Implement the logic to apply and display a change of a selected Basic Variable
        }

        private static void DisplayConstraintRHSRange(double[,] solutionTableau)
        {
            // Implement the logic to display the range of a selected constraint right-hand-side value
        }

        private static void ApplyConstraintRHSChange(double[,] solutionTableau)
        {
            // Implement the logic to apply and display a change of a selected constraint right-hand-side value
        }

        private static void DisplayNonBasicVariableColumnRange(double[,] solutionTableau)
        {
            // Implement the logic to display the range of a selected constraint right-hand-side value
        }

        private static void ApplyNonBasicVariableColumnChange(double[,] solutionTableau)
        {
            // Implement the logic to apply and display a change of a selected constraint right-hand-side value
        }

        private static void AddNewActivity(double[,] solutionTableau)
        {
            // Implement the logic to add a new activity to an optimal solution
        }

        private static void AddNewConstraint(double[,] solutionTableau)
        {
            // Implement the logic to add a new constraint to an optimal solution
        }

        private static void DisplayShadowPrices(double[,] tableau/*, double[,] solutionTableau*/)
        {
            // Implement the logic to display the shadow prices
        }

        private static void PerformDuality(double[,] tableau)
        {
            // Implement the logic to solve the Dual Programming Model
        }

        private static double[,] Init(LinearProgrammingModel model, double[,] solutionTableau, string table)
        {
            double[,] initOutput;
            List<int> bv = new List<int>();
            var iTable = ConvertToCanonicalForm(model);
            bv = FindBV(solutionTableau);
            double[,] cbv = new double[1, bv.Count];
            double[,] b = new double[iTable.GetLength(0) - 1, bv.Count];
            double[,] rhsb = new double[iTable.GetLength(0) - 1,1];

            for (int k = 0; k < bv.Count; k++)
            {
                int colIndex = bv[k];

                // Store the value at row 0, matched column index
                cbv[0, k] = iTable[0, colIndex];

                // Store the values below row 0 for the matched column index
                for (int i = 1; i < iTable.GetLength(0); i++)
                {
                    b[i - 1, k] = iTable[i, colIndex];
                }
            }
            b = InvertMatrix(b);
            switch (table)
            {
                case "cbvb-":                    
                    initOutput = MultiplyMatrices(cbv,b);
                    return initOutput;
                case "b-":
                    initOutput = b;
                    return initOutput;
                case "b":
                    int rhsIndex = iTable.GetLength(1) - 1;
                    for (int i = 1; i < iTable.GetLength(0); i++)
                    {
                        rhsb[i - 1,0] = iTable[i, rhsIndex];
                    }
                    initOutput = rhsb;
                    return initOutput;
                case "":
                    initOutput = solutionTableau;
                    return initOutput;
                default:
                    initOutput = solutionTableau;
                    return initOutput;
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

        private static double[,] ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int rows = model.Constraints.Count + 1;
            int columns = model.ObjectiveCoefficients.Count + model.Constraints.Count + 1;
            double[,] tableau = new double[rows, columns];

            // Objective function
            for (int j = 0; j < model.ObjectiveCoefficients.Count; j++)
                tableau[0, j] = model.ObjectiveCoefficients[j];

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
        static double[,] MultiplyMatrices(double[,] matrixA, double[,] matrixB)
        {
            int rowsA = matrixA.GetLength(0);
            int colsA = matrixA.GetLength(1);
            int rowsB = matrixB.GetLength(0);
            int colsB = matrixB.GetLength(1);

            // Check if the matrices can be multiplied
            if (colsA != rowsB)
            {
                throw new InvalidOperationException("The number of columns in Matrix A must be equal to the number of rows in Matrix B.");
            }

            // Initialize the result matrix
            double[,] result = new double[rowsA, colsB];

            // Multiply the matrices
            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    for (int k = 0; k < colsA; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return result;
        }
        static double[,] InvertMatrix(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = new double[n, n];
            double[,] augmentedMatrix = new double[n, 2 * n];

            // Create the augmented matrix [matrix | I]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, i + n] = 1;
            }

            // Perform Gaussian elimination
            for (int i = 0; i < n; i++)
            {
                // Make the diagonal contain all 1's
                double diag = augmentedMatrix[i, i];
                if (diag == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }
                for (int j = 0; j < 2 * n; j++)
                {
                    augmentedMatrix[i, j] /= diag;
                }

                // Make all rows below this one 0 in the current column
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmentedMatrix[k, i];
                        for (int j = 0; j < 2 * n; j++)
                        {
                            augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                        }
                    }
                }
            }

            // Extract the inverse matrix from the augmented matrix
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = augmentedMatrix[i, j + n];
                }
            }

            return result;
        }
        public static List<int> FindBV(double[,] matrix)
        {
            List<(int columnIndex, int rowIndex)> columnRowPairs = new List<(int, int)>();

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int j = 0; j < cols; j++)
            {
                int oneCount = 0;
                int rowIndex = -1;

                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, j] == 1.000)
                    {
                        oneCount++;
                        rowIndex = i;
                    }
                    else if (matrix[i, j] != 0.000)
                    {
                        oneCount = -1; // Column contains non-zero values other than 1
                        break;
                    }
                }

                if (oneCount == 1)
                {
                    columnRowPairs.Add((j, rowIndex)); // Save the column index along with the row index
                }
            }

            // Sort the list based on rowIndex to prioritize earlier rows
            columnRowPairs.Sort((a, b) => a.rowIndex.CompareTo(b.rowIndex));

            // Extract the column indexes from the sorted list
            List<int> sortedColumnIndexes = new List<int>();
            foreach (var pair in columnRowPairs)
            {
                sortedColumnIndexes.Add(pair.columnIndex);
            }

            return sortedColumnIndexes;
        }
    }
}
