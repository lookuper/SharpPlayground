using ICSharpCode.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    class ScriptProvider : ICSharpScriptProvider
    {
        public string GetUsing()
        {
            return "" +
                "using System; " +
                "using System.Collections.Generic; " +
                "using System.Linq; " +
                "using System.Text; ";
        }


        public string GetVars()
        {
            return null;
        }
    }
}
