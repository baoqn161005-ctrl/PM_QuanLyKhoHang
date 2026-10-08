using System.Windows.Forms;

namespace QLKhoHang.GUI.Forms.Auth
{
    /// <summary>
    /// Màn hình đăng nhập khung; nghiệp vụ sẽ được bổ sung ở giai đoạn xác thực.
    /// </summary>
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Đăng nhập - QLKhoHang";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new System.Drawing.Size(800, 450);
        }
    }
}
