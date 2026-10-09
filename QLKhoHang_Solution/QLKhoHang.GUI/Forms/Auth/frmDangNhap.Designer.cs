namespace QLKhoHang.GUI.Forms.Auth
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelHero = new System.Windows.Forms.Panel();
            this.lblHeroDescription = new System.Windows.Forms.Label();
            this.lblHeroTitle = new System.Windows.Forms.Label();
            this.lblBrand = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.lblLogoGlyph = new System.Windows.Forms.Label();
            this.lblHeroCaption = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.lblFooter = new System.Windows.Forms.Label();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnDangNhap = new System.Windows.Forms.Button();
            this.chkGhiNhoTaiKhoan = new System.Windows.Forms.CheckBox();
            this.cboVaiTro = new System.Windows.Forms.ComboBox();
            this.lblVaiTro = new System.Windows.Forms.Label();
            this.txtMatKhau = new System.Windows.Forms.TextBox();
            this.lblMatKhau = new System.Windows.Forms.Label();
            this.txtTaiKhoan = new System.Windows.Forms.TextBox();
            this.lblTaiKhoan = new System.Windows.Forms.Label();
            this.lblMoTa = new System.Windows.Forms.Label();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.lblEyebrow = new System.Windows.Forms.Label();
            this.panelHero.SuspendLayout();
            this.panelLogo.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHero
            //
            this.panelHero.BackColor = System.Drawing.Color.FromArgb(16, 44, 59);
            this.panelHero.Controls.Add(this.lblHeroDescription);
            this.panelHero.Controls.Add(this.lblHeroTitle);
            this.panelHero.Controls.Add(this.lblBrand);
            this.panelHero.Controls.Add(this.panelLogo);
            this.panelHero.Controls.Add(this.lblHeroCaption);
            this.panelHero.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelHero.Location = new System.Drawing.Point(0, 0);
            this.panelHero.Name = "panelHero";
            this.panelHero.Size = new System.Drawing.Size(300, 660);
            this.panelHero.TabIndex = 0;
            //
            // lblHeroDescription
            //
            this.lblHeroDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeroDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHeroDescription.ForeColor = System.Drawing.Color.FromArgb(199, 220, 224);
            this.lblHeroDescription.Location = new System.Drawing.Point(32, 286);
            this.lblHeroDescription.Name = "lblHeroDescription";
            this.lblHeroDescription.Size = new System.Drawing.Size(236, 76);
            this.lblHeroDescription.TabIndex = 4;
            this.lblHeroDescription.Text = "Theo dõi hàng hóa rõ ràng, vận hành nhập xuất chính xác mỗi ngày.";
            //
            // lblHeroTitle
            //
            this.lblHeroTitle.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold);
            this.lblHeroTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeroTitle.Location = new System.Drawing.Point(32, 231);
            this.lblHeroTitle.Name = "lblHeroTitle";
            this.lblHeroTitle.Size = new System.Drawing.Size(236, 48);
            this.lblHeroTitle.TabIndex = 3;
            this.lblHeroTitle.Text = "Quản lý kho hàng";
            //
            // lblBrand
            //
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(91, 38);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(184, 52);
            this.lblBrand.TabIndex = 2;
            this.lblBrand.Text = "QL KHO HÀNG\r\nWAREHOUSE SYSTEM";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelLogo
            //
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(225, 244, 239);
            this.panelLogo.Controls.Add(this.lblLogoGlyph);
            this.panelLogo.Location = new System.Drawing.Point(32, 40);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(48, 48);
            this.panelLogo.TabIndex = 1;
            //
            // lblLogoGlyph
            //
            this.lblLogoGlyph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogoGlyph.Font = new System.Drawing.Font("Segoe UI Symbol", 21F, System.Drawing.FontStyle.Bold);
            this.lblLogoGlyph.ForeColor = System.Drawing.Color.FromArgb(7, 139, 124);
            this.lblLogoGlyph.Location = new System.Drawing.Point(0, 0);
            this.lblLogoGlyph.Name = "lblLogoGlyph";
            this.lblLogoGlyph.Size = new System.Drawing.Size(48, 48);
            this.lblLogoGlyph.TabIndex = 0;
            this.lblLogoGlyph.Text = "▤";
            this.lblLogoGlyph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblHeroCaption
            //
            this.lblHeroCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeroCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHeroCaption.ForeColor = System.Drawing.Color.FromArgb(199, 220, 224);
            this.lblHeroCaption.Location = new System.Drawing.Point(32, 586);
            this.lblHeroCaption.Name = "lblHeroCaption";
            this.lblHeroCaption.Size = new System.Drawing.Size(236, 42);
            this.lblHeroCaption.TabIndex = 5;
            this.lblHeroCaption.Text = "Đăng nhập bảo mật theo vai trò nhân viên";
            //
            // panelContent
            //
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Controls.Add(this.lblFooter);
            this.panelContent.Controls.Add(this.btnThoat);
            this.panelContent.Controls.Add(this.btnDangNhap);
            this.panelContent.Controls.Add(this.chkGhiNhoTaiKhoan);
            this.panelContent.Controls.Add(this.cboVaiTro);
            this.panelContent.Controls.Add(this.lblVaiTro);
            this.panelContent.Controls.Add(this.txtMatKhau);
            this.panelContent.Controls.Add(this.lblMatKhau);
            this.panelContent.Controls.Add(this.txtTaiKhoan);
            this.panelContent.Controls.Add(this.lblTaiKhoan);
            this.panelContent.Controls.Add(this.lblMoTa);
            this.panelContent.Controls.Add(this.lblTieuDe);
            this.panelContent.Controls.Add(this.lblEyebrow);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(300, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(48, 0, 48, 0);
            this.panelContent.Size = new System.Drawing.Size(620, 660);
            this.panelContent.TabIndex = 1;
            //
            // lblFooter
            //
            this.lblFooter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFooter.AutoSize = true;
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(113, 128, 144);
            this.lblFooter.Location = new System.Drawing.Point(50, 608);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(247, 15);
            this.lblFooter.TabIndex = 12;
            this.lblFooter.Text = "©  QL Kho Hàng  |  Hệ thống quản lý nội bộ";
            //
            // btnThoat
            //
            this.btnThoat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThoat.BackColor = System.Drawing.Color.FromArgb(241, 245, 247);
            this.btnThoat.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(215, 224, 231);
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThoat.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnThoat.ForeColor = System.Drawing.Color.FromArgb(67, 87, 103);
            this.btnThoat.Location = new System.Drawing.Point(385, 520);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(139, 46);
            this.btnThoat.TabIndex = 5;
            this.btnThoat.Text = "THOÁT";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.BtnThoat_Click);
            //
            // btnDangNhap
            //
            this.btnDangNhap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDangNhap.BackColor = System.Drawing.Color.FromArgb(19, 148, 132);
            this.btnDangNhap.FlatAppearance.BorderSize = 0;
            this.btnDangNhap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(7, 139, 124);
            this.btnDangNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDangNhap.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDangNhap.ForeColor = System.Drawing.Color.White;
            this.btnDangNhap.Location = new System.Drawing.Point(50, 520);
            this.btnDangNhap.Name = "btnDangNhap";
            this.btnDangNhap.Size = new System.Drawing.Size(319, 46);
            this.btnDangNhap.TabIndex = 4;
            this.btnDangNhap.Text = "ĐĂNG NHẬP";
            this.btnDangNhap.UseVisualStyleBackColor = false;
            this.btnDangNhap.Click += new System.EventHandler(this.BtnDangNhap_Click);
            //
            // chkGhiNhoTaiKhoan
            //
            this.chkGhiNhoTaiKhoan.AutoSize = true;
            this.chkGhiNhoTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkGhiNhoTaiKhoan.ForeColor = System.Drawing.Color.FromArgb(82, 101, 118);
            this.chkGhiNhoTaiKhoan.Location = new System.Drawing.Point(52, 462);
            this.chkGhiNhoTaiKhoan.Name = "chkGhiNhoTaiKhoan";
            this.chkGhiNhoTaiKhoan.Size = new System.Drawing.Size(214, 21);
            this.chkGhiNhoTaiKhoan.TabIndex = 3;
            this.chkGhiNhoTaiKhoan.Text = "Ghi nhớ tài khoản trên máy này";
            this.chkGhiNhoTaiKhoan.UseVisualStyleBackColor = true;
            //
            // cboVaiTro
            //
            this.cboVaiTro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVaiTro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVaiTro.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboVaiTro.FormattingEnabled = true;
            this.cboVaiTro.Items.AddRange(new object[] {
            "Quản lý kho",
            "Thủ kho",
            "Kế toán kho"});
            this.cboVaiTro.Location = new System.Drawing.Point(50, 400);
            this.cboVaiTro.Name = "cboVaiTro";
            this.cboVaiTro.Size = new System.Drawing.Size(474, 25);
            this.cboVaiTro.TabIndex = 2;
            this.cboVaiTro.AccessibleName = "Vai trò";
            this.cboVaiTro.SelectedIndex = 0;
            //
            // lblVaiTro
            //
            this.lblVaiTro.AutoSize = true;
            this.lblVaiTro.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblVaiTro.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblVaiTro.Location = new System.Drawing.Point(50, 374);
            this.lblVaiTro.Name = "lblVaiTro";
            this.lblVaiTro.Size = new System.Drawing.Size(48, 17);
            this.lblVaiTro.TabIndex = 8;
            this.lblVaiTro.Text = "Vai trò";
            //
            // txtMatKhau
            //
            this.txtMatKhau.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMatKhau.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMatKhau.Location = new System.Drawing.Point(50, 310);
            this.txtMatKhau.MaxLength = 255;
            this.txtMatKhau.Name = "txtMatKhau";
            this.txtMatKhau.Size = new System.Drawing.Size(474, 25);
            this.txtMatKhau.TabIndex = 1;
            this.txtMatKhau.AccessibleName = "Mật khẩu";
            this.txtMatKhau.UseSystemPasswordChar = true;
            //
            // lblMatKhau
            //
            this.lblMatKhau.AutoSize = true;
            this.lblMatKhau.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblMatKhau.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblMatKhau.Location = new System.Drawing.Point(50, 284);
            this.lblMatKhau.Name = "lblMatKhau";
            this.lblMatKhau.Size = new System.Drawing.Size(63, 17);
            this.lblMatKhau.TabIndex = 6;
            this.lblMatKhau.Text = "Mật khẩu";
            //
            // txtTaiKhoan
            //
            this.txtTaiKhoan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTaiKhoan.Location = new System.Drawing.Point(50, 220);
            this.txtTaiKhoan.MaxLength = 50;
            this.txtTaiKhoan.Name = "txtTaiKhoan";
            this.txtTaiKhoan.Size = new System.Drawing.Size(474, 25);
            this.txtTaiKhoan.TabIndex = 0;
            this.txtTaiKhoan.AccessibleName = "Tài khoản";
            //
            // lblTaiKhoan
            //
            this.lblTaiKhoan.AutoSize = true;
            this.lblTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTaiKhoan.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblTaiKhoan.Location = new System.Drawing.Point(50, 194);
            this.lblTaiKhoan.Name = "lblTaiKhoan";
            this.lblTaiKhoan.Size = new System.Drawing.Size(66, 17);
            this.lblTaiKhoan.TabIndex = 4;
            this.lblTaiKhoan.Text = "Tài khoản";
            //
            // lblMoTa
            //
            this.lblMoTa.AutoSize = true;
            this.lblMoTa.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(113, 128, 144);
            this.lblMoTa.Location = new System.Drawing.Point(52, 137);
            this.lblMoTa.Name = "lblMoTa";
            this.lblMoTa.Size = new System.Drawing.Size(237, 19);
            this.lblMoTa.TabIndex = 2;
            this.lblMoTa.Text = "Đăng nhập để truy cập hệ thống kho";
            //
            // lblTieuDe
            //
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTieuDe.ForeColor = System.Drawing.Color.FromArgb(21, 43, 59);
            this.lblTieuDe.Location = new System.Drawing.Point(47, 88);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(165, 45);
            this.lblTieuDe.TabIndex = 1;
            this.lblTieuDe.Text = "Đăng nhập";
            //
            // lblEyebrow
            //
            this.lblEyebrow.AutoSize = true;
            this.lblEyebrow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEyebrow.ForeColor = System.Drawing.Color.FromArgb(7, 139, 124);
            this.lblEyebrow.Location = new System.Drawing.Point(50, 57);
            this.lblEyebrow.Name = "lblEyebrow";
            this.lblEyebrow.Size = new System.Drawing.Size(153, 15);
            this.lblEyebrow.TabIndex = 0;
            this.lblEyebrow.Text = "CỔNG THÔNG TIN NỘI BỘ";
            //
            // frmDangNhap
            //
            this.AcceptButton = this.btnDangNhap;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(238, 244, 247);
            this.CancelButton = this.btnThoat;
            this.ClientSize = new System.Drawing.Size(920, 660);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHero);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(850, 620);
            this.Name = "frmDangNhap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập - QLKhoHang";
            this.panelHero.ResumeLayout(false);
            this.panelLogo.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelHero;
        private System.Windows.Forms.Label lblHeroDescription;
        private System.Windows.Forms.Label lblHeroTitle;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Label lblLogoGlyph;
        private System.Windows.Forms.Label lblHeroCaption;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label lblFooter;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnDangNhap;
        private System.Windows.Forms.CheckBox chkGhiNhoTaiKhoan;
        private System.Windows.Forms.ComboBox cboVaiTro;
        private System.Windows.Forms.Label lblVaiTro;
        private System.Windows.Forms.TextBox txtMatKhau;
        private System.Windows.Forms.Label lblMatKhau;
        private System.Windows.Forms.TextBox txtTaiKhoan;
        private System.Windows.Forms.Label lblTaiKhoan;
        private System.Windows.Forms.Label lblMoTa;
        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Label lblEyebrow;
    }
}
