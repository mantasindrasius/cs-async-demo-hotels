using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hotels
{
    /// <summary>
    /// Interaction logic for Toggler.xaml
    /// </summary>
    public partial class Toggler : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private ToggleManager m_manager;

        private Toggler()
        {
            InitializeComponent();
            Loaded += Toggler_Loaded;
        }

        public Toggler(ToggleManager manager): this()
        {
            m_manager = manager;
        }

        void Toggler_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Latency_KeyUp(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;

            switch (e.Key)
            {
                case Key.Up:
                    textbox.ChangeBy(100);
                    break;
                case Key.Down:
                    textbox.ChangeBy(-100);
                    break;
            }
        }
    }

    static class RichTextBox
    {
        public static void ChangeBy(this TextBox textBox, int amount)
        {
            var value = textBox.Text;
            long longValue;

            if (long.TryParse(value, out longValue))
            {
                textBox.Text = (longValue + amount).ToString();
            }
        }
    }
}
