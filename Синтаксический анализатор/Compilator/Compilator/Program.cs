using System;
using System.Collections.Generic;
using System.IO;

namespace Compilator
{
    class Program
    {
        static int GetIndexLine(Lex lex)
        {
            if (lex.Name == "STAT")
                return 0;
            else if (lex.Name == "ITERATION")
                return 1;
            else if (lex.Name == "ASSIGNMENT_EXP")
                return 2;
            else if (lex.Name == "EXP")
                return 3;
            else if (lex.Name == "ADDITIVE_EXP")
                return 4;
            else if (lex.Name == "MULT_EXP")
                return 5;
            else if (lex.Name == "CAST_EXP")
                return 6;
            else if (lex.Name == "UNARY_EXP")
                return 7;
            else if (lex.Name == ";")
                return 8;
            else if (lex.Name == "while")
                return 9;
            else if (lex.Name == "(")
                return 10;
            else if (lex.Name == ")")
                return 11;
            else if (lex.Name == ",")
                return 12;
            else if (lex.Name == "id")
                return 13;
            else if (lex.Name == "=")
                return 14;
            else if (lex.Name == "additive_operator")
                return 15;
            else if (lex.Name == "mult_operator")
                return 16;
            else if (lex.Name == "type_name")
                return 17;
            else if (lex.Name == "int")
                return 18;
            else if (lex.Name == "float")
                return 19;
            else if (lex.Name == "prefix_operator")
                return 20;
            else
                return 21;
        }
        static int GetIndexColumn(Lex lex)
        {
            if (lex.Name == ";")
                return 0;
            else if (lex.Name == "while")
                return 1;
            else if (lex.Name == "(")
                return 2;
            else if (lex.Name == ")")
                return 3;
            else if (lex.Name == ",")
                return 4;
            else if (lex.Name == "id")
                return 5;
            else if (lex.Name == "=")
                return 6;
            else if (lex.Name == "additive_operator")
                return 7;
            else if (lex.Name == "mult_operator")
                return 8;
            else if (lex.Name == "type_name")
                return 9;
            else if (lex.Name == "int")
                return 10;
            else if (lex.Name == "float")
                return 11;
            else if (lex.Name == "prefix_operator")
                return 12;
            else
                return 13;
        }
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
        enum States { Start, INT, FLOAT, ID, ERROR, MINUS, PLUS };
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@"C:\Users\BatMA\Desktop\Синтаксический анализатор\Compilator\Compilator\input.txt");
            StreamWriter sw = new StreamWriter(@"C:\Users\BatMA\Desktop\Синтаксический анализатор\Compilator\Compilator\output.txt");

            string text = sr.ReadToEnd();

            States state = States.Start;

            List<string> KeyWords = new List<string>() { "int", "double", "char", "long", "short", "while" };
            List<char> Separator = new List<char>() { ';', '(', ')', ',' };
            List<char> Operations = new List<char>() { '-', '+', '*', '/', '%', '=' };


            List<string> listID = new List<string>();
            List<int> listINT = new List<int>();
            List<Lex> lexs = new List<Lex>();

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
                                buf += symbol;
                                Lex lex = new Lex(buf, buf);
                                lexs.Add(lex);
                                buf = "";
                                state = States.Start;
                            }

                            else
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "OPERATIONS", symbol, Operations.IndexOf(symbol));
                                buf += symbol;
                                Lex lex = new Lex("mult_operator", buf);
                                lexs.Add(lex);
                                buf = "";
                                state = States.Start;
                            }
                        }

                        else if (Separator.Contains(symbol))
                        {
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                            buf += symbol;
                            Lex lex = new Lex(buf, buf);
                            lexs.Add(lex);
                            buf = "";
                            state = States.Start;
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
                                    if (buf == "while")
                                    {
                                        Lex lex = new Lex(buf, buf);
                                        lexs.Add(lex);
                                    }
                                    else
                                    {
                                        Lex lex = new Lex("type_name", buf);
                                        lexs.Add(lex);
                                    }
                                    buf = "";
                                }
                                else
                                {
                                    if (!listID.Contains(buf))
                                    {
                                        listID.Add(buf);
                                    }
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ID", buf, listID.IndexOf(buf));
                                    Lex lex = new Lex("id", buf);
                                    lexs.Add(lex);
                                    buf = "";
                                }
                                if (symbol == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(symbol))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", symbol, Separator.IndexOf(symbol));
                                    buf += symbol;
                                    Lex lex = new Lex(buf, buf);
                                    lexs.Add(lex);
                                    buf = "";
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
                                Lex lex = new Lex("int", buf);
                                lexs.Add(lex);
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
                                    buf += symbol;
                                    Lex lex2 = new Lex(buf, buf);
                                    lexs.Add(lex2);
                                    buf = "";
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
                            if (symbol == '0' && dt == 0)
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
                                Lex lex = new Lex("float", buf);
                                lexs.Add(lex);
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
                                    buf += symbol;
                                    Lex lex2 = new Lex(buf, buf);
                                    lexs.Add(lex2);
                                    buf = "";
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
                                buf += symbol;
                                Lex lex = new Lex(buf, buf);
                                lexs.Add(lex);
                                buf = "";
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
                            Lex lex = new Lex("additive_operator", buf);
                            lexs.Add(lex);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '-')
                        {
                            buf += symbol;
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "PREFIX", buf, 11);
                            Lex lex = new Lex("prefix_operator", buf);
                            lexs.Add(lex);
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
                            Lex lex = new Lex("additive_operator", buf);
                            lexs.Add(lex);
                            buf = "";
                            state = States.Start;
                        }
                        else if (symbol == '+')
                        {
                            buf += symbol;
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "PREFIX", buf, 11);
                            Lex lex = new Lex("prefix_operator", buf);
                            lexs.Add(lex);
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

            Lex end = new Lex("END", "END");
            lexs.Add(end);

            List<Lex> score = new List<Lex>();
            Lex nabla = new Lex("NABLA", "NABLA");
            score.Add(nabla);

            int countLexs = 0;
            bool repeat = true;

            int[,] controlTable = new int[22, 14]
            { 
                {-1,  -1,   -1,  -1,  -1,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1,  1},
                {-1,  -1,   -1,  -1,  -1,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1,  2},
                { 0,  -1,   -1,   3,   3,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,   0,   0,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                { 4,  -1,   -1,   4,   4,  -1,   -1,     0,   -1,  -1,  -1, -1,  -1, -1},
                { 5,  -1,   -1,   5,   5,  -1,   -1,     5,    0,  -1,  -1, -1,  -1, -1},
                { 6,  -1,   -1,   6,   6,  -1,   -1,     6,    6,  -1,  -1, -1,  -1, -1},
                { 7,  -1,   -1,   7,   7,  -1,   -1,     7,    7,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,  -1,  -1,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1,  8},
                {-1,  -1,    0,  -1,  -1,  -1,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                {-1,   0,   -1,  -1,  -1,   0,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1},
                { 9,  -1,   -1,   9,   9,  -1,    0,     9,    9,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,   0,   0,  0,   0, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,   0,   0,  0,   0, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,   0,   0,  0,   0, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,   0,   0,  0,   0, -1},
                {10,  -1,   -1,  10,  10,  -1,   -1,    10,   10,  -1,  -1, -1,  -1, -1},
                {11,  -1,   -1,  11,  11,  -1,   -1,    11,   11,  -1,  -1, -1,  -1, -1},
                {-1,  -1,   -1,  -1,  -1,   0,   -1,    -1,   -1,  -1,   0,  0,   0, -1},
                {-1,   0,   -1,  -1,  -1,   0,   -1,    -1,   -1,  -1,  -1, -1,  -1, -1}
            };
            // -1 - отвергнуть 
            // 0 - перенос 
            // 1-11 - опознать соответственно

            while (repeat)
            {
                int line = GetIndexLine(score[score.Count - 1]);
                score.Add(lexs[countLexs]);

                countLexs++;

                int column = GetIndexColumn(score[score.Count - 1]);
                int controlTableState = controlTable[line, column];

                if (controlTableState >= 1 && controlTableState <= 11)
                {
                    score.RemoveAt(score.Count - 1);
                    countLexs--;
                }

                switch (controlTableState)
                {
                    case -1:
                        Console.WriteLine("ОТВЕРГНУТЬ");
                        repeat = false;
                        break;
                    case 0:
                        break;
                    case 1:
                        if (score[0].Name == "NABLA" && score[1].Name == "STAT")
                        {
                            Console.WriteLine("ДОПУСТИТЬ");
                            repeat = false;
                        }
                        else if (score[score.Count - 5].Name == "while" && score[score.Count - 4].Name == "(" && score[score.Count - 3].Name == "EXP" && score[score.Count - 2].Name == ")" && score[score.Count - 1].Name == "STAT")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("ITERATION", "ITERATION");
                            score.Add(lex);
                        }
                        else 
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 2:
                        if (score[score.Count - 1].Name == "ITERATION")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("STAT", "STAT");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 3:
                        if (score[score.Count - 3].Name == "EXP" && score[score.Count - 2].Name == "," && score[score.Count - 1].Name == "ASSIGNMENT_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("EXP", "EXP");
                            score.Add(lex);
                        }
                        else if (score[score.Count - 1].Name == "ASSIGNMENT_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("EXP", "EXP");
                            score.Add(lex);
                        }
                        break;
                    case 4:
                        if (score[score.Count - 3].Name == "id" && score[score.Count - 2].Name == "=" && score[score.Count - 1].Name == "ADDITIVE_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("ASSIGNMENT_EXP", "ASSIGNMENT_EXP");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 5:
                        if (score[score.Count - 3].Name == "ADDITIVE_EXP" && score[score.Count - 2].Name == "additive_operator" && score[score.Count - 1].Name == "MULT_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("ADDITIVE_EXP", "ADDITIVE_EXP");
                            score.Add(lex);
                        }
                        else if (score[score.Count - 1].Name == "MULT_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("ADDITIVE_EXP", "ADDITIVE_EXP");
                            score.Add(lex);
                        }
                        break;
                    case 6:
                        if (score[score.Count - 3].Name == "MULT_EXP" && score[score.Count - 2].Name == "mult_operator" && score[score.Count - 1].Name == "CAST_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("MULT_EXP", "MULT_EXP");
                            score.Add(lex);
                        }
                        else if (score[score.Count - 2].Name == "type_name" && score[score.Count - 1].Name == "CAST_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("CAST_EXP", "CAST_EXP");
                            score.Add(lex);
                        }
                        else if (score[score.Count - 1].Name == "CAST_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("MULT_EXP", "MULT_EXP");
                            score.Add(lex);
                        }
                        break;
                    case 7:
                        if (score[score.Count - 2].Name == "prefix_operator" && score[score.Count - 1].Name == "UNARY_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("UNARY_EXP", "UNARY_EXP");
                            score.Add(lex);
                        }
                        else if (score[score.Count - 1].Name == "UNARY_EXP")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("CAST_EXP", "CAST_EXP");
                            score.Add(lex);
                        }
                        break;
                    case 8:
                        if (score[score.Count - 2].Name == "ASSIGNMENT_EXP" && score[score.Count - 1].Name == ";")
                        {
                            score.RemoveAt(score.Count - 1);
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("STAT", "STAT");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 9:
                        if (score[score.Count - 1].Name == "id")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("UNARY_EXP", "UNARY_EXP");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 10:
                        if (score[score.Count - 1].Name == "int")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("UNARY_EXP", "UNARY_EXP");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                    case 11:
                        if (score[score.Count - 1].Name == "float")
                        {
                            score.RemoveAt(score.Count - 1);
                            Lex lex = new Lex("UNARY_EXP", "UNARY_EXP");
                            score.Add(lex);
                        }
                        else
                        {
                            Console.WriteLine("ОТВЕРГНУТЬ");
                            repeat = false;
                        }
                        break;
                }
            }

            sr.Close();
            sw.Close();

        }
    }
}
