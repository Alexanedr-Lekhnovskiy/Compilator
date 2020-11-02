using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace lex_analyzer
{
    class Program
    {
        public static int getCountsOfDigits(int number)
        {
            int count = (number == 0) ? 1 : 0;
            while (number != 0)
            {
                count++;
                number /= 10;
            }
            return count;
        }
        enum States { Start, INT, FLOAT, ID, ERROR, MINUS, PLUS, CHAR };
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@"C:\Users\BatMA\Documents\GitHub\lex_analyzer\input.txt");
            StreamWriter sw = new StreamWriter(@"C:\Users\BatMA\Documents\GitHub\lex_analyzer\output.txt");

            string text = sr.ReadToEnd();

            States state = States.Start;

            List<string> KeyWords = new List<string>() { "int", "double", "char", "long", "short", "while" };
            List<char> Separator = new List<char>() { ';', '{', '}', '(', ')', ',' };
            List<char> Operations = new List<char>() { '-', '+', '*', '/', '%', '=' };


            List<string> listID = new List<string>();
            List<int> listINT = new List<int>();

            int countLine = 1;

            string buf = "";

            int dt = 0;
            double nf = 0;
            int negative = 0;
            int countZero = 0;

            foreach (char symbol in text)
            {
                switch (state)
                {
                    case States.Start:
                        if (symbol == '\n')
                        {
                            countLine++;
                            continue;
                        }

                        else if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r')
                        {
                            continue;
                        }

                        else if (char.IsLetter(symbol))
                        {
                            buf += symbol;
                            state = States.ID;
                        }

                        else if (char.IsDigit(symbol))
                        {
                            buf += symbol;
                            dt = (int)(symbol - '0');
                            state = States.INT;
                        }

                        else if (symbol == '.')
                        {
                            buf += symbol;
                            state = States.FLOAT;
                        }

                        else if (symbol == '\'')
                        {
                            state = States.CHAR;
                        }

                        else if (Operations.Contains(symbol))
                        {
                            if (symbol == '-')
                            {
                                buf += symbol;
                                state = States.MINUS;
                            }

                            else if (symbol == '+')
                            {
                                buf += symbol;
                                state = States.PLUS;
                            }

                            else if (symbol == '=')
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ASSIGNMENT", symbol, Operations.IndexOf(symbol));
                                state = States.Start;

                            }

                            else
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "OPERATIONS", symbol, Operations.IndexOf(symbol));
                                state = States.Start;
                            }
                        }

                        else if (Separator.Contains(symbol))
                        {
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                        }

                        else
                        {
                            buf += symbol;
                            state = States.ERROR;
                        }
                        break;



                    case States.ID:
                        if (char.IsLetterOrDigit(symbol) || symbol == '_')
                        {
                            buf += symbol;
                        }
                        else
                        {
                            if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r' || symbol == '\n' || Separator.Contains(symbol))
                            {
                                if (KeyWords.Contains(buf))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "KEYWORD", buf, KeyWords.IndexOf(buf));
                                    buf = "";
                                }
                                else
                                {
                                    if (!listID.Contains(buf))
                                    {
                                        listID.Add(buf);
                                    }
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ID", buf, listID.IndexOf(buf));
                                    buf = "";
                                }
                                if (symbol == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(symbol))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                                }
                                state = States.Start;
                            }
                            else
                            {
                                buf += symbol;
                                state = States.ERROR;
                            }
                        }
                        break;


                    case States.INT:
                        if (Char.IsDigit(symbol))
                        {
                            buf += symbol;
                            dt = dt * 10 + (int)(symbol - '0');
                        }
                        else
                        {
                            if (symbol == '.')
                            {
                                buf += symbol;
                                nf = dt;
                                state = States.FLOAT;
                                dt = 0;
                            }
                            else if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r' || symbol == '\n' || Separator.Contains(symbol))
                            {
                                if (negative == 1)
                                {
                                    dt *= -1;
                                    negative = 0;
                                }
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "INT", dt, "NONE");
                                listINT.Add(dt);
                                dt = 0;
                                buf = "";
                                if (symbol == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(symbol))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                                }
                                state = States.Start;
                            }
                            else
                            {
                                buf += symbol;
                                state = States.ERROR;
                            }
                        }
                        break;

                    case States.FLOAT:
                        if (Char.IsDigit(symbol))
                        {
                            buf += symbol;
                            dt = dt * 10 + (int)(symbol - '0');
                            if (symbol == '0' && dt == 0 )
                            {
                                countZero++;
                            }
                        }
                        else
                        {
                            if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r' || symbol == '\n' || Separator.Contains(symbol))
                            {
                                nf += dt / Math.Pow(10, getCountsOfDigits(dt) + countZero);
                                if (negative == 1)
                                {
                                    nf *= -1;
                                    negative = 0;
                                }
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "FLOAT", nf, "NONE");
                                nf = 0;
                                dt = 0;
                                countZero = 0;
                                buf = "";
                                if (symbol == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(symbol))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                                }
                                state = States.Start;
                            }
                            else
                            {
                                buf += symbol;
                                state = States.ERROR;
                            }
                        }
                        break;

                    case States.CHAR:
                        if (symbol != '\'' || buf == "\\" && symbol == '\'' || buf == "\\" && symbol == '\\')
                        {
                            buf += symbol;
                        }
                        else
                        {
                            if (buf == "\\\'")
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "CHAR", '\'', "NONE");
                            }
                            else if (buf == "\\\\")
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "CHAR", '\\', "NONE");
                            }
                            else
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "CHAR", buf, "NONE");
                            }
                            buf = "";
                            state = States.Start;
                        }
                        break;


                    case States.ERROR:
                        if (symbol != ' ' && symbol != '\t' && symbol != '\0' && symbol != '\r' && symbol != '\n' && !Separator.Contains(symbol))
                        {
                            buf += symbol;
                        }
                        else
                        {
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ERROR", buf, "NONE");
                            if (symbol == '\n')
                            {
                                countLine++;
                            }
                            if (Separator.Contains(symbol))
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                            }
                            buf = "";
                            state = States.Start;
                        }
                        break;

                    case States.MINUS:
                        if (char.IsDigit(symbol))
                        {
                            negative = 1;
                            buf += symbol;
                            dt = (int)(symbol - '0');
                            state = States.INT;
                        }
                        else if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r' || symbol == '\n')
                        {
                            if (symbol == '\n')
                            {
                                countLine++;
                            }
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "OPERATIONS", buf, 0);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '-')
                        {
                            buf += symbol;
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "PREFIX", buf, 11);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '.')
                        {
                            dt = 0;
                            negative = 1;
                            state = States.FLOAT;
                        }
                        else
                        {
                            buf += symbol;
                            state = States.ERROR;
                        }
                        break;



                    case States.PLUS:
                        if (char.IsDigit(symbol))
                        {
                            buf += symbol;
                            dt = (int)(symbol - '0');
                            state = States.INT;
                        }
                        else if (symbol == ' ' || symbol == '\t' || symbol == '\0' || symbol == '\r' || symbol == '\n')
                        {
                            if (symbol == '\n')
                            {
                                countLine++;
                            }
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "OPERATIONS", buf, 1);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '+')
                        {
                            buf += symbol;
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "PREFIX", buf, 11);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '.')
                        {
                            dt = 0;
                            state = States.FLOAT;
                        }
                        else
                        {
                            buf += symbol;
                            state = States.ERROR;
                        }
                        break;
                }
            }
            sr.Close();
            sw.Close();

        }
    }
}
