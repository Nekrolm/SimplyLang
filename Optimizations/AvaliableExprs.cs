using System;
using System.Collections.Generic;
using System.Linq;
using ThreeAddr;


namespace SimpleLang.Optimizations
{
    // arg1 op arg2
    using ExprSet = HashSet<(String, String, String)>;
    using KillerSet = HashSet<String>;

    public static  class AvaliableExprs
    {
        public static ExprSet GetGenExprSet(BaseBlock bblock)
        {
            var ret = new ExprSet();
            foreach (var line in bblock.Code)
            {
                if (ThreeAddrOpType.IsGoto(line.OpType) || line.OpType == ThreeAddrOpType.Nop ||
                    line.OpType == ThreeAddrOpType.Write) continue;

                ret.RemoveWhere(x => x.Item1 == line.Accum || x.Item3 == line.Accum);

                if (ThreeAddrOpType.Computable.Contains(line.OpType)){
                    ret.Add( (line.LeftOp, line.OpType, line.RightOp)  );
                }
            }            
            return ret;
        }

        public static KillerSet GetKillerSet(BaseBlock bblock){
            return new KillerSet(bblock.Code
                                 .Where(l => ThreeAddrOpType.IsDefinition(l.OpType))
                                 .Select(l => l.Accum));
        }


        public static (List<ExprSet>, List<KillerSet>) GetGenAndKillerSets(List<BaseBlock> bblocks)
        {
            return (bblocks.Select(b => GetGenExprSet(b)).ToList(),
                    bblocks.Select(b => GetKillerSet(b)).ToList());

        }



        public static ExprSet TransferByGenAndKiller(ExprSet X, ExprSet gen, KillerSet kill)
        {
            if (X == null) return gen;
            return new ExprSet(X.Where(e => !kill.Contains(e.Item1) && !kill.Contains(e.Item3))
                               .Union(gen));
        }


    }
}
