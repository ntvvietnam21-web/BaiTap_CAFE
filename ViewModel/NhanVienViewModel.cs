using Cafe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class NhanVienViewModel : BaseViewModel
    {
        private QL_CuaHangNuocEntities _db = new QL_CuaHangNuocEntities();

        private ObservableCollection<NguoiDung> _listNhanVien;
        public ObservableCollection<NguoiDung> ListNhanVien
        {
            get => _listNhanVien;
            set { _listNhanVien = value; OnPropertyChanged(); }
        }
        private string _tenDangNhap;
        public string TenDangNhap { get => _tenDangNhap; set { _tenDangNhap = value; OnPropertyChanged(); } }
        private string _matKhau;
        public string MatKhau { get => _matKhau; set { _matKhau = value; OnPropertyChanged(); } }
        private string _vaiTroDuocChon;
        public string VaiTroDuocChon { get => _vaiTroDuocChon; set { _vaiTroDuocChon = value; OnPropertyChanged(); } }
        public List<string> ListVaiTro { get; set; } = new List<string> { "Admin", "Nhân viên" };
        private NguoiDung _selectedItem;
        public NguoiDung SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenDangNhap = SelectedItem.TenDangNhap.Trim();
                    MatKhau = SelectedItem.MatKhau.Trim();
                    VaiTroDuocChon = SelectedItem.VaiTro;
                }
            }
        }

        // Quản lý tìm kiếm
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
        public ICommand SearchCommand { get; set; } 
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public NhanVienViewModel()
        {
            LoadData();

            // Nút Làm Mới
            SearchCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                SearchText = string.Empty;
                ClearForm();
            });

            // ==========================================
            // THÊM MỚI (CHỈ ADMIN MỚI ĐƯỢC THÊM)
            // ==========================================
            AddCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin", 
                (p) =>
                {
                    // Validation
                    if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(VaiTroDuocChon))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập, Mật khẩu và Vai trò!");
                        return;
                    }

                    string checkTen = TenDangNhap.Trim();
                    if (_db.NguoiDungs.Any(x => x.TenDangNhap.Trim() == checkTen))
                    {
                        MessageBox.Show("Tên đăng nhập này đã tồn tại trong hệ thống!");
                        return;
                    }

                    try
                    {
                        var nvMoi = new NguoiDung
                        {
                            TenDangNhap = checkTen,
                            MatKhau = MatKhau.Trim(),
                            VaiTro = VaiTroDuocChon
                        };

                        _db.NguoiDungs.Add(nvMoi);
                        _db.SaveChanges();

                        MessageBox.Show("Thêm tài khoản nhân viên thành công!");
                        LoadData();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi lưu: " + (ex.InnerException?.Message ?? ex.Message));
                    }
                });

            // ==========================================
            // SỬA (CHỈ ADMIN VÀ PHẢI ĐANG CHỌN 1 DÒNG)
            // ==========================================
            EditCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin" && SelectedItem != null,
                (p) =>
                {
                    if (string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(VaiTroDuocChon))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ Mật khẩu và Vai trò!");
                        return;
                    }

                    try
                    {
                        var nvSua = _db.NguoiDungs.SingleOrDefault(x => x.TenDangNhap == SelectedItem.TenDangNhap);
                        if (nvSua != null)
                        {
                            nvSua.MatKhau = MatKhau.Trim();
                            nvSua.VaiTro = VaiTroDuocChon;

                            _db.SaveChanges();
                            MessageBox.Show("Cập nhật tài khoản thành công!");
                            LoadData();
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                });

            // ==========================================
            // XÓA (CHỈ ADMIN VÀ PHẢI ĐANG CHỌN 1 DÒNG)
            // ==========================================
            DeleteCommand = new RelayCommand<object>(
                (p) => LoginViewModel.AppSession.VaiTro == "Admin" && SelectedItem != null,
                (p) =>
                {
                    if (SelectedItem.TenDangNhap.Trim() == LoginViewModel.AppSession.TenHienThi.Trim())
                    {
                        MessageBox.Show("Bạn không thể tự xóa tài khoản của chính mình đang đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var nvXoa = _db.NguoiDungs.SingleOrDefault(x => x.TenDangNhap == SelectedItem.TenDangNhap);
                            if (nvXoa != null)
                            {
                                _db.NguoiDungs.Remove(nvXoa);
                                _db.SaveChanges();
                                MessageBox.Show("Xóa tài khoản thành công!");
                                LoadData();
                                ClearForm();
                            }
                        }
                        catch (Exception ex) { MessageBox.Show("Lỗi khi xóa: " + ex.Message); }
                    }
                });
        }

        void LoadData()
        {
            try
            {
                ListNhanVien = new ObservableCollection<NguoiDung>(_db.NguoiDungs.ToList());
            }
            catch (Exception ex) { MessageBox.Show("Lỗi CSDL: " + ex.Message); }
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
                var ketQua = _db.NguoiDungs.Where(x => x.TenDangNhap.ToLower().Contains(tuKhoa)).ToList();
                ListNhanVien = new ObservableCollection<NguoiDung>(ketQua);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); }
        }

        void ClearForm()
        {
            TenDangNhap = string.Empty;
            MatKhau = string.Empty;
            VaiTroDuocChon = null;
            SelectedItem = null;
        }
    }
}