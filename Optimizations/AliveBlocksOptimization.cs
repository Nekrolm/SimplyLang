using System;
using ThreeAddr;
using System.Collections.Generic;

namespace SimpleLang.Optimizations
{
    public class AliveBlocksOptimization : CrossBlocksOptimization
    {   
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var code = BaseBlockHelper.JoinBaseBlocks(codeBlocks);

            int startsz = code.Count;

            BaseBlockHelper.FixLabelsNumeration(code);

            var ncodeBlocks = BaseBlockHelper.GenBaseBlocks(code);
            var CFG = new ControlFlowGraph(ncodeBlocks);
            ncodeBlocks = CFG.GetAliveBlocks();

            code = BaseBlockHelper.JoinBaseBlocks(ncodeBlocks);
            BaseBlockHelper.FixLabelsNumeration(code);
            codeBlocks.Clear();
            codeBlocks.InsertRange(0, BaseBlockHelper.GenBaseBlocks(code));
            return startsz != code.Count;
        }
    }
}
