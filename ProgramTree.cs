using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };
    public enum BinaryOpType { Minus, Plus, Multiplies, Divides, Less, 
        Greater, Equals, UnEquals, LessOrEquals, GreaterOrEquals, And, Or, Not };

    


    


   
    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
    }

    
    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }


    public class BinaryOpNode : ExprNode
    {
        public ExprNode LeftNode { get; set; }
        public ExprNode RightNode { get; set; }

        public BinaryOpType OpType { get; set; }


        public override void Visit(Visitor v)
        {
            v.VisitBinaryOpNode(this);
        }

        public BinaryOpNode(ExprNode left, BinaryOpType opType, ExprNode right)
        {
            LeftNode = left;
            RightNode = right;
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

    public class ForCycleNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitForCycleNode(this);
        }

        public IdNode Counter { get; set; }
        public ExprNode LeftBound { get; set; }
        public ExprNode RightBound { get; set; }
        public ExprNode Step { get; set; }
        public StatementNode Stat { get; set; }

        public ForCycleNode(IdNode counter, ExprNode left, ExprNode right, ExprNode step, StatementNode stat)
        {
            Counter = counter;
            LeftBound = left;
            RightBound = right;
            Step = step;
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