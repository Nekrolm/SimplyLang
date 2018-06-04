using System;
using System.Collections.Generic;
using System.Linq;
using ThreeAddr;
using SimpleLang.Utility;

namespace SimpleLang.Optimizations
{
    class IsNotInitVariable : CrossBlocksOptimization
    {
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            //var CFG = new ControlFlowGraph(codeBlocks);

            //неиспользую
            //var active = CFG.GenerateInputOutputActiveDefs(codeBlocks).Item1;
            //var active1 = CFG.GenerateInputOutputActiveDefs(codeBlocks).Item2;

            HashSet<String> bInit = new HashSet<String>();
            HashSet<String> bNotInit = new HashSet<string>();
            foreach (var l in BaseBlockHelper.JoinBaseBlocks(codeBlocks))
            {
                if ((l.LeftOp != null) && (l.LeftOp.Contains("v")) && (!bInit.Contains(l.LeftOp)))
                {
                    bNotInit.Add(l.LeftOp);
                }
                if ((l.RightOp != null) && (l.RightOp.Contains("v")) && (!bInit.Contains(l.RightOp)))
                {
                    bNotInit.Add(l.RightOp);
                }
                if (ThreeAddrOpType.IsDefinition(l.OpType))
                {
                    bInit.Add(l.Accum);
                }
            }

            if (bNotInit.Count() > 0)
                throw new NotInitVariableException(bNotInit.ToList(), "Ошибка");
            return false;
        }
    }
}
