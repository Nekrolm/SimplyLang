using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using ThreeAddr;
using System.Text;
using System.Text.RegularExpressions;

namespace Interpreter
{

    class Program
    {
        public static void InterpreterMain(List<ThreeAddrLine> codeLine, Dictionary<string, int> nameVariables)
        {
            int i = 0;
            int countRunInstruction = 0;

            while ((i < codeLine.Count) && ((codeLine[i].OpType == ThreeAddrOpType.Goto ? codeLine.Count - 1 !=  Convert.ToInt16(codeLine[i].RightOp):true)))
            {
                int paramLeft = 0;
                int paramRight = 0;

                switch (codeLine[i].OpType)
                {
                    case ThreeAddrOpType.Assign:
                        nameVariables[codeLine[i].Accum] = Regex.IsMatch(codeLine[i].RightOp, @"\D") ?  nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);
                        i++;
                        break;
                    case ThreeAddrOpType.Plus:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft + paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Minus:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft - paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Mul:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft * paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Div:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft / paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Less:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft < paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Greater:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft > paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.LessOrEq:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft <= paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.GreaterOrEq:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft >= paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Eq:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft == paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.UnEq:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = paramLeft != paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Or:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = Convert.ToBoolean(paramLeft) || Convert.ToBoolean(paramRight) ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.And:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = Convert.ToBoolean(paramLeft) || Convert.ToBoolean(paramRight) ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Not:
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);

                        nameVariables[codeLine[i].Accum] = Convert.ToInt16(!Convert.ToBoolean(paramRight));
                        i++;
                        break;
                    case ThreeAddrOpType.Read:
                        Console.Write("Введите значение для переменой " + codeLine[i].Accum + ": ");

                        nameVariables[codeLine[i].Accum] =  int.Parse(Console.ReadLine());
                        i++;
                        break;
                    case ThreeAddrOpType.Write:
                        paramRight = Regex.IsMatch(codeLine[i].RightOp, @"\D") ? nameVariables[codeLine[i].RightOp] : int.Parse(codeLine[i].RightOp);
                        Console.WriteLine("Вывод значения, строка " + codeLine[i].Label + ": " + paramRight);

                        i++;
                        break;
                    case ThreeAddrOpType.IfGoto:
                        paramLeft = Regex.IsMatch(codeLine[i].LeftOp, @"\D") ? nameVariables[codeLine[i].LeftOp] : int.Parse(codeLine[i].LeftOp);
                        if (Convert.ToBoolean(paramLeft))
                            i = Convert.ToInt16(codeLine[i].RightOp);
                        else
                            i++;
                        break;
                    case ThreeAddrOpType.Goto:
                        i = Convert.ToInt16(codeLine[i].RightOp);
                        break;
                    default: Console.WriteLine("Неизвестная операция: " + codeLine[i].OpType);
                        break;
                }
                countRunInstruction++;
            }

            Console.WriteLine("Количество выполненых инструкций: " + countRunInstruction);
        }

        private static void CreateVariables(List<ThreeAddrLine>codeLines, Dictionary<string, int> nameVariables)
        {
            foreach (var codeLine in codeLines)
            {
                int value = 0;
                if ((codeLine.Accum != null) && (!nameVariables.TryGetValue(codeLine.Accum, out value)))
                    nameVariables.Add(codeLine.Accum, 0);
            }

        }

        static void Main(string[] args)
        {

            Console.Write("Введите путь к файлу: ");
            string path = Console.ReadLine();

            List<ThreeAddrLine> codeLine = new List<ThreeAddrLine>();
            Dictionary<string, int> nameVariables = new Dictionary<string, int>();

            CodePrinter cp = new CodePrinter("a_TreeAddrLine.txt", false);

            if (!Regex.IsMatch(path, @".nsl"))
                codeLine = cp.ReadTextFile(path);
            else
                codeLine = cp.ReadBinaryFile(path);

            CreateVariables(codeLine, nameVariables);
            InterpreterMain(codeLine, nameVariables);

            Console.WriteLine("Количество строк в файле: " + codeLine.Count);

            Console.Read();
        }
    }
}
