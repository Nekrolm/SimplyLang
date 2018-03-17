using System;
using ThreeAddr;

// Считается, что перед этой оптимизацией выполнена обработка констант.
namespace SimpleLang.Optimizations
{
    public class ExprCanon : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {
                if (ThreeAddrOpType.Computable.Contains(line.OpType))
                {
                    answer |= Canonize(line); // Выполняем оптимизацию
                }
            }
            return answer;
        }
        private bool Canonize(ThreeAddrLine line) // метод получает строку трехадресного кода, конвертирует операнты и записывает результат.
        {
            int a = 0, b = 0;
            bool isaconst = int.TryParse(line.LeftOp, out a);
            bool isbconst = int.TryParse(line.RightOp, out b);
            string firstop = null;
            string secondop = null;
            string newop = line.OpType;

            if (line.LeftOp == null)
            {
                isaconst = true;
                a = 0;
            }
            if (IsCommutative(line.OpType) || IsCompare(line.OpType))
            {
                if (isaconst) { firstop = line.RightOp; secondop = line.LeftOp; newop = GetOppositeOp(line.OpType); }
                else if (isbconst) { firstop = line.LeftOp; secondop = line.RightOp; }
                else
                {
                    if (string.Compare(line.LeftOp, line.RightOp) < 0) { firstop = line.LeftOp; secondop = line.RightOp; }
                    else { firstop = line.RightOp; secondop = line.LeftOp; newop = GetOppositeOp(line.OpType); }
                }
            }
            if (firstop != null)
            {
                line.LeftOp = firstop;
                line.RightOp = secondop;
                line.OpType = newop;
                return true;
            }
            return false;

        }

        private bool IsCompare(string OpType)
        {
            return OpType == ThreeAddrOpType.Less || OpType == ThreeAddrOpType.Greater 
                                            || OpType == ThreeAddrOpType.GreaterOrEq ||
                                            OpType == ThreeAddrOpType.LessOrEq;
        } 

        string GetOppositeOp(string OpType){
            if (OpType == ThreeAddrOpType.Less)
                return ThreeAddrOpType.Greater;
            if (OpType == ThreeAddrOpType.Greater)
                return ThreeAddrOpType.Less;
            if (OpType == ThreeAddrOpType.LessOrEq)
                return ThreeAddrOpType.GreaterOrEq;
            if (OpType == ThreeAddrOpType.GreaterOrEq)
                return ThreeAddrOpType.LessOrEq;
            return OpType;
        }

        private bool IsCommutative(string OpType)
        {
            return (OpType == ThreeAddrOpType.And || OpType == ThreeAddrOpType.Or || OpType == ThreeAddrOpType.Mul || OpType == ThreeAddrOpType.Plus ||
                    OpType == ThreeAddrOpType.Eq || OpType == ThreeAddrOpType.UnEq) 
        }
    }


}
