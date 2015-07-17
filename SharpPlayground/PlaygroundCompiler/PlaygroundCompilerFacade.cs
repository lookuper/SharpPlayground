using CommonTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void Compile()
        {
            var compilation = CSharpCompilation.Create("TestCompile", new[] { Tree },
                new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Thread).Assembly.Location)
                }, new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            DiagnosticMessages = compilation.GetDiagnostics()
                .Select(d => new SyntaxTreeDiagnosticResult(d.ToString()))
                .ToList();

            var semanticModel = compilation.GetSemanticModel(Tree);
            var root = Tree.GetRoot();

            var variableDeclarations = root.DescendantNodes()
                .OfType<LocalDeclarationStatementSyntax>()
                .ToList();

            var expressions = root.DescendantNodes()
                .OfType<ExpressionSyntax>()
                .ToList();

            var variables = root.DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .ToList();

            var declarator = variables[1];
            //var initializerExpression = declarator.Initializer.Value as ExpressionSyntax;
            //var test = initializerExpression.Expressions;

            foreach (var varDeclaration in variableDeclarations)
            {
                //var test = semanticModel.GetDeclaredSymbol(varDeclaration);
                //var symbolInfo = semanticModel.GetSymbolInfo(varDeclaration.Declaration.Type);
                //var typeSymbol = symbolInfo.Symbol;

                //var test = semanticModel.GetSymbolInfo(varDeclaration);
                //var test2 = semanticModel.GetDeclaredSymbol(varDeclaration.Declaration.Variables[0]);

                //semanticModel.GetSymbolInfo()
            }

            //var uc = new UsingCollector();
            //uc.Visit(Tree.GetRoot());
            //var s = semanticModel.LookupSymbols(20);
            //semanticModel.GetSymbolInfo()
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
