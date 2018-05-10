using System;
using ThreeAddr;
using System.Collections.Generic;
            
namespace SimpleLang.Optimizations
{
    using ExprSet = HashSet<(String, String, String)>;


    public class GlobalCommonSubexpressionsOptimization : CrossBlocksOptimization
    {
        private int _tempcount = 0;

        public override bool Optimize(List<BaseBlock> codeBlocks)
        {
            var CFG = new ControlFlowGraph(codeBlocks);
            var (InExprs, OutExpr) = CFG.GenerateInputOutputAvaliableExpr();


            var startToId = new Dictionary<int, int>();
            for (int i = 0; i < codeBlocks.Count; ++i)
            {
                startToId[codeBlocks[i].StartLabel] = i;
            }

            for (int i = 0; i < codeBlocks.Count; ++i)
            {
                if (TryOptimize(codeBlocks[i], InExprs[i], CFG.PrevBlocks(codeBlocks[i])))
                {
                    var code = BaseBlockHelper.JoinBaseBlocks(codeBlocks);
                    BaseBlockHelper.FixLabelsNumeration(code);
                    codeBlocks.Clear();
                    codeBlocks.InsertRange(0, BaseBlockHelper.GenBaseBlocks(code));

                    return true;
                }
            }
            return false;
        } 


        private bool TryOptimize(BaseBlock bblock, ExprSet input, IEnumerable<BaseBlock> prev){
            for (int i = 0; i < bblock.Code.Count; ++i)
            {
                var line = bblock.Code[i];
                if (ThreeAddrOpType.IsDefinition(line.OpType) && line.OpType != ThreeAddrOpType.Assign &&
                    line.OpType != ThreeAddrOpType.Read)
                {
                    var expr = (line.LeftOp, line.OpType, line.RightOp);

                    if (input.Contains(expr))
                    {
                        if (TryExtract(prev, expr))
                        {
                            line.RightOp = "tt" + _tempcount.ToString();
                            _tempcount++;
                            line.OpType = ThreeAddrOpType.Assign;
                            line.LeftOp = null;
                            return true;
                        }
                    }
                }
                input = TransferByLine(input, line);
            }
            return false;
        }

        private bool TryExtract( IEnumerable<BaseBlock> prev, (String, String, String) expr )
        {
            bool ret = false;

            foreach (var bblock in prev)
            {
                var insline = new ThreeAddrLine();
                insline.Accum = "tt" + _tempcount.ToString();
                insline.Label = null;
                (insline.LeftOp, insline.OpType, insline.RightOp) = expr;


                for (int i = bblock.Code.Count - 1; i >= 0; --i){
                    var line = bblock.Code[i];
                    if (line.LeftOp == insline.LeftOp && line.RightOp == insline.RightOp 
                        && line.OpType == insline.OpType){

                        ret = true;

                        if (i == 0){
                            insline.Label = line.Label;
                            line.Label = null;
                        }

                        bblock.Code.Insert(i, insline);
                        break;
                    }
                }

            }
            return ret;
        }

        private ExprSet TransferByLine(ExprSet s, ThreeAddrLine line)
        {
            if (s == null)
                return null;

            var ret = new ExprSet(s);

            if (ThreeAddrOpType.IsDefinition(line.OpType))
            {
                //if (line.OpType != ThreeAddrOpType.Assign && line.OpType != ThreeAddrOpType.Read)
                //{
                //    ret.Add( (line.LeftOp, line.OpType, line.RightOp) );
                //}

                ret.RemoveWhere(  it => (it.Item1 == line.Accum || it.Item3 == line.Accum) );

            }

            return ret;
        }

    }
}
