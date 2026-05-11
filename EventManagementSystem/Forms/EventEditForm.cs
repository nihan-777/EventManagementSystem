using System;
using System.Drawing;
using System.Windows.Forms;
using EventManagementSystem.Models;

namespace EventManagementSystem.Forms
{
    /// <summary>
    /// Modal dialog for adding or editing an event.
    /// Returns DialogResult.OK when the admin confirms.
    /// </summary>
    public class EventEditForm : Form
    {
        public Event EventData { get; private set; }

        private TextBox txtTitle, txtDate, txtLocation, txtCapacity, txtPrice;
        private Button btnSave, btnCancel;

        private readonly bool _isEdit;

        public EventEditForm(Event existingEvent)
        {
            _isEdit = existingEvent != null;
            EventData = existingEvent ?? new Event();
            InitializeComponent();

            if (_isEdit)
            {
                txtTitle.Text = EventData.Title;
                txtDate.Text = EventData.Date;
                txtLocation.Text = EventData.Location;
                txtCapacity.Text = EventData.Capacity.ToString();
                txtPrice.Text = EventData.Price.ToString("F2");
            }
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Edit Event" : "Add New Event";
            this.Size = new Size(400, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(25, 37, 55);

            int lx = 15, tx = 160, w = 210, row = 20, gap = 45;

            AddLabel("Event Title:", lx, row);
            txtTitle = AddTextBox(tx, row, w); row += gap;

            AddLabel("Date (YYYY-MM-DD):", lx, row);
            txtDate = AddTextBox(tx, row, w); row += gap;

            AddLabel("Location:", lx, row);
            txtLocation = AddTextBox(tx, row, w); row += gap;

            AddLabel("Capacity:", lx, row);
            txtCapacity = AddTextBox(tx, row, 80); row += gap;

            AddLabel("Price (AED):", lx, row);
            txtPrice = AddTextBox(tx, row, 100); row += gap;

            btnSave = new Button
            {
                Text = _isEdit ? "💾 Save Changes" : "➕ Add Event",
                Location = new Point(15, row),
                Size = new Size(170, 35),
                BackColor = Color.FromArgb(0, 120, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(200, row),
                Size = new Size(170, 35),
                BackColor = Color.FromArgb(100, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtDate.Text) ||
                string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int cap) || cap <= 0)
            { MessageBox.Show("Capacity must be a positive number."); return; }

            if (!double.TryParse(txtPrice.Text, out double price) || price < 0)
            { MessageBox.Show("Price must be a valid non-negative number."); return; }

            EventData.Title = txtTitle.Text.Trim();
            EventData.Date = txtDate.Text.Trim();
            EventData.Location = txtLocation.Text.Trim();
            EventData.Capacity = cap;
            EventData.Price = price;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y + 5),
                Size = new Size(140, 22),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 9)
            });
        }

        private TextBox AddTextBox(int x, int y, int w)
        {
            var tb = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 26),
                BackColor = Color.FromArgb(45, 60, 85),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(tb);
            return tb;
        }
    }
}
