using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Model
{
    public partial class NGUYENLIEU
    {
        public bool IsLowStock
        {
            get => TonKho <= MucCanhBao;
        }
    }
}
