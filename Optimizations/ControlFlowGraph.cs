using System;
using System.Linq;
using System.Collections.Generic;
using ThreeAddr;


namespace SimpleLang.Optimizations
{

    using GenSet = Dictionary<String, int>;
    using KillSet = Dictionary<int, String>;
    using LabelSet = HashSet<int>;

    public class ControlFlowGraph
    {
        public Dictionary<int, List<int>> Next { get; set; }
        public Dictionary<int, List<int>> Prev { get; set; }
        public int StartBlockId { get; set; }

        private Dictionary<int, BaseBlock> _baseBlockByStart;
        private Dictionary<int, GenSet> _genByStart;
        private Dictionary<int, KillSet> _killByStart;

        public ControlFlowGraph(List<BaseBlock> baseBlocks)
        {
            Next = new Dictionary<int, List<int>>();
            Prev = new Dictionary<int, List<int>>();

            StartBlockId = baseBlocks[0].StartLabel;
            _baseBlockByStart = new Dictionary<int, BaseBlock>();
            _genByStart = new Dictionary<int, GenSet>();
            _killByStart = new Dictionary<int, KillSet>();

            foreach (var bblock in baseBlocks)
            {
                _baseBlockByStart[bblock.StartLabel] = bblock;
                Prev[bblock.StartLabel] = new List<int>();
                Next[bblock.StartLabel] = new List<int>();
            }



            for (int i = 0; i < baseBlocks.Count; ++i)
            {
                var bblock = baseBlocks[i];

                if (ThreeAddrOpType.IsGoto(bblock.LastLine.OpType))
                {
                    Next[bblock.StartLabel].Add(int.Parse(bblock.LastLine.RightOp));
                    if (bblock.LastLine.OpType == ThreeAddrOpType.Goto)
                        continue;
                }

                if (i + 1 < baseBlocks.Count)
                {
                    Next[bblock.StartLabel].Add(baseBlocks[i + 1].StartLabel);
                }
            }

            foreach (var vTo in Next)
            {
                foreach (var to in vTo.Value)
                    Prev[to].Add(vTo.Key);
            }

        }


        public void GenerateGenAndKillSets()
        {
            var bblocks = _baseBlockByStart.Select(kp => kp.Value).ToList();
            var (gen, kill) = ReachingDefinitions.GetGenAndKillSets(bblocks);

            for (int i = 0; i < bblocks.Count(); ++i)
            {
                _genByStart[bblocks[i].StartLabel] = gen[i];
                _killByStart[bblocks[i].StartLabel] = kill[i];
            }

        }

        public (List<LabelSet>, List<LabelSet>) GenerateInputOutputDefs(List<BaseBlock> bblocks){
            var In = new List<LabelSet>();
            var Out = new List<LabelSet>();

            var startToId = new Dictionary<int, int>();

            for (int i = 0; i < bblocks.Count(); ++i)
            {
                startToId[bblocks[i].StartLabel] = i;
                In.Add(new LabelSet());
                Out.Add(new LabelSet());
            }

            bool change = true;
            while (change){
                change = false;

                for (int i = 1; i < bblocks.Count(); ++i){
                    var st = bblocks[i].StartLabel;
                    In[i] = new LabelSet(Prev[st].SelectMany(p=>Out[startToId[p]]));
                    int sz = Out[i].Count;

                    Out[i] = ReachingDefinitions.TransferByGenAndKill(In[i], _genByStart[st], _killByStart[st]);

                    change |= sz != Out[i].Count;
                }

            }

            return (In, Out);

        }


        public List<BaseBlock> GetAliveBlocks()
        {
            Queue<int> Q = new Queue<int>(); Q.Enqueue(StartBlockId);
            HashSet<int> visited = new HashSet<int>();
            visited.Add(StartBlockId);
            while (Q.Count > 0)
            {
                var cur = Q.Dequeue();
                foreach (var to in Next[cur])
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
