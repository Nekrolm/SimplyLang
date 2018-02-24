using System.Collections.Generic;
using System.Diagnostics;
using ProgramTree;
using ThreeAddr;

namespace SimpleLang.Visitors
{
    public class ThreeAddressGenerationVisitor : AutoVisitor
    {
        public List<ThreeAddrLine> Data { get; }

        public override void VisitIntNumNode(IntNumNode num)
        {
            
        }

        public override void VisitIdNode(IdNode id)
        {
            
        }

        public override void VisitIfNode(IfNode bl)
        {
            // TODO add three-address code generation
        }
    }
}