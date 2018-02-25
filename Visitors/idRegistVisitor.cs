using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class idRegistVisitor : AutoVisitor
    {
        public Dictionary<int, string> IDDict = new Dictionary<int, string>();

        public override void VisitIdNode(IdNode id)
        {
            if (IDDict.ContainsValue(id.Name)) return; // Если в словареуже есть такое имя, выходим.
            else IDDict.Add(IDDict.Count, id.Name); // Иначе добавляем имя переменной в словарь.
        }
    }
}
