using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StackBasedCalculator
{
    //Author: Kurt Carpenter
    //Date: 1/26/2015
    //Version: 1.0
    //Stack-based calculator, using custom array-backed stack and dictionary classes.
    //Input a standard (infix) integer/floating point math expression using the four basic operations,
    //parentheses, and exponents. Converts to Reverse Polish Rotation, calculates using a stack,
    //and outputs result.
    class Program
    {
        private const string REGEX_INPUT = "^([0-9]|[x\\+\\-\\/\\(\\)\\*\\^\\.\\s])*$";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Kurt Carpenter's Stack-Based Calculator");
            Console.WriteLine("**************************************************");
            Console.WriteLine("Please press <Enter> to proceed");
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
            Console.Clear();

            bool ready = true;

            while (ready)
            {
                string input = CollectExpression();
                Console.Clear();
                input = SpaceValues(input);
                Console.WriteLine("Input: " + input);
                input = ConvertInfixToPostfix(input);
                Console.WriteLine("Reverse Polish Notation: " + input);
                double result = -1;
                try
                {
                    result = EvaluatePostfixExpression(input);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
                Console.WriteLine("Result: " + result);
                Console.WriteLine();
                Console.WriteLine("Press <Enter> to use again or enter any other key to quit");
                while (true)
                {
                    if (Console.ReadKey().Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        break;
                    }
                    else
                        ready = false;
                }
            }            
        }

        //Returns the string defined by user
        private static string CollectExpression()
        {
            string input;
            Regex inputValidator = new Regex(REGEX_INPUT);
            while (true)
            {
                Console.WriteLine("Please enter a math expression, then press <Enter>");
                input = Console.ReadLine();
                if (inputValidator.IsMatch(input) && ParensMatch(input))
                {
                    break;
                }
                else if (!inputValidator.IsMatch(input))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input! Allowed characters are 0-9 + - / x * ^ ( )");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input! Mismatched parens");
                }
            }
            return input;
        }

        //Returns true if all parens are matched and properly closed
        //Uses an ArrayStack to match parens
        private static bool ParensMatch(string input)
        {
            ArrayStack<char> parensStack = new ArrayStack<char>();
            foreach (char c in input)
            {
                if (c.CompareTo('(') == 0)
                {
                    parensStack.Push(c);
                }
                else if (c.CompareTo(')') == 0)
                {
                    if (parensStack.IsEmpty() || parensStack.Pop().CompareTo('(') != 0)
                    {
                        return false;
                    }
                }
            }
            if (!parensStack.IsEmpty())
            {
                return false;
            }
            return true;
        }

        //Given a string denoting an infix math expression, converts to postfix
        //Uses Dijkstra's Shunting-Yard Algorithm
        private static string ConvertInfixToPostfix(string input)
        {
            string[] values = input.Split(' ');
            StringBuilder rpn = new StringBuilder(input.Length);
            ArrayDictionary<string, int> operators = new ArrayDictionary<string, int>(5);
            operators.Add("^", 4);
            operators.Add("*", 3);
            operators.Add("x", 3);
            operators.Add("/", 3);
            operators.Add("+", 2);
            operators.Add("-", 2);
            operators.Add("(", -1);
            operators.Add(")", -1);
            ArrayStack<string> operatorStack = new ArrayStack<string>();
            
            foreach (string item in values)
            {
                if(item.Equals(""))
                {
                    break;
                }
                if (operators.ContainsKey(item)) //item is an operator
                {
                    if (operatorStack.Size == 0) //stack is empty, we push operator to stack
                    {
                        operatorStack.Push(item);
                        continue;
                    }
                   
                    if (item.Equals(")")) //we have a right paren
                    {
                        bool found = false;
                        while (!operatorStack.IsEmpty()) //searching for left paren
                        {
                            if (operatorStack.Peek().Equals("("))
                            {
                                found = true;
                                operatorStack.Pop();
                                break;
                            }
                            else
                            {
                                rpn.Append(operatorStack.Pop());
                                rpn.Append(" ");
                            }
                        }
                        if (!found && operatorStack.IsEmpty()) //stack empty, no left paren in sight
                        {
                            throw new ArgumentException("Mismatched parens");
                        }
                    }
                    else if (item.Equals("(")) //left parens always added to stack
                    {
                        operatorStack.Push(item);
                    }
                    else //we have an operator
                    {
                        if (item.Equals("^") && operators[item] < operators[operatorStack.Peek()])
                        {
                            rpn.Append(operatorStack.Pop());
                            rpn.Append(" ");
                        }
                        else if (operators[item] <= operators[operatorStack.Peek()])
                        {
                            rpn.Append(operatorStack.Pop());
                            rpn.Append(" ");
                        }                        
                        operatorStack.Push(item);
                    }                    
                }
                else //item is a number
                {
                    double n;
                    if (! double.TryParse(item, out n))
                        throw new FormatException("Invalid input of number");
                    rpn.Append(n);
                    rpn.Append(" ");
                }                
            }
            while (!operatorStack.IsEmpty()) //we're done, dump stack onto output
            {
                if (operatorStack.Peek().Equals(")") || operatorStack.Peek().Equals("("))
                    throw new ArgumentException("Mismatched parens");
                rpn.Append(operatorStack.Pop());
                rpn.Append(" ");
            }

            return rpn.ToString().Trim();
        }

        //Evaluates a math expression in Postfix notation
        private static double EvaluatePostfixExpression(string rpn)
        {
            string[] values = rpn.Split(' ');
            ArrayStack<object> calculationStack = new ArrayStack<object>();
            foreach (string s in values)
            {
                double n;
                if (double.TryParse(s, out n))
                {
                    calculationStack.Push(n);
                }
                else //if (leftOperators.Contains(s))
                {
                    try
                    {
                        double right = double.Parse(calculationStack.Pop().ToString());
                        double left = double.Parse(calculationStack.Pop().ToString());
                    
                        switch (s)
                        {
                            case "+":
                                calculationStack.Push((left + right).ToString());
                                break;
                            case "-":
                                calculationStack.Push((left - right).ToString());
                                break;
                            case "/":
                                calculationStack.Push((left / right).ToString());
                                break;
                            case "*":
                                calculationStack.Push((left * right).ToString());
                                break;
                            case "x":
                                calculationStack.Push((left * right).ToString());
                                break;
                            case "^":
                                calculationStack.Push((Math.Pow(left, right)).ToString());
                                break;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new FormatException("Invalid format. Please avoid double negation (--5) and rewrite negative numbers as 0 - n.");
                    }
                }
            }
            return double.Parse(calculationStack.Pop().ToString());
        }

        //Input a math expression
        //Output string of space-seperated values
        private static string SpaceValues(string expression)
        {
            StringBuilder stringy = new StringBuilder(); //used for constructing the return value
            string operators = "+-/*x^()"; //list of non-numeric characters. Period counts as numeric
            char[] chars = expression.ToCharArray();
            bool processingNumber = false; //are we currently processing a number?
            for (int i = 0; i < chars.Length; i++)
            {
                if (operators.Contains(chars[i])) //we have an operator
                {
                    if (processingNumber) //we were processing a number but are not anymore
                    {
                        stringy.Append(" ");
                        processingNumber = false;
                    }
                    stringy.Append(chars[i]);
                    stringy.Append(" ");
                }
                else if (chars[i].Equals("") || chars[i].Equals(' ') || chars[i].Equals('\t')) //we have whitespace or tab
                {
                    continue;
                }
                else //we have a number or period
                {
                    stringy.Append(chars[i]);                    
                    processingNumber = true;
                }
            }
            return stringy.ToString().Trim();
        }
    }
}
