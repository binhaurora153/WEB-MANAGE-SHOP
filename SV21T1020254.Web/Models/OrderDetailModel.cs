using _21T1020254.DomainModels;

namespace SV21T1020254.Web.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }

       
    }
}
