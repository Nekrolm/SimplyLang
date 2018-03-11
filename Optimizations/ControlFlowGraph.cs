using System;
using System.Linq;
using System.Collections.Generic;
using ThreeAddr;


namespace SimpleLang.Optimizations
{
    public class ControlFlowGraph
    {
        public Dictionary<int, List<int>> Graph { get; set; }
        public int StartBlockId { get; set; }

        private Dictionary<int, BaseBlock> _baseBlockByStart;

        public ControlFlowGraph(List<BaseBlock> baseBlocks)
        {
            Graph = new Dictionary<int, List<int>>();
            StartBlockId = baseBlocks[0].StartLabel;
            _baseBlockByStart = new Dictionary<int, BaseBlock>();
            foreach (var bblock in baseBlocks)
                _baseBlockByStart[bblock.StartLabel] = bblock;

            for (int i = 0; i < baseBlocks.Count; ++i)
            {
                var bblock = baseBlocks[i];
                Graph[bblock.StartLabel] = new List<int>();


                if (ThreeAddrOpType.IsGoto(bblock.LastLine.OpType))
                {
                    Graph[bblock.StartLabel].Add(int.Parse(bblock.LastLine.RightOp));
                    if (bblock.LastLine.OpType == ThreeAddrOpType.Goto)
                        continue;
                }

                if (i + 1 < baseBlocks.Count)
                {
                    Graph[bblock.StartLabel].Add(baseBlocks[i + 1].StartLabel);
                }
            }

        }


        public List<BaseBlock> GetAliveBlocks()
        {
            Queue<int> Q = new Queue<int>(); Q.Enqueue(StartBlockId);
            HashSet<int> visited = new HashSet<int>();
            visited.Add(StartBlockId);
            while (Q.Count > 0)
            {
                var cur = Q.Dequeue();
                foreach (var to in Graph[cur])
                {
                    if (!visited.Contains(to))
                    {
                        visited.Add(to);
                        Q.Enqueue(to);
                    }

                }
            }
              
            return visited.OrderBy(x=>x)
                          .Select(x=>_baseBlockByStart[x]).ToList();

        }

    }

}
