using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using ThreeAddr;

namespace Interpreter
{
    public class Options
    {
        [Option('w', "write", Required = false, Default = "a_TreeAddrLine.out",
                HelpText = "Output file")]
        public string OutputFile { get; set; }

        [Option("binary", Default = false)]
        public bool OutBinary { get; set; }

        [Value(0, MetaName = "input", HelpText = "File to Compile", Default = "../../a.txt")]
        public String InputFile { get; set; }
    }

    class Program
    {
        public static void InterpreterMain(List<ThreeAddrLine> codeLine, Dictionary<string, double> nameVariables)
        {
            int i = 0;

            while ((i < codeLine.Count) && ((codeLine[i].OpType == ThreeAddrOpType.Goto ? codeLine.Count - 1 !=  Convert.ToInt16(codeLine[i].RightOp):true)))
            {
                double paramLeft = 0;
                double paramRight = 0;

                switch (codeLine[i].OpType)
                {
                    case ThreeAddrOpType.Assign:
                        nameVariables[codeLine[i].Accum] = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];
                        i++;
                        break;
                    case ThreeAddrOpType.Plus:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft + paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Minus:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft - paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Mul:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft * paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Div:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft / paramRight;
                        i++;
                        break;
                    case ThreeAddrOpType.Less:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft < paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Greater:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft > paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.LessOrEq:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft <= paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.GreaterOrEq:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft >= paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Eq:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft == paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.UnEq:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = paramLeft != paramRight ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Or:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = Convert.ToBoolean(paramLeft) || Convert.ToBoolean(paramRight) ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.And:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = Convert.ToBoolean(paramLeft) || Convert.ToBoolean(paramRight) ? 1 : 0;
                        i++;
                        break;
                    case ThreeAddrOpType.Not:
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];

                        nameVariables[codeLine[i].Accum] = Convert.ToInt16(!Convert.ToBoolean(paramRight));
                        i++;
                        break;
                    case ThreeAddrOpType.Read:
                        Console.Write("Введите значение для переменой " + codeLine[i].Accum + ": ");

                        nameVariables[codeLine[i].Accum] =  Convert.ToDouble(Console.ReadLine());
                        i++;
                        break;
                    case ThreeAddrOpType.Write:
                        paramRight = (codeLine[i].RightOp[0] != 'p') && (codeLine[i].RightOp[0] != 'v') ? double.Parse(codeLine[i].RightOp) : nameVariables[codeLine[i].RightOp];
                        Console.WriteLine("Вывод значения, строка " + codeLine[i].Label + ": " + paramRight);

                        i++;
                        break;
                    case ThreeAddrOpType.IfGoto:
                        paramLeft = (codeLine[i].LeftOp[0] != 'p') && (codeLine[i].LeftOp[0] != 'v') ? double.Parse(codeLine[i].LeftOp) : nameVariables[codeLine[i].LeftOp];
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
            }
        }

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\a_TreeAddrLine.txt";
            
            string filesname = File.ReadAllText(path);
            string[] fileLine = filesname.Split('\n');
            List<ThreeAddrLine> codeLine = new List<ThreeAddrLine>();
            Dictionary<string, double> nameVariables = new Dictionary<string, double>();

            FileParser.Parser(fileLine, codeLine, nameVariables);

            InterpreterMain(codeLine, nameVariables);

            Console.WriteLine("Количество строк в файле: " + codeLine.Count);

            Console.Read();
        }
    }
}
