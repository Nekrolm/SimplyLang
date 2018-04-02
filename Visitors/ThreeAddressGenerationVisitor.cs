using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ProgramTree;
using SimpleLang.Utility;
using ThreeAddr;
using System.Linq;

namespace SimpleLang.Visitors
{
    public class ThreeAddressGenerationVisitor : AutoVisitor
    {
        private const string Tag = "Generator";
        private const string TempVariableName = "p";

        public List<ThreeAddrLine> Data { get; }


        private int _temporaryVariablesCount;

        private void InsertNop(){;
            var line = new ThreeAddrLine();
            line.Label = GenNewLabel();
            line.OpType = ThreeAddrOpType.Nop;
            Data.Add(line);
        }

        private String GenNewTemporaryVariable(){
            return TempVariableName + (++_temporaryVariablesCount);
        }

        private String GenNewLabel(){
            return Data.Count.ToString();
        }

        private ThreeAddrLine GetLastLine() {
            var id = Data.Count;
            if (id > 0) 
                return Data[id - 1];
            return null;
        }

        public ThreeAddressGenerationVisitor()
        {
            Data = new List<ThreeAddrLine>();
            _temporaryVariablesCount = 0;
        }

        public override void VisitBinaryOpNode(BinaryOpNode binop)
        {
            if (binop == null) return;

            var line = new ThreeAddrLine();

            line.Accum = GenNewTemporaryVariable();

            if (binop.LeftNode != null)
            {
                binop.LeftNode.Visit(this);
                line.LeftOp = GetLastLine().Accum;
            }

            binop.RightNode.Visit(this);
            line.RightOp = GetLastLine().Accum;
            line.OpType = ToStringHelper.ToString(binop.OpType);
            line.Label = GenNewLabel();

            Data.Add(line);
        }

        public override void VisitBlockNode(BlockNode bl)
        {
            Console.WriteLine(Tag + " VisitBlockNode");
            if (bl == null) return;
            foreach (var st in bl.StList)
            {
                if (st != null)
                {
                    st.Visit(this);
                }
            }
        }




        public override void VisitAssignNode(AssignNode a)
        {
            Console.WriteLine(Tag + " VisitAssingNode");
            if (a == null) return;
            a.Expr.Visit(this);
            var line = new ThreeAddrLine();
            line.Accum = a.Id.Name;
            line.RightOp = GetLastLine().Accum;
            line.Label = GenNewLabel();
            line.OpType = ThreeAddrOpType.Assign;
            Data.Add(line);
        }




        public override void VisitIdNode(IdNode id)
        {
            Console.WriteLine(Tag + " VisitIdNode");
            var line = new ThreeAddrLine();
            line.Accum = GenNewTemporaryVariable();
            line.Label = GenNewLabel();
            line.OpType = ThreeAddrOpType.Assign;
            line.RightOp = id.Name;
            Data.Add(line);
        }


        public override void VisitIntNumNode(IntNumNode num)
        {
            var line = new ThreeAddrLine();
            line.Accum = num.Num.ToString();
            line.Label = GenNewLabel();
            line.OpType = ThreeAddrOpType.Nop;
            Data.Add(line);
        }


        public override void VisitIfNode(IfNode bl)
        {
            Console.WriteLine(Tag + " VisitIfNode");

            bl.Cond.Visit(this);
            var ifThenLine = new ThreeAddrLine();
            ifThenLine.Label = GenNewLabel();
            ifThenLine.LeftOp = GetLastLine().Accum;
            ifThenLine.OpType = ThreeAddrOpType.IfGoto;
            Data.Add(ifThenLine);
            if (bl.ElseB != null) bl.ElseB.Visit(this);
            var outsideIfLine = new ThreeAddrLine();
            outsideIfLine.Label = GenNewLabel();
            outsideIfLine.OpType = ThreeAddrOpType.Goto;
            Data.Add(outsideIfLine);
            ifThenLine.RightOp = GenNewLabel();
            bl.ThenB.Visit(this);
            InsertNop();
            outsideIfLine.RightOp = GetLastLine().Label;
        }

        public override void VisitCycleNode(CycleNode c)
        {
            Console.WriteLine(Tag + " VisitCycleNode");

            var startLoopLabel = GenNewLabel();
            c.Expr.Visit(this);

            var whileLine = new ThreeAddrLine();
            whileLine.Label = GenNewLabel();
            whileLine.LeftOp = GetLastLine().Accum;
            whileLine.OpType = ThreeAddrOpType.IfGoto;
            Data.Add(whileLine);

            var outsideWhileLine = new ThreeAddrLine();
            outsideWhileLine.Label = GenNewLabel();
            outsideWhileLine.OpType = ThreeAddrOpType.Goto;
            Data.Add(outsideWhileLine);
            whileLine.RightOp = GenNewLabel();
            c.Stat.Visit(this);

            var gotoStartLine = new ThreeAddrLine();
            gotoStartLine.Label = GenNewLabel();
            gotoStartLine.OpType = ThreeAddrOpType.Goto;
            gotoStartLine.RightOp = startLoopLabel;
            Data.Add(gotoStartLine);

            InsertNop();
            outsideWhileLine.RightOp = GetLastLine().Label;
        }

        public override void VisitWriteNode(WriteNode w)
        {
            Console.WriteLine(Tag + " VisitWriteNode");
            w.Expr.Visit(this);
            var printLine = new ThreeAddrLine();
            printLine.Label = GenNewLabel();
            printLine.RightOp = GetLastLine().Accum;
            printLine.OpType = ThreeAddrOpType.Write;
            Data.Add(printLine);
        }

        public override void VisitReadNode(ReadNode rd)
        {
            var readLine = new ThreeAddrLine();
            readLine.Label = GenNewLabel();
            readLine.Accum = rd.Id.Name;
            readLine.OpType = ThreeAddrOpType.Read;
            Data.Add(readLine);
        }

        public override void VisitForCycleNode(ForCycleNode fc)
        {
            Console.WriteLine(Tag + " VisitForCycleNoe");

            fc.Step.Visit(this);

            if (GetLastLine().Accum.StartsWith(TempVariableName)){
                GetLastLine().Accum = "lc" + GetLastLine().Label;
            }

            var step = GetLastLine().Accum;
            fc.LeftBound.Visit(this);
            var L = GetLastLine().Accum;
            fc.RightBound.Visit(this);

            if (GetLastLine().Accum.StartsWith(TempVariableName))
            {
                GetLastLine().Accum = "lr" + GetLastLine().Label;
            }
            var R = GetLastLine().Accum;

            var initCounterLine = new ThreeAddrLine();
            initCounterLine.Accum = fc.Counter.Name;
            initCounterLine.OpType = ThreeAddrOpType.Assign;
            initCounterLine.RightOp = L;
            initCounterLine.Label = GenNewLabel();

            Data.Add(initCounterLine);

            var startLoopLabel = GenNewLabel();

            var checkLine = new ThreeAddrLine();
            checkLine.Label = startLoopLabel;
            checkLine.Accum = GenNewTemporaryVariable();
            checkLine.LeftOp = fc.Counter.Name;
            checkLine.OpType = ToStringHelper.ToString(BinaryOpType.Less);
            checkLine.RightOp = R;

            Data.Add(checkLine);

            var whileLine = new ThreeAddrLine();
            whileLine.Label = GenNewLabel();
            whileLine.LeftOp = GetLastLine().Accum;
            whileLine.OpType = ThreeAddrOpType.IfGoto;
            Data.Add(whileLine);

            var outsideWhileLine = new ThreeAddrLine();
            outsideWhileLine.Label = GenNewLabel();
            outsideWhileLine.OpType = ThreeAddrOpType.Goto;
            Data.Add(outsideWhileLine);
            whileLine.RightOp = GenNewLabel();
            fc.Stat.Visit(this);

            var counterIncLine = new ThreeAddrLine();
            counterIncLine.Label = GenNewLabel();
            counterIncLine.Accum = fc.Counter.Name;
            counterIncLine.LeftOp = fc.Counter.Name;
            counterIncLine.OpType = ToStringHelper.ToString(BinaryOpType.Plus);
            counterIncLine.RightOp = step;
            Data.Add(counterIncLine);


            var gotoStartLine = new ThreeAddrLine();
            gotoStartLine.Label = GenNewLabel();
            gotoStartLine.OpType = ThreeAddrOpType.Goto;
            gotoStartLine.RightOp = startLoopLabel;
            Data.Add(gotoStartLine);

            InsertNop();
            outsideWhileLine.RightOp = GetLastLine().Label;

        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var line in Data)
            {
                if (line.IsEmpty())
                {
                    continue;
                }

                builder
                    .Append(line.ToString())
                    .Append(Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}