using System;
using ThreeAddr;
namespace SimpleLang.Optimizations
{
    public class TemporaryExprPropagation : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            for (int i = 0; i < bblock.Code.Count; ++i){

                var line = bblock.Code[i];

                if (ThreeAddrOpType.IsDefinition(line.OpType) && line.Accum.StartsWith("p") ){

                    bool ok = false;
                    bool can_del = true;

                    for (int j = i+1; j < bblock.Code.Count; ++j){

                        var line_acc = bblock.Code[j];
                    
                        if (line_acc.OpType == ThreeAddrOpType.Assign && line.Accum == line_acc.RightOp){
                            line_acc.OpType = line.OpType;
                            line_acc.RightOp = line.RightOp;
                            line_acc.LeftOp = line.LeftOp;
                            ok = true;
                            continue;
                        }

                        if (line.Accum == line_acc.RightOp || line.Accum == line_acc.LeftOp ){
                            can_del = false;
                        }
                    }

                    if (ok){
                        if (can_del){
                            line.OpType = ThreeAddrOpType.Nop;
                        }
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
