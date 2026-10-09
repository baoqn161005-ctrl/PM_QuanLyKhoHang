using System;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
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

        public frmDangNhap()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                NapTaiKhoanDaGhiNho();
            }
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

        private void BtnThoat_Click(object sender, EventArgs e)
        {
            Close();
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
