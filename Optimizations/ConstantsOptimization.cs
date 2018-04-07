using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLang.Utility;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    class ConstantsOptimization : BaseBlockOptimization
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
            int a=0, b=0;
            bool isaconst = int.TryParse(line.LeftOp, out a); // Получаем левый оперант.
            bool isbconst = int.TryParse(line.RightOp, out b);

            if (line.LeftOp == null)
            {
                isaconst = true;
                a = 0;
            }

            if (!isaconst || !isbconst) return false;
            // Получаем правый оперант.
            var res = ComputeHelper.Calculate(a, b, line.OpType).ToString(); // Записываем вправо вычисленное значение.
            if (res != null){
                line.LeftOp = null; // Просто зануляем.
                line.OpType = ThreeAddrOpType.Assign; // записываем в тип операции assign.
                line.RightOp = res;
                return true;
            }
            return false;
        }

    }
}
