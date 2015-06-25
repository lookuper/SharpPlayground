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
            var res = compilerFacade.GetSourceCodeDiagnostics(SourceCode);
            if (res.Count() != 0)
            {
                foreach (var item in res)
                {
                    var lineString = item.Substring(1, 2);
                    var lineNumber = Int32.Parse(lineString);

                    var lr = Output.ElementAt(lineNumber) as LineResult;
                    lr.Value += item;
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
