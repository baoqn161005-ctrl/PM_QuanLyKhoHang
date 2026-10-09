using System;
using System.Drawing;
using System.Windows.Forms;
using QLKhoHang.BUS;
using QLKhoHang.DTO;
using QLKhoHang.GUI.Common;
using QLKhoHang.GUI.Forms.Auth;

namespace QLKhoHang.GUI.Forms.Main
{
    /// <summary>
    /// Shell chính: điều hướng module, hiển thị session và đăng xuất.
    /// </summary>
    public class frmMain : Form
    {
        private static readonly Color MauNen = Color.FromArgb(248, 250, 252);
        private static readonly Color MauSidebar = Color.FromArgb(30, 41, 59);
        private static readonly Color MauTeal = Color.FromArgb(13, 148, 136);
        private readonly PhanQuyenBUS _phanQuyenBUS = new PhanQuyenBUS();
        private readonly Panel pnlWorkspace = new Panel();
        private readonly Label lblTrangThai = new Label();

        public frmMain()
        {
            if (!SessionManager.DaDangNhap)
            {
                throw new UnauthorizedAccessException("Cần đăng nhập trước khi mở màn hình chính.");
            }

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            NhanVienDTO nhanVien = SessionManager.NguoiDungHienTai;
            Text = "QLKhoHang - Hệ thống quản lý kho";
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1024, 680);
            BackColor = MauNen;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            Panel pnlSidebar = new Panel
            {
                Name = "pnlSidebar",
                Dock = DockStyle.Left,
                Width = 235,
                BackColor = MauSidebar
            };

            Label lblLogo = new Label
            {
                Name = "lblLogo",
                Text = "QL KHO HÀNG",
                Dock = DockStyle.Top,
                Height = 76,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold)
            };

            Label lblNguoiDung = new Label
            {
                Name = "lblNguoiDung",
                Text = nhanVien.HoTen + Environment.NewLine + nhanVien.VaiTro,
                Dock = DockStyle.Top,
                Height = 62,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 8, 0),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            FlowLayoutPanel flpMenu = new FlowLayoutPanel
            {
                Name = "flpMenu",
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = MauSidebar
            };

            AddMenu(flpMenu, "Tổng quan", ChucNang.TongQuan);
            AddMenu(flpMenu, "Hàng hóa", ChucNang.HangHoa);
            AddMenu(flpMenu, "Nhà cung cấp", ChucNang.NhaCungCap);
            AddMenu(flpMenu, "Nhập kho", ChucNang.NhapKho);
            AddMenu(flpMenu, "Xuất kho", ChucNang.XuatKho);
            AddMenu(flpMenu, "Kiểm kê kho", ChucNang.KiemKe);
            AddMenu(flpMenu, "Thẻ kho", ChucNang.TheKho);
            AddMenu(flpMenu, "Báo cáo nhập - xuất - tồn", ChucNang.BaoCaoTonKho);
            AddMenu(flpMenu, "Quản lý tài khoản", ChucNang.QuanLyTaiKhoan);
            AddMenu(flpMenu, "Sao lưu dữ liệu", ChucNang.SaoLuu);
            AddMenu(flpMenu, "Kết nối cơ sở dữ liệu", ChucNang.KetNoiCSDL);

            Button btnDoiMatKhau = TaoNutMenu("btnDoiMatKhau", "Đổi mật khẩu");
            btnDoiMatKhau.Click += BtnDoiMatKhau_Click;
            btnDoiMatKhau.Dock = DockStyle.Bottom;

            Button btnDangXuat = TaoNutMenu("btnDangXuat", "Đăng xuất");
            btnDangXuat.Dock = DockStyle.Bottom;
            btnDangXuat.Click += BtnDangXuat_Click;

            pnlSidebar.Controls.Add(flpMenu);
            pnlSidebar.Controls.Add(lblNguoiDung);
            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Controls.Add(btnDoiMatKhau);
            pnlSidebar.Controls.Add(btnDangXuat);

            Panel pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 66,
                BackColor = Color.White,
                Padding = new Padding(22, 0, 18, 0)
            };
            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "HỆ THỐNG QUẢN LÝ KHO HÀNG",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold)
            };
            Button btnHeaderDangXuat = new Button
            {
                Name = "btnHeaderDangXuat",
                Text = "Đăng xuất  ·  " + nhanVien.HoTen,
                Dock = DockStyle.Right,
                Width = 205,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85)
            };
            btnHeaderDangXuat.FlatAppearance.BorderSize = 0;
            btnHeaderDangXuat.Click += BtnDangXuat_Click;
            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(btnHeaderDangXuat);

            pnlWorkspace.Name = "pnlWorkspace";
            pnlWorkspace.Dock = DockStyle.Fill;
            pnlWorkspace.BackColor = MauNen;
            pnlWorkspace.Padding = new Padding(26);

            Panel pnlStatus = new Panel
            {
                Name = "pnlStatus",
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.White
            };
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Text = "Kết nối cơ sở dữ liệu sẽ được kiểm tra khi truy vấn.";
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            lblTrangThai.Padding = new Padding(14, 0, 0, 0);
            lblTrangThai.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            lblTrangThai.ForeColor = Color.FromArgb(71, 85, 105);
            pnlStatus.Controls.Add(lblTrangThai);

            Controls.Add(pnlWorkspace);
            Controls.Add(pnlStatus);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);
            HienThiTongQuan();
            FormClosed += delegate { SessionManager.DangXuat(); };
        }

        private void AddMenu(FlowLayoutPanel menu, string text, ChucNang chucNang)
        {
            if (!_phanQuyenBUS.CoQuyen(SessionManager.NguoiDungHienTai, chucNang))
            {
                return;
            }

            Button button = TaoNutMenu("btn" + chucNang, text);
            button.Tag = chucNang;
            button.Click += MenuItem_Click;
            menu.Controls.Add(button);
        }

        private static Button TaoNutMenu(string name, string text)
        {
            Button button = new Button
            {
                Name = name,
                Text = text,
                Height = 42,
                Width = 205,
                Margin = new Padding(0, 3, 0, 3),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = MauSidebar,
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null || !(button.Tag is ChucNang))
            {
                return;
            }

            OpenModule((ChucNang)button.Tag, button.Text);
        }

        private void OpenModule(ChucNang chucNang, string tenModule)
        {
            try
            {
                // Kiểm tra quyền cả tại điểm điều hướng; các form nghiệp vụ cần kiểm tra lại khi triển khai.
                _phanQuyenBUS.YeuCauQuyen(SessionManager.NguoiDungHienTai, chucNang);
                if (chucNang == ChucNang.SaoLuu)
                {
                    using (frmSaoLuu form = new frmSaoLuu())
                    {
                        form.ShowDialog(this);
                    }
                    return;
                }

                if (chucNang == ChucNang.KetNoiCSDL)
                {
                    using (frmKetNoiCSDL form = new frmKetNoiCSDL())
                    {
                        form.ShowDialog(this);
                    }
                    return;
                }

                pnlWorkspace.Controls.Clear();
                Label lblModule = new Label
                {
                    Text = tenModule + Environment.NewLine + Environment.NewLine +
                           "Màn hình này sẽ được tích hợp khi hoàn tất module được phân công.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular)
                };
                pnlWorkspace.Controls.Add(lblModule);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HienThiTongQuan()
        {
            NhanVienDTO nhanVien = SessionManager.NguoiDungHienTai;
            Label lblChaoMung = new Label
            {
                Text = "Xin chào, " + nhanVien.HoTen,
                Dock = DockStyle.Top,
                Height = 52,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold)
            };
            Label lblHuongDan = new Label
            {
                Text = "Bạn đang đăng nhập với vai trò " + nhanVien.VaiTro + ". Chọn chức năng ở thanh điều hướng.",
                Dock = DockStyle.Top,
                Height = 42,
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular)
            };
            Label lblTrangThaiModule = new Label
            {
                Text = "Các thẻ thống kê sẽ hiển thị khi các module dữ liệu và báo cáo được tích hợp.",
                Dock = DockStyle.Top,
                Height = 72,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 10F, FontStyle.Italic)
            };
            pnlWorkspace.Controls.Clear();
            pnlWorkspace.Controls.Add(lblTrangThaiModule);
            pnlWorkspace.Controls.Add(lblHuongDan);
            pnlWorkspace.Controls.Add(lblChaoMung);
        }

        private void BtnDoiMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                _phanQuyenBUS.YeuCauQuyen(SessionManager.NguoiDungHienTai, ChucNang.DoiMatKhau);
                using (frmDoiMatKhau form = new frmDoiMatKhau())
                {
                    form.ShowDialog(this);
                }
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
