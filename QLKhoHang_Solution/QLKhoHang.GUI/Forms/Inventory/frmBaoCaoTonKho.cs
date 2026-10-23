using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClosedXML.Excel;
using QLKhoHang.DAL;

namespace QLKhoHang.GUI.Forms.Inventory
{
    public partial class frmBaoCaoTonKho : Form
    {
        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("vi-VN");
        private DataTable _baoCaoTable;

        public frmBaoCaoTonKho()
        {
            InitializeComponent();
            BackColor = ColorTranslator.FromHtml("#F8FAFC");
        }

        private void frmBaoCaoTonKho_Load(object sender, EventArgs e)
        {
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDenNgay.Value = DateTime.Today;
            NapDanhSachLoaiHang();
            CapNhatBieuDo(null);
        }

        private void NapDanhSachLoaiHang()
        {
            try
            {
                const string sql = @"
SELECT DISTINCT LoaiHang
FROM dbo.HangHoa
WHERE LoaiHang IS NOT NULL
ORDER BY LoaiHang;";
                DataTable loaiHang = DatabaseHelper.ExecuteQuery(sql, null);
                DataRow tatCa = loaiHang.NewRow();
                tatCa["LoaiHang"] = "Tất cả";
                loaiHang.Rows.InsertAt(tatCa, 0);

                cboLoaiHang.DisplayMember = "LoaiHang";
                cboLoaiHang.ValueMember = "LoaiHang";
                cboLoaiHang.DataSource = loaiHang;
                cboLoaiHang.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                btnLocBaoCao.Enabled = false;
                HienThiLoi("Không thể tải danh mục nhóm hàng. Vui lòng kiểm tra kết nối cơ sở dữ liệu. " + ex.Message);
            }
        }

        private void btnLocBaoCao_Click(object sender, EventArgs e)
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            if (tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Khoảng thời gian không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLocBaoCao.Enabled = false;
            btnXuatExcel.Enabled = false;
            dgvBaoCaoTonKho.DataSource = null;
            _baoCaoTable = null;
            DatLaiKPI();
            CapNhatBieuDo(null);
            UseWaitCursor = true;

            try
            {
                string loaiHang = cboLoaiHang.SelectedValue == null
                    ? string.Empty
                    : Convert.ToString(cboLoaiHang.SelectedValue, CultureInfo.CurrentCulture);
                if (string.Equals(loaiHang, "Tất cả", StringComparison.CurrentCultureIgnoreCase))
                {
                    loaiHang = string.Empty;
                }

                string tuKhoa = txtTimKiem.Text.Trim();
                DataTable baoCao = DatabaseHelper.ExecuteQuery(
                    @";WITH NhapKy AS
(
    SELECT CTPN.MaSP, SUM(CONVERT(BIGINT, CTPN.SoLuongNhap)) AS SLNhapTrongKy
    FROM dbo.ChiTietPhieuNhap AS CTPN
    INNER JOIN dbo.PhieuNhap AS PN ON PN.MaPN = CTPN.MaPN
    WHERE PN.NgayNhap >= @TuNgay
      AND PN.NgayNhap < DATEADD(DAY, 1, @DenNgay)
    GROUP BY CTPN.MaSP
), XuatKy AS
(
    SELECT CTPX.MaSP, SUM(CONVERT(BIGINT, CTPX.SoLuongXuat)) AS SLXuatTrongKy
    FROM dbo.ChiTietPhieuXuat AS CTPX
    INNER JOIN dbo.PhieuXuat AS PX ON PX.MaPX = CTPX.MaPX
    WHERE PX.NgayXuat >= @TuNgay
      AND PX.NgayXuat < DATEADD(DAY, 1, @DenNgay)
    GROUP BY CTPX.MaSP
), NhapSauKy AS
(
    SELECT CTPN.MaSP, SUM(CONVERT(BIGINT, CTPN.SoLuongNhap)) AS TongNhapSauKy
    FROM dbo.ChiTietPhieuNhap AS CTPN
    INNER JOIN dbo.PhieuNhap AS PN ON PN.MaPN = CTPN.MaPN
    WHERE PN.NgayNhap >= DATEADD(DAY, 1, @DenNgay)
    GROUP BY CTPN.MaSP
), XuatSauKy AS
(
    SELECT CTPX.MaSP, SUM(CONVERT(BIGINT, CTPX.SoLuongXuat)) AS TongXuatSauKy
    FROM dbo.ChiTietPhieuXuat AS CTPX
    INNER JOIN dbo.PhieuXuat AS PX ON PX.MaPX = CTPX.MaPX
    WHERE PX.NgayXuat >= DATEADD(DAY, 1, @DenNgay)
    GROUP BY CTPX.MaSP
), TonTheoKy AS
(
    SELECT HH.MaSP,
           HH.TenSP,
           HH.LoaiHang,
           HH.DonViTinh,
           HH.GiaNhapGanNhat,
           HH.TonToiThieu,
           HH.TonToiDa,
           COALESCE(NK.SLNhapTrongKy, CONVERT(BIGINT, 0)) AS SLNhap,
           COALESCE(XK.SLXuatTrongKy, CONVERT(BIGINT, 0)) AS SLXuat,
           CONVERT(BIGINT, HH.SoLuongTon)
             - COALESCE(NSK.TongNhapSauKy, CONVERT(BIGINT, 0))
             + COALESCE(XSK.TongXuatSauKy, CONVERT(BIGINT, 0)) AS TonCuoi
    FROM dbo.HangHoa AS HH
    LEFT JOIN NhapKy AS NK ON NK.MaSP = HH.MaSP
    LEFT JOIN XuatKy AS XK ON XK.MaSP = HH.MaSP
    LEFT JOIN NhapSauKy AS NSK ON NSK.MaSP = HH.MaSP
    LEFT JOIN XuatSauKy AS XSK ON XSK.MaSP = HH.MaSP
)
SELECT ROW_NUMBER() OVER (ORDER BY MaSP ASC) AS STT,
       MaSP,
       TenSP,
       LoaiHang,
       DonViTinh,
       TonCuoi - SLNhap + SLXuat AS TonDau,
       SLNhap,
       SLXuat,
       TonCuoi,
       CONVERT(DECIMAL(28, 2), TonCuoi * GiaNhapGanNhat) AS GiaTriTon,
       CASE
           WHEN TonCuoi <= 0 THEN N'Hết hàng'
           WHEN TonCuoi <= TonToiThieu THEN N'Chạm ngưỡng tối thiểu'
           WHEN TonCuoi > TonToiDa THEN N'Vượt tồn tối đa'
           ELSE N'Bình thường'
       END AS TrangThai,
       TonToiThieu,
       TonToiDa,
       SLNhap + SLXuat AS TongLuanChuyen
FROM TonTheoKy
WHERE (@LoaiHang = N'' OR LoaiHang = @LoaiHang)
  AND (@TuKhoa = N'' OR MaSP LIKE N'%' + @TuKhoa + N'%' OR TenSP LIKE N'%' + @TuKhoa + N'%')
ORDER BY MaSP ASC;",
                    new[]
                    {
                        new SqlParameter("@TuNgay", SqlDbType.DateTime) { Value = tuNgay },
                        new SqlParameter("@DenNgay", SqlDbType.DateTime) { Value = denNgay },
                        new SqlParameter("@LoaiHang", SqlDbType.NVarChar, 50) { Value = loaiHang },
                        new SqlParameter("@TuKhoa", SqlDbType.NVarChar, 200) { Value = tuKhoa }
                    });

                _baoCaoTable = baoCao;
                dgvBaoCaoTonKho.DataSource = baoCao;
                CapNhatKPI(baoCao);
                CapNhatBieuDo(baoCao);
                btnXuatExcel.Enabled = true;
            }
            catch (Exception ex)
            {
                HienThiLoi("Không thể tổng hợp báo cáo tồn kho. Vui lòng kiểm tra dữ liệu và kết nối SQL Server. " + ex.Message);
            }
            finally
            {
                UseWaitCursor = false;
                btnLocBaoCao.Enabled = true;
            }
        }

        private void CapNhatKPI(DataTable baoCao)
        {
            decimal tongNhap = 0;
            decimal tongXuat = 0;
            decimal tongGiaTriTon = 0;
            foreach (DataRow row in baoCao.Rows)
            {
                tongNhap += Convert.ToDecimal(row["SLNhap"], CultureInfo.InvariantCulture);
                tongXuat += Convert.ToDecimal(row["SLXuat"], CultureInfo.InvariantCulture);
                tongGiaTriTon += Convert.ToDecimal(row["GiaTriTon"], CultureInfo.InvariantCulture);
            }

            lblTongMaHang.Text = baoCao.Rows.Count.ToString("N0", _culture);
            lblTongSLNhap.Text = tongNhap.ToString("N0", _culture);
            lblTongSLXuat.Text = tongXuat.ToString("N0", _culture);
            lblTongGiaTriTon.Text = tongGiaTriTon.ToString("N0", _culture) + " VNĐ";
        }

        private void DatLaiKPI()
        {
            lblTongMaHang.Text = "0";
            lblTongSLNhap.Text = "0";
            lblTongSLXuat.Text = "0";
            lblTongGiaTriTon.Text = "0 VNĐ";
        }

        private void CapNhatBieuDo(DataTable baoCao)
        {
            seriesSoLuongNhap.Points.Clear();
            seriesSoLuongXuat.Points.Clear();
            if (baoCao == null || baoCao.Rows.Count == 0)
            {
                return;
            }

            DataView topHangHoa = baoCao.DefaultView;
            topHangHoa.Sort = "TongLuanChuyen DESC, MaSP ASC";
            int soMatHang = Math.Min(10, topHangHoa.Count);
            for (int index = 0; index < soMatHang; index++)
            {
                DataRowView row = topHangHoa[index];
                string tenHienThi = Convert.ToString(row["TenSP"], CultureInfo.CurrentCulture);
                double slNhap = Convert.ToDouble(row["SLNhap"], CultureInfo.InvariantCulture);
                double slXuat = Convert.ToDouble(row["SLXuat"], CultureInfo.InvariantCulture);
                seriesSoLuongNhap.Points.AddXY(tenHienThi, slNhap);
                seriesSoLuongXuat.Points.AddXY(tenHienThi, slXuat);
            }
        }

        private void dgvBaoCaoTonKho_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataRowView row = dgvBaoCaoTonKho.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (row == null)
            {
                return;
            }

            decimal tonCuoi = Convert.ToDecimal(row["TonCuoi"], CultureInfo.InvariantCulture);
            decimal tonToiThieu = Convert.ToDecimal(row["TonToiThieu"], CultureInfo.InvariantCulture);
            if (tonCuoi <= tonToiThieu)
            {
                e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                e.CellStyle.BackColor = Color.FromArgb(254, 249, 195);
                e.CellStyle.SelectionBackColor = Color.FromArgb(253, 230, 138);
                e.CellStyle.SelectionForeColor = Color.FromArgb(153, 27, 27);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (_baoCaoTable == null)
            {
                MessageBox.Show("Vui lòng tổng hợp báo cáo trước khi xuất Excel.", "Chưa có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Xuất báo cáo tổng hợp nhập - xuất - tồn";
                dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                dialog.DefaultExt = "xlsx";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = "BaoCaoTonKho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    XuatBaoCaoExcel(dialog.FileName);
                    MessageBox.Show("Đã xuất báo cáo thành công:\n" + dialog.FileName, "Xuất Excel thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                IXLWorksheet worksheet = workbook.Worksheets.Add("Bao cao ton kho");
                worksheet.Range(1, 1, 1, dgvBaoCaoTonKho.Columns.Count).Merge();
                worksheet.Cell(1, 1).Value = "BÁO CÁO TỔNG HỢP NHẬP - XUẤT - TỒN KHO HÀNG";
                worksheet.Range(2, 1, 2, dgvBaoCaoTonKho.Columns.Count).Merge();
                worksheet.Cell(2, 1).Value = "Kỳ báo cáo: " + dtpTuNgay.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    + " - " + dtpDenNgay.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                const int headerRow = 4;
                const int firstDataRow = headerRow + 1;
                for (int columnIndex = 0; columnIndex < dgvBaoCaoTonKho.Columns.Count; columnIndex++)
                {
                    IXLCell headerCell = worksheet.Cell(headerRow, columnIndex + 1);
                    headerCell.Value = dgvBaoCaoTonKho.Columns[columnIndex].HeaderText;
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.FontColor = XLColor.White;
                    headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int excelRow = firstDataRow;
                foreach (DataGridViewRow gridRow in dgvBaoCaoTonKho.Rows)
                {
                    if (gridRow.IsNewRow)
                    {
                        continue;
                    }

                    for (int columnIndex = 0; columnIndex < dgvBaoCaoTonKho.Columns.Count; columnIndex++)
                    {
                        GhiGiaTriExcel(worksheet.Cell(excelRow, columnIndex + 1), gridRow.Cells[columnIndex].Value);
                    }

                    if ((excelRow - firstDataRow) % 2 == 1)
                    {
                        worksheet.Range(excelRow, 1, excelRow, dgvBaoCaoTonKho.Columns.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                    }

                    excelRow++;
                }

                if (dgvBaoCaoTonKho.Rows.Count == 0)
                {
                    worksheet.Cell(firstDataRow, 3).Value = "Không có mặt hàng phù hợp với bộ lọc.";
                    excelRow++;
                }

                int totalRow = excelRow;
                worksheet.Range(totalRow, 1, totalRow, 5).Merge();
                worksheet.Cell(totalRow, 1).Value = "TỔNG CỘNG";
                worksheet.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Cell(totalRow, 1).Style.Font.Bold = true;
                for (int column = 6; column <= 10; column++)
                {
                    string columnLetter = ((char)('A' + column - 1)).ToString();
                    worksheet.Cell(totalRow, column).FormulaA1 = "=SUM(" + columnLetter + firstDataRow + ":" + columnLetter + (excelRow - 1) + ")";
                    worksheet.Cell(totalRow, column).Style.Font.Bold = true;
                }

                IXLRange reportRange = worksheet.Range(headerRow, 1, totalRow, dgvBaoCaoTonKho.Columns.Count);
                reportRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                reportRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                reportRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
                reportRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");
                reportRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(totalRow, 1, totalRow, dgvBaoCaoTonKho.Columns.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");
                worksheet.Range(firstDataRow, 6, totalRow, 9).Style.NumberFormat.Format = "#,##0";
                worksheet.Range(firstDataRow, 10, totalRow, 10).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                worksheet.Range(firstDataRow, 6, totalRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range(1, 1, totalRow, dgvBaoCaoTonKho.Columns.Count).Style.Font.FontName = "Segoe UI";
                worksheet.Range(1, 1, 1, dgvBaoCaoTonKho.Columns.Count).Style.Font.FontSize = 16;
                worksheet.Range(1, 1, 1, dgvBaoCaoTonKho.Columns.Count).Style.Font.Bold = true;
                worksheet.Range(1, 1, 1, dgvBaoCaoTonKho.Columns.Count).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Column(10).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                worksheet.Columns(1, dgvBaoCaoTonKho.Columns.Count).AdjustToContents();
                worksheet.Column(3).Width = Math.Max(worksheet.Column(3).Width, 28);
                worksheet.Column(11).Width = Math.Max(worksheet.Column(11).Width, 25);
                worksheet.SheetView.FreezeRows(headerRow);
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
            MessageBox.Show(message, "Báo cáo tồn kho", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
