# LPR381_Project_GroupV1

# Operations Research Programming Project

## Introduction
Operations research is a scientific approach to decision making that seeks to best design and operate a system, under conditions requiring the allocation of scarce resources. This approach usually involves the use of one or more mathematical models. If the decision variables in an optimization model are always multiplied by constants and added together, the model is linear. If one or more decision variables must be integer, the optimization model is discrete or an integer model.

## Project Overview
This project involves creating a program that solves Linear Programming (LP) and Integer Programming (IP) models and analyzes how changes in an LP’s parameters affect the optimal solution. The project should be built as a Visual Studio project using any .NET programming language. The final executable (solve.exe) should be menu-driven and meet the following criteria:

### Minimum Requirements
- Accept a random amount of decision variables.
- Accept a random amount of constraints.
- Use comments in the code.
- Implement programming best practices.

### Input Text File Criteria
- **First line**: Indicates if it is a maximization or minimization problem (max or min), followed by the objective function coefficients.
- **Constraints**: Each line specifies the coefficients for the decision variables, the relation used (e.g., =, <=, >=), and the right-hand side of the constraint.
- **Sign Restrictions**: Below all constraints, specify sign restrictions (+, -, urs, int, bin) in the same order as the objective function.

#### Example of Input File

### Processing Requirements
- Option to select which algorithm to use for solving the programming model.
- Options for performing sensitivity analysis operations after solving the model.

### Programming Model Criteria
- Solve normal max Linear Programming Models (specifically the given Knapsack IP).
- Solve binary Integer Programming Models (specifically the given Knapsack IP).

### Algorithms to be Available
- Primal Simplex Algorithm and Revised Primal Simplex Algorithm.
- Branch and Bound Simplex Algorithm or Revised Branch and Bound Simplex Algorithm.
- Cutting Plane Algorithm or Revised Cutting Plane Algorithm.
- Branch and Bound Knapsack algorithm.

### Algorithm Criteria
- Display the Canonical Form and solve using the Primal Simplex Algorithm. Display all tableau iterations.
- Display the Canonical Form and solve using the Revised Primal Simplex Algorithm. Display all Product Form and Price Out iterations.
- Display the Canonical Form and solve using the Branch & Bound Simplex Algorithm.
  - Implement backtracking.
  - Create all possible sub-problems to branch on.
  - Fathom all possible nodes of sub-problems.
  - Display all tableau iterations of sub-problems.
  - Display the best candidate.
- Display the Canonical Form and solve using the Cutting Plane Algorithm. Display all Product Form and Price Out iterations.
- Solve using Branch and Bound Knapsack algorithm.
  - Implement backtracking.
  - Create all possible sub-problems to branch on.
  - Fathom all possible nodes of sub-problems.
  - Display all tableau iterations of sub-problems.
  - Display the best candidate.

### Output File Format
- Contain the Canonical form and all tableau iterations of the algorithm selected to solve the Programming Model.
- Round all decimal values to three points.

### Sensitivity Analysis Criteria
- Display and apply changes for selected Non-Basic and Basic Variables.
- Display and apply changes for selected constraint right-hand-side values.
- Add a new activity or constraint to an optimal solution.
- Display shadow prices.
- Apply and solve Dual Programming Model.
- Verify whether the Programming Model has Strong or Weak Duality.

### Special Case Requirements
- Identify and resolve infeasible or unbounded programming models.

### Bonus Criteria
- Solve any non-linear problem like \( f(x)=x^2 \) with any algorithm and explain the code for this part.

## Submission
- Submit a video or link to a video demonstrating the program's functionality. Files necessary to run the program should be included. Marks will be awarded based on the video demonstration.

## Subject: Linear Programming 381
## Assessment: Project
## Total: 100 Marks
