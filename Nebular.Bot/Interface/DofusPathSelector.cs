using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Interface
{
    [Obfuscation(Exclude = true)]
    internal class DofusPathSelector : Form
    {
        private Button btnSelectionDofusPath;

        public DofusPathSelector()
        {
            InitializeComponent();
        }

        private void buttonOpenFileDialog_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ankama\\Retro");
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = "exe files (*.exe)|*.exe";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                File.WriteAllText("PathDofus.txt", selectedFilePath);
            }
            this.Close();
        }

        private void InitializeComponent()
        {
            this.btnSelectionDofusPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectionDofusPath
            // 
            this.btnSelectionDofusPath.Location = new System.Drawing.Point(58, 34);
            this.btnSelectionDofusPath.Name = "btnSelectionDofusPath";
            this.btnSelectionDofusPath.Size = new System.Drawing.Size(167, 34);
            this.btnSelectionDofusPath.TabIndex = 0;
            this.btnSelectionDofusPath.Text = "Choisir un chemain vers Dofus Retro";
            this.btnSelectionDofusPath.UseVisualStyleBackColor = true;
            this.btnSelectionDofusPath.Click += new System.EventHandler(this.buttonOpenFileDialog_Click);
            // 
            // DofusPathSelector
            // 
            this.ClientSize = new System.Drawing.Size(285, 100);
            this.Controls.Add(this.btnSelectionDofusPath);
            this.Name = "DofusPathSelector";
            this.ResumeLayout(false);

        }
    }
}
