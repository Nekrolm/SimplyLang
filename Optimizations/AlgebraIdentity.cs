using System;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    public class AlgebraIdentity : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {
                if (ThreeAddrOpType.Computable.Contains(line.OpType))
                {
                    answer |= Recognize(line); // Выполняем оптимизацию
                }
            }
            return answer;
        }
        private bool Recognize(ThreeAddrLine line) // метод получает строку трехадресного кода, конвертирует операнты и записывает результат.
        {
            int a = 0, b = 0;
            bool isaconst = int.TryParse(line.LeftOp, out a); // Получаем левый оперант.
            bool isbconst = int.TryParse(line.RightOp, out b);

            if (line.LeftOp == null)
            {
                isaconst = true;
                a = 0;
            }
            string res;
            if ((isaconst == isbconst) || (line.LeftOp != line.RightOp)) return false; //если у нас обе константы, или обе переменны, которые не равны друг другу, то не наш случай

            // Конвертируем операнды и считаем результат
            if (isaconst && (a == 1 || a == 0)) { res = Compute(line.RightOp, a, line.OpType).ToString();  }
            else if (isbconst && (b == 1 || b == 0)) { res = Compute(line.LeftOp, b, line.OpType).ToString(); }
            else if (line.OpType== "/") { res = "1"; }
            else if (line.OpType == "-") { res = "0"; }

            if (res != null)
            {
                line.LeftOp = null; // Просто зануляем.
                line.OpType = ThreeAddrOpType.Assign; // записываем в тип операции assign.
                line.RightOp = res;
                return true;
            }
            return false;
        }
        private int? Compute(string MyVar, int MyConst, string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
        {
            switch (OpType)
            {
                case "+": if (MyConst==0) return MyVar; 
                case "-": if (MyConst==0) return MyVar; 
                case "*": if (MyConst == 0) return 0; else return MyVar;
                case "/": if (MyConst == 1) return MyVar; else return 0;
                case ThreeAddrOpType.And:
                    if (MyConst==0) return 0;
                case ThreeAddrOpType.Or:
                    if (MyConst == 1) return 1;
                default:
                    return null; 
            }
        }
    }
}