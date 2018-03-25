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

                    for (int j = i + 1; j < bblock.Code.Count; ++j)
                    {   
                        
                        ThreeAddrLine nextLine = bblock.Code[j];


                        if ((leftOp == nextLine.LeftOp) && (opType == nextLine.OpType) && (rightOp == nextLine.RightOp) )
                        {
                            nextLine.OpType = ThreeAddrOpType.Assign;
                            nextLine.LeftOp = null;
                            nextLine.RightOp = line.Accum;
                            return true;
                        }

                        if (nextLine.Accum == line.Accum ||
                            nextLine.Accum == rightOp ||
                            nextLine.Accum == leftOp)
                            break;

                    }
                }
            }
            return false;
        }

    }
}
