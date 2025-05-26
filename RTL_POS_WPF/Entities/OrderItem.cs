using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTL_POS_WPF.Entities
{
    public class OrderItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Modifiers { get; set; }
        public decimal Price { get; set; }
    }
}
