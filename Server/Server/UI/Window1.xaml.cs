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

    enum CommandsEnum { CmdInjection = 0, ExecProcess = 1, CreateFile = 2, ReadFile = 3, DeleteFile = 4, MouseInvert = 5, DisplayRotate = 6, OsMessage = 7 };
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public event EventHandler SentPressed;

        public Window1()
        {
            InitializeComponent();
            SetCommand();
        }

        public void SetLabel(string content)
        {
            label.Content = content;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OnSentPressed(EventArgs.Empty);
        }

        protected virtual void OnSentPressed(EventArgs e)
        {
            SentPressed?.Invoke(this, e);
        }

        public int GetClientId()
        {
            return clientsComboBox.SelectedIndex;
        }

        public int GetCommandId()
        {
            return commandComboBox.SelectedIndex;
        }


        public string GetParameters()
        {
            return parametersTextBox.Text;
        }

        private void SetCommand()
        {
            foreach (CommandsEnum command in Enum.GetValues(typeof(CommandsEnum)))
                commandComboBox.Items.Add(command.ToString());
        }

        public void SetClients(IEnumerable<string> clientNames)
        {
            foreach (var clientName in clientNames)
                clientsComboBox.Items.Add(clientName);
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
