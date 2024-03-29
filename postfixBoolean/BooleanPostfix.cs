﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PostfixBoolean
{
    public class BooleanPostfix
    {
        //private Dictionary<string, object> variables;

        public string CalculatePostFix(string postFixEquation)
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

        public string ConvertToInfix(string postfixEquation)
        {
            // lowest precedence, formula
            Stack<Tuple<int, string>> sEquation = new Stack<Tuple<int, string>>();

            foreach (string s in postfixEquation.Split(' '))
            {
                if (IsOperator(s))
                {
                    string item = "";
                    int precedence = -1;

                    switch (OperatorParameterNumber(s))
                    {
                        case 1:
                            precedence = Math.Max(OpPrecedence(s), sEquation.Peek().Item1);
                            if ((sEquation.Peek().Item1 <= OpPrecedence(s)) || (sEquation.Peek().Item1 == -1))
                            {
                                item = string.Format("{0} {1}", s, sEquation.Pop().Item2);
                            }
                            else
                            {
                                item = string.Format("{0} ({1})", s, sEquation.Pop().Item2);
                            }

                            break;

                        case 2:
                            Tuple<int, string> i2 = sEquation.Pop();
                            Tuple<int, string> i1 = sEquation.Pop();
                            precedence = Math.Max(Math.Max(OpPrecedence(s), i1.Item1), i2.Item1);
                            int opPrec = OpPrecedence(s);

                            if ((i1.Item1 == -1) && (i2.Item1 == -1))
                            {
                                item = string.Format("{0} {1} {2}", i1.Item2, s, i2.Item2);
                            }
                            else if (i1.Item1 > opPrec)
                            {
                                if (i2.Item1 > opPrec)
                                {
                                    item = string.Format("({0}) {1} ({2})", i1.Item2, s, i2.Item2);
                                }
                                else
                                {
                                    item = string.Format("({0}) {1} {2}", i1.Item2, s, i2.Item2);
                                }
                            }
                            else if (i2.Item1 > opPrec)
                            {
                                item = string.Format("{0} {1} ({2})", i1.Item2, s, i2.Item2);
                            }
                            else
                            {
                                item = string.Format("{0} {1} {2}", i1.Item2, s, i2.Item2);
                            }
                            break;

                        default:
                            // TODO: some sort of exception
                            break;
                    }

                    sEquation.Push(new Tuple<int, string>(precedence, item));
                }
                else
                {
                    sEquation.Push(new Tuple<int, string>(-1, s));
                }
            }

            return sEquation.Pop().Item2;
        }

        public string ConvertToPostfix(string equation)
        {
            try
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
                        item = item.ToUpper();

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
                            }
                        }
                    }
                    else
                    {
                        item = item.ToLower();

                        if ((item == "true") || (item == "false"))
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
            catch
            {
                throw new InvalidEquationException("Invalid equation");
            }
        }

        private string Calculate(string op, params string[] operands)
        {
            string sReturn = null;

            switch (op)
            {
                case "NOT":
                    {
                        bool.TryParse(operands[0], out bool o1);
                        sReturn = (!o1).ToString();
                        break;
                    }
                case "AND":
                    {
                        bool.TryParse(operands[0], out bool o1);
                        bool.TryParse(operands[1], out bool o2);
                        sReturn = (o1 && o2).ToString();
                        break;
                    }
                case "OR":
                    {
                        bool.TryParse(operands[0], out bool o1);
                        bool.TryParse(operands[1], out bool o2);
                        sReturn = (o1 || o2).ToString();
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

            string test = op.ToUpper();

            if ((test.Last() == '(') ||
                (test == ")") ||
                (test == "NOT") ||
                (test == "AND") ||
                (test == "OR")
                )
            {
                return true;
            }

            return false;
        }

        private int OperatorParameterNumber(string op)
        {
            string[] _1op = { "NOT" };
            string[] _2op = { "AND", "OR" };

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
                case ")": { return 1; }
                case "NOT": { return 2; }
                case "AND": { return 3; }
                case "OR": { return 4; }

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
            equation = equation.Replace("NOT", "NOT ");
            equation = equation.Replace("AND", " AND ");
            equation = equation.Replace("OR", " OR ");

            return equation;
        }
    }
}