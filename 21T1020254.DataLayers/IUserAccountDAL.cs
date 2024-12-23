using _21T1020254.DomainModels;

namespace _21T1020254.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Kiểm tra xem tên đăng nhâp và mật khẩu có đúng hay không?
        /// nếu đúng trả vể thông tin của User, nếu sai return null;
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);

        bool ChangePassword(string username, string oldpassword, string newpassword);
        bool Register(string email, string password, string customerName, string contactName, string province, string address, string phone);


            // Các phương thức khác đã có
            bool CheckEmailExists(string email);
        Customer? GetCustomerInfo(int customerID);
        bool UpdateCustomerInfo(Customer model);
    }
}
