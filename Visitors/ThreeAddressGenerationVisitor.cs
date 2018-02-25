using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ProgramTree;
using SimpleLang.Utility;
using ThreeAddr;

namespace SimpleLang.Visitors
{
    public class ThreeAddressGenerationVisitor : AutoVisitor
    {
        private const string Tag = "ThreeAddressGenerator";

        public List<ThreeAddrLine> Data { get; }
        private ThreeAddrLine _currentLine;

        public ThreeAddressGenerationVisitor()
        {
            Data = new List<ThreeAddrLine>();
        }

        public override void VisitBlockNode(BlockNode bl)
        {
            Console.WriteLine(Tag + " VisitBlockNode");
            if (bl == null) return;
            foreach (var st in bl.StList)
            {
                _currentLine = new ThreeAddrLine();
                if (st != null)
                {
                    st.Visit(this);
                }

                Data.Add(_currentLine);
            }
        }

        public override void VisitAssignNode(AssignNode a)
        {
            Console.WriteLine(Tag + " VisitAssingNode");
            if (a == null) return;
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }

        public override void VisitBinaryOpNode(BinaryOpNode binop)
        {
            Console.WriteLine(Tag + " VisitBinaryOpNode");
            if (binop == null) return;
            if (binop.LeftNode != null)
            {
                binop.LeftNode.Visit(this, NodeOrder.Left);
            }

            binop.RightNode.Visit(this, NodeOrder.Right);

            _currentLine.OpType = ToStringHelper.ToString(binop.OpType);
        }

        public override void VisitBinaryOpNode(BinaryOpNode binop, NodeOrder order)
        {
            Console.WriteLine(Tag + " VisitBinaryOpNode");
            VisitBinaryOpNode(binop);
        }

        public override void VisitIdNode(IdNode id)
        {
            Console.WriteLine(Tag + " VisitIdNode");
            _currentLine.SrcDst = id.Name;
        }

        public override void VisitIdNode(IdNode id, NodeOrder order)
        {
            Console.WriteLine(Tag + " VisitIdNode");
            if (order == NodeOrder.Left)
            {
                _currentLine.LeftOp = id.Name;
            }
            else
            {
                _currentLine.RightOp = id.Name;
            }
        }

        public override void VisitIntNumNode(IntNumNode num)
        {
            Console.WriteLine(Tag + " VisiIntNumNode");
            _currentLine.LeftOp = num.Num.ToString();
        }

        public override void VisitIntNumNode(IntNumNode num, NodeOrder order)
        {
            Console.WriteLine(Tag + " VisitInNumNode");
            if (order == NodeOrder.Left)
            {
                _currentLine.LeftOp = num.Num.ToString();
            }
            else
            {
                _currentLine.RightOp = num.Num.ToString();
            }
        }

        public override void VisitIfNode(IfNode bl)
        {
            Console.WriteLine(Tag + " VisitIfNode");
        }

        public override void VisitCycleNode(CycleNode c)
        {
            Console.WriteLine(Tag + " VsitCycleNode");
        }

        public override void VisitWriteNode(WriteNode w)
        {
            Console.WriteLine(Tag + " VisitWriteNode");
        }

        public override void VisitForCycleNode(ForCycleNode fc)
        {
            Console.WriteLine(Tag + " VisitForCycleNoe");
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
                    .Append(line.SrcDst)
                    .Append(" ")
                    .Append(line.LeftOp)
                    .Append(" ")
                    .Append(line.OpType)
                    .Append(" ")
                    .Append(line.RightOp)
                    .Append(Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}