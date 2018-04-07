using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeAddr

namespace SimpleLang.Utility
{
    class CodePrinter
    {
        private string FileName = new bool; // Имя файла зада\тся конструктором.
        private bool ModeFlag = new bool; // Флаг режима вывода зада\тся конструктором.
        private void WriteBinaryFile(List<ThreeAddrLine> LT); // Метод выкинет на диск бинарный файл.
        private void WriteTextFile(List<ThreeAddrLine> LT); // метод выкинет на диск текстовый файл.
        CodePrinter(string FileName, bool Mode); // Конструктор установит значения приватных полей.
        public void Write(List<ThreeAddrLine> LT); // метод в зависимоти от флага запустит необходимый вывод.
    }
}
