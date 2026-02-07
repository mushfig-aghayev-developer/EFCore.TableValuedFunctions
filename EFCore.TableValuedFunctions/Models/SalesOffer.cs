using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.TableValuedFunctions.Models
{
    public class SalesOffer
    {
        public string? CarrierTrackingNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalSum { get; set; }
        public string Category { get; set; } = null!;
        public decimal DiscountPct { get; set; }
        public int SpecialOfferID { get; set; }
    }
}
