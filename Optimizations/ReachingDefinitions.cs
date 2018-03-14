using System;
using System.Collections.Generic;
using ThreeAddr;
using System.Linq;

namespace SimpleLang.Optimizations
{

    using GenSet = Dictionary<String, int>;
    using KillSet = Dictionary<int, String>;
    using LabelSet = HashSet<int>;

    public static class ReachingDefinitions
    {

        public static LabelSet ToLabelSet(GenSet s){
            return new LabelSet(s.Select(kp => kp.Value));
        }

        public static LabelSet ToLabelSet(KillSet s)
        {
            return new LabelSet(s.Select(kp => kp.Key));
        }


        public static GenSet GetGenSet(BaseBlock bblock)
        {
            var ret = new GenSet();
           

            foreach (var line in bblock.Code.AsEnumerable().Reverse())
            {
                if (ThreeAddrOpType.IsDefinition(line.OpType)){
                    if (!ret.ContainsKey(line.Accum)){
                        ret[line.Accum] = int.Parse(line.Label);
                    }
                }
            }
            return ret;
        }

        public static KillSet GetKillSet(GenSet bblock, List<GenSet> others)
        {
            var ret = new KillSet();

            foreach (var oblock in others)
            {
                foreach(var def in bblock)
                    if (oblock.ContainsKey(def.Key)){
                        ret[oblock[def.Key]] = def.Key;
                    }
            }

            return ret;
        }


        public static (List<GenSet>, List<KillSet>) GetGenAndKillSets(List<BaseBlock> bblocks)
        {
            var gen = bblocks.Select(b => GetGenSet(b)).ToList();
            var kill = new List<KillSet>();
            for (int i = 0; i < bblocks.Count; ++i){
                var others = new List<GenSet>();
                for (int j = 0; j < bblocks.Count(); ++j)
                    if (i != j)
                        others.Add(gen[j]);
                kill.Add(GetKillSet(gen[i], others));
            }

            return (gen, kill);
        }


        public static LabelSet TransferByGenAndKill(LabelSet X, GenSet gen, KillSet kill)
        {
            if (X == null) 
                return ToLabelSet(gen);
            return new LabelSet(X.Except(ToLabelSet(kill)).Union(ToLabelSet(gen)));
        }

    }



}
