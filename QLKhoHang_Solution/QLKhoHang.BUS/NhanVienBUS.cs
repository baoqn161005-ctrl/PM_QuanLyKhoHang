using System;
using QLKhoHang.DAL;
using QLKhoHang.DTO;

namespace QLKhoHang.BUS
{
    /// <summary>
    /// Kiểm tra dữ liệu đầu vào và điều phối nghiệp vụ nhân viên.
    /// </summary>
    public class NhanVienBUS
    {
        private readonly NhanVienDAL _nhanVienDAL;

        public NhanVienBUS() : this(new NhanVienDAL())
        {
        }

        internal NhanVienBUS(NhanVienDAL nhanVienDAL)
        {
            _nhanVienDAL = nhanVienDAL ?? throw new ArgumentNullException(nameof(nhanVienDAL));
        }

        public NhanVienDTO DangNhap(string taiKhoan, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(taiKhoan))
            {
                throw new ArgumentException("Vui lòng nhập tài khoản.", nameof(taiKhoan));
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                throw new ArgumentException("Vui lòng nhập mật khẩu.", nameof(matKhau));
            }

            NhanVienDTO nhanVien = _nhanVienDAL.DangNhap(taiKhoan.Trim(), matKhau);
            if (nhanVien == null)
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng.");
            }

            return nhanVien;
        }

        public void DoiMatKhau(NhanVienDTO nhanVien, string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            if (nhanVien == null || string.IsNullOrWhiteSpace(nhanVien.MaNV))
            {
                throw new UnauthorizedAccessException("Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.");
            }

            if (string.IsNullOrEmpty(matKhauCu))
            {
                throw new ArgumentException("Vui lòng nhập mật khẩu hiện tại.", nameof(matKhauCu));
            }

            if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 8)
            {
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 8 ký tự.", nameof(matKhauMoi));
            }

            if (!string.Equals(matKhauMoi, xacNhanMatKhau, StringComparison.Ordinal))
            {
                throw new ArgumentException("Mật khẩu xác nhận không khớp.", nameof(xacNhanMatKhau));
            }

            if (string.Equals(matKhauCu, matKhauMoi, StringComparison.Ordinal))
            {
                throw new ArgumentException("Mật khẩu mới phải khác mật khẩu hiện tại.", nameof(matKhauMoi));
            }

            if (!_nhanVienDAL.DoiMatKhau(nhanVien.MaNV, matKhauCu, matKhauMoi))
            {
                throw new UnauthorizedAccessException(
                    "Không đổi được mật khẩu. Hãy kiểm tra mật khẩu hiện tại hoặc đăng nhập lại.");
            }
        }
    }
}
