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
            List<int> bbFromDelete = new List<int>(); // Здесь будет храниться номера строк блока, в которых nop
            int i = 0;
            int startLabel = bblock.StartLabel;
            //ищем строки с nop
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {
                if (line.OpType == ThreeAddrOpType.Nop)
                    bbFromDelete.Add(i);
                i++;
            }

            //если блок состоит из nop, то оставляем только первый nop
            if ((bblock.Code.Count() != 0) && (bbFromDelete.Count() == bblock.Code.Count()))
                bbFromDelete.RemoveAt(0);
            //проверяем была ли найдена хоть один nop
            if (bbFromDelete.Count() != 0)
            {
                //удаляе строки nop
                i = 0;
                foreach (var line in bbFromDelete) // Проход по всему базовому блоку.
                {
                    bblock.Code.RemoveAt(line - i);
                    i++;
                }
                //далее обновляем метки
                i = startLabel;
                foreach (var line in bblock.Code) // Проход по всему базовому блоку.
                {
                    line.Label = i.ToString();

                    i++;
                }

                answer = true;
            }
            return answer;
        }
    }
}
