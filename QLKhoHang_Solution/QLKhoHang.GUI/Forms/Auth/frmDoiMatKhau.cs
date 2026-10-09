using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.GUI.Common;

namespace QLKhoHang.GUI.Forms.Auth
{
    /// <summary>
    /// Đổi mật khẩu của tài khoản đang đăng nhập.
    /// </summary>
    public class frmDoiMatKhau : Form
    {
        private readonly NhanVienBUS _nhanVienBUS = new NhanVienBUS();
        private readonly TextBox txtMatKhauCu = new TextBox();
        private readonly TextBox txtMatKhauMoi = new TextBox();
        private readonly TextBox txtXacNhanMatKhau = new TextBox();

        public frmDoiMatKhau()
        {
            if (!SessionManager.DaDangNhap)
            {
                throw new UnauthorizedAccessException("Cần đăng nhập trước khi đổi mật khẩu.");
            }

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Đổi mật khẩu";
            Name = "frmDoiMatKhau";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(460, 330);
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            Label lblTieuDe = new Label
            {
                Text = "ĐỔI MẬT KHẨU",
                Dock = DockStyle.Top,
                Height = 58,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold)
            };

            txtMatKhauCu.Name = "txtMatKhauCu";
            txtMatKhauMoi.Name = "txtMatKhauMoi";
            txtXacNhanMatKhau.Name = "txtXacNhanMatKhau";
            AddField("lblMatKhauCu", "Mật khẩu hiện tại", txtMatKhauCu, 72);
            AddField("lblMatKhauMoi", "Mật khẩu mới (ít nhất 8 ký tự)", txtMatKhauMoi, 132);
            AddField("lblXacNhanMatKhau", "Xác nhận mật khẩu mới", txtXacNhanMatKhau, 192);

            Button btnLuu = new Button
            {
                Name = "btnLuuMatKhau",
                Text = "CẬP NHẬT",
                Location = new Point(226, 263),
                Size = new Size(112, 38),
                BackColor = Color.FromArgb(13, 148, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            Button btnDong = new Button
            {
                Name = "btnDong",
                Text = "ĐÓNG",
                Location = new Point(346, 263),
                Size = new Size(82, 38),
                FlatStyle = FlatStyle.Flat
            };
            btnDong.Click += delegate { Close(); };

            Controls.Add(btnDong);
            Controls.Add(btnLuu);
            Controls.Add(lblTieuDe);
        }

        private void AddField(string labelName, string caption, TextBox textBox, int top)
        {
            Label label = new Label
            {
                Name = labelName,
                Text = caption,
                Location = new Point(30, top),
                Size = new Size(190, 24),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
            textBox.Location = new Point(226, top - 2);
            textBox.Size = new Size(202, 29);
            textBox.UseSystemPasswordChar = true;
            textBox.MaxLength = 255;
            Controls.Add(textBox);
            Controls.Add(label);
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                _nhanVienBUS.DoiMatKhau(
                    SessionManager.NguoiDungHienTai,
                    txtMatKhauCu.Text,
                    txtMatKhauMoi.Text,
                    txtXacNhanMatKhau.Text);

                MessageBox.Show("Đổi mật khẩu thành công.", "Hoàn tất",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Kiểm tra thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không thể đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                MessageBox.Show("Không kết nối được SQL Server để cập nhật mật khẩu.",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Không đọc được connection string trong App.config.",
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
