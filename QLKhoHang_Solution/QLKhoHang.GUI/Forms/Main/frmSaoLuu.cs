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
    /// Backup database hiện cấu hình; restore chỉ tạo database test mới, không thay thế database nguồn.
    /// </summary>
    public class frmSaoLuu : Form
    {
        private readonly SaoLuuBUS _saoLuuBUS = new SaoLuuBUS();
        private readonly Label lblDatabase = new Label();
        private readonly Label lblTrangThai = new Label();
        private readonly Button btnSaoLuu = new Button();
        private readonly Button btnKhoiPhuc = new Button();

        public frmSaoLuu()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Sao lưu và khôi phục dữ liệu";
            Name = "frmSaoLuu";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(620, 350);
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            Label lblTieuDe = new Label
            {
                Name = "lblTieuDeSaoLuu",
                Text = "SAO LƯU / KHÔI PHỤC DATABASE",
                Location = new Point(28, 20),
                Size = new Size(560, 38),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold)
            };
            Label lblDatabaseTitle = new Label
            {
                Name = "lblDatabaseTitle",
                Text = "Database theo App.config:",
                Location = new Point(32, 77),
                Size = new Size(190, 26)
            };
            lblDatabase.Name = "lblDatabaseNguon";
            lblDatabase.Text = _saoLuuBUS.LayDatabaseDangCauHinh();
            lblDatabase.Location = new Point(225, 77);
            lblDatabase.Size = new Size(350, 26);
            lblDatabase.ForeColor = Color.FromArgb(13, 116, 88);
            lblDatabase.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            Label lblCanhBao = new Label
            {
                Name = "lblCanhBaoSaoLuu",
                Text = "Backup ghi file do dịch vụ SQL Server truy cập được. Restore chỉ tạo database mới có tên QLKhoHang_TestRestore_...; không ghi đè database hiện cấu hình.",
                Location = new Point(32, 116),
                Size = new Size(555, 68),
                ForeColor = Color.FromArgb(153, 27, 27),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            btnSaoLuu.Name = "btnSaoLuuDatabase";
            btnSaoLuu.Text = "SAO LƯU DATABASE";
            btnSaoLuu.Location = new Point(32, 205);
            btnSaoLuu.Size = new Size(245, 48);
            btnSaoLuu.BackColor = Color.FromArgb(37, 99, 235);
            btnSaoLuu.ForeColor = Color.White;
            btnSaoLuu.FlatStyle = FlatStyle.Flat;
            btnSaoLuu.FlatAppearance.BorderSize = 0;
            btnSaoLuu.Click += BtnSaoLuu_Click;

            btnKhoiPhuc.Name = "btnKhoiPhucDatabaseTest";
            btnKhoiPhuc.Text = "KHÔI PHỤC SANG DB TEST MỚI";
            btnKhoiPhuc.Location = new Point(296, 205);
            btnKhoiPhuc.Size = new Size(291, 48);
            btnKhoiPhuc.BackColor = Color.FromArgb(220, 38, 38);
            btnKhoiPhuc.ForeColor = Color.White;
            btnKhoiPhuc.FlatStyle = FlatStyle.Flat;
            btnKhoiPhuc.FlatAppearance.BorderSize = 0;
            btnKhoiPhuc.Click += BtnKhoiPhuc_Click;

            lblTrangThai.Name = "lblTrangThaiSaoLuu";
            lblTrangThai.Text = "Chưa thực hiện thao tác.";
            lblTrangThai.Location = new Point(32, 275);
            lblTrangThai.Size = new Size(555, 42);
            lblTrangThai.ForeColor = Color.FromArgb(71, 85, 105);
            lblTrangThai.Font = new Font("Segoe UI", 9F, FontStyle.Italic);

            Controls.Add(lblTieuDe);
            Controls.Add(lblDatabaseTitle);
            Controls.Add(lblDatabase);
            Controls.Add(lblCanhBao);
            Controls.Add(btnSaoLuu);
            Controls.Add(btnKhoiPhuc);
            Controls.Add(lblTrangThai);
        }

        private async void BtnSaoLuu_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Chọn file backup mới";
                dialog.Filter = "SQL Server backup (*.bak)|*.bak";
                dialog.DefaultExt = "bak";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.FileName = "QLKhoHang_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                DialogResult confirmation = MessageBox.Show(
                    "Tạo backup của database " + lblDatabase.Text + " tại:\n" + dialog.FileName +
                    "\n\nĐảm bảo SQL Server service có quyền ghi vào đường dẫn này. Tiếp tục?",
                    "Xác nhận backup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmation != DialogResult.Yes)
                {
                    return;
                }

                string path = dialog.FileName;
                await RunOperationAsync("Đang sao lưu...", () =>
                {
                    _saoLuuBUS.SaoLuu(SessionManager.NguoiDungHienTai, path);
                    return "Backup và VERIFYONLY thành công: " + path;
                });
            }
        }

        private async void BtnKhoiPhuc_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn file backup cần khôi phục";
                dialog.Filter = "SQL Server backup (*.bak)|*.bak|Tất cả file (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                DialogResult confirmation = MessageBox.Show(
                    "File sẽ được kiểm tra và khôi phục thành một database test mới có tên QLKhoHang_TestRestore_... .\n" +
                    "Database QLKhoHang hiện tại không bị thay thế. SQL Server cần quyền tạo database và quyền đọc file backup.\n\nTiếp tục?",
                    "Xác nhận restore an toàn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmation != DialogResult.Yes)
                {
                    return;
                }

                string path = dialog.FileName;
                await RunOperationAsync("Đang VERIFYONLY và restore sang database test mới...", () =>
                {
                    string databaseName = _saoLuuBUS.KhoiPhucSangDatabaseTestMoi(
                        SessionManager.NguoiDungHienTai, path);
                    return "Restore thành công sang database test mới: " + databaseName;
                });
            }
        }

        private async Task RunOperationAsync(string progress, Func<string> operation)
        {
            SetBusy(true);
            lblTrangThai.Text = progress;
            try
            {
                string result = await Task.Run(operation);
                lblTrangThai.Text = result;
                MessageBox.Show(result, "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                ShowError("SQL Server không thể thực hiện thao tác. Kiểm tra quyền, trạng thái database và đường dẫn mà SQL Server service truy cập được. " + ex.Message);
            }
            catch (IOException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                ShowError(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                ShowError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (ConfigurationErrorsException ex)
            {
                ShowError("Không đọc được cấu hình ứng dụng. " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            btnSaoLuu.Enabled = !busy;
            btnKhoiPhuc.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private void ShowError(string message)
        {
            lblTrangThai.Text = message;
            lblTrangThai.ForeColor = Color.FromArgb(185, 28, 28);
            MessageBox.Show(message, "Thao tác thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
