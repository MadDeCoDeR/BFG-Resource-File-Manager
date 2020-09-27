using ResourceFileEditor.Manager;
using ResourceFileEditor.utils;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourceFileEditor
{
    public partial class Form1 : Form
    {
        private ManagerImpl manager;

        private readonly double warningPercent = (((double)1 * 1024 * 1024 * 1024) / UInt32.MaxValue) * 100;
        public Form1()
        {
            InitializeComponent();
            this.manager = new ManagerImpl(this);
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel3.Text = "";
            Boolean hasNodes = treeView1.Nodes.Count > 0;
            saveFileToolStripMenuItem.Enabled = hasNodes;
            entryToolStripMenuItem.Enabled = hasNodes;

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "DOOM 3 BFG Edition resource files (*.resources)|*.resources";
            ofd.Title = "Load File";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = ofd.OpenFile()) != null)
                {
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(new TreeNode("root"));
                    
                    manager.loadFile(myStream);

                    treeView1.Nodes[0].Expand();
                    Boolean hasNodes = treeView1.Nodes.Count > 0;
                    saveFileToolStripMenuItem.Enabled = hasNodes;
                    entryToolStripMenuItem.Enabled = hasNodes;
                    updateToolStripBar(ofd.FileName.Substring(ofd.FileName.LastIndexOf("\\") + 1));
                }
            }
        }

        public TreeView GetTreeView()
        {
            return this.treeView1;
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "DOOM 3 BFG Edition resource files(*.resources)| *.resources";
                sfd.Title = "Save Resource File";
                sfd.ShowDialog();

                if (sfd.FileName != "")
                {
                    FileStream file = File.Create(sfd.FileName);
                    manager.writeFile(file);
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load File";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = ofd.OpenFile()) != null)
                {
                    TreeNode node = treeView1.SelectedNode;
                    if (node == null)
                    {
                        MessageBox.Show("Please select a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    string fullPath = ((FileStream)myStream).Name;
                    string relativePath = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
                    string relativeDirectory = PathParser.NodetoPath(node);
                    if (FileCheck.isFile(relativeDirectory))
                    {
                        MessageBox.Show("You have Selected a file. Please select a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    relativePath = relativeDirectory + relativePath;
                    manager.AddFile(myStream, relativePath);
                    updateToolStripBar(toolStripStatusLabel1.Text);
                }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEntry_logic();
        }

        private void createFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("root");
            manager.CreateFile();
            Boolean hasNodes = treeView1.Nodes.Count > 0;
            saveFileToolStripMenuItem.Enabled = hasNodes;
            entryToolStripMenuItem.Enabled = hasNodes;
            updateToolStripBar("New File");
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right)
            {
                addEntryToolStripMenuItem.Visible = !FileCheck.isFile(treeView1.SelectedNode.Text);
                deleteEntryToolStripMenuItem.Visible = FileCheck.isFile(treeView1.SelectedNode.Text);
                extractEntryToolStripMenuItem.Visible = FileCheck.isFile(treeView1.SelectedNode.Text);
                contextMenuStrip1.Show(treeView1, e.Location);
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEntry_logic();
        }

        private void AddEntry_logic()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Please add Entry name");
            if (name == null || name == "")
            {
                return;
            }
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("Please create a File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TreeNode subNode = new TreeNode(name);
            if (FileCheck.isFile(node.Text))
            {
                MessageBox.Show("You have Selected a file. Please select a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            node.Nodes.Add(subNode);
        }

        private void deleteEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteEntry_logic();
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteEntry_logic();
        }

        private void DeleteEntry_logic()
        {
            TreeNode node = treeView1.SelectedNode;
            string relativePath = PathParser.NodetoPath(node);
            manager.DeleteEntry(relativePath);
            updateToolStripBar(toolStripStatusLabel1.Text);
            node = treeView1.SelectedNode;
            if (node.Parent == null)
            {
                treeView1.Nodes.Remove(node);
            } else
            {
                node.Parent.Nodes.Remove(node);
            }
        }

        private void extractEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractEntry_logic();
        }

        private void ExtractEntry_logic()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("Please select a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string relativePath = PathParser.NodetoPath(node);
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select Folder to extract the file into";
            fbd.ShowDialog();

            if (fbd.SelectedPath != null)
            {
                manager.ExtractEntry(relativePath, fbd.SelectedPath);
            }
        }

        private string ConvertBytesToString(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int repeats = 1;
            long finalSize = bytes / 1024;
            while (finalSize > 1024)
            {
                finalSize = finalSize / 1024;
                repeats++;
            }
            string result = finalSize + sizes[repeats];
            return result;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            addFolderToolStripMenuItem.Visible = !FileCheck.isFile(e.Node.Text);
            addToolStripMenuItem.Visible = !FileCheck.isFile(e.Node.Text);
            deleteEntryToolStripMenuItem1.Visible = FileCheck.isFile(e.Node.Text);
            if (FileCheck.isFile(e.Node.Text))
            {
                string relativePath = PathParser.NodetoPath(e.Node);
                toolStripStatusLabel3.Text = e.Node.Text + " file Size: " + ConvertBytesToString(manager.GetFileSize(relativePath));
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void updateToolStripBar(string fileName)
        {
            toolStripStatusLabel1.Text = fileName;
            long bytes = manager.GetResourceFileSize();
            double covered = ((double)bytes) / UInt32.MaxValue;
            toolStripProgressBar1.Value = (int)(covered * 100);
            if (toolStripProgressBar1.Value >= warningPercent)
            {
                toolStripProgressBar1.ForeColor = Color.Yellow;
            } else
            {
                toolStripProgressBar1.ForeColor = Color.Green;
            }
            toolStripStatusLabel2.Text = ConvertBytesToString(bytes) + "/" + ConvertBytesToString(UInt32.MaxValue);
        }
    }

}
