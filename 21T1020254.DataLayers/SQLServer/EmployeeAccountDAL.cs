using _21T1020254.DomainModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _21T1020254.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId,
                                Email as UserName,
                                FullName as DisplayName,
                                Photo,  
                                RoleNames
                            from Employees                      
                            where Email=@Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string oldpassword, string newpassword)
        {
            bool result = false;
            // so sánh mật khẩu cũ trong database
            UserAccount userAccount = Authorize(username, oldpassword);
            if (userAccount == null) return result;
            using (var connection = OpenConnection())
            {
                // cập nhật mật khẩu mới
                var sql = @"UPDATE  Employees
                    SET     Password = @Password
                    WHERE   Email = @Email";

                var parameters = new
                {
                    Email = username,
                    Password = newpassword
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool CheckEmailExists(string email)
        {
            throw new NotImplementedException();
        }

       

        public Customer? GetCustomerInfo(int customerID)
        {
            throw new NotImplementedException();
        }

        public bool Register(string email, string password, string customerName, string contactName, string province, string address, string phone)
        {
            // Không hỗ trợ đăng ký nhân viên
            throw new NotImplementedException("Đăng ký nhân viên chưa được hỗ trợ.");
        }

        public bool UpdateCustomerInfo(Customer model)
        {
            throw new NotImplementedException();
        }
    }
}
