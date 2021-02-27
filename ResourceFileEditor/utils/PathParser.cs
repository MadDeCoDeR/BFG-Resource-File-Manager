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
