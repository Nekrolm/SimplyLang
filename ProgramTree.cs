using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
    public enum BinaryNumericOpType { Minus, Plus, Multiplies, Divides };
    public enum BinaryCompareOpType { Less, Greater, Equals, UnEquals, LessOrEquals, GreaterOrEquals };
    public enum BinaryBoolOpType { And, Or, Not };

    


    


   
    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
    }

    
    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }


    public abstract class BinaryOpNode : ExprNode
    {
        public ExprNode LeftNode { get; set; }
        public ExprNode RightNode { get; set; }

        public BinaryOpNode(ExprNode left, ExprNode right)
        {
            LeftNode = left;
            RightNode = right;
        }

    }
    

    public class BinaryNumericOpNode : BinaryOpNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitBinNumOpNode(this);
        }

        public BinaryNumericOpType OpType { get; set; }
        public BinaryNumericOpNode(ExprNode left, BinaryNumericOpType opType, ExprNode right)
            : base(left, right)
        {
            OpType = opType;
        }
    }

    public class BinaryBoolOpNode : BinaryOpNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitBinBoolOpNode(this);
        }

        public BinaryBoolOpType OpType { get; set; }
        public BinaryBoolOpNode(ExprNode left, BinaryBoolOpType opType, ExprNode right)
            : base(left, right)
        {
            OpType = opType;
        }
    }


    public class BinaryCompareOpNode : BinaryOpNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitBinCompOpNode(this);
        }

        public BinaryCompareOpType OpType { get; set; }
        public BinaryCompareOpNode(ExprNode left, BinaryCompareOpType opType, ExprNode right)
            : base(left, right)
        {
            OpType = opType;
        }
    }

    public class IdNode : ExprNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class WriteNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitWriteNode(this);
        }
        public ExprNode Expr { get; set; }
        public WriteNode(ExprNode expr)
        {
            Expr = expr;
        }
    }

    public class AssignNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
        }
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
    }


    public class IfNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitIfNode(this);
        }
        public ExprNode Cond { get; set;  }
        public StatementNode ThenB { get; set; }
        public StatementNode ElseB { get; set; }
        public IfNode(ExprNode cond, StatementNode thenBranch, StatementNode elseBranch = null)
        {
            Cond = cond;
            ThenB = thenBranch;
            ElseB = elseBranch;
        }
    }
    public class CycleNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitCycleNode(this);
        }

        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class BlockNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }

        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

}