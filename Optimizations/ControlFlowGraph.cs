﻿using System;
using System.Linq;
using System.Collections.Generic;
using ThreeAddr;


namespace SimpleLang.Optimizations
{

    using GenSet = Dictionary<String, int>;
    using KillSet = Dictionary<int, String>;
    using LabelSet = HashSet<int>;
    using VarsSet = HashSet<String>;
    using ValueSet = Dictionary<String, String>;
    using ExprSet = HashSet<(String, String, String)>;


    public class ControlFlowGraph
    {
        public Dictionary<int, List<int>> Next { get; set; }
        public Dictionary<int, List<int>> Prev { get; set; }
        public int StartBlockId { get; set; }

        private Dictionary<int, BaseBlock> _baseBlockByStart;
        private Dictionary<int, GenSet> _genByStart;
        private Dictionary<int, KillSet> _killByStart;
        private Dictionary<int, VarsSet> _useByStart;
        private Dictionary<int, VarsSet> _defByStart;
        private Dictionary<int, ExprSet> _genExprByStart;


        private List<BaseBlock> _bblocks;


        public IEnumerable<BaseBlock> PrevBlocks(BaseBlock bblock)
        {
            return Prev[bblock.StartLabel].Select(id => _baseBlockByStart[id]);
        }

        public ControlFlowGraph(List<BaseBlock> baseBlocks)
        {
            Next = new Dictionary<int, List<int>>();
            Prev = new Dictionary<int, List<int>>();

            StartBlockId = baseBlocks[0].StartLabel;

            _bblocks = baseBlocks;

            _baseBlockByStart = new Dictionary<int, BaseBlock>();
            _genByStart = new Dictionary<int, GenSet>();
            _killByStart = new Dictionary<int, KillSet>();
            _useByStart = new Dictionary<int, VarsSet>();
            _defByStart = new Dictionary<int, VarsSet>();
            _genExprByStart = new Dictionary<int, ExprSet>();

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


            GenerateGenAndKillSets();
            GenerateUseAndDefSets();
            GenerateGenExprSets();
        }


        private void GenerateGenExprSets()
        {
            var bblocks = _baseBlockByStart.Select(kp => kp.Value).ToList();

            foreach (var bblock in bblocks)
            {
                _genExprByStart[bblock.StartLabel] = AvaliableExprs.GetGenExprSet(bblock);
            }

        }
    
        private void GenerateUseAndDefSets()
        {
            var bblocks = _baseBlockByStart.Select(kp => kp.Value).ToList();
            var (use, def) = ActiveDefinitions.GetUseAndDefSets(bblocks);

            for (int i = 0; i < bblocks.Count(); ++i)
            {
                _useByStart[bblocks[i].StartLabel] = use[i];
                _defByStart[bblocks[i].StartLabel] = def[i];
            } 
        }

        private void GenerateGenAndKillSets()
        {
            var bblocks = _baseBlockByStart.Select(kp => kp.Value).ToList();
            var (gen, kill) = ReachingDefinitions.GetGenAndKillSets(bblocks);

            for (int i = 0; i < bblocks.Count(); ++i)
            {
                _genByStart[bblocks[i].StartLabel] = gen[i];
                _killByStart[bblocks[i].StartLabel] = kill[i];
            }

        }


        private Dictionary<int, int> GetStartToId(List<BaseBlock> bblocks)
        {
            var startToId = new Dictionary<int, int>();

            for (int i = 0; i < bblocks.Count(); ++i)
            {
                startToId[bblocks[i].StartLabel] = i;
            }

            return startToId;
        }


        public (List<VarsSet>, List<VarsSet>) GenerateInputOutputActiveDefs()
        {
            var In = new List<VarsSet>();
            var Out = new List<VarsSet>();

            var startToId = GetStartToId(_bblocks);

            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                In.Add(new VarsSet());
                Out.Add(new VarsSet());
            }

            bool change = true;
            while (change)
            {
                change = false;

                for (int i = 0; i < _bblocks.Count(); ++i)
                {
                    var st = _bblocks[i].StartLabel;
                    Out[i] = new VarsSet(Next[st].SelectMany(p => In[startToId[p]]));
                    int sz = In[i].Count;

                    In[i] = ActiveDefinitions.TransferByUseAndDef(Out[i], _useByStart[st], _defByStart[st]);

                    change |= sz != In[i].Count; 
                }

            }

            return (In, Out);

        }

        private ValueSet GenFullValueSet(IEnumerable<ThreeAddrLine> lines){
            ValueSet ret = new ValueSet();
            foreach (var l in lines)
                if (ThreeAddrOpType.IsDefinition(l.OpType))
                    ret[l.Accum] = "NAC";
            return ret;
        }

        public (List<ValueSet>, List<ValueSet>) GenerateInputOutputValues()
        {
            var In = new List<ValueSet>();
            var Out = new List<ValueSet>();

            var startToId = GetStartToId(_bblocks);

            var code = BaseBlockHelper.JoinBaseBlocks(_bblocks);

            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                In.Add(GenFullValueSet(code));
                Out.Add(GenFullValueSet(code));
            }


            bool change = true;
            while (change)
            {
                change = false;

                for (int i = 0; i < _bblocks.Count; ++i)
                {
                    var st = _bblocks[i].StartLabel;

                    if (Prev[st].Count() != 0){
                        In[i] = ReachingValues.Combine(Prev[st].Select(l => Out[startToId[l]]));
                    }

                    var nOut = ReachingValues.TransferByBBlock(In[i], _bblocks[i]);

                    foreach (var vk in nOut)
                        if (vk.Value != Out[i][vk.Key]){
                            change = true;
                            break;
                        }

                    Out[i] = nOut;
                }

            }

            return (In, Out);

        }


        public bool IsReducible(){
            var ret = TopSort();

            var retreat = ret.Item2;
            var reverse = ret.Item3;

            return retreat.IsSubsetOf(reverse) && reverse.IsSubsetOf(retreat);

        }

        // returns sorted order of bblocks, retreat edges, reverse edges 
        // in terms of initial bblocks ordering
        public (List<int>, HashSet<(int, int)>, HashSet<(int,int)>) TopSort(){
            
            var order = new List<int>();

            var used = new HashSet<int>();

            dfs(StartBlockId, used, order);

            order.Reverse();

            Dictionary<int, int> orderedToId = new Dictionary<int, int>();
            for (int i = 0; i < order.Count; ++i){
                orderedToId[order[i]] = i;
            }

            var startToId = GetStartToId(_bblocks);
            var domms = FindDommBlocks();

            HashSet<(int, int)> retreat = new HashSet<(int, int)>();
            HashSet<(int, int)> reverse = new HashSet<(int, int)>();




            foreach(var v in order){
                foreach (var to in Next[v])
                {
                    if (orderedToId[v] >= orderedToId[to]){
                        retreat.Add((startToId[v], startToId[to]));
                    }

                    if (domms[startToId[v]].Contains(startToId[to])){
                        reverse.Add((startToId[v], startToId[to]));
                    }
                }
            }


            return (order.Select(st => startToId[st]).ToList(), retreat, reverse);

        }

        private void dfs(int v, HashSet<int> used, List<int> order)
        {
            if (used.Contains(v)) return;

            used.Add(v);
            foreach(var to in Next[v]){
                dfs(to, used, order);
            }
            order.Add(v);
        }


        public List<int> CalcDommTree(){
            var tree = new List<int>(Enumerable.Range(0, _bblocks.Count));

            var domms = FindDommBlocks();

            for (int i = 0; i < domms.Count; ++i)
                domms[i].Remove(i);

            var Q = new Queue<int>();
            Q.Enqueue(0);

            while (Q.Count > 0){
                int v = Q.Dequeue();
                for (int to = 0; to < domms.Count; ++to){
                    if (domms[to].Count == 1 && domms[to].Contains(v)){
                        tree[to] = v;
                        Q.Enqueue(to);
                    }
                    domms[to].Remove(v);
                }
            }

            return tree;
        }

        public List<HashSet<int>> FindDommBlocks()
        {
            var In = new List<HashSet<int>>();
            var Out = new List<HashSet<int>>();

            var startToId = GetStartToId(_bblocks);

            var allIds = Enumerable.Range(0, _bblocks.Count);

            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                In.Add(null);
                Out.Add(new HashSet<int>(allIds));
            }


            bool change = true;
            while (change)
            {
                change = false;

                for (int i = 0; i < _bblocks.Count(); ++i)
                {
                    var st = _bblocks[i].StartLabel;

                    In[i] = null;

                    foreach (var p in Prev[st])
                    {
                        if (In[i] == null)
                        {
                            In[i] = new HashSet<int>(Out[startToId[p]]);
                        }
                        else
                        {
                            In[i].IntersectWith(Out[startToId[p]]);
                        }
                    }

                    int sz = Out[i].Count;

                    if (In[i] == null){
                        Out[i] = new HashSet<int>{i};
                    }else{
                        Out[i] = In[i];
                        Out[i].Add(i);
                    }

                    change |= sz != Out[i].Count;
                }

            }

            return Out;

        }





        public (List<ExprSet>, List<ExprSet>) GenerateInputOutputAvaliableExpr()
        {
            var In = new List<ExprSet>();
            var Out = new List<ExprSet>();

            var startToId = GetStartToId(_bblocks);


            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                
                In.Add(null);
                Out.Add(new ExprSet());

                if (i > 0)
                {
                    Out[i] = new ExprSet(_genExprByStart.SelectMany(kv => kv.Value.ToList()));
                }

            }

            bool change = true;
            while (change)
            {
                change = false;

                for (int i = 0; i < _bblocks.Count(); ++i)
                {
                    var st = _bblocks[i].StartLabel;

                    In[i] = null;

                    foreach (var p in Prev[st])
                    {
                        if (In[i] == null){
                            In[i] = new ExprSet(Out[startToId[p]]);
                        } else{
                            In[i].IntersectWith(Out[startToId[p]]);
                        }
                    }

                    int sz =Out[i].Count;

                    Out[i] = AvaliableExprs.TransferByGenAndKiller(In[i], _genExprByStart[st], _defByStart[st]);

                    change |= sz != Out[i].Count;
                }

            }

            return (In, Out);

        }



        public (List<LabelSet>, List<LabelSet>) GenerateInputOutputReachingDefs()
        {
            var In = new List<LabelSet>();
            var Out = new List<LabelSet>();

            var startToId = GetStartToId(_bblocks);

            for (int i = 0; i < _bblocks.Count(); ++i)
            {
                In.Add(new LabelSet());
                Out.Add(new LabelSet());
            }

            bool change = true;
            while (change)
            {
                change = false;

                for (int i = 0; i < _bblocks.Count(); ++i)
                {
                    var st = _bblocks[i].StartLabel;
                    In[i] = new LabelSet(Prev[st].SelectMany(p => Out[startToId[p]]));
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

            //Propagate empty blocks
            foreach (var k in visited)
            {
                var bblock = _baseBlockByStart[k];
                if (ThreeAddrOpType.IsGoto(bblock.LastLine.OpType)){
                    int to = int.Parse(bblock.LastLine.RightOp);
                    var tobl = _baseBlockByStart[to];
                    if (tobl.Code[0].OpType == ThreeAddrOpType.Goto){
                        bblock.LastLine.RightOp = tobl.Code[0].RightOp;
                    }else if (tobl.Code.Count == 1 && tobl.Code[0].OpType == ThreeAddrOpType.Nop){
                        if (Next[to].Count == 1)
                            bblock.LastLine.RightOp = Next[to][0].ToString();
                    }
                }
            }

              
            return visited.OrderBy(x=>x)
                          .Select(x=>_baseBlockByStart[x]).ToList();

        }

    }

}
