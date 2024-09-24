using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace R_R_19102023
{
    internal class decor : UIElement
    {
        public decor(int type)
        {
            this.type = type;
        }

        private int type = 0;
    }
}
