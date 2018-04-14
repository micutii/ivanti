using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Server.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public event EventHandler SentPressed;

        public Window1()
        {
            InitializeComponent();
        }

        public void SetLabel(string content)
        {
            label.Content = content;
        }

        public string GetMessage()
        {
            return textBox.Text;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OnSentPressed(EventArgs.Empty);
        }

        protected virtual void OnSentPressed(EventArgs e)
        {
            //EventHandler handler = SentPressed;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}

            SentPressed?.Invoke(this, e);
        }
    }
}
