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
            List<int> bbLabel = new List<int>(); // Здесь будет храниться номера строк блока, в которых nop
            int i = 0;
            //ищем строки с nop
            foreach (var line in bblock.Code) // Проход по всему базовому блоку.
            {
                if (line.OpType == "nop")
                    bbLabel.Add(i);
                i++;
            }

            //если блок состоит из nop, то оставляем только первый nop
            if ((bblock.Code.Count() != 0) && (bbLabel.Count() == bblock.Code.Count()))
                bbLabel.RemoveAt(0);
            //проверяем была ли найдена хоть один nop
            if (bbLabel.Count() != 0)
            {
                //удаляе строки nop
                i = 0;
                foreach (var line in bbLabel) // Проход по всему базовому блоку.
                {
                    bblock.Code.RemoveAt(line - i);
                    i++;
                }
                //далее обновляем метки
                if (bbLabel[0] != 0)
                    i = bblock.StartLabel;
                else
                {
                    int val = 0;
                    while ((bbLabel.Count() > val + 1) && ((bbLabel[val + 1] - val) == 1))
                        val = bbLabel[val + 1];
                    i = bblock.StartLabel - val - 1;
                }
                
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
