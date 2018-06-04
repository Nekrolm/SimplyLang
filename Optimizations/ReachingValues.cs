using System;
using System.Collections.Generic;
using ThreeAddr;
using SimpleLang.Utility;
using System.Linq;

namespace SimpleLang.Optimizations
{
    using ValueSet = Dictionary<String, String>;


    public static class ReachingValues
    {
        public static ValueSet TransferByBBlock(ValueSet In, BaseBlock bblock)
        {
            ValueSet Out = new ValueSet();
            foreach (var vk in In)
                Out[vk.Key] = vk.Value;
            
            foreach (var line in bblock.Code){
                if (ThreeAddrOpType.IsDefinition(line.OpType)){
                    Out[line.Accum] = Compute(line, Out);
                }    
            }

            return Out;
        }


        public static ValueSet Combine(IEnumerable<ValueSet> valSets){
            ValueSet ret = new ValueSet();

            foreach (var vs in valSets){
                foreach (var vk in vs){
                    if (ret.ContainsKey(vk.Key))
                    {
                        if (ret[vk.Key] != vk.Value)
                            ret[vk.Key] = "NAC";
                    }
                    else
                        ret[vk.Key] = vk.Value;
                }
            }

            return ret;
        }



        public static String Compute(ThreeAddrLine line, ValueSet vals)
        {
            if (line.OpType == ThreeAddrOpType.Read){
                return "NAC";
            }


            int a = 0, b = 0;
            bool isaconst = true;
            bool isbconst = true;

            if (!ComputeHelper.IsConst(line.LeftOp)){
                if (line.LeftOp != null){
                    isaconst = int.TryParse(vals[line.LeftOp], out a);
                }
            }else{
                a = int.Parse(line.LeftOp);
            }

            if (!ComputeHelper.IsConst(line.RightOp))
            {
                isbconst = int.TryParse(vals[line.RightOp], out b);
            }else{
                b = int.Parse(line.RightOp);
            }

            if (line.OpType == ThreeAddrOpType.Assign)
            {
                if (isbconst)
                {
                    return b.ToString();
                }
                else
                    return vals[line.RightOp];
            }


            if (!isaconst || !isbconst)
                return "NAC";

            return ComputeHelper.Calculate(a, b, line.OpType).ToString();

        }



    }
}
