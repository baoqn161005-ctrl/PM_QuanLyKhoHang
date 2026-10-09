using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.DTO;
using QLKhoHang.GUI.Common;
using QLKhoHang.GUI.Forms.Auth;

namespace QLKhoHang.GUI.Forms.Main
{
    /// <summary>Shell điều hướng của ứng dụng kho hàng.</summary>
    public partial class frmMain : Form
    {
        private readonly PhanQuyenBUS _phanQuyenBUS = new PhanQuyenBUS();

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

            if (!designTime)
            {
                FormClosed += delegate { SessionManager.DangXuat(); };
            }
        }

        private static NhanVienDTO GetDesignUser()
        {
            return new NhanVienDTO { HoTen = "Nguyễn Văn An", VaiTro = "Quản lý kho" };
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
