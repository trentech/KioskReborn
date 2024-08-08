using KioskReborn;
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
using WindowsInput;
using WindowsInput.Native;

namespace TouchKeyboard
{
    public partial class KeyboardRemodel : Window
    {
        private bool caps = false;
        private bool shift = false;
        InputSimulator inputSimulator = new InputSimulator();

        public KeyboardRemodel()
        {
            Settings settings = Settings.Get();

            ResourceDictionary dict = new ResourceDictionary();

            Application.Current.Resources.MergedDictionaries.Clear();

            switch (settings.Color)
            {
                case Settings.Colors.Gray:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/Gray.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Blue:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/Blue.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Classic:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/Classic.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Green:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/Green.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Dark:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/Dark.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.LJU:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/KioskReborn;component/Themes/LJU.xaml", UriKind.Relative) });
                    break;
                default:
                    break;
            }

            InitializeComponent();

            Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            Top = SystemParameters.PrimaryScreenHeight - Height;

            InitializeComponent();
        }

        private void OnDrag(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void OnEsc(object sender, RoutedEventArgs e)
        {
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        }

        private void OnShift(object sender, RoutedEventArgs e)
        {
            this.shift ^= true;

            if (this.shift)
            {
                UpperKeys();

                Style style = Application.Current.FindResource("KeyboardButtonShift") as Style;
                RightShift.Style = style;
                LeftShift.Style = style;
            }
            else
            {
                if (!caps)
                {
                    LowerKeys();

                    Style style = Application.Current.FindResource("KeyboardButton") as Style;
                    RightShift.Style = style;
                    LeftShift.Style = style;
                }
            }
        }

        private void OnCaps(object sender, RoutedEventArgs e)
        {
            this.caps ^= true;

            if (this.caps)
            {
                UpperKeys();

                Style style = Application.Current.FindResource("KeyboardButtonShift") as Style;
                RightShift.Style = style;
                LeftShift.Style = style;
                Caps.Style = style;

                if (shift)
                {
                    this.shift ^= true;
                }
            }
            else
            {
                LowerKeys();

                Style style = Application.Current.FindResource("KeyboardButton") as Style;
                RightShift.Style = style;
                LeftShift.Style = style;
                Caps.Style = style;

                if (shift)
                {
                    this.shift ^= true;
                }
            }
        }

        private void OnTilde(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_3, sender, e);
        }

        private void OnOne(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_1, sender, e);
        }

        private void OnTwo(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_2, sender, e);
        }

        private void OnThree(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_3, sender, e);
        }

        private void OnFour(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_4, sender, e);
        }

        private void OnFive(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_5, sender, e);
        }

        private void OnSix(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_6, sender, e);
        }

        private void OnSeven(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_7, sender, e);
        }

        private void OnEight(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_8, sender, e);
        }

        private void OnNine(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_9, sender, e);
        }

        private void OnZero(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_0, sender, e);
        }

        private void OnMinus(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_MINUS, sender, e);
        }

        private void OnEquals(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_PLUS, sender, e);
        }

        private void OnBackspace(object sender, RoutedEventArgs e)
        {
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
        }


        private void OnTab(object sender, RoutedEventArgs e)
        {
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
        }

        private void OnQ(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_Q, sender, e);
        }

        private void OnW(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_W, sender, e);
        }

        private void OnE(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_E, sender, e);
        }

        private void OnR(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_R, sender, e);
        }

        private void OnT(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_T, sender, e);
        }

        private void OnY(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_Y, sender, e);
        }

        private void OnU(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_U, sender, e);
        }

        private void OnI(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_I, sender, e);
        }

        private void OnO(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_O, sender, e);
        }

        private void OnP(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_P, sender, e);
        }

        private void OnLeftBracket(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_4, sender, e);
        }

        private void OnRightBracket(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_6, sender, e);
        }

        private void OnBackSlash(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_5, sender, e);
        }

        private void OnDel(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.DELETE, sender, e);
        }

        private void OnA(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_A, sender, e);
        }

        private void OnS(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_S, sender, e);
        }

        private void OnD(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_D, sender, e);
        }

        private void OnF(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_F, sender, e);
        }

        private void OnG(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_G, sender, e);
        }

        private void OnH(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_H, sender, e);
        }

        private void OnJ(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_J, sender, e);
        }

        private void OnK(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_K, sender, e);
        }

        private void OnL(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_L, sender, e);
        }

        private void OnSemiColon(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_1, sender, e);
        }

        private void OnQuote(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_7, sender, e);
        }

        private void OnEnter(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.RETURN, sender, e);
        }

        private void OnZ(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_Z, sender, e);
        }

        private void OnX(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_X, sender, e);
        }

        private void OnC(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_C, sender, e);
        }

        private void OnV(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_V, sender, e);
        }

        private void OnB(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_B, sender, e);
        }

        private void OnN(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_N, sender, e);
        }

        private void OnM(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.VK_M, sender, e);
        }

        private void OnComma(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_COMMA, sender, e);
        }

        private void OnPeriod(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_PERIOD, sender, e);
        }

        private void OnForwardSlash(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.OEM_2, sender, e);
        }

        private void OnSpace(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.SPACE, sender, e);
        }

        private void OnUp(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.UP, sender, e);
        }

        private void OnLeft(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.LEFT, sender, e);
        }

        private void OnDown(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.DOWN, sender, e);
        }

        private void OnRight(object sender, RoutedEventArgs e)
        {
            keyPress(VirtualKeyCode.RIGHT, sender, e);
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpperKeys()
        {
            this.Tilde.Content = "~\r";
            this.One.Content = "!";
            this.Two.Content = "@";
            this.Three.Content = "#";
            this.Four.Content = "$";
            this.Five.Content = "%";
            this.Six.Content = "^";
            this.Seven.Content = "&";
            this.Eight.Content = "*";
            this.Nine.Content = "(";
            this.Zero.Content = ")";
            this.Minus.Content = "_";
            this.Equal.Content = "+";
            this.BackSlash.Content = "|";
            this.RightBracket.Content = "}";
            this.LeftBracket.Content = "{";
            this.P.Content = "P";
            this.O.Content = "O";
            this.I.Content = "I";
            this.U.Content = "U";
            this.T.Content = "T";
            this.Y.Content = "Y";
            this.R.Content = "R";
            this.E.Content = "E";
            this.W.Content = "W";
            this.Q.Content = "Q";
            this.Quote.Content = "\"";
            this.SemiColon.Content = ":";
            this.L.Content = "L";
            this.K.Content = "K";
            this.J.Content = "J";
            this.G.Content = "G";
            this.H.Content = "H";
            this.F.Content = "F";
            this.D.Content = "D";
            this.S.Content = "S";
            this.A.Content = "A";
            this.ForwardSlash.Content = "?";
            this.Period.Content = ">";
            this.Comma.Content = "<";
            this.M.Content = "M";
            this.B.Content = "B";
            this.N.Content = "N";
            this.V.Content = "V";
            this.C.Content = "C";
            this.X.Content = "X";
            this.Z.Content = "Z";
        }

        private void LowerKeys()
        {
            this.Tilde.Content = "`";
            this.One.Content = "1";
            this.Two.Content = "2";
            this.Three.Content = "3";
            this.Four.Content = "4";
            this.Five.Content = "5";
            this.Six.Content = "6";
            this.Seven.Content = "7";
            this.Eight.Content = "8";
            this.Nine.Content = "9";
            this.Zero.Content = "0";
            this.Minus.Content = "-";
            this.Equal.Content = "=";
            this.BackSlash.Content = "\\";
            this.RightBracket.Content = "]";
            this.LeftBracket.Content = "[";
            this.P.Content = "p";
            this.O.Content = "o";
            this.I.Content = "i";
            this.U.Content = "u";
            this.T.Content = "y";
            this.Y.Content = "t";
            this.R.Content = "r";
            this.E.Content = "e";
            this.W.Content = "w";
            this.Q.Content = "q";
            this.Quote.Content = "'";
            this.SemiColon.Content = ";";
            this.L.Content = "l";
            this.K.Content = "k";
            this.J.Content = "j";
            this.H.Content = "h";
            this.G.Content = "g";
            this.F.Content = "f";
            this.D.Content = "d";
            this.S.Content = "s";
            this.A.Content = "a";
            this.ForwardSlash.Content = "/";
            this.Period.Content = ".";
            this.Comma.Content = ",";
            this.M.Content = "m";
            this.N.Content = "n";
            this.B.Content = "b";
            this.V.Content = "v";
            this.C.Content = "c";
            this.X.Content = "x";
            this.Z.Content = "z";
        }

        private void keyPress(VirtualKeyCode key, object sender, RoutedEventArgs e)
        {
            if (shift || caps)
            {
                inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, key);

                if (shift && !caps)
                {
                    OnShift(sender, e);
                }
            }
            else
            {
                inputSimulator.Keyboard.KeyPress(key);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }
}
