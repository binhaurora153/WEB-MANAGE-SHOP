using _21T1020254.DomainModels;

namespace _21T1020254.DataLayers
{
    


    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu liên quan đến mặt hàng
    /// </summary>
    public interface IProductDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang (0 nếu không phân trang)</param>
        /// <param name="searchValue">Tên mặt hàng cần tìm (để trống nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns>Danh sách sản phẩm</returns>
        List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng cần tìm (để trống nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns>Số lượng sản phẩm</returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID">Mã sản phẩm</param>
        /// <returns>Thông tin sản phẩm</returns>
        Product? Get(int productID);

        /// <summary>
        /// Thêm sản phẩm mới (hàm trả về mã của mặt hàng được bổ sung)
        /// </summary>
        /// <param name="data">Dữ liệu sản phẩm</param>
        /// <returns>Mã sản phẩm</returns>
        int Add(Product data);

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="data">Dữ liệu sản phẩm</param>
        /// <returns>Trạng thái cập nhật</returns>
        bool Update(Product data);

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <param name="productID">Mã sản phẩm</param>
        /// <returns>Trạng thái xóa</returns>
        bool Delete(int productID);

        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có đơn hàng liên quan hay không?
        /// </summary>
        /// <param name="productID">Mã sản phẩm</param>
        /// <returns>Trạng thái sử dụng</returns>
        bool InUsed(int productID);

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID">Mã sản phẩm</param>
        /// <returns>Danh sách ảnh sản phẩm</returns>
        List<ProductPhoto> ListPhotos(int productID);

        /// <summary>
        /// Lấy thông tin một ảnh dựa vào ID
        /// </summary>
        /// <param name="photoID">Mã ảnh</param>
        /// <returns>Thông tin ảnh</returns>
        ProductPhoto? GetPhoto(long photoID);

        /// <summary>
        /// Bổ sung một ảnh cho mặt hàng (hàm trả về mã của ảnh được bổ sung)
        /// </summary>
        /// <param name="data">Dữ liệu ảnh</param>
        /// <returns>Mã ảnh</returns>
        long AddPhoto(ProductPhoto data);

        /// <summary>
        /// Cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="data">Dữ liệu ảnh</param>
        /// <returns>Trạng thái cập nhật</returns>
        bool UpdatePhoto(ProductPhoto data);


        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID">Mã ảnh</param>
        /// <returns>Trạng thái xóa</returns>
        bool DeletePhoto(long photoID);

        /// <summary>
        /// Lấy danh sách các thuộc tính của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID">Mã sản phẩm</param>
        /// <returns>Danh sách thuộc tính sản phẩm</returns>
        List<ProductAttribute> ListAttributes(int productID);

        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID">Mã thuộc tính</param>
        /// <returns>Thông tin thuộc tính</returns>
        ProductAttribute? GetAttribute(long attributeID);
        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data">Dữ liệu thuộc tính</param>
        /// <returns>Mã thuộc tính</returns>
        long AddAttribute(ProductAttribute data);

        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data">Dữ liệu thuộc tính</param>
        /// <returns>Trạng thái cập nhật</returns>
        bool UpdateAttribute(ProductAttribute data);

        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="attributeID">Mã thuộc tính</param>
        /// <returns>Trạng thái xóa</returns>
        bool DeleteAttribute(long attributeID);

        public long GetConflictingAttributeID(int productID, int displayOrder, long attributeID);

        public long GetConflictingPhotoID(int productID, int displayOrder, long photoID);
    }
}
