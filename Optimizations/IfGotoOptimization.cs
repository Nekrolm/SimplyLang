using System;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    public class IfGotoOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock){
            var lastLine = bblock.LastLine;
            if (lastLine == null)
                return false;
            if (lastLine.OpType == ThreeAddrOpType.IfGoto){
                int val = 0;
                if (!int.TryParse(lastLine.LeftOp, out val))
                    return false;

                if (val == 0){
                    lastLine.Accum = null;
                    lastLine.LeftOp = null;
                    lastLine.RightOp = null;
                    lastLine.OpType = ThreeAddrOpType.Nop;
                }else{
                    lastLine.Accum = null;
                    lastLine.LeftOp = null;
                    lastLine.OpType = ThreeAddrOpType.Goto;
                }
                return true;
            }
            return false;
        }
    }
}
