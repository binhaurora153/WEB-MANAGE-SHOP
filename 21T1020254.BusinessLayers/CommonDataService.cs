using _21T1020254.DataLayers;
using _21T1020254.DataLayers.SQLServer;
using _21T1020254.DomainModels;

namespace _21T1020254.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly IcommonDAL<Customer> customerDB;
        private static readonly IcommonDAL<Category> categoryDB;
        private static readonly IcommonDAL<Shipper> shipperDB;
        private static readonly IcommonDAL<Supplier> supplierDB;
        private static readonly IcommonDAL<Employee> employeeDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;

        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
        }
        //tim kiem vaf lay danh sach customer phan trang
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }

        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);

            return customerDB.List(page, pageSize, searchValue);
        }

        public static List<Customer> ListOfCustomers(string searchValue = "")
        {
            

            return customerDB.List(1,0,searchValue).ToList();
        }
        //lấy 1 khách hàng có mã là id
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);

            return categoryDB.List(page, pageSize, searchValue);
        }
        public static List<Category> ListOfCategories(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return categoryDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Trả về tên danh mục dựa trên ID
        /// </summary>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>Tên danh mục hoặc "Không xác định" nếu không tìm thấy</returns>
        public static string GetCategoryName(int? categoryId)
        {
            if (categoryId == null)
                return "Không xác định";

            var category = categoryDB.Get(categoryId.Value);
            return category != null ? category.CategoryName : "Không xác định";
        }

        /// <summary>
        /// Trả về tên nhà cung cấp dựa trên ID
        /// </summary>
        /// <param name="supplierId">ID của nhà cung cấp</param>
        /// <returns>Tên nhà cung cấp hoặc "Không xác định" nếu không tìm thấy</returns>
        public static string GetSupplierName(int? supplierId)
        {
            if (supplierId == null)
                return "Không xác định";

            var supplier = supplierDB.Get(supplierId.Value);
            return supplier != null ? supplier.SupplierName : "Không xác định";
        }
    

    //lấy 1 ncc có mã là id
    public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        //
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);

            return supplierDB.List(page, pageSize, searchValue);
        }
        public static List<Supplier> ListOfSuppliers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return supplierDB.List(page, pageSize, searchValue);
        }

        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }

        //
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);

            return employeeDB.List(page, pageSize, searchValue);
        }
        //lấy 1 nv có mã là id
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        //
        
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);

            return shipperDB.List(page, pageSize, searchValue);
        }
        public static List<Shipper> ListOfShippers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return shipperDB.List(page, pageSize, searchValue);
        }
        //lấy 1 shipper có mã là id
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        //

    }
}

//lop static la gi
//contructor trong lop static suw dung the nao?khac constructor cua lop thong thuong cho nao



