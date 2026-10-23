using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ClosedXML.Excel;
using QLKhoHang.DAL;

namespace QLKhoHang.GUI.Forms.Inventory
{
    public partial class frmTheKho : Form
    {
        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("vi-VN");
        private string _donViTinh = string.Empty;

        public frmTheKho()
        {
            InitializeComponent();
            BackColor = ColorTranslator.FromHtml("#F8FAFC");
        }

        private void frmTheKho_Load(object sender, EventArgs e)
        {
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDenNgay.Value = DateTime.Today;

            try
            {
                const string sql = @"
SELECT MaSP, CONVERT(NVARCHAR(20), MaSP) + N' - ' + TenSP AS TenHienThi
FROM dbo.HangHoa
ORDER BY TenSP, MaSP;";
                DataTable hangHoa = DatabaseHelper.ExecuteQuery(sql, null);
                cboHangHoa.DisplayMember = "TenHienThi";
                cboHangHoa.ValueMember = "MaSP";
                cboHangHoa.DataSource = hangHoa;

                bool coHangHoa = hangHoa.Rows.Count > 0;
                cboHangHoa.Enabled = coHangHoa;
                btnXemTheKho.Enabled = coHangHoa;
                btnXuatExcel.Enabled = false;

                if (!coHangHoa)
                {
                    lblGhiChu.Text = "Chưa có mặt hàng trong danh mục.";
                }
            }
            catch (Exception ex)
            {
                btnXemTheKho.Enabled = false;
                btnXuatExcel.Enabled = false;
                HienThiLoi("Không thể tải danh sách mặt hàng. Vui lòng kiểm tra kết nối cơ sở dữ liệu. " + ex.Message);
            }
        }

        private void btnXemTheKho_Click(object sender, EventArgs e)
        {
            if (cboHangHoa.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn mặt hàng cần xem.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            if (tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Khoảng thời gian không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnXemTheKho.Enabled = false;
            btnXuatExcel.Enabled = false;
            dgvTheKho.DataSource = null;
            lblTonDau.Text = "—";
            lblTongNhap.Text = "—";
            lblTongXuat.Text = "—";
            lblTonCuoi.Text = "—";
            UseWaitCursor = true;
            try
            {
                string maSP = Convert.ToString(cboHangHoa.SelectedValue, CultureInfo.InvariantCulture);
                DataTable thongTinHang = DatabaseHelper.ExecuteQuery(
                    @"SELECT SoLuongTon, DonViTinh, GiaNhapGanNhat
FROM dbo.HangHoa
WHERE MaSP = @MaSP;",
                    new[]
                    {
                        new SqlParameter("@MaSP", SqlDbType.VarChar, 20) { Value = maSP }
                    });

                if (thongTinHang.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Mặt hàng đã bị xóa hoặc không còn tồn tại trong danh mục.");
                }

                DataRow hangHoa = thongTinHang.Rows[0];
                decimal soLuongTonHienTai = Convert.ToDecimal(hangHoa["SoLuongTon"], CultureInfo.InvariantCulture);
                decimal giaNhapGanNhat = Convert.ToDecimal(hangHoa["GiaNhapGanNhat"], CultureInfo.InvariantCulture);
                _donViTinh = Convert.ToString(hangHoa["DonViTinh"], CultureInfo.CurrentCulture);

                decimal tongNhapTuNgayDenNay = LayTongSoLuong(
                    @"SELECT COALESCE(SUM(CONVERT(BIGINT, CTPN.SoLuongNhap)), 0)
FROM dbo.ChiTietPhieuNhap AS CTPN
INNER JOIN dbo.PhieuNhap AS PN ON PN.MaPN = CTPN.MaPN
WHERE CTPN.MaSP = @MaSP
  AND PN.NgayNhap >= @TuNgay
  AND PN.NgayNhap <= GETDATE();",
                    maSP,
                    tuNgay);
                decimal tongXuatTuNgayDenNay = LayTongSoLuong(
                    @"SELECT COALESCE(SUM(CONVERT(BIGINT, CTPX.SoLuongXuat)), 0)
FROM dbo.ChiTietPhieuXuat AS CTPX
INNER JOIN dbo.PhieuXuat AS PX ON PX.MaPX = CTPX.MaPX
WHERE CTPX.MaSP = @MaSP
  AND PX.NgayXuat >= @TuNgay
  AND PX.NgayXuat <= GETDATE();",
                    maSP,
                    tuNgay);

                decimal tonDauKy = soLuongTonHienTai - tongNhapTuNgayDenNay + tongXuatTuNgayDenNay;
                DataTable giaoDich = DatabaseHelper.ExecuteQuery(
                    @"SELECT PN.NgayNhap AS NgayGhi,
       PN.MaPN AS ChungTu,
       N'Nhập hàng ' + ISNULL(NCC.TenNCC, N'') AS DienGiai,
       CTPN.DonGiaNhap AS DonGia,
       CTPN.SoLuongNhap AS SLNhap,
       0 AS SLXuat
FROM dbo.PhieuNhap AS PN
INNER JOIN dbo.ChiTietPhieuNhap AS CTPN ON CTPN.MaPN = PN.MaPN
LEFT JOIN dbo.NhaCungCap AS NCC ON NCC.MaNCC = PN.MaNCC
WHERE CTPN.MaSP = @MaSP
  AND PN.NgayNhap >= @TuNgay
  AND PN.NgayNhap < DATEADD(DAY, 1, @DenNgay)
UNION ALL
SELECT PX.NgayXuat AS NgayGhi,
       PX.MaPX AS ChungTu,
       N'Xuất giao ' + ISNULL(PX.NguoiNhan, N'') + N' (' + ISNULL(PX.LyDoXuat, N'') + N')' AS DienGiai,
       CTPX.DonGiaXuat AS DonGia,
       0 AS SLNhap,
       CTPX.SoLuongXuat AS SLXuat
FROM dbo.PhieuXuat AS PX
INNER JOIN dbo.ChiTietPhieuXuat AS CTPX ON CTPX.MaPX = PX.MaPX
WHERE CTPX.MaSP = @MaSP
  AND PX.NgayXuat >= @TuNgay
  AND PX.NgayXuat < DATEADD(DAY, 1, @DenNgay)
ORDER BY NgayGhi ASC, ChungTu ASC;",
                    new[]
                    {
                        new SqlParameter("@MaSP", SqlDbType.VarChar, 20) { Value = maSP },
                        new SqlParameter("@TuNgay", SqlDbType.DateTime) { Value = tuNgay },
                        new SqlParameter("@DenNgay", SqlDbType.DateTime) { Value = denNgay }
                    });

                DataTable theKho = TaoBangTheKho();
                decimal soDuHienTai = tonDauKy;
                theKho.Rows.Add(
                    tuNgay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    "DUKY",
                    "Tồn đầu kỳ",
                    giaNhapGanNhat,
                    "-",
                    "-",
                    soDuHienTai);

                decimal tongNhapTrongKy = 0;
                decimal tongXuatTrongKy = 0;
                foreach (DataRow giaoDichRow in giaoDich.Rows)
                {
                    DateTime ngayGhi = Convert.ToDateTime(giaoDichRow["NgayGhi"], CultureInfo.InvariantCulture);
                    decimal donGia = Convert.ToDecimal(giaoDichRow["DonGia"], CultureInfo.InvariantCulture);
                    decimal slNhap = Convert.ToDecimal(giaoDichRow["SLNhap"], CultureInfo.InvariantCulture);
                    decimal slXuat = Convert.ToDecimal(giaoDichRow["SLXuat"], CultureInfo.InvariantCulture);
                    soDuHienTai = soDuHienTai + slNhap - slXuat;
                    tongNhapTrongKy += slNhap;
                    tongXuatTrongKy += slXuat;

                    theKho.Rows.Add(
                        ngayGhi.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Convert.ToString(giaoDichRow["ChungTu"], CultureInfo.CurrentCulture),
                        Convert.ToString(giaoDichRow["DienGiai"], CultureInfo.CurrentCulture),
                        donGia,
                        slNhap,
                        slXuat,
                        soDuHienTai);
                }

                dgvTheKho.DataSource = theKho;
                CapNhatKPI(tonDauKy, tongNhapTrongKy, tongXuatTrongKy, soDuHienTai);
                lblGhiChu.Text = "Đơn vị tính: " + _donViTinh + "  |  " + giaoDich.Rows.Count.ToString("N0", _culture) + " giao dịch trong kỳ.";
                btnXuatExcel.Enabled = true;
            }
            catch (Exception ex)
            {
                btnXuatExcel.Enabled = false;
                HienThiLoi("Không thể tải thẻ kho. Vui lòng kiểm tra dữ liệu và kết nối SQL Server. " + ex.Message);
            }
            finally
            {
                UseWaitCursor = false;
                btnXemTheKho.Enabled = cboHangHoa.Enabled && cboHangHoa.Items.Count > 0;
            }
        }

        private decimal LayTongSoLuong(string sql, string maSP, DateTime tuNgay)
        {
            object result = DatabaseHelper.ExecuteScalar(
                sql,
                new[]
                {
                    new SqlParameter("@MaSP", SqlDbType.VarChar, 20) { Value = maSP },
                    new SqlParameter("@TuNgay", SqlDbType.DateTime) { Value = tuNgay }
                });

            if (result == null || result == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
        }

        private static DataTable TaoBangTheKho()
        {
            DataTable table = new DataTable("TheKho");
            table.Columns.Add("NgayGhi", typeof(string));
            table.Columns.Add("ChungTu", typeof(string));
            table.Columns.Add("DienGiai", typeof(string));
            table.Columns.Add("DonGia", typeof(decimal));
            table.Columns.Add("SLNhap", typeof(object));
            table.Columns.Add("SLXuat", typeof(object));
            table.Columns.Add("SoDuTon", typeof(decimal));
            return table;
        }

        private void CapNhatKPI(decimal tonDau, decimal tongNhap, decimal tongXuat, decimal tonCuoi)
        {
            lblTonDau.Text = DinhDangSoLuong(tonDau) + " " + _donViTinh;
            lblTongNhap.Text = "+" + DinhDangSoLuong(tongNhap) + " " + _donViTinh;
            lblTongXuat.Text = "-" + DinhDangSoLuong(tongXuat) + " " + _donViTinh;
            lblTonCuoi.Text = DinhDangSoLuong(tonCuoi) + " " + _donViTinh;
        }

        private string DinhDangSoLuong(decimal value)
        {
            return value.ToString("N0", _culture);
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvTheKho.Rows.Count == 0 || cboHangHoa.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng xem dữ liệu thẻ kho trước khi xuất Excel.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Xuất báo cáo thẻ kho";
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.DefaultExt = "xlsx";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = "TheKho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    XuatBaoCaoExcel(dialog.FileName);
                    MessageBox.Show("Đã xuất báo cáo thẻ kho thành công:\n" + dialog.FileName, "Xuất Excel thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xuất file Excel. Vui lòng kiểm tra quyền ghi và thử lại. " + ex.Message, "Xuất Excel thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void XuatBaoCaoExcel(string filePath)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("The kho");
                worksheet.Range(1, 1, 1, 7).Merge();
                worksheet.Cell(1, 1).Value = "BÁO CÁO THẺ KHO";
                worksheet.Cell(2, 1).Value = "Mặt hàng: " + cboHangHoa.Text;
                worksheet.Range(2, 1, 2, 7).Merge();
                worksheet.Cell(3, 1).Value = "Kỳ báo cáo: " + dtpTuNgay.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + dtpDenNgay.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Range(3, 1, 3, 7).Merge();

                worksheet.Cell(5, 1).Value = lblTieuDeTonDau.Text;
                worksheet.Cell(5, 3).Value = lblTieuDeTongNhap.Text;
                worksheet.Cell(5, 5).Value = lblTieuDeTongXuat.Text;
                worksheet.Cell(5, 7).Value = lblTieuDeTonCuoi.Text;
                worksheet.Cell(6, 1).Value = lblTonDau.Text;
                worksheet.Cell(6, 3).Value = lblTongNhap.Text;
                worksheet.Cell(6, 5).Value = lblTongXuat.Text;
                worksheet.Cell(6, 7).Value = lblTonCuoi.Text;

                worksheet.Range(5, 1, 5, 7).Style.Font.Bold = true;
                worksheet.Range(5, 1, 5, 7).Style.Font.FontColor = XLColor.White;
                worksheet.Range(5, 1, 5, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                worksheet.Range(6, 1, 6, 7).Style.Font.Bold = true;
                worksheet.Range(6, 1, 6, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

                worksheet.Range(8, 1, 8, 7).Merge();
                worksheet.Cell(8, 1).Value = "CHI TIẾT GIAO DỊCH";
                worksheet.Cell(8, 1).Style.Font.Bold = true;
                worksheet.Cell(8, 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(8, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D9488");

                int headerRow = 9;
                for (int columnIndex = 0; columnIndex < dgvTheKho.Columns.Count; columnIndex++)
                {
                    IXLCell headerCell = worksheet.Cell(headerRow, columnIndex + 1);
                    headerCell.Value = dgvTheKho.Columns[columnIndex].HeaderText;
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.FontColor = XLColor.White;
                    headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int excelRow = headerRow + 1;
                foreach (DataGridViewRow gridRow in dgvTheKho.Rows)
                {
                    if (gridRow.IsNewRow)
                    {
                        continue;
                    }

                    for (int columnIndex = 0; columnIndex < dgvTheKho.Columns.Count; columnIndex++)
                    {
                        GhiGiaTriExcel(worksheet.Cell(excelRow, columnIndex + 1), gridRow.Cells[columnIndex].Value);
                    }

                    if ((excelRow - headerRow) % 2 == 0)
                    {
                        worksheet.Range(excelRow, 1, excelRow, dgvTheKho.Columns.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                    }

                    excelRow++;
                }

                IXLRange dataRange = worksheet.Range(headerRow, 1, excelRow - 1, dgvTheKho.Columns.Count);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
                dataRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");
                dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Column(4).Style.NumberFormat.Format = "#,##0";
                worksheet.Column(5).Style.NumberFormat.Format = "#,##0";
                worksheet.Column(6).Style.NumberFormat.Format = "#,##0";
                worksheet.Column(7).Style.NumberFormat.Format = "#,##0";
                worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                dataRange.SetAutoFilter();
                worksheet.SheetView.FreezeRows(headerRow);
                worksheet.Columns(1, 7).AdjustToContents();
                worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 13);
                worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 34);
                worksheet.Range(1, 1, 1, 7).Style.Font.FontName = "Segoe UI";
                worksheet.Range(1, 1, excelRow - 1, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(1, 1, 1, 7).Style.Font.FontSize = 16;
                worksheet.Range(1, 1, 1, 7).Style.Font.Bold = true;
                worksheet.Range(1, 1, 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 1, excelRow - 1, 7).Style.Font.FontName = "Segoe UI";
                worksheet.Row(1).Height = 28;
                workbook.SaveAs(filePath);
            }
        }

        private static void GhiGiaTriExcel(IXLCell cell, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                cell.Clear(XLClearOptions.Contents);
                return;
            }

            if (value is DateTime)
            {
                cell.Value = (DateTime)value;
                return;
            }

            if (value is byte || value is short || value is int || value is long || value is decimal || value is double || value is float)
            {
                cell.Value = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return;
            }

            cell.Value = Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        private void HienThiLoi(string message)
        {
            lblGhiChu.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            lblGhiChu.Text = message;
            MessageBox.Show(message, "Thẻ kho", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
