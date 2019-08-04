using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using postfix_calculator;

namespace PostfixTester
{
    class Program
    {
        public static void RunPostfixTest()
        {
            bool allTestsPassed = true;

            PostFix postfix = new PostFix();
            PostFix booleanPostfix = new PostFix(true);
            postfix.AssignVariable("@PI", "3.14159265358979323846");
            postfix.AssignVariable("@MNum", "16");

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "Sin((@PI+@MNum)-((@MNum*@MNum)/((@MNum))))", "-6.98296672221876E-15");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "-(2 + 4) - (2 - -2)", "-10");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "(-(2 + 4) - (2 - -2)) - -(-(2 + 4) - (2 - -2))", "-20");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "(-(2 + 4) - (2 + 2)) + (-(2 + 4) - (2 + 2)) ", "-20");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "40 + -10 - 3 * 10", "0");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "40 + -10 - 3 * 10", "0");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "1 + 2 / 3 * 4", "3.66666666666667");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "-(-10) -(10 + 10)", "-10");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "-(10 + -(10))", "0");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(postfix, "1 + 1 - 1", "1");
            Console.WriteLine();

            allTestsPassed = allTestsPassed && DetailDebugQuery(booleanPostfix, "true + true - true + false", "true");
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("All Tests passed? " + allTestsPassed);
        }

        public static bool DetailDebugQuery(PostFix postfix, string equation, string expectedOutput)
        {
            try
            {
                string pfEquation = postfix.ConvertToPostfix(equation);
                string inFixModifiedEquation = postfix.ConvertToInfix(pfEquation);

                Console.WriteLine("Equation:   " + equation);
                Console.WriteLine("PostFix:    " + pfEquation);
                Console.WriteLine("InFix:      " + inFixModifiedEquation);
                //Console.WriteLine();

                string answer = postfix.CalculatePostFix(pfEquation);
                Console.WriteLine("Calculated: " + answer);
                if (expectedOutput == answer)
                {
                    Console.WriteLine("Correct");
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed");
                    return false;
                }
            }
            catch (InvalidOperandException ex)
            {
                Console.WriteLine(String.Format("InvalidOperandException caught: {0}", ex.Message));
            }
            catch (InvalidVariableException ex)
            {
                Console.WriteLine(String.Format("InvalidVariableException caught: {0}", ex.Message));
            }
            catch (InvalidEquationException ex)
            {
                Console.WriteLine(String.Format("InvalidVariableException caught: {0}", ex.Message));
            }
            
            return false;
        }

        private static void Main(string[] args)
        {
            //RunPostfixTest();

            Console.Read();
        }
    }
}
