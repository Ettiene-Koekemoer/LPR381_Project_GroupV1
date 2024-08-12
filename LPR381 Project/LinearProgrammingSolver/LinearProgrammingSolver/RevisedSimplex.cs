using System;
using System.Collections.Generic;
using System.Text;

namespace LinearProgrammingSolver
{
    public static class RevisedSimplex
    {
        public static void Solve(LinearProgrammingModel model)
        {
            StringBuilder outputString = new StringBuilder();
            Console.WriteLine("Solving using Revised Simplex Algorithm...");

            // Convert the model to the canonical form
            var tableau = ConvertToCanonicalForm(model);
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
                }
            }
            var cbvb = Init(model, tableau, "cbvb-");
            Console.WriteLine("Final CBVB-");
            DisplayTableau(cbvb);
            outputString.Append(BuildTable(cbvb));
            var b = Init(model, tableau, "b-");
            Console.WriteLine("Final B-");
            DisplayTableau(b);
            outputString.Append(BuildTable(b));
            DisplayTableau(tableau);
            outputString.Append(BuildTable(tableau));

            outputString.Append($"Optimal Solution: {tableau[0, tableau.GetLength(1) - 1]}");
            WriteOutput(outputString.ToString());
            SaveSolution(tableau, model);
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
            string outputFilePath = "revised_output.txt";
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

        private static double[,] ConvertToCanonicalFormRevised(LinearProgrammingModel model)
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

        private static double[,] Init(LinearProgrammingModel model, double[,] solutionTableau, string table)
        {
            double[,] initOutput;
            List<int> bv = new List<int>();
            var iTable = ConvertToCanonicalForm(model);
            bv = FindBV(solutionTableau);
            double[,] cbv = new double[1, bv.Count];
            double[,] b = new double[iTable.GetLength(0) - 1, bv.Count];
            double[,] rhsb = new double[iTable.GetLength(0) - 1, 1];

            for (int k = 0; k < bv.Count; k++)
            {
                int colIndex = bv[k];

                // Store the value at row 0, matched column index
                cbv[0, k] = Math.Round(iTable[0, colIndex],3);

                // Store the values below row 0 for the matched column index
                for (int i = 1; i < iTable.GetLength(0); i++)
                {
                    b[i - 1, k] = Math.Round(iTable[i, colIndex],3);
                }
            }
            b = InvertMatrix(b);
            switch (table)
            {
                case "cbvb-":
                    initOutput = MultiplyMatrices(cbv, b);
                    return initOutput;
                case "b-":
                    initOutput = b;
                    return initOutput;
                case "b":
                    int rhsIndex = iTable.GetLength(1) - 1;
                    for (int i = 1; i < iTable.GetLength(0); i++)
                    {
                        rhsb[i - 1, 0] = iTable[i, rhsIndex];
                    }
                    initOutput = rhsb;
                    return initOutput;
                case "bb":
                    initOutput = b;
                    return initOutput;
                default:
                    initOutput = solutionTableau;
                    return initOutput;
            }
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

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = Math.Round(result[i, j], 3);
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
                    //throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
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

        public static int FindMaxIndexInFirstRow(double[,] array)
        {
            int columns = array.GetLength(1);  // Number of columns in the array
            int maxIndex = 0; // Assume the first element is the maximum initially
            double maxValue = array[0, 0]; // Start with the first value in the first row

            // Loop through the first row to find the maximum value
            for (int i = 1; i < columns; i++)
            {
                if (array[0, i] > maxValue)
                {
                    maxValue = array[0, i];
                    maxIndex = i; // Update the index of the maximum value
                }
            }

            return maxIndex; // Return the index of the largest value
        }
    }
}
