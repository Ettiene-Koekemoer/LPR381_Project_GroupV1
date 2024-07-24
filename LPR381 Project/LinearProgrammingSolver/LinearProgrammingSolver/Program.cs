using System;

namespace LinearProgrammingSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Linear Programming Solver");
            ShowMenu();
        }

        static void ShowMenu()
        {
            LinearProgrammingModel model = null;

            while (true)
            {
                Console.WriteLine("1. Load Model from File");
                Console.WriteLine("2. Select Algorithm");
                Console.WriteLine("3. Perform Sensitivity Analysis");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        model = LoadModel();
                        break;
                    case "2":
                        if (model != null)
                            SelectAlgorithm(model);
                        else
                            Console.WriteLine("Please load a model first.");
                        break;
                    case "3":
                        if (model != null)
                            SensitivityAnalysis.Perform(model);
                        else
                            Console.WriteLine("Please load a model first.");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static LinearProgrammingModel LoadModel()
        {
            Console.WriteLine("Enter the path to the input file:");
            var filePath = Console.ReadLine();

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    var model = new LinearProgrammingModel();
                    model.ParseInputFile(filePath);
                    Console.WriteLine("Model loaded successfully.");
                    return model;
                }
                else
                {
                    Console.WriteLine("File not found. Please check the path and try again.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }

        static void SelectAlgorithm(LinearProgrammingModel model)
        {
            Console.WriteLine("Select Algorithm:");
            Console.WriteLine("1. Primal Simplex Algorithm");
            Console.WriteLine("2. Revised Primal Simplex Algorithm");
            Console.WriteLine("3. Branch & Bound Simplex Algorithm");
            Console.WriteLine("4. Cutting Plane Algorithm");
            Console.WriteLine("5. Branch and Bound Knapsack Algorithm");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PrimalSimplex.Solve(model);
                    break;
                case "2":
                    RevisedSimplex.Solve(model);
                    break;
                case "3":
                    BranchAndBoundSimplex.Solve(model);
                    break;
                case "4":
                    CuttingPlane.Solve(model);
                    break;
                case "5":
                    BranchAndBoundKnapsack.Solve(model);
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }
}
