using Gma.System.MouseKeyHook;
using Nebular.Bot.Game.Entity;
using Nebular.Bot.Interface;
using Nebular.Bot.Interface.InventaireTab;
using Nebular.Bot.Scripting;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Nebular.Bot
{
    [Obfuscation(Exclude = true)]
    public partial class MainUI : Form
    {
        private static MainUI _instance;
        private System.Windows.Forms.Timer timer;
        private IKeyboardMouseEvents _globalHook;
        public static MapControl mapControl { get; set; }
        public static Dofus client { get; private set; }
        public InventaireControl InventaireControl { get; private set; }
        // Private constructor to prevent external instantiation
        public MainUI()
        {
            InitializeComponent();
            this.buttonRunScript.BackColor = Color.Red;
            this.btnLoadIA.BackColor = Color.Red;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
            _instance = this;
            mapControl = new MapControl();
            mapControl.Size = new Size(749, 595);
            MapTab.Controls.Add(mapControl);
            //_globalHook = Gma.System.MouseKeyHook.Hook.GlobalEvents();
            //_globalHook.MouseDown += GlobalHookMouseDown;
            InventaireControl = new InventaireControl();
            this.InventaireTab.Controls.Add(InventaireControl);
        }

        private void YourForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        // Public method to access the singleton instance
        public static MainUI GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MainUI();
            }
            return _instance;
        }


        private void GlobalHookMouseDown(object sender, MouseEventArgs e)
        {
            // Convert the global mouse position to a position relative to the panel
            var panelPoint = DofusWindowPanel.PointToClient(new Point(e.X, e.Y));

            // Check if the mouse position is within the bounds of the panel
            if (DofusWindowPanel.ClientRectangle.Contains(panelPoint))
            {
                //XInputText.Text = $"{panelPoint.X}";
                inputScriptName.Text = $"{panelPoint.Y}";
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Point globalMousePosition = Cursor.Position;
            Point localMousePosition = DofusWindowPanel.PointToClient(globalMousePosition);

            // Vérifier si la position de la souris est à l'intérieur du panneau
            if (!DofusWindowPanel.ClientRectangle.Contains(localMousePosition))
                return; // Si ce n'est pas le cas, ne pas mettre à jour les coordonnées

            Xpos.Text = "Pos X: " + (localMousePosition.X-8).ToString();
            Ypos.Text = "Pos Y: " + (localMousePosition.Y-31).ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            DofusLoginForm loginForm = new DofusLoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                client = new Dofus();
                InventaireControl.SetClientDofus(client);
                if (checkBoxEatClient.Checked)
                    client.AbsorbApplication(DofusWindowPanel);
                await Task.Delay(1000);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (client == null || inputScriptName.Text == "") return;
            if (btnScriptCreator.Text == "Start Creator")
            {
                btnScriptCreator.Text = "Stop Creator";
                client.ScriptCreator.Start();
                client.ScriptCreator.Name = inputScriptName.Text;
            }
            else
            {
                btnScriptCreator.Text = "Start Creator";
                client.ScriptCreator.Save();
            }
        }

        private void checkBoxFightScript_CheckedChanged(object sender, EventArgs e)
        {
            client.ScriptCreator.Fight = this.checkBoxFightScript.Checked;
        }

        private void checkBoxHarverstScript_CheckedChanged(object sender, EventArgs e)
        {
            client.ScriptCreator.Harvest = this.checkBoxHarverstScript.Checked;
        }

        public static Script Script { get; private set; }

        private void buttonRunScript_Click(object sender, EventArgs e)
        {
            if (client == null || client.Account == null || client.Account.Character == null) return;
            if(Script == null) 
            {
                Script = new Script(client);
            }
            if (Script.IsRunning)
            {
                Script.Stop();
                Script = null;
            }
            else
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Script.Start(openFileDialog.FileName);
                    }
                }
            }
            UpdateUI();
        }

        public void UpdateUI(ScriptType forceValue = ScriptType.NONE)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ScriptType>(UpdateUI), forceValue);
                return;
            }
            if(forceValue == ScriptType.IA)
                this.btnLoadIA.BackColor = Color.Green;
            else if(client.Account.Character.Fight.LuaScript != null)
                this.btnLoadIA.BackColor = (client.Account.Character.Fight.LuaScript.AllGood ? Color.Green : Color.Red);
            else
                this.btnLoadIA.BackColor = Color.Red;


            if (forceValue == ScriptType.TRAVEL)
                this.buttonRunScript.BackColor = Color.Green;
            else if(Script != null && Script.LuaScript != null)
                this.buttonRunScript.BackColor = (Script.LuaScript.AllGood ? Color.Green : Color.Red);
            else
                this.buttonRunScript.BackColor = Color.Red;
        }


        private void btnLoadIA_Click(object sender, EventArgs e)
        {
            if (client == null) return;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    client.PathIaFile = openFileDialog.FileName;
                    if(client.Account != null && client.Account.Character != null && client.Account.Character.Fight != null)
                    {
                        client.Account.Character.Fight.LuaScript = new LuaScript(openFileDialog.FileName, ScriptType.IA, client);
                    }
                    this.btnLoadIA.BackColor = Color.Green;
                    Console.WriteLine("New AI load");
                }
            }
        }

    }
}
