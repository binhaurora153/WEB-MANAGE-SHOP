using _21T1020254.DomainModels;

namespace _21T1020254.BusinessLayers
{
    public static class CartService
    {
        private static List<BusinessCartItem> Cart = new List<BusinessCartItem>();

        public static List<BusinessCartItem> GetBusinessCartItems()
        {
            return Cart;
        }
        public static void ClearCart()
        {
            Cart.Clear(); // Cart là danh sách các mục trong giỏ hàng
        }

        public static void AddToCart(int productId, int quantity)
        {
            var product = ProductDataService.GetProduct(productId);
            if (product == null) return;

            var item = Cart.FirstOrDefault(x => x.Product.ProductID == productId);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                Cart.Add(new BusinessCartItem { Product = product, Quantity = quantity });
            }
        }

        public static void RemoveFromCart(int productId)
        {
            var item = Cart.FirstOrDefault(x => x.Product.ProductID == productId);
            if (item != null)
            {
                Cart.Remove(item);
            }
        }

        public static void UpdateCart(int productId, int quantity)
        {
            var item = Cart.FirstOrDefault(x => x.Product.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

    }

    public class BusinessCartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

}
