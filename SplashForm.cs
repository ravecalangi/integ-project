using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class SplashForm : Form
    {
        public SplashForm() { InitializeComponent(); }

        private void InitializeComponent()
        {
            this.Text = "FuelTrack";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(800, 500);
            this.BackColor = Color.FromArgb(10, 14, 26);

            try
            {
                this.BackgroundImage = Image.FromFile(
                    System.IO.Path.Combine(Application.StartupPath, "assets", "cover-img.jpg"));
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }

            // Dark overlay + root split
            var overlay = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(195, 10, 14, 26)
            };
            overlay.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48f));
            overlay.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52f));
            this.Controls.Add(overlay);

            // ── LEFT ─────────────────────────────────────────────
            var leftOuter = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            overlay.Controls.Add(leftOuter, 0, 0);

            // gold accent bar on left edge
            leftOuter.Controls.Add(new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = Color.FromArgb(255, 200, 0)
            });

            var left = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(28, 0, 12, 0)
            };
            leftOuter.Controls.Add(left);

            // Version bar pinned at bottom
            var verBar = new Panel { Dock = DockStyle.Bottom, Height = 32, BackColor = Color.Transparent };
            verBar.Controls.Add(new Label
            {
                Text = "v1.0  |  Oil Price Monitoring System",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(80, 100, 130),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(2, 10)
            });
            left.Controls.Add(verBar);

            // Content flows top-down via FlowLayoutPanel
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 0)
            };
            left.Controls.Add(flow);

            // Top spacer
            flow.Controls.Add(Spacer(1, 80));

            flow.Controls.Add(new Label
            {
                Text = "⛽",
                Font = new Font("Segoe UI", 28),
                ForeColor = Color.FromArgb(255, 200, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 2)
            });

            flow.Controls.Add(new Label
            {
                Text = "FuelTrack",
                Font = new Font("Segoe UI", 34, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 2)
            });

            flow.Controls.Add(new Label
            {
                Text = "Monitor. Track. Save.",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 200, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 8)
            });

            flow.Controls.Add(new Panel
            {
                Width = 180,
                Height = 2,
                BackColor = Color.FromArgb(70, 255, 200, 0),
                Margin = new Padding(0, 0, 0, 10)
            });

            flow.Controls.Add(new Label
            {
                Text = "Stay updated with the latest\nfuel prices in real-time.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(185, 200, 225),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(2, 0, 0, 20)
            });

            var btnStart = new Button
            {
                Text = "Get Started  →",
                Size = new Size(185, 46),
                BackColor = Color.FromArgb(255, 200, 0),
                ForeColor = Color.FromArgb(10, 14, 26),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 215, 40);
            btnStart.Click += (s, e) => this.Close();
            flow.Controls.Add(btnStart);

            // ── RIGHT ────────────────────────────────────────────
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20, 0, 20, 0) };
            overlay.Controls.Add(right, 1, 0);

            // Top spacer pinned
            var rightFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            right.Controls.Add(rightFlow);

            rightFlow.Controls.Add(Spacer(1, 70));

            rightFlow.Controls.Add(new Label
            {
                Text = "FEATURES",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8)
            });

            rightFlow.Controls.Add(MakeCard(right, "⚡", "Real-time Updates", "Get the latest fuel prices as soon as they change."));
            rightFlow.Controls.Add(MakeCard(right, "📊", "Price History", "Track and compare fuel prices over time."));
            rightFlow.Controls.Add(MakeCard(right, "🔔", "Smart Notifications", "Instant alerts when prices are updated."));
        }

        private Panel MakeCard(Panel parent, string icon, string title, string desc)
        {
            var card = new Panel
            {
                Height = 96,
                BackColor = Color.FromArgb(28, 255, 255, 255),
                Margin = new Padding(0, 0, 0, 8)
            };
            // Make card width track parent resize
            card.SizeChanged += (s, e) => { };
            parent.SizeChanged += (s, e) => card.Width = parent.ClientSize.Width - 40;
            card.Width = 420;

            card.Controls.Add(new Panel
            {
                Dock = DockStyle.Left,
                Width = 3,
                BackColor = Color.FromArgb(255, 200, 0)
            });

            var inner = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 10, 10, 10)
            };
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 38f));
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            inner.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            inner.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            card.Controls.Add(inner);

            inner.Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI", 15), ForeColor = Color.FromArgb(255, 200, 0), AutoSize = true, BackColor = Color.Transparent }, 0, 0);
            inner.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent }, 1, 0);
            inner.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 1);
            inner.Controls.Add(new Label { Text = desc, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(168, 188, 215), AutoSize = true, BackColor = Color.Transparent }, 1, 1);

            return card;
        }

        private Panel Spacer(int w, int h) => new Panel { Width = w, Height = h, BackColor = Color.Transparent };
    }
}