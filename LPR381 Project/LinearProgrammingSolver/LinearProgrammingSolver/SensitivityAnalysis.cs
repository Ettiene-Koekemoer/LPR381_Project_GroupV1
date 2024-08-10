using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                            ApplyBasicVariableChange(optimalSolution, model);
                            Console.ReadLine();
                            break;
                        case "5":
                            DisplayConstraintRHSRange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "6":
                            ApplyConstraintRHSChange(optimalSolution, model);
                            Console.ReadLine();
                            break;
                        case "7":
                            DisplayNonBasicVariableColumnRange(optimalSolution);
                            Console.ReadLine();
                            break;
                        case "8":
                            ApplyNonBasicVariableColumnChange(optimalSolution, model);
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
        }

        private static void ApplyNonBasicVariableChange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to apply and display a change of a selected Non-Basic Variable
            double[,] iTableau = ConvertToCanonicalForm(model);
            double[,] a = new double[iTableau.GetLength(0) - 1, 1];
            var bv = FindBV(solutionTableau);

            Console.WriteLine("Select a non basic variable coeffiecient to change:");
            for (int j = 0; j < model.ObjectiveCoefficients.Count; j++)
            {
                if (!bv.Contains(j))
                {
                    Console.WriteLine($"{j}. {model.ObjectiveCoefficients[j]}");
                }
            }
            var choice = Console.ReadLine();
            int choiceIndex;
            //possible valid input verifying
            if (int.TryParse(choice, out choiceIndex))
            {
                //
            }
            else
            {
                while (!(int.TryParse(choice, out choiceIndex)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer value.");
                    choice = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter the new coefficient value:");
            var input = Console.ReadLine();
            double value;
            if (double.TryParse(input, out value))
            {
                //
            }
            else
            {
                while (!(double.TryParse(input, out value)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid double value.");
                    input = Console.ReadLine();
                }
            }

            double[,] cbvb = Init(model, solutionTableau, "cbvb-");
            int varIndex = choiceIndex;
            for (int i = 1; i < iTableau.GetLength(0); i++)
            {
                a[i - 1, 0] = iTableau[i, varIndex];
            }
            double[,] newC = new double[1, 1];
            newC = (MultiplyMatrices(cbvb, a));
            newC[0,0] = Math.Round((newC[0,0] - value),3);

            solutionTableau[0,varIndex] = newC[0,0];
            DisplayTableau(solutionTableau);
            while (true)
            {
                int pivotColumn = SelectPivotColumn(solutionTableau, model);
                if (pivotColumn == -1)
                    break; // Optimal solution found

                int pivotRow = SelectPivotRow(solutionTableau, pivotColumn);
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                Pivot(solutionTableau, pivotRow, pivotColumn);
            }
            DisplayTableau(solutionTableau);
        }

        private static void DisplayBasicVariableRange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to display the range of a selected Basic Variable
        }

        private static void ApplyBasicVariableChange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to apply and display a change of a selected Basic Variable
            double[,] iTableau = ConvertToCanonicalForm(model);
            double[,] a = new double[iTableau.GetLength(0) - 1, 1];
            var bv = FindBV(solutionTableau);

            Console.WriteLine("Select a basic variable coeffiecient to change:");
            for (int j = 0; j < model.ObjectiveCoefficients.Count; j++)
            {
                if (bv.Contains(j))
                {
                    Console.WriteLine($"{j}. {model.ObjectiveCoefficients[j]}");
                }
            }
            var choice = Console.ReadLine();
            int choiceIndex;
            //possible valid input verifying
            if (int.TryParse(choice, out choiceIndex))
            {
                //
            }
            else
            {
                while (!(int.TryParse(choice, out choiceIndex)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer value.");
                    choice = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter the new coefficient value:");
            var input = Console.ReadLine();
            double value;
            if (double.TryParse(input, out value))
            {
                //
            }
            else
            {
                while (!(double.TryParse(input, out value)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid double value.");
                    input = Console.ReadLine();
                }
            }

            model.ObjectiveCoefficients[choiceIndex] = value;

            double[,] cbvb = Init(model, solutionTableau, "cbvb-");
            double[,] newC = new double[1, 1];
            for (int j = 0; j < model.ObjectiveCoefficients.Count; j++)
            {
                for (int i = 1; i < iTableau.GetLength(0); i++)
                {
                    a[i - 1, 0] = iTableau[i, j];
                }
                newC = (MultiplyMatrices(cbvb, a));
                if (j == choiceIndex)
                {
                    newC[0, 0] = Math.Round((newC[0, 0] - value), 3);
                }
                else
                {
                    newC[0, 0] = Math.Round((newC[0, 0] - iTableau[0, j]), 3);
                }                
                solutionTableau[0, j] = newC[0, 0];
            }

            List<int> counts = GetOperatorCounts(model);
            // Output the counts
            for (int i = 0; i < model.ConstraintOperators.Count; i++)
            {
                if (model.ConstraintOperators[i] == ">=")
                {
                    solutionTableau[0, model.ObjectiveCoefficients.Count + i] = -(cbvb[0, counts[i] - 1]);
                }
                else
                {
                    solutionTableau[0, model.ObjectiveCoefficients.Count + i] = cbvb[0, counts[i] - 1];
                }                
            }

            double[,] b = Init(model, solutionTableau, "b");
            double[,] z = new double[1, 1];
            z = MultiplyMatrices(cbvb,b);          
            solutionTableau[0, solutionTableau.GetLength(1) - 1] = z[0, 0];

            while (true)
            {
                int pivotColumn = SelectPivotColumn(solutionTableau, model);
                if (pivotColumn == -1)
                    break; // Optimal solution found

                int pivotRow = SelectPivotRow(solutionTableau, pivotColumn);
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                Pivot(solutionTableau, pivotRow, pivotColumn);
            }
            DisplayTableau(solutionTableau);
        }

        private static void DisplayConstraintRHSRange(double[,] solutionTableau)
        {
            // Implement the logic to display the range of a selected constraint right-hand-side value
        }

        private static void ApplyConstraintRHSChange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to apply and display a change of a selected constraint right-hand-side value
            double[,] iTableau = ConvertToCanonicalForm(model);
            double[,] b = Init(model, solutionTableau, "b");
            Console.WriteLine("Select RHS value to change:");
            for (int i = 1; i < iTableau.GetLength(0); i++)
            {
                Console.WriteLine($"{i}. {iTableau[i, iTableau.GetLength(1)-1]}");
            }
            var choice = Console.ReadLine();
            int choiceIndex;
            if (int.TryParse(choice, out choiceIndex)) { }
            else
            {
                while (!(int.TryParse(choice, out choiceIndex)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer value.");
                    choice = Console.ReadLine();
                }
            }
            Console.WriteLine("Enter the new RHS value:");
            var input = Console.ReadLine();
            double value;
            if (double.TryParse(input, out value)) { }
            else
            {
                while (!(double.TryParse(input, out value)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid double value.");
                    input = Console.ReadLine();
                }
            }
            b[choiceIndex - 1, 0] = value;

            double[,] cbvb = Init(model, solutionTableau, "cbvb-");            
            double[,] bi = Init(model, solutionTableau, "b-");
            double[,] newb = new double[b.GetLength(0), b.GetLength(1)];
            newb = (MultiplyMatrices(bi, b));

            for (int i = 0; i < b.GetLength(0); i++)
            {
                solutionTableau[i + 1,solutionTableau.GetLength(1) - 1] = newb[i, 0];
            }

            double[,] z = new double[1, 1];
            z = MultiplyMatrices(cbvb, b);
            solutionTableau[0, solutionTableau.GetLength(1) - 1] = z[0, 0];

            while (true)
            {
                int pivotDualRow = SelectDualPivotRow(solutionTableau);
                if (pivotDualRow == -1)
                    break; // Optimal solution found
                int pivotDualColumn = SelectDualPivotColumn(solutionTableau, pivotDualRow);
                if (pivotDualColumn == -1)
                {
                    Console.WriteLine("Infeasible solution.");
                    return;
                }

                Pivot(solutionTableau, pivotDualRow, pivotDualColumn);
            }
            while (true)
            {
                int pivotColumn = SelectPivotColumn(solutionTableau, model);
                if (pivotColumn == -1)
                    break; // Optimal solution found

                int pivotRow = SelectPivotRow(solutionTableau, pivotColumn);
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                Pivot(solutionTableau, pivotRow, pivotColumn);
            }
            DisplayTableau(solutionTableau);
        }

        private static void DisplayNonBasicVariableColumnRange(double[,] solutionTableau)
        {
            // Implement the logic to display the range of a selected constraint right-hand-side value
        }

        private static void ApplyNonBasicVariableColumnChange(double[,] solutionTableau, LinearProgrammingModel model)
        {
            // Implement the logic to apply and display a change of a selected constraint right-hand-side value
            double[,] iTableau = ConvertToCanonicalForm(model);
            double[,] a = new double[iTableau.GetLength(0) - 1, 1];
            var bv = FindBV(solutionTableau);

            Console.WriteLine("Select the constraint you wish to change:");
            for (int i = 1; i < iTableau.GetLength(0); i++)
            {
                Console.Write($"{i}. ");
                for (int j = 0; j < iTableau.GetLength(1)-1; j++)
                {
                    Console.Write($"{iTableau[i,j]}  ");
                }
                Console.WriteLine();
            }
            var choicei = Console.ReadLine();
            int choiceIndexi;
            //possible valid input verifying
            if (int.TryParse(choicei, out choiceIndexi)) { }
            else
            {
                while (!(int.TryParse(choicei, out choiceIndexi)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer value.");
                    choicei = Console.ReadLine();
                }
            }
            Console.WriteLine("Select a non basic variable constraint coeffiecient to change:");
            for (int j = 0; j < iTableau.GetLength(1); j++)
            {
                if (!bv.Contains(j))
                {
                    Console.WriteLine($"{j}. {iTableau[choiceIndexi,j]}");
                }
            }
            var choicej = Console.ReadLine();
            int choiceIndexj;
            if (int.TryParse(choicej, out choiceIndexj)) { }
            else
            {
                while (!(int.TryParse(choicej, out choiceIndexj)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer value.");
                    choicej = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter the new coefficient value:");
            var input = Console.ReadLine();
            double value;
            if (double.TryParse(input, out value)) { }
            else
            {
                while (!(double.TryParse(input, out value)))
                {
                    Console.WriteLine("Invalid input. Please enter a valid double value.");
                    input = Console.ReadLine();
                }
            }

            double[,] cbvb = Init(model, solutionTableau, "cbvb-");
            double[,] b = Init(model, solutionTableau, "b-");
            for (int i = 1; i < iTableau.GetLength(0); i++)
            {
                a[i - 1, 0] = iTableau[i, choiceIndexj];
            }
            a[choiceIndexi - 1, 0] = value;
            double[,] newC = new double[1, 1];
            newC = (MultiplyMatrices(cbvb, a));
            newC[0, 0] = Math.Round((newC[0, 0] - iTableau[0, choiceIndexj]), 3);
            
            double[,] newA = new double[a.GetLength(0), 1];
            newA = MultiplyMatrices(b, a);

            solutionTableau[0, choiceIndexj] = newC[0, 0];
            for (int i = 1; i < solutionTableau.GetLength(0); i++)
            {
                solutionTableau[i,choiceIndexj] = newA[i - 1,0];
            }
            while (true)
            {
                int pivotColumn = SelectPivotColumn(solutionTableau, model);
                if (pivotColumn == -1)
                    break; // Optimal solution found

                int pivotRow = SelectPivotRow(solutionTableau, pivotColumn);
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                Pivot(solutionTableau, pivotRow, pivotColumn);
            }
            DisplayTableau(solutionTableau);
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

        public static List<int> GetOperatorCounts(LinearProgrammingModel model)
        {
            Dictionary<string, int> operatorCount = new Dictionary<string, int>()
            {
                { "<=", 0 },
                { ">=", 0 }
            };

            List<int> counts = new List<int>();

            foreach (var op in model.ConstraintOperators)
            {
                if (operatorCount.ContainsKey(op))
                {
                    operatorCount[op]++;
                    counts.Add(operatorCount[op]);
                }
                else
                {
                    // Handle unexpected operators if necessary
                    counts.Add(0); // Or throw an exception, or handle it in another way
                }
            }

            return counts;
        }

        private static int SelectDualPivotRow(double[,] tableau)
        {
            int pivotRow = -1;
            double minValue = 0;
            for (int j = 1; j < tableau.GetLength(0); j++)
            {
                if (tableau[j, tableau.GetLength(1) - 1] < minValue)
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
            for (int i = 0; i < tableau.GetLength(1) - 1; i++)
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
    }
}
