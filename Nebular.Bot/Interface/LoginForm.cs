using Nebular.Bot.Network;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Nebular.Bot.Interface
{
    [Obfuscation(Exclude = true)]
    public partial class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Label labelEmail;
        private Label labelPassword;
        private Label labelTitle;
        private Button BtnLogin;
        private Label errorLabel;

        public static LoginForm Instance { get;private set; }

        public void FailConnect()
        {
            if (BtnLogin.InvokeRequired)
            {
                BtnLogin.Invoke(new Action(FailConnect));
            }
            else
            {
                BtnLogin.Enabled = true;
                errorLabel.Text = "Incorrect Email or Password";
            }
        }


        public LoginForm()
        {
            Instance = this;
            InitializeComponent();
        }
        

        private void InitializeComponent()
        {
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.BtnLogin = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(55, 99);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(168, 20);
            this.txtEmail.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(55, 148);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(168, 20);
            this.txtPassword.TabIndex = 1;
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(55, 80);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(32, 13);
            this.labelEmail.TabIndex = 2;
            this.labelEmail.Text = "Email";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(55, 135);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(75, 35);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(119, 25);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "NebularBot";
            // 
            // BtnLogin
            // 
            this.BtnLogin.Location = new System.Drawing.Point(97, 190);
            this.BtnLogin.Name = "BtnLogin";
            this.BtnLogin.Size = new System.Drawing.Size(75, 23);
            this.BtnLogin.TabIndex = 5;
            this.BtnLogin.Text = "Login";
            this.BtnLogin.UseVisualStyleBackColor = true;
            this.BtnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.BackColor = System.Drawing.SystemColors.Menu;
            this.errorLabel.Location = new System.Drawing.Point(69, 58);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 6;
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.BtnLogin);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelEmail);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtEmail);
            this.Name = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public static string Token = "";

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez saisir un email et un mot de passe.", "Erreur de connexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BtnLogin.Enabled = false;
            await Login();
        }

        private async Task Login()
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string stringjson = await NebularServer.NebularClient.PostRequest(email, password);
            dynamic jsonObject = JsonConvert.DeserializeObject(stringjson);
            string token = "";
            if (jsonObject != null && jsonObject.token != null)
            {
                token = jsonObject.token;
            }

            if (token != "")
            {
                Token = token;
                NebularServer.NebularClient.UpdateUser();
                PacketHandler.Initialize();
                Program.LoadData();
                new Nebular.Zaap.ZaapAPI.ZaapServer().Start();
                new Bot.Hook.ProxyHook().Start();
                this.Hide();


                Bot.MainUI formMain = new Bot.MainUI();
                formMain.Closed += (s, args) => this.Close();
                formMain.Show();
            }
            else
            {
                BtnLogin.Enabled = true;
                MessageBox.Show("Login incorrect");
            }
        }


    }
}
