using System;
using System.Collections.Generic;
using ThreeAddr;
using System.Linq;

namespace SimpleLang.Optimizations
{
    public static class ReachingDefinitions
    {
        public static HashSet<int> GetGenSet(BaseBlock bblock)
        {
            var ret = new HashSet<int>();
            var used = new HashSet<String>();

            //foreach (var line in bblock.Code.Reversed) 
            return null;
        }
    }
}
