using Microsoft.CodeAnalysis.CSharp;
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

        public PlaygroundCompilerFacade()
        {

        }

        public PlaygroundCompilerFacade(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException(nameof(input));

            SourceCode = input;
        }

        public IEnumerable<String> GetSourceCodeDiagnostics(string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var diagMessages = syntaxTree.GetDiagnostics()?
                .Select(d => d.ToString())
                .ToList();

            if (diagMessages == null || diagMessages.Count == 0)
                return new List<String>();

            return diagMessages;
        }
    }
}
