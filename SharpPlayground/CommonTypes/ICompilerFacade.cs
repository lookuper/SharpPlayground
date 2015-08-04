using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface ICompilerFacade
    {
        Stack<String> PreviousCompilationSourceCode { get; }

        IEnumerable<SyntaxTreeDiagnosticResult> Compile(string sourceCode, ScriptOptions options = null);

        IEnumerable<SyntaxTreeDiagnosticResult> DiagnosticMessages { get; }
        IEnumerable<SyntaxTreeDiagnosticResult> GetVariables();        
    }
}
