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
            
        }
        private void recognize(ThreeAddrLine line) // метод получает строку трехадресного кода, конвертирует операнты и записывает результат.
        {
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
                default: return 0; // В остальных случаях будет возвращ\н ноль.
            }
        }
    }
}
