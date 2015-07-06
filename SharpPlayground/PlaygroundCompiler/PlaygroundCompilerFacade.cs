using CommonTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaygroundCompiler
{
    public class PlaygroundCompilerFacade
    {
        public string SourceCode { get; private set; }
        public SyntaxTree Tree { get; private set; }
        public CompilationUnitSyntax Root { get; private set; }
        public IList<LiteralExpressionSyntax> Literals { get; private set; }

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

            return diagMessages;
        }

        public void Compile()
        {
            var compilation = CSharpCompilation.Create("TestCompile")
                .AddReferences(MetadataReference.CreateFromFile((typeof(Object).Assembly.Location)))
                .AddSyntaxTrees(Tree);

            var semanticModel = compilation.GetSemanticModel(Tree); 
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
