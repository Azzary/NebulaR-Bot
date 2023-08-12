using Nebular.Bot.Handlers;
using Nebular.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nebular.Bot.Interface
{
    [Obfuscation(Exclude = true)]
    public partial class DofusLoginForm : Form
    {
        private TextBox textBoxEmail;
        private TextBox textBoxPassword;
        private Button buttonAddAccount;
        private ListBox listBoxAccounts;
        private Label label1;
        private Label label2;
        private TextBox encryptionKey;
        private Label label3;
        private Button Play;
        private Label label4;
        private TextBox textBoxSpeudo;
        private ComboBox comboBoxServerList;
        private Label label5;
        private List<DofusAccount> Accounts;

        public DofusLoginForm()
        {
            InitializeComponent();
            LoadAccounts();
            foreach (string server in LoginHandler.Servers.Keys)
            {
                comboBoxServerList.Items.Add(server);
            }
        }


        private void LoadAccounts()
        {
            string filePath = "accounts.json";

            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                Accounts = JsonConvert.DeserializeObject<List<DofusAccount>>(jsonContent);
                if(Accounts == null)
                    Accounts = new List<DofusAccount>();
                // Mettez à jour le ListBox avec les e-mails des comptes
                listBoxAccounts.Items.Clear();
                foreach (var account in Accounts)
                {
                    if (String.IsNullOrEmpty(account.Speudo))
                        continue;
                    listBoxAccounts.Items.Add(account.Speudo);
                }
            }
            else
            {
                File.Create(filePath).Close();
                Accounts = new List<DofusAccount>();
            }
            Console.WriteLine("okai");
        }

        private void buttonAddAccount_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(encryptionKey.Text))
            {
                MessageBox.Show("La clé de chiffrement ne peut pas être vide.");
                return;
            }
            if(string.IsNullOrEmpty(this.textBoxEmail.Text) || string.IsNullOrEmpty(this.textBoxSpeudo.Text) || string.IsNullOrEmpty(this.textBoxEmail.Text))
            {
                MessageBox.Show("Erreur email ou mdp ou speudo.");
                return;
            }
            if (string.IsNullOrEmpty(comboBoxServerList.SelectedItem.ToString()))
            {
                MessageBox.Show("Erreur select server");
                return;
            }

            DofusAccount newAccount = new DofusAccount { Serveur = comboBoxServerList.SelectedItem.ToString(), Speudo = this.textBoxSpeudo.Text,  Email = this.textBoxEmail.Text, Password = Encrypt(this.textBoxPassword.Text, encryptionKey.Text) };
            if (String.IsNullOrEmpty(newAccount.Email))
                return;
            Accounts.Add(newAccount);
            listBoxAccounts.Items.Add(newAccount.Speudo);
            string jsonContent = JsonConvert.SerializeObject(Accounts);
            File.WriteAllText("accounts.json", jsonContent);
            LoadAccounts();
            this.textBoxEmail.Clear();
            this.textBoxPassword.Clear();
        }

        private string Encrypt(string clearText, string encryptionKey)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    aesAlg.Key = key.GetBytes(32);
                    aesAlg.IV = key.GetBytes(16);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(clearText);
                            }
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }
        private string Decrypt(string cipherText, string encryptionKey)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    aesAlg.Key = key.GetBytes(32);
                    aesAlg.IV = key.GetBytes(16);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }



        private void InitializeComponent()
        {
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonAddAccount = new System.Windows.Forms.Button();
            this.listBoxAccounts = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.encryptionKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Play = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSpeudo = new System.Windows.Forms.TextBox();
            this.comboBoxServerList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxEmail
            // 
            this.textBoxEmail.Location = new System.Drawing.Point(20, 40);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Size = new System.Drawing.Size(150, 20);
            this.textBoxEmail.TabIndex = 1;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(20, 84);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(150, 20);
            this.textBoxPassword.TabIndex = 2;
            // 
            // buttonAddAccount
            // 
            this.buttonAddAccount.Location = new System.Drawing.Point(37, 266);
            this.buttonAddAccount.Name = "buttonAddAccount";
            this.buttonAddAccount.Size = new System.Drawing.Size(111, 26);
            this.buttonAddAccount.TabIndex = 3;
            this.buttonAddAccount.Text = "Add Account";
            this.buttonAddAccount.Click += new System.EventHandler(this.buttonAddAccount_Click);
            // 
            // listBoxAccounts
            // 
            this.listBoxAccounts.Location = new System.Drawing.Point(183, 40);
            this.listBoxAccounts.Name = "listBoxAccounts";
            this.listBoxAccounts.Size = new System.Drawing.Size(200, 212);
            this.listBoxAccounts.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Email";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Mdp";
            // 
            // encryptionKey
            // 
            this.encryptionKey.Location = new System.Drawing.Point(23, 207);
            this.encryptionKey.Name = "encryptionKey";
            this.encryptionKey.Size = new System.Drawing.Size(147, 20);
            this.encryptionKey.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Clée de chiffrement";
            // 
            // Play
            // 
            this.Play.Location = new System.Drawing.Point(230, 266);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(107, 26);
            this.Play.TabIndex = 9;
            this.Play.Text = "Play";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.buttonSelectAccount_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Speudo";
            // 
            // textBoxSpeudo
            // 
            this.textBoxSpeudo.Location = new System.Drawing.Point(20, 126);
            this.textBoxSpeudo.Name = "textBoxSpeudo";
            this.textBoxSpeudo.Size = new System.Drawing.Size(150, 20);
            this.textBoxSpeudo.TabIndex = 10;
            // 
            // comboBoxServerList
            // 
            this.comboBoxServerList.FormattingEnabled = true;
            this.comboBoxServerList.Location = new System.Drawing.Point(23, 164);
            this.comboBoxServerList.Name = "comboBoxServerList";
            this.comboBoxServerList.Size = new System.Drawing.Size(147, 21);
            this.comboBoxServerList.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Serveur";
            // 
            // DofusLoginForm
            // 
            this.ClientSize = new System.Drawing.Size(395, 304);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxServerList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSpeudo);
            this.Controls.Add(this.Play);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.encryptionKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEmail);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.buttonAddAccount);
            this.Controls.Add(this.listBoxAccounts);
            this.Name = "DofusLoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        public static DofusAccount SelectedAccount = null;


        private void buttonSelectAccount_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxAccounts.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= Accounts.Count)
            {
                MessageBox.Show("Veuillez sélectionner un compte!");
                return;
            }

            DofusAccount selectedAccount = Accounts.FirstOrDefault(x =>x.Speudo == listBoxAccounts.Text);
            // Vérifier la clé de déchiffrement
            string key = encryptionKey.Text; // Supposons que la clé de déchiffrement est dans un contrôle TextBox nommé encryptionKey
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Veuillez entrer la clé de déchiffrement!");
                return;
            }
            if (selectedAccount == null)
            {
                MessageBox.Show("Erreur impossible de trouver le compte");
                return;
            }

            // Déchiffrer le mot de passe
            SelectedAccount = selectedAccount;
            SelectedAccount.Password = Decrypt(selectedAccount.Password, key);
            Zaap.ZaapAPI.ZaapServer.SelectedAccount = SelectedAccount;
            if (string.IsNullOrEmpty(SelectedAccount.Password))
            {
                MessageBox.Show("Erreur avec la clé de déchiffrement!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
            try
            {
 
            }
            catch
            {
                MessageBox.Show("Erreur lors du déchiffrement du mot de passe. Veuillez vérifier la clé.");
            }
        }

    }



}
