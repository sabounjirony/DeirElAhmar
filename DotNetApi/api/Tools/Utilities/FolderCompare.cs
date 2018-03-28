using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Tools.Utilities
{
    public class FolderCompare : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name);
        }
        public int GetHashCode(FileInfo fi)
        {
            var s = fi.Name;
            return s.GetHashCode();
        }
        public bool Compare(string sourcePath, string destinationPath, bool update = false)
        {
            var sourceDirectory = new DirectoryInfo(sourcePath);
            var destinationDirectory = new DirectoryInfo(destinationPath);

            //Loop directories
            IEnumerable<DirectoryInfo> subDir1 = sourceDirectory.GetDirectories("*.*",
                                                                                SearchOption.TopDirectoryOnly);

            IEnumerable<DirectoryInfo> subDir2 = destinationDirectory.GetDirectories("*.*",
                                                                                     SearchOption.TopDirectoryOnly);

            foreach (var d1 in subDir1)
            {
                var isInDestination = subDir2.Any(d2 => d1.Name == d2.Name);

                if (!isInDestination && update)
                {
                    Directory.CreateDirectory(Path.Combine(destinationPath, d1.Name));
                }

                if (!isInDestination && !update) return false;

                //Deep compare
                var subResult = Compare(d1.FullName, Path.Combine(destinationPath, d1.Name), update);
                if (!subResult && !update) return false;
            }


            IEnumerable<FileInfo> list1 = sourceDirectory.GetFiles("*.*",
                                                                   SearchOption.TopDirectoryOnly);

            IEnumerable<FileInfo> list2 = destinationDirectory.GetFiles("*.*",
                                                                        SearchOption.TopDirectoryOnly);

            foreach (var s in list1.Where(s => s.FullName.ToUpper().Contains("UPDATER") == false))
            {
                var isInDestination = false;

                foreach (var s2 in list2.Where(s2 => s2.FullName.ToUpper().Contains("UPDATER") == false))
                {
                    if (s.Name == s2.Name)
                    {
                        if (s.LastWriteTime == s2.LastWriteTime)
                        {
                            isInDestination = true;
                            break;
                        }
                    }
                }

                if (!isInDestination && update)
                {
                    File.Copy(s.FullName, Path.Combine(destinationPath, s.Name), true);
                }
                if (!isInDestination && !update) return false;
            }
            return true;
        }
    }
}