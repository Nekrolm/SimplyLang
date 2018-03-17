using System;
using System.Collections.Generic;
using System.Linq;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    using VarsSet = HashSet<String>;
   
    public static class ActiveDefinitions
    {
        private static bool IsConst(String v){
            int x;
            return int.TryParse(v, out x);
        }

        public static VarsSet GetUseSet(BaseBlock bblock)
        {
            var ret = new VarsSet();
            foreach (var line in bblock.Code)
            {
                if (line.LeftOp != null && !IsConst(line.LeftOp))
                    ret.Add(line.LeftOp);
                if (line.RightOp != null && !IsConst(line.RightOp))
                    ret.Add(line.RightOp);
            }

            return ret;
        }

        public static VarsSet GetDefSet(BaseBlock bblock)
        {
            return new VarsSet(bblock.Code
                               .Where(l => ThreeAddrOpType.IsDefinition(l.OpType))
                               .Select(l => l.Accum));
        }


        public static (List<VarsSet>, List<VarsSet>) GetUseAndDefSets(List<BaseBlock> bblocks)
        {
            return (bblocks.Select(b => GetUseSet(b)).ToList(),
                    bblocks.Select(b => GetDefSet(b)).ToList());
            
        }


        public static VarsSet TransferByUseAndDef(VarsSet X, VarsSet use, VarsSet def)
        {
            if (X == null)
                return use;
            return new VarsSet(X.Except(def).Union(use));
        }


    }
}
