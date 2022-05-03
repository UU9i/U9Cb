using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.Devices;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace U9Cb.MVVM.View
{

    public partial class FileView : UserControl
    {
        bool isClear = false;
        public FileView()
        {
            InitializeComponent();
        }

        string[] fullPath;//拖入的文件夹全路径
        private void FileViewTreeView_Drop(object sender, DragEventArgs e)
        {
            fullPath = (string[])e.Data.GetData(DataFormats.FileDrop);//    拖入的文件数组 有多个第一级目录  E:\JiYF\BenXH E:\JiYF\BenXH2 E:\JiYF\BenXH3 E:\JiYF\1.jpg

            TreeViewDropLoop(fullPath, FileViewTreeView);

        }


        private void Button_DeletST_Click(object sender, RoutedEventArgs e)
        {

            if (DeletST.Text.Length == 1)
            {
                if (DeletST.Text == "(" || DeletST.Text == ")")
                {
                    Re(@"\(.*?\)");
                }
                else if (DeletST.Text == "[" || DeletST.Text == "]")
                {
                    Re(@"\[.*?\]");
                }
                else if (DeletST.Text == "{" || DeletST.Text == "}")
                {
                    Re(@"\{.*?\}");
                }
                else if (DeletST.Text == "（" || DeletST.Text == "）")
                {
                    Re(@"（.*?）");
                }
                else if (DeletST.Text == "【" || DeletST.Text == "】")
                {
                    Re(@"【.*?】");
                }
                else if (DeletST.Text == "｛" || DeletST.Text == "｝")
                {
                    Re(@"｛.*?｝");
                }
                else
                {
                    string delet = Regex.Escape(DeletST.Text);
                    string Textbox_Delete = "(" + delet + ")" + "*";
                    Re(Textbox_Delete);
                }
            }
            else
            {
                string delet = Regex.Escape(DeletST.Text);
                string Textbox_Delete = "(" + delet + ")" + "*";
                Re(Textbox_Delete);
            }
        }
        private void Button_AddST_Click(object sender, RoutedEventArgs e)
        {
            Add(AddST.Text);
        }

        private void Button_AddTime_Click(object sender, RoutedEventArgs e)
        {

            AddTime();
        }
        private void Button_Hidden(object sender, RoutedEventArgs e)
        {
            Attrib("attrib +s +a +h +r \"", "已隐藏");
        }
        private void Button_UnHidden(object sender, RoutedEventArgs e)
        {
            Attrib("attrib -s -a -h -r \"", "已取消隐藏");
        }


        void AddTime()
        {
            try
            {
                Computer MyComputer = new Computer();
                string fileName = null;
                string sourcePath = null;
                string destPath = null;
                string format = "";
                string squareLeft = "";
                string squareRight = "";
                int num = 0;
                string CM = "";
                if (CheckBox_TimeHms.IsChecked == false)
                {
                    format = "yyyy-MM-dd";
                }
                else
                {
                    format = "yyyy-MM-dd HH:mm:ss";
                }
                if (TimeHms1.IsChecked == true)
                {
                    squareLeft = "";
                    squareRight = "-";
                }
                else if (TimeHms2.IsChecked == true)
                {
                    squareLeft = "[";
                    squareRight = "]";
                }
                else if (TimeHms3.IsChecked == true)
                {
                    squareLeft = "(";
                    squareRight = ")";
                }
                for (int i = 0; i <= fullPath.Length - 1; i++)
                {
                    if (File.Exists(fullPath[i]))
                    {//如果是文件
                        FileInfo di = new FileInfo(fullPath[i]);
                        fileName = di.Name;

                        sourcePath = di.FullName;
                        destPath = Directory.GetParent(di.FullName).FullName + "\\";
                        if (CreateTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.CreationTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }
                        else if (ModifyTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.LastWriteTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }
                        else if (AccessTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.LastAccessTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }
                        fullPath[i] = destPath + fileName;

                        while (File.Exists(fullPath[i]))
                        {//重名则需要加(1)
                            string fileNameWE = Path.GetFileNameWithoutExtension(fullPath[i]);
                            string fileE = Path.GetExtension(fullPath[i]);
                            CM = "但有重名文件";
                            num++;
                            fileName = fileNameWE + "(" + num + ")" + fileE;
                            fullPath[i] = destPath + fileName;
                        }

                        MyComputer.FileSystem.RenameFile(sourcePath, fileName);    //di.FullName是路径,后面的是重命名后的文件名

                    }
                    if (Directory.Exists(fullPath[i]))
                    {//如果是文件夹
                        DirectoryInfo di = new DirectoryInfo(fullPath[i]);
                        fileName = di.Name;
                        sourcePath = di.FullName;
                        destPath = Directory.GetParent(di.FullName).FullName + "\\";

                        if (CreateTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.CreationTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }
                        else if (ModifyTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.LastWriteTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }
                        else if (AccessTime.IsChecked == true)
                        {
                            fileName = squareLeft + di.LastAccessTime.ToString(format).Replace(":", "`") + squareRight + fileName;
                        }

                        fullPath[i] = destPath + fileName;

                        while (Directory.Exists(fullPath[i]))
                        {
                            //重名则需要加(1)
                            num++;
                            CM = "但有重名文件";
                            fileName = fileName + "(" + num + ")";
                            fullPath[i] = destPath + fileName;
                        }

                        MyComputer.FileSystem.RenameDirectory(sourcePath, fileName);    //di.FullName是路径,后面的是重命名后的文件名

                    }
                    num = 0;
                }
                Result.Text = "已添加" + CM;
                CM = "";
                TreeViewDropLoop(fullPath, FileViewTreeView);//用于刷新
            }
            catch (System.NullReferenceException) { Result.Text = "未拖入文件"; }

        }


        void Re(string a)
        {
            try
            {
                Computer MyComputer = new Computer();
                string fileNameWE = null;
                string fileE = null;
                string fileName = null;
                string sourcePath = null;
                string destPath = null;
                string oriFileName = null;
                string CM = null;
                int num = 0;

                for (int i = 0; i <= fullPath.Length - 1; i++)
                {
                    if (File.Exists(fullPath[i]))
                    {//如果是文件
                        FileInfo di = new FileInfo(fullPath[i]);
                        oriFileName = di.Name;
                        fileName = di.Name;
                        sourcePath = di.FullName;

                        fileNameWE = Path.GetFileNameWithoutExtension(fullPath[i]);
                        fileE = Path.GetExtension(fullPath[i]);


                        destPath = Directory.GetParent(di.FullName).FullName + "\\";

                        fileNameWE = Regex.Replace(fileNameWE, a, "").Trim();//trim()去掉开头的多余空格

                        fileName = fileNameWE + fileE;

                        fullPath[i] = destPath + fileName;


                        while (File.Exists(fullPath[i]) && fileName != oriFileName)
                        {//重名则需要加(1)

                            CM = "但有重名文件";
                            num++;
                            fileName = fileNameWE + "(" + num + ")" + fileE;
                            fullPath[i] = destPath + fileName;
                        }
                        if (fileName != oriFileName)
                        {
                            MyComputer.FileSystem.RenameFile(sourcePath, fileName);    //di.FullName是路径,后面的是重命名后的文件名
                        }

                    }

                    if (Directory.Exists(fullPath[i]))
                    {//如果是文件夹
                        DirectoryInfo di = new DirectoryInfo(fullPath[i]);
                        oriFileName = di.Name;
                        fileName = di.Name;
                        sourcePath = di.FullName;
                        destPath = Directory.GetParent(di.FullName).FullName + "\\";


                        fileName = Regex.Replace(fileName, a, "").Trim();//先去掉多余空格

                        fullPath[i] = destPath + fileName;

                        while (Directory.Exists(fullPath[i]) && fileName != oriFileName)
                        {
                            //重名则需要加(1)
                            num++;
                            CM = "但有重名文件";
                            fileName = fileName + "(" + num + ")";
                            fullPath[i] = destPath + fileName;
                        }

                        if (fileName != oriFileName)
                        {
                            MyComputer.FileSystem.RenameDirectory(sourcePath, fileName);    //di.FullName是路径,后面的是重命名后的文件名
                        }

                    }
                    num = 0;
                }
                Result.Text = "已删除" + CM;
                CM = "";
                TreeViewDropLoop(fullPath, FileViewTreeView);//用于刷新
            }
            catch (System.NullReferenceException) { Result.Text = "未拖入文件"; }


        }
        void Add(string a)
        {
            try
            {
                Computer MyComputer = new Computer();
                string fileName = null;
                string oriFileName = null;
                string sourcePath = null;
                string destPath = null;
                int num = 0;
                string CM = "";
                for (int i = 0; i <= fullPath.Length - 1; i++)
                {
                    if (File.Exists(fullPath[i]))
                    {//如果是文件
                        FileInfo di = new FileInfo(fullPath[i]);
                        fileName = di.Name;
                        oriFileName = di.Name;
                        sourcePath = di.FullName;

                        destPath = Directory.GetParent(di.FullName).FullName + "\\";

                        fileName = a + fileName;
                        fullPath[i] = destPath + fileName;
                        while (File.Exists(fullPath[i])&&fileName!= oriFileName)
                        {//重名则需要加(1)
                            string fileNameWE = Path.GetFileNameWithoutExtension(fullPath[i]);
                            string fileE = Path.GetExtension(fullPath[i]);
                            CM = "但有重名文件";
                            num++;
                            fileName = fileNameWE + "(" + num + ")" + fileE;
                            fullPath[i] = destPath + fileName;
                        }
                        if(fileName != oriFileName)
                        {
                        MyComputer.FileSystem.RenameFile(sourcePath, fileName);//di.FullName是路径,后面的是重命名后的文件名
                        }
                    }
                    else if (Directory.Exists(fullPath[i]))
                    {//如果是文件夹
                        DirectoryInfo di = new DirectoryInfo(fullPath[i]);
                        fileName = di.Name;
                        oriFileName = di.Name;
                        sourcePath = di.FullName;
                        destPath = Directory.GetParent(di.FullName).FullName + "\\";

                        fileName = a + fileName;
                        fullPath[i] = destPath + fileName;

                        while (Directory.Exists(fullPath[i]) && fileName != oriFileName)
                        {
                            //重名则需要加(1)
                            num++;
                            CM = "但有重名文件";
                            fileName = fileName + "(" + num + ")";
                            fullPath[i] = destPath + fileName;
                        }
                        if (fileName != oriFileName)
                        {
                            MyComputer.FileSystem.RenameDirectory(sourcePath, fileName);    //di.FullName是路径,后面的是重命名后的文件名
                        }
                    }
                    num = 0;
                }
                Result.Text = "已添加" + CM;
                CM = "";
                TreeViewDropLoop(fullPath, FileViewTreeView);//用于刷新
            }
            catch (System.NullReferenceException) { Result.Text = "未拖入文件"; }

        }


        void Attrib(string a,string b)
        {
            try
            {
                for (int i = 0; i < fullPath.Length; i++)
                {

                    string dypath = a + fullPath[i] + "\"";
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";//设置要启动的应用程序
                    p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;// 接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;//输出信息
                                                              //p.StartInfo.RedirectStandardError = true;// 输出错误
                    p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                    p.Start();//启动程序

                    p.StandardInput.WriteLine(dypath + " & exit");

                    //  "copy/b" + s[0] + s[1] + System.IO.Path.GetFullPath(s[0]) + "example.jpg" //向cmd窗口发送输入信息
                    //p.StandardInput.AutoFlush = true;//string strOuput = p.StandardOutput.ReadToEnd();//获取输出信息
                    //p.WaitForExit();//等待程序执行完退出进程
                    p.Close();

                }
                Result.Text = b;
            }
            catch (System.NullReferenceException)
            {
                Result.Text = "未拖入文件";
            }
        }





        private void TreeViewDropLoop(string[] path, TreeView tree1)
        {

            if (!isClear) //先清空一次先前保存的数据
            {
                tree1.Items.Clear();
                isClear = true;
            }
            //默认拖入为 文件夹
            for (int i = 0; i < path.Length; i++)
            {//通过for循环 将拖入的第一层级文件夹全部显示

                TreeViewItem tvi = new TreeViewItem();//    创建一个TreeViewItem数组
                tvi.Header = Path.GetFileName(path[i]);//   获取拖入文件夹的名称作为Header
                tree1.Items.Add(tvi);
            }
            isClear = false;
        }
        private void FileViewTreeView_DragEnter(object sender, DragEventArgs e)
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
