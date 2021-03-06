﻿using CommonTypes;
using ICSharpCode.AvalonEdit.Document;
using PlaygroundCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    public class PlaygroundViewModel : BaseViewModel
    {
        private PlaygroundCompilerFacade compilerFacade = new PlaygroundCompilerFacade();

        private string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set { _sourceCode = value; OnPropertyChanged("SourceCode"); }
        }

        private int _lines;
        public int EditorLines
        {
            get { return _lines; }
            set { _lines = value; OnPropertyChanged("Lines"); }
        }

        private TextDocument _document;
        public TextDocument Document
        {
            get { return _document; }
            set { _document = value; OnPropertyChanged("Document"); }
        }

        private IList<LineResult> _output;
        public IList<LineResult> Output
        {
            get { return _output; }
            set { _output = value; OnPropertyChanged("Output"); }
        }

        private IList<SyntaxTreeDiagnosticResult> _diagnostics;
        public IList<SyntaxTreeDiagnosticResult> Diagnostics
        {
            get { return _diagnostics; }
            set { _diagnostics = value; OnPropertyChanged("Diagnostics"); }
        }

        public PlaygroundViewModel()
        {
            Output = new List<LineResult>();
        }

        public void DocumentChangedEvent(String text)
        {
            if (!ShouldCompile(text))
                return;

            if (EditorLines != Output.Count)
                RegenerateLineResult();
            else
            {
                FillCodeDiagnostics();
                FillCodeLiterals();
            }
        }

        private bool ShouldCompile(string text)
        {
            if (text == ";" || text == Environment.NewLine)
                return true;
            else
                return false;
        }

        private void FillCodeLiterals()
        {
            if (compilerFacade.Root == null)
                return;

            foreach (var literal in compilerFacade.GetLiterals())
            {
                var lineNumber = literal.GetLocation()
                    .GetMappedLineSpan()
                    .StartLinePosition
                    .Line;

                var value = literal.Token.Value;
                var element = Output.ElementAtOrDefault(lineNumber);

                if (element != null)
                    element.Value = value.ToString();
            }

            FillCodeDiagnostics(compilerFacade.DiagnosticMessages.OrderByDescending(x => x.LinePosition).ToList());

        }
        private void FillCodeDiagnostics()
        {            
            var diagMessages = compilerFacade.GetSourceCodeDiagnostics(SourceCode);
            if (diagMessages.Count != 0)
            {
                foreach (var diagResult in diagMessages)
                {
                    var line = Output.ElementAtOrDefault(diagResult.LineNumber);

                    if (line == null)
                    {
                        RegenerateLineResultWithoutCodeDiagnostic();
                        return;
                    }

                    line.Value += " " + diagResult.Message;                    
                }
            }
            else
            {
                // remove previour errors
                foreach (var item in Output)
                {
                    item.Value = String.Empty;
                }
            }
        }

        private void FillCodeDiagnostics(IList<SyntaxTreeDiagnosticResult> source)
        {
            foreach (var compilationConstant in source)
            {
                var line = Output.ElementAtOrDefault(compilationConstant.LineNumber);
                line.Value = compilationConstant.Message;
            }
        }

        public void RegenerateLineResultWithoutCodeDiagnostic()
        {
            var generatedEmptyLines = Enumerable.Range(1, EditorLines)
                .Select(x => new LineResult { Line = x, Value = String.Empty, CanExpand = false })
                .ToList();

            Output = generatedEmptyLines;
        }

        public void RegenerateLineResult()
        {
            var generatedEmptyLines = Enumerable.Range(1, EditorLines)
                .Select(x => new LineResult { Line = x, Value = String.Empty, CanExpand = false })
                .ToList();

            Output = generatedEmptyLines;
            FillCodeDiagnostics();
            FillCodeLiterals();
        }
    }
}
