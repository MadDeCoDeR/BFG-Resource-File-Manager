using System.Windows.Forms;

namespace ResourceFileEditor.utils
{
    class NodeUtils
    {
        public static void addNode(TreeNodeCollection root, TreeNode child)
        {
            TreeNode result = FindByName(root, child.Text);
            if (result != null)
            {
                populateParent(result, child.Nodes);
                //Console.WriteLine(child.Text);
            }
            else
            {
                root.Add(child);
            }
        }

        public static void populateParent(TreeNode parent, TreeNodeCollection newChilds)
        {
            foreach (TreeNode subChild in newChilds)
            {
                TreeNode subKid = FindByName(parent.Nodes, subChild.Text);
                if (subKid == null)
                {
                    parent.Nodes.Add(subChild);
                }
                else
                {
                    populateParent(subKid, subChild.Nodes);
                }
            }
        }

        public static TreeNode FindByName(TreeNodeCollection root, string key)
        {
            foreach (TreeNode node in root)
            {
                if (node.Text == key)
                {
                    return node;
                }
            }
            return null;
        }
    }
}
