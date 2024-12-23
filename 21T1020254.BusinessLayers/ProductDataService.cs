using _21T1020254.DataLayers;
using _21T1020254.DataLayers.SQLServer;
using _21T1020254.DomainModels;

namespace _21T1020254.BusinessLayers
{
    /// <summary>
    /// Các nghiệp vụ quản lý hàng hóa
    /// </summary>
    public class ProductDataService
    {
        private static readonly ProductDAL productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(searchValue: searchValue);
        }

        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
        }

        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }

        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }

        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }


        public static bool DeleteProduct(int productID)
        {
            return productDB.Delete(productID);
        }

        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }

        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID);
        }

        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }

        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }

        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }

        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }

        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID);
        }

        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }

        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }

        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }

        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }

        public static long GetConflictingAttributeID(int ProductID, int DisplayOrder, long AttributeID)
        {
            return productDB.GetConflictingAttributeID(ProductID, DisplayOrder, AttributeID);
        }

        public static long GetConflictingPhotoID(int productID, int displayOrder, long photoID)
        {
            return productDB.GetConflictingPhotoID(productID, displayOrder, photoID);
        }

        public static bool ExistsAttribute(int productID, string attributeName, string attributeValue, long attributeID)
        {
            return productDB.ExistsAttribute(productID, attributeName, attributeValue, attributeID);
        }

        public static bool ExistsDisplayOrderPhoto(int ProductID, int DisplayOrder, long PhotoID)
        {
            return productDB.ExistsDisplayOrderPhoto(ProductID, DisplayOrder, PhotoID);
        }
    }
}

