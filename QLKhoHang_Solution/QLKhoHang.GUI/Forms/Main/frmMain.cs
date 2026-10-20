using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.DTO;
using QLKhoHang.GUI;
using QLKhoHang.GUI.Common;
using QLKhoHang.GUI.Forms.Auth;

namespace QLKhoHang.GUI.Forms.Main
{
    /// <summary>Shell điều hướng của ứng dụng kho hàng.</summary>
    public partial class frmMain : Form
    {
        private readonly PhanQuyenBUS _phanQuyenBUS = new PhanQuyenBUS();
        private Form _activeChildForm = null;

        public frmMain()
        {
            bool designTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            if (!designTime && !SessionManager.DaDangNhap)
            {
                throw new UnauthorizedAccessException("Cần đăng nhập trước khi mở màn hình chính.");
            }

            InitializeComponent();
            NhanVienDTO user = designTime ? GetDesignUser() : SessionManager.NguoiDungHienTai;
            HienThiNguoiDung(user);
            CauHinhMenu(user);
            HienThiTongQuan(user);

            // 1. Gọi hàm tạo nút Giới thiệu ở đây
            ThemNutGioiThieu();

            // 2. Đoạn này phải nằm trong frmMain(), KHÔNG nằm trong ThemNutGioiThieu
            if (!designTime)
            {
                FormClosed += delegate { SessionManager.DangXuat(); };
            }
        }

        // Phương thức tạo nút Giới thiệu gắn vào thanh menu
        private void ThemNutGioiThieu()
        {
            Button btnAbout = new Button();
            btnAbout.Text = "Giới thiệu đề tài";
            btnAbout.Size = new Size(200, 42);
            btnAbout.FlatStyle = FlatStyle.Flat;
            btnAbout.FlatAppearance.BorderSize = 0;
            btnAbout.BackColor = Color.FromArgb(16, 44, 59); // Màu nền chuẩn menu
            btnAbout.ForeColor = Color.FromArgb(226, 232, 240);
            btnAbout.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            btnAbout.TextAlign = ContentAlignment.MiddleLeft;
            btnAbout.Padding = new Padding(12, 0, 0, 0);
            btnAbout.Cursor = Cursors.Hand;

            // Hiệu ứng rê chuột
            btnAbout.MouseEnter += (s, e) => { btnAbout.BackColor = Color.FromArgb(19, 126, 119); };
            btnAbout.MouseLeave += (s, e) => { btnAbout.BackColor = Color.FromArgb(16, 44, 59); };

            // Bấm vào để mở popup frmAbout
            btnAbout.Click += (s, e) =>
            {
                using (frmAbout f = new frmAbout())
                {
                    f.ShowDialog(this);
                }
            };

            // Đưa nút vào danh sách menu bên trái (kết thúc tại đây)
            flpMenu.Controls.Add(btnAbout);
        }

        private static NhanVienDTO GetDesignUser()
        {
            return new NhanVienDTO { HoTen = "Nguyễn Văn An", VaiTro = "Quản lý kho" };
        }

        private void HienThiFormCon(Form childForm, string tieuDe)
        {
            // Đóng và giải phóng form con cũ nếu đang mở
            if (_activeChildForm != null)
            {
                _activeChildForm.Close();
                _activeChildForm.Dispose();
                _activeChildForm = null;
            }

            _activeChildForm = childForm;
            lblTieuDe.Text = tieuDe;
            pnlDashboard.Visible = false;
            pnlModulePlaceholder.Visible = true;
            lblModulePlaceholder.Visible = false; // Ẩn nhãn tạm thời của TV1

            // Quy cách nhúng Form con theo Mục 1.3: TopLevel = false, None, Dock = Fill
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlModulePlaceholder.Controls.Add(childForm);
            pnlModulePlaceholder.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void HienThiNguoiDung(NhanVienDTO user)
        {
            if (user == null) return;
            lblNguoiDung.Text = user.HoTen + Environment.NewLine + user.VaiTro;
            lblAvatar.Text = Initials(user.HoTen);
            btnHeaderDangXuat.Text = "Đăng xuất  ·  " + user.HoTen;
        }

        private void CauHinhMenu(NhanVienDTO user)
        {
            ConfigureMenuItem(btnTongQuan, ChucNang.TongQuan, user);
            ConfigureMenuItem(btnHangHoa, ChucNang.HangHoa, user);
            ConfigureMenuItem(btnNhaCungCap, ChucNang.NhaCungCap, user);
            ConfigureMenuItem(btnNhapKho, ChucNang.NhapKho, user);
            ConfigureMenuItem(btnXuatKho, ChucNang.XuatKho, user);
            ConfigureMenuItem(btnKiemKeKho, ChucNang.KiemKe, user);
            ConfigureMenuItem(btnTheKho, ChucNang.TheKho, user);
            ConfigureMenuItem(btnBaoCaoTonKho, ChucNang.BaoCaoTonKho, user);
            ConfigureMenuItem(btnQuanLyTaiKhoan, ChucNang.QuanLyTaiKhoan, user);
            ConfigureMenuItem(btnSaoLuu, ChucNang.SaoLuu, user);
            ConfigureMenuItem(btnKetNoiCSDL, ChucNang.KetNoiCSDL, user);
        }

        private void ConfigureMenuItem(Button button, ChucNang chucNang, NhanVienDTO user)
        {
            button.Tag = chucNang;
            button.Visible = _phanQuyenBUS.CoQuyen(user, chucNang);
        }

        private static string Initials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "NV";
            string[] parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0]).ToUpperInvariant();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null || !(button.Tag is ChucNang)) return;

            foreach (Control control in flpMenu.Controls)
            {
                Button item = control as Button;
                if (item == null) continue;
                bool selected = ReferenceEquals(item, button);
                item.BackColor = selected ? Color.FromArgb(19, 126, 119) : Color.FromArgb(16, 44, 59);
                item.ForeColor = selected ? Color.White : Color.FromArgb(226, 232, 240);
            }

            ChucNang chucNang = (ChucNang)button.Tag;
            if (chucNang == ChucNang.TongQuan)
            {
                if (_activeChildForm != null)
                {
                    _activeChildForm.Close();
                    _activeChildForm.Dispose();
                    _activeChildForm = null;
                }
                lblModulePlaceholder.Visible = true;
                pnlDashboard.Visible = true;
                pnlModulePlaceholder.Visible = false;
                lblTieuDe.Text = "TỔNG QUAN HỆ THỐNG";
                return;
            }

            OpenModule(chucNang, button.Text);
        }

        private void OpenModule(ChucNang chucNang, string tenModule)
        {
            try
            {
                _phanQuyenBUS.YeuCauQuyen(SessionManager.NguoiDungHienTai, chucNang);
                if (chucNang == ChucNang.SaoLuu)
                {
                    using (frmSaoLuu form = new frmSaoLuu()) form.ShowDialog(this);
                    return;
                }
                if (chucNang == ChucNang.KetNoiCSDL)
                {
                    using (frmKetNoiCSDL form = new frmKetNoiCSDL()) form.ShowDialog(this);
                    return;
                }

                // Tích hợp form Nhà cung cấp của TV2
                if (chucNang == ChucNang.NhaCungCap)
                {
                    HienThiFormCon(new frmNhaCungCap(), "QUẢN LÝ DANH MỤC NHÀ CUNG CẤP");
                    return;
                }

                // Tích hợp form Hàng hóa của TV2 (sẵn sàng khi bạn hoàn thành frmHangHoa)
                 if (chucNang == ChucNang.HangHoa)
                {
                   HienThiFormCon(new frmHangHoa(), "QUẢN LÝ DANH MỤC HÀNG HÓA");
                    return;
                 }

                // Các module khác chưa hoàn tất sẽ hiển thị màn hình chờ mặc định
                if (_activeChildForm != null)
                {
                    _activeChildForm.Close();
                    _activeChildForm.Dispose();
                    _activeChildForm = null;
                }
                lblModulePlaceholder.Visible = true;
                lblModulePlaceholder.Text = tenModule + Environment.NewLine + Environment.NewLine +
                    "Màn hình này sẽ được tích hợp khi hoàn tất module được phân công.";
                lblTieuDe.Text = tenModule.ToUpperInvariant();
                pnlDashboard.Visible = false;
                pnlModulePlaceholder.Visible = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HienThiTongQuan(NhanVienDTO user)
        {
            lblChaoMung.Text = "Xin chào, " + user.HoTen + ". Bạn đang đăng nhập với vai trò " + user.VaiTro + ".";
            lblTrangThai.Text = "●  Kết nối cơ sở dữ liệu sẽ được kiểm tra khi truy vấn.";
            lblDatabaseStatus.Text = "Database theo cấu hình ứng dụng: QLKhoHang";
        }

        private void BtnDoiMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                _phanQuyenBUS.YeuCauQuyen(SessionManager.NguoiDungHienTai, ChucNang.DoiMatKhau);
                using (frmDoiMatKhau form = new frmDoiMatKhau()) form.ShowDialog(this);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            SessionManager.DangXuat();
            Close();
        }
    }
}