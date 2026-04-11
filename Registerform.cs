using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Label lblError;
        private Button btnRegister;

        public RegisterForm() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "FuelTrack — Create Account";
            this.Size = new Size(460, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            // Root TableLayoutPanel
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62f));  // Header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Body
            this.Controls.Add(root);

            // ── HEADER ────────────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 14, 26), Margin = new Padding(0) };
            header.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 3, BackColor = Color.FromArgb(255, 200, 0) });
            header.Controls.Add(new Label
            {
                Text = "👤  Create Account",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(20, 22)
            });
            root.Controls.Add(header, 0, 0);

            // ── BODY ──────────────────────────────────────────────
            var body = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(36, 24, 36, 16)
            };
            root.Controls.Add(body, 0, 1);

            // Subtitle
            body.Controls.Add(new Label
            {
                Text = "Fill in the details below to create your account.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(130, 140, 160),
                AutoSize = true,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 20)
            });

            // Username
            body.Controls.Add(FieldLabel("USERNAME"));
            txtUsername = MakeField();
            body.Controls.Add(txtUsername);
            body.Controls.Add(Spacer(14));

            // Password
            body.Controls.Add(FieldLabel("PASSWORD"));
            txtPassword = MakeField(true);
            body.Controls.Add(txtPassword);
            body.Controls.Add(Spacer(6));

            body.Controls.Add(new Label
            {
                Text = "Minimum 6 characters",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(170, 178, 195),
                AutoSize = true,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10)
            });

            // Confirm Password
            body.Controls.Add(FieldLabel("CONFIRM PASSWORD"));
            txtConfirmPassword = MakeField(true);
            body.Controls.Add(txtConfirmPassword);
            body.Controls.Add(Spacer(6));

            // Note: role is always 'user'
            var notePanel = new Panel
            {
                Size = new Size(380, 36),
                BackColor = Color.FromArgb(248, 249, 252),
                Margin = new Padding(0, 0, 0, 12)
            };
            notePanel.Controls.Add(new Label
            {
                Text = "ℹ  New accounts are registered as User only.",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 112, 135),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(10, 9)
            });
            body.Controls.Add(notePanel);

            // Error label
            lblError = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(210, 50, 50),
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 8.5f),
                Margin = new Padding(0, 0, 0, 6)
            };
            body.Controls.Add(lblError);

            // Register button
            btnRegister = new Button
            {
                Text = "✅  Create Account",
                Size = new Size(380, 44),
                BackColor = Color.FromArgb(255, 200, 0),
                ForeColor = Color.FromArgb(10, 14, 26),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 215, 40);
            btnRegister.Click += BtnRegister_Click;
            body.Controls.Add(btnRegister);

            // Back to login link
            var btnBack = new Button
            {
                Text = "← Back to Login",
                Size = new Size(380, 34),
                BackColor = Color.FromArgb(235, 237, 245),
                ForeColor = Color.FromArgb(55, 68, 92),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            body.Controls.Add(btnBack);

            this.AcceptButton = btnRegister;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblError.Text = "⚠  Please fill in all fields.";
                return;
            }

            if (txtUsername.Text.Trim().Length < 3)
            {
                lblError.Text = "⚠  Username must be at least 3 characters.";
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                lblError.Text = "⚠  Password must be at least 6 characters.";
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblError.Text = "⚠  Passwords do not match.";
                return;
            }

            // Check if username already exists
            if (InMemoryStore.UsernameExists(txtUsername.Text.Trim()))
            {
                lblError.Text = "⚠  Username already taken. Try another.";
                return;
            }

            // Register
            bool success = InMemoryStore.RegisterUser(txtUsername.Text.Trim(), txtPassword.Text);

            if (success)
            {
                MessageBox.Show(
                    $"✅ Account created successfully!\n\nUsername: {txtUsername.Text.Trim()}\n\nYou can now log in.",
                    "Registration Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                lblError.Text = "⚠  Something went wrong. Please try again.";
            }
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

        private TextBox MakeField(bool isPassword = false) => new TextBox
        {
            Size = new Size(380, 34),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(248, 249, 252),
            PasswordChar = isPassword ? '●' : '\0',
            Margin = new Padding(0, 0, 0, 0)
        };

        private Panel Spacer(int h) => new Panel
        {
            Size = new Size(1, h),
            BackColor = Color.White
        };
    }
}