using _21T1020254.DataLayers.SQLServer;
using _21T1020254.DataLayers;
using _21T1020254.DomainModels;

namespace _21T1020254.BusinessLayers
{
    public static class OrderCustomerService
    {
        private static readonly IOrderDAL orderCustomerDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static OrderCustomerService()
        {
            string connectionString = Configuration.ConnectionString;

            orderCustomerDB = new OrderDAL(connectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang
        /// </summary>
        //public static List<Order> ListOrders(out int rowCount, int page = 1, int pageSize = 0,
        //int status = 0, DateTime? fromTime = null, DateTime? toTime = null,

        //string searchValue = "")
        //{
        //    rowCount = orderCustomerDB.Count(status, fromTime, toTime, searchValue);
        //    return orderCustomerDB.List(page, pageSize, status, fromTime, toTime, searchValue).ToList();
        //}
        public static List<Order> ListOrders(out int rowCount, int page = 1, int pageSize = 0,
        int status = 0, DateTime? fromTime = null, DateTime? toTime = null,

        string searchValue = "", int? customerId = null)
        {
            rowCount = orderCustomerDB.CountOrderCustomer(status, fromTime, toTime, searchValue,customerId);
            return orderCustomerDB.ListOrderCustomers(page, pageSize, status, fromTime, toTime, searchValue,customerId).ToList();
        }
        public static int InitOrder( int customerID,

        string deliveryProvince, string deliveryAddress,
        IEnumerable<OrderDetail> details)

        {
            if (details.Count() == 0)
                return 0;
            Order data = new Order()
            {
               
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress
            };
            int orderID = orderCustomerDB.Add(data);
            if (orderID > 0)
            {
                foreach (var item in details)
                {
                    orderCustomerDB.SaveDetail(orderID, item.ProductID, item.Quantity, item.SalePrice);
                }
                return orderID;
            }
            return 0;
        }
        public static List<OrderDetail> ListOrderDetails(int orderID)
        {
            return orderCustomerDB.ListDetails(orderID).ToList();
        }
        public static Order? GetOrder(int orderID)
        {
            return orderCustomerDB.Get(orderID);
        }
        /// <summary>
        /// Lấy thông tin của 1 mặt hàng được bán trong đơn hàng
        /// </summary>
        public static OrderDetail? GetOrderDetail(int orderID, int productID)
        {
            return orderCustomerDB.GetDetail(orderID, productID);
        }

    }
}
