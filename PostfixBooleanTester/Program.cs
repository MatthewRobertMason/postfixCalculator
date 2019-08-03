using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PostfixBoolean;

namespace PostfixBooleanTester
{
    internal class Program
    {
        public static void testPostFix(string equation)
        {
            try
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(equation);
                BooleanPostfix bp = new BooleanPostfix();
                string postfix = bp.ConvertToPostfix(equation);
                Console.WriteLine(postfix);
                string answer = bp.CalculatePostFix(postfix);
                Console.WriteLine(answer);
                Console.WriteLine("---");

                string infix = bp.ConvertToInfix(postfix);
                Console.WriteLine(infix);
                postfix = bp.ConvertToPostfix(infix);
                Console.WriteLine(postfix);
                answer = bp.CalculatePostFix(postfix);
                Console.WriteLine(answer);
                Console.WriteLine("---");

                infix = bp.ConvertToInfix(postfix);
                Console.WriteLine(infix);
                postfix = bp.ConvertToPostfix(infix);
                Console.WriteLine(postfix);
                answer = bp.CalculatePostFix(postfix);
                Console.WriteLine(answer);
            }
            catch (InvalidOperandException ex)
            {
                Console.WriteLine(String.Format("InvalidOperandException caught: {0}", ex.Message));
            }
            Console.WriteLine();
        }

        private static void Main(string[] args)
        {
            //testPostFix("True AND False");
            //testPostFix("false AND false OR true");
            //testPostFix("false OR false AND true");

            //Console.WriteLine();
            //testPostFix("(false OR true) AND true");
            //testPostFix("false OR (true AND true)");

            Console.WriteLine();
            //testPostFix("NOT ( NOT false AND true )");
            //testPostFix("NOT (true AND  false OR true AND (true OR NOT true))");

            //TestPostFix("true AND false OR false AND true");
            //testPostFix("true OR false AND NOT false OR NOT true");

            //testPostFix("true OR false OR (true AND false)");
            //testPostFix("(((true AND false) AND true) OR false)");
            //testPostFix("( true OR false OR true ) AND false");
            //testPostFix("NOT true OR NOT false AND true OR NOT (false AND true OR false)");
            //testPostFix("false AND true OR true AND false");

            testPostFix("true OR (true AND false)");
            testPostFix("(true OR true) AND false");
            Console.Read();
        }
    }
}