using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SprocToPOCO.Logic;
using SprocToPOCO.Properties;

namespace SprocToPOCO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default.ConnectionString;
            textBox2.Text = Settings.Default.Query;   
        }


        private string FirstWord(string str)
        {
            if (str.IndexOf(" ") < 0)
            {
                return str;
            }
            else
            {
                return str.Substring(0, str.IndexOf(" "));
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            RunStoredProc rsp = new RunStoredProc(textBox1.Text);
            richTextBox1.Document.Blocks.Clear();
            richTextBox2.Document.Blocks.Clear();

            try
            {
                var cols = rsp.GetColumnsFromResultSet(textBox2.Text);

                string s = CSharWriter.ToPOCO(FirstWord(textBox2.Text), cols);

                richTextBox1.AppendText(s);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message + Environment.NewLine + Environment.NewLine);
                richTextBox1.AppendText(" throwed by: " + ex.GetType().ToString() + Environment.NewLine);

                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.AppendText("StackTrace: " + Environment.NewLine);
                richTextBox1.AppendText(ex.StackTrace);
            }


            try
            {

                var pars = rsp.GetParamsFromStoredProc(FirstWord(textBox2.Text));

                string s = CSharWriter.ToDataProvider(FirstWord(textBox2.Text), pars);

                richTextBox2.AppendText(s);
            }
            catch (Exception ex)
            {
                richTextBox2.AppendText(ex.Message + Environment.NewLine + Environment.NewLine);
                richTextBox2.AppendText(" throwed by: " + ex.GetType().ToString() + Environment.NewLine);

                richTextBox2.AppendText(Environment.NewLine);
                richTextBox2.AppendText(Environment.NewLine);
                richTextBox2.AppendText("StackTrace: " + Environment.NewLine);
                richTextBox2.AppendText(ex.StackTrace);
            }

            try
            {
                var pars = rsp.GetParamsFromStoredProc(FirstWord(textBox2.Text));

                string s = SQLWriter.ToInsertSproc(FirstWord(textBox2.Text), pars);

                richTextBox3.AppendText(s);
            }
            catch (Exception ex)
            {
                richTextBox3.AppendText(ex.Message + Environment.NewLine + Environment.NewLine);
                richTextBox3.AppendText(" throwed by: " + ex.GetType().ToString() + Environment.NewLine);

                richTextBox3.AppendText(Environment.NewLine);
                richTextBox3.AppendText(Environment.NewLine);
                richTextBox3.AppendText("StackTrace: " + Environment.NewLine);
                richTextBox3.AppendText(ex.StackTrace);
            }
            


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default["ConnectionString"] = textBox1.Text;
            Properties.Settings.Default["Query"] = textBox2.Text;
            Properties.Settings.Default.Save();
        }
    }
}
