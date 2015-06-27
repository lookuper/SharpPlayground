using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlaygroundCompiler
{
    class UsingCollector : CSharpSyntaxWalker
    {
        public readonly List<LiteralExpressionSyntax> Variables = new List<LiteralExpressionSyntax>();
        
        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            base.VisitLiteralExpression(node);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            var p = new PlaygroundCompilerFacade(PlaygroundCompiler.Properties.Resources.TestCode + "/-i;");
            //var d = p.GetSourceCodeDiagnostics(p.SourceCode);


            var code = PlaygroundCompiler.Properties.Resources.TestCode;
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code + "");
            var diagnosticAfterParsing = syntaxTree.GetDiagnostics()?.ToList();

            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            var uc = new UsingCollector();
            uc.Visit(root);

            var usings = uc.Variables.ToList();

            var firstMember = root.Members[0];
            var declaration = (NamespaceDeclarationSyntax)firstMember;
            var programDeclaration = (ClassDeclarationSyntax)declaration.Members[0];
            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];

            var firstParameter = from methodDeclaration in root.DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 where methodDeclaration.Identifier.ValueText == "Main"
                                 select methodDeclaration.Modifiers.FirstOrDefault();

            //var literalInfo = root.DescendantNodes()
            //    .OfType<LiteralExpressionSyntax>()
            //    .ToList();

            //var compilationSimple = CSharpCompilation.Create("HelloWorld")
            //    .AddReferences(MetadataReference.CreateFromFile((typeof(Object).Assembly.Location)))
            //    .AddSyntaxTrees(syntaxTree);

            //var diagnosticsSimple = compilationSimple.GetDiagnostics();
            //var model = compilationSimple.GetSemanticModel(syntaxTree);
            //var nameInfo = model.GetSymbolInfo(root.Usings.First().Name);
            //var systemSymbol = nameInfo.Symbol as INamespaceSymbol;
            //var namespaceMembers = systemSymbol.GetNamespaceMembers().Select(x => x.Name).ToList();

            //var intValue = literalInfo.First();
            //var literalIntInfo = model.GetTypeInfo(intValue);
            //var intTypeSymbol = literalIntInfo.Type as INamedTypeSymbol;
            //var publicMembers = intTypeSymbol.GetMembers()
            //    .OfType<IMethodSymbol>()
            //    .Where(x => x.ReturnType.Equals(intTypeSymbol) && x.DeclaredAccessibility == Accessibility.Public)
            //    .Select(x => x.Name)
            //    .ToList();

            //var assemblyName = Path.GetRandomFileName();
            //var references = new MetadataReference[]
            //{
            //    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            //};

            //CSharpCompilation compilation = CSharpCompilation.Create(
            //    assemblyName,
            //    syntaxTrees: new[] { syntaxTree },
            //    references: references,
            //    options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            //var dignosticsAfterCompilation = compilation.GetDiagnostics();

            //using (var ms = new MemoryStream())
            //{
            //    EmitResult result = compilation.Emit(ms);

            //    if (!result.Success)
            //    {
            //        foreach (var diag in result.Diagnostics)
            //        {
            //            Console.WriteLine(diag);
            //        }
            //    }
            //    else
            //    {
            //        ms.Seek(0, SeekOrigin.Begin);
            //        Assembly assembly = Assembly.Load(ms.ToArray());
            //    }              
            //}

        }
    }
}
