using System.Collections.Generic;
using ProgramTree;
using ThreeAddr;

namespace SimpleLang.Visitors
{
    public class ThreeAddressGenerationVisitor : AutoVisitor
    {
        public List<ThreeAddrLine> Data { get; }

        public override void VisitBinNumOpNode(BinaryNumericOpNode binop)
        {
            
        }

        public override void VisitAssignNode(AssignNode a)
        {
        }

        public override void VisitIfNode(IfNode bl)
        {
            // TODO add three-address code generation
        }
    }
}