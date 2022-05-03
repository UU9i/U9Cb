using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace U9Cb.MVVM.View
{

    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }
        Random rd = new Random();


        string[] text =
        {
            "心情会变好",
            "别看我烂代码哭兮兮(只要我代码写的够烂本身也是一种混淆",
            "想出去吃东西",
            "睡觉的时候下大雨",
            "家里的猫猫别咬我",
            "你不会反而把文件弄乱了吧不会吧不会吧",
            "希望躺着也能赚钱",
            "建议备份文件哦",
            "白天有太阳",
            "晚上有月亮"
        };
        private void RandomTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            int num = rd.Next(10);
            RandomTextBlock.Text = text[num];
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }
    }
}
