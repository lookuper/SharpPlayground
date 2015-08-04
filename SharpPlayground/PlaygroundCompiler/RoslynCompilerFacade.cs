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

        public IEnumerable<SyntaxTreeDiagnosticResult> Compile(string sourceCode, ScriptOptions options = null)
        {
            if (String.IsNullOrEmpty(sourceCode))
                throw new ArgumentException(nameof(sourceCode));

            _previousCompilations.Push(sourceCode);

            Script prev = _scriptQueue.Any() ? _scriptQueue.Pop() : null;

            var script = CSharpScript.Create(sourceCode, options ?? DefaultOptions).WithPrevious(prev);
            ScriptState endState = null;

            try { endState = script.RunAsync(); }
            catch (CompilationErrorException ex)
            {
                var compilationError = new SyntaxTreeDiagnosticResult(ex.Message);
                _diagnosticMessages.Add(compilationError);
            }

            _scriptQueue.Push(endState.Script);

            if (endState.Variables != null)
            {
                var res = endState.Variables
                    .Select(v => new SyntaxTreeDiagnosticResult(-1, -1, v.Name + v.Value) { Name = v.Name, Value = v.Value.ToString() })
                    .ToList();

                _variables.AddRange(res);
            }

            return _diagnosticMessages.Any() ? _diagnosticMessages : _variables;
        }
    }
}
