using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ThreeAddr

namespace SimpleLang.Utility
{
    class CodePrinter
    {
        private string Name = new string(); // Имя файла задаётся конструктором.
        private bool ModeFlag = new bool(); // Флаг режима вывода задаётся конструктором.
        private void WriteBinaryFile(List<ThreeAddrLine> LT); // Метод выкинет на диск бинарный файл.
        private void WriteTextFile(List<ThreeAddrLine> LT) // метод выкинет на диск текстовый файл.
        {
            string Empty = "    "; // вместа таба.
            string Space = " "; // пробел.
            List<string> Code;
for (int i = 0; i < LT.Count; i++)
            {
                string NewLine = new string();
                switch (LT[i].OpType)
                {
                    case ThreeAddrOpType.Nop: { NewLine = LT[i].Label + Space + LT[i].OpType; break; }
                    case ThreeAddrOpType.Write: { NewLine = LT[i].Label + Space + LT[i].OpType + Space + Empty + Space + Empty + LT[i].RightOp; break; }
                    case ThreeAddrOpType.Read: { NewLine = LT[i].Label + Space + LT[i].Accum; break; }
                    case ThreeAddrOpType.IfGoto: { NewLine = LT[i].Label + Space + LT[i].OpType + Space + Empty + Space + LT[i].LeftOp + Space + LT[i].RightOp; break; }
                    case ThreeAddrOpType.Goto: { NewLine = LT[i].Label + Space + LT[i].OpType + Space + Empty + Space + Empty + LT[i].RightOp; break; }
                    case ThreeAddrOpType.Not: { NewLine = LT[i].Label + Space + LT[i].OpType + Space + LT[i].Accum + Space + Empty + LT[i].RightOp; break; }
                    case ThreeAddrOpType.Minus:
                        {
                            if (LT[i].LeftOp == null) NewLine = LT[i].Label + Space + LT[i].OpType + Space + LT[i].Accum + Space + Empty + Space + LT[i].RightOp;
                            else NewLine = LT[i].Label + Space + LT[i].OpType + Space + LT[i].Accum + Space + Empty + Space + LT[i].RightOp;
                            break;
                        }
                    default: NewLine = LT[i].Label + Space + LT[i].OpType + Space + LT[i].Accum + Space + LT[i].LeftOp + Space + LT[i].RightOp;
                }
                Code.Add(NewLine);
            }
            File.WriteAllLines(Name, Code);
        }

        CodePrinter(string FileName, bool Mode) // Конструктор установит значения приватных полей.
        {
            Name = FileName;
            ModeFlag = Mode;
        }

        public void Write(List<ThreeAddrLine> LT) // метод в зависимоти от флага запустит необходимый вывод.
        {
            WriteBinaryFile(LT); // бинарный пишем в любом случае.
            if (ModeFlag) WriteTextFile(LT); // если есть флаг, пишем текстовый файл.
        }
    }
}
