using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DocumentSelector.Filter;
using System.Threading;

namespace DocumentSelector
{
    public partial class Form1 : Form
    {
        private List<Item> sourceFileList = new List<Item>();
        private List<Item> rightFileList = new List<Item>();

        private MyOpaqueLayer m_OpaqueLayer = null;//半透明蒙板层

        ImageForm imageForm = null;
        public Form1()
        {
            InitializeComponent();
            this.label4.Text = "";
            this.toolStripStatusLabel1.Text = "当前文件总数：0";
            this.toolStripStatusLabel2.Text = "选中文件数：0";
            this.toolStripStatusLabel3.Text = "当前文件总数：0";
            this.toolStripStatusLabel4.Text = "选中文件数：0";
            this.listBox1.DisplayMember = "text";
            this.listBox2.DisplayMember = "text";
            this.label2.Text = "";
            this.toolStripStatusLabel5.Text = "";
            this.label5.Text = "";
            this.label6.Text = "";
            this.label7.Text = "";
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.panel11.Visible = false;
        }

        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control"></param>
        /// <param name="alpha"></param>
        /// <param name="showLoadingImage"></param>
        protected void ShowOpaqueLayer(Control control, int alpha, bool showLoadingImage)
        {
            if (this.m_OpaqueLayer == null)
            {
                this.m_OpaqueLayer = new MyOpaqueLayer(alpha, showLoadingImage);
                control.Controls.Add(this.m_OpaqueLayer);
                this.m_OpaqueLayer.Dock = DockStyle.Fill;
                this.m_OpaqueLayer.BringToFront();
            }
            this.m_OpaqueLayer.Enabled = true;
            this.m_OpaqueLayer.Visible = true;


        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        protected void HideOpaqueLayer()
        {
            if (this.m_OpaqueLayer != null)
            {
                this.m_OpaqueLayer.Enabled = false;
                this.m_OpaqueLayer.Visible = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "C:\\";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
                button2_Click(this.button2, EventArgs.Empty);
            }
        }



        private bool existInlistBox(string fileName)
        {
            if (!this.checkBox1.Checked) return false;
            if (this.listBox1.Items.Count == 0) return false;
            foreach (var obj in this.listBox1.Items)
            {
                Item item = obj as Item;
                if (item != null && item.text == fileName)
                {
                    return true;
                }
            }

            return false;
        }

        private int sourceFileCount = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                return;
            }
            if (this.listBox1.DataSource == null)
            {
                if (MessageBox.Show("重新获取文件列表，是否确定继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            CheckForIllegalCrossThreadCalls = false;
            this.toolStripStatusLabel1.Text = "文件获取中，请稍候....";
            Application.DoEvents();
            try
            {
                FileSystemInfo fsi = new DirectoryInfo(this.textBox1.Text);
                string filter = this.textBox2.Text;
                this.listBox1.DataSource = null;
                sourceFileList.Clear();
                FileUtility.getAllFileUnderDirectory(fsi, filter, sourceFileList);

                this.listBox1.DataSource = sourceFileList;

                this.toolStripStatusLabel1.Text = "当前文件总数：" + sourceFileList.Count;
                this.label4.Text = "所有文件总数：" + sourceFileList.Count;
                sourceFileCount = sourceFileList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取文件列表出现异常，" + ex.Message);
            }
        }


        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pos = new Point(e.Node.Bounds.X + e.Node.Bounds.Width, e.Node.Bounds.Y + e.Node.Bounds.Height / 2);
                this.contextMenuStrip1.Show(this.treeView1, pos);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "C:\\";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                this.textBox3.Text = path;
                this.treeView1.Nodes.Clear();
                DirectoryInfo theFolder = new DirectoryInfo(path);

                TreeNode node = new TreeNode(this.textBox3.Text);
                NodeData data = new NodeData();
                data.title = this.textBox3.Text;
                data.path = this.textBox3.Text;
                node.Tag = data;
                addTreeViewNode(null, node);

                listDir(node, theFolder);
                this.treeView1.ExpandAll();
            }
        }

        private void listDir(TreeNode parentNode, DirectoryInfo theFolder)
        {
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            if (dirInfo.Length == 0)
            {
                return;
            }

            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                string path = NextFolder.FullName;
                string name = NextFolder.Name;
                TreeNode node = new TreeNode(name);
                NodeData data = new NodeData();
                data.title = name;
                data.path = path;
                //if (parentNode != null && parentNode.Tag != null)
                //{
                //    NodeData parentDate = parentNode.Tag as NodeData;
                //    if (parentDate.filter != null && parentDate.filter.Length > 0)
                //    {
                //        data.filter = parentDate.filter + "+" + name;

                //    }
                //    else
                //    {
                //        data.filter = name;
                //    }
                //}
                //node.Text = name + "(" + data.filter + ")";
                node.Tag = data;
                addTreeViewNode(parentNode, node);
                Application.DoEvents();
                listDir(node, NextFolder);
            }
        }

        private TreeNode addTreeViewNode(TreeNode parentNode, TreeNode node)
        {
            if (parentNode == null)
            {
                this.treeView1.Nodes.Add(node);

            }
            else
            {
                parentNode.Nodes.Add(node);
            }

            return node;
        }

        private void 新建文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;

            string title = Microsoft.VisualBasic.Interaction.InputBox("输入新建文件夹名称", "提示");
            if (title == null || title.Trim().Length == 0)
            {
                MessageBox.Show("请输入文件夹名称");
            }
            else
            {
                NodeData parentData = node.Tag as NodeData;
                TreeNode newnode = new TreeNode(title);
                NodeData data = new NodeData();
                data.title = title;
                data.path = parentData.path + "\\" + title;

                newnode.Tag = data;
                addTreeViewNode(node, newnode);
                FileUtility.createDirectory(data.path);
                node.Expand();
            }

        }

        private void 过滤条件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            string filter = Microsoft.VisualBasic.Interaction.InputBox("输入过滤条件", "提示");
            if (filter == null || filter.Trim().Length == 0)
            {
                MessageBox.Show("请输入过滤条件");
            }
            else
            {
                if (node != null)
                {
                    NodeData data = node.Tag as NodeData;
                    node.Text = data.title + "(过滤条件：" + filter + ")";
                    data.filter = filter;
                }
            }
        }

        private void 分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("请选择文件夹");
                return;
            }

            if (MessageBox.Show("按过滤条件进行文件分类，如果文件名出现同名，将覆盖，是否确定继续?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                NodeData data = node.Tag as NodeData;
                string path = data.path;
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }


                string filterStr = data.filter;
                IFilter filter = new StringFilter();


                int count = 0;
                foreach (var obj in this.listBox1.Items)
                {
                    Item item = obj as Item;
                    if (item == null) continue;
                    try
                    {
                        bool isMatch = filter.match(item.text, filterStr);
                        if (isMatch)
                        {
                            File.Copy(item.value, path + "\\" + item.text, true);
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                MessageBox.Show("分类完成，已经归类了" + count + "个文件");
            }



        }

        private void button5_Click(object sender, EventArgs e)
        {
            IFilter filter = new StringFilter();
            bool isMatch = filter.match("6.21苏宁云商.jpg", "苏宁+易购,云商");
            Console.WriteLine(isMatch);
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int posindex = listBox1.IndexFromPoint(new Point(e.X, e.Y));
                listBox1.ContextMenuStrip = null;
                if (posindex >= 0 && posindex <= listBox1.Items.Count)
                {
                    listBox1.SelectedIndex = posindex;
                    contextMenuStrip2.Show(listBox1, new Point(e.X, e.Y));
                }
            }

            listBox1.Refresh();
        }

        private void 排除文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ListBox.SelectedIndexCollection sic = listBox1.SelectedIndices;//得到选择的Item的下标
            if (sic.Count == 0) return;
            List<int> list = new List<int>();
            for (int i = 0; i < sic.Count; i++)
            {

                list.Add(sic[i]);

            }
            list.Sort();

            listBox1.DataSource = null;
            while (list.Count != 0)
            {
                //listBox1.Items.RemoveAt(list[list.Count - 1]);
                sourceFileList.RemoveAt(list[list.Count - 1]);
                list.RemoveAt(list.Count - 1);
            }

            listBox1.DataSource = sourceFileList;

        }

        private void 升序_Click(object sender, EventArgs e)
        {
            List<Item> list = this.sourceFileList.OrderBy(item => item.text).ToList();
            sourceFileList = list;
            listBox1.DataSource = null;
            listBox1.DataSource = sourceFileList;
        }

        private void 降序_Click(object sender, EventArgs e)
        {
            List<Item> list = this.sourceFileList.OrderByDescending(item => item.text).ToList();
            sourceFileList = list;
            listBox1.DataSource = null;
            listBox1.DataSource = sourceFileList;
        }

        private void 查询_Click(object sender, EventArgs e)
        {
            string cond = this.查询条件.Text;
            if (this.textBox1.Text == "")
            {
                return;
            }
            if (this.textBox3.Text == "")
            {
                return;
            }
            //if(cond=="") {
            //    listBox1.DataSource = null;
            //    listBox1.DataSource = this.sourceFileList;
            //    return;
            //}

            List<Item> sourceList = new List<Item>();
            FileSystemInfo sourceFSI = new DirectoryInfo(this.textBox1.Text);
            FileSystemInfo targetFSI = new DirectoryInfo(this.textBox3.Text);
            FileUtility.getAllFileUnderDirectory(sourceFSI, cond, sourceList);
            sourceFileList = sourceList;
            //List<Item> list = this.sourceFileList.FindAll(item => item.text.IndexOf(cond) > -1).ToList();
            if (this.checkBox1.Checked)
            {
                List<Item> list = compareFileList(targetFSI);
                sourceFileList = list;
            }

            listBox1.DataSource = null;
            listBox1.DataSource = sourceFileList;

            this.toolStripStatusLabel1.Text = "当前文件总数：" + sourceFileList.Count;

            //this.label4.Text = "当前文件总数：" + sourceFileList.Count;

            this.listBox1.SelectedIndex = -1;
            int count =  sourceFileList.Count;
            if (count > 0)
            {
                if (listbox_selectedIndex < count)
                {
                    this.listBox1.SelectedIndex = listbox_selectedIndex;
                }
                else
                {
                    this.listBox1.SelectedIndex = count - 1;
                }
            }

            int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
            listBox1.TopIndex = Math.Max(this.listBox1.SelectedIndex - visibleItems/2, 0);
        }

        private void 文件分类ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileClassify();

        }

        private void fileClassify()
        {
            TreeNode node = this.treeView1.SelectedNode as TreeNode;
            if (node == null)
            {
                MessageBox.Show("请选择分类的目标文件夹");
                return;
            }

            //if (MessageBox.Show("如果有同名文件将会被覆盖，是否确定继续?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            //{
            //    return;
            //}
            this.ShowOpaqueLayer(this.listBox1, 125, true);
            try
            {

                NodeData nodeDate = node.Tag as NodeData;
                string filePath = nodeDate.path;
                FileUtility.createDirectory(filePath);
                try
                {
                    for (int i = 0; i < this.listBox1.SelectedItems.Count; i++)
                    {
                        Item item = this.listBox1.SelectedItems[i] as Item;
                        if (item != null)
                        {
                            FileUtility.CopyFile(item.value, filePath + "\\" + item.text);
                        }

                        if (i % 50 == 49)
                        {
                            //loadFileListToListBox2();
                        
                            Application.DoEvents();
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("文件分类出现异常，请重新分类，异常原因：" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                loadFileListToListBox2();
                查询_Click(this.查询, EventArgs.Empty);

            }
            finally
            {
                this.HideOpaqueLayer();
            } 

        }

        private void listBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int posindex = listBox2.IndexFromPoint(new Point(e.X, e.Y));
                listBox2.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < listBox2.Items.Count)
                {
                    listBox2.SelectedIndex = posindex;


                    loadDynamicMenuItem(contextMenuStrip3);
                    contextMenuStrip3.Show(listBox2, new Point(e.X, e.Y));
                }
            }
            listBox2.Refresh();
        }

        private void loadDynamicMenuItem(ContextMenuStrip menu)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;

            if (this.移到子目录ToolStripMenuItem.DropDownItems.Count > 0)
            {
                this.移到子目录ToolStripMenuItem.DropDownItems.Clear();

            }
            if (node.Nodes.Count == 0)
            {
                this.移到子目录ToolStripMenuItem.Enabled = false;
                return;
            }

            this.移到子目录ToolStripMenuItem.Enabled = true;
            TreeNode childnode;
            ToolStripMenuItem tsmi;
            NodeData nodeData;
            foreach (var item in node.Nodes)
            {
                childnode = item as TreeNode;
                if (childnode != null)
                {
                    nodeData = childnode.Tag as NodeData;
                    tsmi = new ToolStripMenuItem(nodeData.title);
                    tsmi.Tag = nodeData.path;
                    tsmi.Click += new EventHandler(tsmi_Click);
                    this.移到子目录ToolStripMenuItem.DropDownItems.Add(tsmi);
                }
            }

        }

        void tsmi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (tsmi == null) return;
            string filename = tsmi.Text;
            string new_filepath = tsmi.Tag.ToString();
            if (MessageBox.Show("将选中的文件移至子目录【" + filename + "】，是否确定继续?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }

            Item item = null;
            foreach (var obj in this.listBox2.SelectedItems)
            {
                item = obj as Item;
                if (item != null)
                {
                    FileUtility.moveFile(item.value, new_filepath + "\\" + item.text);
                }
            }

            loadFileListToListBox2();
        }

        private void loadFileListToListBox2()
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;
            NodeData nodeDate = node.Tag as NodeData;
            DirectoryInfo theFolder = new DirectoryInfo(nodeDate.path);

            this.label2.Text = "选中的文件夹路径：" + nodeDate.path;
            this.toolStripStatusLabel5.Text = "选中的文件夹路径：" + nodeDate.path;

            List<Item> fileList = new List<Item>();
            if (this.checkBox2.Checked)
            {
                FileUtility.getAllFileUnderDirectory(theFolder, null, fileList);
            }
            else
            {
                FileUtility.getFileUnderDirectory(theFolder, null, fileList);
            }
            this.listBox2.DataSource = null;
            this.listBox2.DataSource = fileList;

            this.toolStripStatusLabel3.Text = "当前目录的文件总数：" + fileList.Count;

            this.listBox2.SelectedIndex = -1;
            int count = fileList.Count;
            if (count > 0)
            {
                if (listbox2_selectedIndex < count)
                {
                    this.listBox2.SelectedIndex = listbox2_selectedIndex;
                }
                else
                {
                    this.listBox2.SelectedIndex = count - 2;
                }
            }

            int visibleItems = listBox2.ClientSize.Height / listBox2.ItemHeight;
            listBox2.TopIndex = Math.Max(this.listBox2.SelectedIndex - visibleItems / 2, 0);
            
        }

        private void 删除文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("将删除指定的文件，是否确定继续?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.ShowOpaqueLayer(this.listBox2, 125, true);

                    for (int i = 0; i < this.listBox2.SelectedItems.Count; i++)
                    {
                        Item item = this.listBox2.SelectedItems[i] as Item;
                        if (item != null)
                        {
                            FileUtility.deleteFile(item.value);
                        }

                        if (i % 50 == 49)
                        {
                           // loadFileListToListBox2();
                       
                            Application.DoEvents();
                        }
                    }

                    loadFileListToListBox2();

                    this.HideOpaqueLayer();
                }
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            loadFileListToListBox2();

        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(new Point(e.X, e.Y));
            }
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;
            NodeData nodeDate = node.Tag as NodeData;
            System.Diagnostics.Process.Start(nodeDate.path);
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Node.Bounds);
            if (e.State == TreeNodeStates.Selected)//做判断
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0xFF, 0xF6, 0x84)), new Rectangle(e.Node.Bounds.Left, e.Node.Bounds.Top, e.Node.Bounds.Width, e.Node.Bounds.Height));//背景色为蓝色
                e.Graphics.DrawString(e.Node.Text, this.treeView1.Font, Brushes.Black, e.Bounds);
                //字体为白色
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.Items.Count < 1)
                return;

            lb.ItemHeight = 25;
  
            using (Graphics g = e.Graphics)
            {
                if (e.Index != -1)
                {

                    //如果该项被选择
                    Brush myBrush = Brushes.Black;

                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0xFF, 0xF6, 0x84)), e.Bounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(lb.BackColor), e.Bounds);
                    }
                    //画出项文本
                    e.Graphics.DrawString(lb.GetItemText(lb.Items[e.Index]), e.Font, myBrush, e.Bounds.X, e.Bounds.Y);
                    e.DrawFocusRectangle();
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.checkBox3.Checked) return;
            showThumbImage();
        }

        private void showThumbImage()
        {
            Item item = this.listBox1.SelectedItem as Item;
            if (item == null) return;
            Bitmap bmp = null;
            try
            {
                string filepath = item.value;
                //this.textBox1.Text = filepath;
                bmp = new Bitmap(filepath);
                Point ptLoction = new Point(bmp.Size);
                if (ptLoction.X > this.pictureBox1.Size.Width || ptLoction.Y > this.pictureBox1.Size.Height)
                {
                    //图像框的停靠方式   
                    //pcbPic.Dock = DockStyle.Fill;   
                    //图像充滿图像框，並且图像維持比例   
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    //图像在图像框置中   
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                //LoadAsync：非同步转入图像   
                this.pictureBox1.LoadAsync(filepath);
            }
            catch (Exception ex)
            {
                this.pictureBox1.Image = null;
            }
            finally
            {
                if (bmp != null)
                {
                    bmp.Dispose();
                }
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            //设置拖拽类型(这里是复制拖拽)
            Item lvi = (Item)e.Data.GetData(typeof(Item));
            if (lvi != null)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                Cursor = Cursors.No;
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            //获取值
            Item lvi = (Item)e.Data.GetData(typeof(Item));

            //根据鼠标坐标确定要移动到的目标节点
            Point pt;
            TreeNode targetNode;
            pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            targetNode = this.treeView1.GetNodeAt(pt);

            if (targetNode != null)
            {
                NodeData nodeData = targetNode.Tag as NodeData;
                string filePath = nodeData.path;
                FileUtility.createDirectory(filePath);
                foreach (var obj in this.listBox1.SelectedItems)
                {
                    Item item = obj as Item;
                    if (item != null)
                    {
                        FileUtility.CopyFile(item.value, filePath + "\\" + item.text);
                    }
                }

                loadFileListToListBox2();
            }


        }

        private void 删除文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确定删除该文件夹?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;

            }

            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;
            NodeData nodeDate = node.Tag as NodeData;
            DirectoryInfo di = new DirectoryInfo(nodeDate.path);
            if (!di.Exists)
            {
                return;
            }

            FileInfo[] subFiles = di.GetFiles();
            DirectoryInfo[] subDirs = di.GetDirectories();
            if (subFiles == null || subFiles.Length == 0)
            {
                this.treeView1.Nodes.Remove(node);
                di.Delete(true);
            }
            else if (subDirs == null || subDirs.Length == 0)
            {
                this.treeView1.Nodes.Remove(node);
                di.Delete(true);
            }
            else
            {
                if (MessageBox.Show("该文件夹下存在子目录或子文件，是否确定删除?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.treeView1.Nodes.Remove(node);
                    di.Delete(true);

                }
            }
        }

        private void 修改文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode as TreeNode;
            if (node == null)
            {
                MessageBox.Show("请选择要修改的目标文件夹");
                return;
            }

            //if (MessageBox.Show("如果有同名文件将会被覆盖，是否确定继续?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            //{
            //    return;
            //}


            string title = Microsoft.VisualBasic.Interaction.InputBox("输入新文件夹名称", "提示");
            if (title == null || title.Trim().Length == 0)
            {
                MessageBox.Show("请输入文件夹名称");
            }
            else
            {

                NodeData nodeDate = node.Tag as NodeData;
                NodeData parentnodeDate = node.Parent.Tag as NodeData;
                string old_filePath = nodeDate.path;
                string new_filePath = parentnodeDate.path + "\\" + title;
                node.Text = title;
                nodeDate.title = title;
                nodeDate.path = new_filePath;
                FileUtility.RenameDirectory(old_filePath, new_filePath);
            }
        }

        private void 同步_Click(object sender, EventArgs e)
        {
            查询_Click(this.查询, EventArgs.Empty);
            //string sourcePath = this.textBox1.Text;
            //string targetPath = this.textBox3.Text;
            //if (sourcePath.Trim().Length == 0 || targetPath.Trim().Length == 0)
            //{
            //    return;
            //}
            ////FileSystemInfo sourceFSI = new DirectoryInfo(sourcePath);
            //FileSystemInfo targetFSI = new DirectoryInfo(targetPath);
            //if (!targetFSI.Exists)
            //{
            //    return;
            //}

            //List<Item> resultList = compareFileList(targetFSI);
            //sourceFileList = resultList;
            //this.listBox1.DataSource = null;
            //this.listBox1.DataSource = sourceFileList;
        }

        private List<Item> compareFileList(FileSystemInfo targetFSI)
        {
            //List<Item> sourceList = new List<Item>();
            List<Item> targetList = new List<Item>();
            //FileUtility.getAllFileUnderDirectory(sourceFSI, this.textBox2.Text, sourceList);
            FileUtility.getAllFileUnderDirectory(targetFSI, "", targetList);


            List<Item> resultList = sourceFileList.Except(targetList, new ItemComparer()).ToList();
            //this.label4.Text = "当前总文件数：" + sourceFileList.Count;
            this.label5.Text = "当前已归类文件数：" + (sourceFileList.Count - resultList.Count);
            this.label7.Text = "当前未归类文件数：" + resultList.Count;
            return resultList;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fileClassify();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = "选中文件数：" + this.listBox1.SelectedItems.Count;
        }

        private int listbox_selectedIndex = -1;
        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // this.toolStripStatusLabel2.Text = "选中文件数：" + this.listBox1.SelectedItems.Count;
            listbox_selectedIndex = this.listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.toolStripStatusLabel4.Text = "选中文件数：" + this.listBox2.SelectedItems.Count;
            if (this.listBox2.SelectedItems.Count > 0)
            {
                Item item = this.listBox2.SelectedItems[0] as Item;
                if (item != null)
                {
                    this.label6.Text = "选中的文件路径：" + item.value;
                }
            }
        }

        private void 查询条件_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                查询_Click(this.查询, EventArgs.Empty);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.textBox3.Text.Trim() == "") return;

            if (!Directory.Exists(this.textBox3.Text))
            {
                Directory.CreateDirectory(this.textBox3.Text);
            }

            try
            {
                this.treeView1.Nodes.Clear();
                DirectoryInfo theFolder = new DirectoryInfo(this.textBox3.Text);

                TreeNode node = new TreeNode(this.textBox3.Text);
                NodeData data = new NodeData();
                data.title = this.textBox3.Text;
                data.path = this.textBox3.Text;
                node.Tag = data;
                addTreeViewNode(null, node);

                listDir(node, theFolder);
                this.treeView1.ExpandAll();

                this.listBox2.DataSource = null;
                this.label6.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取数据目录树出现异常，请重新选择目录， 异常原因：" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listBox1.Items.Count; i++)
            {
                this.listBox1.SetSelected(i, true);
                if (i % 50 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void 反选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b;
            for (int i = 0; i < this.listBox1.Items.Count; i++)
            {
                b = !this.listBox1.GetSelected(i);
                this.listBox1.SetSelected(i, b);
                if (i % 50 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void 全不选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listBox1.Items.Count; i++)
            {
                this.listBox1.SetSelected(i, false);
                if (i % 50 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < this.listBox1.Items.Count; i++)
                {
                    this.listBox1.SetSelected(i, true);
                    if (i % 50 == 0)
                    {
                        Application.DoEvents();
                    }
                }
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.C)
            {
                Item item = this.listBox1.SelectedItem as Item;
                if (item == null) return;
                Clipboard.SetDataObject(item.text);
                
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < this.listBox2.Items.Count; i++)
                {
                    this.listBox2.SetSelected(i, true);
                    if (i % 50 == 0)
                    {
                        Application.DoEvents();
                    }
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                删除文件ToolStripMenuItem_Click(删除文件ToolStripMenuItem, EventArgs.Empty);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string iniFilePath = path + "config.ini";
            FileUtility.createFile(iniFilePath);
            IniFiles ini = new IniFiles(iniFilePath);
            string sourcePath = ini.ReadString("source", "filepath", "");
            string targetPath = ini.ReadString("target", "filepath", "");
            this.textBox1.Text = sourcePath;
            this.textBox3.Text = targetPath;

            if (this.textBox1.Text.Trim().Length > 0)
            {
                button2_Click(button2, EventArgs.Empty);
            }
            if (this.textBox2.Text.Trim().Length > 0)
            {
                button6_Click(button6, EventArgs.Empty);
            }


            //this.label6.Left = this.panel4.Width - this.label6.Width;
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string iniFilePath = path + "config.ini";
            FileUtility.createFile(iniFilePath);
            IniFiles ini = new IniFiles(iniFilePath);
            ini.WriteString("source", "filepath", this.textBox1.Text);
            ini.WriteString("target", "filepath", this.textBox3.Text);
        }

        private void 清除_Click(object sender, EventArgs e)
        {
            this.查询条件.Text = "";
            this.查询_Click(this.查询, EventArgs.Empty);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            loadFileListToListBox2();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button6_Click(this.button6, EventArgs.Empty);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button2_Click(this.button2, EventArgs.Empty);
            }
        }

        private void 复制文件名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Item item = this.listBox1.SelectedItem as Item;
            if (item == null) return;
            Clipboard.SetDataObject(item.text);
        }

        private void 打开所在的文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Item item = this.listBox1.SelectedItem as Item;
            if (item == null) return;
            int i = item.value.LastIndexOf('\\');
            string path = item.value.Substring(0, i);
            System.Diagnostics.Process.Start(path);
        }

        private void 以其他方式打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Item item = this.listBox1.SelectedItem as Item;
            if (item == null) return;
            System.Diagnostics.Process.Start(item.value);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //int posindex = this.listBox1.IndexFromPoint(e.Location);
            //if (posindex >= 0 && posindex < listBox1.Items.Count)
            //{
            //    listBox1.SelectedIndex = posindex;

            //    Item item = this.listBox1.SelectedItem as Item;
            //    if (item == null) return;
            //    if (imageForm == null)
            //    {
            //        imageForm = new ImageForm();
            //    }

            //    imageForm.setImage(item.value);
            //    imageForm.ShowDialog();
            //}
            Item item = this.listBox1.SelectedItem as Item;
            if (item == null) return;
            System.Diagnostics.Process.Start(item.value);

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.panel11.Visible = this.checkBox3.Checked;
            if (!this.checkBox3.Checked) return;
            showThumbImage();
        }

        private int listbox2_selectedIndex = -1;
        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            listbox2_selectedIndex = this.listBox2.SelectedIndex;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            删除文件ToolStripMenuItem_Click(删除文件ToolStripMenuItem, EventArgs.Empty);
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Item item = this.listBox2.SelectedItem as Item;
            if (item == null) return;
            System.Diagnostics.Process.Start(item.value);
        }

        private void 个月ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;

            for (int i = 0; i < 12; i++ )
            {
                string filename = ""+(i+1) + "月";
                if(hasNode(filename)) continue;

                NodeData parentData = node.Tag as NodeData;
                TreeNode newnode = new TreeNode(filename);
                NodeData data = new NodeData();
                data.title = filename;
                data.path = parentData.path + "\\" + filename;

                newnode.Tag = data;
                addTreeViewNode(node, newnode);
                FileUtility.createDirectory(data.path);
                node.Expand();
            }
        }


        private bool hasNode(string title)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return false;

            foreach (var item in node.Nodes)
            {
                NodeData data = (item as TreeNode).Tag as NodeData;
                if (data.title == title)
                {
                    return true;
                }
            }

            return false;
            
        }

        private void 当月ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;

            string filename = (DateTime.Now.Month + 1)+ "月";
            if (hasNode(filename))
            {
                MessageBox.Show("文件夹[" + filename + "]已经存在");
                return;
            }

            NodeData parentData = node.Tag as NodeData;
            TreeNode newnode = new TreeNode(filename);
            NodeData data = new NodeData();
            data.title = filename;
            data.path = parentData.path + "\\" + filename;

            newnode.Tag = data;
            addTreeViewNode(node, newnode);
            FileUtility.createDirectory(data.path);
            node.Expand();
        }

        private void 按月份新建子文件夹(string filename)
        {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) return;

            if (hasNode(filename))
            {
                MessageBox.Show("文件夹[" + filename + "]已经存在");
                return;
            }

            NodeData parentData = node.Tag as NodeData;
            TreeNode newnode = new TreeNode(filename);
            NodeData data = new NodeData();
            data.title = filename;
            data.path = parentData.path + "\\" + filename;

            newnode.Tag = data;
            addTreeViewNode(node, newnode);
            FileUtility.createDirectory(data.path);
            node.Expand();
        }

        private void 月ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem10_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }

        private void 月ToolStripMenuItem11_Click(object sender, EventArgs e)
        {
            string filename = (sender as ToolStripMenuItem).Text;
            按月份新建子文件夹(filename);
        }


    }
}
