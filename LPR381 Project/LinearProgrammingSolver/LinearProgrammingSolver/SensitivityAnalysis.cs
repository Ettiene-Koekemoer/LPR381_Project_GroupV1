using System;
using System.Collections.Generic;

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
                            DisplayNonBasicVariableRange(optimalSolution);
                            break;
                        case "2":
                            ApplyNonBasicVariableChange(optimalSolution);
                            break;
                        case "3":
                            DisplayBasicVariableRange(optimalSolution);
                            break;
                        case "4":
                            ApplyBasicVariableChange(optimalSolution);
                            break;
                        case "5":
                            DisplayConstraintRHSRange(optimalSolution);
                            break;
                        case "6":
                            ApplyConstraintRHSChange(optimalSolution);
                            break;
                        case "7":
                            DisplayNonBasicVariableColumnRange(optimalSolution);
                            break;
                        case "8":
                            ApplyNonBasicVariableColumnChange(optimalSolution);
                            break;
                        case "9":
                            AddNewActivity(optimalSolution);
                            break;
                        case "10":
                            AddNewConstraint(optimalSolution);
                            break;
                        case "11":
                            DisplayShadowPrices(optimalSolution);
                            break;
                        case "12":
                            PerformDuality(optimalSolution);
                            break;
                        case "13":
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
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

        private static void DisplayNonBasicVariableRange(double[,] solutionTableau)
        {
            // Implement the logic to display the range of a selected Non-Basic Variable
        }

        private static void ApplyNonBasicVariableChange(double[,] solutionTableau)
        {
            // Implement the logic to apply and display a change of a selected Non-Basic Variable
        }

        private static void DisplayBasicVariableRange(double[,] solutionTableaub)
        {
            // Implement the logic to display the range of a selected Basic Variable
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
    }
}
