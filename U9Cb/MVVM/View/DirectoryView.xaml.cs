using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.Devices;
using U9Cb.Core;
namespace U9Cb.MVVM.View
{
    #region 
    public partial class DirectoryView : UserControl
    {
        [System.Runtime.InteropServices.DllImport("Shlwapi.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]    //Win10按文件名称排序
        public static extern int StrCmpLogicalW(string psz1, string psz2);

        //list.Sort(StrCmpLogicalW);   //使用方法

        /// <summary>
        　　/// C#按文件名排序（顺序）
        　　/// </summary>
        　　/// <param name="arrFi">待排序数组</param>

        public FileInfo[] GetFilesNameSort(string path, out DirectoryInfo DiInfo)
        {//GetFiles按照Win10名称排序
            DiInfo = new DirectoryInfo(path);

            FileInfo[] files = DiInfo.GetFiles();//获取DocumentsFullPath数组元素路径下的文件

            Array.Sort(files, new FileNameSort());//调用windous的dll 正统!完全正确!

            //Array.Sort(files, (x1, x2) => int.Parse(Regex.Match(x1.Name.PadLeft(10, '0'), @"\d+").Value).CompareTo(int.Parse(Regex.Match(x2.Name.PadLeft(10, '0'), @"\d+").Value)));//名称排序不大行 和windows排序不同

            //Array.Sort(files, (x1, x2) => x1.Name.PadLeft(10, '0').CompareTo(x2.Name.PadLeft(10, '0')));
            return files;
        }
        public DirectoryInfo[] GetDirectoriesNameSort(string path, out DirectoryInfo DiInfo)
        {//GetDirectories按照Win10名称排序

            DiInfo = new DirectoryInfo(path);

            DirectoryInfo[] dirs = DiInfo.GetDirectories();

            Array.Sort(dirs, new FileNameSort());

            //Array.Sort(dirs, (x1, x2) => int.Parse(Regex.Match(x1.Name.PadLeft(10, '0'), @"\d+").Value).CompareTo(int.Parse(Regex.Match(x2.Name.PadLeft(10, '0'), @"\d+").Value)));

            //Array.Sort(dirs, (x1, x2) => x1.Name.PadLeft(10, '0').CompareTo(x2.Name.PadLeft(10, '0')));

            return dirs;
        }


        bool isClear = false;


        private void TreeViewDropLoop(string[] path, string[] mainPath, TreeView tree1, List<string> list, List<string> list_Flag_MainFolder, List<string> list_Folders)
        {//path是拖入的文件路径(层级随递归改变)||mainPath是拖入的主文件路径(就你拖入的那几个)||list保存文件路径||list_Flag_MainFolder保存list列表每个元素对应的拖入的那个主目录路径||list_Folders保存每一个文件夹路径
         //path[0]=D:\测试文件夹1
            try
            {
                if (!isClear)
                {
                    list.Clear();
                    list_Flag_MainFolder.Clear();
                    list_Folders.Clear();
                    tree1.Items.Clear();
                    isClear = true;
                }
                //默认拖入为 文件夹
                for (int i = 0; i < path.Length; i++)
                {//通过for循环 将拖入的第一层级文件夹全部显示

                    TreeViewItem tvi = new TreeViewItem();//    创建一个TreeViewItem数组
                    tvi.Header = Path.GetFileName(path[i]);//   获取拖入文件夹的名称作为Header
                    tree1.Items.Add(tvi);

                    list_Folders.Add(path[i]);//获取每一个 文件夹 的路径

                    TreeViewModel(path[i], mainPath[i], tvi, list, list_Flag_MainFolder, list_Folders);
                    //path[i]=D:\测试文件夹i
                    //检测拖入的 主目录 下有无文件


                    FileInfo[] preFiles = GetFilesNameSort(path[i], out DirectoryInfo di2);


                    foreach (FileInfo k in preFiles)
                    {
                        TreeViewItem fileItem = new TreeViewItem();

                        fileItem.Header = k.Name;

                        tvi.Items.Add(fileItem);

                        list.Add(k.FullName);//将该 文件 全路径加入list

                        list_Flag_MainFolder.Add(mainPath[i]);//同时记录主目录

                    }
                    //检测拖入的 主目录 下有无文件

                }

                isClear = false;
            }
            catch (DirectoryNotFoundException)
            {
                AllClear();
            }
        }

        public void TreeViewModel(string path, string mainPath, TreeViewItem tree1, List<string> list, List<string> list_Flag_MainFolder, List<string> list_Folders)
        {//path=D:/测试文件夹i

            try
            {
                DirectoryInfo[] dirs = GetDirectoriesNameSort(path, out DirectoryInfo di);
                foreach (DirectoryInfo i in dirs)   //用i去遍历子 文件夹 目录

                { //将递归遍历得到的 文件夹 路径与treeviewitem节点进行对应,并动态创建treeviewitem的Selected事件(选中事件),触发Selected事件,将该目录下得到的所有 文件夹 和 文件 路径添加到list4集合,若在文件夹之下遍历到 子文件夹 则创建子节点与 子文件夹 对应

                    TreeViewItem subDi = new TreeViewItem();     //子目录的TreeView节点

                    subDi.Header = i.Name;       //以 文件夹 名作为子节点的名字

                    tree1.Items.Add(subDi);     //添加 文件夹 子节点

                    list_Folders.Add(i.FullName);//递归获取子目录 文件夹


                    TreeViewModel(i.FullName, mainPath, subDi, list, list_Flag_MainFolder, list_Folders);


                    FileInfo[] preFiles = GetFilesNameSort(i.FullName, out DirectoryInfo di2);


                    foreach (FileInfo k in preFiles)
                    {
                        TreeViewItem fileItem = new TreeViewItem();

                        fileItem.Header = k.Name;

                        subDi.Items.Add(fileItem);

                        list.Add(k.FullName);//将该 文件 全路径加入list

                        list_Flag_MainFolder.Add(mainPath);//同时记录主目录
                    }
                }

            }
            catch (DirectoryNotFoundException)
            {
                AllClear();
            }


        }
        public void TreeView_Reflash(string[] a, string[] b, TreeView tvi, List<string> c, List<string> d, List<string> e)
        {//用于完成操作后刷新TreeView
            c.Clear();  //清空
            d.Clear(); //清空
            TreeViewDropLoop(a, a, tvi, c, d, e);   //显示移动后的文件夹目录

        }






        public DirectoryView()
        {
            InitializeComponent();
        }

        #endregion

        List<string> list = new List<string>(); //该集合存储 文件 的路径

        List<string> list_Flag_MainFolder = new List<string>(); //用作下标记录当前是属于哪个 主文件夹路径

        List<string> list_Folders = new List<string>();  //用于记录当前文件的 父文件夹 路径


        string[] DocumentsFullPath;//拖入的文件夹全路径  D:\测试文件夹1 D:\测试文件夹2 两个一起拖入  则 DocumentsFullPath[0]=D:\测试文件夹1  DocumentsFullPath[1]=D:\测试文件夹
        private void TreeView_Drop(object sender, DragEventArgs e)
        {

            try
            {
                DocumentsFullPath = (string[])e.Data.GetData(DataFormats.FileDrop);//    拖入的文件夹数组 有多个第一级目录  E:\JiYF\BenXH E:\JiYF\BenXH2 E:\JiYF\BenXH3

                TreeViewDropLoop(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
            }
            catch (IOException)
            {
                AllClear();
                Result.Text = "拖入的不是文件夹";
            }

        }

        private void Btn_Integration_Click(object sender, RoutedEventArgs e)
        {//文件夹整合
            try
            {
                int i = 1;//文件编号从001开始 (不一定三位

                int filesSum = list.Count;//获取总共有多少文件

                foreach (string a in DocumentsFullPath)
                {

                    FileInfo[] files = GetFilesNameSort(a, out DirectoryInfo di);

                    foreach (FileInfo file in files)
                    {
                        string extension = Path.GetExtension(file.FullName);//后缀名包含 . 点

                        string num = i.ToString().PadLeft(filesSum.ToString().Length, '0');//files.Length.ToString().Length 根据文件数量让编号位数不同 如 001  002  003  或者 01 02 03

                        if (!Directory.Exists(DocumentsFullPath[0] + "[整合]"))//检测是否存在该文件夹
                        {
                            Directory.CreateDirectory(DocumentsFullPath[0] + "[整合]");
                        }
                        if (CheckBox_Path.IsChecked == false)
                        {//不保留路径 就直接整合为 01 02 03 04 
                            File.Move(file.FullName, DocumentsFullPath[0] + "[整合]" + "\\" + num + extension);
                        }
                        else
                        {//保留路径 就整合为 文件夹1-01 文件夹1-02 ..... 文件夹2-01 文件夹2-02

                            File.Move(file.FullName, DocumentsFullPath[0] + "[整合]" + "\\" + "[" + di.Name + "]" + num + extension);
                        }

                        i++;        //文件编号+1
                    }

                    if (CheckBox_Path.IsChecked == true)
                    {//保留路径 所以下一个文件夹 重新计数 i要变成1
                        i = 1;
                    }
                }
                DocumentsFullPath[0] = DocumentsFullPath[0] + "[整合]";
                TreeView_Reflash(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
                Result.Text = "已整合";
            }
            catch (NullReferenceException)
            {
                Result.Text = "未拖入文件夹";
            }


        }
        private void Btn_Rename_Click(object sender, RoutedEventArgs e)
        {

            foreach (string a in list_Folders)
            {
                bool flag = true;//检测是否有重名但不是自己的文件

                while (flag)
                {
                    int i = 1;//文件编号从001开始 (不一定三位
                    int i_duplication = 1;//重名序号
                    FileInfo[] files = GetFilesNameSort(a, out DirectoryInfo di);
                    flag = false;
                    foreach (FileInfo file in files)
                    {
                        Computer MyComputer = new Computer();

                        string num = i.ToString().PadLeft(files.Length.ToString().Length, '0');//files.Length.ToString().Length 根据文件数量让编号位数不同 如 001  002  003  或者 01 02 03
                        string extension = Path.GetExtension(file.FullName);//后缀名包含 . 点

                        if (File.Exists(di.FullName + "\\" + num + extension) && !file.FullName.Equals(di.FullName + "\\" + num + extension))
                        {//若有重名但不是自己

                            if (!flag)
                            {//有重名但不是自己 接下来还要循环一遍
                                flag = true;
                            }

                            while (File.Exists(di.FullName + "\\" + num + "(" + i_duplication + ")" + extension))
                            {
                                i_duplication++;//编号增加
                            }
                            MyComputer.FileSystem.RenameFile(file.FullName, num + "(" + i_duplication + ")" + extension);//先把自己改名 如 1(1).jpg


                            i_duplication = 1;//编号归一
                        }
                        else if (!File.Exists(di.FullName + "\\" + num + extension))
                        {//若无重名

                            MyComputer.FileSystem.RenameFile(file.FullName, num + extension);    //file.FullName是路径,后面的是重命名后的文件名

                        }
                        else
                        {
                            //有重名 就是自己所以就不用操作了
                        }
                        i++;
                    }
                }

            }
            Result.Text = "已排序";
            try
            {
                TreeView_Reflash(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
            }


            catch (NullReferenceException)
            {
                Result.Text = "未拖入文件夹";
            }

        }


        private void Btn_MoveOut_Click(object sender, RoutedEventArgs e)
        {//点击移出按钮
            try
            {
                if (FileName.IsChecked == true)
                {

                    for (int i = 0; i < list.Count; i++)
                    {

                        string path_Replace = Regex.Replace(list[i], "[\\\\]", "][").Replace(":", "][").Remove(0, Directory.GetParent(list_Flag_MainFolder[i]).FullName.Length);//将路径中的斜杠与冒号替换成横杠作为文件名 同时去掉开头部分字符 使得名称开始为拖入的文件夹名称
                                                                                                                                                                                //变成[文件夹1][文件夹]01.jpg的样式
                        string fileName = Path.GetFileName(list[i]);//获取文件名

                        string oriList = list[i].ToString();


                        if (File.Exists(list_Flag_MainFolder[i] + "\\" + fileName))
                        {//如果存在同名文件
                            if (oriList != list[i])
                            {
                                string NameReplace = Regex.Replace(list_Flag_MainFolder[i] + "\\" + "[" + path_Replace + "]", @"\[" + fileName + "]", string.Empty);//将原本路径中的   [文件夹1-01.jpg]-01.jpg  变为[文件夹1]

                                File.Move(list[i], NameReplace + fileName);
                            }
                        }
                        else
                        {
                            string NameReplace = Regex.Replace(list_Flag_MainFolder[i] + "\\" + "[" + path_Replace + "]", @"\[" + fileName + "]", string.Empty);//将原本路径中的   [文件夹1-01.jpg]  变为[文件夹1]

                            File.Move(list[i], list_Flag_MainFolder[i] + "\\" + fileName);
                        }

                    }

                    TreeView_Reflash(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
                }
                else
                {

                    for (int i = 0; i < list.Count; i++)
                    {
                        string path_Replace = Regex.Replace(list[i], "[\\\\]", "][").Replace(":", "][").Remove(0, Directory.GetParent(list_Flag_MainFolder[i]).FullName.Length);//将路径中的斜杠与冒号替换成横杠作为文件名 同时去掉开头部分字符 使得名称开始为拖入的文件夹名称
                                                                                                                                                                                //变成[文件夹1][文件夹]01.jpg的样式
                        string fileName = Path.GetFileName(list[i]);//获取文件名

                        string NameReplace = Regex.Replace(list_Flag_MainFolder[i] + "\\" + "[" + path_Replace + "]", @"\[" + fileName + "]", string.Empty);//将原本路径中的   [文件夹1-01.jpg]  变为[文件夹1]

                        File.Move(list[i], NameReplace + fileName);
                    }


                    TreeView_Reflash(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
                }
                Result.Text = "已移出";
            }
            catch (NullReferenceException)
            {
                Result.Text = "未拖入文件夹";
            }

        }
        private void Btn_DeletFiles_Click(object sender, RoutedEventArgs e)
        {//删除拖入文件夹内所有指定后缀的文件
            foreach (string a in list)
            {
                if (Path.GetExtension(a).Equals("." + TextBox_DeletFiles.Text, StringComparison.OrdinalIgnoreCase))
                {
                    FileSystem.DeleteFile(a, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
            try
            {
                TreeView_Reflash(DocumentsFullPath, DocumentsFullPath, TreeView, list, list_Flag_MainFolder, list_Folders);
            }
            catch (NullReferenceException)
            {
                Result.Text = "未拖入文件夹";
            }

        }

        void AllClear()
        {
            list.Clear();
            list_Flag_MainFolder.Clear();
            list_Folders.Clear();
            DocumentsFullPath = null;
            TreeView.Items.Clear();
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e)
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
