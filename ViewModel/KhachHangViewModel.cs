using Cafe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class KhachHangViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db;

        private ObservableCollection<KHACHHANG> _listKhachHang;
        public ObservableCollection<KHACHHANG> ListKhachHang { get => _listKhachHang; set { _listKhachHang = value; OnPropertyChanged(); } }

        public List<string> ListLoaiThanhVien { get; set; } = new List<string> { "Thường", "Thân thiết", "VIP" };

        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        private string _maKH;
        public string MaKH { get => _maKH; set { _maKH = value; OnPropertyChanged(); } }

        private string _hoTen;
        public string HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); } }

        private DateTime? _ngaySinh;
        public DateTime? NgaySinh { get => _ngaySinh; set { _ngaySinh = value; OnPropertyChanged(); } }

        private string _dienThoai;
        public string DienThoai { get => _dienThoai; set { _dienThoai = value; OnPropertyChanged(); } }

        private string _loaiThanhVienDuocChon;
        public string LoaiThanhVienDuocChon { get => _loaiThanhVienDuocChon; set { _loaiThanhVienDuocChon = value; OnPropertyChanged(); } }

        private KHACHHANG _selectedItem;
        public KHACHHANG SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (_selectedItem != null)
                {
                    MaKH = _selectedItem.MaKH;
                    HoTen = _selectedItem.HoTen;
                    NgaySinh = _selectedItem.NgaySinh;
                    DienThoai = _selectedItem.DienThoai;
                    LoaiThanhVienDuocChon = _selectedItem.LoaiThanhVien;
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public KhachHangViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand<object>((p) =>
            {
                return !string.IsNullOrEmpty(MaKH) && !string.IsNullOrEmpty(HoTen);
            }, (p) =>
            {
                var kh = _db.KHACHHANGs.FirstOrDefault(x => x.MaKH == MaKH);
                if (kh == null)
                {
                    var newKH = new KHACHHANG
                    {
                        MaKH = MaKH,
                        HoTen = HoTen,
                        NgaySinh = NgaySinh,
                        DienThoai = DienThoai,
                        LoaiThanhVien = LoaiThanhVienDuocChon ?? "Thường"
                    };
                    _db.KHACHHANGs.Add(newKH);
                    _db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo");
                }
                else
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại!", "Cảnh báo");
                }
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null;
            }, (p) =>
            {
                var kh = _db.KHACHHANGs.FirstOrDefault(x => x.MaKH == SelectedItem.MaKH);
                if (kh != null)
                {
                    kh.HoTen = HoTen;
                    kh.NgaySinh = NgaySinh;
                    kh.DienThoai = DienThoai;
                    kh.LoaiThanhVien = LoaiThanhVienDuocChon;
                    _db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");
                }
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null;
            }, (p) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var kh = _db.KHACHHANGs.FirstOrDefault(x => x.MaKH == SelectedItem.MaKH);
                    if (kh != null)
                    {
                        _db.KHACHHANGs.Remove(kh);
                        _db.SaveChanges();
                        LoadData();
                        ClearFields();
                    }
                }
            });

            SearchCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    LoadData();
                }
                else
                {
                    var list = _db.KHACHHANGs.Where(x => x.HoTen.Contains(SearchText) || x.DienThoai.Contains(SearchText)).ToList();
                    ListKhachHang = new ObservableCollection<KHACHHANG>(list);
                }
            });
        }

        private void LoadData()
        {
            _db = new QL_CuaHangNuocEntities();
            ListKhachHang = new ObservableCollection<KHACHHANG>(_db.KHACHHANGs.ToList());
        }

        private void ClearFields()
        {
            MaKH = string.Empty;
            HoTen = string.Empty;
            NgaySinh = null;
            DienThoai = string.Empty;
            LoaiThanhVienDuocChon = null;
            SearchText = string.Empty;
        }
    }
}