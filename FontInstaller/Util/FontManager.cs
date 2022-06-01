using Microsoft.Win32;
using System;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace FontInstaller.Util
{
    public class FontManager : SingletonBase<FontManager>
    {
        [DllImport("gdi32.dll")]
        private static extern int AddFontResource(string fontFilePath);

        private static string _keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

        public static void RegisterFont(string sourceFontFilePath)
        {
            string targetFontFileName = Path.GetFileName(sourceFontFilePath);
            string targetFontFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), targetFontFileName);

            if (!File.Exists(targetFontFilePath))
            {
                File.Copy(sourceFontFilePath, targetFontFilePath);

                PrivateFontCollection collection = new PrivateFontCollection();

                collection.AddFontFile(targetFontFilePath);

                string actualFontName = collection.Families[0].Name;

                AddFontResource(targetFontFilePath);

                Registry.SetValue(_keyName, actualFontName, targetFontFileName, RegistryValueKind.String);

                Main.Log($"{actualFontName} 설치완료");
            }
        }
    }
}
