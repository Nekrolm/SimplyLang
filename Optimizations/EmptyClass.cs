/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreeAddr;

namespace SimpleLang.Optimizations
{
    // Совершает только одну протяжку
    public class CopyPropagationOptimization : BaseBlockOptimization
    {
        public override bool Optimize(BaseBlock bblock)
        {
            bool res = false;

            // Обход строк всех строк блока, исключая строки вида a:=const
            for (int i = 0; i < bblock.Code.Count; i++)
            {
                ThreeAddrLine line = bblock.Code[i];

                if (IsAssignConst(line)) // исключаем
                {

                    continue;
                }

                // Обратный обход строк (начиная с (i-1)-ой), предшествующих i-ой строке и являющихся строкой вида a:=const
                for (int j = i - 1; j >= 0; j--)
                {

                    ThreeAddrLine assignLine = bblock.Code[j];

                    if (!IsAssignConst(line))
                    {
                        continue;
                    }

                    // Если нашлась Assign-строка, у которой Accum равен (левому или правому) операнду в current-строке
                    if (assignLine.Accum == line.LeftOp)
                    {
                        // Заменяем операнд в текущей строке, операндом из Assign-строки
                        line.LeftOp = assignLine.RightOp;
                        res = true;

                    }
                    if (assignLine.Accum == line.RightOp)
                    {
                        // Заменяем операнд в текущей строке, операндом из Assign-строки
                        line.RightOp = assignLine.RightOp;
                        res = true;
                    }

                    if (res)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        // Проверяет, является ли строка присваением константы
        public bool IsAssignConst(ThreeAddrLine line)
        {
            if (line.OpType == ThreeAddrOpType.Assign)
            {
                int b = 0; // Можно ли убрать этот костыль?
                bool isbconst = int.TryParse(line.RightOp, out b); // Получаем правый операнд
                if (isbconst)
                {
                    return true;
                }

            }
            return false;
        }
    }
}*/
