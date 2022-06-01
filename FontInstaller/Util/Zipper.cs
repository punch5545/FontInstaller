using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;

namespace FontInstaller.Util
{
    public class Zipper: SingletonBase<Zipper>
    {        
        public static async Task<bool> UnzipFile(string zipPath, string unzipPath)
        {
            await Task.Run(() => {
                try
                {
                    using (ZipFile zip = ZipFile.Read(zipPath))
                    {
                        FileInfo fi = new FileInfo(zipPath);

                        DirectoryInfo dir = new DirectoryInfo(unzipPath);

                        Main.Log(Path.GetFileName(zipPath) + " 압축해제중");

                        if (!dir.Exists)
                        {
                            dir.Create();
                        }

                        string saveFolderPath = $"{unzipPath}\\{Path.GetFileNameWithoutExtension(zipPath)}";

                        for (int i = 0; i < zip.Entries.Count; i++)
                        {
                            ZipEntry entry = zip[i];

                            byte[] byteIbm437 = Encoding.GetEncoding("IBM437").GetBytes(zip[i].FileName);

                            try
                            {
                                string fileName = Encoding.Default.GetString(byteIbm437);
                                zip[i].FileName = fileName;

                                entry.Extract(saveFolderPath.Replace("/", ""));
                            }
                            catch
                            {
                                string fileName = Encoding.UTF8.GetString(byteIbm437);
                                zip[i].FileName = fileName;

                                entry.Extract(saveFolderPath.Replace("/", ""));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Main.Log($"{Path.GetFileName(zipPath)} 파일 압축 해제 실패 : {ex.Message}");
                }
            });

            return true;
        }
    }
}