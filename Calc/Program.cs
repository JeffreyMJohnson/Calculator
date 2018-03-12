using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            // App loop.
            while (!_QuittingApplication)
            {
                Update();
            }
        }

        private static bool _QuittingApplication = false;
        private static Calculator calc = new Calculator();

        private static void Update()
        {
            Console.WriteLine("Input a mathematical expression to evaluate, or 'Q' to quit.");
            HandleInput(Console.ReadLine());
        }

        private static void HandleInput(string input)
        {
            if (input == "Q" || input == "q")
            {
                _QuittingApplication = true;
                Console.WriteLine("GoodBye");
                return;
            }

            if (calc.EvaluateExpression(input, out double result))
            {
                Console.WriteLine(result.ToString());
            }
            else
            {
                OutputError();
            }
        }

        private static void OutputError()
        {
            Console.WriteLine("The expression must contain at least 2 valid operands seperated by a single valid operator (+,-,/,*) and end in '=' sign.");
            Console.WriteLine("(eg '2+2*3=')");
        }
    }

    public class Calculator
    {
        private double _previousResult = 0;

        // Evaluates the given expression and assigns the value to result. This returns true if the given expression is valid, otherwise returns false and result will be assigned 0.
        public bool EvaluateExpression(string expression, out double result)
        {
            Expression e = new Expression(expression);

            // if expression failed, might be the special case of an added expression (e.g. '*4+3=' which uses the previous result as the lhs operand.
            if (!e.IsValid)
            {
                string buildExpression = _previousResult.ToString() + expression;
                e = new Expression(buildExpression);
            }

            result = e.IsValid ? e.EvaluatedValue : 0;
            _previousResult = result;
            return e.IsValid;
        }
    }

    public class Expression
    {
        public bool IsValid { get; private set; }
        public double EvaluatedValue { get; private set; }

        private static readonly Regex _validOperandExpression = new Regex(@"^[-]?[0-9]+$");
        private static readonly Regex _validOperatorExpression = new Regex(@"^[+,\-,/,*,=]{1}$");
        private static readonly Dictionary<char, int> _precedenceTable = new Dictionary<char, int>()
        {
            {'=', 0 },
            {'-', 1 },
            {'+', 1 },
            {'*', 2 },
            {'/', 2 },
        };

        public Expression(string expression)
        {
            List<string> tokensList = Tokenize(expression);
            IsValid = Validate(tokensList);

            List<string> rpn = IsValid ? ConvertToRPN(tokensList) : null;
            EvaluatedValue = IsValid ? EvaluateRPN(rpn) : 0;
        }

        public static bool IsValidOperand(string operand)
        {
            return _validOperandExpression.IsMatch(operand);
        }

        public static bool IsValidOperator(string op)
        {
            return _validOperatorExpression.IsMatch(op);
        }

        private double EvaluateRPN(List<string> tokensList)
        {
            Stack<double> outputStack = new Stack<double>();

            foreach (var token in tokensList)
            {
                if (IsValidOperand(token))
                {
                    outputStack.Push(Convert.ToDouble(token));
                }
                else if (token == "=")
                {
                    // ignore '=' sign when evaluating.
                    continue;
                }
                else
                {
                    double rhs = outputStack.Pop();
                    double lhs = outputStack.Pop();
                    double retVal = 0;

                    switch (token)
                    {
                        case "+":
                            retVal = lhs + rhs;
                            break;
                        case "-":
                            retVal = lhs - rhs;
                            break;
                        case "/":
                            retVal = lhs / rhs;
                            break;
                        case "*":
                            retVal = lhs * rhs;
                            break;
                    }
                    outputStack.Push(retVal);
                }
            }

            return outputStack.Pop();
        }

        private bool Validate(List<string> tokensList)
        {
            // min expression is operand-operator-operand-equals
            if (tokensList.Count < 4 || tokensList.Count % 2 != 0)
            {
                return false;
            }

            for (int i = 0; i <  tokensList.Count; ++i)
            {
                if (i % 2 == 0)
                {
                    if (!IsValidOperand(tokensList[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!IsValidOperator(tokensList[i]))
                    {
                        return false;
                    }

                    //last token should be equals
                    if (i == tokensList.Count - 1 && tokensList[i] != "=")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Returns list with given expression tokens.
        private List<string> Tokenize(string expression)
        {
            List<string> result = new List<string>();

            string token = "";

            foreach (var c in expression)
            {
                //skip whitespace
                if (c == ' ')
                {
                    continue;
                }

                if (IsValidOperator(c.ToString()))
                {
                    //neg operand edge case
                    if (c != '-' || token.Length > 0)
                    {
                        // don't add the token if it is empty
                        if (token.Length > 0)
                        {
                            result.Add(token);
                        }
                        result.Add(c.ToString());
                        token = "";
                        continue;
                    }
                }
                token += c;
            }

            if (token.Length > 0)
            {
                result.Add(token);
            }

            return result;
        }

        // returns given tokensList converted to a tokensList in reverse polish notation.
        private List<string> ConvertToRPN(List<string> tokensList)
        {
            List<string> output = new List<string>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (var token in tokensList)
            {
                if (IsValidOperand(token))
                {
                    output.Add(token);
                }
                else
                {
                    // while (top of operator stack has greater precedence) OR (operator at top of stack has equal precedence)
                    while (operatorStack.Count > 0 && CompareOperatorPrecedence(operatorStack.Peek()[0], token[0]) >= 0)
                    {
                       output.Add(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                }
            }

            //while there are still operator tokens on operator stack
           while(operatorStack.Count > 0)
            {
                output.Add(operatorStack.Pop());
            }

            return output;
        }
        
        private int CompareOperatorPrecedence(char lhs, char rhs)
        {
            if (_precedenceTable[lhs] == _precedenceTable[rhs])
            {
                return 0;
            }

            return _precedenceTable[lhs] > _precedenceTable[rhs] ? 1 : -1;
        }
    }
}
