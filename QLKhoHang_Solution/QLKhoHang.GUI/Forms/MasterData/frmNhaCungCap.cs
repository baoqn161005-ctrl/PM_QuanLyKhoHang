using QLKhoHang.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using QLKhoHang.GUI.Helpers;

namespace QLKhoHang.GUI
{
    public partial class frmNhaCungCap : Form
    {
        private bool isAddingNew = false;

        public frmNhaCungCap()
        {
            InitializeComponent();
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            SetupUI();
            LoadDanhSachNhaCungCap();
            LamMoiForm();
        }

        #region 1. KHỞI TẠO GIAO DIỆN CHUẨN UI/UX
        private void SetupUI()
        {
            // Áp dụng định dạng chuẩn cho DataGridView từ UIThemeHelper
            UIThemeHelper.ApplyStandardGridStyle(dgvNhaCungCap);

            // Định dạng màu sắc và hiệu ứng nút bấm chuẩn UI/UX
            UIThemeHelper.ApplyButtonStyle(btnThem, UIThemeHelper.ButtonType.SaveOrAdd);
            UIThemeHelper.ApplyButtonStyle(btnLuu, UIThemeHelper.ButtonType.SaveOrAdd);
            UIThemeHelper.ApplyButtonStyle(btnXoa, UIThemeHelper.ButtonType.Danger);
            UIThemeHelper.ApplyButtonStyle(btnLamMoi, UIThemeHelper.ButtonType.Neutral);
            UIThemeHelper.ApplyButtonStyle(btnTim, UIThemeHelper.ButtonType.Primary);

            this.Font = UIThemeHelper.ControlFont;
            this.BackColor = UIThemeHelper.WorkspaceBgColor;
        }
        #endregion

        #region 2. TẢI VÀ TÌM KIẾM DỮ LIỆU
        private void LoadDanhSachNhaCungCap()
        {
            try
            {
                string sql = @"SELECT MaNCC, TenNCC, DienThoai, NguoiLienHe, DiaChi, Email 
                               FROM dbo.NhaCungCap 
                               ORDER BY MaNCC ASC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, null);
                dgvNhaCungCap.DataSource = dt;

                // Tùy biến tiêu đề cột hiển thị tiếng Việt thân thiện
                if (dgvNhaCungCap.Columns["MaNCC"] != null)
                {
                    dgvNhaCungCap.Columns["MaNCC"].HeaderText = "Mã NCC";
                    dgvNhaCungCap.Columns["MaNCC"].FillWeight = 40;
                }
                if (dgvNhaCungCap.Columns["TenNCC"] != null)
                {
                    dgvNhaCungCap.Columns["TenNCC"].HeaderText = "Tên công ty cung ứng";
                    dgvNhaCungCap.Columns["TenNCC"].FillWeight = 110;
                }
                if (dgvNhaCungCap.Columns["DienThoai"] != null)
                {
                    dgvNhaCungCap.Columns["DienThoai"].HeaderText = "Điện thoại";
                    dgvNhaCungCap.Columns["DienThoai"].FillWeight = 50;
                }
                if (dgvNhaCungCap.Columns["NguoiLienHe"] != null)
                {
                    dgvNhaCungCap.Columns["NguoiLienHe"].HeaderText = "Người liên hệ";
                    dgvNhaCungCap.Columns["NguoiLienHe"].FillWeight = 60;
                }
                if (dgvNhaCungCap.Columns["DiaChi"] != null)
                {
                    dgvNhaCungCap.Columns["DiaChi"].HeaderText = "Địa chỉ";
                    dgvNhaCungCap.Columns["DiaChi"].FillWeight = 110;
                }
                if (dgvNhaCungCap.Columns["Email"] != null)
                {
                    dgvNhaCungCap.Columns["Email"].HeaderText = "Email";
                    dgvNhaCungCap.Columns["Email"].FillWeight = 70;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhà cung cấp: " + ex.Message,
                                "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimKiemNhaCungCap()
        {
            try
            {
                string keyword = txtTimNCC.Text.Trim();
                string sql = @"SELECT MaNCC, TenNCC, DienThoai, NguoiLienHe, DiaChi, Email 
                               FROM dbo.NhaCungCap 
                               WHERE MaNCC LIKE @kw 
                                  OR TenNCC LIKE @kw 
                                  OR DienThoai LIKE @kw 
                                  OR NguoiLienHe LIKE @kw 
                               ORDER BY MaNCC ASC";

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@kw", "%" + keyword + "%")
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, pars);
                dgvNhaCungCap.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 3. THAO TÁC CRUD
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAddingNew = true;
            LamMoiForm();
            txtMaNCC.Enabled = true;
            txtMaNCC.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra tính hợp lệ dữ liệu nhập (Validation)
            string maNCC = txtMaNCC.Text.Trim();
            string tenNCC = txtTenNCC.Text.Trim();
            string dienThoai = txtDienThoai.Text.Trim();
            string email = txtEmail.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string nguoiLienHe = txtNguoiLienHe.Text.Trim();

            if (string.IsNullOrEmpty(maNCC))
            {
                MessageBox.Show("Vui lòng nhập Mã nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNCC.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenNCC))
            {
                MessageBox.Show("Vui lòng nhập Tên nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return;
            }

            if (string.IsNullOrEmpty(dienThoai))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại liên hệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDienThoai.Focus();
                return;
            }

            try
            {
                if (isAddingNew)
                {
                    // Kiểm tra trùng khóa chính MaNCC
                    string sqlCheck = "SELECT COUNT(*) FROM dbo.NhaCungCap WHERE MaNCC = @MaNCC";
                    SqlParameter[] parCheck = { new SqlParameter("@MaNCC", maNCC) };
                    object result = DatabaseHelper.ExecuteScalar(sqlCheck, parCheck);

                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        MessageBox.Show($"Mã nhà cung cấp '{maNCC}' đã tồn tại trong hệ thống! Vui lòng chọn mã khác.",
                                        "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMaNCC.Focus();
                        return;
                    }

                    // Thực hiện INSERT với SqlParameter tuyệt đối an toàn
                    string sqlInsert = @"INSERT INTO dbo.NhaCungCap (MaNCC, TenNCC, DienThoai, Email, DiaChi, NguoiLienHe)
                                         VALUES (@MaNCC, @TenNCC, @DienThoai, @Email, @DiaChi, @NguoiLienHe)";

                    SqlParameter[] parInsert = new SqlParameter[]
                    {
                        new SqlParameter("@MaNCC", maNCC),
                        new SqlParameter("@TenNCC", tenNCC),
                        new SqlParameter("@DienThoai", dienThoai),
                        new SqlParameter("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                        new SqlParameter("@DiaChi", string.IsNullOrEmpty(diaChi) ? (object)DBNull.Value : diaChi),
                        new SqlParameter("@NguoiLienHe", string.IsNullOrEmpty(nguoiLienHe) ? (object)DBNull.Value : nguoiLienHe)
                    };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlInsert, parInsert);
                    if (rows > 0)
                    {
                        MessageBox.Show("Thêm mới nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Thực hiện UPDATE
                    string sqlUpdate = @"UPDATE dbo.NhaCungCap 
                                         SET TenNCC = @TenNCC,
                                             DienThoai = @DienThoai,
                                             Email = @Email,
                                             DiaChi = @DiaChi,
                                             NguoiLienHe = @NguoiLienHe
                                         WHERE MaNCC = @MaNCC";

                    SqlParameter[] parUpdate = new SqlParameter[]
                    {
                        new SqlParameter("@MaNCC", maNCC),
                        new SqlParameter("@TenNCC", tenNCC),
                        new SqlParameter("@DienThoai", dienThoai),
                        new SqlParameter("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                        new SqlParameter("@DiaChi", string.IsNullOrEmpty(diaChi) ? (object)DBNull.Value : diaChi),
                        new SqlParameter("@NguoiLienHe", string.IsNullOrEmpty(nguoiLienHe) ? (object)DBNull.Value : nguoiLienHe)
                    };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlUpdate, parUpdate);
                    if (rows > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadDanhSachNhaCungCap();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maNCC = txtMaNCC.Text.Trim();
            string tenNCC = txtTenNCC.Text.Trim();

            if (string.IsNullOrEmpty(maNCC) || isAddingNew)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp từ bảng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra an toàn toàn vẹn dữ liệu: Có vướng khóa ngoại PhieuNhap không?
                // Đoạn chuẩn:
                string sqlCheckFK = "SELECT COUNT(*) FROM dbo.PhieuNhap WHERE MaNCC = @MaNCC";
                SqlParameter[] parCheck = { new SqlParameter("@MaNCC", maNCC) };
                object count = DatabaseHelper.ExecuteScalar(sqlCheckFK, parCheck);

                if (count != null && Convert.ToInt32(count) > 0)
                {
                    MessageBox.Show($"Không thể xóa nhà cung cấp [{maNCC}] vì đã có {count} phiếu nhập kho liên kết trong hệ thống!\nĐể đảm bảo tính toàn vẹn dữ liệu kế toán, bạn chỉ có thể cập nhật thông tin.",
                                    "Chặn thao tác xóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhà cung cấp:\n[{maNCC}] - {tenNCC} không?",
                                                  "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    string sqlDelete = "DELETE FROM dbo.NhaCungCap WHERE MaNCC = @MaNCC";
                    SqlParameter[] parDelete = { new SqlParameter("@MaNCC", maNCC) };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlDelete, parDelete);
                    if (rows > 0)
                    {
                        MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhSachNhaCungCap();
                        LamMoiForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoiForm();
            LoadDanhSachNhaCungCap();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            TimKiemNhaCungCap();
        }

        private void txtTimNCC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimKiemNhaCungCap();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void dgvNhaCungCap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvNhaCungCap.Rows.Count)
            {
                DataGridViewRow row = dgvNhaCungCap.Rows[e.RowIndex];
                if (row == null || row.Cells["MaNCC"].Value == null) return;

                txtMaNCC.Text = row.Cells["MaNCC"].Value.ToString();
                txtTenNCC.Text = row.Cells["TenNCC"].Value?.ToString() ?? "";
                txtDienThoai.Text = row.Cells["DienThoai"].Value?.ToString() ?? "";
                txtNguoiLienHe.Text = row.Cells["NguoiLienHe"].Value?.ToString() ?? "";
                txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";

                // Khóa ô mã NCC khi đang chọn sửa dòng có sẵn
                txtMaNCC.Enabled = false;
                isAddingNew = false;
            }
        }

        private void LamMoiForm()
        {
            txtMaNCC.Clear();
            txtTenNCC.Clear();
            txtDienThoai.Clear();
            txtNguoiLienHe.Clear();
            txtDiaChi.Clear();
            txtEmail.Clear();
            txtTimNCC.Clear();

            txtMaNCC.Enabled = true;
            isAddingNew = true;
            txtMaNCC.Focus();
        }
        #endregion
    }
}