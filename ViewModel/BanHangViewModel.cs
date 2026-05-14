using Cafe.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class BanHangViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();

        private ObservableCollection<MENU> _listSanPham;
        public ObservableCollection<MENU> ListSanPham { get => _listSanPham; set { _listSanPham = value; OnPropertyChanged(); } }

        private ObservableCollection<KHACHHANG> _listKhachHang;
        public ObservableCollection<KHACHHANG> ListKhachHang { get => _listKhachHang; set { _listKhachHang = value; OnPropertyChanged(); } }

        private ObservableCollection<GioHang> _listGioHang;
        public ObservableCollection<GioHang> ListGioHang { get => _listGioHang; set { _listGioHang = value; OnPropertyChanged(); } }

        private KHACHHANG _selectedKhachHang;
        public KHACHHANG SelectedKhachHang { get => _selectedKhachHang; set { _selectedKhachHang = value; OnPropertyChanged(); } }

        private int _tongTienHD;
        public int TongTienHD { get => _tongTienHD; set { _tongTienHD = value; OnPropertyChanged(); } }

        public ICommand ChonMonCommand { get; set; }
        public ICommand XoaMonCommand { get; set; }
        public ICommand ThanhToanCommand { get; set; }

        // KHỞI TẠO VIEWMODEL VÀ CÁC COMMAND THAO TÁC
        public BanHangViewModel()
        {
            ListSanPham = new ObservableCollection<MENU>(_db.MENUs.ToList());
            ListKhachHang = new ObservableCollection<KHACHHANG>(_db.KHACHHANGs.ToList());
            ListGioHang = new ObservableCollection<GioHang>();

            ChonMonCommand = new RelayCommand<MENU>((p) => p != null, (p) =>
            {
                var itemDaCo = ListGioHang.FirstOrDefault(x => x.SanPham.MaMon == p.MaMon);
                if (itemDaCo != null)
                {
                    itemDaCo.SoLuong++;
                }
                else
                {
                    var newItem = new GioHang { SanPham = p, DonGiaItem = p.DonGia ?? 0, SoLuong = 1 };
                    newItem.PropertyChanged += (sender, e) => TinhTongTien();
                    ListGioHang.Add(newItem);
                }
                TinhTongTien();
            });

            XoaMonCommand = new RelayCommand<GioHang>((p) => p != null, (p) =>
            {
                ListGioHang.Remove(p);
                TinhTongTien();
            });

            ThanhToanCommand = new RelayCommand<object>(
                (p) => ListGioHang.Count > 0 && SelectedKhachHang != null,
                (p) =>
                {
                    try
                    {
                        var hdBan = new HOADON_BAN
                        {
                            NgayTao = DateTime.Now,
                            MaKH = SelectedKhachHang.MaKH,
                            TenDangNhap = LoginViewModel.AppSession.TenHienThi ?? "admin",
                            TongTien = TongTienHD
                        };
                        _db.HOADON_BAN.Add(hdBan);
                        _db.SaveChanges();

                        foreach (var item in ListGioHang)
                        {
                            var chiTiet = new CHITIET_HDB
                            {
                                MaHDB = hdBan.MaHDB,
                                MaMon = item.SanPham.MaMon,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGiaItem,
                                ThanhTien = item.ThanhTien
                            };
                            _db.CHITIET_HDB.Add(chiTiet);
                        }
                        _db.SaveChanges();

                        TuDongTruKho();

                        MessageBox.Show($"Thanh toán thành công!\nMã Hóa Đơn: {hdBan.MaHDB}\nTổng tiền: {TongTienHD:N0} VNĐ", "Thông báo");
                        ListGioHang.Clear();
                        SelectedKhachHang = null;
                        TinhTongTien();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi thanh toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
        }

        // HÀM TÍNH TỔNG TIỀN HÓA ĐƠN
        private void TinhTongTien()
        {
            TongTienHD = ListGioHang.Sum(x => x.ThanhTien);
        }

        // HÀM TỰ ĐỘNG TRỪ KHO NGUYÊN LIỆU DỰA TRÊN ĐỊNH MỨC
        private void TuDongTruKho()
        {
            try
            {
                foreach (var item in ListGioHang)
                {
                    var congThucs = _db.CONGTHUCs.Where(ct => ct.MaMon == item.SanPham.MaMon).ToList();

                    foreach (var ct in congThucs)
                    {
                        var nguyenLieu = _db.NGUYENLIEUx.FirstOrDefault(nl => nl.MaNL == ct.MaNL);

                        if (nguyenLieu != null)
                        {
                            decimal tongTieuHao = (decimal)(ct.SoLuongTieuHao * item.SoLuong);
                            nguyenLieu.TonKho -= tongTieuHao;
                        }
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật kho: " + ex.Message);
            }
        }
    }
}
