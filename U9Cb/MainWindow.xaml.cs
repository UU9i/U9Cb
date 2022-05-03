
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace U9Cb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool isTopmost =true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void U9CbWindow_Loaded(object sender, RoutedEventArgs e)
        {//监听键盘按键事件，如果按ESC则关闭该窗口
            this.KeyDown += Esc_Exit_KeyDown;
        }

        private void Esc_Exit_KeyDown(object sender, KeyEventArgs e)
        {//监听键盘按键事件，如果按ESC则关闭该窗口
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Btn_Topmost_Click(object sender, MouseButtonEventArgs e)
        {
            //置顶按钮
            {
                if (isTopmost)
                {
                    Topmost = false;
                    isTopmost = false;
                    BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Images/Pin.png", UriKind.Absolute));
                    Btn_Topmost.Source = image;
                }
                else
                {
                    Topmost = true;
                    isTopmost = true;
                    BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Images/PinTopmost.png", UriKind.Absolute));
                    Btn_Topmost.Source = image;
                }
            }
        }

        private void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
