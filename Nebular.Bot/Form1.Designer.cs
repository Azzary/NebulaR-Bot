using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nebular.Bot
{
    partial class MainUI
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                _globalHook?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.inputScriptName = new System.Windows.Forms.TextBox();
            this.DofusWindowPanel = new System.Windows.Forms.Panel();
            this.AccountTab = new System.Windows.Forms.TabControl();
            this.ClientTab = new System.Windows.Forms.TabPage();
            this.btnLoadIA = new System.Windows.Forms.Button();
            this.buttonRunScript = new System.Windows.Forms.Button();
            this.checkBoxHarverstScript = new System.Windows.Forms.CheckBox();
            this.checkBoxFightScript = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxEatClient = new System.Windows.Forms.CheckBox();
            this.Ypos = new System.Windows.Forms.TextBox();
            this.Xpos = new System.Windows.Forms.TextBox();
            this.btnScriptCreator = new System.Windows.Forms.Button();
            this.MapTab = new System.Windows.Forms.TabPage();
            this.InventaireTab = new System.Windows.Forms.TabPage();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.AccountTab.SuspendLayout();
            this.ClientTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 25);
            this.button2.TabIndex = 1;
            this.button2.Text = "Play";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // inputScriptName
            // 
            this.inputScriptName.Location = new System.Drawing.Point(104, 18);
            this.inputScriptName.Name = "inputScriptName";
            this.inputScriptName.Size = new System.Drawing.Size(100, 20);
            this.inputScriptName.TabIndex = 3;
            // 
            // DofusWindowPanel
            // 
            this.DofusWindowPanel.Location = new System.Drawing.Point(6, 53);
            this.DofusWindowPanel.Name = "DofusWindowPanel";
            this.DofusWindowPanel.Size = new System.Drawing.Size(749, 595);
            this.DofusWindowPanel.TabIndex = 5;
            // 
            // AccountTab
            // 
            this.AccountTab.Controls.Add(this.ClientTab);
            this.AccountTab.Controls.Add(this.MapTab);
            this.AccountTab.Controls.Add(this.InventaireTab);
            this.AccountTab.Location = new System.Drawing.Point(1, 0);
            this.AccountTab.Name = "AccountTab";
            this.AccountTab.SelectedIndex = 0;
            this.AccountTab.Size = new System.Drawing.Size(847, 702);
            this.AccountTab.TabIndex = 8;
            // 
            // ClientTab
            // 
            this.ClientTab.Controls.Add(this.btnLoadIA);
            this.ClientTab.Controls.Add(this.buttonRunScript);
            this.ClientTab.Controls.Add(this.checkBoxHarverstScript);
            this.ClientTab.Controls.Add(this.checkBoxFightScript);
            this.ClientTab.Controls.Add(this.label1);
            this.ClientTab.Controls.Add(this.checkBoxEatClient);
            this.ClientTab.Controls.Add(this.DofusWindowPanel);
            this.ClientTab.Controls.Add(this.Ypos);
            this.ClientTab.Controls.Add(this.button2);
            this.ClientTab.Controls.Add(this.Xpos);
            this.ClientTab.Controls.Add(this.btnScriptCreator);
            this.ClientTab.Controls.Add(this.inputScriptName);
            this.ClientTab.Location = new System.Drawing.Point(4, 22);
            this.ClientTab.Name = "ClientTab";
            this.ClientTab.Padding = new System.Windows.Forms.Padding(3);
            this.ClientTab.Size = new System.Drawing.Size(839, 676);
            this.ClientTab.TabIndex = 0;
            this.ClientTab.Text = "Client";
            this.ClientTab.UseVisualStyleBackColor = true;
            // 
            // btnLoadIA
            // 
            this.btnLoadIA.Location = new System.Drawing.Point(758, 82);
            this.btnLoadIA.Name = "btnLoadIA";
            this.btnLoadIA.Size = new System.Drawing.Size(75, 23);
            this.btnLoadIA.TabIndex = 13;
            this.btnLoadIA.Text = "Load IA";
            this.btnLoadIA.UseVisualStyleBackColor = true;
            this.btnLoadIA.Click += new System.EventHandler(this.btnLoadIA_Click);
            // 
            // buttonRunScript
            // 
            this.buttonRunScript.Location = new System.Drawing.Point(758, 53);
            this.buttonRunScript.Name = "buttonRunScript";
            this.buttonRunScript.Size = new System.Drawing.Size(75, 23);
            this.buttonRunScript.TabIndex = 0;
            this.buttonRunScript.Text = "Run Script";
            this.buttonRunScript.UseVisualStyleBackColor = true;
            this.buttonRunScript.Click += new System.EventHandler(this.buttonRunScript_Click);
            // 
            // checkBoxHarverstScript
            // 
            this.checkBoxHarverstScript.AutoSize = true;
            this.checkBoxHarverstScript.Location = new System.Drawing.Point(210, 27);
            this.checkBoxHarverstScript.Name = "checkBoxHarverstScript";
            this.checkBoxHarverstScript.Size = new System.Drawing.Size(75, 17);
            this.checkBoxHarverstScript.TabIndex = 12;
            this.checkBoxHarverstScript.Text = "Harverst ?";
            this.checkBoxHarverstScript.UseVisualStyleBackColor = true;
            this.checkBoxHarverstScript.CheckedChanged += new System.EventHandler(this.checkBoxHarverstScript_CheckedChanged);
            // 
            // checkBoxFightScript
            // 
            this.checkBoxFightScript.AutoSize = true;
            this.checkBoxFightScript.Location = new System.Drawing.Point(211, 7);
            this.checkBoxFightScript.Name = "checkBoxFightScript";
            this.checkBoxFightScript.Size = new System.Drawing.Size(58, 17);
            this.checkBoxFightScript.TabIndex = 11;
            this.checkBoxFightScript.Text = "Fight ?";
            this.checkBoxFightScript.UseVisualStyleBackColor = true;
            this.checkBoxFightScript.CheckedChanged += new System.EventHandler(this.checkBoxFightScript_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Script Name";
            // 
            // checkBoxEatClient
            // 
            this.checkBoxEatClient.AutoSize = true;
            this.checkBoxEatClient.Checked = true;
            this.checkBoxEatClient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEatClient.Location = new System.Drawing.Point(421, 18);
            this.checkBoxEatClient.Name = "checkBoxEatClient";
            this.checkBoxEatClient.Size = new System.Drawing.Size(68, 17);
            this.checkBoxEatClient.TabIndex = 8;
            this.checkBoxEatClient.Text = "EatClient";
            this.checkBoxEatClient.UseVisualStyleBackColor = true;
            // 
            // Ypos
            // 
            this.Ypos.Location = new System.Drawing.Point(551, 27);
            this.Ypos.Name = "Ypos";
            this.Ypos.ReadOnly = true;
            this.Ypos.Size = new System.Drawing.Size(100, 20);
            this.Ypos.TabIndex = 7;
            // 
            // Xpos
            // 
            this.Xpos.Location = new System.Drawing.Point(551, 1);
            this.Xpos.Name = "Xpos";
            this.Xpos.ReadOnly = true;
            this.Xpos.Size = new System.Drawing.Size(100, 20);
            this.Xpos.TabIndex = 6;
            // 
            // btnScriptCreator
            // 
            this.btnScriptCreator.Location = new System.Drawing.Point(297, 14);
            this.btnScriptCreator.Name = "btnScriptCreator";
            this.btnScriptCreator.Size = new System.Drawing.Size(75, 23);
            this.btnScriptCreator.TabIndex = 4;
            this.btnScriptCreator.Text = "Start Creator";
            this.btnScriptCreator.UseVisualStyleBackColor = true;
            this.btnScriptCreator.Click += new System.EventHandler(this.button3_Click);
            // 
            // MapTab
            // 
            this.MapTab.Location = new System.Drawing.Point(4, 22);
            this.MapTab.Name = "MapTab";
            this.MapTab.Padding = new System.Windows.Forms.Padding(3);
            this.MapTab.Size = new System.Drawing.Size(839, 676);
            this.MapTab.TabIndex = 1;
            this.MapTab.Text = "Map";
            this.MapTab.UseVisualStyleBackColor = true;
            // 
            // InventaireTab
            // 
            this.InventaireTab.Location = new System.Drawing.Point(4, 22);
            this.InventaireTab.Name = "InventaireTab";
            this.InventaireTab.Padding = new System.Windows.Forms.Padding(3);
            this.InventaireTab.Size = new System.Drawing.Size(839, 676);
            this.InventaireTab.TabIndex = 2;
            this.InventaireTab.Text = "Inventaire";
            this.InventaireTab.UseVisualStyleBackColor = true;
            // 
            // MainUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(847, 701);
            this.Controls.Add(this.AccountTab);
            this.Name = "MainUI";
            this.Text = "NebularBot";
            this.AccountTab.ResumeLayout(false);
            this.ClientTab.ResumeLayout(false);
            this.ClientTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private TextBox inputScriptName;
        private Panel DofusWindowPanel;
        private TextBox Xpos;
        private TextBox Ypos;
        private TabControl AccountTab;
        private TabPage ClientTab;
        private TabPage MapTab;
        private CheckBox checkBoxEatClient;
        private Button buttonRunScript;
        private Button btnScriptCreator;
        private CheckBox checkBoxHarverstScript;
        private CheckBox checkBoxFightScript;
        private Label label1;
        private Button btnLoadIA;
        private TabPage InventaireTab;
        private PageSetupDialog pageSetupDialog1;
    }
}

