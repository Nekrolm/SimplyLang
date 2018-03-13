using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;


namespace SimpleLang.Optimizations
{
    class NopDeleteOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            List<int> bb = new List<int>();
            int i = 0;
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {   
                 if (line.OpType == "nop")
                     bb.Add(i);
                i++;                  
            }
            
            if (bb.Count() != 0)
                if(bb.Count() == bblock.Code.Count())
                    bb.RemoveAt(0);
            if (bb.Count()!=0)
                answer = true;

            i = 0;
            foreach (var line in bb) // Проход по всему базовому блоку.
            { 
                bblock.Code.RemoveAt(line-i);
                i++;
            }
            if (bb.Count() != 0)
            {
                if (bb[0] != 0)
                    i = bblock.StartLabel;
                else
                {
                    int val = 0;
                    while ((bb.Count() > val+1) && ((bb[val + 1] - val) == 1))
                        val = bb[val + 1];
                    i = bblock.StartLabel - val-1;
                }
                foreach (var line in bblock.Code) // Проход по всему базовому блоку.
                {
                    line.Label = i.ToString();
                    i++;
                }
            }

            return answer;
        }
    }
}
