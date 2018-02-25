using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class PrettyPrintVisitor : Visitor
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

        public static String ConvertOpType(BinaryOpType t)
        {
            switch (t)
            {
                case BinaryOpType.Divides: return "/";

                case BinaryOpType.Multiplies: return "*";

                case BinaryOpType.Minus: return "-";

                case BinaryOpType.Plus: return "+";

                case BinaryOpType.And: return "and";

                case BinaryOpType.Or: return "or";

                case BinaryOpType.Not: return "not";

                case BinaryOpType.Equals: return "==";
                case BinaryOpType.Greater: return ">";
                case BinaryOpType.GreaterOrEquals: return ">=";
                case BinaryOpType.Less: return "<";
                case BinaryOpType.LessOrEquals: return "<=";
                case BinaryOpType.UnEquals: return "!=";

                default:
                    return "";
            }
        }

        public override void VisitBinaryOpNode(BinaryOpNode binop)
        {
            Text += "(";

            if (binop.LeftNode != null)
                binop.LeftNode.Visit(this);

            Text += " " + ConvertOpType(binop.OpType) + " ";
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

            var count = bl.StList.Count;

            if (count > 0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < count; i++)
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