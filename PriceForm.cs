using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program
{
    public class PriceForm : Form
    {
        public FuelPrice FuelPrice { get; private set; }

        private ComboBox cmbFuelType;
        private TextBox txtPrice;
        private DateTimePicker dtpDate;
        private TextBox txtStation;
        private TextBox txtNotes;
        private readonly bool _isEdit;

        public PriceForm(FuelPrice existing)
        {
            _isEdit = existing != null;
            FuelPrice = existing ?? new FuelPrice { EffectiveDate = DateTime.Today };
            InitializeComponent();
            if (_isEdit) PopulateFields();
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Edit Fuel Price" : "Add Fuel Price";
            this.Size = new Size(450, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(247, 248, 252);
            this.Font = new Font("Segoe UI", 9.5f);

            // Root: header | body | footer using TableLayoutPanel
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0),
                Margin = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58f));   // Header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Body
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66f));   // Footer buttons
            this.Controls.Add(root);

            // ── HEADER ────────────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 14, 26), Margin = new Padding(0) };
            header.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 3, BackColor = Color.FromArgb(255, 200, 0) });
            header.Controls.Add(new Label
            {
                Text = _isEdit ? "✏  Edit Fuel Price" : "➕  Add Fuel Price",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(20, 22)
            });
            root.Controls.Add(header, 0, 0);

            // ── BODY ──────────────────────────────────────────────
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0) };
            root.Controls.Add(body, 0, 1);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(26, 18, 26, 8)
            };
            body.Controls.Add(flow);

            // Fuel Type
            flow.Controls.Add(FieldLabel("FUEL TYPE"));
            cmbFuelType = new ComboBox
            {
                Size = new Size(375, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(248, 249, 252),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 2, 0, 14)
            };
            cmbFuelType.Items.AddRange(new[] { "Gasoline", "Diesel", "Kerosene" });
            cmbFuelType.SelectedIndex = 0;
            flow.Controls.Add(cmbFuelType);

            // Price
            flow.Controls.Add(FieldLabel("PRICE (₱)"));
            txtPrice = MakeField(375);
            flow.Controls.Add(txtPrice);

            // Effective Date
            flow.Controls.Add(FieldLabel("EFFECTIVE DATE"));
            dtpDate = new DateTimePicker
            {
                Size = new Size(375, 32),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 2, 0, 14)
            };
            flow.Controls.Add(dtpDate);

            // Station
            flow.Controls.Add(FieldLabel("STATION NAME"));
            txtStation = MakeField(375);
            flow.Controls.Add(txtStation);

            // Notes
            flow.Controls.Add(FieldLabel("NOTES  (optional)"));
            txtNotes = new TextBox
            {
                Size = new Size(375, 52),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 252),
                Margin = new Padding(0, 2, 0, 0)
            };
            flow.Controls.Add(txtNotes);

            // ── FOOTER ────────────────────────────────────────────
            var footer = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(247, 248, 252), Margin = new Padding(0) };
            footer.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 224, 236)), 0, 0, footer.Width, 0);
            root.Controls.Add(footer, 0, 2);

            var btnSave = new Button
            {
                Text = _isEdit ? "💾  Update Price" : "✅  Add Price",
                Location = new Point(26, 14),
                Size = new Size(186, 38),
                BackColor = Color.FromArgb(30, 155, 78),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(38, 178, 92);
            btnSave.Click += BtnSave_Click;
            footer.Controls.Add(btnSave);

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(226, 14),
                Size = new Size(198, 38),
                BackColor = Color.FromArgb(230, 232, 242),
                ForeColor = Color.FromArgb(55, 68, 92),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            footer.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private Label FieldLabel(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(115, 128, 155),
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 2)
        };

        private TextBox MakeField(int width) => new TextBox
        {
            Size = new Size(width, 32),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(248, 249, 252),
            Margin = new Padding(0, 2, 0, 14)
        };

        private void PopulateFields()
        {
            cmbFuelType.SelectedItem = FuelPrice.FuelType;
            txtPrice.Text = FuelPrice.Price.ToString("F2");
            dtpDate.Value = FuelPrice.EffectiveDate;
            txtStation.Text = FuelPrice.StationName;
            txtNotes.Text = FuelPrice.Notes;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbFuelType.SelectedItem == null || string.IsNullOrWhiteSpace(txtPrice.Text))
            { MessageBox.Show("Fuel type and price are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); this.DialogResult = DialogResult.None; return; }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            { MessageBox.Show("Please enter a valid price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); this.DialogResult = DialogResult.None; return; }

            FuelPrice = new FuelPrice
            {
                Id = FuelPrice.Id,
                FuelType = cmbFuelType.SelectedItem.ToString(),
                Price = price,
                EffectiveDate = dtpDate.Value.Date,
                StationName = txtStation.Text.Trim(),
                Notes = txtNotes.Text.Trim()
            };
        }
    }
}