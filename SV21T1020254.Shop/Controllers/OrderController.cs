using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Shop.Models;
using System.Globalization;
using System.Security.Claims;

namespace SV21T1020254.Shop.Controllers
{
    [Authorize] // Yêu cầu đăng nhập trước khi truy cập
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 10;
        public IActionResult Index()
        {
            ViewBag.StatusList = new Dictionary<int, string>
            {
                { 0, "-- Trạng thái --" },
                { 1, "Đơn hàng mới (chờ duyệt)" },
                { 2, "Đơn hàng đã duyệt (chờ chuyển hàng)" },
                { 3, "Đơn hàng đang được giao" },
                { 4, "Đơn hàng đã hoàn tất thành công" },
                { -1, "Đơn hàng bị hủy" },
                { -2, "Đơn hàng bị từ chối" }
            };
            var customerIdClaim = User.FindFirstValue(nameof(WebUserData.UserId));
            int? customerId = null;
            if (!string.IsNullOrEmpty(customerIdClaim))
            {
                customerId = int.Parse(customerIdClaim);
            }
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-US");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddYears(-2).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyy", cultureInfo)} ",
                    CustomerID = customerId // Thêm CustomerId vào điều kiện
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var customerIdClaim = User.FindFirstValue(nameof(WebUserData.UserId));
            int? customerId = null;
            if (!string.IsNullOrEmpty(customerIdClaim))
            {
                customerId = int.Parse(customerIdClaim);
            }
            var data = OrderCustomerService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                                        condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "", customerId);
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);

        }

        [HttpPost]
        public IActionResult Init([FromBody] OrderInitRequest request)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            var customerIdClaim = User.FindFirstValue(nameof(WebUserData.UserId));
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                // Trả về mã lỗi 401 nếu chưa đăng nhập
                return Unauthorized(); // Thay vì chuyển hướng, trả về mã lỗi 401
            }

            // Kiểm tra tính hợp lệ của thông tin giao hàng
            if (request == null || string.IsNullOrWhiteSpace(request.DeliveryProvince) || string.IsNullOrWhiteSpace(request.DeliveryAddress))
            {
                return BadRequest("Thông tin giao hàng không hợp lệ.");
            }

            // Lấy danh sách sản phẩm từ giỏ hàng
            var cartItems = CartService.GetBusinessCartItems();
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Giỏ hàng của bạn đang trống.");
            }

            // Tạo danh sách chi tiết đơn hàng
            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                ProductID = item.Product.ProductID,
                Quantity = item.Quantity,
                SalePrice = item.Product.Price
            }).ToList();

            // Chuyển customerId từ Claim sang int
            int customerID = int.Parse(customerIdClaim);

            // Tạo đơn hàng mới
            int orderID = OrderCustomerService.InitOrder(
                customerID: customerID,
                deliveryProvince: request.DeliveryProvince,
                deliveryAddress: request.DeliveryAddress,
                details: orderDetails
            );

            if (orderID > 0)
            {
                // Xóa giỏ hàng sau khi tạo đơn hàng thành công
                CartService.ClearCart();

                // Trả về mã đơn hàng để chuyển hướng sang trang chi tiết đơn hàng
                return Json(orderID);
            }

            return StatusCode(500, "Không thể tạo đơn hàng. Vui lòng thử lại.");
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderCustomerService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderCustomerService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }


    }

    // DTO cho yêu cầu tạo đơn hàng
    public class OrderInitRequest
    {
        public string DeliveryProvince { get; set; }
        public string DeliveryAddress { get; set; }
    }
}

