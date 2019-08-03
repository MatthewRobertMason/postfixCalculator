using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace postfix_calculator
{
    internal class Program
    {
        public static void testPostFix(string equation)
        {
            PostFix postfix = new PostFix();

            try
            {
                Console.WriteLine(equation);
                string pfEquation = postfix.ConvertToPostfix(equation);
                Console.WriteLine(pfEquation);

                string answer = postfix.CalculatePostFix(pfEquation);
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
            //testPostFix("A + B * (1 + 2)");
            //testPostFix("(A+B)*(C+D)/(E-F)");
            //testPostFix("(@var1+@var2)*(@var3+158)/(@var4-32.64)");
            //testPostFix("(3.14159+0.0000005)*(Sin(2*@PI))");
            //testPostFix("A * B + C * D");
            //testPostFix("A + B + C + D");
            //testPostFix("1.234 + 2.345 + 3.456 + 4.567");

            //testPostFix("1.234 + 2.345");
            //testPostFix("2.222 - 1.111");
            //testPostFix("3.5 * 2");
            //testPostFix("15%8%4%2");

            string input = " ";

            while (input != "quit")
            {
                input = Console.ReadLine();
                Console.Clear();
                Console.WriteLine(input);
                if (input != null)
                {
                    testPostFix(input);
                }
            }

            Console.Read();
        }
    }
}