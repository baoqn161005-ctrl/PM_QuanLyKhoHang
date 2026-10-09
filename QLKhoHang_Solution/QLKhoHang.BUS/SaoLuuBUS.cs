using System;
using QLKhoHang.DAL;
using QLKhoHang.DTO;

namespace QLKhoHang.BUS
{
    /// <summary>
    /// Ràng buộc quyền truy cập các thao tác backup và restore.
    /// </summary>
    public class SaoLuuBUS
    {
        private readonly SaoLuuDAL _saoLuuDAL = new SaoLuuDAL();
        private readonly PhanQuyenBUS _phanQuyenBUS = new PhanQuyenBUS();

        public string LayDatabaseDangCauHinh()
        {
            return _saoLuuDAL.LayDatabaseDangCauHinh();
        }

        public void SaoLuu(NhanVienDTO nguoiDung, string backupPath)
        {
            _phanQuyenBUS.YeuCauQuyen(nguoiDung, ChucNang.SaoLuu);
            if (string.IsNullOrWhiteSpace(backupPath))
            {
                throw new ArgumentException("Chưa chọn đường dẫn file backup.", nameof(backupPath));
            }

            _saoLuuDAL.SaoLuuDatabase(backupPath);
        }

        public string KhoiPhucSangDatabaseTestMoi(NhanVienDTO nguoiDung, string backupPath)
        {
            _phanQuyenBUS.YeuCauQuyen(nguoiDung, ChucNang.SaoLuu);
            if (string.IsNullOrWhiteSpace(backupPath))
            {
                throw new ArgumentException("Chưa chọn file backup.", nameof(backupPath));
            }

            return _saoLuuDAL.RestoreToNewTestDatabase(backupPath);
        }
    }
}
