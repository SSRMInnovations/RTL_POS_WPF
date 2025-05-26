using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTL_POS_WPF.Entities
{
    public class OrderSummary
    {
        public string CustomerId { get; set; }
        public string PaymentMethodId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
