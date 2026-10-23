using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLKhoHang.GUI.Helpers
{
    /// <summary>
    /// Lớp hỗ trợ quy chuẩn giao diện (UI/UX Guidelines) theo tài liệu dự án Quản lý Kho hàng:
    /// - Font hệ thống: Segoe UI
    /// - Bảng màu: Header/Sidebar (#1E293B), Workspace (#F8FAFC), Nút Thêm/Lưu (#0D9488 / #2563EB), Nút Xóa (#DC2626)
    /// - Chuẩn hóa DataGridView: ReadOnly, FullRowSelect, tô màu xen kẽ hàng
    /// - Cảnh báo trực quan tồn kho an toàn (Tô màu đỏ cho sản phẩm chạm ngưỡng tồn tối thiểu)
    /// </summary>
    public static class UIThemeHelper
    {
        #region 1. BẢNG MÀU CHUẨN (PALETTE COLORS)
        // Màu giao diện chính
        public static readonly Color HeaderSidebarColor = ColorTranslator.FromHtml("#1E293B"); // Slate đậm
        public static readonly Color WorkspaceBgColor = ColorTranslator.FromHtml("#F8FAFC"); // Nền xám sáng
        public static readonly Color CardBgColor = Color.White;
        public static readonly Color BorderColor = ColorTranslator.FromHtml("#E2E8F0"); // Viền xám nhạt

        // Màu nút bấm hành động (Action Buttons)
        public static readonly Color PrimaryTealColor = ColorTranslator.FromHtml("#0D9488"); // Nút Thêm / Lưu (Teal)
        public static readonly Color PrimaryBlueColor = ColorTranslator.FromHtml("#2563EB"); // Nút Thao tác / Tìm kiếm (Blue)
        public static readonly Color DangerRedColor = ColorTranslator.FromHtml("#DC2626"); // Nút Cảnh báo / Xóa (Red)
        public static readonly Color NeutralGrayColor = ColorTranslator.FromHtml("#64748B"); // Nút Làm mới / Hủy (Slate Gray)

        // Màu cảnh báo tồn kho trên DataGridView
        public static readonly Color WarningRowBgColor = ColorTranslator.FromHtml("#FEE2E2"); // Đỏ nhạt nền dòng cảnh báo
        public static readonly Color WarningTextRedColor = ColorTranslator.FromHtml("#B91C1C"); // Đỏ đậm chữ cảnh báo
        public static readonly Color NormalRowBgColor = Color.White;
        public static readonly Color AlternatingRowBgColor = ColorTranslator.FromHtml("#F1F5F9"); // Xen kẽ hàng
        public static readonly Color HeaderRowBgColor = ColorTranslator.FromHtml("#0F172A"); // Header DataGridView
        public static readonly Color SelectedRowBgColor = ColorTranslator.FromHtml("#93C5FD"); // Dòng đang chọn
        #endregion

        #region 2. BỘ FONT SEGOE UI CHUẨN
        public static readonly Font TitleFont = new Font("Segoe UI", 15f, FontStyle.Bold);
        public static readonly Font SubTitleFont = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font ControlFont = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font ControlBoldFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font SmallItalicFont = new Font("Segoe UI", 8.5f, FontStyle.Italic);
        public static readonly Font GridHeaderFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        #endregion

        #region 3. CHUẨN HÓA DATAGRIDVIEW
        /// <summary>
        /// Áp dụng toàn bộ quy chuẩn DataGridView theo Mục 1.3 Báo cáo tiểu luận:
        /// ReadOnly = true, SelectionMode = FullRowSelect, MultiSelect = false,
        /// AutoSizeColumnsMode = Fill, RowHeadersVisible = false, tô màu xen kẽ hàng.
        /// </summary>
        public static void ApplyStandardGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            // Thiết lập tương tác & hiển thị
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = WorkspaceBgColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BorderColor;
            dgv.EnableHeadersVisualStyles = false;

            // Định dạng tiêu đề cột (Column Headers)
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderRowBgColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = GridHeaderFont;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Định dạng hàng dữ liệu (Rows)
            dgv.RowTemplate.Height = 34;
            dgv.DefaultCellStyle.Font = ControlFont;
            dgv.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#1E293B");
            dgv.DefaultCellStyle.BackColor = NormalRowBgColor;
            dgv.DefaultCellStyle.SelectionBackColor = SelectedRowBgColor;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Định dạng màu xen kẽ hàng (Alternating rows)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternatingRowBgColor;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#1E293B");
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectedRowBgColor;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black;
        }

        /// <summary>
        /// Tô màu cảnh báo trực quan cho dòng sản phẩm có lượng tồn chạm hoặc dưới ngưỡng tối thiểu:
        /// SoLuongTon <= TonToiThieu.
        /// Thường gọi bên trong sự kiện dgvHangHoa_CellFormatting.
        /// </summary>
        public static void FormatHangHoaRowWarning(DataGridViewCellFormattingEventArgs e, DataGridView dgv, string colSoLuongTon, string colTonToiThieu)
        {
            if (dgv == null || e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;

            var row = dgv.Rows[e.RowIndex];
            if (row == null || row.DataBoundItem == null) return;

            if (row.Cells[colSoLuongTon]?.Value != null && row.Cells[colTonToiThieu]?.Value != null)
            {
                if (int.TryParse(row.Cells[colSoLuongTon].Value.ToString(), out int soLuongTon) &&
                    int.TryParse(row.Cells[colTonToiThieu].Value.ToString(), out int tonToiThieu))
                {
                    if (soLuongTon <= tonToiThieu)
                    {
                        // Tô màu nền đỏ nhạt cho cả dòng
                        row.DefaultCellStyle.BackColor = WarningRowBgColor;
                        row.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#FECACA");

                        // Riêng ô số lượng tồn in chữ đỏ đậm cảnh báo
                        if (dgv.Columns[e.ColumnIndex].Name == colSoLuongTon)
                        {
                            e.CellStyle.ForeColor = WarningTextRedColor;
                            e.CellStyle.Font = ControlBoldFont;
                        }
                    }
                }
            }
        }
        #endregion

        #region 4. QUY CHUẨN NÚT BẤM (BUTTONS)
        public enum ButtonType
        {
            SaveOrAdd,  // Lưu hoặc Thêm mới (#0D9488 Teal)
            Primary,    // Tìm kiếm, Xem (#2563EB Blue)
            Danger,     // Xóa, Hủy đơn (#DC2626 Red)
            Neutral     // Làm mới, Bỏ qua (#64748B Slate)
        }

        /// <summary>
        /// Định dạng nút bấm phẳng hiện đại, bo nhẹ cạnh và đúng bảng màu quy chuẩn.
        /// </summary>
        public static void ApplyButtonStyle(Button btn, ButtonType type)
        {
            if (btn == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = ControlBoldFont;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Height = Math.Max(btn.Height, 36);

            switch (type)
            {
                case ButtonType.SaveOrAdd:
                    btn.BackColor = PrimaryTealColor;
                    btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#0F766E");
                    break;
                case ButtonType.Primary:
                    btn.BackColor = PrimaryBlueColor;
                    btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#1D4ED8");
                    break;
                case ButtonType.Danger:
                    btn.BackColor = DangerRedColor;
                    btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#B91C1C");
                    break;
                case ButtonType.Neutral:
                    btn.BackColor = NeutralGrayColor;
                    btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#475569");
                    break;
            }
        }
        #endregion

        #region 5. QUY CÁCH FORM CON (EMBEDDED CHILD FORM)
        /// <summary>
        /// Cấu hình Form con để nhúng trực tiếp vào Panel Workspace của frmMain theo Mục 1.3:
        /// TopLevel = false, FormBorderStyle = None, Dock = Fill.
        /// </summary>
        public static void EmbedChildForm(Form childForm, Panel targetPanel)
        {
            if (childForm == null || targetPanel == null) return;

            if (targetPanel.Controls.Count > 0)
            {
                Control current = targetPanel.Controls[0];
                targetPanel.Controls.Remove(current);
                current.Dispose();
            }

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            targetPanel.Controls.Add(childForm);
            targetPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        #endregion
    }
}