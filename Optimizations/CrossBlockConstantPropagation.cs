using System;
using System.Collections.Generic;
using ThreeAddr;

using SimpleLang.Utility;

namespace SimpleLang.Optimizations
{
    using ValueSet = Dictionary<String, String>;

    public class CrossBlockConstantPropagation : CrossBlocksOptimization
    {
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);

            var Invals = CFG.GenerateInputOutputValues(codeBlocks).Item1;

            bool ret = false;

            for (int i = 0; i < codeBlocks.Count; ++i){
                if (OptimizeBlock(codeBlocks[i], Invals[i]))
                    ret = true;
            }

            return ret;

        }

        private bool OptimizeBlock(BaseBlock bblock, ValueSet vals){
            bool ret = false;
            foreach (var line in bblock.Code){
                if (Update(line, vals))
                    ret = true;
            }
            return ret;
        }

        private bool Update(ThreeAddrLine line, ValueSet vals){
            if (!ThreeAddrOpType.IsDefinition(line.OpType)) return false;

            var ret = ReachingValues.Compute(line, vals);

            vals[line.Accum] = ret;
            bool ok = false;


            if (ComputeHelper.IsConst(ret)){
                
                if (ThreeAddrOpType.Assign == line.OpType && line.RightOp == ret)
                    return false;

                line.OpType = ThreeAddrOpType.Assign;
                line.LeftOp = null;
                line.RightOp = ret;



                return true;
            }



            if (line.LeftOp != null && vals.ContainsKey(line.LeftOp))
            {
                if (vals[line.LeftOp] != "NAC"){
                    line.LeftOp = vals[line.LeftOp];
                    ok = true;
                }
            }


            if (line.RightOp != null && vals.ContainsKey(line.RightOp))
            {
                if (vals[line.RightOp] != "NAC")
                {
                    line.RightOp = vals[line.RightOp];
                    ok = true;
                }
            }


            return ok;
        }

    }
}
