using Cafe.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cafe.View
{
    /// <summary>
    /// Interaction logic for BaoCaoUC.xaml
    /// </summary>
    public partial class BaoCaoUC : UserControl
    {
        public BaoCaoUC()
        {
            InitializeComponent();
        }
        private void btnInBaoCao_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                PrintArea.Effect = null;
                printDialog.PrintVisual(PrintArea, "Báo Cáo Doanh Thu Cafe");
                PrintArea.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 10,
                    ShadowDepth = 5,
                    Opacity = 0.3
                };
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as BaoCaoViewModel;
            if (viewModel != null)
            {
                viewModel.LoadBaoCao();
            }
        }
    }
}
