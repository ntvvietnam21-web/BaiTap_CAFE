using Cafe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.ViewModel
{
    public class GioHang : BaseViewModel
    {
        public MENU SanPham { get; set; }

        private int _donGiaItem;
        public int DonGiaItem
        {
            get => _donGiaItem;
            set
            {
                _donGiaItem = value;
                ThanhTien = _soLuong * _donGiaItem;
                OnPropertyChanged();
            }
        }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                _soLuong = value;
                ThanhTien = _soLuong * DonGiaItem;
                OnPropertyChanged();
            }
        }

        private int _thanhTien;
        public int ThanhTien
        {
            get => _thanhTien;
            set { _thanhTien = value; OnPropertyChanged(); }
        }
    }
}