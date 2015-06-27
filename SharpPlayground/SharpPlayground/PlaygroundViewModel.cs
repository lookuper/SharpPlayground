﻿using ICSharpCode.AvalonEdit.Document;
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

        public PlaygroundViewModel()
        {
            Output = new List<LineResult>();
        }

        public void DocumentChangedEvent(DocumentChangeEventArgs e)
        {
            if (EditorLines != Output.Count)
                RegenerateLineResult();

            FillCodeDiagnostics();
        }

        private void FillCodeDiagnostics()
        {
            var diagMessages = compilerFacade.GetSourceCodeDiagnostics(SourceCode);
            if (diagMessages.Count != 0)
            {
                foreach (var message in diagMessages)
                {
                    var line = Output.ElementAt(message.LineNumber);
                    line.Value += message;                    
                }
            }
        }

        public void RegenerateLineResult()
        {
            var generatedEmptyLines = Enumerable.Range(1, EditorLines)
                .Select(x => new LineResult { Line = x, Value = x.ToString(), CanExpand = false })
                .ToList();

            Output = generatedEmptyLines; FillCodeDiagnostics();
        }
    }
}
