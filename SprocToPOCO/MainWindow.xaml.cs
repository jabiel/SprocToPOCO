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
using System.Windows.Controls.Primitives;

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
            // Load settings
            textBoxConnectionString.Text = Settings.Default.ConnectionString;
            textBoxStoreProc.Text = Settings.Default.Query;   
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Store settings
            Properties.Settings.Default["ConnectionString"] = textBoxConnectionString.Text;
            Properties.Settings.Default["Query"] = textBoxStoreProc.Text;
            Properties.Settings.Default.Save();
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

        private void HandleException(TextBoxBase control, Exception ex)
        {
            control.AppendText(ex.Message + Environment.NewLine + Environment.NewLine);
            control.AppendText(" throwed by: " + ex.GetType().ToString() + Environment.NewLine);

            control.AppendText(Environment.NewLine);
            control.AppendText(Environment.NewLine);
            control.AppendText("StackTrace: " + Environment.NewLine);
            control.AppendText(ex.StackTrace);
        }

        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            SqlMetaProvider rsp = new SqlMetaProvider(textBoxConnectionString.Text);
            richTextBoxPOCO.Document.Blocks.Clear();
            richTextBoxStoreProc.Document.Blocks.Clear();



            try
            {
                var cols = rsp.GetResultsetColumnsFromStoredProc(textBoxStoreProc.Text);

                string s = CSharWriter.ToPOCO(FirstWord(textBoxStoreProc.Text), cols);

                richTextBoxPOCO.AppendText(s);
            }
            catch (Exception ex)
            {
                HandleException(richTextBoxPOCO, ex);
            }
 

            try
            {
                var pars = rsp.GetParamsFromStoredProc(FirstWord(textBoxStoreProc.Text));

                string s = CSharWriter.ToDataProvider(FirstWord(textBoxStoreProc.Text), pars);

                richTextBoxStoreProc.AppendText(s);
            }
            catch (Exception ex)
            {
                HandleException(richTextBoxStoreProc, ex);
            }

        }


    }
}
