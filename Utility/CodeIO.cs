using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ThreeAddr;
using System.Text.RegularExpressions;



namespace SimpleLang.Utility
{
    class CodeIO
    {
        private string Name; // Имя файла задаётся конструктором.
        private bool _isBinary; // Флаг режима вывода задаётся конструктором.
        private void WriteBinaryFile(List<ThreeAddrLine> LT) // Метод выкинет на диск бинарный файл.
        {
            BinaryFormatter Formater = new BinaryFormatter();
            using (FileStream fs = new FileStream(Name, FileMode.OpenOrCreate))
            {
                    Formater.Serialize(fs, LT);
            }
        }

        private void WriteTextFile(List<ThreeAddrLine> LT) // метод выкинет на диск текстовый файл.
        {
            List<string> Code = new List<string>(); // Список строк кода.
            for (int i = 0; i < LT.Count; i++) // Проход по всему списку.
            {
                Code.Add(LT[i].ToString()); // Заброс строки в список.
            }
            File.WriteAllLines(Name, Code); // Запись списка в файл.
        }

        public List<ThreeAddrLine> ReadBinaryFile(string FilePath) // Метод читает трёхадерсную программу из бинарного файла.
        {
            BinaryFormatter Formater = new BinaryFormatter();
            List<ThreeAddrLine> Code;
            using (FileStream FS = new FileStream(FilePath, FileMode.OpenOrCreate))
            {
                Code = (List<ThreeAddrLine>)Formater.Deserialize(FS);
            }
            return Code;
        }

        public List<ThreeAddrLine> ReadTextFile(string FilePath) // Метод читает трёхадерсную программу из текстового файла.
        {
            BinaryFormatter Formater = new BinaryFormatter();
            List<ThreeAddrLine> Code = new List<ThreeAddrLine>();
            using (StreamReader FS = new StreamReader(FilePath))
            {
                string line;
                while ((line = FS.ReadLine()) != null)
                {
                    
                    line = line.Replace("=", " ");
                    line = line.Replace(":", " ");
                    line = Regex.Replace(line, @"\s+", " ");

                    string[] tokens = line.Split(' ');
                    int size_tokens = tokens.Count();

                    ThreeAddrLine threeAddrLine = new ThreeAddrLine();

                    threeAddrLine.Label = tokens[0];

                    // 1
                    if (tokens[1] == "nop") // 35:  =  nop 
                    {
                        threeAddrLine.OpType = "nop";
                        Code.Add(threeAddrLine);
                        continue;
                    }

                    if (tokens[1] == "write" ||
                        tokens[1] == "goto"
                       ) // 39:  =  write p11
                    {
                        threeAddrLine.OpType = tokens[1];
                        threeAddrLine.RightOp = tokens[2];
                        Code.Add(threeAddrLine);
                        continue;
                    }

                    // 2
                    if (tokens[2] == "nop" ||
                        tokens[2] == "read"
                       ) // 40: 5 =  nop 
                    {
                        threeAddrLine.OpType = tokens[2];
                        threeAddrLine.Accum = tokens[1];
                        Code.Add(threeAddrLine);
                        continue;
                    }

                    if (tokens[2] == "ifgoto") // 78:  = p29 ifgoto 80
                    {
                        threeAddrLine.OpType = "ifgoto";
                        threeAddrLine.LeftOp = tokens[1];
                        threeAddrLine.RightOp = tokens[3];
                        Code.Add(threeAddrLine);
                        continue;
                    }

                    if (tokens[2] == "assign" ||
                        tokens[2] == "not"
                       ) // 36: p10 =  assign v2
                    {
                        threeAddrLine.OpType = tokens[2];
                        threeAddrLine.Accum = tokens[1];
                        threeAddrLine.RightOp = tokens[3];
                        Code.Add(threeAddrLine);
                        continue;
                    }


                    // 3
                    if (tokens[3] == "or" ||
                        tokens[3] == "and" ||
                        tokens[3] == "+" ||
                        tokens[3] == "-" ||
                        tokens[3] == "/" ||
                        tokens[3] == "*" ||
                        tokens[3] == "<" ||
                        tokens[3] == ">" ||
                        tokens[3] == "<=" ||
                        tokens[3] == ">=" ||
                        tokens[3] == "==" ||
                        tokens[3] == "!=" ||
                        tokens[3] == "assign"
                       )  // 45: p12 = p13 or 0
                    {
                       
                        threeAddrLine.OpType = tokens[3];
                        threeAddrLine.Accum = tokens[1];
                        threeAddrLine.LeftOp = tokens[2];
                        threeAddrLine.RightOp = tokens[4];

                        Code.Add(threeAddrLine);
                        continue;
                    }


                   
                }
            }
            return Code;
        }


        public CodeIO(string FileName, bool isBinary) // Конструктор установит значения приватных полей.
        {
            Name = FileName;
            _isBinary = isBinary;
        }

        public void Write(List<ThreeAddrLine> LT) // метод в зависимоти от флага запустит необходимый вывод.
        {
            if (!_isBinary)
            {
                WriteTextFile(LT);
            }else{
                WriteBinaryFile(LT);
            }
                
        }
    }
}
