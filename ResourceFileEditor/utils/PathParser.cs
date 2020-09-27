using System.Windows.Forms;

namespace ResourceFileEditor.utils
{
    class PathParser
    {
        public static TreeNode parsePath(string Path)
        {
            

            string[] names = Path.Split('/');

            TreeNode rootnode = new TreeNode(names[0]);

            TreeNode node = rootnode;

            for (int i = 1; i < names.Length; i++)
            {
                TreeNode childNode = new TreeNode(names[i]);
                node.Nodes.Add(childNode);
                node = childNode;

            }

            return rootnode;
        }

        public static string NodetoPath(TreeNode node)
        {
            string relativePath = "";
            do
            {
                string name = node.Text;
                if (FileCheck.isFile(name))
                {
                    relativePath = name;
                }
                else
                {
                    relativePath = name + "/" + relativePath;
                }
                if (node.Parent == null)
                {
                    break;
                }
                node = node.Parent;
            } while (true);
            if (relativePath.StartsWith("root"))
            {
                relativePath = relativePath.Substring(5);
            }
            return relativePath;
        }
    }
}
