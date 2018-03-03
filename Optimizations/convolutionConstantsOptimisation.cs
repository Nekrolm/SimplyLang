using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    class convolutionConstantsOptimisation : Optimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool Answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            for (int i = 0; i < bblock.Code.Count; i++) // Проход по всему базовому блоку.
            {
                if (bblock.Code[i].OpType != "assign") // Если в типе операции не сидит assign
                {
                    recognize(bblock.Code[i]); // Выполняем оптимизацию.
                        Answer = true; // перекидываем флажок.
                }
                { }
                return Answer;
                        }
        private void recognize(ThreeAddrLine line) // метод получает строку трехадресного кода, конвертирует операнты и записывает результат.
        {
            int a = int.Parse(line.LeftOp); // Получаем левый оперант.
            int b = int.Parse(line.RightOp); // Получаем правый оперант.
            line.RightOp = calculate(a, b, line.OpType); // Записываем вправо вычисленное значение.
            line.LeftOp = null; // Просто зануляем.
            line.OpType = "assign"; // записываем в тип операции assign.
        }
        private int calculate(int a, int b, string OpType) // Метод в зависимости от операции выполняет вычисление и возвращает значение.
        {
            switch (OpType)
            {
                case "+": return a + b; // Если найден плюс: верн\тся сумма.
                case "-": return a - b; // Если минус: то разность.
                case "*": return a * b;
                case "/": return a / b;
                case "<": if (a < b) return 1; else return 0; // Если a меньше b, верн\тся 1: иначе 0.
                case ">": if (a > b) return 1; else return 0;
                case "==": if (a == b) return 1; else return 0;
                    case "not": if (b == 0) return 1; else return 0;
                    case "and": if ((bool)a && (bool)b) return 1; else return 0;
                    case "or": if ((bool)a || (bool)b) return 1; else return 0;
                default: return 0; // В остальных случаях будет возвращён ноль.
            }
        }
    }
}
