using Cafe.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Cafe.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private DispatcherTimer _timer;

        private string _doanhThu;
        public string DoanhThu { get => _doanhThu; set { _doanhThu = value; OnPropertyChanged(); } }

        private int _soHoaDon;
        public int SoHoaDon { get => _soHoaDon; set { _soHoaDon = value; OnPropertyChanged(); } }

        private int _soKhachHang;
        public int SoKhachHang { get => _soKhachHang; set { _soKhachHang = value; OnPropertyChanged(); } }

        public ICommand RefreshCommand { get; set; }

        public MainViewModel()
        {
            // Cấu hình Timer để tự động cập nhật mỗi 30 giây
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += (s, e) => UpdateThongKe();
            _timer.Start();

            // Khởi tạo dữ liệu lần đầu từ CSDL[cite: 3, 10]
            UpdateThongKe();

            // Xử lý nút Reset - Đưa mọi thứ về 0[cite: 4]
            RefreshCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                _timer.Stop();
                DoanhThu = "0đ";
                SoHoaDon = 0;
                SoKhachHang = 0;
                _timer.Start();
            });
        }

        public void UpdateThongKe()
        {
            try
            {
                using (var db = new QL_CuaHangNuocEntities())
                {
                    DateTime today = DateTime.Today;
                    var tongTien = db.HOADON_BAN
                        .Where(hd => DbFunctions.TruncateTime(hd.NgayTao) == today)
                        .Select(hd => (double?)hd.TongTien)
                        .DefaultIfEmpty(0)
                        .Sum();

                    DoanhThu = string.Format("{0:N0}đ", tongTien);
                    SoHoaDon = db.HOADON_BAN.Count(hd => DbFunctions.TruncateTime(hd.NgayTao) == today);
                    SoKhachHang = db.KHACHHANGs.Count();
                }
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(DoanhThu)) DoanhThu = "0đ";
            }
        }
    }
}