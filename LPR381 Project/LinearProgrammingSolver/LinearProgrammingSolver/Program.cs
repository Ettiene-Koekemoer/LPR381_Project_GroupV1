using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LinearProgrammingSolver
{
    class Program
    {
        static LinearProgrammingModel optimalModel = null; // Global variable to store the optimal model

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Linear Programming Solver");

            LinearProgrammingModel model = null;

            while (true)
            {
                if (model == null)
                {
                    model = LoadModel();
                    if (model == null)
                    {
                        Console.WriteLine("Failed to load model. Exiting...");
                        return;
                    }
                }

                ShowMenu(model);
                model = null; // Reset model after menu actions
            }
        }

        static void ShowMenu(LinearProgrammingModel model)
        {
            while (true)
            {
                Console.WriteLine("1. Select Algorithm");
                Console.WriteLine("2. Perform Sensitivity Analysis");
                Console.WriteLine("3. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectAlgorithm(model);
                        break;
                    case "2":
                        if (optimalModel != null)
                        {
                            // Perform sensitivity analysis only if the optimal table is available
                            // SensitivityAnalysis.Perform(optimalModel);
                        }
                        else
                        {
                            Console.WriteLine("Please run an algorithm first to get the optimal table.");
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        static LinearProgrammingModel LoadModel()
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..","..","Models");
            basePath = Path.GetFullPath(basePath); // Resolve relative path to absolute

            try
            {
                var files = Directory.GetFiles(basePath, "*.txt");
                if (files.Length == 0)
                {
                    Console.WriteLine("No model files found in the directory.");
                    return null;
                }
                
                Console.WriteLine();
                Console.WriteLine("Please select one of the available models:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }
                
                Console.WriteLine();
                Console.WriteLine("Enter the number of the model you want to load:");
                
                if (int.TryParse(Console.ReadLine(), out int fileIndex) && fileIndex > 0 && fileIndex <= files.Length)
                {
                    var filePath = files[fileIndex - 1];
                    
                    if (filePath.ToLower().Contains("knapsack"))
                    {
                        var model = ReadKnapsackInput(filePath);
                        Console.WriteLine("Knapsack model loaded successfully.");
                        return model;
                    }
                    else
                    {
                        var model = new LinearProgrammingModel();
                        model.ParseInputFile(filePath);
                        Console.WriteLine("Model loaded successfully.");
                        return model;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing directory: {ex.Message}");
                return null;
            }
        }

        static LinearProgrammingModel ReadKnapsackInput(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var firstLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool isMaximization = firstLine[0].ToLower() == "max";
            var objectiveCoefficients = firstLine.Skip(1).Select(double.Parse).ToArray();
            var constraints = new List<double[]>();
            var rhs = new List<double>();

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && (parts[0].StartsWith("x") || parts[0] == "bin"))
                {
                    break;
                }

                var constraint = parts.Take(parts.Length - 2).Select(double.Parse).ToArray();
                constraints.Add(constraint);
                rhs.Add(double.Parse(parts.Last()));
            }

            var lastLine = lines.Last().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var variables = lastLine.Take(lastLine.Length - 1).ToList();
            var variableTypes = lastLine.Last(); 

            if (variableTypes != "bin")
            {
                throw new ArgumentException("Variable type must be binary for knapsack problems.");
            }

            var A = new double[constraints.Count, objectiveCoefficients.Length];
            var b = rhs.ToArray();

            for (int i = 0; i < constraints.Count; i++)
            {
                for (int j = 0; j < constraints[i].Length; j++)
                {
                    A[i, j] = constraints[i][j];
                }
            }

            var variableIndices = Enumerable.Range(0, objectiveCoefficients.Length).ToList();
            var N = variables; 

            return new LinearProgrammingModel(
                B: variableIndices,
                N: variables,
                A: A,
                cB: objectiveCoefficients,
                cN: objectiveCoefficients,
                b: b
            );
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
                    BranchAndBoundSimplex.RunBranchAndBoundAlgorithm();
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
