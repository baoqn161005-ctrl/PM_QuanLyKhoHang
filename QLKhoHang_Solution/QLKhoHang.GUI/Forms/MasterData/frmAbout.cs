using System;
using System.Windows.Forms;
using QLKhoHang.GUI.Helpers;

namespace QLKhoHang.GUI
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            SetupUI();
        }

        private void SetupUI()
        {
            // Thiết lập font chuẩn Segoe UI theo Mục 1.3 Báo cáo tiểu luận
            this.Font = UIThemeHelper.ControlFont;

            // Định dạng nút bấm chuẩn bảng màu UIThemeHelper
            UIThemeHelper.ApplyButtonStyle(btnDong, UIThemeHelper.ButtonType.Neutral);

            // Bật phím Esc để đóng nhanh hộp thoại
            this.CancelButton = btnDong;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}