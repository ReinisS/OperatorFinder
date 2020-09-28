# OperatorFinder

## Introduction

This console program finds all of the possible combinations of '+' and '-' operators which can be used in-between the given lists
of numbers to reach the given targets. The input numbers are read from the `inputs.txt` file and can be separated by spaces,
commas or equal signs. The last number in each line is considered to be the target number.

Example input: `17 3 2 5 = 23`

Output: `17 + 3 - 2 + 5 = 23`

An example input file has been provided in the repository.

In general the code complexity here is `O(2^n)`, where  `n` is the number of missing operators to find,
because all of the possible combinations are checked. However, some `O(n)` checks are used before evaluation
to check if the given list of numbers is even worth evaluating.

This console program is built using the `.NET Core 3.1` framework.

## Building the project

To build the project, .NET Core 3.1 compatible SDK is required. More information here: <https://dotnet.microsoft.com/download>

To build the project run `dotnet build` from the project root folder.

To publish, run `dotnet publish`.

To run the source code, run `dotnet run` command from the project root folder.

## Running the project

A Windows x64 executable file has also been included in `executables/OperatorFinder.exe`. To run it, a
.NET Core 3.1 compatible runtime is required. More information here: <https://dotnet.microsoft.com/download>

The files in the `executables` folder have been published using the command:

`dotnet publish -c Release -f netcoreapp3.1 -r win-x64 -o  output --self-contained false`
