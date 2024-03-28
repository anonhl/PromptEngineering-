using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // 提供用于处理文件和目录的类型
using System.Diagnostics;



namespace prompt
{
    public partial class Form1 : Form
    {
        private string path1; // 用于保存路径1
        private string path2; // 用于保存路径2
        
        public Form1()
        {
            InitializeComponent();
            // 初始化路径
            path1 = "F:/信息";
            path2 = "";

            // 根据path1加载TreeView
            LoadTreeView(path1);
        }
        private void LoadTreeView(string path)
        {

            // 创建一个节点，用于表示当前路径
            TreeNode rootNode = new TreeNode(Path.GetFileName(path));
            rootNode.Tag = path;  // 把完整路径保存在Tag属性中，以便后续使用
            treeView1.Nodes.Add(rootNode);

            // 添加文件夹和文件到TreeView
            AddDirectoriesAndFilesToTreeView(rootNode);

            // 展开TreeView的根节点
            rootNode.Expand();
        }

        private void AddDirectoriesAndFilesToTreeView(TreeNode parentNode)
        {
            // 获取parentNode所表示的路径
            string path = (string)parentNode.Tag;

            // 获取该路径下的所有目录，并为每个目录创建一个节点，添加到parentNode下
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                TreeNode directoryNode = new TreeNode(Path.GetFileName(directory));
                directoryNode.Tag = directory;  // 把完整路径保存在Tag属性中，以便后续使用
                parentNode.Nodes.Add(directoryNode);

                // 递归地添加该目录下的目录和文件
                AddDirectoriesAndFilesToTreeView(directoryNode);
            }

            // 获取该路径下的所有文件，并为每个文件创建一个节点，添加到parentNode下
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                TreeNode fileNode = new TreeNode(Path.GetFileName(file));
                fileNode.Tag = file;  // 把完整路径保存在Tag属性中，以便后续使用
                parentNode.Nodes.Add(fileNode);

                // 如果文件是"提示词.txt"，则读取文件内容并创建子节点
                if (Path.GetFileName(file) == "提示词.txt")
                {
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        TreeNode lineNode = new TreeNode(line);
                        fileNode.Nodes.Add(lineNode);
                    }
                }
            }
        }



        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            // 获取鼠标位置下的节点
            TreeNode node = treeView1.GetNodeAt(e.X, e.Y);

            if (node != null)
            {
                // 选中这个节点
                treeView1.SelectedNode = node;

                // 检测是否为左键点击
                if (e.Button == MouseButtons.Left)
                {
                    // 获取鼠标位置下的节点
                    TreeNode selectedNode = treeView1.GetNodeAt(e.X, e.Y);
                    if (selectedNode != null)
                    {
                        // 选中这个节点
                        treeView1.SelectedNode = selectedNode;

                        // 检查选中的节点是否是"提示词.txt"
                        if (selectedNode.Text == "提示词.txt")
                        {
                            // 获取节点的完整路径，并移除根节点部分
                            string relativePath = selectedNode.FullPath.Substring(treeView1.Nodes[0].Text.Length + 1);  // +1 是因为还有一个分隔符

                            // 更新路径2为"提示词.txt"所在的文件夹路径
                            path2 = Path.Combine(path1, relativePath);
                            path2 = Path.GetDirectoryName(path2);
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // 获取节点的文本内容
                    string nodeText = node.Text;

                    // 如果文本内容存在以“----”的分隔符
                    if (nodeText.Contains("----"))
                    {
                        string[] parts = nodeText.Split(new string[] { "----" }, StringSplitOptions.None);

                        if (checkBox2.Checked)
                        {
                            // 如果选择框2选中，复制中间的内容
                            Clipboard.SetText(parts[1]);
                        }
                        else
                        {
                            // 如果选择框2未选中，复制右边的内容
                            Clipboard.SetText(parts[2]);
                        }
                    }
                }
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)  // 这里 checkBox1 是选择框1的名称
            {
                string contentToWrite = Environment.NewLine + textBox1.Text + "----" + textBox2.Text + "----" + textBox3.Text;  // 这里 textBox1, textBox2, textBox3 分别是文本框1,2,3的名称

                File.AppendAllText(path2 + @"\提示词.txt", contentToWrite, Encoding.UTF8);
            }
            else
            {
                MessageBox.Show("错误：没有进入编辑模式");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // 清空文本框1的内容
            textBox1.Text = "";

            // 清空文本框2的内容
            textBox2.Text = "";

            // 清空文本框3的内容
            textBox3.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 检查文件是否存在
            if (File.Exists(Path.Combine(path2, "提示词.txt")))
            {
                // 使用系统默认的文本编辑器打开文件
                System.Diagnostics.Process.Start(Path.Combine(path2, "提示词.txt"));
            }
            else
            {
                MessageBox.Show("文件不存在");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();  // 首先清除旧的节点
            LoadTreeView(path1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenPathInExplorer(path2);
        }

        private void OpenPathInExplorer(string path)
        {
            try
            {
                Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开路径: {path}\n错误详情: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 获取textBox1的内容作为文件夹名称
            string folderName = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(folderName))
            {
                MessageBox.Show("文件夹名称不能为空。");
                return;
            }

            // 定义目标路径
            string fullPath = Path.Combine(path2, folderName);

            // 检查文件夹是否存在
            if (Directory.Exists(fullPath))
            {
                MessageBox.Show("该文件夹已存在。");
            }
            else
            {
                try
                {
                    // 创建文件夹
                    Directory.CreateDirectory(fullPath);
                    // 定义目标路径
                    
                    // 在新建的文件夹中创建文本文件“提示词.txt”
                    string filePath = Path.Combine(fullPath, "提示词.txt");
                    // 使用File.Create来创建文件后，需要关闭该文件的句柄，否则可能会导致后续操作该文件时出现异常
                    using (var stream = File.Create(filePath)) { }
                    MessageBox.Show($"文件夹 '{folderName}' 和文件 '提示词.txt' 创建成功。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发生错误: {ex.Message}");
                }
            }
        }
    }
}
