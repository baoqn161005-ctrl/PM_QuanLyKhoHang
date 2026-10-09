using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.GUI.Common;

namespace QLKhoHang.GUI.Forms.Main
{
    /// <summary>
    /// Thay server name cho Windows user hiện tại sau khi kết nối thử thành công.
    /// </summary>
    public class frmKetNoiCSDL : Form
    {
        private readonly CauHinhCSDL_BUS _cauHinhBUS = new CauHinhCSDL_BUS();
        private readonly TextBox txtServerName = new TextBox();
        private readonly Label lblKetQua = new Label();
        private readonly Button btnKiemTraVaLuu = new Button();

        public frmKetNoiCSDL()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Cấu hình kết nối cơ sở dữ liệu";
            Name = "frmKetNoiCSDL";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(560, 260);
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            Label lblTieuDe = new Label
            {
                Name = "lblTieuDeKetNoi",
                Text = "KẾT NỐI SQL SERVER",
                Location = new Point(28, 20),
                Size = new Size(500, 34),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold)
            };
            Label lblHuongDan = new Label
            {
                Name = "lblHuongDanKetNoi",
                Text = "Nhập Server name như trong SSMS. Ứng dụng dùng Windows Authentication và database QLKhoHang.",
                Location = new Point(30, 62),
                Size = new Size(500, 42),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
            Label lblServer = new Label
            {
                Name = "lblServerName",
                Text = "Server name",
                Location = new Point(30, 117),
                Size = new Size(130, 25)
            };
            txtServerName.Name = "txtServerName";
            txtServerName.Location = new Point(160, 113);
            txtServerName.Size = new Size(365, 30);
            txtServerName.MaxLength = 128;
            txtServerName.Text = _cauHinhBUS.LayServerNameHienTai();

            lblKetQua.Name = "lblKetQuaKetNoi";
            lblKetQua.Text = "Server mới chỉ được lưu sau khi kết nối thử thành công.";
            lblKetQua.Location = new Point(30, 154);
            lblKetQua.Size = new Size(500, 38);
            lblKetQua.ForeColor = Color.FromArgb(71, 85, 105);
            lblKetQua.Font = new Font("Segoe UI", 9F, FontStyle.Italic);

            btnKiemTraVaLuu.Name = "btnKiemTraVaLuuServer";
            btnKiemTraVaLuu.Text = "KIỂM TRA VÀ LƯU";
            btnKiemTraVaLuu.Location = new Point(322, 205);
            btnKiemTraVaLuu.Size = new Size(153, 36);
            btnKiemTraVaLuu.BackColor = Color.FromArgb(13, 148, 136);
            btnKiemTraVaLuu.ForeColor = Color.White;
            btnKiemTraVaLuu.FlatStyle = FlatStyle.Flat;
            btnKiemTraVaLuu.FlatAppearance.BorderSize = 0;
            btnKiemTraVaLuu.Click += BtnKiemTraVaLuu_Click;

            Button btnDong = new Button
            {
                Name = "btnDongKetNoi",
                Text = "ĐÓNG",
                Location = new Point(485, 205),
                Size = new Size(70, 36),
                FlatStyle = FlatStyle.Flat
            };
            btnDong.Click += delegate { Close(); };

            Controls.Add(lblTieuDe);
            Controls.Add(lblHuongDan);
            Controls.Add(lblServer);
            Controls.Add(txtServerName);
            Controls.Add(lblKetQua);
            Controls.Add(btnKiemTraVaLuu);
            Controls.Add(btnDong);
            AcceptButton = btnKiemTraVaLuu;
        }

        private async void BtnKiemTraVaLuu_Click(object sender, EventArgs e)
        {
            btnKiemTraVaLuu.Enabled = false;
            UseWaitCursor = true;
            lblKetQua.Text = "Đang kiểm tra kết nối...";
            try
            {
                string serverName = txtServerName.Text.Trim();
                string databaseName = await Task.Run(() => _cauHinhBUS.KiemTraVaLuu(
                    SessionManager.NguoiDungHienTai, serverName));
                lblKetQua.Text = "Đã kết nối database " + databaseName + " và lưu server cho user Windows hiện tại.";
                lblKetQua.ForeColor = Color.FromArgb(13, 116, 88);
                MessageBox.Show("Kết nối thành công tới " + serverName + " / " + databaseName + ".",
                    "Đã kết nối", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                HienThiLoi(ex.Message);
            }
            catch (ArgumentException ex)
            {
                HienThiLoi(ex.Message);
            }
            catch (SqlException ex)
            {
                HienThiLoi("SQL Server từ chối kết nối. Kiểm tra tên instance, database và quyền Windows. " + ex.Message);
            }
            catch (ConfigurationErrorsException ex)
            {
                HienThiLoi("Không đọc được cấu hình ứng dụng. " + ex.Message);
            }
            catch (IOException ex)
            {
                HienThiLoi("Không lưu được cấu hình riêng cho user Windows. " + ex.Message);
            }
            finally
            {
                UseWaitCursor = false;
                btnKiemTraVaLuu.Enabled = true;
            }
        }

        private void HienThiLoi(string message)
        {
            lblKetQua.Text = message;
            lblKetQua.ForeColor = Color.FromArgb(185, 28, 28);
            MessageBox.Show(message, "Không thể kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
