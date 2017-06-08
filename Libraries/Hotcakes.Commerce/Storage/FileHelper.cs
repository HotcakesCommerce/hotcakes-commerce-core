#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.IO;

namespace Hotcakes.Commerce.Storage
{
    internal class FileHelper
    {
        internal static void DeleteDirectoryAndFiles(string sourceFolder)
        {
            foreach (var f in Directory.GetFiles(sourceFolder))
            {
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
            }
            Directory.Delete(sourceFolder);
        }

        internal static void DeleteDirectoryAndFilesRecursive(string sourceFolder)
        {
            foreach (var d in Directory.GetDirectories(sourceFolder))
            {
                DeleteDirectoryAndFilesRecursive(d);
            }

            foreach (var f in Directory.GetFiles(sourceFolder))
            {
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
            }
            Directory.Delete(sourceFolder);
        }

        internal static bool CopyAllFiles(string source, string dest)
        {
            var result = false;

            if (!Directory.Exists(source))
            {
                return false;
            }
            if (!Directory.Exists(dest))
            {
                return false;
            }
            try
            {
                FileCopyNoBackup(source, dest);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        internal static bool CopySingle(string source, string dest, string fileName)
        {
            return CopySingle(source, dest, fileName, false);
        }

        internal static bool CopySingle(string source, string dest, string fileName, bool createPath)
        {
            var result = false;

            if (Directory.Exists(source) == false)
            {
                return false;
            }

            if (Directory.Exists(dest) == false)
            {
                if (createPath)
                {
                    Directory.CreateDirectory(dest);
                }
                else
                {
                    return false;
                }
            }

            try
            {
                var destinationFileName = dest + "\\" + fileName;
                File.Copy(Path.Combine(source, fileName), destinationFileName, true);
                if (File.Exists(destinationFileName))
                {
                    File.SetAttributes(destinationFileName, FileAttributes.Normal);
                }
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        internal static void FileCopyNoBackup(string source, string dest)
        {
            var i = 0;
            string[] subDirs;
            string[] files;
            var destinationFileName = string.Empty;

            files = Directory.GetFiles(source);
            subDirs = Directory.GetDirectories(source);

            // Create dest dir
            if (!CreateAndCheckDirectory(dest))
            {
                throw new ApplicationException(dest + " does not exist and cannot be created.");
            }

            for (i = 0; i < files.Length; i++)
            {
                destinationFileName = dest + "\\" + Path.GetFileName(files[i]);

                File.Copy(files[i], destinationFileName, true);
                if (File.Exists(destinationFileName))
                {
                    File.SetAttributes(destinationFileName, FileAttributes.Normal);
                }
                else
                {
                    throw new ApplicationException("Unable to copy file " + files[i]);
                }
            }

            for (i = 0; i < subDirs.Length; i++)
            {
                FileCopyNoBackup(source + "\\" + Path.GetFileName(subDirs[i]),
                    dest + "\\" + Path.GetFileName(subDirs[i]));
            }
        }

        internal static bool CreateAndCheckDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!Directory.Exists(dir))
            {
                return false;
            }
            return true;
        }

        private static int FileCount(string dirName)
        {
            var iCount = 0;
            if (Directory.Exists(dirName))
            {
                iCount = Directory.GetFiles(dirName).Length;
                if (Directory.GetDirectories(dirName).Length > 0)
                {
                    var subdirs = Directory.GetDirectories(dirName);
                    var i = 0;
                    for (i = 0; i < subdirs.Length; i++)
                    {
                        iCount += FileCount(subdirs[i]);
                    }
                }
            }
            return iCount;
        }

        internal static void ChangeFileAttributesAndRemove(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                foreach (var f in files)
                {
                    File.SetAttributes(f, FileAttributes.Normal);
                    File.Delete(f);
                }

                var dirs = Directory.GetDirectories(path);

                foreach (var d in dirs)
                {
                    ChangeFileAttributesAndRemove(d);
                    Directory.Delete(d);
                }
            }
        }
    }
}