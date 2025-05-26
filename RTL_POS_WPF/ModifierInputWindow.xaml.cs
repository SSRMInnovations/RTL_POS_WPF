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

namespace RTL_POS_WPF
{
    /// <summary>
    /// Interaction logic for ModifierInputWindow.xaml
    /// </summary>
    public partial class ModifierInputWindow : Window
    {
        public string Modifiers { get; private set; }

        public ModifierInputWindow(string productName)
        {
            InitializeComponent();
            PromptTextBlock.Text = $"Enter modifiers for: {productName}";
            ModifiersTextBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Modifiers = ModifiersTextBox.Text.Trim();
            DialogResult = true;
            Close();
        }
    }
}
