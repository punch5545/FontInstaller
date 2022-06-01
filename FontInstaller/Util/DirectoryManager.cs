using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontInstaller.Util
{
    public class DirectoryManager : SingletonBase<DirectoryManager>
    {
        public List<FileInfo> GetChildItems(string path, params string[] extensions)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            string ext = "";

            foreach(var extension in extensions)
            {
                ext += $"*.{extension}|";
            }

            ext = ext.TrimEnd('|');

            Console.WriteLine(ext);

            return info.GetFiles("*.*").Where(s=>extensions.Any(e=>s.Name.ToLower().EndsWith(e))).ToList();
        }

        public List<DirectoryInfo> GetAllChildDirectories(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);

            List<DirectoryInfo> infos = new List<DirectoryInfo>();

            infos.Add(info);

            var dirs = info.GetDirectories();
            foreach (var dir in dirs)
            {
                if (dir.GetDirectories().Length > 0)
                {
                    var i = GetAllChildDirectories(dir.FullName);
                    infos.AddRange(i);
                }

                infos.Add(dir);
            }
            return infos;
        }

        public void RemoveAllEmptyChilds(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);

            var dirs = info.GetDirectories();
            foreach (var dir in dirs)
            {
                if (dir.GetDirectories().Length > 0)
                    RemoveAllEmptyChilds(dir.FullName);
                else
                {
                    dir.Delete(true);
                }
            }
        }

        public void MoveAllFiles(string targetDir, string destDir, params string[] extensions)
        {
            var dirs = GetAllChildDirectories(targetDir);
            var files = new List<FileInfo>();

            foreach (var dir in dirs)
            {
                var filelist = GetChildItems(dir.FullName, extensions);
                files.AddRange(filelist);
            }
            foreach (var file in files)
            {
                try
                {
                    if(file.Length > 2048)
                        file.MoveTo(destDir + "\\" + file.Name);

                    if (file.Directory.GetFiles().Length < 1)
                        file.Directory.Delete();
                }
                catch { }
            }
        }

        public void CopyAllFiles(string targetDir, string destDir, params string[] extensions)
        {
            var dirs = GetAllChildDirectories(targetDir);
            var files = new List<FileInfo>();

            // 모든 파일 검색
            foreach (var dir in dirs)
            {
                var filelist = GetChildItems(dir.FullName, extensions);
                files.AddRange(filelist);
            }
            // 임시경로로 복사
            foreach (var file in files)
            {
                try
                {
                    if (file.Length > 2048)
                        file.CopyTo(destDir + "\\" + file.Name);
                }
                catch { }
            }
        }
    }
}
