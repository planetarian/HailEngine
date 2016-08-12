using System;
using System.Collections.Generic;
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
using Graupel.Lexer;

namespace GraupelTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lexer _fileLexer;
        private string _fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                _fileName = fileDialog.FileName;
                FilenameTextBox.Text = _fileName;
                FileInfo fileInfo;
                try
                {
                    fileInfo = new FileInfo(_fileName);
                }
                catch (Exception ex)
                {
                    FilenameTextBox.Text = ex.Message;
                    return;
                }
                if (fileInfo.Length > 1024*16)
                {
                    FilenameTextBox.Text = "File too large!";
                    return;
                }

                string contents;
                try
                {
                    contents = File.ReadAllText(_fileName);
                }
                catch (Exception ex)
                {
                    FilenameTextBox.Text = ex.Message;
                    return;
                }

                var reader = new StringSourceReader(_fileName, contents);
                _fileLexer = new Lexer(reader);

                var morpher = new Morpher(_fileLexer);
                Token token;
                do
                {
                    token = morpher.ReadToken();
                    GraupelTextBox.Text += token + Environment.NewLine;
                } while (token.Type != TokenType.EOF);
            }
        }
    }
}
