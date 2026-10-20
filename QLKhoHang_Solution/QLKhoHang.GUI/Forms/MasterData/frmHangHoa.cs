using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QLKhoHang.DAL;
using QLKhoHang.GUI.Helpers;

namespace QLKhoHang.GUI
{
    public partial class frmHangHoa : Form
    {
        private bool isAddingNew = false;
        private int currentSoLuongTon = 0; // Lưu số lượng tồn hiện hành khi sửa

        public frmHangHoa()
        {
            InitializeComponent();
        }

        private void frmHangHoa_Load(object sender, EventArgs e)
        {
            SetupUI();
            InitComboBoxes();
            LoadDanhSachHangHoa();
            LamMoiForm();
        }

        #region 1. KHỞI TẠO GIAO DIỆN CHUẨN UI/UX
        private void SetupUI()
        {
            // Áp dụng chuẩn DataGridView từ UIThemeHelper (Mục 1.3 Báo cáo)
            UIThemeHelper.ApplyStandardGridStyle(dgvHangHoa);

            // Bật DoubleBuffered cho DataGridView để cuộn mượt mà không bị giật lag
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, dgvHangHoa, new object[] { true });

            // Định dạng màu sắc nút bấm theo chuẩn
            UIThemeHelper.ApplyButtonStyle(btnThem, UIThemeHelper.ButtonType.SaveOrAdd);
            UIThemeHelper.ApplyButtonStyle(btnLuu, UIThemeHelper.ButtonType.SaveOrAdd);
            UIThemeHelper.ApplyButtonStyle(btnXoa, UIThemeHelper.ButtonType.Danger);
            UIThemeHelper.ApplyButtonStyle(btnLamMoi, UIThemeHelper.ButtonType.Neutral);
            UIThemeHelper.ApplyButtonStyle(btnTim, UIThemeHelper.ButtonType.Primary);

            this.Font = UIThemeHelper.ControlFont;
            this.BackColor = UIThemeHelper.WorkspaceBgColor;
        }

        private void InitComboBoxes()
        {
            // 3 Phân nhóm hàng FMCG theo Mục 1.1 Báo cáo tiểu luận
            cboLoaiHang.Items.Clear();
            cboLoaiHang.Items.Add("Thực phẩm ăn liền");
            cboLoaiHang.Items.Add("Đồ uống & Sữa");
            cboLoaiHang.Items.Add("Hóa mỹ phẩm");
            cboLoaiHang.SelectedIndex = 0;

            // Đơn vị tính quy chuẩn theo Mục 1.1
            cboDVT.Items.Clear();
            cboDVT.Items.AddRange(new object[] { "Thùng", "Gói", "Hộp", "Chai", "Lon", "Bao", "Túi", "Lốc" });
            cboDVT.SelectedIndex = 0;
        }
        #endregion

        #region 2. TẢI VÀ TÌM KIẾM DỮ LIỆU
        private void LoadDanhSachHangHoa()
        {
            try
            {
                string sql = @"SELECT MaSP, TenSP, LoaiHang, DonViTinh, 
                                      GiaNhapGanNhat, SoLuongTon, TonToiThieu, TonToiDa
                               FROM dbo.HangHoa 
                               ORDER BY MaSP ASC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, null);
                dgvHangHoa.DataSource = dt;

                // Tùy biến tiêu đề cột hiển thị tiếng Việt và định dạng số
                if (dgvHangHoa.Columns["MaSP"] != null)
                {
                    dgvHangHoa.Columns["MaSP"].HeaderText = "Mã SP";
                    dgvHangHoa.Columns["MaSP"].FillWeight = 45;
                }
                if (dgvHangHoa.Columns["TenSP"] != null)
                {
                    dgvHangHoa.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                    dgvHangHoa.Columns["TenSP"].FillWeight = 130;
                }
                if (dgvHangHoa.Columns["LoaiHang"] != null)
                {
                    dgvHangHoa.Columns["LoaiHang"].HeaderText = "Nhóm loại";
                    dgvHangHoa.Columns["LoaiHang"].FillWeight = 75;
                }
                if (dgvHangHoa.Columns["DonViTinh"] != null)
                {
                    dgvHangHoa.Columns["DonViTinh"].HeaderText = "ĐVT";
                    dgvHangHoa.Columns["DonViTinh"].FillWeight = 40;
                    dgvHangHoa.Columns["DonViTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvHangHoa.Columns["GiaNhapGanNhat"] != null)
                {
                    dgvHangHoa.Columns["GiaNhapGanNhat"].HeaderText = "Giá nhập gần nhất";
                    dgvHangHoa.Columns["GiaNhapGanNhat"].FillWeight = 65;
                    dgvHangHoa.Columns["GiaNhapGanNhat"].DefaultCellStyle.Format = "N0";
                    dgvHangHoa.Columns["GiaNhapGanNhat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvHangHoa.Columns["SoLuongTon"] != null)
                {
                    dgvHangHoa.Columns["SoLuongTon"].HeaderText = "Tồn kho";
                    dgvHangHoa.Columns["SoLuongTon"].FillWeight = 45;
                    dgvHangHoa.Columns["SoLuongTon"].DefaultCellStyle.Format = "N0";
                    dgvHangHoa.Columns["SoLuongTon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvHangHoa.Columns["TonToiThieu"] != null)
                {
                    dgvHangHoa.Columns["TonToiThieu"].HeaderText = "Tồn Min";
                    dgvHangHoa.Columns["TonToiThieu"].FillWeight = 40;
                    dgvHangHoa.Columns["TonToiThieu"].DefaultCellStyle.Format = "N0";
                    dgvHangHoa.Columns["TonToiThieu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvHangHoa.Columns["TonToiDa"] != null)
                {
                    dgvHangHoa.Columns["TonToiDa"].HeaderText = "Tồn Max";
                    dgvHangHoa.Columns["TonToiDa"].FillWeight = 40;
                    dgvHangHoa.Columns["TonToiDa"].DefaultCellStyle.Format = "N0";
                    dgvHangHoa.Columns["TonToiDa"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách hàng hóa: " + ex.Message,
                                "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TimKiemHangHoa()
        {
            try
            {
                string keyword = txtTimKiem.Text.Trim();
                string sql = @"SELECT MaSP, TenSP, LoaiHang, DonViTinh, 
                                      GiaNhapGanNhat, SoLuongTon, TonToiThieu, TonToiDa
                               FROM dbo.HangHoa 
                               WHERE MaSP LIKE @kw 
                                  OR TenSP LIKE @kw 
                                  OR LoaiHang LIKE @kw 
                                  OR DonViTinh LIKE @kw 
                               ORDER BY MaSP ASC";

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@kw", "%" + keyword + "%")
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, pars);
                dgvHangHoa.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 3. THUẬT TOÁN TÔ MÀU CẢNH BÁO TỒN KHO AN TOÀN
        /// <summary>
        /// Tự động tô màu đỏ cảnh báo cho dòng hàng hóa có SoLuongTon <= TonToiThieu
        /// Đúng theo yêu cầu Mockup 3.3 và phân công Thành viên 2
        /// </summary>
        private void dgvHangHoa_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvHangHoa.Rows.Count) return;

            DataGridViewRow row = dgvHangHoa.Rows[e.RowIndex];
            if (row == null || row.DataBoundItem == null) return;

            if (row.Cells["SoLuongTon"]?.Value != null && row.Cells["TonToiThieu"]?.Value != null)
            {
                if (int.TryParse(row.Cells["SoLuongTon"].Value.ToString(), out int soLuongTon) &&
                    int.TryParse(row.Cells["TonToiThieu"].Value.ToString(), out int tonToiThieu))
                {
                    if (soLuongTon <= tonToiThieu)
                    {
                        // Đổi màu nền cả dòng sang đỏ nhạt
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FEE2E2");
                        row.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#FECACA");
                        row.DefaultCellStyle.SelectionForeColor = Color.Black;

                        // Riêng ô Tồn kho in chữ đỏ đậm và in đậm
                        if (dgvHangHoa.Columns[e.ColumnIndex].Name == "SoLuongTon")
                        {
                            e.CellStyle.ForeColor = ColorTranslator.FromHtml("#B91C1C");
                            e.CellStyle.Font = UIThemeHelper.ControlBoldFont;
                        }
                    }
                }
            }
        }
        #endregion

        #region 4. THAO TÁC CRUD
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAddingNew = true;
            currentSoLuongTon = 0;
            LamMoiForm();
            txtMaSP.Enabled = true;
            txtMaSP.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra tính hợp lệ dữ liệu nhập (Validation)
            string maSP = txtMaSP.Text.Trim();
            string tenSP = txtTenSP.Text.Trim();
            string loaiHang = cboLoaiHang.SelectedItem?.ToString() ?? "";
            string donViTinh = cboDVT.SelectedItem?.ToString() ?? "";
            int tonMin = (int)numTonMin.Value;
            int tonMax = (int)numTonMax.Value;

            if (string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Vui lòng nhập Mã sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSP.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenSP))
            {
                MessageBox.Show("Vui lòng nhập Tên sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus();
                return;
            }

            // Xử lý đơn giá nhập (hỗ trợ định dạng số)
            string rawGiaNhap = txtGiaNhap.Text.Trim().Replace(",", "").Replace(".", "");
            if (!decimal.TryParse(rawGiaNhap, out decimal giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Đơn giá nhập không hợp lệ! Vui lòng nhập số tiền >= 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaNhap.Focus();
                return;
            }

            // Kiểm tra ràng buộc CK_HangHoa_TonToiDa: TonToiDa >= TonToiThieu
            if (tonMax < tonMin)
            {
                MessageBox.Show("Định mức Tồn tối đa phải lớn hơn hoặc bằng Tồn tối thiểu!", "Lỗi logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numTonMax.Focus();
                return;
            }

            try
            {
                if (isAddingNew)
                {
                    // Kiểm tra trùng mã sản phẩm PK_HangHoa
                    string sqlCheck = "SELECT COUNT(*) FROM dbo.HangHoa WHERE MaSP = @MaSP";
                    SqlParameter[] parCheck = { new SqlParameter("@MaSP", maSP) };
                    object result = DatabaseHelper.ExecuteScalar(sqlCheck, parCheck);

                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        MessageBox.Show($"Mã sản phẩm '{maSP}' đã tồn tại trong kho! Vui lòng kiểm tra lại.",
                                        "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMaSP.Focus();
                        return;
                    }

                    // Thực hiện INSERT
                    string sqlInsert = @"INSERT INTO dbo.HangHoa (MaSP, TenSP, LoaiHang, DonViTinh, SoLuongTon, TonToiThieu, TonToiDa, GiaNhapGanNhat)
                                         VALUES (@MaSP, @TenSP, @LoaiHang, @DonViTinh, @SoLuongTon, @TonToiThieu, @TonToiDa, @GiaNhapGanNhat)";

                    SqlParameter[] parInsert = new SqlParameter[]
                    {
                        new SqlParameter("@MaSP", maSP),
                        new SqlParameter("@TenSP", tenSP),
                        new SqlParameter("@LoaiHang", loaiHang),
                        new SqlParameter("@DonViTinh", donViTinh),
                        new SqlParameter("@SoLuongTon", 0), // Hàng mới tạo mặc định số lượng = 0 (tăng qua Phiếu Nhập)
                        new SqlParameter("@TonToiThieu", tonMin),
                        new SqlParameter("@TonToiDa", tonMax),
                        new SqlParameter("@GiaNhapGanNhat", giaNhap)
                    };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlInsert, parInsert);
                    if (rows > 0)
                    {
                        MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Thực hiện UPDATE (không sửa trực tiếp SoLuongTon để bảo vệ toàn vẹn dữ liệu Nhập - Xuất)
                    string sqlUpdate = @"UPDATE dbo.HangHoa 
                                         SET TenSP = @TenSP,
                                             LoaiHang = @LoaiHang,
                                             DonViTinh = @DonViTinh,
                                             TonToiThieu = @TonToiThieu,
                                             TonToiDa = @TonToiDa,
                                             GiaNhapGanNhat = @GiaNhapGanNhat
                                         WHERE MaSP = @MaSP";

                    SqlParameter[] parUpdate = new SqlParameter[]
                    {
                        new SqlParameter("@MaSP", maSP),
                        new SqlParameter("@TenSP", tenSP),
                        new SqlParameter("@LoaiHang", loaiHang),
                        new SqlParameter("@DonViTinh", donViTinh),
                        new SqlParameter("@TonToiThieu", tonMin),
                        new SqlParameter("@TonToiDa", tonMax),
                        new SqlParameter("@GiaNhapGanNhat", giaNhap)
                    };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlUpdate, parUpdate);
                    if (rows > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadDanhSachHangHoa();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text.Trim();
            string tenSP = txtTenSP.Text.Trim();

            if (string.IsNullOrEmpty(maSP) || isAddingNew)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm từ bảng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra ràng buộc khóa ngoại với ChiTietPhieuNhap và ChiTietPhieuXuat
                string sqlCheckPN = "SELECT COUNT(*) FROM dbo.ChiTietPhieuNhap WHERE MaSP = @MaSP";
                SqlParameter[] parCheckPN = { new SqlParameter("@MaSP", maSP) };
                int countPN = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlCheckPN, parCheckPN) ?? 0);

                string sqlCheckPX = "SELECT COUNT(*) FROM dbo.ChiTietPhieuXuat WHERE MaSP = @MaSP";
                SqlParameter[] parCheckPX = { new SqlParameter("@MaSP", maSP) };
                int countPX = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlCheckPX, parCheckPX) ?? 0);

                if (countPN > 0 || countPX > 0)
                {
                    MessageBox.Show($"Không thể xóa sản phẩm [{maSP}] vì đã phát sinh giao dịch lịch sử trong kho ({countPN} dòng nhập, {countPX} dòng xuất)!\nĐể đảm bảo toàn vẹn dữ liệu kế toán, sản phẩm này chỉ có thể cập nhật thông tin.",
                                    "Chặn thao tác xóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // Kiểm tra tồn kho còn lại
                if (currentSoLuongTon > 0)
                {
                    DialogResult drTon = MessageBox.Show($"Sản phẩm này hiện vẫn còn tồn kho {currentSoLuongTon} đơn vị. Bạn có chắc chắn vẫn muốn xóa không?",
                                                         "Cảnh báo tồn kho", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (drTon != DialogResult.Yes) return;
                }

                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm:\n[{maSP}] - {tenSP} không?",
                                                  "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    string sqlDelete = "DELETE FROM dbo.HangHoa WHERE MaSP = @MaSP";
                    SqlParameter[] parDelete = { new SqlParameter("@MaSP", maSP) };

                    int rows = DatabaseHelper.ExecuteNonQuery(sqlDelete, parDelete);
                    if (rows > 0)
                    {
                        MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDanhSachHangHoa();
                        LamMoiForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoiForm();
            LoadDanhSachHangHoa();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            TimKiemHangHoa();
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimKiemHangHoa();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void dgvHangHoa_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvHangHoa.Rows.Count)
            {
                DataGridViewRow row = dgvHangHoa.Rows[e.RowIndex];
                if (row == null || row.Cells["MaSP"].Value == null) return;

                txtMaSP.Text = row.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = row.Cells["TenSP"].Value?.ToString() ?? "";

                string loaiHang = row.Cells["LoaiHang"].Value?.ToString() ?? "";
                if (cboLoaiHang.Items.Contains(loaiHang))
                    cboLoaiHang.SelectedItem = loaiHang;

                string dvt = row.Cells["DonViTinh"].Value?.ToString() ?? "";
                if (cboDVT.Items.Contains(dvt))
                    cboDVT.SelectedItem = dvt;

                if (decimal.TryParse(row.Cells["GiaNhapGanNhat"].Value?.ToString(), out decimal giaNhap))
                {
                    txtGiaNhap.Text = giaNhap.ToString("N0", CultureInfo.InvariantCulture);
                }

                if (int.TryParse(row.Cells["SoLuongTon"].Value?.ToString(), out int slTon))
                {
                    currentSoLuongTon = slTon;
                }

                if (int.TryParse(row.Cells["TonToiThieu"].Value?.ToString(), out int tonMin))
                {
                    numTonMin.Value = Math.Max(numTonMin.Minimum, Math.Min(numTonMin.Maximum, tonMin));
                }

                if (int.TryParse(row.Cells["TonToiDa"].Value?.ToString(), out int tonMax))
                {
                    numTonMax.Value = Math.Max(numTonMax.Minimum, Math.Min(numTonMax.Maximum, tonMax));
                }

                txtMaSP.Enabled = false;
                isAddingNew = false;
            }
        }

        private void LamMoiForm()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtGiaNhap.Text = "0";
            txtTimKiem.Clear();

            if (cboLoaiHang.Items.Count > 0) cboLoaiHang.SelectedIndex = 0;
            if (cboDVT.Items.Count > 0) cboDVT.SelectedIndex = 0;

            numTonMin.Value = 10;
            numTonMax.Value = 500;
            currentSoLuongTon = 0;

            txtMaSP.Enabled = true;
            isAddingNew = true;
            txtMaSP.Focus();
        }
        #endregion
    }
}