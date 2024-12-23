
using _21T1020254.DomainModels;
using BCrypt.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _21T1020254.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserId,
                    Email as UserName,
                    CustomerName as DisplayName,
                    N'' as Photo,
                    N'' as RoleNames,
                    Password
            from Customers
            where Email = @Email";
                var parameters = new
                {
                    Email = username
                };

                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, parameters, commandType: CommandType.Text);

                if (data != null)
                {
                    try
                    {
                        // Kiểm tra mật khẩu với BCrypt
                        if (BCrypt.Net.BCrypt.Verify(password, data.Password))
                        {
                            return data; // Nếu mật khẩu đúng
                        }
                    }
                    catch (SaltParseException)
                    {
                        // Nếu gặp lỗi SaltParseException, trả về null và hiển thị thông báo lỗi chung
                        return null;
                    }
                }
                connection.Close();
            }

            // Trả về null nếu tài khoản không tồn tại hoặc mật khẩu sai
            return null;
        }




        public bool ChangePassword(string username, string password)
        {
            throw new NotImplementedException();
        }


        public bool ChangePassword(string username, string oldpassword, string newpassword)
        {
            bool result = false;
            // Check the old password
            UserAccount userAccount = Authorize(username, oldpassword);
            if (userAccount == null) return result; // Old password doesn't match

            using (var connection = OpenConnection())
            {
                // Hash the new password before saving it
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newpassword);

                var sql = @"UPDATE Customers
                            SET Password = @Password
                            WHERE Email = @Email";

                var parameters = new
                {
                    Email = username,
                    Password = hashedPassword
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool Register(string email, string password, string customerName, string contactName, string province, string address, string phone)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // Hash the password before saving it
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var sql = @"INSERT INTO Customers (Email, Password, CustomerName, ContactName, Province, Address, Phone, IsLocked) 
                            VALUES (@Email, @Password, @CustomerName, @ContactName, @Province, @Address, @Phone, 0)";

                var parameters = new
                {
                    Email = email,
                    Password = hashedPassword,
                    CustomerName = customerName,
                    ContactName = contactName,
                    Province = province,
                    Address = address,
                    Phone = phone
                };

                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        public bool CheckEmailExists(string email)
        {
            using (var connection = OpenConnection())
            {

                string query = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int count = (int)command.ExecuteScalar();
                    return count > 0; // Trả về true nếu email đã tồn tại
                }
            }
        }

        public Customer? GetCustomerInfo(int customerID)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT 
                        CustomerID,
                        CustomerName,
                        ContactName,
                        Province,
                        Address,
                        Phone,
                        Email,
                        IsLocked
                    FROM Customers
                    WHERE CustomerID = @CustomerID";
                return connection.QueryFirstOrDefault<Customer>(sql, new { customerID = customerID });
            }
        }

        public bool UpdateCustomerInfo(Customer model)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers
                    SET CustomerName = @CustomerName,
                        ContactName = @ContactName,
                        Province = @Province,
                        Address = @Address,
                        Phone = @Phone                     
                    WHERE CustomerID = @CustomerID";
                return connection.Execute(sql, model) > 0;
            }
        }

    }

}

//using _21T1020254.DomainModels;
//using Dapper;
//using Microsoft.Data.SqlClient;
//using System.Data;

//namespace _21T1020254.DataLayers.SQLServer
//{
//    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
//    {
//        public CustomerAccountDAL(string connectionString) : base(connectionString)
//        {
//        }

//        public UserAccount? Authorize(string username, string password)
//        {
//            UserAccount? data = null;
//            using (var connection = OpenConnection())
//            {
//                var sql = @"select	CustomerID as UserId,
//		                    Email as UserName,
//		                    CustomerName as DisplayName,
//		                    N'' as Photo,
//		                    N'' as RoleNames 
//                    from Customers
//                    where Email = @Email and Password = @Password";
//                var parameters = new
//                {
//                    Email = username,
//                    password = password

//                };
//                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, parameters, commandType: CommandType.Text);
//                connection.Close();

//            }
//            return data;
//        }

//        public bool ChangePassword(string username, string password)
//        {
//            throw new NotImplementedException();
//        }

//        public bool ChangePassword(string username, string oldpassword, string newpassword)
//        {
//            bool result = false;
//            // so sánh mật khẩu cũ trong database
//            UserAccount userAccount = Authorize(username, oldpassword);
//            if (userAccount == null) return result;
//            using (var connection = OpenConnection())
//            {
//                // cập nhật mật khẩu mới
//                var sql = @"UPDATE  Customers
//                    SET     Password = @Password
//                    WHERE   Email = @Email";

//                var parameters = new
//                {
//                    Email = username,
//                    Password = newpassword
//                };
//                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
//                connection.Close();
//            }
//            return result;
//        }
//        public bool Register(string email, string password, string customerName, string contactName, string province, string address, string phone)
//        {
//            bool result = false;
//            using (var connection = OpenConnection())
//            {
//                var sql = @"INSERT INTO Customers (Email, Password, CustomerName, ContactName, Province, Address, Phone, IsLocked) 
//                    VALUES (@Email, @Password, @CustomerName, @ContactName, @Province, @Address, @Phone, 0)";

//                var parameters = new
//                {
//                    Email = email,
//                    Password = password,
//                    CustomerName = customerName,
//                    ContactName = contactName,
//                    Province = province,
//                    Address = address,
//                    Phone = phone
//                };

//                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
//                connection.Close();
//            }
//            return result;
//        }
//        public bool CheckEmailExists(string email)
//        {
//            using (var connection = OpenConnection())
//            {

//                string query = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("@Email", email);
//                    int count = (int)command.ExecuteScalar();
//                    return count > 0; // Trả về true nếu email đã tồn tại
//                }
//            }
//        }

//        public Customer? GetCustomerInfo(int customerID)
//        {
//            using (var connection = OpenConnection())
//            {
//                var sql = @"SELECT 
//                        CustomerID,
//                        CustomerName,
//                        ContactName,
//                        Province,
//                        Address,
//                        Phone,
//                        Email,
//                        IsLocked
//                    FROM Customers
//                    WHERE CustomerID = @CustomerID";
//                return connection.QueryFirstOrDefault<Customer>(sql, new { customerID = customerID });
//            }
//        }

//        public bool UpdateCustomerInfo(Customer model)
//        {
//            using (var connection = OpenConnection())
//            {
//                var sql = @"UPDATE Customers
//                    SET CustomerName = @CustomerName,
//                        ContactName = @ContactName,
//                        Province = @Province,
//                        Address = @Address,
//                        Phone = @Phone                     
//                    WHERE CustomerID = @CustomerID";
//                return connection.Execute(sql, model) > 0;
//            }
//        }

//    }

//}
