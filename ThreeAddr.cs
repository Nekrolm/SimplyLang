using System;
using System.Collections.Generic;
using System.Text;


namespace ThreeAddr
{
    public class ThreeAddrLine
    {
        public string Label { get; set; }
        public string Accum { get; set; }
        public string LeftOp { get; set; }
        public string RightOp { get; set; }
        public string OpType { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Label) &&
                   string.IsNullOrEmpty(Accum) &&
                   string.IsNullOrEmpty(LeftOp) &&
                   string.IsNullOrEmpty(RightOp) &&
                   string.IsNullOrEmpty(OpType);
        }

        public override String ToString() 
        {
            return $"{Label}: {Accum} = {LeftOp} {OpType} {RightOp}";
        }

    }

    public class BaseBlock
    {
        public List<ThreeAddrLine> Code { get; set; }

        public BaseBlock() { Code = new List<ThreeAddrLine>(); }

        public int StartLabel { get { return int.Parse(Code[0].Label); } }
        public int EndLabel { get { return int.Parse(Code[Code.Count-1].Label); } }
    
        public ThreeAddrLine LastLine { get { return Code[Code.Count - 1]; }}

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("BLOCK START");
            foreach (var line in Code)
            {
                if (line.IsEmpty())
                {
                    continue;
                }
                builder.AppendLine(line.ToString());
            }
            builder.AppendLine("BLOCK END");
            return builder.ToString();
        }
    }

    public class ControlFlowGraph
    {
        public Dictionary<int, List<int>> Graph { get; set; }
        public int StartBlockId { get; set; }

        private Dictionary<int, BaseBlock> _baseBlockByStart;

        public ControlFlowGraph(List<BaseBlock> baseBlocks){
            Graph = new Dictionary<int, List<int>>();
            StartBlockId = baseBlocks[0].StartLabel;
            _baseBlockByStart = new Dictionary<int, BaseBlock>();
            foreach (var bblock in baseBlocks)
                _baseBlockByStart[bblock.StartLabel] = bblock;

            foreach (var bblock in baseBlocks)
            {
                Graph[bblock.StartLabel] = new List<int>();
                var next = bblock.EndLabel + 1;
                if (_baseBlockByStart.ContainsKey(next))
                    Graph[bblock.StartLabel].Add(next);
                if (bblock.LastLine.OpType.EndsWith("goto"))
                    Graph[bblock.StartLabel].Add(int.Parse(bblock.LastLine.RightOp));

            }
        

        }



    }

    public static class BaseBlockHelper
    {

        public static List<BaseBlock> GenBaseBlocks(List<ThreeAddrLine> code)
        {

            var _isNewBlock = new bool[code.Count + 1];
            _isNewBlock.Initialize();
            _isNewBlock[0] = true;
            for (int i = 0; i < code.Count; ++i)
                if (code[i].OpType.EndsWith("goto"))
                {
                    _isNewBlock[i + 1] = true;
                    int dst = int.Parse(code[i].RightOp);
                    _isNewBlock[dst] = true;
                }

            var baseBlocks = new List<BaseBlock>();

            for (int i = 0; i < code.Count; ++i) {
                if (_isNewBlock[i])
                    baseBlocks.Add(new BaseBlock());
                baseBlocks[baseBlocks.Count - 1].Code.Add(code[i]);



            }
                    
            return baseBlocks;
        }

      }
          


}