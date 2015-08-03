using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System.Reflection;
using System.IO;

namespace PlaygroundCompiler
{
    public class RoslynCompilerFacade : ICompilerFacade
    {
        private List<SyntaxTreeDiagnosticResult> _diagnosticMessages = new List<SyntaxTreeDiagnosticResult>();
        private List<SyntaxTreeDiagnosticResult> _variables = new List<SyntaxTreeDiagnosticResult>();
        private Stack<String> _previousCompilations = new Stack<String>();
        private Stack<Script> _scriptQueue = new Stack<Script>();

        public static RoslynCompilerFacade Instance = new RoslynCompilerFacade();
        public IEnumerable<SyntaxTreeDiagnosticResult> DiagnosticMessages { get { return new List<SyntaxTreeDiagnosticResult>(_diagnosticMessages); } }
        public Stack<String> PreviousCompilationSourceCode { get { return new Stack<string>(_previousCompilations); } }
        public IEnumerable<SyntaxTreeDiagnosticResult> GetVariables() { return new List<SyntaxTreeDiagnosticResult>(_variables); }

        public ScriptOptions DefaultOptions
        {
            get
            {
                return ScriptOptions.Default
                .AddReferences(Assembly.GetAssembly(typeof(Path)))
                .AddReferences(Assembly.GetAssembly(typeof(Enumerable)))
                .AddNamespaces("System")
                .AddNamespaces("System.IO")
                .AddNamespaces("System.Linq");
            }
        }

        private RoslynCompilerFacade()
        {

        }

        public void Compile(string sourceCode, ScriptOptions options = null)
        {
            if (String.IsNullOrEmpty(sourceCode))
                throw new ArgumentException(nameof(sourceCode));

            _previousCompilations.Push(sourceCode);

            Script prev = _scriptQueue.Count > 0 ? _scriptQueue.Pop() : null;

            var script = CSharpScript.Create(sourceCode, DefaultOptions).WithPrevious(prev);
            var endState = script.RunAsync();
            _scriptQueue.Push(endState.Script);
            
            //_variables = endState.Variables
            throw new NotImplementedException();
        }
    }
}
