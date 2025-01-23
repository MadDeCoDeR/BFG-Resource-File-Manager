/*
===========================================================================

BFG Resource File Manager GPL Source Code
Copyright (C) 2021 George Kalampokis

This file is part of the BFG Resource File Manager GPL Source Code ("BFG Resource File Manager Source Code").

BFG Resource File Manager Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

BFG Resource File Manager Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with BFG Resource File Manager Source Code.  If not, see <http://www.gnu.org/licenses/>.

===========================================================================
*/
using System.Windows.Forms;

namespace ResourceFileEditor.utils
{
    sealed class NodeUtils
    {
        public static void addNode(TreeNodeCollection root, TreeNode child)
        {
            TreeNode? result = FindByName(root, child.Text);
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
                TreeNode? subKid = FindByName(parent.Nodes, subChild.Text);
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

        public static TreeNode? FindByName(TreeNodeCollection root, string key)
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
