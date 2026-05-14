using Cafe.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class NhapHangViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();

        private ObservableCollection<MENU> _listSanPham;
        public ObservableCollection<MENU> ListSanPham { get => _listSanPham; set { _listSanPham = value; OnPropertyChanged(); } }

        private ObservableCollection<NHACUNGCAP> _listNhaCungCap;
        public ObservableCollection<NHACUNGCAP> ListNhaCungCap { get => _listNhaCungCap; set { _listNhaCungCap = value; OnPropertyChanged(); } }

        private ObservableCollection<GioHang> _listGioHang;
        public ObservableCollection<GioHang> ListGioHang { get => _listGioHang; set { _listGioHang = value; OnPropertyChanged(); } }

        private NHACUNGCAP _selectedNCC;
        public NHACUNGCAP SelectedNCC { get => _selectedNCC; set { _selectedNCC = value; OnPropertyChanged(); } }

        private int _tongTienHD;
        public int TongTienHD { get => _tongTienHD; set { _tongTienHD = value; OnPropertyChanged(); } }

        public ICommand ChonMonCommand { get; set; }
        public ICommand XoaMonCommand { get; set; }
        public ICommand ThanhToanCommand { get; set; }

        public NhapHangViewModel()
        {
            ListSanPham = new ObservableCollection<MENU>(_db.MENUs.ToList());
            ListNhaCungCap = new ObservableCollection<NHACUNGCAP>(_db.NHACUNGCAPs.ToList());
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
                    int giaNhapUocTinh = (int)((p.DonGia ?? 0) * 0.7);
                    var newItem = new GioHang { SanPham = p, DonGiaItem = giaNhapUocTinh, SoLuong = 1 };
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
                (p) => ListGioHang.Count > 0 && SelectedNCC != null && LoginViewModel.AppSession.VaiTro == "Admin",
                (p) =>
                {
                    try
                    {
                        var hdNhap = new HOADON_NHAP
                        {
                            NgayNhap = DateTime.Now,
                            MaNCC = SelectedNCC.MaNCC,
                            TenDangNhap = LoginViewModel.AppSession.TenHienThi ?? "admin",
                            TongTien = TongTienHD
                        };
                        _db.HOADON_NHAP.Add(hdNhap);
                        _db.SaveChanges();

                        foreach (var item in ListGioHang)
                        {
                            var chiTiet = new CHITIET_HDN
                            {
                                MaHDN = hdNhap.MaHDN,
                                MaMon = item.SanPham.MaMon,
                                SoLuong = item.SoLuong,
                                DonGiaNhap = item.DonGiaItem,
                                ThanhTien = item.ThanhTien
                            };
                            _db.CHITIET_HDN.Add(chiTiet);
                        }
                        _db.SaveChanges();

                        TuDongCongKho();

                        MessageBox.Show($"Tạo phiếu nhập thành công!\nMã Phiếu: {hdNhap.MaHDN}\nTổng tiền: {TongTienHD:N0} VNĐ", "Thông báo");
                        ListGioHang.Clear();
                        SelectedNCC = null;
                        TinhTongTien();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi lưu hóa đơn: " + ex.Message); }
                });
        }

        private void TinhTongTien()
        {
            TongTienHD = ListGioHang.Sum(x => x.ThanhTien);
        }

        private void TuDongCongKho()
        {
            foreach (var item in ListGioHang)
            {
                var congThucs = _db.CONGTHUCs.Where(ct => ct.MaMon == item.SanPham.MaMon).ToList();
                foreach (var ct in congThucs)
                {
                    var nguyenLieu = _db.NGUYENLIEUx.FirstOrDefault(nl => nl.MaNL == ct.MaNL);
                    if (nguyenLieu != null)
                    {
                        decimal tongNhap = (decimal)(ct.SoLuongTieuHao * item.SoLuong);
                        nguyenLieu.TonKho += tongNhap;
                    }
                }
            }
            _db.SaveChanges();
        }
    }
}