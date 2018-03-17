using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    class DeleteDeathCodeOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.
            List<String> deathVariable = new List<String>();
            List<String> liveVariable = new List<String>();
            bool flag = true;
           
            while (flag)
            {
                deathVariable.Clear();
                liveVariable.Clear();
                flag = false;
               
              //  Console.WriteLine(bblock.Code.Count() - 1);
                for (int i = bblock.Code.Count()-1; i>=0; i=i-1) // Проход по всему базовому блоку.
                {
                    char[] findCh = { 'v', 'p' };
                    if ((bblock.Code[i].OpType == ThreeAddrOpType.Write)&&(bblock.Code[i].RightOp.IndexOfAny(findCh)!= -1))
                    {
                        liveVariable.Add(bblock.Code[i].RightOp);
                        liveVariable = liveVariable.Distinct().ToList();
                        if (deathVariable.IndexOf(bblock.Code[i].RightOp)!=-1)
                            deathVariable.RemoveAt(deathVariable.IndexOf(bblock.Code[i].RightOp));
                        continue;
                    }
                    if (bblock.Code[i].OpType == ThreeAddrOpType.Read)
                    {
                        if (deathVariable.IndexOf(bblock.Code[i].Accum) != -1)
                        {
                            bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                            answer = true;
                            flag = true;
                        }
                        else
                        {
                            deathVariable.Add(bblock.Code[i].Accum);
                            if (liveVariable.IndexOf(bblock.Code[i].Accum) != -1)
                                liveVariable.RemoveAt(liveVariable.IndexOf(bblock.Code[i].Accum));
                        }
                        continue;
                    }
                    if (bblock.Code[i].OpType == ThreeAddrOpType.Assign)
                    {
                        if (deathVariable.IndexOf(bblock.Code[i].Accum) != -1) 
                        {
                            bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                            answer = true;
                            flag = true;
                        }
                        else
                        {
                            if ((bblock.Code[i].Accum.IndexOf('p') != -1) && (liveVariable.IndexOf(bblock.Code[i].Accum) == -1))
                            {
                                bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                                answer = true;
                                flag = true;
                            }
                            else
                            {
                                deathVariable.Add(bblock.Code[i].Accum);
                                if (liveVariable.IndexOf(bblock.Code[i].Accum) != -1)
                                    liveVariable.RemoveAt(liveVariable.IndexOf(bblock.Code[i].Accum));
                            }
                        }
                        if (bblock.Code[i].RightOp.IndexOfAny(findCh) != -1)
                        {
                            liveVariable.Add(bblock.Code[i].RightOp);
                            liveVariable = liveVariable.Distinct().ToList();
                            if (deathVariable.IndexOf(bblock.Code[i].RightOp) != -1)
                                deathVariable.RemoveAt(deathVariable.IndexOf(bblock.Code[i].RightOp));
                        }
                        continue;
                    }
                    if ((bblock.Code[i].OpType != ThreeAddrOpType.Goto) && (bblock.Code[i].OpType != ThreeAddrOpType.IfGoto) && (bblock.Code[i].OpType != ThreeAddrOpType.Nop))
                    {
                        if ((deathVariable.IndexOf(bblock.Code[i].Accum) != -1)||((bblock.Code[i].RightOp.IndexOf('p')!=-1)&&(liveVariable.IndexOf(bblock.Code[i].Accum) == -1)))
                        {
                            bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                            answer = true;
                            flag = true;
                        }
                        else
                        {
                            if ((bblock.Code[i].Accum.IndexOf('p') != -1) && (liveVariable.IndexOf(bblock.Code[i].Accum) == -1))
                            {
                                bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                                answer = true;
                                flag = true;
                            }
                            else
                            {
                                deathVariable.Add(bblock.Code[i].Accum);
                                if (liveVariable.IndexOf(bblock.Code[i].Accum) != -1)
                                    liveVariable.RemoveAt(liveVariable.IndexOf(bblock.Code[i].Accum));
                            }
                        }
                        if (bblock.Code[i].RightOp.IndexOfAny(findCh) != -1)
                        {
                            liveVariable.Add(bblock.Code[i].RightOp);
                            liveVariable = liveVariable.Distinct().ToList();
                            if (deathVariable.IndexOf(bblock.Code[i].RightOp) != -1)
                                deathVariable.RemoveAt(deathVariable.IndexOf(bblock.Code[i].RightOp));        
                        }
                        if (bblock.Code[i].LeftOp.IndexOfAny(findCh) != -1)
                        {
                            liveVariable.Add(bblock.Code[i].LeftOp);
                            liveVariable = liveVariable.Distinct().ToList();
                            if (deathVariable.IndexOf(bblock.Code[i].LeftOp) != -1)
                                deathVariable.RemoveAt(deathVariable.IndexOf(bblock.Code[i].LeftOp));
                        }
                        continue;
                    }
                 }
            }

            return answer;
        }
    }
}
