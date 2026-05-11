using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EventManagementSystem.Models;
using EventManagementSystem.Repository;

namespace EventManagementSystem.Forms
{
    public class UserDashboardForm : Form
    {
        private TabControl tabDash;
        private TabPage tabEvents, tabMyBookings;
        private DataGridView dgvEvents;
        private TextBox txtSearch;
        private Button btnSearch, btnBook;
        private Label lblWelcome;
        private DataGridView dgvBookings;
        private Button btnCancel, btnRefreshBookings;

        private readonly EventRepository _eventRepo = new EventRepository();
        private readonly BookingRepository _bookingRepo = new BookingRepository();

        public UserDashboardForm()
        {
            InitializeComponent();
            LoadEvents();
            LoadMyBookings();
        }

        private void InitializeComponent()
        {
            this.Text = "Event Management – User Dashboard";
            this.Size = new Size(950, 650);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 30, 48);

            // ── Header ──
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215)
            };

            lblWelcome = new Label
            {
                Text = $"🎟  Welcome, {Session.CurrentUser.Name}",
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
                BackColor = Color.FromArgb(200, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => { Session.Logout(); this.Close(); };
            header.Controls.Add(lblWelcome);
            header.Controls.Add(btnLogout);
            this.Controls.Add(header);

            // ── Tab Control fills the rest of the form ──
            tabDash = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Padding = new Point(15, 5)
            };

            tabEvents = new TabPage("  Browse Events  ");
            tabMyBookings = new TabPage("  My Bookings  ");

            BuildEventsTab();
            BuildMyBookingsTab();

            tabDash.TabPages.Add(tabEvents);
            tabDash.TabPages.Add(tabMyBookings);

            // Add TabControl AFTER header
            this.Controls.Add(tabDash);
            tabDash.BringToFront();
        }

        private void BuildEventsTab()
        {
            tabEvents.BackColor = Color.FromArgb(25, 37, 55);

            // ── Search bar panel ──
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(25, 37, 55),
                Padding = new Padding(5, 7, 5, 0)
            };

            txtSearch = new TextBox
            {
                Location = new Point(5, 8),
                Size = new Size(480, 28),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 60, 85),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter) LoadEvents(txtSearch.Text.Trim());
            };

            btnSearch = MakeButton("🔍 Search", 493, 6, 110, Color.FromArgb(0, 120, 215));
            btnSearch.Click += (s, e) => LoadEvents(txtSearch.Text.Trim());

            btnBook = MakeButton("📋 Book Tickets", 611, 6, 130, Color.FromArgb(0, 160, 80));
            btnBook.Click += BtnBook_Click;

            var btnRefresh = MakeButton("🔄 Refresh", 749, 6, 100, Color.FromArgb(80, 80, 120));
            btnRefresh.Click += (s, e) => LoadEvents();

            pnlTop.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnBook, btnRefresh });
            tabEvents.Controls.Add(pnlTop);

            // ── Events Grid ──
            dgvEvents = MakeGrid();
            dgvEvents.Dock = DockStyle.Fill;
            dgvEvents.Columns.Add("EventID", "ID");
            dgvEvents.Columns.Add("Title", "Event Name");
            dgvEvents.Columns.Add("Date", "Date");
            dgvEvents.Columns.Add("Location", "Location");
            dgvEvents.Columns.Add("Available", "Seats Left");
            dgvEvents.Columns.Add("Capacity", "Capacity");
            dgvEvents.Columns.Add("Price", "Price (AED)");

            dgvEvents.Columns["EventID"].Width = 40;
            dgvEvents.Columns["Title"].Width = 200;
            dgvEvents.Columns["Date"].Width = 95;
            dgvEvents.Columns["Location"].Width = 190;
            dgvEvents.Columns["Available"].Width = 80;
            dgvEvents.Columns["Capacity"].Width = 75;
            dgvEvents.Columns["Price"].Width = 90;

            tabEvents.Controls.Add(dgvEvents);
        }

        private void BuildMyBookingsTab()
        {
            tabMyBookings.BackColor = Color.FromArgb(25, 37, 55);

            // ── Buttons panel ──
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(25, 37, 55)
            };

            btnRefreshBookings = MakeButton("🔄 Refresh", 5, 7, 120, Color.FromArgb(0, 120, 215));
            btnRefreshBookings.Click += (s, e) => LoadMyBookings();

            btnCancel = MakeButton("❌ Cancel Booking", 133, 7, 150, Color.FromArgb(200, 60, 60));
            btnCancel.Click += BtnCancelBooking_Click;

            pnlTop.Controls.AddRange(new Control[] { btnRefreshBookings, btnCancel });
            tabMyBookings.Controls.Add(pnlTop);

            // ── Bookings Grid ──
            dgvBookings = MakeGrid();
            dgvBookings.Dock = DockStyle.Fill;
            dgvBookings.Columns.Add("BookingID", "Booking #");
            dgvBookings.Columns.Add("EventTitle", "Event");
            dgvBookings.Columns.Add("Tickets", "Tickets");
            dgvBookings.Columns.Add("TotalPrice", "Total (AED)");
            dgvBookings.Columns.Add("BookingDate", "Booked On");

            dgvBookings.Columns["BookingID"].Width = 90;
            dgvBookings.Columns["EventTitle"].Width = 260;
            dgvBookings.Columns["Tickets"].Width = 70;
            dgvBookings.Columns["TotalPrice"].Width = 110;
            dgvBookings.Columns["BookingDate"].Width = 180;

            tabMyBookings.Controls.Add(dgvBookings);
        }

        private void LoadEvents(string keyword = "")
        {
            dgvEvents.Rows.Clear();
            var events = string.IsNullOrWhiteSpace(keyword)
                ? _eventRepo.GetAllEvents()
                : _eventRepo.SearchEvents(keyword);

            foreach (var ev in events)
            {
                dgvEvents.Rows.Add(
                    ev.EventID, ev.Title, ev.Date, ev.Location,
                    ev.AvailableSeats, ev.Capacity, ev.Price.ToString("F2"));

                if (ev.AvailableSeats <= 0)
                    dgvEvents.Rows[dgvEvents.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Salmon;
            }
        }

        private void LoadMyBookings()
        {
            dgvBookings.Rows.Clear();
            var bookings = _bookingRepo.GetBookingsByUser(Session.CurrentUser.UserID);
            foreach (var b in bookings)
                dgvBookings.Rows.Add(
                    b.BookingID, b.EventTitle, b.Tickets,
                    b.TotalPrice.ToString("F2"), b.BookingDate);
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (dgvEvents.CurrentRow == null) { MessageBox.Show("Please select an event first."); return; }

            int eventId = Convert.ToInt32(dgvEvents.CurrentRow.Cells["EventID"].Value);
            int available = Convert.ToInt32(dgvEvents.CurrentRow.Cells["Available"].Value);
            string title = dgvEvents.CurrentRow.Cells["Title"].Value.ToString();
            double price = double.Parse(dgvEvents.CurrentRow.Cells["Price"].Value.ToString());

            if (available <= 0) { MessageBox.Show("This event is sold out!"); return; }

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Event: {title}\nAvailable: {available} seats\nPrice: AED {price:F2} per ticket\n\nHow many tickets?",
                "Book Tickets", "1");

            if (!int.TryParse(input, out int tickets) || tickets <= 0)
            { MessageBox.Show("Enter a valid number of tickets."); return; }

            if (tickets > available)
            { MessageBox.Show($"Only {available} seats available."); return; }

            double total = tickets * price;
            var confirm = MessageBox.Show(
                $"Confirm Booking?\n\nEvent: {title}\nTickets: {tickets}\nTotal: AED {total:F2}",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            int bookingId = _bookingRepo.CreateBooking(
                Session.CurrentUser.UserID, eventId, tickets, price);

            if (bookingId > 0)
            {
                MessageBox.Show(
                    $"✅ Booking Confirmed!\nBooking ID: #{bookingId}\nTotal: AED {total:F2}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEvents();
                LoadMyBookings();
                // Switch to My Bookings tab automatically
                tabDash.SelectedTab = tabMyBookings;
            }
            else MessageBox.Show("Booking failed. Please try again.");
        }

        private void BtnCancelBooking_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow == null) { MessageBox.Show("Please select a booking."); return; }

            int bookingId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["BookingID"].Value);
            string title = dgvBookings.CurrentRow.Cells["EventTitle"].Value.ToString();

            var confirm = MessageBox.Show(
                $"Cancel booking #{bookingId} for '{title}'?",
                "Cancel Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _bookingRepo.CancelBooking(bookingId);
            MessageBox.Show("Booking cancelled successfully.", "Done",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadMyBookings();
            LoadEvents();
        }

        private DataGridView MakeGrid()
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
                    SelectionBackColor = Color.FromArgb(0, 120, 215),
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 9)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(0, 90, 160),
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
