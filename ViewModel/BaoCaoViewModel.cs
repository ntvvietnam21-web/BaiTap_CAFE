using Cafe.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class ChiTietBaoCao
    {
        public int STT { get; set; }
        public string TenLoai { get; set; } 
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public int SoLuong { get; set; }
        public int ThanhTien { get; set; }
    }

    public class BaoCaoViewModel : BaseViewModel
    {
        private ObservableCollection<ChiTietBaoCao> _listDuLieuIn;
        public ObservableCollection<ChiTietBaoCao> ListDuLieuIn
        {
            get => _listDuLieuIn;
            set { _listDuLieuIn = value; OnPropertyChanged(); }
        }

        private string _tongTien;
        public string TongTien
        {
            get => _tongTien;
            set { _tongTien = value; OnPropertyChanged(); }
        }

        private string _ngayLap;
        public string NgayLap
        {
            get => _ngayLap;
            set { _ngayLap = value; OnPropertyChanged(); }
        }

        private string _nguoiLap;
        public string NguoiLap
        {
            get => _nguoiLap;
            set { _nguoiLap = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; set; }

        public BaoCaoViewModel()
        {
            LoadBaoCao();

            RefreshCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                LoadBaoCao();
            });
        }

        public void LoadBaoCao()
        {
            try
            {
                using (var db = new QL_CuaHangNuocEntities())
                {
                    NgayLap = "Kỳ báo cáo: Tất cả thời gian (In ngày " + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    string tenNV = LoginViewModel.AppSession.TenHienThi ?? "Nguyễn Trường Vỹ";
                    NguoiLap = "Người lập: " + tenNV;

                    var query = from ct in db.CHITIET_HDB
                                join hd in db.HOADON_BAN on ct.MaHDB equals hd.MaHDB
                                join m in db.MENUs on ct.MaMon equals m.MaMon
                                join lm in db.LOAI_MON on m.MaLoai equals lm.MaLoai 
                                group ct by new { lm.TenLoai, m.MaMon, m.TenMon } into g
                                select new
                                {
                                    TenLoai = g.Key.TenLoai, 
                                    MaMon = g.Key.MaMon,
                                    TenMon = g.Key.TenMon,
                                    SoLuong = g.Sum(x => x.SoLuong),
                                    ThanhTien = g.Sum(x => x.ThanhTien)
                                };

                    var listData = query.OrderBy(x => x.TenLoai).ThenBy(x => x.TenMon).ToList();
                    var resultList = new ObservableCollection<ChiTietBaoCao>();

                    int stt = 1;
                    long tongTienAll = 0;

                    foreach (var item in listData)
                    {
                        resultList.Add(new ChiTietBaoCao
                        {
                            STT = stt++,
                            TenLoai = item.TenLoai,
                            MaMon = item.MaMon,
                            TenMon = item.TenMon,
                            SoLuong = item.SoLuong ?? 0,
                            ThanhTien = item.ThanhTien ?? 0
                        });
                        tongTienAll += item.ThanhTien ?? 0;
                    }

                    ListDuLieuIn = resultList;
                    TongTien = string.Format("{0:N0} VNĐ", tongTienAll);
                }
            }
            catch (Exception)
            {
                ListDuLieuIn = new ObservableCollection<ChiTietBaoCao>();
                TongTien = "0 VNĐ";
            }
        }
    }
}