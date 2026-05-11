using System;
using System.Drawing;
using System.Windows.Forms;
using EventManagementSystem.Models;
using EventManagementSystem.Repository;

namespace EventManagementSystem.Forms
{
    public class AdminDashboardForm : Form
    {
        private TabControl tabDash;
        private TabPage tabEvents, tabBookings, tabUsers;

        // Events tab
        private DataGridView dgvEvents;
        private Button btnAdd, btnEdit, btnDelete, btnRefreshEvents;

        // Bookings tab
        private DataGridView dgvBookings;
        private Button btnRefreshBookings;

        // Users tab
        private DataGridView dgvUsers;
        private Button btnRefreshUsers;

        private readonly EventRepository _eventRepo = new EventRepository();
        private readonly BookingRepository _bookingRepo = new BookingRepository();
        private readonly UserRepository _userRepo = new UserRepository();

        public AdminDashboardForm()
        {
            InitializeComponent();
            LoadEvents();
            LoadBookings();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "Event Management – Admin Panel";
            this.Size = new Size(950, 650);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 30, 48);

            // ── Header ──
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(180, 60, 0)
            };

            var lblTitle = new Label
            {
                Text = $"⚙  Admin Panel  |  {Session.CurrentUser.Name}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0)
            };

            var btnLogout = new Button
            {
                Text = "Logout",
                Dock = DockStyle.Right,
                Width = 80,
                BackColor = Color.FromArgb(80, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => { Session.Logout(); this.Close(); };

            header.Controls.Add(lblTitle);
            header.Controls.Add(btnLogout);
            this.Controls.Add(header);

            // ── Tabs ──
            tabDash = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Padding = new Point(15, 5)
            };

            tabEvents = new TabPage("  Manage Events  ");
            tabBookings = new TabPage("  All Bookings  ");
            tabUsers = new TabPage("  All Users  ");

            BuildEventsTab();
            BuildBookingsTab();
            BuildUsersTab();

            tabDash.TabPages.Add(tabEvents);
            tabDash.TabPages.Add(tabBookings);
            tabDash.TabPages.Add(tabUsers);

            this.Controls.Add(tabDash);
            tabDash.BringToFront();
        }

        // ─── Events Tab ───────────────────────────────────────────
        private void BuildEventsTab()
        {
            tabEvents.BackColor = Color.FromArgb(25, 37, 55);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(25, 37, 55)
            };

            btnAdd = MakeButton("➕ Add Event", 5, 7, 130, Color.FromArgb(0, 150, 80));
            btnEdit = MakeButton("✏ Edit Event", 143, 7, 130, Color.FromArgb(0, 100, 200));
            btnDelete = MakeButton("🗑 Delete Event", 281, 7, 130, Color.FromArgb(200, 60, 60));
            btnRefreshEvents = MakeButton("🔄 Refresh", 419, 7, 100, Color.FromArgb(80, 80, 120));

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefreshEvents.Click += (s, e) => LoadEvents();

            pnlTop.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefreshEvents });
            tabEvents.Controls.Add(pnlTop);

            dgvEvents = MakeGrid(Color.FromArgb(120, 40, 0));
            dgvEvents.Dock = DockStyle.Fill;
            dgvEvents.Columns.Add("EventID", "ID");
            dgvEvents.Columns.Add("Title", "Event Title");
            dgvEvents.Columns.Add("Date", "Date");
            dgvEvents.Columns.Add("Location", "Location");
            dgvEvents.Columns.Add("Capacity", "Capacity");
            dgvEvents.Columns.Add("Booked", "Booked");
            dgvEvents.Columns.Add("Available", "Available");
            dgvEvents.Columns.Add("Price", "Price (AED)");

            dgvEvents.Columns["EventID"].Width = 40;
            dgvEvents.Columns["Title"].Width = 200;
            dgvEvents.Columns["Date"].Width = 90;
            dgvEvents.Columns["Location"].Width = 175;
            dgvEvents.Columns["Capacity"].Width = 70;
            dgvEvents.Columns["Booked"].Width = 60;
            dgvEvents.Columns["Available"].Width = 70;
            dgvEvents.Columns["Price"].Width = 90;

            tabEvents.Controls.Add(dgvEvents);
        }

        // ─── Bookings Tab ─────────────────────────────────────────
        private void BuildBookingsTab()
        {
            tabBookings.BackColor = Color.FromArgb(25, 37, 55);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(25, 37, 55)
            };

            btnRefreshBookings = MakeButton("🔄 Refresh", 5, 7, 120, Color.FromArgb(80, 80, 120));
            btnRefreshBookings.Click += (s, e) => LoadBookings();
            pnlTop.Controls.Add(btnRefreshBookings);
            tabBookings.Controls.Add(pnlTop);

            dgvBookings = MakeGrid(Color.FromArgb(120, 40, 0));
            dgvBookings.Dock = DockStyle.Fill;
            dgvBookings.Columns.Add("BookingID", "Booking #");
            dgvBookings.Columns.Add("UserName", "User");
            dgvBookings.Columns.Add("EventTitle", "Event");
            dgvBookings.Columns.Add("Tickets", "Tickets");
            dgvBookings.Columns.Add("TotalPrice", "Total (AED)");
            dgvBookings.Columns.Add("BookingDate", "Date");

            dgvBookings.Columns["BookingID"].Width = 80;
            dgvBookings.Columns["UserName"].Width = 150;
            dgvBookings.Columns["EventTitle"].Width = 220;
            dgvBookings.Columns["Tickets"].Width = 65;
            dgvBookings.Columns["TotalPrice"].Width = 100;
            dgvBookings.Columns["BookingDate"].Width = 160;

            tabBookings.Controls.Add(dgvBookings);
        }

        // ─── Users Tab ────────────────────────────────────────────
        private void BuildUsersTab()
        {
            tabUsers.BackColor = Color.FromArgb(25, 37, 55);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(25, 37, 55)
            };

            btnRefreshUsers = MakeButton("🔄 Refresh", 5, 7, 120, Color.FromArgb(80, 80, 120));
            btnRefreshUsers.Click += (s, e) => LoadUsers();
            pnlTop.Controls.Add(btnRefreshUsers);
            tabUsers.Controls.Add(pnlTop);

            dgvUsers = MakeGrid(Color.FromArgb(120, 40, 0));
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.Columns.Add("UserID", "ID");
            dgvUsers.Columns.Add("Name", "Full Name");
            dgvUsers.Columns.Add("Email", "Email");
            dgvUsers.Columns.Add("Role", "Role");

            dgvUsers.Columns["UserID"].Width = 50;
            dgvUsers.Columns["Name"].Width = 200;
            dgvUsers.Columns["Email"].Width = 280;
            dgvUsers.Columns["Role"].Width = 100;

            tabUsers.Controls.Add(dgvUsers);
        }

        // ─── Data Loading ─────────────────────────────────────────
        private void LoadEvents()
        {
            dgvEvents.Rows.Clear();
            foreach (var ev in _eventRepo.GetAllEvents())
                dgvEvents.Rows.Add(
                    ev.EventID, ev.Title, ev.Date, ev.Location,
                    ev.Capacity, ev.BookedTickets, ev.AvailableSeats,
                    ev.Price.ToString("F2"));
        }

        private void LoadBookings()
        {
            dgvBookings.Rows.Clear();
            foreach (var b in _bookingRepo.GetAllBookings())
                dgvBookings.Rows.Add(
                    b.BookingID, b.UserName, b.EventTitle,
                    b.Tickets, b.TotalPrice.ToString("F2"), b.BookingDate);
        }

        private void LoadUsers()
        {
            dgvUsers.Rows.Clear();
            foreach (var u in _userRepo.GetAllUsers())
                dgvUsers.Rows.Add(u.UserID, u.Name, u.Email, u.Role);
        }

        // ─── CRUD Handlers ────────────────────────────────────────
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new EventEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _eventRepo.AddEvent(form.EventData);
                LoadEvents();
                MessageBox.Show("✅ Event added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEvents.CurrentRow == null) { MessageBox.Show("Select an event to edit."); return; }

            var ev = new Event
            {
                EventID = Convert.ToInt32(dgvEvents.CurrentRow.Cells["EventID"].Value),
                Title = dgvEvents.CurrentRow.Cells["Title"].Value.ToString(),
                Date = dgvEvents.CurrentRow.Cells["Date"].Value.ToString(),
                Location = dgvEvents.CurrentRow.Cells["Location"].Value.ToString(),
                Capacity = Convert.ToInt32(dgvEvents.CurrentRow.Cells["Capacity"].Value),
                Price = double.Parse(dgvEvents.CurrentRow.Cells["Price"].Value.ToString())
            };

            var form = new EventEditForm(ev);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _eventRepo.UpdateEvent(form.EventData);
                LoadEvents();
                MessageBox.Show("✅ Event updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEvents.CurrentRow == null) { MessageBox.Show("Select an event to delete."); return; }

            int id = Convert.ToInt32(dgvEvents.CurrentRow.Cells["EventID"].Value);
            string title = dgvEvents.CurrentRow.Cells["Title"].Value.ToString();

            var confirm = MessageBox.Show(
                $"Delete event '{title}'?\nThis will also delete all related bookings.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _eventRepo.DeleteEvent(id);
            LoadEvents();
            LoadBookings();
            MessageBox.Show("🗑 Event deleted.", "Done",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ─── Helpers ──────────────────────────────────────────────
        private DataGridView MakeGrid(Color headerColor)
        {
            return new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(30, 42, 62),
                GridColor = Color.FromArgb(60, 80, 110),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 42, 62),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(180, 60, 0),
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 9)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = headerColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
        }

        private Button MakeButton(string text, int x, int y, int w, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}