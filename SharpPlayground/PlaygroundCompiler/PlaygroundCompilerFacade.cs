using CommonTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PlaygroundCompiler
{
    public class PlaygroundCompilerFacade : BaseViewModel
    {
        public string SourceCode { get; private set; }
        public SyntaxTree Tree { get; private set; }
        public CompilationUnitSyntax Root { get; private set; }
        public IList<LiteralExpressionSyntax> Literals { get; private set; }

        private List<SyntaxTreeDiagnosticResult> _diagnosticMessages;
        public List<SyntaxTreeDiagnosticResult> DiagnosticMessages
        {
            get { return _diagnosticMessages; }
            set { _diagnosticMessages = value; OnPropertyChanged("DiagnosticMessages"); }
        }

        public PlaygroundCompilerFacade()
        {
            Literals = new List<LiteralExpressionSyntax>();
        }

        public PlaygroundCompilerFacade(string input) : this()
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException(nameof(input));

            SourceCode = input;
        }

        public IList<SyntaxTreeDiagnosticResult> GetSourceCodeDiagnostics(string sourceCode)
        {
            Tree = CSharpSyntaxTree.ParseText(sourceCode);
            Root = Tree.GetRoot() as CompilationUnitSyntax;

            var dig = Tree.GetDiagnostics();
            var diagMessages = dig?
                .Select(d => new SyntaxTreeDiagnosticResult(d.ToString()))
                .ToList();

            if (diagMessages == null || diagMessages.Count == 0)
            {
                Compile();

                return new List<SyntaxTreeDiagnosticResult>();
            }

            DiagnosticMessages = diagMessages;
            return diagMessages;
        }

        public Task<List<SyntaxTreeDiagnosticResult>> Compile(string sourceCode)
        {
            var t = Task.Run(() =>
            {
                return new List<SyntaxTreeDiagnosticResult>();
            });

            return t;
        }

        public void Compile()
        {
            var compilation = CSharpCompilation.Create("TestCompile", new[] { Tree },
                new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Thread).Assembly.Location)
                }, new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            var semanticModel = compilation.GetSemanticModel(Tree);

            var binaryExpressions = Root.DescendantNodes()
                .OfType<BinaryExpressionSyntax>()
                .OrderBy(x => x.FullSpan)
                .ToList();

            var resultingList = new List<SyntaxTreeDiagnosticResult>();

            foreach (var variable in binaryExpressions)
            {
                var startLine = Tree.GetLineSpan(variable.Span).StartLinePosition;
                var value = semanticModel.GetConstantValue(variable);
                var syntaxResult = new SyntaxTreeDiagnosticResult(startLine.Line, 0, value.Value?.ToString());

                if (!value.HasValue)
                {
                    var  res = new StringBuilder();
                    foreach (var item in variable.ChildNodesAndTokens())
                    {
                        if (item.IsToken)
                        {
                            res.Append(item.ToFullString());
                            continue;
                        }

                        var expressionValue = semanticModel.GetConstantValue(item.AsNode());

                        if (!expressionValue.HasValue)
                        {
                            res.Append(item.ToFullString());
                        }
                        else
                            res.Append(expressionValue.Value?.ToString());
                    }

                    syntaxResult.LinePosition = -1;
                    syntaxResult.Message = res.ToString();

                    resultingList.Add(syntaxResult);
                }
                else
                    resultingList.Add(syntaxResult);
            }

            DiagnosticMessages = resultingList;
        }

        public IList<LiteralExpressionSyntax> GetLiterals()
        {
            var res = Root.DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .ToList();

            return res;
        }
    }
}
