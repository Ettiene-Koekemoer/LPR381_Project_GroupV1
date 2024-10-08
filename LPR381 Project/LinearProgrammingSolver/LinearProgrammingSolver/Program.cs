﻿using System;
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
                        //Alternative parsing route for when working with Knapsack BnB
                        var model = LinearProgrammingModel.ReadKnapsackInput(filePath);
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
                    BranchAndBoundSimplex.RunBranchAndBoundAlgorithm(); //This is a joke... it doen't even use the loaded modal...
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
