using _21T1020254.BusinessLayers;
using _21T1020254.DomainModels;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace SV21T1020254.Shop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var product = ProductDataService.GetProduct(id); // Lấy thông tin sản phẩm
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public int GetCartItemCountDistinct()
        {
            var cart = CartService.GetBusinessCartItems();
            return cart.Count; // Đếm số lượng mặt hàng (không phụ thuộc vào số lượng từng mặt hàng)
        }

        public IActionResult ShoppingCart()
        {
            var cart = CartService.GetBusinessCartItems(); // Lấy các sản phẩm trong giỏ hàng
            ViewData["CartItemCountDistinct"] = GetCartItemCountDistinct(); // Số mặt hàng trong giỏ
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            CartService.AddToCart(id, 1); // Thêm sản phẩm vào giỏ hàng với số lượng 1
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult RemoveFromCart(int id)
        {
            CartService.RemoveFromCart(id); // Xóa sản phẩm khỏi giỏ hàng
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, int quantity)
        {
            if (quantity > 0)
            {
                CartService.UpdateCart(id, quantity); // Cập nhật số lượng trong giỏ hàng
            }
            return RedirectToAction("ShoppingCart"); // Quay lại giỏ hàng
        }
        public IActionResult ClearCart()
        {
            CartService.ClearCart(); // Xóa toàn bộ giỏ hàng
            return RedirectToAction("Index", "Home"); // Quay lại trang chủ
        }

    }
}
