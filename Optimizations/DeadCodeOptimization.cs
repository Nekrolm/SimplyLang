using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    class DeadCodeOptimization : BaseBlockOptimization
    {
        HashSet<String> deathVariable = new HashSet<String>();
        HashSet<String> liveVariable = new HashSet<String>();

        private void UpdateLive(String varName)
        {
            liveVariable.Add(varName);
            deathVariable.Remove(varName);
        }

        private void UpdateDead(String varName)
        {
            liveVariable.Remove(varName);
            deathVariable.Add(varName);
        }

        private bool IsConst(String varName)
        {
            int v;
            return int.TryParse(varName, out v);
        }

        public override bool Optimize(BaseBlock bblock)
        {
            bool answer = false; // Индикатор того, что хоть один раз, но оптимизация была выполнена.

            deathVariable.Clear();
            liveVariable.Clear();

            for (int i = bblock.Code.Count() - 1; i >= 0; --i) // Проход по всему базовому блоку.
            {
                if ((bblock.Code[i].OpType == ThreeAddrOpType.Write) && !IsConst(bblock.Code[i].RightOp))
                {
                    UpdateLive(bblock.Code[i].RightOp);
                    continue;
                }
                if (bblock.Code[i].OpType == ThreeAddrOpType.Read)
                {
                    UpdateDead(bblock.Code[i].Accum);
                    continue;
                }
                if (ThreeAddrOpType.IsDefinition(bblock.Code[i].OpType))
                {
                    if (deathVariable.Contains(bblock.Code[i].Accum) ||
                        (bblock.Code[i].Accum.StartsWith("p") && !liveVariable.Contains(bblock.Code[i].Accum)))
                    {
                        bblock.Code[i].OpType = ThreeAddrOpType.Nop;
                        answer = true;
                    }
                    else
                    {
                        UpdateDead(bblock.Code[i].Accum);
                    }

                    if (!IsConst(bblock.Code[i].RightOp))
                    {
                        UpdateLive(bblock.Code[i].RightOp);
                    }
                    if (bblock.Code[i].LeftOp != null && !IsConst(bblock.Code[i].LeftOp))
                    {
                        UpdateLive(bblock.Code[i].RightOp);
                    }
                    continue;
                }
                if (bblock.Code[i].OpType == ThreeAddrOpType.IfGoto){
                    UpdateLive(bblock.Code[i].LeftOp);
                }

            }

            return answer;
        }
    }
}
