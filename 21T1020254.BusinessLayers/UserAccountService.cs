using _21T1020254.DataLayers;
using _21T1020254.DataLayers.SQLServer;
using _21T1020254.DomainModels;

namespace _21T1020254.BusinessLayers
{
    public static class UserAccountService
    {

        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
        public static bool ChangePassword(UserTypes userType, string username, string oldpassword, string newpassword)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.ChangePassword(username, oldpassword, newpassword);
            else
                return customerAccountDB.ChangePassword(username, oldpassword, newpassword);
        }
        // Thêm phương thức Register
        public static bool Register(UserTypes userType, string email, string password, string customerName, string contactName, string province, string address, string phone)
        {
           if (userType == UserTypes.Customer)
            {
                return customerAccountDB.Register(email, password, customerName, contactName, province, address, phone);
            }

            return false;
        }
        public static bool CheckEmailExists(string email)
        {
            return customerAccountDB.CheckEmailExists(email);
        }
        public static Customer? GetCustomerInfo(int customerID)
        {
            return customerAccountDB.GetCustomerInfo(customerID);
        }
        public static bool UpdateCustomerInfo(Customer model)
        {
            return customerAccountDB.UpdateCustomerInfo(model);
        }

    }
    public enum UserTypes
    {
        Employee,
        Customer
    }



}
