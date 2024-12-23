using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020254.Web.Models;
using System.Globalization;

namespace SV21T1020254.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.SALE}")]
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        //ten bien session luu gio hang
        private const string SHOPPING_CART = "ShoppingCart";

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
                    TimeRange = $"{DateTime.Today.AddYears(-2).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyy", cultureInfo)} "
                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                                        condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
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
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount=rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity < 0)

                return Json("Giá bán và số lượng không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }


        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
       
        public IActionResult Init(int customerID=0,string deliveryProvince="",string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng chọn một mặt hàng cần bán");

            if (customerID==0||string.IsNullOrWhiteSpace(deliveryProvince)|| string.IsNullOrWhiteSpace(deliveryAddress))
            
                return Json("Vui nhập đầy đủ thông tin khách hàng và nơi giao hàng");

            /*  int employeeID = 1; //todo: Thay bởi ID của nhân viên đnag login vào hệ thống*/
            var userData = User.GetUserData();
            int employeeID = 0;
            if (userData != null)
            {
                employeeID = int.Parse(userData.UserId);
            }
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach(var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID=item.ProductID,
                    Quantity=item.Quantity,
                    SalePrice=item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }


        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }


        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            // Lấy chi tiết đơn hàng dựa trên OrderID và ProductID
            var orderDetail = OrderDataService.GetOrderDetail(id, productId);

            if (orderDetail == null)
            {
                return RedirectToAction("Index");
            }

            // Truyền chi tiết đơn hàng tới view
            return View(orderDetail);
        }
        public ActionResult UpdateDetail(OrderDetail data)
        {
            // Kiểm tra mã sản phẩm
            if (data.ProductID <= 0)
            {
                ModelState.AddModelError(nameof(data.ProductID), "Sản phẩm không tồn tại.");
            }

            // Kiểm tra số lượng
            if (data.Quantity < 1)
            {
                ModelState.AddModelError(nameof(data.Quantity), "Số lượng phải lớn hơn 0.");
            }

            // Kiểm tra giá bán
            if (data.SalePrice <= 0) // Thêm điều kiện kiểm tra
            {
                ModelState.AddModelError(nameof(data.SalePrice), "Vui lòng nhập giá bán lớn hơn 0.");
            }

            // Nếu có lỗi, trả về trang chi tiết cùng với các thông báo lỗi
            if (!ModelState.IsValid)
            {
                // Lấy thông tin chi tiết đơn hàng hiện tại
                var orderDetails = OrderDataService.ListOrderDetails(data.OrderID);
                var order = OrderDataService.GetOrder(data.OrderID);

                // Trả về view "Details" cùng với dữ liệu lỗi và thông tin đơn hàng
                return View("Details", new OrderDetailModel
                {
                    Order = order,
                    Details = orderDetails
                });
            }

            // Lưu thông tin chi tiết đơn hàng nếu tất cả kiểm tra hợp lệ
            OrderDataService.SaveOrderDetail(data.OrderID, data.ProductID, data.Quantity, data.SalePrice);

            return RedirectToAction("Details", new { id = data.OrderID });
        }
        public IActionResult DeleteDetail(int id, int productId)
        {
            // Kiểm tra thông tin đơn hàng
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return RedirectToAction("Details", new { id });
            }

            // Kiểm tra trạng thái đơn hàng trước khi cho phép xóa
            if (order.Status != Constants.ORDER_INIT && order.Status != Constants.ORDER_ACCEPTED)
            {
                ModelState.AddModelError("", "Chỉ có thể xóa chi tiết đơn hàng khi đơn hàng ở trạng thái 'Init' hoặc 'Accepted'.");
                return RedirectToAction("Details", new { id });
            }

            // Xóa chi tiết đơn hàng
            bool success = OrderDataService.DeleteOrderDetail(id, productId);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể xóa chi tiết đơn hàng.");
            }

            // Quay lại trang chi tiết đơn hàng
            return RedirectToAction("Details", new { id });
        }



        public IActionResult Shipping(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var shippers = CommonDataService.ListOfShippers();
            ViewBag.Shippers = shippers; // Dữ liệu cho dropdown

            return View(order); // Gửi thông tin đơn hàng sang View
        }
        [HttpPost]
        public IActionResult Shipping(int id, int shipperID)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("Details", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Shipping"
            bool success = OrderDataService.ShipOrder(id, shipperID);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể chuyển giao đơn hàng.");
                return View("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }
        [HttpGet]
        public IActionResult Finish(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("Details", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Finished"
            bool success = OrderDataService.FinishOrder(id);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể hoàn thành đơn hàng.");
                return View("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }


        // Canceled - Hủy bỏ đơn hàng
        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("Details", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Canceled"
            bool success = OrderDataService.CancelOrder(id);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể hủy đơn hàng.");
                return View("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }


        // Rejected - Từ chối đơn hàng
        [HttpGet]
        public IActionResult Reject(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("Details", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Rejected"
            bool success = OrderDataService.RejectOrder(id);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể từ chối đơn hàng.");
                return View("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Lấy thông tin đơn hàng cần xóa
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return RedirectToAction("Index");
            }

            // Thực hiện xóa đơn hàng
            bool success = OrderDataService.DeleteOrder(id);
            if (!success)
            {
                ModelState.AddModelError("loii", "Không thể xóa đơn hàng. Chỉ có thể xóa các đơn hàng ở trạng thái khởi tạo, hủy hoặc từ chối.");
                return RedirectToAction("Details", new { id });
            }

            // Nếu xóa thành công, quay lại danh sách đơn hàng
            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpGet]
        public IActionResult Accept(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return RedirectToAction("Details", new { id });
            }

            // Lấy thông tin nhân viên từ user đang đăng nhập
            var userData = User.GetUserData();
            if (userData == null)
            {
                ModelState.AddModelError("", "Không thể lấy thông tin nhân viên đang đăng nhập.");
                return RedirectToAction("Details", new { id });
            }

            int employeeID = int.Parse(userData.UserId);
            string employeeName = userData.DisplayName;

            // Duyệt đơn hàng
            bool success = OrderDataService.AcceptOrder(id, employeeID, employeeName);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể chấp nhận đơn hàng.");
                return RedirectToAction("Details", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("Details", new { id });
        }

        //public IActionResult Accept(int id)
        //{
        //    var order = OrderDataService.GetOrder(id);
        //    if (order == null)
        //    {
        //        ModelState.AddModelError("", "Đơn hàng không tồn tại.");
        //        return View("Details", new { id });
        //    }

        //    // Cập nhật trạng thái đơn hàng thành "Accepted"
        //    bool success = OrderDataService.AcceptOrder(id);
        //    if (!success)
        //    {
        //        ModelState.AddModelError("", "Không thể chấp nhận đơn hàng.");
        //        return View("Details", new { id });
        //    }

        //    // Nếu thành công, quay lại trang Details
        //    return RedirectToAction("Details", new { id });
        //}

    }
}

