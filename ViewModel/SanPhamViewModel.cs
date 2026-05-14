using Cafe.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;

namespace Cafe.ViewModel
{
    public class SanPhamViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();
        private ObservableCollection<MENU> _listSanPham;
        public ObservableCollection<MENU> ListSanPham
        {
            get => _listSanPham;
            set { _listSanPham = value; OnPropertyChanged(); }
        }

        private ObservableCollection<LOAI_MON> _listLoai;
        public ObservableCollection<LOAI_MON> ListLoai
        {
            get => _listLoai;
            set { _listLoai = value; OnPropertyChanged(); }
        }
        private string _maMon;
        public string MaMon { get => _maMon; set { _maMon = value; OnPropertyChanged(); } }

        private string _tenMon;
        public string TenMon { get => _tenMon; set { _tenMon = value; OnPropertyChanged(); } }

        private int _donGia;
        public int DonGia { get => _donGia; set { _donGia = value; OnPropertyChanged(); } }

        private LOAI_MON _selectedLoai;
        public LOAI_MON SelectedLoai { get => _selectedLoai; set { _selectedLoai = value; OnPropertyChanged(); } }

        private string _hinhAnh;
        public string HinhAnh { get => _hinhAnh; set { _hinhAnh = value; OnPropertyChanged(); } }

        private MENU _selectedItem;
        public MENU SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    MaMon = SelectedItem.MaMon.Trim();
                    TenMon = SelectedItem.TenMon.Trim();
                    DonGia = SelectedItem.DonGia ?? 0;
                    SelectedLoai = ListLoai.FirstOrDefault(x => x.MaLoai == SelectedItem.MaLoai);
                    if (!string.IsNullOrEmpty(SelectedItem.HinhAnh))
                    {
                        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                        HinhAnh = Path.Combine(folderPath, Path.GetFileName(SelectedItem.HinhAnh));
                    }
                    else
                    {
                        HinhAnh = null;
                    }
                }
            }
        }
        private ObservableCollection<string> _listGoiY;
        public ObservableCollection<string> ListGoiY
        {
            get => _listGoiY;
            set { _listGoiY = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ThucHienTimKiem(); 
            }
        }

        public ICommand ChooseImageCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; } 
        public ICommand DeleteCommand { get; set; } 

        public SanPhamViewModel()
        {
            LoadData();
            LoadGoiY();
            ChooseImageCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "Chọn hình ảnh sản phẩm";
                openDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (openDialog.ShowDialog() == true)
                {
                    HinhAnh = openDialog.FileName;
                }
            });

            SearchCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                SearchText = string.Empty;
                ClearForm();
            });
            AddCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin",
                (p) =>
                {
                    if (string.IsNullOrWhiteSpace(MaMon) || string.IsNullOrWhiteSpace(TenMon) || SelectedLoai == null)
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Mã, Tên và chọn Loại sản phẩm!");
                        return;
                    }

                    string checkMa = MaMon.Trim();
                    if (_db.MENUs.Any(x => x.MaMon.Trim() == checkMa))
                    {
                        MessageBox.Show("Mã sản phẩm này đã tồn tại!");
                        return;
                    }

                    try
                    {
                        string duongDanLuuVaoDB = null;
                        if (!string.IsNullOrEmpty(HinhAnh) && File.Exists(HinhAnh))
                        {
                            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                            string extension = Path.GetExtension(HinhAnh);
                            string newFileName = checkMa + "_" + DateTime.Now.Ticks + extension;
                            string destinationPath = Path.Combine(folderPath, newFileName);

                            File.Copy(HinhAnh, destinationPath, true);
                            duongDanLuuVaoDB = "/Images/" + newFileName;
                        }

                        var spMoi = new MENU
                        {
                            MaMon = checkMa,
                            TenMon = TenMon.Trim(),
                            DonGia = DonGia,
                            MaLoai = SelectedLoai.MaLoai,
                            HinhAnh = duongDanLuuVaoDB
                        };

                        _db.MENUs.Add(spMoi);
                        _db.SaveChanges();

                        MessageBox.Show("Thêm sản phẩm thành công!");
                        LoadData();
                        LoadGoiY();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu: " + (ex.InnerException?.Message ?? ex.Message));
                    }
                });
            EditCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin" && SelectedItem != null, // Điều kiện: Admin và đang chọn SP[cite: 2]
                (p) =>
                {
                    if (string.IsNullOrWhiteSpace(TenMon) || SelectedLoai == null)
                    {
                        MessageBox.Show("Tên món và Loại món không được để trống!");
                        return;
                    }

                    try
                    {
                        var spSua = _db.MENUs.SingleOrDefault(x => x.MaMon == SelectedItem.MaMon);
                        if (spSua != null)
                        {
                            spSua.TenMon = TenMon.Trim();
                            spSua.DonGia = DonGia;
                            spSua.MaLoai = SelectedLoai.MaLoai;
                            if (!string.IsNullOrEmpty(HinhAnh) && !HinhAnh.Contains("Images\\") && !HinhAnh.Contains("Images/"))
                            {
                                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                                string extension = Path.GetExtension(HinhAnh);
                                string newFileName = spSua.MaMon.Trim() + "_edit_" + DateTime.Now.Ticks + extension;
                                string destinationPath = Path.Combine(folderPath, newFileName);

                                File.Copy(HinhAnh, destinationPath, true);
                                spSua.HinhAnh = "/Images/" + newFileName;
                            }

                            _db.SaveChanges();
                            MessageBox.Show("Cập nhật sản phẩm thành công!");
                            LoadData();
                            LoadGoiY();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi cập nhật: " + (ex.InnerException?.Message ?? ex.Message));
                    }
                });

            DeleteCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin" && SelectedItem != null, // Điều kiện: Admin và đang chọn SP[cite: 2]
                (p) =>
                {
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var spXoa = _db.MENUs.SingleOrDefault(x => x.MaMon == SelectedItem.MaMon);
                            if (spXoa != null)
                            {
                                _db.MENUs.Remove(spXoa);
                                _db.SaveChanges();
                                MessageBox.Show("Xóa sản phẩm thành công!");
                                LoadData();
                                LoadGoiY();
                                ClearForm();
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Không thể xóa sản phẩm này vì nó đã tồn tại trong các Hóa đơn cũ!", "Lỗi ràng buộc", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });
        }

        void LoadData()
        {
            try
            {
                ListSanPham = new ObservableCollection<MENU>(_db.MENUs.ToList());
                ListLoai = new ObservableCollection<LOAI_MON>(_db.LOAI_MON.ToList());
            }
            catch (Exception ex) { MessageBox.Show("Lỗi CSDL: " + ex.Message); }
        }

        void LoadGoiY()
        {
            try
            {
                ListGoiY = new ObservableCollection<string>(_db.MENUs.Select(x => x.TenMon).ToList());
            }
            catch { }
        }

        private void ThucHienTimKiem()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadData();
                return;
            }

            try
            {
                string tuKhoa = SearchText.Trim().ToLower();
                var ketQua = _db.MENUs.Where(x =>
                                x.TenMon.ToLower().Contains(tuKhoa) ||
                                x.MaMon.ToLower().Contains(tuKhoa)).ToList();

                ListSanPham = new ObservableCollection<MENU>(ketQua);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        void ClearForm()
        {
            MaMon = string.Empty;
            TenMon = string.Empty;
            DonGia = 0;
            SelectedLoai = null;
            HinhAnh = null;
            SelectedItem = null;
        }
    }
}