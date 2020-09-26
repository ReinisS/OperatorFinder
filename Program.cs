using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OperatorFinder
{
    class Program
    {
        // Path to the input file containing lists of numbers and targets
        private const string _inputFilePath = "input.txt";

        // Separators by which the numbers are separated in the input file
        private static readonly char[] _separators = new char[] { ' ', ',', '=' };

        private enum Operator
        {
            Plus, // Later represented by '1' in binary number
            Minus // Later represented by '0' in binary number
        }

        /// <summary>
        /// Program entrypoint
        /// </summary>
        static void Main(string[] args)
        {
            // Read lists of numbers from the input file
            var numberLists = GetNumberListsFromLines(GetValidLinesFromFile(_inputFilePath), _separators);

            foreach (List<int> numberList in numberLists)
            {
                var target = TakeTargetFromNumberList(numberList);
                var operatorCount = numberList.Count - 1;

                Console.WriteLine($"Processing list of numbers: {String.Join(", ", numberList)} | Target: {target.ToString()}");

                // Check if the given list of numbers is even worth processing. If not, skip it
                // Doing these checks is worth it because they are currently at most O(n) in complexity, while the main process is O(2^n) in complexity (n - operatorCount)
                if (NumberListIsNotWorthChecking(numberList, operatorCount, target))
                    continue;

                var expressionsFound = new List<string>();

                // Iterate through all of the possible combinations of the operators and evaluate the expression we get in each iteration
                // To get all of the possible combinations, we loop through numbers from 0 to 2^n - 1 and convert the number in each iteration to binary (n - operatorCount)
                // This way we will get all of the possible combinations of 0's and 1's, which we convert to '-' and '+' operators respectively
                // For example, '010' translates to '-+-'
                for (long i = 0; i < Math.Pow(2, operatorCount); i++)
                {
                    var operatorList = ConvertBinaryCombinationToOperators(Convert.ToString(i, 2).PadLeft(operatorCount, '0'));
                    var expressionResult = GetExpressionResult(numberList, operatorList);

                    if (expressionResult == target)
                    {
                        var expressionString = GetExpressionString(numberList, operatorList, target);
                        Console.WriteLine($"Found a match: {expressionString}");
                        expressionsFound.Add(expressionString);
                    }
                }
                if (!expressionsFound.Any())
                    Console.WriteLine("No results found!");
            }
            Console.WriteLine("Finished! Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Reads lines from the given file which are not empty or only whitespace
        /// </summary>
        /// <param name="inputFilePath">Path to the file to read</param>
        /// <returns>List of strings, each corresponding to a line read from the given file</returns>
        private static List<string> GetValidLinesFromFile(string inputFilePath)
        {
            return File.ReadAllLines(inputFilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }

        /// <summary>
        /// Converts the given string list into a list of lists of integers
        /// </summary>
        /// <param name="lines">String list where each string should contain some number of integers with separators in-between</param>
        /// <param name="separators">List of separators by which to split the string into separate integers</param>
        /// <returns>List of lists of integers read from the given string list</returns>
        private static List<List<int>> GetNumberListsFromLines(List<string> lines, char[] separators)
        {
            return lines.Select(
                line => line.Split(separators)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(Int32.Parse)
                    .ToList())
                .ToList();
        }

        /// <summary>
        /// Take the last number (the target) from the given list and remove it from the list so that only the numbers to be processed are left
        /// </summary>
        /// <param name="numberList">List of integers</param>
        /// <returns>The last numbers from the given list (the target)</returns>
        private static int TakeTargetFromNumberList(List<int> numberList)
        {
            var target = numberList.Last();
            numberList.RemoveAt(numberList.Count - 1);
            return target;
        }

        /// <summary>
        /// Check if the given list of numbers is worth checking, given the operator count and the target. All of the checks are currently at most O(n)
        /// </summary>
        /// <param name="numberList">List of numbers from which we're trying to reach the target</param>
        /// <param name="operatorCount">Operator count (should be numberList.Count - 1)</param>
        /// <param name="target">The target we're trying to reach from the given list of numbers</param>
        /// <returns>Returns true if the given target is impossible to reach given the provided list of numbers</returns>
        private static bool NumberListIsNotWorthChecking(List<int> numberList, int operatorCount, int target)
        {
            // At least 2 numbers needed to create any expression with operators
            if (numberList.Count < 2)
            {
                Console.WriteLine("WARNING! Not enough numbers provided (at least 2 needed to create an expression)! Skipping...");
                return true;
            }
            // Check if the parity of the sum (it doesn't matter if we use '+' or '-' operators for checking the parity) of the given numbers equals the parity of the target
            // If the parity doesn't match, then no matter what the operators are, the target cannot be reached
            if (numberList.Sum() % 2 != target % 2)
            {
                Console.WriteLine("WARNING! The parity of the given list of numbers does not match the target's parity, so the target can never be reached! Skipping...");
                return true;
            }
            // Get lowest and highest possible results from the given numbers. If the target is outside of these bounds, the given number list is not worth processing
            // lowerBound - all operators are '-'
            var lowerBound = GetExpressionResult(numberList, Enumerable.Repeat(Operator.Minus, operatorCount).ToList());
            // upperBound - all operators are '+'
            var upperBound = GetExpressionResult(numberList, Enumerable.Repeat(Operator.Plus, operatorCount).ToList());
            if (!(target >= lowerBound && target <= upperBound))
            {
                Console.WriteLine("WARNING! The target is outside of the reachable bounds of the given list of numbers! Skipping...");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Given a binary number string, convert it to a list of <see cref="Operator"/> using the <see cref="ConvertBinaryDigitToOperator"/> method
        /// </summary>
        /// <param name="binaryNumberString">String representing a binary number</param>
        /// <returns>List of <see cref="Operator"/></returns>
        private static List<Operator> ConvertBinaryCombinationToOperators(string binaryNumberString)
        {
            return binaryNumberString.ToCharArray()
                .Select(binaryDigit => ConvertBinaryDigitToOperator(binaryDigit))
                .ToList();
        }

        /// <summary>
        /// Given a character representing a binary digit, convert it to an <see cref="Operator"/>. '0' converts to <see cref="Operator.Minus"/>, '1' converts to <see cref="Operator.Plus"/>
        /// </summary>
        /// <param name="binaryDigit">A character representing a binary digit ('0' or '1')</param>
        /// <returns>An <see cref="Operator"/></returns>
        private static Operator ConvertBinaryDigitToOperator(char binaryDigit)
        {
            return binaryDigit switch
            {
                '1' => Operator.Plus,
                '0' => Operator.Minus,
                _ => throw new ArgumentException($"ERROR! Unrecognised binary digit '{binaryDigit}'!")
            };

        }

        /// <summary>
        /// Evaluate the given list of numbers and list of operators in the order in which they should be applied as an expression and return the result
        /// </summary>
        /// <param name="numberList">List of integers</param>
        /// <param name="operatorList">List of <see cref="Operator"/></param>
        /// <returns>Result of the evaluated expression</returns>
        private static int GetExpressionResult(List<int> numberList, List<Operator> operatorList)
        {
            if (numberList.Count - 1 != operatorList.Count)
                throw new ArgumentException("ERROR! There should be 1 more number than operators!");

            var currentResult = numberList.First();
            for (int i = 0; i < operatorList.Count; i++)
            {
                var currentOperator = operatorList[i];
                var currentNumber = numberList[i + 1];
                switch (currentOperator)
                {
                    case Operator.Plus:
                        currentResult += currentNumber;
                        break;
                    case Operator.Minus:
                        currentResult -= currentNumber;
                        break;
                    default:
                        throw new ArgumentException("ERROR! Unexpected operator found!");
                }
            }
            return currentResult;
        }

        /// <summary>
        /// Convert the given list of numbers, list of operators and the target to a string representation of the expression
        /// </summary>
        /// <param name="numberList">List of integers</param>
        /// <param name="operatorList">List of <see cref="Operator"/></param>
        /// <param name="target">The target integer</param>
        /// <returns>String representation of an expression</returns>
        private static string GetExpressionString(List<int> numberList, List<Operator> operatorList, int target)
        {
            var expressionString = numberList.First().ToString();
            for (int i = 0; i < operatorList.Count; i++)
            {
                var currentOperator = operatorList[i];
                switch (currentOperator)
                {
                    case Operator.Plus:
                        expressionString += " + ";
                        break;
                    case Operator.Minus:
                        expressionString += " - ";
                        break;
                    default:
                        throw new ArgumentException("ERROR! Unexpected operator found!");
                }
                expressionString += numberList[i + 1].ToString();
            }
            return expressionString + $" = {target}";
        }
    }
}
