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
using ResourceFileEditor.Manager.Audio;
using ResourceFileEditor.TableOfContent;
using ResourceFileEditor.utils;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourceFileEditor.Manager
{
    sealed class ManagerImpl : Manager
    {
        private static readonly UInt32 RESOURCE_FILE_MAGIC = 0xD000000D;
        private static readonly Int32 RESOURCE_FILE_HEAD_OFFSET = 12;
        private ManagerUi managerUI;

        private List<TableOfContentEntry>? contents;

        private bool isDirty;
        private bool closeAfterSave;

        string? resourceFile;

        public ManagerImpl(ManagerUi managerUI)
        {
            this.managerUI = managerUI;
        }
        public void loadFile(Stream file)
        {
            using (file)
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
                            if (contents[i].Filename != null)
                            {
                                NodeUtils.addNode(managerUI.GetTreeView().Nodes[0].Nodes, PathParser.parsePath(contents[i].Filename!));
                                // form1.GetTreeView().Nodes.Add(PathParser.parsePath(tocs[i].Filename));
                                Console.WriteLine(contents[i].Filename);
                            }
                        }
                        resourceFile = ((FileStream)file).Name;
                        managerUI.UpdateTitle(GetResourceFileName(), isDirty);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                    Console.WriteLine(ex.StackTrace);
                }
            }

        }

        public void writeFile(Stream file)
        {
            if (resourceFile != null && contents != null)
            {
                using (file)
                {
                    try
                    {
                        string tempres;
                        if (((FileStream)file).Name == resourceFile)
                        {
                            tempres = resourceFile + ".bak";
                        }
                        else
                        {
                            tempres = resourceFile;
                        }
                        FileManager.FileManager.writeUint32Swapped(file, 0x0, RESOURCE_FILE_MAGIC);
                        FileManager.FileManager.writeUint32Swapped(file, 4, 0);
                        FileManager.FileManager.writeUint32Swapped(file, 8, 0);
                        UInt32 dataOffset = 0;
                        for (int i = 0; i < contents.Count; i++)
                        {

                            byte[] buffer;
                            if (contents[i].file == null)
                            {
                                Stream resourceStream = File.OpenRead(tempres);
                                buffer = FileManager.FileManager.readByteArray(resourceStream, (int)contents[i].filePos, (int)contents[i].fileSize);
                                resourceStream.Close();
                            }
                            else
                            {
                                MemoryStream ms = new MemoryStream();
                                contents[i].file!.Seek(0, SeekOrigin.Begin);
                                contents[i].file!.CopyTo(ms);
                                buffer = ms.ToArray();
                                if (buffer.Length == 0)
                                {
                                    throw new EntryPointNotFoundException("Empty stream");
                                }
                                contents[i].file!.Close();
                                contents[i].file = null;
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
                        if (((FileStream)file).Name == resourceFile)
                        {

                            File.Delete(tempres);
                        }
                        resourceFile = ((FileStream)file).Name;
                        file.Close();
                        isDirty = false;
                        managerUI.UpdateTitle(this.GetResourceFileName(), isDirty);
                        if (closeAfterSave)
                        {
                            CloseFile();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        public void AddFile(Stream file, string relativePath)
        {
            TableOfContentEntry? oldToc = FindContentByPath(relativePath);
            if (oldToc != null)
            {
                this.DeleteEntry(relativePath);
            }
            TableOfContentEntry toc = new TableOfContentEntry();
            toc.Filename = relativePath;
            toc.fileSize = (uint)((FileStream)file).Length;
            toc.file = file;
            if (contents != null)
            {
                contents.Add(toc);
            }
            isDirty = true;
            managerUI.UpdateTitle(this.GetResourceFileName(), isDirty);
            NodeUtils.addNode(managerUI.GetTreeView().Nodes[0].Nodes, PathParser.parsePath(relativePath));
        }

        public void CreateFile()
        {
            contents = new List<TableOfContentEntry>();
            isDirty = true;
            resourceFile = "New File";
            managerUI.UpdateTitle(this.GetResourceFileName(), isDirty);
        }

        public void DeleteEntry(string relativePath)
        {
            string filename = relativePath.Remove(relativePath.Length - 1);

            TableOfContentEntry? content = FindContentByPath(relativePath);
            if (content != null)
            {
                if (contents != null)
                {
                    contents.Remove(content);
                }
                isDirty = true;
                managerUI.UpdateTitle(this.GetResourceFileName(), isDirty);
            }
        }

        public async Task ExtractFolder(string relativePath, string outputFolder)
        {
            while (managerUI.extractProgressBar == null || !managerUI.extractProgressBar.IsHandleCreated)
            {
                Thread.Sleep(5);
            }
            List<string> children = GetFilesFromPath(relativePath);
            if (children.Count > 0)
            {
                while (managerUI.extractProgressBar == null || !managerUI.extractProgressBar.IsHandleCreated)
                {
                    Thread.Sleep(5);
                }
                managerUI.extractProgressBar.Invoke(new Action(() => managerUI.extractProgressBar.Maximum = children.Count));
                foreach (string child in children)
                {
                    if (child.Contains('/'))
                    {
                        string outputPath = Path.Combine(outputFolder, child.Substring(0, child.LastIndexOf('/')));
                        Directory.CreateDirectory(outputPath);
                        managerUI.extractProgressLabel!.Invoke(new Action(() => managerUI.extractProgressLabel.Text = child));
                        await Task.Run(() => ExtractEntry(child, outputPath));
                    }
                    else
                    {
                        managerUI.extractProgressLabel!.Invoke(new Action(() => managerUI.extractProgressLabel.Text = child));
                        await Task.Run(() => ExtractEntry(child, outputFolder));
                    }
                    if (managerUI.extractProgressBar == null)
                        break;
                    managerUI.extractProgressBar.Invoke(new Action(() => managerUI.extractProgressBar.Increment(1)));
                }

                managerUI.extractProgressLabel!.Invoke(new Action(() => managerUI.extractProgressLabel.Text = "Completed"));
            }
        }

        public void ExtractEntry(string relativePath, string outputFolder)
        {
            TableOfContentEntry? content = FindContentByPath(relativePath);
            if (content != null)
            {
                string TempFile = relativePath.Substring(relativePath.LastIndexOf('/') + 1);
                string outputPath = outputFolder + "/" + TempFile;
                FileStream file = new FileStream(outputPath, FileMode.OpenOrCreate);
                if (content.file != null)
                {
                    content.file.CopyTo(file);
                }
                else
                {
                    if (resourceFile != null)
                    {
                        Stream resourceStream = File.OpenRead(resourceFile);
                        byte[] buffer = FileManager.FileManager.readByteArray(resourceStream, (int)content.filePos, (int)content.fileSize);
                        file.Write(buffer, 0, buffer.Length);
                        resourceStream.Close();
                    }
                }
                file.Close();
            }
        }

        public async Task ExtractAndExportFolder(string relativePath, string outputFolder)
        {
            while (managerUI.extractProgressBar == null || !managerUI.extractProgressBar.IsHandleCreated)
            {
                Thread.Sleep(5);
            }
            List<string> children = GetFilesFromPath(relativePath);
            if (children.Count > 0)
            {
                managerUI.extractProgressBar.Invoke(new Action(() => managerUI.extractProgressBar.Maximum = children.Count));
                foreach (string child in children)
                {
                    if (child.Contains('/'))
                    {
                        string outputPath = Path.Combine(outputFolder, child.Substring(0, child.LastIndexOf('/')));
                        Directory.CreateDirectory(outputPath);
                        managerUI.extractProgressLabel!.Invoke(new Action(() =>managerUI.extractProgressLabel.Text = child));
                        await Task.Run(() => ExportEntry(child, outputPath));
                    }
                    else
                    {
                        managerUI.extractProgressLabel!.Invoke(new Action(() => managerUI.extractProgressLabel.Text = child));
                        await Task.Run(() => ExportEntry(child, outputFolder));
                    }
                    if (managerUI.extractProgressBar == null)
                        break;
                    managerUI.extractProgressBar.Invoke(new Action(() => managerUI.extractProgressBar.Increment(1)));
                }

                managerUI.extractProgressLabel!.Invoke(new Action(() => managerUI.extractProgressLabel.Text = "Completed"));
            }
        }

        public void ExportEntry(string relativePath, string outputFolder)
        {
            TableOfContentEntry? content = FindContentByPath(relativePath);
            if (content != null)
            {
                string TempFile = relativePath.Substring(relativePath.LastIndexOf('/') + 1);
                string outputPath = outputFolder + "/" + TempFile;
                string fileExtension = outputPath.Substring(outputPath.LastIndexOf('.') + 1);
                switch (fileExtension)
                {
                    case "idwav":
                        outputPath = outputPath.Replace("idwav", "wav");
                        break;
                    case "bimage":
                        outputPath = outputPath.Replace("bimage", "tga");
                        break;
                }
                FileStream file = new FileStream(outputPath, FileMode.OpenOrCreate);
                if (content.file != null)
                {
                    content.file.CopyTo(file);
                }
                else
                {
                    if (resourceFile != null)
                    {
                        Stream resourceStream = File.OpenRead(resourceFile);
                        byte[] buffer = FileManager.FileManager.readByteArray(resourceStream, (int)content.filePos, (int)content.fileSize);

                        file.Write(buffer, 0, buffer.Length);
                        resourceStream.Close();
                    }
                }
                Stream exportedFile = new MemoryStream();
                switch (fileExtension)
                {
                    case "idwav":
                        exportedFile = AudioManager.LoadFile(file);
                        file.Position = 0;
                        exportedFile.CopyTo(file);

                        break;
                    case "bimage":
                        exportedFile = Image.ImageManager.LoadImage(file)!;
                        //Second Pass in order to make a valid tga
                        ImageResult imageResult = ImageResult.FromStream(exportedFile, ColorComponents.RedGreenBlueAlpha);
                        byte[] data = imageResult.Data;
                        file.Position = 0;
                        StbImageWriteSharp.ImageWriter writer = new StbImageWriteSharp.ImageWriter();
                        writer.WriteTga(data, imageResult.Width, imageResult.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, file);
                        break;
                    default:
                        if (resourceFile != null)
                        {
                            Stream resourceStream = File.OpenRead(resourceFile);
                            byte[] buffer = FileManager.FileManager.readByteArray(resourceStream, (int)content.filePos, (int)content.fileSize);
                            file.Write(buffer, 0, buffer.Length);
                        }
                        break;
                }
                exportedFile.Close();
                file.Close();

            }
        }

        public long GetFileSize(string relativePath)
        {
            TableOfContentEntry? content = FindContentByPath(relativePath);
            return content != null ? content.fileSize : -1L;
        }

        private TableOfContentEntry? FindContentByPath(string relativePath)
        {
            if (contents != null)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    if (contents[i].Filename == relativePath)
                    {
                        return contents[i];
                    }
                }
            }
            return null;
        }
        private List<string> GetFilesFromPath(string relativePath)
        {
            List<string> innerEntries = new List<string>();
            if (contents != null)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    if (contents[i].Filename != null && contents[i].Filename!.StartsWith(relativePath, StringComparison.InvariantCulture))
                    {
                        innerEntries.Add(contents[i].Filename!);
                    }
                }
            }
            return innerEntries;
        }

        public long GetResourceFileSize()
        {
            long tocSize = 0;
            long fileSize = 0;
            if (contents != null)
            {
                foreach (TableOfContentEntry entry in contents)
                {
                    tocSize += entry.GetByteSize();
                    fileSize += entry.fileSize;
                }
            }

            return 16 + tocSize + fileSize;
        }

        public void CloseFile()
        {
            if (isDirty)
            {
                DialogResult result = MessageBox.Show("File has not been saved, do you want to save it?", "Warning", MessageBoxButtons.YesNo);
                switch (result)
                {
                    case DialogResult.Yes:
                        closeAfterSave = true;
                        managerUI.executeSave();
                        break;
                    case DialogResult.No:
                        doClose();
                        return;
                }
            }
            else
            {
                doClose();
            }
        }

        private void doClose()
        {
            isDirty = false;
            managerUI.clearEditor();
            managerUI.UpdateTitle(null, isDirty);
            resourceFile = null;
            contents = null;
            closeAfterSave = false;
        }

        public Stream? loadEntry(string relativePath)
        {
            TableOfContentEntry? content = FindContentByPath(relativePath);
            if (content != null)
            {
                if (content.file != null)
                {
                    return new MemoryStream(FileManager.FileManager.readByteArray(content.file, (int)0, (int)content.fileSize));
                }
                else
                {
                    if (resourceFile != null)
                    {
                        Stream resourceStream = File.OpenRead(resourceFile);
                        byte[] buffer = FileManager.FileManager.readByteArray(resourceStream, (int)content.filePos, (int)content.fileSize);
                        resourceStream.Close();
                        return new MemoryStream(buffer);
                    }
                }
            }
            return null;
        }

        public void updateEntry(string relativePath, Stream data)
        {
            TableOfContentEntry? content = FindContentByPath(relativePath);
            if (content != null)
            {
                content.filePos = 0;
                content.fileSize = (uint)data.Length;
                content.file = data;
                isDirty = true;
                managerUI.UpdateTitle(this.GetResourceFileName(), isDirty);
            }
        }

        public string? GetResourceFileName()
        {
            if (resourceFile != null)
            {
                int lastIndex = resourceFile.LastIndexOf(FileCheck.getPathSeparator(), StringComparison.InvariantCulture);
                return resourceFile.Substring(lastIndex + 1);
            }
            return null;
        }

        public string? GetResourceFullPath()
        {
            return resourceFile;
        }
    }
}
