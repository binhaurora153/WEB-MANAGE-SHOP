using _21T1020254.BusinessLayers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SV21T1020254.Shop.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Gọi phương thức để tính số mặt hàng
            ViewData["CartItemCountDistinct"] = CartService.GetBusinessCartItems().Count;
            base.OnActionExecuting(context);
        }
    }

}
