using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThreeAddr;

namespace Interpreter
{
    class FileParser
    {
        private static void CreateThreeAddrLine(MatchCollection matches, ThreeAddrLine codeLine)
        {
            switch (matches.Count)
            {
                case 6: codeLine.Label = matches[0].Value;
                        codeLine.Accum = matches[1].Value;
                        codeLine.LeftOp = matches[3].Value;
                        codeLine.OpType = matches[4].Value;
                        codeLine.RightOp = matches[5].Value;
                    break; 
                case 5: codeLine.OpType = matches[3].Value;
                        codeLine.RightOp = matches[4].Value;
                        codeLine.Label = matches[0].Value;
                        if (codeLine.OpType.Equals(ThreeAddrOpType.Assign) || codeLine.OpType.Equals(ThreeAddrOpType.Not))
                            codeLine.Accum = matches[1].Value;
                        else
                            codeLine.LeftOp = matches[2].Value;
                    break;
                case 4: codeLine.Label = matches[0].Value;
                        if (matches[1].Value.Equals("="))
                        {
                            codeLine.OpType = matches[2].Value;
                            codeLine.RightOp = matches[3].Value;
                        }
                        else
                        {
                            codeLine.Accum = matches[1].Value;
                            codeLine.OpType = matches[3].Value;
                        }
                    break;
                default: Console.WriteLine("Неизвестная строка кода");
                    break;
               
            }
        }

        private static void CreateVariables(ThreeAddrLine codeLine, Dictionary<string, double> nameVariables)
        {
            double value = 0;
            if ((codeLine.Accum != null) && (!nameVariables.TryGetValue(codeLine.Accum, out value)))
                    nameVariables.Add(codeLine.Accum, 0);
                
        }

        public static void Parser(string[] text, List<ThreeAddrLine> code, Dictionary<string, double> nameVariables)
        {
            for (var i = 0; i < text.Length; i++)
            {
                ThreeAddrLine codeLine = new ThreeAddrLine();

                text[i] = text[i].Remove(text[i].IndexOf(':'), 1);

                string pattern = @"\S+";
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(text[i]);

                CreateThreeAddrLine(matches, codeLine);
                CreateVariables(codeLine, nameVariables);
                // codeLine.Label = matches[0].Value;
                //  codeLine.Accum = matches[1].Value;

                code.Add(codeLine);
            }
        }
    }
}
