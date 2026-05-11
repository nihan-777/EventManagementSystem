using System;
using System.Drawing;
using System.Windows.Forms;
using EventManagementSystem.Repository;

namespace EventManagementSystem.Forms
{
    /// <summary>
    /// Login and Registration form.
    /// Validates credentials and routes user/admin to their dashboard.
    /// </summary>
    public class LoginForm : Form
    {
        // ─── Controls ───────────────────────────────────────────
        private TabControl tabMain;
        private TabPage tabLogin, tabRegister;

        // Login tab
        private TextBox txtLoginEmail, txtLoginPassword;
        private Button btnLogin;
        private Label lblLoginTitle, lblLoginEmail, lblLoginPwd;

        // Register tab
        private TextBox txtRegName, txtRegEmail, txtRegPassword, txtRegConfirm;
        private Button btnRegister;
        private Label lblRegTitle, lblRegName, lblRegEmail, lblRegPwd, lblRegConfirm;

        private Label lblStatus;

        private readonly UserRepository _userRepo = new UserRepository();

        // ─── Constructor ─────────────────────────────────────────
        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Event Management System – Login";
            this.Size = new Size(420, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(20, 30, 48);

            // ── Header banner ──
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 120, 215)
            };
            var lblHeader = new Label
            {
                Text = "🎟  Online Event Management",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            banner.Controls.Add(lblHeader);
            this.Controls.Add(banner);

            // ── Tabs ──
            tabMain = new TabControl
            {
                Location = new Point(20, 70),
                Size = new Size(375, 255),
                Font = new Font("Segoe UI", 10)
            };

            tabLogin = new TabPage("Login");
            tabRegister = new TabPage("Register");

            BuildLoginTab();
            BuildRegisterTab();

            tabMain.TabPages.Add(tabLogin);
            tabMain.TabPages.Add(tabRegister);
            this.Controls.Add(tabMain);

            // ── Status label ──
            lblStatus = new Label
            {
                Location = new Point(20, 335),
                Size = new Size(375, 22),
                ForeColor = Color.Salmon,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblStatus);
        }

        // ─── Login Tab ────────────────────────────────────────────
        private void BuildLoginTab()
        {
            tabLogin.BackColor = Color.FromArgb(30, 42, 62);

            lblLoginTitle = MakeLabel("Sign in to your account", 10, 10, 340, bold: true, color: Color.White);
            lblLoginEmail = MakeLabel("Email:", 10, 45, 100, color: Color.Silver);
            txtLoginEmail = MakeTextBox(10, 65, 340);

            lblLoginPwd = MakeLabel("Password:", 10, 100, 100, color: Color.Silver);
            txtLoginPassword = MakeTextBox(10, 120, 340, password: true);

            btnLogin = MakeButton("Login", 10, 165, 340);
            btnLogin.Click += BtnLogin_Click;

            tabLogin.Controls.AddRange(new Control[]
            { lblLoginTitle, lblLoginEmail, txtLoginEmail, lblLoginPwd, txtLoginPassword, btnLogin });
        }

        // ─── Register Tab ─────────────────────────────────────────
        private void BuildRegisterTab()
        {
            tabRegister.BackColor = Color.FromArgb(30, 42, 62);

            lblRegTitle = MakeLabel("Create a new account", 10, 10, 340, bold: true, color: Color.White);
            lblRegName = MakeLabel("Full Name:", 10, 42, 100, color: Color.Silver);
            txtRegName = MakeTextBox(10, 60, 340);

            lblRegEmail = MakeLabel("Email:", 10, 90, 100, color: Color.Silver);
            txtRegEmail = MakeTextBox(10, 108, 340);

            lblRegPwd = MakeLabel("Password:", 10, 138, 100, color: Color.Silver);
            txtRegPassword = MakeTextBox(10, 156, 340, password: true);

            lblRegConfirm = MakeLabel("Confirm Password:", 10, 186, 150, color: Color.Silver);
            txtRegConfirm = MakeTextBox(165, 184, 185, password: true);

            btnRegister = MakeButton("Register", 10, 215, 340);
            btnRegister.Click += BtnRegister_Click;

            tabRegister.Controls.AddRange(new Control[]
            {
                lblRegTitle, lblRegName, txtRegName,
                lblRegEmail, txtRegEmail,
                lblRegPwd, txtRegPassword,
                lblRegConfirm, txtRegConfirm,
                btnRegister
            });
        }

        // ─── Events ───────────────────────────────────────────────
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            if (string.IsNullOrWhiteSpace(txtLoginEmail.Text) ||
                string.IsNullOrWhiteSpace(txtLoginPassword.Text))
            {
                lblStatus.Text = "Please enter email and password.";
                return;
            }

            var user = _userRepo.Login(txtLoginEmail.Text.Trim(), txtLoginPassword.Text);

            if (user == null)
            {
                lblStatus.Text = "Invalid email or password.";
                return;
            }

            Session.CurrentUser = user;
            this.Hide();

            if (user.IsAdmin)
                new AdminDashboardForm().ShowDialog();
            else
                new UserDashboardForm().ShowDialog();

            this.Show();
            txtLoginEmail.Clear();
            txtLoginPassword.Clear();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            if (string.IsNullOrWhiteSpace(txtRegName.Text) ||
                string.IsNullOrWhiteSpace(txtRegEmail.Text) ||
                string.IsNullOrWhiteSpace(txtRegPassword.Text))
            {
                lblStatus.Text = "All fields are required.";
                return;
            }

            if (txtRegPassword.Text != txtRegConfirm.Text)
            {
                lblStatus.Text = "Passwords do not match.";
                return;
            }

            bool success = _userRepo.Register(
                txtRegName.Text.Trim(),
                txtRegEmail.Text.Trim(),
                txtRegPassword.Text);

            if (!success)
            {
                lblStatus.Text = "Email already exists. Please use a different email.";
                return;
            }

            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Text = "Account created! You can now log in.";
            tabMain.SelectedTab = tabLogin;
            txtRegName.Clear(); txtRegEmail.Clear();
            txtRegPassword.Clear(); txtRegConfirm.Clear();
        }

        // ─── Helper Factory Methods ───────────────────────────────
        private Label MakeLabel(string text, int x, int y, int w,
                                bool bold = false, Color? color = null)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 22),
                ForeColor = color ?? Color.White,
                Font = bold
                    ? new Font("Segoe UI", 10, FontStyle.Bold)
                    : new Font("Segoe UI", 9)
            };
        }

        private TextBox MakeTextBox(int x, int y, int w, bool password = false)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 26),
                PasswordChar = password ? '●' : '\0',
                BackColor = Color.FromArgb(45, 60, 85),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Button MakeButton(string text, int x, int y, int w)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
