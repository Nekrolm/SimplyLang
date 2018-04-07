using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeAddr;
using SimpleLang.Utility;

namespace SimpleLang.Optimizations
{
    class IsNotInitVariable : CrossBlocksOptimization
    {
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);

            //неиспользую
            var active = CFG.GenerateInputOutputActiveDefs(codeBlocks).Item1;
            var active1 = CFG.GenerateInputOutputActiveDefs(codeBlocks).Item2;

            List<String> bInit = new List<String>();
            List<String> bNotInit = new List<String>();
            foreach (var line in codeBlocks) // Проход по всему базовому блоку.
            {
                foreach (var l in line.Code)
                {
                    if ((l.LeftOp != null) && (l.LeftOp.Contains("v")) && (!bInit.Contains(l.LeftOp)) && (!bNotInit.Contains(l.LeftOp)))
                    {
                        bNotInit.Add(l.LeftOp);
                    }
                    if ((l.RightOp != null) && (l.RightOp.Contains("v")) && (!bInit.Contains(l.RightOp)) && (!bNotInit.Contains(l.RightOp)))
                    {
                        bNotInit.Add(l.RightOp);
                    }
                    if ((l.Accum != null) && (l.Accum.Contains("v")) && (!bInit.Contains(l.Accum)) && (!bNotInit.Contains(l.Accum)))
                    {
                        bInit.Add(l.Accum);
                    }
                }
            }

            if (bNotInit.Count() > 0)
                throw new NotInitVariableException(bNotInit, "Ошибка");
            return false;
        }
    }
}
