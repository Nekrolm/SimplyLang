using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang.Utility
{
    class NotInitVariableException : Exception
    {
        public List<string> VarList { get; set; }

        public NotInitVariableException(List<string> param, string message)
                : base(message)
        {
            this.VarList = param;
        }

    }
}
