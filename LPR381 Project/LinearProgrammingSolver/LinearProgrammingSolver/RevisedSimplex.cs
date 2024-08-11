using System;
using System.Collections.Generic;
using System.IO;

namespace LinearProgrammingSolver
{
    public static class RevisedSimplex
    {
        public static (List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)? Solve(LinearProgrammingModel model)
        {
            using (var writer = new StreamWriter("Output.txt"))
            {
                writer.WriteLine("Solving using Revised Primal Simplex Algorithm...");

                // Convert the model to the canonical form
                var (B, N, A, cB, cN, b) = ConvertToCanonicalForm(model);
                DisplayTableau(writer, B, N, A, cB, cN, b);

                // Perform simplex iterations
                while (true)
                {
                    var (pivotColumn, d) = SelectPivotColumn(A, cB, cN, B, N);
                    if (pivotColumn == -1)
                        break; // Optimal solution found

                    int pivotRow = SelectPivotRow(b, d);
                    if (pivotRow == -1)
                    {
                        writer.WriteLine("Unbounded solution.");
                        return null;
                    }

                    Pivot(ref B, ref N, ref A, ref cB, ref cN, ref b, pivotRow, pivotColumn, d);
                    DisplayTableau(writer, B, N, A, cB, cN, b);
                }

                // Output the results
                WriteOutput(writer, B, N, A, cB, cN, b);

                return (B, N, A, cB, cN, b);
            }
        }

        public static (List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b) ConvertToCanonicalForm(LinearProgrammingModel model)
        {
            int m = model.Constraints.Count;
            int n = model.ObjectiveCoefficients.Count;

            var B = new List<int>();
            var N = new List<int>();
            var A = new double[m, n + m];
            var cB = new double[m];
            var cN = new double[n];
            var b = new double[m];

            for (int i = 0; i < n; i++)
            {
                N.Add(i);
                cN[i] = model.IsMaximization ? model.ObjectiveCoefficients[i] : -model.ObjectiveCoefficients[i];
            }

            for (int i = 0; i < m; i++)
            {
                B.Add(n + i);
                A[i, n + i] = 1;
                b[i] = model.RightHandSides[i];
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = model.Constraints[i][j];
                }
            }

            return (B, N, A, cB, cN, b);
        }

        private static (int pivotColumn, double[] d) SelectPivotColumn(double[,] A, double[] cB, double[] cN, List<int> B, List<int> N)
        {
            int m = B.Count;
            int n = N.Count;
            double[] u = new double[m];
            double[] d = new double[m];
            double[] reducedCosts = new double[n];
            int pivotColumn = -1;

            for (int i = 0; i < m; i++)
            {
                u[i] = 0;
                for (int j = 0; j < m; j++)
                {
                    u[i] += cB[j] * A[j, B[i]];
                }
            }

            for (int j = 0; j < n; j++)
            {
                reducedCosts[j] = cN[j];
                for (int i = 0; i < m; i++)
                {
                    reducedCosts[j] -= u[i] * A[i, N[j]];
                }

                if (reducedCosts[j] < 0)
                {
                    pivotColumn = j;
                    for (int i = 0; i < m; i++)
                    {
                        d[i] = A[i, N[pivotColumn]];
                    }
                    break;
                }
            }

            return (pivotColumn, d);
        }

        private static int SelectPivotRow(double[] b, double[] d)
        {
            int m = b.Length;
            int pivotRow = -1;
            double minRatio = double.PositiveInfinity;

            for (int i = 0; i < m; i++)
            {
                if (d[i] > 0)
                {
                    double ratio = b[i] / d[i];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            return pivotRow;
        }

        private static void Pivot(ref List<int> B, ref List<int> N, ref double[,] A, ref double[] cB, ref double[] cN, ref double[] b, int pivotRow, int pivotColumn, double[] d)
        {
            int m = B.Count;
            int n = N.Count;
            int enteringVariable = N[pivotColumn];
            int leavingVariable = B[pivotRow];

            double pivotValue = d[pivotRow];

            for (int j = 0; j < n; j++)
            {
                A[pivotRow, N[j]] /= pivotValue;
            }
            b[pivotRow] /= pivotValue;

            for (int i = 0; i < m; i++)
            {
                if (i != pivotRow)
                {
                    double factor = d[i];
                    for (int j = 0; j < n; j++)
                    {
                        A[i, N[j]] -= factor * A[pivotRow, N[j]];
                    }
                    b[i] -= factor * b[pivotRow];
                }
            }

            N[pivotColumn] = leavingVariable;
            B[pivotRow] = enteringVariable;
            cB[pivotRow] = cN[pivotColumn];
        }

        private static void DisplayTableau(StreamWriter writer, List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            int m = B.Count;
            int n = N.Count;

            writer.WriteLine("B: " + string.Join(", ", B));
            writer.WriteLine("N: " + string.Join(", ", N));
            writer.WriteLine("A:");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    writer.Write($"{A[i, N[j]],10:F3} ");
                }
                writer.WriteLine();
            }
            writer.WriteLine("cB: " + string.Join(", ", cB));
            writer.WriteLine("cN: " + string.Join(", ", cN));
            writer.WriteLine("b: " + string.Join(", ", b));
            writer.WriteLine();
        }

        private static void WriteOutput(StreamWriter writer, List<int> B, List<int> N, double[,] A, double[] cB, double[] cN, double[] b)
        {
            writer.WriteLine("Revised Primal Simplex Algorithm Output:");
            writer.WriteLine("B: " + string.Join(", ", B));
            writer.WriteLine("N: " + string.Join(", ", N));
            writer.WriteLine("A:");
            for (int i = 0; i < B.Count; i++)
            {
                for (int j = 0; j < N.Count; j++)
                {
                    writer.Write($"{A[i, N[j]],10:F3} ");
                }
                writer.WriteLine();
            }
            writer.WriteLine("cB: " + string.Join(", ", cB));
            writer.WriteLine("cN: " + string.Join(", ", cN));
            writer.WriteLine("b: " + string.Join(", ", b));
        }
    }
}
