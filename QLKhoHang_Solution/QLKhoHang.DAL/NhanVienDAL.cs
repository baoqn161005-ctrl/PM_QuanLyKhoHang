using System;
using System.Data;
using System.Data.SqlClient;
using QLKhoHang.DTO;

namespace QLKhoHang.DAL
{
    /// <summary>
    /// Đọc tài khoản nhân viên và cập nhật mật khẩu đã băm.
    /// </summary>
    public class NhanVienDAL
    {
        public NhanVienDTO DangNhap(string taiKhoan, string matKhau)
        {
            const string sql = @"
SELECT MaNV, HoTen, TaiKhoan, MatKhau, VaiTro
FROM dbo.NhanVien
WHERE TaiKhoan = @TaiKhoan;";

            DataTable rows = DatabaseHelper.ExecuteQuery(
                sql,
                new[] { new SqlParameter("@TaiKhoan", SqlDbType.VarChar, 50) { Value = taiKhoan } });

            if (rows.Rows.Count != 1)
            {
                return null;
            }

            DataRow row = rows.Rows[0];
            string storedHash = Convert.ToString(row["MatKhau"]);
            if (!PasswordHasher.Verify(matKhau, storedHash))
            {
                return null;
            }

            return new NhanVienDTO
            {
                MaNV = Convert.ToString(row["MaNV"]),
                HoTen = Convert.ToString(row["HoTen"]),
                TaiKhoan = Convert.ToString(row["TaiKhoan"]),
                VaiTro = Convert.ToString(row["VaiTro"])
            };
        }

        public bool DoiMatKhau(string maNV, string matKhauCu, string matKhauMoi)
        {
            const string selectSql = @"
SELECT MatKhau
FROM dbo.NhanVien
WHERE MaNV = @MaNV;";

            DataTable rows = DatabaseHelper.ExecuteQuery(
                selectSql,
                new[] { new SqlParameter("@MaNV", SqlDbType.VarChar, 10) { Value = maNV } });

            if (rows.Rows.Count != 1)
            {
                return false;
            }

            string storedHash = Convert.ToString(rows.Rows[0]["MatKhau"]);
            if (!PasswordHasher.Verify(matKhauCu, storedHash))
            {
                return false;
            }

            const string updateSql = @"
UPDATE dbo.NhanVien
SET MatKhau = @MatKhauMoi
WHERE MaNV = @MaNV AND MatKhau = @MatKhauCu;";

            int affected = DatabaseHelper.ExecuteNonQuery(
                updateSql,
                new[]
                {
                    new SqlParameter("@MatKhauMoi", SqlDbType.VarChar, 255)
                    {
                        Value = PasswordHasher.Hash(matKhauMoi)
                    },
                    new SqlParameter("@MaNV", SqlDbType.VarChar, 10) { Value = maNV },
                    new SqlParameter("@MatKhauCu", SqlDbType.VarChar, 255) { Value = storedHash }
                });

            return affected == 1;
        }
    }
}
