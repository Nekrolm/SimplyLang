using System;
using System.Collections.Generic;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    public abstract class BaseBlockOptimization
    {
        // returns true if optimization was succesfully applied;
        public abstract bool Optimize(BaseBlock bblock);
    }


    public abstract class CrossBlocksOptimization
    {

        // returns true if optimization was succesfully applied;
        public abstract bool Optimize(List<BaseBlock> code);
    }


    public class BaseBlockOptimizator{
        private List<BaseBlockOptimization> _opts = new List<BaseBlockOptimization>();

        public void AddOptimization(BaseBlockOptimization opt){_opts.Add(opt);}

        public bool Optimize(List<BaseBlock> code){
            bool res = false;

            foreach (var bblock in code){
                res |= OptimizeBlock(bblock);
            }

            return res;
        }

        private bool OptimizeBlock(BaseBlock bblock){
            int i = 0;
            bool res = false;
            while (i < _opts.Count)
            {
                bool applied = false;
                while (_opts[i].Optimize(bblock)){
                    applied = true;
                }
                if (applied)
                {
                    res = true;
                    i = 0;
                }
                else
                    ++i;
            }
            return res;
        }

    }

}
