using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlaygroundCompiler
{
    public class CodeWalker : CSharpSyntaxWalker
    {
        public SemanticModel CurrentSemanticModel { get; set; }

        public CodeWalker(SemanticModel model)
        {
            CurrentSemanticModel = model;
        }



        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            //var vars = CurrentSemanticModel.AnalyzeDataFlow(node.Parent.Parent.Parent.Parent.Parent).AlwaysAssigned;
            base.VisitBinaryExpression(node);
        }

        public override void Visit(SyntaxNode node)
        {
            if (node is ExpressionSyntax)
            {
                var constant = CurrentSemanticModel.GetConstantValue(node);
                if (constant.HasValue)
                    Debug.WriteLine(constant.Value);
            }

            //if (node is AssignmentExpressionSyntax)
            //{
            //    var assigment = node as AssignmentExpressionSyntax;
            //    var assigmentInfo = CurrentSemanticModel.GetSymbolInfo(node).Symbol;
            //    var declaredSymbolInfo = CurrentSemanticModel.GetDeclaredSymbol(node);

            //    int i = 5;
            //}
            base.Visit(node);
        }
    }
}
