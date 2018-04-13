using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ThreeAddr;

namespace SimpleLang.Utility
{
    class CodePrinter
    {
        private string Name; // Имя файла задаётся конструктором.
        private bool ModeFlag = new bool(); // Флаг режима вывода задаётся конструктором.
        private void WriteBinaryFile(List<ThreeAddrLine> LT) // Метод выкинет на диск бинарный файл.
        {
            BinaryFormatter Formater = new BinaryFormatter();
            using (FileStream fs = new FileStream(Name + ".nsl", FileMode.OpenOrCreate))
            {
                    Formater.Serialize(fs, LT);
            }
        }

        private void WriteTextFile(List<ThreeAddrLine> LT) // метод выкинет на диск текстовый файл.
        {
            string Empty = "    "; // вместа таба.
            string Space = " "; // пробел.
            List<string> Code = new List<string>(); // Список строк кода.
for (int i = 0; i < LT.Count; i++) // Проход по всему списку.
            {
                string NewLine; // строка в которой формируются строки для вывода.
                switch (LT[i].OpType) // Переключатель, определяющий вывод строк разного типа.
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
                    default: {NewLine = LT[i].Label + Space + LT[i].OpType + Space + LT[i].Accum + Space + LT[i].LeftOp + Space + LT[i].RightOp; break;}
                }
                Code.Add(NewLine); // Заброс строки в список.
            }
            File.WriteAllLines(Name + ".txt", Code); // Запись списка в файл.
        }

        public List<ThreeAddrLine> RiadBinaryFile(string FilePash) // Метод читает трёхадерсную программу из файла.
        {
            BinaryFormatter Formater = new BinaryFormatter();
            List<ThreeAddrLine> Code;
            using (FileStream FS = new FileStream(FilePash, FileMode.OpenOrCreate))
            {
                Code = (List<ThreeAddrLine>)Formater.Deserialize(FS);
            }
            return Code;
        }

        public CodePrinter() { } // Пригодится конструктор без параметров?

        public CodePrinter(string FileName, bool Mode) // Конструктор установит значения приватных полей.
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
