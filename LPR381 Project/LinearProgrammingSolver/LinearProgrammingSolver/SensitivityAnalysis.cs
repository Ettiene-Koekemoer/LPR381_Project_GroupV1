// using System;
// using System.Collections.Generic;

// namespace LinearProgrammingSolver
// {
//     public static class SensitivityAnalysis
//     {
//         public static void Perform(LinearProgrammingModel model)
//         {
//             double[,] optimalSolution = model.Solution;
//             if (optimalSolution != null)
//             {
//                 while (true)
//                 {
//                     Console.WriteLine();
//                     Console.WriteLine("Select Sensitivity Analysis to perform:");
//                     Console.WriteLine("1.  Display the range of a selected Non-Basic Variable");
//                     Console.WriteLine("2.  Apply and display a change of a selected Non-Basic Variable");
//                     Console.WriteLine("3.  Display the range of a selected Basic Variable");
//                     Console.WriteLine("4.  Apply and display a change of a selected Basic Variable");
//                     Console.WriteLine("5.  Display the range of a selected constraint right-hand-side value");
//                     Console.WriteLine("6.  Apply and display a change of a selected constraint right-hand-side value");
//                     Console.WriteLine("7.  Display the range of a selected variable in a Non-Basic Variable column");
//                     Console.WriteLine("8.  Apply and display a change of a selected variable in a Non-Basic Variable column");
//                     Console.WriteLine("9.  Add a new activity to an optimal solution");
//                     Console.WriteLine("10. Add a new constraint to an optimal solution");
//                     Console.WriteLine("11. Display the shadow prices");
//                     Console.WriteLine("12. Duality");
//                     Console.WriteLine("13. Exit");

//                     var choice = Console.ReadLine();

//                     switch (choice)
//                     {
//                         case "1":
//                             DisplayNonBasicVariableRange(optimalSolution);
//                             break;
//                         case "2":
//                             ApplyNonBasicVariableChange(optimalSolution);
//                             break;
//                         case "3":
//                             DisplayBasicVariableRange(optimalSolution);
//                             break;
//                         case "4":
//                             ApplyBasicVariableChange(optimalSolution);
//                             break;
//                         case "5":
//                             DisplayConstraintRHSRange(optimalSolution);
//                             break;
//                         case "6":
//                             ApplyConstraintRHSChange(optimalSolution);
//                             break;
//                         case "7":
//                             DisplayNonBasicVariableColumnRange(optimalSolution);
//                             break;
//                         case "8":
//                             ApplyNonBasicVariableColumnChange(optimalSolution);
//                             break;
//                         case "9":
//                             AddNewActivity(optimalSolution);
//                             break;
//                         case "10":
//                             AddNewConstraint(optimalSolution);
//                             break;
//                         case "11":
//                             DisplayShadowPrices(optimalSolution);
//                             break;
//                         case "12":
//                             PerformDuality(optimalSolution);
//                             break;
//                         case "13":
//                             return;
//                         default:
//                             Console.WriteLine("Invalid choice, please try again.");
//                             break;
//                     }
//                 }            
//             }
//             else
//             {
//                 Console.WriteLine("An optimal solution was not present or there was an error");
//                 Console.WriteLine();
//             }
//         }

//         private static void DisplayNonBasicVariableRange(double[,] solutionTableau)
//         {
//             // Implement the logic to display the range of a selected Non-Basic Variable
//         }

//         private static void ApplyNonBasicVariableChange(double[,] solutionTableau)
//         {
//             // Implement the logic to apply and display a change of a selected Non-Basic Variable
//         }

//         private static void DisplayBasicVariableRange(double[,] solutionTableaub)
//         {
//             int m = b.Length; // Number of constraints
//             int numBasicVariables = B.Count;

//             Console.WriteLine("Basic Variable Ranges:");

//             for (int i = 0; i < numBasicVariables; i++)
//             {
//                 int basicVariableIndex = B[i];
//                 double minRange = double.NegativeInfinity;
//                 double maxRange = double.PositiveInfinity;

//                 double cBi = cB[i];

//                 for (int j = 0; j < m; j++)
//                 {
//                     if (A[j, basicVariableIndex] != 0)
//                     {
//                         double aij = A[j, basicVariableIndex];
//                         double rhs = b[j];
//                         double range = (rhs - cBi) / aij;

//                         if (A[j, basicVariableIndex] > 0)
//                         {
//                             minRange = Math.Max(minRange, range);
//                         }
//                         else
//                         {
//                             maxRange = Math.Min(maxRange, range);
//                         }
//                     }
//                 }

//                 Console.WriteLine($"Variable {B[i]}: [{minRange}, {maxRange}]");
//             }
//         }

//         private static void ApplyBasicVariableChange(double[,] solutionTableau)
//         {
//             // Implement the logic to apply and display a change of a selected Basic Variable
//         }

//         private static void DisplayConstraintRHSRange(double[,] solutionTableau)
//         {
//             // Implement the logic to display the range of a selected constraint right-hand-side value
//         }

//         private static void ApplyConstraintRHSChange(double[,] solutionTableau)
//         {
//             // Implement the logic to apply and display a change of a selected constraint right-hand-side value
//         }

//         private static void DisplayNonBasicVariableColumnRange(double[,] solutionTableau)
//         {
//             // Implement the logic to display the range of a selected constraint right-hand-side value
//         }

//         private static void ApplyNonBasicVariableColumnChange(double[,] solutionTableau)
//         {
//             // Implement the logic to apply and display a change of a selected constraint right-hand-side value
//         }

//         private static void DisplayNonBasicVariableColumnRange(double[,] solutionTableau)
//         {
//             int m = b.Length; // Number of constraints
//             int numBasicVariables = B.Count;

//             Console.WriteLine("Constraint Ranges:");

//             for (int i = 0; i < m; i++)
//             {
//                 double minRange = double.NegativeInfinity;
//                 double maxRange = double.PositiveInfinity;

//                 for (int j = 0; j < numBasicVariables; j++)
//                 {
//                     int basicVariableIndex = B[j];
//                     double aij = A[i, basicVariableIndex];
//                     double rhs = b[i];

//                     if (aij != 0)
//                     {
//                         double impact = (rhs - cB[j]) / aij;

//                         if (aij > 0)
//                         {
//                             minRange = Math.Max(minRange, impact);
//                         }
//                         else
//                         {
//                             maxRange = Math.Min(maxRange, impact);
//                         }
//                     }
//                 }

//                 Console.WriteLine($"Constraint {i + 1}: [{minRange}, {maxRange}]");
//             }
//         }

//         private static void ApplyNonBasicVariableColumnChange(double[,] solutionTableau)
//         {
//             Console.WriteLine("Applying change to a Constraint...");

//             Console.Write("Enter the index of the constraint to change: ");
//             int constraintIndex = int.Parse(Console.ReadLine());

//             Console.Write("Enter the new RHS value for the constraint: ");
//             double newRHS = double.Parse(Console.ReadLine());

//             b[constraintIndex] = newRHS;

//             Console.WriteLine($"New RHS for constraint {constraintIndex}: {newRHS}");
//         }

//         private static void AddNewActivity(double[,] solutionTableau)
//         {
//             // Implement the logic to add a new activity to an optimal solution
//         }

//         private static void AddNewConstraint(double[,] solutionTableau)
//         {
//             Console.WriteLine("Adding a new Constraint...");

//             Console.Write("Enter the new constraint's RHS value: ");
//             double newRHS = double.Parse(Console.ReadLine());

//             Console.Write("Enter the number of coefficients for the new constraint: ");
//             int numCoefficients = int.Parse(Console.ReadLine());

//             // Update the constraints
//             Array.Resize(ref b, b.Length + 1);
//             b[b.Length - 1] = newRHS;

//             double[,] newA = new double[A.GetLength(0) + 1, A.GetLength(1)];
//             Array.Copy(A, newA, A.Length);

//             Console.WriteLine("Enter coefficients for the new constraint:");
//             for (int i = 0; i < numCoefficients; i++)
//             {
//                 Console.Write($"Coefficient {i + 1}: ");
//                 newA[newA.GetLength(0) - 1, i] = double.Parse(Console.ReadLine());
//             }
//             A = newA;
//         }

//         private static void DisplayShadowPrices(double[,] tableau/*, double[,] solutionTableau*/)
//         {
//             Console.WriteLine("Shadow Prices:");

//             // Assuming shadow prices are calculated as dual values in a simplex tableau or similar
//             // Placeholder: Adjust according to actual shadow price calculation
//             for (int i = 0; i < b.Length; i++)
//             {
//                 Console.WriteLine($"Constraint {i + 1}: {b[i] * 0.5}"); // Placeholder calculation
//             }
//         }

//         private static void PerformDuality(double[,] tableau)
//         {
//             Console.WriteLine("Solving Dual Programming Model...");

//             // Implement dual programming model solver here
//             // Placeholder implementation: Adapt based on dual programming method used
//             Console.WriteLine("Dual programming model solved.");
//         }



//         private static void DisplayTableau(double[,] tableau)
//         {
//             for (int i = 0; i < tableau.GetLength(0); i++)
//             {
//                 for (int j = 0; j < tableau.GetLength(1); j++)
//                 {
//                     Console.Write($"{tableau[i, j],10:F3}");
//                 }
//                 Console.WriteLine();
//             }
//             Console.WriteLine();
//         }
//     }
// }
