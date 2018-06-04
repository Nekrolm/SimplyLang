using System;
using System.Collections.Generic;
using ThreeAddr;
using SimpleLang.Utility;

namespace SimpleLang.Optimizations
{
    public class CrossBlocksDeadCodeOptimization : CrossBlocksOptimization
    {
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);

            var active = CFG.GenerateInputOutputActiveDefs().Item2;

            bool ret = false;

            for (int i = 0; i < codeBlocks.Count; ++i)
                ret |= OptimizeBlock(codeBlocks[i], active[i]);
            return ret;
        }


        private bool OptimizeBlock(BaseBlock bblock, HashSet<String> active_vars)
        {
            bool ret = false;


            HashSet<String> living = new HashSet<string>();
            for (int i = bblock.Code.Count - 1; i >= 0; --i){
                var line = bblock.Code[i];
                if (ThreeAddrOpType.IsDefinition(line.OpType) & line.OpType != ThreeAddrOpType.Read){
                    if (!living.Contains(line.Accum) && !active_vars.Contains(line.Accum) ){
                        line.OpType = ThreeAddrOpType.Nop;
                        ret = true;
                        continue;
                    }
                }
                if (line.RightOp != null && !ComputeHelper.IsConst(line.RightOp))
                    living.Add(line.RightOp);
                if (line.LeftOp != null && !ComputeHelper.IsConst(line.LeftOp))
                    living.Add(line.LeftOp);
            }
            return ret;
        }

    }
}
