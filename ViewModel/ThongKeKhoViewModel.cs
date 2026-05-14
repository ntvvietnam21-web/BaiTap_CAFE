using Cafe.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Cafe.ViewModel
{
    public class ThongKeKhoViewModel : BaseViewModel
    {
        private ObservableCollection<NGUYENLIEU> _listNguyenLieu;
        public ObservableCollection<NGUYENLIEU> ListNguyenLieu
        {
            get => _listNguyenLieu;
            set { _listNguyenLieu = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; set; }

        public ThongKeKhoViewModel()
        {
            LoadData();
            RefreshCommand = new RelayCommand<object>((p) => true, (p) => LoadData());
        }

        public void LoadData()
        {
            using (var db = new QL_CuaHangNuocEntities())
            {
                var list = db.NGUYENLIEUx.ToList();
                ListNguyenLieu = new ObservableCollection<NGUYENLIEU>(list);
            }
        }
    }
}