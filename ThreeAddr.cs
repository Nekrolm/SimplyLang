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

    public static class BaseBlockGenerator
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