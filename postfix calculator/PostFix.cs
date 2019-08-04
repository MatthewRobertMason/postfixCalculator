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
        private bool BooleanMode = false;

        private const string OPERAND_REGEX = "(^true$)|(^false$)|([0-9]*)|(@[a-zA-Z]*)|(\\$[a-zA-Z]*)"; //"[0-9@$].*";
        private Dictionary<string, string> _variables;
        
        private Dictionary<string, string> Variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new Dictionary<string, string>();
                }

                return _variables;
            }
        }

        public PostFix(bool BooleanMode = false) 
        {
            this.BooleanMode = BooleanMode;
        }

        public void AssignVariable(string name, string value)
        {
            if (Variables.Keys.Contains(name.ToLower()))
            {
                throw new InvalidVariableException(string.Format("AssignVariable(): Variable Already Exists ({0})", name));
            }

            Variables.Add(name.ToLower(), value);
        }

        /// <summary>
        /// If var is a variable retrieve the value, otherwise return var
        /// </summary>
        /// <param name="var">The variable or value to examine</param>
        /// <returns>Value if a value or variable, InvalidVariableException if invalid variable</returns>
        public string TryGetVariable(string var)
        {
            if (var.First() == '@')
            {
                string temp = "";
                if (Variables.TryGetValue(var, out temp))
                {
                    var = temp;
                    return var;
                }
                else
                {
                    throw new InvalidVariableException(string.Format("Calculate(): Invalid Variable ({0}), Possibly not set.", var));
                }
            }

            return var;
        }

        public string CalculatePostFix(string postFixEquation)
        {
            try
            {
                Stack<string> operandStack = new Stack<string>();

                List<string> sEquation = postFixEquation.Split(' ').ToList();

                foreach (string item in sEquation)
                {
                    string tempOp = item.ToLower();
                    if (IsOperator(tempOp))
                    {
                        int opParams = OperatorParameterNumber(tempOp);

                        if (operandStack.Count >= OperatorParameterNumber(tempOp))
                        {
                            string[] p = new string[opParams];

                            for (int i = 0; i < opParams; i++)
                            {
                                p[i] = operandStack.Pop();
                            }

                            operandStack.Push(Calculate(tempOp, p.Reverse().ToArray()));
                        }
                        else
                        {
                            // TODO: use a better exception
                            throw new Exception("This is Garbage!");
                        }
                    }
                    else
                    {
                        operandStack.Push(tempOp);
                    }
                }

                //return new Tuple<Type, object>(typeof(double), 0.0);
                if (operandStack.Count > 0)
                {
                    if (BooleanMode)
                    {
                        string value = operandStack.Pop();
                        double dValue = 0.0;

                        double.TryParse(value, out dValue);
                        if (dValue != 0.0)
                            return "true";
                        else
                            return "false";
                    }

                    return operandStack.Pop();
                }
                else
                    throw new InvalidEquationException(string.Format("CalculatePostFix(): Invalid Equation: ({0})", postFixEquation));
            }
            catch (InvalidOperandException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidEquationException(string.Format("CalculatePostFix(): Invalid Equation: {0}", ex.Message));
            }
        }

        public string ConvertToPostfix(string initialEquation)
        {
            try
            {
                string[] equation = PrepareEquation(initialEquation);
                
                String output = "";
                Stack<string> opstack = new Stack<string>();
                
                foreach (string s in equation)
                    {
                    string item = s.ToLower().Trim();

                    if (item == "")
                    {
                        continue;
                    }

                    if (IsOperator(item))
                    {
                        if (item != ")")
                        {
                            // If the token is a left paren
                            if (item == "(")
                            {
                                opstack.Push(item);
                            }
                            else
                            {
                                //bool test = false;
                                //string rhs = "";

                                /*
                                 while ((there is a function at the top of the operator stack)
                                       or (there is an operator at the top of the operator stack with greater precedence)
                                       or (the operator at the top of the operator stack has equal precedence and is left associative))
                                      and (the operator at the top of the operator stack is not a left parenthesis):
                                    pop operators from the operator stack onto the output queue.
                                push it onto the operator stack.
                                */
                                if (opstack.Count > 0)
                                {
                                    while ( (opstack.Count > 0) && 
                                              (
                                                  (OpPrecedence(item) >= OpPrecedence(opstack.Peek())) 
                                                  && (opstack.Peek().Trim() != "(")
                                              ) 
                                          )
                                    {
                                        output += " " + opstack.Pop();
                                    }
                                }

                                opstack.Push(item);
                            }
                        }
                        else
                        {
                            // If the token is a right paren

                            while (opstack.Peek().Trim() != "(")
                            {
                                output += " " + opstack.Pop();
                            }

                            if (opstack.Peek().Trim() == "(")
                            {
                                opstack.Pop();
                            }

                            //string popped = " ";
                            //while (popped.Last() != '(')
                            //{
                            //    popped = opstack.Pop();
                            //    //if (popped != "(")
                            //    if (popped.Last() != '(')
                            //    {
                            //        output += " " + popped;
                            //    }
                            //
                            //    //else if (popped.Length > 1)
                            //    //{
                            //    //    output += " " + popped.Substring(0, popped.Length - 1);
                            //    //}
                            //}
                        }
                    }
                    else
                    {
                        // If the token is a value

                        if (Regex.Match(item, OPERAND_REGEX).Success)
                        {
                            output += " " + item;
                        }
                        else
                        {
                            throw new InvalidOperandException(string.Format("ConvertToPostfix(): Invalid operand in expresssion", item));
                        }
                    }
                }

                while (opstack.Count > 0)
                {
                    output += " " + opstack.Pop();
                }

                return output.Trim();
            }
            catch (InvalidOperandException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidEquationException(string.Format("Invalid Equation: {0}", ex.Message));
            }
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

            string infix = sEquation.Pop().Item2;
            infix = infix.Replace("?.. ", "-");

            return infix;
            //return sEquation.Pop().Item2;
        }

        private string Calculate(string op, params string[] operands)
        {
            string sReturn = null;


            List<string> valList = new List<string>();
            foreach(string var in operands)
            {
                string temp = TryGetVariable(var);
                
                if (temp.Trim() == "true")
                    temp = "1";
                if (temp.Trim() == "false")
                    temp = "0";

                valList.Add(temp);
            }
            string[] values = valList.ToArray();
            
            switch (op)
            {
                case "!":
                    {
                        bool o1;
                        bool.TryParse(values[0], out o1);
                        sReturn = (!o1).ToString();
                        break;
                    }
                case "~":
                    {
                        int o1;
                        int.TryParse(values[0], out o1);
                        sReturn = (~o1).ToString();
                        break;
                    }
                case "?..":
                    {
                        double o1;
                        double.TryParse(values[0], out o1);
                        sReturn = (-o1).ToString();
                        break;
                    }
                case "*":
                    {
                        double o1, o2;
                        double.TryParse(values[0], out o1);
                        double.TryParse(values[1], out o2);
                        sReturn = (o1 * o2).ToString();
                        break;
                    }
                case "/":
                    {
                        double o1, o2;
                        double.TryParse(values[0], out o1);
                        double.TryParse(values[1], out o2);
                        sReturn = (o1 / o2).ToString();
                        break;
                    }
                case "%":
                    {
                        int o1, o2;
                        int.TryParse(values[0], out o1);
                        int.TryParse(values[1], out o2);
                        sReturn = (o1 % o2).ToString();
                        break;
                    }
                case "+":
                    {
                        double o1, o2;
                        double.TryParse(values[0], out o1);
                        double.TryParse(values[1], out o2);
                        sReturn = (o1 + o2).ToString();
                        break;
                    }
                case "-":
                    {
                        double o1, o2;
                        double.TryParse(values[0], out o1);
                        double.TryParse(values[1], out o2);
                        sReturn = (o1 - o2).ToString();
                        break;
                    }
                case "sin":
                    {
                        double o1;
                        double.TryParse(values[0], out o1);
                        sReturn = Math.Sin(o1).ToString();
                        break;
                    }

                default:
                    {
                        throw new InvalidOperandException(string.Format("Calculate(): Invalid Operand ({0})", op));
                        //sReturn = null; // Unknown operator
                        //break;
                    }
            }
            return sReturn;
        }

        private bool IsOperator(string op)
        {
            if (op == null)
                return false;

            string[] ops = { ")", "sin", "!", "?..", "+", "-", "*", "/", "%" };

            if ((ops.Any(p => p == op)) || (op.Last() == '('))
            {
                return true;
            }

            if (Regex.Match(op, OPERAND_REGEX).Success)
            {
                return false;
            }

            throw new InvalidOperandException(string.Format("IsOperator(): Invalid Operand ({0})", op));
        }

        private int OperatorParameterNumber(string op)
        {
            string _op = op.ToLower();
            string[] _ignoreOp = { "(", ")" };
            string[] _1op = { "sin", "!", "?.." };
            string[] _2op = { "+", "-", "*", "/", "%" };

            if (_ignoreOp.Any(p => p == op))
            {
                return -1;
            }

            if (_1op.Any(p => p == op))
            {
                return 1;
            }

            if (_2op.Any(p => p == op))
            {
                return 2;
            }

            throw new InvalidOperandException(string.Format("OperatorParameterNumber(): Invalid Operand ({0})", op));
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
                case "sin":
                    {
                        return 1;
                    }

                case "!":
                case "~":
                case "?..":
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
                        throw new InvalidOperandException(string.Format("OpPrecedence(): Invalid Operand ({0})", op));
                    }
            }
        }

        private string[] PrepareEquation(string equation)
        {
            equation = equation.ToLower();
            equation = equation.Replace("(", "( ");
            equation = equation.Replace(")", " ) ");
            equation = equation.Replace("*", " * ");
            equation = equation.Replace("/", " / ");
            equation = equation.Replace("+", " + ");
            equation = equation.Replace("-", " - ");
            equation = equation.Replace("%", " % ");
            
            equation = equation.Replace("sin", " sin ");

            List<string> splitString = new List<string>(equation.Trim().Split(' '));
            List<string> newEquation = new List<string>();
            
            int i = 0;
            //for (int i = 0; i < splitString.Length; i++)
            while (i < splitString.Count)
            {
                if (i == 0)
                {
                    if (splitString[i] == "-")
                    {
                        splitString[i] = "?..";
                    }
                }
                else if ((splitString[i] == "-") && (splitString[i - 1] != ")") && (IsOperator(splitString[i - 1])))
                {
                    splitString[i] = "?..";
                } 

                if (splitString[i].Trim() != "")
                {
                    newEquation.Add(splitString[i]);
                    i++;
                }
                else
                {
                    splitString.RemoveAt(i);
                }
            }

            return newEquation.ToArray();
        }
    }
}