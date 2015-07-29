using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    public class ReactivePlaygroundViewModel : ReactiveObject
    {
        private string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set { this.RaiseAndSetIfChanged(ref _sourceCode, value); }
        }

        private int _editorLines;
        public int EditorLines
        {
            get { return _editorLines; }
            set { this.RaiseAndSetIfChanged(ref _editorLines, value); }
        }

        private TextDocument _document;
        public TextDocument Document
        {
            get { return _document; }
            set { this.RaiseAndSetIfChanged(ref _document, value); }
        }

        public ReactiveCommand<Object> DocumentChanged { get; protected set; }
        public ReactiveList<LineResult> Output { get; protected set; }
    }
}
