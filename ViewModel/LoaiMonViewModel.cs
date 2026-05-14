using Cafe.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class LoaiMonViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();

        private ObservableCollection<LOAI_MON> _danhSachLoai;
        public ObservableCollection<LOAI_MON> DanhSachLoai
        {
            get => _danhSachLoai;
            set { _danhSachLoai = value; OnPropertyChanged(); }
        }

        private string _maLoai;
        public string MaLoai { get => _maLoai; set { _maLoai = value; OnPropertyChanged(); } }

        private string _tenLoai;
        public string TenLoai { get => _tenLoai; set { _tenLoai = value; OnPropertyChanged(); } }

        // Thêm Property để quản lý từ khóa tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }
        private LOAI_MON _selectedItem;
        public LOAI_MON SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    MaLoai = SelectedItem.MaLoai.Trim();
                    TenLoai = SelectedItem.TenLoai.Trim();
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SearchCommand { get; set; } 

        public LoaiMonViewModel()
        {
            LoadData();

            // Command Tìm kiếm (Xử lý rỗng và tìm theo Tên)
            SearchCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadData();
                    return;
                }

                try
                {
                    string tuKhoa = SearchText.Trim().ToLower();
                    var ketQua = _db.LOAI_MON
                                    .Where(x => x.TenLoai.ToLower().Contains(tuKhoa))
                                    .ToList();
                    DanhSachLoai = new ObservableCollection<LOAI_MON>(ketQua);

                    if (DanhSachLoai.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy loại món nào khớp với từ khóa!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            });

            // Command Thêm mới
            AddCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                if (string.IsNullOrWhiteSpace(MaLoai) || string.IsNullOrWhiteSpace(TenLoai))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                string checkMa = MaLoai.Trim();
                if (_db.LOAI_MON.Any(x => x.MaLoai.Trim() == checkMa))
                {
                    MessageBox.Show("Mã loại này đã tồn tại!");
                    return;
                }

                try
                {
                    var moi = new LOAI_MON { MaLoai = checkMa, TenLoai = TenLoai.Trim() };
                    _db.LOAI_MON.Add(moi);
                    _db.SaveChanges();

                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + (ex.InnerException?.Message ?? ex.Message));
                }
            });

            // Command Sửa dữ liệu
            EditCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) =>
            {
                try
                {
                    var editObj = _db.LOAI_MON.SingleOrDefault(x => x.MaLoai == SelectedItem.MaLoai);
                    if (editObj != null)
                    {
                        editObj.TenLoai = TenLoai.Trim();
                        _db.SaveChanges();
                        MessageBox.Show("Cập nhật thành công!");
                        LoadData();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            });
        }

        void LoadData()
        {
            try
            {
                DanhSachLoai = new ObservableCollection<LOAI_MON>(_db.LOAI_MON.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối CSDL: " + ex.Message);
            }
        }

        void ClearForm()
        {
            MaLoai = string.Empty;
            TenLoai = string.Empty;
            SearchText = string.Empty;
            SelectedItem = null;
        }
    }
}