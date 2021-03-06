﻿using CommonTypes;
using ICSharpCode.AvalonEdit.Document;
using PlaygroundCompiler;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlayground
{
    public class ReactivePlaygroundViewModel : ReactiveObject
    {
        private PlaygroundCompilerFacade compilerFacade = new PlaygroundCompilerFacade();

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

        public ReactiveCommand<List<SyntaxTreeDiagnosticResult>> DocumentChanged { get; protected set; }

        private ObservableAsPropertyHelper<ReactiveList<LineResult>> _output;
        public ReactiveList<LineResult> Output { get { return _output.Value; }  }

        public ReactivePlaygroundViewModel()
        {
            //var canSearch = this.WhenAny(x => x.SourceCode, x => !String.IsNullOrWhiteSpace(x.Value));
            DocumentChanged = ReactiveCommand.CreateAsyncTask(async value => { return await FillOutput(compilerFacade.Compile((String)value)); });

            this.ObservableForProperty(x => x.SourceCode)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(x => x.Value)
                .Where(value => !String.IsNullOrEmpty(value))
                .InvokeCommand(DocumentChanged);

            _output = this.ObservableForProperty(x => x.EditorLines)
                .SkipWhile(x => x.Value == 0)
                .Select(x => EmptyLineResult(x.Value))
                .ToProperty(this, x => x.Output, new ReactiveList<LineResult>());

        }

        private ReactiveList<LineResult> EmptyLineResult(int lines)
        {
            var generatedEmptyLines = Enumerable.Range(1, lines)
                .Select(x => new LineResult { Line = x, Value = String.Empty, CanExpand = false })
                .ToList();

            return new ReactiveList<LineResult>(generatedEmptyLines);
        }

        private Task<List<SyntaxTreeDiagnosticResult>> FillOutput(IEnumerable<SyntaxTreeDiagnosticResult> source)
        {
            return Task.Run(() =>
            {
                foreach (var item in source)
                {
                    var line = Output.ElementAtOrDefault(item.LineNumber);

                    if (line == null)
                        continue;

                    line.Value = item.Message;
                }

                return new List<SyntaxTreeDiagnosticResult>();
            });
        }
    }
}
