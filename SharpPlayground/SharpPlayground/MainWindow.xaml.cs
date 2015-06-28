using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using ICSharpCode.NRefactory.Editor;
using PlaygroundCompiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;

namespace SharpPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICSharpCode.CodeCompletion.CSharpCompletion completion;
        private readonly string _tempFile = "Program.cs";
        private PlaygroundCompilerFacade compilerFacade = new PlaygroundCompilerFacade();
        private PlaygroundViewModel ViewModel;
        private static int _sendIteration;

        //private readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(5)};

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = this.DataContext as PlaygroundViewModel;
            this.Closing += MainWindow_Closing;
            //_autoSaveTimer.Tick += (s, e) => SaveToDisk();
            //_autoSaveTimer.Start();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            textEditor.Focus();        
            ViewModel.RegenerateLineResult();
            textEditor.Document.Changed += Document_Changed;
        }

        private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            // awfull
            if (_sendIteration != 2)
            {
                _sendIteration++;
                return;
            }       

            ViewModel.DocumentChangedEvent(e);
            _sendIteration = 0;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveToDisk();
        }

        public void SaveToDisk()
        {
            textEditor.SaveFile();
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new ScriptProvider());
            OpenFile(@"..\SampleFiles\Sample1.cs");
        }

        private void OpenFile(string fileName)
        {
            textEditor.Completion = completion;

            if (File.Exists(_tempFile))
            {
                textEditor.OpenFile(_tempFile);

                if (!textEditor.Text.Contains("public static void Main()"))
                {
                    using (var m = new MemoryStream(Encoding.Default.GetBytes(SharpPlayground.Properties.Resources.EmptyProgram)))
                    {
                        textEditor.Clear();
                        textEditor.Load(m);
                    }
                }
            }
        }
    }
}
