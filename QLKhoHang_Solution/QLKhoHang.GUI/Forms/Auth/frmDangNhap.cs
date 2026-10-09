using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.DTO;
using QLKhoHang.GUI.Common;
using QLKhoHang.GUI.Forms.Main;

namespace QLKhoHang.GUI.Forms.Auth
{
    /// <summary>
    /// Đăng nhập bằng thông tin nhân viên trong cơ sở dữ liệu.
    /// </summary>
    public partial class frmDangNhap : Form
    {
        private readonly NhanVienBUS _nhanVienBUS = new NhanVienBUS();
        private readonly TextBox txtTaiKhoan = new TextBox();
        private readonly TextBox txtMatKhau = new TextBox();
        private readonly ComboBox cboVaiTro = new ComboBox();
        private readonly CheckBox chkGhiNhoTaiKhoan = new CheckBox();
        private readonly Button btnDangNhap = new Button();
        private readonly Button btnThoat = new Button();

        public frmDangNhap()
        {
            InitializeComponent();
            NapTaiKhoanDaGhiNho();
        }

        private void InitializeComponent()
        {
            Text = "Đăng nhập - QLKhoHang";
            Name = "frmDangNhap";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(420, 480);
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;

            Panel pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 132,
                BackColor = Color.FromArgb(30, 41, 59)
            };
            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "QUẢN LÝ KHO HÀNG",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 76
            };
            Label lblMoTa = new Label
            {
                Name = "lblMoTa",
                Text = "Đăng nhập để truy cập hệ thống",
                ForeColor = Color.FromArgb(226, 232, 240),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill
            };
            pnlHeader.Controls.Add(lblMoTa);
            pnlHeader.Controls.Add(lblTieuDe);

            Label lblTaiKhoan = TaoNhan("Tài khoản", 154);
            lblTaiKhoan.Name = "lblTaiKhoan";
            txtTaiKhoan.Name = "txtTaiKhoan";
            DatViTri(txtTaiKhoan, 179, 350);
            txtTaiKhoan.MaxLength = 50;

            Label lblMatKhau = TaoNhan("Mật khẩu", 221);
            lblMatKhau.Name = "lblMatKhau";
            txtMatKhau.Name = "txtMatKhau";
            DatViTri(txtMatKhau, 246, 350);
            txtMatKhau.MaxLength = 255;
            txtMatKhau.UseSystemPasswordChar = true;

            Label lblVaiTro = TaoNhan("Vai trò", 288);
            lblVaiTro.Name = "lblVaiTro";
            cboVaiTro.Name = "cboVaiTro";
            cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVaiTro.Items.AddRange(new object[] { "Quản lý kho", "Thủ kho", "Kế toán kho" });
            DatViTri(cboVaiTro, 313, 350);
            cboVaiTro.SelectedIndex = 0;

            chkGhiNhoTaiKhoan.Name = "chkGhiNhoTaiKhoan";
            chkGhiNhoTaiKhoan.Text = "Ghi nhớ tài khoản trên máy này";
            chkGhiNhoTaiKhoan.AutoSize = true;
            chkGhiNhoTaiKhoan.Location = new Point(34, 357);

            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Text = "ĐĂNG NHẬP";
            btnDangNhap.BackColor = Color.FromArgb(13, 148, 136);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.Location = new Point(34, 399);
            btnDangNhap.Size = new Size(210, 42);
            btnDangNhap.Click += BtnDangNhap_Click;

            btnThoat.Name = "btnThoat";
            btnThoat.Text = "THOÁT";
            btnThoat.BackColor = Color.FromArgb(226, 232, 240);
            btnThoat.ForeColor = Color.FromArgb(30, 41, 59);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.FlatAppearance.BorderSize = 0;
            btnThoat.Location = new Point(254, 399);
            btnThoat.Size = new Size(130, 42);
            btnThoat.Click += delegate { Close(); };

            Controls.Add(btnThoat);
            Controls.Add(btnDangNhap);
            Controls.Add(chkGhiNhoTaiKhoan);
            Controls.Add(cboVaiTro);
            Controls.Add(lblVaiTro);
            Controls.Add(txtMatKhau);
            Controls.Add(lblMatKhau);
            Controls.Add(txtTaiKhoan);
            Controls.Add(lblTaiKhoan);
            Controls.Add(pnlHeader);
        }

        private static Label TaoNhan(string text, int top)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(35, top),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
        }

        private static void DatViTri(Control control, int top, int width)
        {
            control.Location = new Point(35, top);
            control.Size = new Size(width, 31);
        }

        private void NapTaiKhoanDaGhiNho()
        {
            try
            {
                txtTaiKhoan.Text = Properties.Settings.Default.TaiKhoanGanNhat ?? string.Empty;
                chkGhiNhoTaiKhoan.Checked = !string.IsNullOrWhiteSpace(txtTaiKhoan.Text);
            }
            catch (ConfigurationErrorsException)
            {
                txtTaiKhoan.Clear();
                chkGhiNhoTaiKhoan.Checked = false;
            }
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                NhanVienDTO nhanVien = _nhanVienBUS.DangNhap(txtTaiKhoan.Text, txtMatKhau.Text);
                if (!string.Equals(nhanVien.VaiTro, Convert.ToString(cboVaiTro.SelectedItem), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Vai trò đã chọn không khớp với tài khoản.", "Không thể đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LuuTaiKhoanDaGhiNho();
                SessionManager.DangNhap(nhanVien);
                Hide();
                using (frmMain mainForm = new frmMain())
                {
                    mainForm.ShowDialog(this);
                }

                txtMatKhau.Clear();
                Show();
                Activate();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Clear();
                txtMatKhau.Focus();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                MessageBox.Show("Không kết nối được SQL Server. Kiểm tra instance, database và quyền Windows của tài khoản.",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Không đọc được cấu hình ứng dụng. Kiểm tra connection string trong App.config.",
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LuuTaiKhoanDaGhiNho()
        {
            try
            {
                Properties.Settings.Default.TaiKhoanGanNhat = chkGhiNhoTaiKhoan.Checked
                    ? txtTaiKhoan.Text.Trim()
                    : string.Empty;
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationErrorsException)
            {
                // Lỗi lưu tùy chọn này không làm thay đổi kết quả đăng nhập.
            }
        }
    }
}
