using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace U9Cb.MVVM.View
{

    public partial class CopyBView : UserControl
    {
        public CopyBView()
        {
            InitializeComponent();
        }


        string[] file;
        string finalName;

        private void CopyBTreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                CopyBTreeView.Items.Clear();
                file = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i <= 1; i++)
                {
                    CopyBTreeView.Items.Add(file[i]);
                }
                FileStream fs = new FileStream(file[1], FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                string fileclass = "";
                for (int k = 0; k < 2; k++)
                {
                    fileclass += reader.ReadByte().ToString();
                }
                if (fileclass == "255216" || fileclass == "7173" || fileclass == "6677" || fileclass == "13780")
                {
                    string temp;
                    temp = file[1];
                    file[1] = file[0];
                    file[0] = temp;
                    //主要是把图片放在前面也就是file[0]是图片,file[1]是压缩包
                }
                fs.Close();
                reader.Close();
                FinalName();
            }
            catch (System.Exception exception) when (exception is System.IndexOutOfRangeException || exception is EndOfStreamException)
            {
                file = null;
                finalName = "";
                CopyBTreeView.Items.Clear();
                Result.Text = "拖入文件有误";
            }

        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dypath = "copy/b \"" + file[0] + "\"" + "+\"" + file[1] + "\"" + " \"" + Path.GetDirectoryName(file[0]) + "\\";//注记\"→"  \\→\
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";//设置要启动的应用程序
                p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;// 接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//输出信息
                //p.StartInfo.RedirectStandardError = true;// 输出错误
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                p.StandardInput.WriteLine(dypath + finalName + "\"" + " & exit");
                Result.Text = finalName + "已生成";


                //  "copy/b" + file[0] + file[1] + System.IO.Path.GetFullPath(file[0]) + "example.jpg" //向cmd窗口发送输入信息
                //p.StandardInput.AutoFlush = true;//string strOuput = p.StandardOutput.ReadToEnd();//获取输出信息
                //p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
            catch (System.NullReferenceException)
            {
                Result.Text = finalName + "未拖入文件";
            }

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FinalName();
        }

        public void FinalName()
        {
            int num=0;
            if (TextBox_Name.Text != "")
            {
                finalName = TextBox_Prefix.Text + TextBox_Name.Text + Path.GetExtension(file[0]);
            }
            else
            {
                finalName = TextBox_Prefix.Text + Path.GetFileNameWithoutExtension(file[1]) + Path.GetExtension(file[0]);
            }

            while(File.Exists(Path.GetDirectoryName(file[0]) + "\\" + finalName))
            {
                num++;
                finalName = TextBox_Prefix.Text + Path.GetFileNameWithoutExtension(file[1]) + "(" + num + ")" + Path.GetExtension(file[0]);
            }

            Result.Text = finalName;
        }
        private void CopyBTreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))

            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

    }
}
