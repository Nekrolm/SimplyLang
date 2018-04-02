using System;
using System.Collections.Generic;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    public class CrossBlocksDeadCodeOptimization : CrossBlocksOptimization
    {
        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);

            var active = CFG.GenerateInputOutputActiveDefs(codeBlocks).Item2;

            bool ret = false;

            for (int i = 0; i < codeBlocks.Count; ++i)
                ret |= OptimizeBlock(codeBlocks[i], active[i]);
            return ret;
        }


        private bool OptimizeBlock(BaseBlock bblock, HashSet<String> active_vars)
        {
            bool ret = false;


            Console.WriteLine("Try opt!");
            Console.WriteLine(bblock.ToString());
            Console.WriteLine("Defs");
            foreach (var s in active_vars){
                Console.Write(s + " ");
            }
            Console.WriteLine();



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
                if (line.RightOp != null && int.TryParse(line.RightOp, out var x))
                    living.Add(line.RightOp);
                if (line.LeftOp != null && int.TryParse(line.LeftOp, out var f))
                    living.Add(line.LeftOp);
            }
            return ret;
        }

    }
}
