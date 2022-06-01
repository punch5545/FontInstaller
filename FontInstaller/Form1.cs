using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using FontInstaller.Util;
using System.IO;
using System.Diagnostics;
using System.Drawing.Text;

namespace FontInstaller
{
    public partial class Main : Form
    {
        public static Main main;
        List<KeyValuePair<string, FileInfo>> fontList = new List<KeyValuePair<string, FileInfo>>();
        public Main()
        {
            InitializeComponent();

            main = this;
        }

        // 폴더선택
        private void button1_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }


        private async void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;

            var tmpDir = new DirectoryInfo(textBox1.Text).Parent.CreateSubdirectory("tmp");

            List<string> exts = new List<string>();
            exts.Add("zip");
            exts.Add("ttf");
            if(checkBox1.Checked) exts.Add("otf");

            DirectoryManager.Instance.CopyAllFiles(textBox1.Text, tmpDir.FullName, exts.ToArray());

            var tmpFiles = DirectoryManager.Instance.GetChildItems(tmpDir.FullName, "zip");

            while (tmpFiles.Count != 0)
            {
                foreach (var tmpFile in tmpFiles)
                {
                    bool unzip = await Zipper.UnzipFile(tmpFile.FullName, tmpDir.FullName);
                    if (unzip)
                        tmpFile.Delete();

                }
                DirectoryManager.Instance.MoveAllFiles(tmpDir.FullName, tmpDir.FullName, exts.ToArray());

                tmpFiles = DirectoryManager.Instance.GetChildItems(tmpDir.FullName, "zip");
            }

            Log("압축해제 완료");

            DirectoryManager.Instance.RemoveAllEmptyChilds(tmpDir.FullName);

            exts.Remove("zip");

            var fonts = DirectoryManager.Instance.GetChildItems(tmpDir.FullName, exts.ToArray());

            foreach(var font in fonts)
            {
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(font.FullName);

                fontList.Add(new KeyValuePair<string, FileInfo>(fontCol.Families[0].Name, font));
            }

            dataGridView1.DataSource = fontList;
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[1].Width = 300;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var style = new DataGridViewCellStyle();
                PrivateFontCollection privateFonts = new PrivateFontCollection();
                privateFonts.AddFontFile(((FileInfo)row.Cells[1].Value).FullName);
                Font font = new Font(privateFonts.Families[0], 12f);
                style.Font = font;
                row.Cells[0].Style = style;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var tmpDir = new DirectoryInfo(textBox1.Text + "\\..\\tmp");

            if(tmpDir.Exists)
                tmpDir.Delete(true);
        }

        public static void Log(string Message)
        {
            main.Invoke(new Action(() =>
            {
                try
                {
                    main.textBox2.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {Message}\r\n");
                    Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {Message}\r\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }));
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(var item in fontList)
            {
                FontManager.RegisterFont(item.Value.FullName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                FontManager.RegisterFont(fontList[row.Index].Value.FullName);
            }
        }
    }
}
