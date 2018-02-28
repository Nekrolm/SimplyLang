using System;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    public abstract class Optimization
    {
        // returns true if optimization was succesfully applied;
        public abstract bool Optimize(BaseBlock bblock);
    }
}
