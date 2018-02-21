using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class PrettyPrintVisitor: Visitor
    {
        public string Text = "";
        private int Indent = 0;

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 2;
        }
        private void IndentMinus()
        {
            Indent -= 2;
        }
        public override void VisitIdNode(IdNode id) 
        {
            Text += id.Name;
        }
        public override void VisitIntNumNode(IntNumNode num) 
        {
            Text += num.Num.ToString();
        }

        public static String ConvertNumericOpType(BinaryNumericOpType t)
        {
            switch (t)
            {
                case BinaryNumericOpType.Divides: return "/";

                case BinaryNumericOpType.Multiplies: return "*";

                case BinaryNumericOpType.Minus: return "-";

                case BinaryNumericOpType.Plus: return "+";
                default:
                    return "";
            }
        }

        public static String ConvertBoolOpType(BinaryBoolOpType t)
        {
            switch (t)
            {
                case BinaryBoolOpType.And: return "and";

                case BinaryBoolOpType.Or: return "or";

                case BinaryBoolOpType.Not: return "not";

                default:
                    return "";
            }
        }

        public static String ConvertCompOpType(BinaryCompareOpType t)
        {
            switch (t)
            {
                case BinaryCompareOpType.Equals: return "==";
                case BinaryCompareOpType.Greater: return ">";
                case BinaryCompareOpType.GreaterOrEquals: return ">=";
                case BinaryCompareOpType.Less: return "<";
                case BinaryCompareOpType.LessOrEquals: return "<=";
                case BinaryCompareOpType.UnEquals: return "!=";

                default:
                    return "";
            }
        }


        public override void VisitBinNumOpNode(BinaryNumericOpNode binop) 
        {
            Text += "(";

            if (binop.LeftNode != null)
                binop.LeftNode.Visit(this);

            Text += " " + ConvertNumericOpType(binop.OpType) + " ";
            binop.RightNode.Visit(this);
            Text += ")";
        }

        public override void VisitBinCompOpNode(BinaryCompareOpNode binop)
        {
            Text += "(";

            if (binop.LeftNode != null)
                binop.LeftNode.Visit(this);

            Text += " " + ConvertCompOpType(binop.OpType) + " ";
            binop.RightNode.Visit(this);
            Text += ")";
        }

        public override void VisitBinBoolOpNode(BinaryBoolOpNode binop)
        {
            Text += "(";
            if (binop.LeftNode != null)
                binop.LeftNode.Visit(this);
            Text += " " + ConvertBoolOpType(binop.OpType) + " ";
            binop.RightNode.Visit(this);
            Text += ")";
        }

        public override void VisitForCycleNode(ForCycleNode fc)
        {
            Text += IndentStr() + "for ";
            fc.Counter.Visit(this);
            Text += " in (";
            fc.LeftBound.Visit(this);
            Text += ", ";
            fc.RightBound.Visit(this);
            Text += ", ";
            fc.Step.Visit(this);
            Text += ")";
            Text += Environment.NewLine;
            fc.Stat.Visit(this);
        }

        public override void VisitIfNode(IfNode bl)
        {
            Text += IndentStr() + "if ";
            bl.Cond.Visit(this);
            Text += Environment.NewLine;
            bl.ThenB.Visit(this);
            if (bl.ElseB != null)
            {
                Text += Environment.NewLine;
                Text += IndentStr() + "else";
                Text += Environment.NewLine;
                bl.ElseB.Visit(this);
            }

        }

        public override void VisitAssignNode(AssignNode a) 
        {
            Text += IndentStr();
            a.Id.Visit(this);
            Text += " := ";
            a.Expr.Visit(this);
        }
        public override void VisitCycleNode(CycleNode c) 
        {
            Text += IndentStr() + "while ";
            c.Expr.Visit(this);
            Text += Environment.NewLine;
            c.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl) 
        {
            Text += IndentStr() + "begin" + Environment.NewLine;
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count>0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                Text += ';';
                if (!(bl.StList[i] == null))
                {
                    Text += Environment.NewLine;
                    bl.StList[i].Visit(this);
                }
            }
            IndentMinus();
            Text += Environment.NewLine + IndentStr() + "end";
        }
        public override void VisitWriteNode(WriteNode w) 
        {
            Text += IndentStr() + "print (";
            w.Expr.Visit(this);
            Text += ")";
        }
        
    }
}
