using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class UserDashboard : Form
    {
        private DataGridView dgvLatest;
        private DataGridView dgvPrices;
        private ComboBox cmbFilterType;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnBell;
        private Label lblStatus;
        private System.Windows.Forms.Timer notifTimer;

        public UserDashboard() { InitializeComponent(); LoadLatestPrices(); LoadPrices(); StartPolling(); }

        private void InitializeComponent()
        {
            this.Text = "FuelTrack — User View";
            this.Size = new Size(1140, 740);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(242, 244, 250);
            this.Font = new Font("Segoe UI", 9.5f);

            // Root TableLayoutPanel: nav | toolbar | current label | current grid | history label | history grid | statusbar
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));   // Row 0: Nav
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));   // Row 1: Filter toolbar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));   // Row 2: "Current Prices" label
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180f));  // Row 3: Current prices grid (3 rows + header)
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));   // Row 4: "Price History" label
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Row 5: History grid
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));   // Row 6: Status bar
            this.Controls.Add(root);

            // ── ROW 0: NAV BAR ────────────────────────────────────
            var nav = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(10, 14, 26),
                Margin = new Padding(0)
            };
            nav.Controls.Add(new Panel
            {
                Dock = DockStyle.Top,
                Height = 3,
                BackColor = Color.FromArgb(255, 200, 0)
            });
            nav.Controls.Add(new Label
            {
                Text = "⛽  FuelTrack",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(18, 18)
            });
            nav.Controls.Add(new Label
            {
                Text = $"👤  {Session.CurrentUser?.Username}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(155, 170, 200),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(192, 20)
            });
            btnBell = new Button
            {
                Text = "🔔  Notifications (0)",
                Size = new Size(172, 30),
                BackColor = Color.FromArgb(34, 46, 72),
                ForeColor = Color.FromArgb(185, 200, 228),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            btnBell.FlatAppearance.BorderSize = 0;
            btnBell.Click += BtnBell_Click;
            nav.Controls.Add(btnBell);
            var btnLogout = new Button
            {
                Text = "⏻  Logout",
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(180, 35, 35),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                notifTimer?.Stop();
                Session.Clear();
                this.Close();
            };
            nav.Controls.Add(btnLogout);
            void PosNavBtns()
            {
                btnLogout.Location = new Point(nav.ClientSize.Width - 116, 15);
                btnBell.Location = new Point(nav.ClientSize.Width - 296, 15);
            }
            nav.Resize += (s, e) => PosNavBtns();
            this.Load += (s, e) => PosNavBtns();
            root.Controls.Add(nav, 0, 0);

            // ── ROW 1: FILTER TOOLBAR ─────────────────────────────
            var toolbar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };
            toolbar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 224, 235)),
                    0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);

            var filterFlow = new FlowLayoutPanel
            {
                Location = new Point(10, 10),
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            filterFlow.Controls.Add(MiniLabel("FILTER"));
            filterFlow.Controls.Add(SmallLabel("Type"));
            cmbFilterType = new ComboBox
            {
                Size = new Size(112, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(248, 249, 252),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 0, 10, 0)
            };
            cmbFilterType.Items.AddRange(new[] { "All", "Gasoline", "Diesel", "Kerosene" });
            cmbFilterType.SelectedIndex = 0;
            filterFlow.Controls.Add(cmbFilterType);
            filterFlow.Controls.Add(SmallLabel("From"));
            dtpFrom = new DateTimePicker
            {
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(4, 0, 10, 0)
            };
            dtpFrom.Value = DateTime.Today.AddMonths(-3);
            filterFlow.Controls.Add(dtpFrom);
            filterFlow.Controls.Add(SmallLabel("To"));
            dtpTo = new DateTimePicker
            {
                Size = new Size(130, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(4, 0, 10, 0)
            };
            filterFlow.Controls.Add(dtpTo);
            var btnSearch = MakeBtn("🔍  Search", Color.FromArgb(10, 14, 26), Color.White);
            btnSearch.Click += (s, e) => LoadPrices();
            filterFlow.Controls.Add(btnSearch);
            toolbar.Controls.Add(filterFlow);
            root.Controls.Add(toolbar, 0, 1);

            // ── ROW 2: CURRENT PRICES LABEL ───────────────────────
            var pnlLblCurrent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(242, 244, 250),
                Margin = new Padding(0)
            };
            pnlLblCurrent.Controls.Add(new Label
            {
                Text = "📊  Current Prices",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 38, 58),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(14, 6)
            });
            root.Controls.Add(pnlLblCurrent, 0, 2);

            // ── ROW 3: CURRENT PRICES GRID ────────────────────────
            var latestWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 0, 14, 4),
                Margin = new Padding(0)
            };
            dgvLatest = BuildGrid();
            dgvLatest.Dock = DockStyle.Fill;
            latestWrap.Controls.Add(dgvLatest);
            root.Controls.Add(latestWrap, 0, 3);

            // ── ROW 4: PRICE HISTORY LABEL ────────────────────────
            var pnlLblHistory = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(242, 244, 250),
                Margin = new Padding(0)
            };
            pnlLblHistory.Controls.Add(new Label
            {
                Text = "📋  Price History",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 38, 58),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(14, 6)
            });
            root.Controls.Add(pnlLblHistory, 0, 4);

            // ── ROW 5: PRICE HISTORY GRID ─────────────────────────
            var historyWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 0, 14, 4),
                Margin = new Padding(0)
            };
            dgvPrices = BuildGrid();
            dgvPrices.Dock = DockStyle.Fill;
            historyWrap.Controls.Add(dgvPrices);
            root.Controls.Add(historyWrap, 0, 5);

            // ── ROW 6: STATUS BAR ─────────────────────────────────
            var statusBar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(230, 233, 242),
                Margin = new Padding(0)
            };
            statusBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(210, 214, 228)), 0, 0, statusBar.Width, 0);
            lblStatus = new Label
            {
                Location = new Point(14, 7),
                AutoSize = true,
                ForeColor = Color.FromArgb(115, 128, 155),
                Font = new Font("Segoe UI", 8)
            };
            statusBar.Controls.Add(lblStatus);
            root.Controls.Add(statusBar, 0, 6);
        }

        private void LoadLatestPrices()
        {
            var prices = InMemoryStore.GetLatestPrices();
            dgvLatest.Columns.Clear();
            dgvLatest.Columns.Add("FuelType", "Fuel Type");
            dgvLatest.Columns.Add("Price", "Latest Price (₱)");
            dgvLatest.Columns.Add("Date", "Effective Date");
            dgvLatest.Columns.Add("Station", "Station");
            foreach (var p in prices)
            {
                int r = dgvLatest.Rows.Add(p.FuelType, $"₱{p.Price:F2}", p.EffectiveDate.ToString("MMM dd, yyyy"), p.StationName);
                dgvLatest.Rows[r].DefaultCellStyle.BackColor = FuelColor(p.FuelType);
                dgvLatest.Rows[r].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void LoadPrices()
        {
            var prices = InMemoryStore.GetAllPrices(cmbFilterType.SelectedItem?.ToString(), dtpFrom.Value.Date, dtpTo.Value.Date);
            dgvPrices.Columns.Clear();
            dgvPrices.Columns.Add("FuelType", "Fuel Type");
            dgvPrices.Columns.Add("Price", "Price (₱)");
            dgvPrices.Columns.Add("EffectiveDate", "Effective Date");
            dgvPrices.Columns.Add("StationName", "Station");
            dgvPrices.Columns.Add("Notes", "Notes");
            dgvPrices.Columns.Add("CreatedAt", "Added On");
            foreach (var p in prices)
                dgvPrices.Rows.Add(p.FuelType, $"₱{p.Price:F2}", p.EffectiveDate.ToString("MMM dd, yyyy"),
                    p.StationName, p.Notes, p.CreatedAt.ToString("MMM dd, yyyy hh:mm tt"));
            lblStatus.Text = $"Showing {prices.Count} record(s)   ·   Last refreshed: {DateTime.Now:hh:mm tt}";
        }

        private void StartPolling()
        {
            notifTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            notifTimer.Tick += (s, e) => CheckNotifications();
            notifTimer.Start();
            CheckNotifications();
        }

        private void CheckNotifications()
        {
            int count = InMemoryStore.GetUnreadCount(Session.CurrentUser.Id);
            btnBell.Text = $"🔔  Notifications ({count})";
            btnBell.BackColor = count > 0 ? Color.FromArgb(190, 82, 12) : Color.FromArgb(34, 46, 72);
            if (count > 0) { LoadLatestPrices(); LoadPrices(); }
        }

        private void BtnBell_Click(object sender, EventArgs e)
        {
            var notifs = InMemoryStore.GetUnreadNotifications(Session.CurrentUser.Id);
            if (notifs.Count == 0) { MessageBox.Show("No new notifications.", "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            string msg = "📬 New Price Updates:\n\n";
            foreach (var n in notifs)
                msg += $"• {n.Message}\n  ({n.CreatedAt:MMM dd, yyyy hh:mm tt})\n\n";
            MessageBox.Show(msg, $"Notifications ({notifs.Count})", MessageBoxButtons.OK, MessageBoxIcon.Information);
            InMemoryStore.MarkAllRead(Session.CurrentUser.Id);
            btnBell.Text = "🔔  Notifications (0)";
            btnBell.BackColor = Color.FromArgb(34, 46, 72);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { notifTimer?.Stop(); base.OnFormClosed(e); }

        private Color FuelColor(string t) => t switch
        {
            "Gasoline" => Color.FromArgb(255, 252, 220),
            "Diesel" => Color.FromArgb(220, 240, 255),
            "Kerosene" => Color.FromArgb(220, 255, 235),
            _ => Color.White
        };

        private Button MakeBtn(string text, Color bg, Color fg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(96, 32),
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 5, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private Label MiniLabel(string t) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 7, FontStyle.Bold),
            ForeColor = Color.FromArgb(175, 182, 200),
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 9, 10, 0)
        };

        private Label SmallLabel(string t) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(100, 112, 135),
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 8, 4, 0)
        };

        private DataGridView BuildGrid()
        {
            var g = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f),
                ColumnHeadersHeight = 40,
                GridColor = Color.FromArgb(228, 232, 244),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            g.RowTemplate.Height = 36;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(16, 22, 40);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(185, 200, 228);
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(16, 22, 40);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 255);
            g.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 175);
            g.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 14, 26);
            return g;
        }
    }
}