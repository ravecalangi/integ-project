using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class AdminDashboard : Form
    {
        private DataGridView dgvPrices;
        private ComboBox cmbFilterType;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Label lblStatus;

        public AdminDashboard() { InitializeComponent(); LoadPrices(); }

        private void InitializeComponent()
        {
            this.Text = "FuelTrack — Admin Dashboard";
            this.Size = new Size(1140, 720);
            this.MinimumSize = new Size(900, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(242, 244, 250);
            this.Font = new Font("Segoe UI", 9.5f);

            // Root TableLayoutPanel — 3 rows: nav | toolbar | grid+statusbar
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));   // Row 0: Nav bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));   // Row 1: Toolbar
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Row 2: Grid area
            this.Controls.Add(root);

            // ── ROW 0: NAV BAR ────────────────────────────────────
            var nav = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(10, 14, 26),
                Margin = new Padding(0)
            };
            // Gold top accent
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
            var badge = new Panel
            {
                Location = new Point(192, 18),
                Size = new Size(56, 22),
                BackColor = Color.FromArgb(255, 200, 0)
            };
            badge.Controls.Add(new Label
            {
                Text = "ADMIN",
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 14, 26),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(6, 4)
            });
            nav.Controls.Add(badge);
            nav.Controls.Add(new Label
            {
                Text = $"👤  {Session.CurrentUser?.Username}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(155, 170, 200),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(260, 20)
            });
            var btnLogout = MakeBtn("⏻  Logout", Color.FromArgb(180, 35, 35), Color.White);
            btnLogout.Size = new Size(100, 30);
            btnLogout.Click += (s, e) =>
            {
                Session.Clear();
                this.Close();
            };
            nav.Controls.Add(btnLogout);
            void PosLogout() => btnLogout.Location = new Point(nav.ClientSize.Width - 116, 15);
            nav.Resize += (s, e) => PosLogout();
            this.Load += (s, e) => PosLogout();
            root.Controls.Add(nav, 0, 0);

            // ── ROW 1: TOOLBAR ────────────────────────────────────
            var toolbar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };
            toolbar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 224, 235)),
                    0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);

            // Filter group (left)
            var filterGroup = new FlowLayoutPanel
            {
                Location = new Point(12, 0),
                AutoSize = true,
                Height = 54,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            toolbar.Controls.Add(filterGroup);

            filterGroup.Controls.Add(MiniLabel("FILTER"));
            filterGroup.Controls.Add(SmallLabel("Type"));
            cmbFilterType = new ComboBox
            {
                Size = new Size(112, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(248, 249, 252),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 14, 10, 0)
            };
            cmbFilterType.Items.AddRange(new[] { "All", "Gasoline", "Diesel", "Kerosene" });
            cmbFilterType.SelectedIndex = 0;
            filterGroup.Controls.Add(cmbFilterType);

            filterGroup.Controls.Add(SmallLabel("From"));
            dtpFrom = new DateTimePicker
            {
                Size = new Size(130, 26),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(4, 14, 10, 0)
            };
            dtpFrom.Value = DateTime.Today.AddMonths(-3);
            filterGroup.Controls.Add(dtpFrom);

            filterGroup.Controls.Add(SmallLabel("To"));
            dtpTo = new DateTimePicker
            {
                Size = new Size(130, 26),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(4, 14, 10, 0)
            };
            filterGroup.Controls.Add(dtpTo);

            var btnSearch = MakeBtn("🔍  Search", Color.FromArgb(10, 14, 26), Color.White);
            btnSearch.Margin = new Padding(0, 11, 4, 0);
            btnSearch.Click += (s, e) => LoadPrices();
            filterGroup.Controls.Add(btnSearch);

            var btnReset = MakeBtn("↺  Reset", Color.FromArgb(230, 232, 242), Color.FromArgb(55, 68, 92));
            btnReset.Margin = new Padding(0, 11, 0, 0);
            btnReset.Click += (s, e) =>
            {
                cmbFilterType.SelectedIndex = 0;
                dtpFrom.Value = DateTime.Today.AddMonths(-3);
                dtpTo.Value = DateTime.Today;
                LoadPrices();
            };
            filterGroup.Controls.Add(btnReset);

            // CRUD group (right)
            var crudGroup = new FlowLayoutPanel
            {
                AutoSize = true,
                Height = 54,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            crudGroup.Controls.Add(new Panel
            {
                Width = 1,
                Height = 30,
                BackColor = Color.FromArgb(218, 222, 234),
                Margin = new Padding(0, 12, 8, 0)
            });
            var btnAdd = MakeBtn("➕  Add", Color.FromArgb(30, 155, 78), Color.White);
            var btnEdit = MakeBtn("✏  Edit", Color.FromArgb(36, 118, 195), Color.White);
            var btnDelete = MakeBtn("🗑  Delete", Color.FromArgb(180, 35, 35), Color.White);
            btnAdd.Margin = new Padding(0, 11, 5, 0);
            btnEdit.Margin = new Padding(0, 11, 5, 0);
            btnDelete.Margin = new Padding(0, 11, 0, 0);
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            crudGroup.Controls.Add(btnAdd);
            crudGroup.Controls.Add(btnEdit);
            crudGroup.Controls.Add(btnDelete);
            toolbar.Controls.Add(crudGroup);

            void PosCrud() =>
                crudGroup.Location = new Point(toolbar.ClientSize.Width - crudGroup.Width - 8, 0);
            toolbar.Resize += (s, e) => PosCrud();
            this.Load += (s, e) => PosCrud();

            root.Controls.Add(toolbar, 0, 1);

            // ── ROW 2: GRID + STATUS BAR ──────────────────────────
            var gridArea = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.FromArgb(242, 244, 250),
                Padding = new Padding(0),
                Margin = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            gridArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // grid
            gridArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f)); // status bar
            root.Controls.Add(gridArea, 0, 2);

            // Grid
            var gridWrap = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 10, 14, 4),
                BackColor = Color.FromArgb(242, 244, 250)
            };
            dgvPrices = BuildGrid();
            dgvPrices.Dock = DockStyle.Fill;
            gridWrap.Controls.Add(dgvPrices);
            gridArea.Controls.Add(gridWrap, 0, 0);

            // Status bar
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
            gridArea.Controls.Add(statusBar, 0, 1);
        }

        private void LoadPrices()
        {
            var prices = InMemoryStore.GetAllPrices(cmbFilterType.SelectedItem?.ToString(), dtpFrom.Value.Date, dtpTo.Value.Date);
            dgvPrices.Rows.Clear();
            dgvPrices.Columns.Clear();
            dgvPrices.Columns.Add("Id", "ID");
            dgvPrices.Columns.Add("FuelType", "Fuel Type");
            dgvPrices.Columns.Add("Price", "Price (₱)");
            dgvPrices.Columns.Add("EffectiveDate", "Effective Date");
            dgvPrices.Columns.Add("StationName", "Station");
            dgvPrices.Columns.Add("Notes", "Notes");
            dgvPrices.Columns.Add("CreatedAt", "Added On");
            foreach (var p in prices)
            {
                int r = dgvPrices.Rows.Add(p.Id, p.FuelType, $"₱{p.Price:F2}",
                    p.EffectiveDate.ToString("MMM dd, yyyy"), p.StationName, p.Notes,
                    p.CreatedAt.ToString("MMM dd, yyyy hh:mm tt"));
                dgvPrices.Rows[r].DefaultCellStyle.BackColor = FuelColor(p.FuelType);
            }
            lblStatus.Text = $"Showing {prices.Count} record(s)   ·   Last refreshed: {DateTime.Now:hh:mm tt}";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new PriceForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                InMemoryStore.AddPrice(form.FuelPrice);
                LoadPrices();
                MessageBox.Show("✅ Price added and users notified!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPrices.SelectedRows.Count == 0) { MessageBox.Show("Select a row to edit.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int id = Convert.ToInt32(dgvPrices.SelectedRows[0].Cells["Id"].Value);
            var selected = InMemoryStore.FuelPrices.Find(p => p.Id == id);
            if (selected == null) return;
            var form = new PriceForm(selected);
            if (form.ShowDialog() == DialogResult.OK)
            {
                form.FuelPrice.Id = id;
                InMemoryStore.UpdatePrice(form.FuelPrice);
                LoadPrices();
                MessageBox.Show("✅ Price updated and users notified!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPrices.SelectedRows.Count == 0) { MessageBox.Show("Select a row to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int id = Convert.ToInt32(dgvPrices.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Delete this record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                InMemoryStore.DeletePrice(id);
                LoadPrices();
            }
        }

        private Color FuelColor(string t) => t switch
        {
            "Gasoline" => Color.FromArgb(255, 252, 235),
            "Diesel" => Color.FromArgb(235, 245, 255),
            "Kerosene" => Color.FromArgb(240, 255, 240),
            _ => Color.White
        };

        private Button MakeBtn(string text, Color bg, Color fg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(92, 32),
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
            Margin = new Padding(0, 18, 10, 0)
        };

        private Label SmallLabel(string t) => new Label
        {
            Text = t,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(100, 112, 135),
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 17, 4, 0)
        };

        private DataGridView BuildGrid()
        {
            var g = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f),
                ColumnHeadersHeight = 42,
                GridColor = Color.FromArgb(228, 232, 244),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            g.RowTemplate.Height = 38;
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