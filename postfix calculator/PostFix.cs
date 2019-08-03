using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace postfix_calculator
{
    public class PostFix
    {
        //private Dictionary<string, object> variables;

        public /*Tuple<Type, object>*/ string CalculatePostFix(string postFixEquation)
        {
            Stack<string> operandStack = new Stack<string>();

            List<string> sEquation = postFixEquation.Split(' ').ToList();

            foreach (string item in sEquation)
            {
                if (IsOperator(item))
                {
                    int opParams = OperatorParameterNumber(item);

                    if (operandStack.Count >= OperatorParameterNumber(item))
                    {
                        string[] p = new string[opParams];

                        for (int i = 0; i < opParams; i++)
                        {
                            p[i] = operandStack.Pop();
                        }

                        operandStack.Push(Calculate(item, p.Reverse().ToArray()));
                    }
                    else
                    {
                        // TODO: use a better exception
                        throw new Exception("This is Garbage!");
                    }
                }
                else
                {
                    operandStack.Push(item);
                }
            }

            //return new Tuple<Type, object>(typeof(double), 0.0);
            if (operandStack.Count > 0)
                return operandStack.Pop();
            else
                return null;
        }

        public string ConvertToPostfix(string equation)
        {
            equation = PrepareEquation(equation);

            String output = "";
            Stack<string> opstack = new Stack<string>();

            foreach (string s in equation.Split(' '))
            {
                string item = s.Trim();

                if (item == "")
                {
                    continue;
                }

                if (IsOperator(item))
                {
                    if (item != ")")
                    {
                        if (item == "(")
                        {
                            opstack.Push(item);
                        }
                        else
                        {
                            bool test = false;
                            string rhs = "";

                            while (test == false)
                            {
                                if (opstack.Count > 0)
                                {
                                    rhs = opstack.Peek();

                                    if ((OpPrecedence(rhs) > 1) && (OpPrecedence(item) >= OpPrecedence(rhs)))
                                    {
                                        output += " " + opstack.Pop();
                                    }
                                    else
                                    {
                                        test = true;
                                    }
                                }
                                else
                                {
                                    test = true;
                                }
                            }

                            opstack.Push(item);
                        }
                    }
                    else
                    {
                        string popped = " ";
                        while (popped.Last() != '(')
                        {
                            popped = opstack.Pop();
                            //if (popped != "(")
                            if (popped.Last() != '(')
                            {
                                output += " " + popped;
                            }

                            if (popped.Length > 1)
                            {
                                output += " " + popped.Substring(0, popped.Length - 1);
                            }
                        }
                    }
                }
                else
                {
                    if (Regex.Match(item, "[0-9@$].*").Success)
                    {
                        output += " " + item;
                    }
                    else
                    {
                        throw new InvalidOperandException("Invalid operand in expresssion");
                    }
                }
            }

            while (opstack.Count > 0)
            {
                output += " " + opstack.Pop();
            }

            return output.Trim();
        }

        private string Calculate(string op, params string[] operands)
        {
            string sReturn = null;

            switch (op)
            {
                case "!":
                    {
                        bool o1;
                        bool.TryParse(operands[0], out o1);
                        sReturn = (!o1).ToString();
                        break;
                    }
                case "~":
                    {
                        int o1;
                        int.TryParse(operands[0], out o1);
                        sReturn = (~o1).ToString();
                        break;
                    }
                case "*":
                    {
                        double o1, o2;
                        double.TryParse(operands[0], out o1);
                        double.TryParse(operands[1], out o2);
                        sReturn = (o1 * o2).ToString();
                        break;
                    }
                case "/":
                    {
                        double o1, o2;
                        double.TryParse(operands[0], out o1);
                        double.TryParse(operands[1], out o2);
                        sReturn = (o1 / o2).ToString();
                        break;
                    }
                case "%":
                    {
                        int o1, o2;
                        int.TryParse(operands[0], out o1);
                        int.TryParse(operands[1], out o2);
                        sReturn = (o1 % o2).ToString();
                        break;
                    }
                case "+":
                    {
                        double o1, o2;
                        double.TryParse(operands[0], out o1);
                        double.TryParse(operands[1], out o2);
                        sReturn = (o1 + o2).ToString();
                        break;
                    }
                case "-":
                    {
                        double o1, o2;
                        double.TryParse(operands[0], out o1);
                        double.TryParse(operands[1], out o2);
                        sReturn = (o1 - o2).ToString();
                        break;
                    }
                case "Sin":
                    {
                        double o1;
                        double.TryParse(operands[0], out o1);
                        sReturn = Math.Sin(o1).ToString();
                        break;
                    }

                default:
                    {
                        sReturn = null; // Unknown operator
                        break;
                    }
            }
            return sReturn;
        }

        private bool IsOperator(string op)
        {
            if (op == null)
                return false;

            string test = op.ToLower();

            if ((test == "+") ||
                (test == "-") ||
                (test == "/") ||
                (test == "*") ||
                (test.Last() == '(') ||
                (test == ")") ||
                (test == "%") ||
                (test == "sin")
                )
            {
                return true;
            }

            return false;
        }

        private int OperatorParameterNumber(string op)
        {
            string[] _1op = { "Sin", "!" };
            string[] _2op = { "+", "-", "*", "/", "%" };

            if (_1op.Any(p => p == op))
            {
                return 1;
            }

            if (_2op.Any(p => p == op))
            {
                return 2;
            }

            // TODO: throw some sort of error
            return -1;
        }

        private int OpPrecedence(string op)
        {
            string test = op;
            if (op.Reverse().First() == '(')
            {
                test = "(";
            }

            switch (test)
            {
                case "(":
                case ")":
                    {
                        return 1;
                    }

                case "!":
                case "~":
                    {
                        return 2;
                    }

                case "*":
                case "/":
                case "%":
                    {
                        return 3;
                    }

                case "+":
                case "-":
                    {
                        return 4;
                    }

                default:
                    {
                        return -1; // Unknown precedence or an error
                    }
            }
        }

        private string PrepareEquation(string equation)
        {
            equation = equation.Replace("(", "( ");
            equation = equation.Replace(")", " ) ");
            equation = equation.Replace("*", " * ");
            equation = equation.Replace("/", " / ");
            equation = equation.Replace("+", " + ");
            equation = equation.Replace("-", " - ");
            equation = equation.Replace("%", " % ");

            return equation;
        }
    }
}