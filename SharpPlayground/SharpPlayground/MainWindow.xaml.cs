﻿using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
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

namespace SharpPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICSharpCode.CodeCompletion.CSharpCompletion completion;
        private readonly string _tempFile = "Program.cs";
        //private readonly DispatcherTimer _autoSaveTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(5)};

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;
            //_autoSaveTimer.Tick += (s, e) => SaveToDisk();
            //_autoSaveTimer.Start();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            textEditor.Focus();

            textEditor.Document.Changed += Document_Changed;
        }

        private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            var i = 5;
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
