using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Models");
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
                        if (model != null)
                        {
                            Console.WriteLine("Knapsack model loaded successfully.");
                            return model;
                        }
                        else
                        {
                            Console.WriteLine("Failed to read Knapsack model. Please check the file format.");
                            return null;
                        }
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
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    throw new InvalidOperationException("File is empty.");
                }

                var firstLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (firstLine.Length < 2)
                {
                    throw new InvalidOperationException("Invalid header line in file.");
                }

                bool isMaximization = firstLine[0].ToLower() == "max";
                var objectiveCoefficients = firstLine.Skip(1).Select(double.Parse).ToArray();
                
                var constraints = new List<double[]>();
                var rhs = new List<double>();

                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0 || parts[0].StartsWith("x") || parts[0] == "bin")
                    {
                        break;
                    }

                    var constraint = parts.Take(parts.Length - 1).Select(double.Parse).ToArray();
                    constraints.Add(constraint);
                    rhs.Add(double.Parse(parts.Last()));
                }

                if (constraints.Count == 0 || rhs.Count == 0)
                {
                    throw new InvalidOperationException("No constraints or RHS values found.");
                }

                var lastLine = lines.Last().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (lastLine.Length < 2 || lastLine.Last() != "bin")
                {
                    throw new InvalidOperationException("Variable type must be binary for knapsack problems.");
                }

                var variables = lastLine.Take(lastLine.Length - 1).ToList();

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
