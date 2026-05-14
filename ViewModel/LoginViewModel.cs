using Cafe.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set { _tenDangNhap = value; OnPropertyChanged(); }
        }

        private string _matKhau;
        public string MatKhau
        {
            get => _matKhau;
            set { _matKhau = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
        public static class AppSession
        {
            public static string TenHienThi { get; set; }
            public static string VaiTro { get; set; }
        }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                    return;
                }

                try
                {
                    var user = _db.NguoiDungs.FirstOrDefault(x =>
                                x.TenDangNhap.Trim() == TenDangNhap.Trim() &&
                                x.MatKhau.Trim() == MatKhau.Trim());

                    if (user != null)
                    {
                        AppSession.TenHienThi = user.TenDangNhap;
                        AppSession.VaiTro = user.VaiTro;
                        MessageBox.Show($"Đăng nhập thành công! Chào {user.TenDangNhap}.");
                        var mainScreen = new Cafe.View.cafe();
                        mainScreen.Show();
                        if (p is Window loginWindow)
                        {
                            loginWindow.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
                }
            });
        }
    }
}