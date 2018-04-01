using ProgramTree;
using SimpleLang.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    class IsNoInitVars
    {
        public bool Optimize(Dictionary<string, int> IDDict, List<BaseBlock> codeBlocks)
        {
            List<String> bInit = new List<String>();
            List<String> bNotInit = new List<String>();
            foreach (var line in codeBlocks) // Проход по всему базовому блоку.
            {
                foreach (var l in line.Code)
                {
                    if ((l.LeftOp != null) && (l.LeftOp.Contains("v")) && (!bInit.Contains(l.LeftOp)) && (!bNotInit.Contains(l.LeftOp)))
                    {
                        bNotInit.Add(l.LeftOp);
                    }
                    if ((l.RightOp != null) && (l.RightOp.Contains("v")) && (!bInit.Contains(l.RightOp)) && (!bNotInit.Contains(l.RightOp)))
                    {
                        bNotInit.Add(l.RightOp);
                    }
                    if ((l.Accum != null) && (l.Accum.Contains("v")) && (!bInit.Contains(l.Accum)) && (!bNotInit.Contains(l.Accum)))
                    {
                        bInit.Add(l.Accum);
                    }
                }
            }

            List<String> bConvertNotInit = new List<String>();
          
            foreach(var i in bNotInit)
            {
                String key = IDDict.First(a =>i.Contains("v"+ a.Value)).Key;
                bConvertNotInit.Add(key);
            }

            try
            {
                if (bConvertNotInit.Count() != 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                foreach (var i in bConvertNotInit)
                    Console.WriteLine("Переменная {0} не инициализирована", i);
                return false;
            }

            return true;
        }
    }
}
