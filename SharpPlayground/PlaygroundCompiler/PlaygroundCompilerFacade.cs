﻿using CommonTypes;
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
            var dig = syntaxTree.GetDiagnostics();
            var diagMessages = dig?
                .Select(d => d.ToString())
                .ToList();

            var diagMessages2 = dig?
    .Select(d => new SyntaxTreeDiagnosticResult(d.ToString()))
    .ToList();
            //diagMessages.First().Substring(1, diagMessages.First().IndexOf(',') - 1)
            if (diagMessages == null || diagMessages.Count == 0)
                return new List<String>();

            return diagMessages;
        }

        public IList<SyntaxTreeDiagnosticResult> GetSourceCodeDiagnostics(string sourceCode, bool t)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var dig = syntaxTree.GetDiagnostics();
            var diagMessages = dig?
                .Select(d => new SyntaxTreeDiagnosticResult(d.ToString()))
                .ToList();
            //diagMessages.First().Substring(1, diagMessages.First().IndexOf(',') - 1)
            if (diagMessages == null || diagMessages.Count == 0)
                return new List<SyntaxTreeDiagnosticResult>();

            return diagMessages;
        }
    }
}