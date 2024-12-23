using _21T1020254.DomainModels;
using Dapper;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _21T1020254.DataLayers.SQLServer
{
   

    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                            VALUES(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                            SELECT SCOPE_IDENTITY();";
                var parameters = new
                {
                    data.ProductName,
                    data.ProductDescription,
                    data.SupplierID,
                    data.CategoryID,
                    data.Unit,
                    data.Price,
                    data.Photo,
                    data.IsSelling,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            SELECT SCOPE_IDENTITY();";
                var parameters = new
                {
                    data.ProductID,
                    data.AttributeName,
                    data.AttributeValue,
                    data.DisplayOrder,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            SELECT SCOPE_IDENTITY();";
                var parameters = new
                {
                    data.ProductID,
                    data.Photo,
                    data.Description,
                    data.DisplayOrder,
                    data.IsHidden,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                            FROM Products
                            WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                AND (Price >= @MinPrice)
                                AND (@MaxPrice <= 0 OR Price <= @MaxPrice)";
                var parameters = new
                {
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                };
                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductPhotos WHERE ProductID = @ProductID;
                            DELETE FROM ProductAttributes WHERE ProductID = @ProductID;
                            DELETE FROM Products WHERE ProductID = @ProductID
                            AND NOT EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID);";
                var parameters = new { ProductID = productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductAttributes WHERE AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductPhotos WHERE PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS (SELECT * FROM OrderDetails WHERE ProductID = @ProductID)
                                SELECT 1
                            ELSE
                                SELECT 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = [];
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                              SELECT ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber, *
                              FROM Products
                              WHERE (ProductName LIKE @SearchValue)
                                AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                AND (@SupplierID = 0 OR SupplierID = @SupplierID)
                                AND (Price >= @MinPrice)
                                AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                            ) AS t
                            WHERE (@PageSize = 0)
                              OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    PageSize = pageSize,
                    Page = page,
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = [];
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE ProductID = @ProductID ORDER BY DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = [];
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE ProductID = @ProductID ORDER BY DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductPhoto>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Products
                            SET ProductName = @ProductName,
                                ProductDescription = @ProductDescription,
                                SupplierID = @SupplierID,
                                CategoryID = @CategoryID,
                                Unit = @Unit,
                                Price = @Price,
                                Photo = @Photo,
                                IsSelling = @IsSelling
                            WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName,
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductAttributes
                            SET ProductID = @ProductID,
                                AttributeName = @AttributeName,
                                AttributeValue = @AttributeValue,
                                DisplayOrder = @DisplayOrder
                            WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    data.ProductID,
                    data.AttributeName,
                    data.AttributeValue,
                    data.DisplayOrder,
                    data.AttributeID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductPhotos
                            SET ProductID = @ProductID,
                                Photo = @Photo,
                                Description = @Description,
                                DisplayOrder = @DisplayOrder,
                                IsHidden = @IsHidden
                            WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    data.ProductID,
                    data.Photo,
                    data.Description,
                    data.DisplayOrder,
                    data.IsHidden,
                    data.PhotoID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool ExistsDisplayOrderPhoto(int ProductID, int DisplayOrder, long PhotoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS ( SELECT PhotoID FROM ProductPhotos
                                        WHERE ProductID = @ProductID
                                            AND DisplayOrder = @DisplayOrder
                                            AND PhotoID <> @PhotoID)
                                SELECT 1
                            ELSE
                                SELECT 0";
                var parameters = new
                {
                    ProductID,
                    DisplayOrder,
                    PhotoID,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public long GetConflictingAttributeID(int productID, int displayOrder, long attributeID)
        {
            long conflictingAttributeID = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS ( SELECT AttributeID FROM ProductAttributes
                                        WHERE ProductID = @ProductID
                                            AND DisplayOrder = @DisplayOrder
                                            AND AttributeID <> @AttributeID)
                                SELECT AttributeID FROM ProductAttributes
                                WHERE ProductID = @ProductID
                                AND DisplayOrder = @DisplayOrder
                            ELSE
                                SELECT 0";
                var parameters = new
                {
                    ProductID = productID,
                    DisplayOrder = displayOrder,
                    AttributeID = attributeID,
                };
                conflictingAttributeID = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return conflictingAttributeID;
        }

        public long GetConflictingPhotoID(int productID, int displayOrder, long photoID)
        {
            long conflictingPhotoID = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS ( SELECT PhotoID FROM ProductPhotos
                                        WHERE ProductID = @ProductID
                                            AND DisplayOrder = @DisplayOrder
                                            AND PhotoID <> @PhotoID)
                                SELECT PhotoID FROM ProductPhotos
                                WHERE ProductID = @ProductID
                                AND DisplayOrder = @DisplayOrder
                            ELSE
                                SELECT 0";
                var parameters = new
                {
                    ProductID = productID,
                    DisplayOrder = displayOrder,
                    PhotoID = photoID,
                };
                conflictingPhotoID = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return conflictingPhotoID;
        }

        public bool ExistsAttribute(int ProductID, string AttributeName, string AttributeValue, long AttributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS ( SELECT AttributeID FROM ProductAttributes
                                        WHERE ProductID = @ProductID
                                            AND AttributeName = @AttributeName
                                            AND AttributeValue = @AttributeValue
                                            AND AttributeID <> @AttributeID)
                                SELECT 1
                            ELSE
                                SELECT 0";
                var parameters = new
                {
                    ProductID,
                    AttributeName,
                    AttributeValue,
                    AttributeID,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }
    }
}