using Cafe.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Cafe.View
{
    public partial class cafe : Window
    {
        public cafe()
        {
            InitializeComponent();
            ChuyenTrang(new TrangChu());
        }

        private void ChuyenTrang(UserControl uc)
        {
            MainContent.Content = uc;
        }

        private void btnTrangChu_Click_1(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new TrangChu());
        }

        private void btnBanHang_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new BanHangUC());
        }

        private void BtnNhapHang_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new NhapHangUC());
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new SanPhamUC());
        }

        private void btnKhachHang_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new KhachHangUC());
        }

        private void BtnNhanVien_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new NhanVienUC());
        }

        private void btnThongKe_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new ThongKeKhoUC());
        }

        private void btnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            ChuyenTrang(new BaoCaoUC());
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var kq = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (kq == MessageBoxResult.Yes)
            {
                LoginView login = new LoginView();
                login.Show();
                this.Close();
            }
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            var kq = MessageBox.Show("Bạn có muốn thoát toàn bộ hệ thống không?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (kq == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}