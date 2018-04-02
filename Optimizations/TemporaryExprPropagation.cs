using System;
using ThreeAddr;
namespace SimpleLang.Optimizations
{
    public class TemporaryExprPropagation : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            foreach(var line in bblock.Code){
                if (ThreeAddrOpType.IsDefinition(line.OpType) && line.Accum.StartsWith("p") ){
                    foreach (var line_acc in bblock.Code){
                        if (line_acc.OpType == ThreeAddrOpType.Assign && line.Accum == line_acc.RightOp){
                            line_acc.OpType = line.OpType;
                            line_acc.RightOp = line.RightOp;
                            line_acc.LeftOp = line.LeftOp;
                            line.OpType = ThreeAddrOpType.Nop;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
