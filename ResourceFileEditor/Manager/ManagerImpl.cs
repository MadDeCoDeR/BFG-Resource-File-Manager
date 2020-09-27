using ResourceFileEditor.TableOfContent;
using ResourceFileEditor.utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace ResourceFileEditor.Manager
{
    class ManagerImpl : Manager
    {
        private static readonly UInt32 RESOURCE_FILE_MAGIC = 0xD000000D;
        private static readonly Int32 RESOURCE_FILE_HEAD_OFFSET = 12;
        private Form1 form1;

        private List<TableOfContentEntry> contents;

        string resourceFile;

        public ManagerImpl(Form1 form1)
        {
            this.form1 = form1;
        }
        public void loadFile(Stream file)
        {
            using(file)
            {
                try
                {
                    UInt32 magic = FileManager.FileManager.readUint32Swapped(file, 0x0);
                    if (magic == RESOURCE_FILE_MAGIC)
                    {
                        UInt32 tableOffset = FileManager.FileManager.readUint32Swapped(file, 4);
                        UInt32 tableLength = FileManager.FileManager.readUint32Swapped(file, 8);
                        UInt32 numberOfFiles = FileManager.FileManager.readUint32Swapped(file, Convert.ToInt32(tableOffset));
                        Console.WriteLine("Number of Files: " + numberOfFiles);
                        byte[] toc = FileManager.FileManager.readByteArray(file, Convert.ToInt32(tableOffset + 4), Convert.ToInt32(tableLength));
                        contents = new List<TableOfContentEntry>(TableOfContentEntry.parseBytes(toc, Convert.ToInt32(numberOfFiles)));
                        for (int i = 0; i < contents.Count; i++)
                        {
                            NodeUtils.addNode(form1.GetTreeView().Nodes[0].Nodes, PathParser.parsePath(contents[i].Filename));
                           // form1.GetTreeView().Nodes.Add(PathParser.parsePath(tocs[i].Filename));
                            Console.WriteLine(contents[i].Filename);
                        }
                        
                    }
                    resourceFile = ((FileStream)file).Name;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            
        }

        public void writeFile(Stream file)
        {
            using (file)
            {
                try
                {
                    FileManager.FileManager.writeUint32Swapped(file, 0x0, RESOURCE_FILE_MAGIC);
                    FileManager.FileManager.writeUint32Swapped(file, 4, 0);
                    FileManager.FileManager.writeUint32Swapped(file, 8, 0);
                    UInt32 dataOffset = 0;
                    for (int i = 0; i < contents.Count; i++)
                    {
                       
                        byte[] buffer;
                        if (contents[i].file == null)
                        {
                            Stream resourceStream = File.OpenRead(resourceFile);
                            buffer = FileManager.FileManager.readByteArray(resourceStream, (int)contents[i].filePos, (int)contents[i].fileSize);
                        }
                        else
                        {
                            MemoryStream ms = new MemoryStream();
                            contents[i].file.CopyTo(ms);
                            buffer = ms.ToArray();
                        }
                        FileManager.FileManager.writeByteArray(file, (int)(RESOURCE_FILE_HEAD_OFFSET + dataOffset), buffer);
                        contents[i].filePos = (uint)(RESOURCE_FILE_HEAD_OFFSET + dataOffset);
                        dataOffset += contents[i].fileSize;
                    }
                    FileManager.FileManager.writeUint32Swapped(file, 4, (uint)(RESOURCE_FILE_HEAD_OFFSET + dataOffset));
                    FileManager.FileManager.writeUint32Swapped(file, (int)(RESOURCE_FILE_HEAD_OFFSET + dataOffset), (uint)contents.Count);

                    Int32 tableOffset = (int)(RESOURCE_FILE_HEAD_OFFSET + dataOffset + 4);
                    UInt32 tableEntryOffset = 0;
                    for (int i = 0; i < contents.Count; i++)
                    {
                        byte[] buffer = contents[i].parseToBytes();
                        FileManager.FileManager.writeByteArray(file, (int)(tableOffset + tableEntryOffset), buffer);
                        tableEntryOffset += Convert.ToUInt32(buffer.Length);
                    }
                    FileManager.FileManager.writeUint32Swapped(file, 8, tableEntryOffset);
                    resourceFile = ((FileStream)file).Name;
                    file.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public void AddFile(Stream file, string relativePath)
        {
            TableOfContentEntry toc = new TableOfContentEntry();
            toc.Filename = relativePath;
            toc.fileSize = (uint)((FileStream)file).Length;
            toc.file = file;
            contents.Add(toc);
            NodeUtils.addNode(form1.GetTreeView().Nodes[0].Nodes, PathParser.parsePath(relativePath));
        }

        public void CreateFile()
        {
            contents = new List<TableOfContentEntry>();
        }

        public void DeleteEntry(string relativePath)
        {
            string filename = relativePath.Remove(relativePath.Length - 1);

            TableOfContentEntry content = FindContentByPath(relativePath);
            if (content != null)
            {
                contents.Remove(content);
            }
        }

        public void ExtractEntry(string relativePath, string outputFolder)
        {
            TableOfContentEntry content = FindContentByPath(relativePath);
            if (content != null)
            {
                string outputPath = outputFolder + relativePath.Substring(relativePath.LastIndexOf("/"));
                FileStream file = new FileStream(outputPath, FileMode.OpenOrCreate);
                if (content.file != null)
                {
                    content.file.CopyTo(file);
                }
                else
                {
                    Stream resourceStream = File.OpenRead(resourceFile);
                    byte[] buffer = FileManager.FileManager.readByteArray(resourceStream, (int)content.filePos, (int)content.fileSize);
                    file.Write(buffer, 0, buffer.Length);
                }
                file.Close();
            }
        }

        public long GetFileSize(string relativePath)
        {
            TableOfContentEntry content = FindContentByPath(relativePath);
            return content != null ? content.fileSize : -1L;
        }

        private TableOfContentEntry FindContentByPath(string relativePath)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i].Filename == relativePath)
                {
                    return contents[i];
                }
            }
            return null;
        }

        public long GetResourceFileSize()
        {
            long tocSize = 0;
            long fileSize = 0;
            foreach (TableOfContentEntry entry in contents)
            {
                tocSize += entry.GetByteSize();
                fileSize += entry.fileSize;
            }

            return 16 + tocSize + fileSize;
        }
    }
}
