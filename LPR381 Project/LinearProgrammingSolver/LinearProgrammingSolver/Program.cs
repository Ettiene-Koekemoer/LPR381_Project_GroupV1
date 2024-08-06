using System;
using System.IO;
using System.Linq;

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
                Console.WriteLine("1. Load Model");
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
            const string basePath = "C:/Users/liamo/Documents/GitHub/LPR381_Project_GroupV1/LPR381 Project/Models";

            try
            {
                var files = Directory.GetFiles(basePath, "*.txt");
                if (files.Length == 0)
                {
                    Console.WriteLine("No model files found in the directory.");
                    return null;
                }

                Console.WriteLine("Available models:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }

                Console.WriteLine("Enter the number of the model you want to load:");
                if (int.TryParse(Console.ReadLine(), out int fileIndex) && fileIndex > 0 && fileIndex <= files.Length)
                {
                    var filePath = files[fileIndex - 1];
                    
                    // Check if the model is a knapsack problem
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

            // Determine if it's a max or min problem
            var firstLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool isMaximization = firstLine[0].ToLower() == "max";

            // Parse objective function coefficients
            var objectiveCoefficients = firstLine.Skip(1).Select(double.Parse).ToArray();

            // Initialize lists for constraints and RHS values
            var constraints = new List<double[]>();
            var rhs = new List<double>();

            // Parse constraints
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && (parts[0].StartsWith("x") || parts[0] == "bin"))
                {
                    break; // End of constraints
                }

                var constraint = parts.Take(parts.Length - 2).Select(double.Parse).ToArray();
                constraints.Add(constraint);
                rhs.Add(double.Parse(parts.Last()));
            }

            // Parse variable types
            var lastLine = lines.Last().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var variables = lastLine.Take(lastLine.Length - 1).ToList();
            var variableTypes = lastLine.Last(); // Should be "bin"

            // Ensure binary constraints
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

            // Create List<int> for indices
            var variableIndices = Enumerable.Range(0, objectiveCoefficients.Length).ToList();

            // Create List<string> for variable names
            var N = variables; 

            // Assuming that the second constructor is appropriate
            return new LinearProgrammingModel(
                B: variableIndices, // List<int> for indices
                N: variables, // List<string> for names
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
                    BranchAndBoundSimplex.Solve(model);
                    break;
                case "4":
                    CuttingPlane.Solve(model);
                    break;
                case "5":
                    BranchAndBoundKnapsack.Solve(model); // Ensure knapsack algorithm is selected
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }
}
