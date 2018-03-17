using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    // Нужно выполнять после 
    public class CommonSubexpressionOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            // Обход строк всех строк блока
            for (int i = 0; i < bblock.Code.Count; i++)
            {
                ThreeAddrLine line = bblock.Code[i];
                string opType = line.OpType;

                // Яляется ли строка строкой вида: a = b op c
                if (ThreeAddrOpType.IsDefinition(opType) && opType != ThreeAddrOpType.Assign && opType != ThreeAddrOpType.Read)
                {
                   
                    string leftOp = line.LeftOp;
                    string rightOp = line.RightOp;
                    // Console.WriteLine("    " + leftOp + ":" + rightOp);

                    // Обратный обход строк (начиная с (i-1)-ой), предшествующих i-ой строке
                    for (int j = i - 1; j >= 0; j--)
                    {   
                        
                        ThreeAddrLine prevLine = bblock.Code[j];

                        // Проверяем изменялись ли leftOp и rightOp
                        if ((prevLine.Accum == leftOp) || (prevLine.Accum == rightOp))
                        {
                            //Console.WriteLine("    " + leftOp + ":" + rightOp);
                            break;   
                        }


                        if ((leftOp == prevLine.LeftOp) && (opType == prevLine.OpType) && (rightOp == prevLine.RightOp) )
                        {
                            for (int k = j + 1; k < i; k++)
                            {
                                ThreeAddrLine nextLine = bblock.Code[k];
                                if (nextLine.Accum == prevLine.Accum){
                                    return false;
                                }
                            }


                            line.OpType = ThreeAddrOpType.Assign;
                            line.LeftOp = null;
                            line.RightOp = prevLine.Accum;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
