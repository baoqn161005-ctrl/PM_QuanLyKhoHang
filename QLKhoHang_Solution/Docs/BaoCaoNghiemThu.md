# Báo cáo nghiệm thu hiện trạng TV1

Ngày ghi nhận: 2026-10-09

## Kết quả

| Hạng mục | Kết quả | Ghi chú |
|---|---|---|
| Build solution Debug | Đạt | Build .NET Framework 4.8 bằng MSBuild thành công. |
| Kiểm tra whitespace diff | Đạt | `git diff --check` không báo lỗi. |
| Kết nối SQL Server từ môi trường Codex | Chưa xác nhận | Kết nối tới `BAOCRY\SQLEXPRESS02` trả lỗi không tìm thấy server/instance. Điều này chỉ phản ánh môi trường chạy Codex; chưa kết luận cấu hình trên máy người dùng sai. |
| Đăng nhập và phân quyền | Chưa chạy | Không truy cập được SQL Server từ môi trường này; chưa có tài khoản mẫu/seed để xác thực thực tế. |
| Tạo backup và kiểm tra backup | Chưa chạy | Chưa xác nhận được kết nối tới database test. SQL Server service cần quyền ghi vào thư mục backup. |
| Restore backup | Chưa chạy | Chưa thực hiện trên SQL Server. Chức năng được thiết kế tạo database test tên mới và không ghi đè database nguồn. |
| Nhập hàng, xuất hàng, chặn xuất vượt tồn | Chưa chạy | Các module nghiệp vụ của TV2/TV3 chưa có trong nhánh hiện tại. |
| Kiểm kê/cân bằng tồn, thẻ kho, báo cáo Excel | Chưa chạy | Các module của TV2/TV3/TV4 chưa có trong nhánh hiện tại. |
| Kiểm tra transaction/rollback | Chưa chạy | Chưa có luồng giao dịch nghiệp vụ để nghiệm thu. |
| Kiểm tra giao diện trực tiếp | Chưa chạy | Mới xác nhận biên dịch; chưa mở ứng dụng và thao tác trên Windows. |

## Ghi chú phạm vi

- Bước 21 (tích hợp nhánh TV2/TV3/TV4) được bỏ qua theo yêu cầu hiện tại.
- Schema hiện tại có 7 bảng nhưng chưa có bảng lưu phiếu kiểm kê/điều chỉnh tồn. Cần thống nhất bổ sung schema khi triển khai nghiệp vụ kiểm kê.
- Trước khi thử backup/restore, cần trỏ cấu hình tới database test. Restore luôn tạo database test mới; không dùng backup/restore trên database vận hành để nghiệm thu.
- Các mục “Chưa chạy” không được xem là đạt cho tới khi chạy và lưu bằng chứng thực tế.
