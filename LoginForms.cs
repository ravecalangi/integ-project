using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;

        public LoginForm() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "FuelTrack — Login";
            this.Size = new Size(860, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            // Root split: left dark panel | right white panel
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370f));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.Controls.Add(tbl);

            // ── LEFT DARK PANEL ───────────────────────────────────
            var leftOuter = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 14, 26) };
            tbl.Controls.Add(leftOuter, 0, 0);

            // Gold top stripe
            leftOuter.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4, BackColor = Color.FromArgb(255, 200, 0) });

            var leftFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(40, 0, 20, 0)
            };
            leftOuter.Controls.Add(leftFlow);

            leftFlow.Controls.Add(Spacer(1, 90));

            leftFlow.Controls.Add(new Label
            {
                Text = "⛽",
                Font = new Font("Segoe UI", 28),
                ForeColor = Color.FromArgb(255, 200, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 4)
            });

            leftFlow.Controls.Add(new Label
            {
                Text = "FuelTrack",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 2)
            });

            leftFlow.Controls.Add(new Label
            {
                Text = "Oil Price Monitoring System",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(140, 155, 185),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 12)
            });

            leftFlow.Controls.Add(new Panel
            {
                Width = 55,
                Height = 2,
                BackColor = Color.FromArgb(255, 200, 0),
                Margin = new Padding(0, 0, 0, 16)
            });

            AddFeatureRow(leftFlow, "⚡", "Real-time price updates");
            AddFeatureRow(leftFlow, "📊", "Full price history tracking");
            AddFeatureRow(leftFlow, "🔔", "Instant notifications");

            var verPanel = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = Color.Transparent };
            verPanel.Controls.Add(new Label
            {
                Text = "v1.0  |  In-Memory Mode",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(65, 80, 108),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(40, 8)
            });
            leftOuter.Controls.Add(verPanel);

            // ── RIGHT WHITE PANEL ─────────────────────────────────
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            tbl.Controls.Add(right, 1, 0);

            // Use FlowLayout for right side content for clean vertical stacking
            var rightFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(60, 0, 40, 0)
            };
            right.Controls.Add(rightFlow);

            rightFlow.Controls.Add(Spacer(1, 68));

            rightFlow.Controls.Add(new Label
            {
                Text = "Welcome back",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 14, 26),
                AutoSize = true,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 4)
            });

            rightFlow.Controls.Add(new Label
            {
                Text = "Sign in to your account to continue",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(130, 140, 160),
                AutoSize = true,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 22)
            });

            // USERNAME
            rightFlow.Controls.Add(FieldLabel("USERNAME"));
            txtUsername = MakeTextBox(false);
            rightFlow.Controls.Add(txtUsername);
            rightFlow.Controls.Add(Spacer(1, 14));

            // PASSWORD
            rightFlow.Controls.Add(FieldLabel("PASSWORD"));
            txtPassword = MakeTextBox(true);
            rightFlow.Controls.Add(txtPassword);
            rightFlow.Controls.Add(Spacer(1, 6));

            // Error
            lblError = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(210, 50, 50),
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 8.5f),
                Margin = new Padding(0, 0, 0, 6)
            };
            rightFlow.Controls.Add(lblError);

            // Login button — width matches textboxes
            btnLogin = new Button
            {
                Text = "SIGN IN",
                Size = new Size(340, 46),
                BackColor = Color.FromArgb(255, 200, 0),
                ForeColor = Color.FromArgb(10, 14, 26),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 14)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 215, 40);
            btnLogin.Click += BtnLogin_Click;
            rightFlow.Controls.Add(btnLogin);

            // Separator
            rightFlow.Controls.Add(new Panel { Width = 340, Height = 1, BackColor = Color.FromArgb(225, 228, 238), Margin = new Padding(0, 0, 0, 10) });

            // Create Account button
            var btnRegister = new Button
            {
                Text = "👤  Create Account",
                Size = new Size(340, 38),
                BackColor = Color.FromArgb(235, 237, 245),
                ForeColor = Color.FromArgb(40, 52, 78),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += (s, e) =>
            {
                var reg = new RegisterForm();
                reg.ShowDialog(this);
            };
            rightFlow.Controls.Add(btnRegister);

            // Demo hint box
            var hintBox = new Panel
            {
                Size = new Size(340, 50),
                BackColor = Color.FromArgb(248, 249, 252)
            };
            hintBox.Controls.Add(new Label
            {
                Text = "Demo credentials",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 112, 135),
                AutoSize = true,
                Location = new Point(10, 8)
            });
            hintBox.Controls.Add(new Label
            {
                Text = "Admin: admin / admin123     User: user1 / user123",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(140, 150, 170),
                AutoSize = true,
                Location = new Point(10, 28)
            });
            rightFlow.Controls.Add(hintBox);

            this.AcceptButton = btnLogin;
        }

        private Label FieldLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 112, 135),
            AutoSize = true,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 4)
        };

        private TextBox MakeTextBox(bool isPassword) => new TextBox
        {
            Size = new Size(340, 34),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(248, 249, 252),
            PasswordChar = isPassword ? '●' : '\0',
            Margin = new Padding(0, 0, 0, 0)
        };

        private void AddFeatureRow(FlowLayoutPanel parent, string icon, string text)
        {
            var row = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10),
                WrapContents = false
            };
            row.Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(255, 200, 0), AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 1, 8, 0) });
            row.Controls.Add(new Label { Text = text, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(158, 172, 200), AutoSize = true, BackColor = Color.Transparent });
            parent.Controls.Add(row);
        }

        private Panel Spacer(int w, int h) => new Panel { Width = w, Height = h, BackColor = Color.Transparent };

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            { lblError.Text = "⚠  Please enter username and password."; return; }

            var user = InMemoryStore.Login(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            if (user == null) { lblError.Text = "⚠  Invalid username or password."; return; }

            Session.CurrentUser = user;
            txtUsername.Clear();
            txtPassword.Clear();
            lblError.Text = "";

            this.Hide();

            Form dashboard = user.IsAdmin ? (Form)new AdminDashboard() : new UserDashboard();
            dashboard.FormClosed += (ds, de) =>
            {
                // When dashboard closes (logout), show login again
                if (Session.CurrentUser == null)
                {
                    this.Show();
                }
            };
            dashboard.Show();
        }
    }
}