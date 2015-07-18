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
    public class CodeRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            SyntaxNode retVal = null;

            if (node.Expression.Kind() == SyntaxKind.AddAssignmentExpression ||
                node.Expression.Kind() == SyntaxKind.SubtractAssignmentExpression ||
                node.Expression.Kind() == SyntaxKind.MultiplyAssignmentExpression ||
                node.Expression.Kind() == SyntaxKind.DivideAssignmentExpression)
            {
                var add = node.Expression as BinaryExpressionSyntax;
                var printValueStmt = add.Left.GetText();

                retVal = base.VisitExpressionStatement(node);
            }

            return base.VisitExpressionStatement(node);
        }
    }
}
