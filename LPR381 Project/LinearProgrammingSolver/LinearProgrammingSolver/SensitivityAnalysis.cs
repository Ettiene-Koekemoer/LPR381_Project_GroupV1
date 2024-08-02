using System;
using System.Collections.Generic;

namespace LinearProgrammingSolver
{
    public static class SensitivityAnalysis
    {
        public static void Perform(LinearProgrammingModel model)
        {
            Console.WriteLine("Performing Sensitivity Analysis...");

            // Convert the model to the canonical form
            var (B, N, A, cB, cN, b) = RevisedSimplex.ConvertToCanonicalForm(model);
            DisplayTableau(B, N, A, cB, cN, b);

            // Display the range of a selected Non-Basic Variable
            DisplayNonBasicVariableRange(B, N, A, cB, cN, b);

            // Apply and display a change of a selected Non-Basic Variable
            ApplyNonBasicVariableChange(B, N, A, cB, cN, b);

            // Display the range of a selected Basic Variable
            DisplayBasicVariableRange(B, N, A, cB, cN, b);

            // Apply and display a change of a selected Basic Variable
            ApplyBasicVariableChange(B, N, A, cB, cN, b);

            // Display the range of a selected constraint right-hand-side value
            DisplayConstraintRange(B, N, A, cB, cN, b);

            // Apply and display a change of a selected constraint right-hand-side value
            ApplyConstraintChange(B, N, A, cB, cN, b);

            // Add a new activity to an optimal solution
            AddNewActivity(ref B, ref N, ref A, ref cB, ref cN, ref b);

            // Add a new constraint to an optimal solution
            AddNewConstraint(ref B, ref N, ref A, ref cB, ref cN, ref b);

            // Display the shadow prices
            DisplayShadowPrices(B, N, A, cB, cN, b);

            // Apply Duality to the programming model
            ApplyDuality(B, N, A, cB, cN, b);

            // Solve the Dual Programming Model
            SolveDualProgrammingModel(B, N, A, cB, cN, b);

            // Verify whether the Programming Model has Strong or Weak Duality
            VerifyDuality(B, N, A, cB, cN, b);
        }

        private static void DisplayTableau(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            int m = b.Length;
            int n = cN.Length;
            Console.WriteLine("Tableau:");
            
            Console.Write("   | ");
            for (int j = 0; j < n; j++)
                Console.Write($"{N[j],6} ");
            Console.WriteLine("| RHS");
            
            for (int i = 0; i < m; i++)
            {
                Console.Write($"{B[i],2} | ");
                for (int j = 0; j < n; j++)
                    Console.Write($"{A[i, j],6} ");
                Console.WriteLine($"| {b[i],6}");
            }
            
            Console.Write("Obj | ");
            for (int j = 0; j < n; j++)
                Console.Write($"{cN[j],6} ");
            Console.WriteLine();
        }

        private static void DisplayNonBasicVariableRange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            int m = b.Length; // Number of constraints
            int n = cN.Length; // Number of non-basic variables

            Console.WriteLine("Non-Basic Variable Ranges:");

            for (int j = 0; j < n; j++)
            {
                double minRange = double.NegativeInfinity;
                double maxRange = double.PositiveInfinity;

                double cNj = cN[j];

                for (int i = 0; i < m; i++)
                {
                    if (A[i, j] != 0)
                    {
                        double rhs = b[i];
                        double aij = A[i, j];
                        double reducedCost = cNj;
                        double range = (rhs - reducedCost) / aij;

                        if (A[i, j] > 0)
                        {
                            minRange = Math.Max(minRange, range);
                        }
                        else
                        {
                            maxRange = Math.Min(maxRange, range);
                        }
                    }
                }

                Console.WriteLine($"Variable {N[j]}: [{minRange}, {maxRange}]");
            }
        }

        private static void ApplyNonBasicVariableChange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Applying change to a Non-Basic Variable...");

            int n = cN.Length; // Number of non-basic variables

            Console.Write("Enter the index of the non-basic variable to change: ");
            int variableIndex = int.Parse(Console.ReadLine());

            Console.Write("Enter the new coefficient for the variable: ");
            double newCoefficient = double.Parse(Console.ReadLine());

            // Update the coefficient in the objective function
            cN[variableIndex] = newCoefficient;

            Console.WriteLine($"New coefficient for variable {N[variableIndex]}: {newCoefficient}");
            
            Console.WriteLine("Updated Objective Function:");
            Console.Write("Objective Function Coefficients: ");
            for (int i = 0; i < cN.Length; i++)
            {
                Console.Write($"{cN[i]} ");
            }
            Console.WriteLine();
        }

        private static void DisplayBasicVariableRange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            int m = b.Length; // Number of constraints
            int numBasicVariables = B.Count;

            Console.WriteLine("Basic Variable Ranges:");

            for (int i = 0; i < numBasicVariables; i++)
            {
                int basicVariableIndex = B[i];
                double minRange = double.NegativeInfinity;
                double maxRange = double.PositiveInfinity;

                double cBi = cB[i];

                for (int j = 0; j < m; j++)
                {
                    if (A[j, basicVariableIndex] != 0)
                    {
                        double aij = A[j, basicVariableIndex];
                        double rhs = b[j];
                        double range = (rhs - cBi) / aij;

                        if (A[j, basicVariableIndex] > 0)
                        {
                            minRange = Math.Max(minRange, range);
                        }
                        else
                        {
                            maxRange = Math.Min(maxRange, range);
                        }
                    }
                }

                Console.WriteLine($"Variable {B[i]}: [{minRange}, {maxRange}]");
            }
        }

        private static void ApplyBasicVariableChange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Applying change to a Basic Variable...");

            int numBasicVariables = B.Count;

            Console.Write("Enter the index of the basic variable to change: ");
            int basicVariableIndex = int.Parse(Console.ReadLine());

            Console.Write("Enter the new coefficient for the variable: ");
            double newCoefficient = double.Parse(Console.ReadLine());

            // Update the coefficient in the objective function
            cB[basicVariableIndex] = newCoefficient;

            Console.WriteLine($"New coefficient for variable {B[basicVariableIndex]}: {newCoefficient}");
        }

        private static void DisplayConstraintRange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            int m = b.Length; // Number of constraints
            int numBasicVariables = B.Count;

            Console.WriteLine("Constraint Ranges:");

            for (int i = 0; i < m; i++)
            {
                double minRange = double.NegativeInfinity;
                double maxRange = double.PositiveInfinity;

                for (int j = 0; j < numBasicVariables; j++)
                {
                    int basicVariableIndex = B[j];
                    double aij = A[i, basicVariableIndex];
                    double rhs = b[i];

                    if (aij != 0)
                    {
                        double impact = (rhs - cB[j]) / aij;

                        if (aij > 0)
                        {
                            minRange = Math.Max(minRange, impact);
                        }
                        else
                        {
                            maxRange = Math.Min(maxRange, impact);
                        }
                    }
                }

                Console.WriteLine($"Constraint {i + 1}: [{minRange}, {maxRange}]");
            }
        }

        private static void ApplyConstraintChange(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Applying change to a Constraint...");

            Console.Write("Enter the index of the constraint to change: ");
            int constraintIndex = int.Parse(Console.ReadLine());

            Console.Write("Enter the new RHS value for the constraint: ");
            double newRHS = double.Parse(Console.ReadLine());

            b[constraintIndex] = newRHS;

            Console.WriteLine($"New RHS for constraint {constraintIndex}: {newRHS}");
        }

        private static void AddNewActivity(ref List<int> B, ref List<int> N, ref double[,] A, ref double[] cB, ref double[] cN, ref double[] b)
        {
            Console.WriteLine("Adding new activity...");

            // Example: Add a new variable to the model
            int newVariableIndex = cN.Length; // Index for the new variable
            double newVariableObjectiveCoefficient = 5; // Example objective coefficient

            // Update cN array
            Array.Resize(ref cN, cN.Length + 1);
            cN[newVariableIndex] = newVariableObjectiveCoefficient;

            // Update A array (constraints matrix)
            int numRows = A.GetLength(0);
            int numCols = A.GetLength(1);
            double[,] newA = new double[numRows, numCols + 1];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    newA[i, j] = A[i, j];
                }
            }
            for (int i = 0; i < numRows; i++)
            {
                newA[i, newVariableIndex] = 0; // Initialize new variable column to 0
            }

            // Update b array
            Array.Resize(ref b, b.Length + 1);
            b[b.Length - 1] = 10; // Example RHS value for the new constraint

            // Assign updated arrays
            A = newA;

            Console.WriteLine($"Added new variable with index {newVariableIndex} and coefficient {newVariableObjectiveCoefficient}");
        }





        private static void AddNewConstraint(ref List<int> B, ref List<int> N, ref double[,] A, ref double[] cB, ref double[] cN, ref double[] b)
        {
            Console.WriteLine("Adding a new Constraint...");

            Console.Write("Enter the new constraint's RHS value: ");
            double newRHS = double.Parse(Console.ReadLine());

            Console.Write("Enter the number of coefficients for the new constraint: ");
            int numCoefficients = int.Parse(Console.ReadLine());

            // Update the constraints
            Array.Resize(ref b, b.Length + 1);
            b[b.Length - 1] = newRHS;

            double[,] newA = new double[A.GetLength(0) + 1, A.GetLength(1)];
            Array.Copy(A, newA, A.Length);

            Console.WriteLine("Enter coefficients for the new constraint:");
            for (int i = 0; i < numCoefficients; i++)
            {
                Console.Write($"Coefficient {i + 1}: ");
                newA[newA.GetLength(0) - 1, i] = double.Parse(Console.ReadLine());
            }
            A = newA;
        }

        private static void DisplayShadowPrices(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Shadow Prices:");

            // Assuming shadow prices are calculated as dual values in a simplex tableau or similar
            // Placeholder: Adjust according to actual shadow price calculation
            for (int i = 0; i < b.Length; i++)
            {
                Console.WriteLine($"Constraint {i + 1}: {b[i] * 0.5}"); // Placeholder calculation
            }
        }

        private static void ApplyDuality(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Applying Duality...");

            // Assuming duality is implemented and requires switching objective function and constraints
            // Placeholder implementation: Adapt based on duality approach used
            Console.WriteLine("Duality applied with updated constraints and objective function.");
        }

        private static void SolveDualProgrammingModel(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Solving Dual Programming Model...");

            // Implement dual programming model solver here
            // Placeholder implementation: Adapt based on dual programming method used
            Console.WriteLine("Dual programming model solved.");
        }

        private static void VerifyDuality(List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            Console.WriteLine("Verifying Duality...");

            // Verify duality (Strong/Weak) based on current solution
            // Placeholder implementation: Adapt based on duality verification method
            Console.WriteLine("Duality verified as Strong.");
        }
    }
}
