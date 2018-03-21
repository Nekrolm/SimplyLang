using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{   
    // Совершает только одну протяжку
    public class CopyPropagationOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            // Обход строк всех строк блока, исключая строки вида a:=const
            for (int i = 0; i < bblock.Code.Count; i++)    
            {
                ThreeAddrLine line = bblock.Code[i];
 
                if (line.OpType == ThreeAddrOpType.Assign)
                {
                    for (int j = i + 1; j < bblock.Code.Count; j++)
                    {
                        var nextLine = bblock.Code[j];
                        if (nextLine.Accum == line.Accum)
                            break;

                        bool res = false;

                        if (nextLine.LeftOp == line.Accum)
                        {
                            nextLine.LeftOp = line.RightOp;
                            res = true;
                        }

                        if (nextLine.RightOp == line.Accum)
                        {
                            res = true;
                            nextLine.RightOp = line.RightOp;
                        }

                        if (res)
                            return true;

                    }    
                }
            }
            return false;
        }

        // Проверяет, является ли строка присваением константы
        public bool IsAssignConst(ThreeAddrLine line)
        {
            if (line.OpType == ThreeAddrOpType.Assign)
            {
                int b = 0; // Можно ли убрать этот костыль?
                bool isbconst = int.TryParse(line.RightOp, out b); // Получаем правый операнд
                if (isbconst)
                {
                    return true;
                }

            }
            return false;
        }

    }
}
