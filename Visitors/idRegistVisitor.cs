using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class idRegistVisitor : AutoVisitor
    {
        public List<string> idList =new List<string>();

        public override void VisitIdNode(IdNode id)
        {
            if (idList.Contains(id.Name)) return; // Если в списке уже есть такое имя: выходим?
            else idList.Add(id.Name); // Иначе добавляем имя переменной в список?
        }
    }
}
