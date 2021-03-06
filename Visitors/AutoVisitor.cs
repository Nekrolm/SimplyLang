﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class AutoVisitor : Visitor
    {
        public override void VisitBinaryOpNode(BinaryOpNode binop)
        {
            if (binop == null) return;
            if (binop.LeftNode != null)
                binop.LeftNode.Visit(this);
            binop.RightNode.Visit(this);
        }

        public override void VisitAssignNode(AssignNode a)
        {
            if (a == null) return;
            a.Id.Visit(this);
            a.Expr.Visit(this);
        }

        public override void VisitCycleNode(CycleNode c)
        {
            if (c == null) return;
            c.Expr.Visit(this);
            c.Stat.Visit(this);
        }

        public override void VisitBlockNode(BlockNode bl)
        {
            if (bl == null) return;
            foreach (var st in bl.StList)
                if (st != null)
                    st.Visit(this);
        }

        public override void VisitWriteNode(WriteNode w)
        {
            if (w == null) return;
            w.Expr.Visit(this);
        }

        public override void VisitReadNode(ReadNode rd)
        {
            rd.Id.Visit(this);
        }

        public override void VisitForCycleNode(ForCycleNode fc)
        {
            fc.Counter.Visit(this);
            fc.LeftBound.Visit(this);
            fc.RightBound.Visit(this);
            fc.Step.Visit(this);
            fc.Stat.Visit(this);
        }

        public override void VisitIfNode(IfNode bl)
        {
            if (bl == null) return;
            bl.Cond.Visit(this);
            bl.ThenB.Visit(this);
            if (bl.ElseB != null)
                bl.ElseB.Visit(this);
        }
    }
}