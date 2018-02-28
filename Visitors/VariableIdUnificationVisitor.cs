using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class VariableIdUnificationVisitor : AutoVisitor
    {
        public Dictionary<string, int> IDDict = new Dictionary<string, int>();

        private int varNum = 0;
        public string VarPrefix { get { return "v"; }}


        public override void VisitIdNode(IdNode id)
        {
            if (!IDDict.ContainsKey(id.Name)){
                IDDict[id.Name] = varNum++;
            }
            id.Name = VarPrefix + IDDict[id.Name];
        }
    }
}
