using Microsoft.Data.SqlClient;

namespace _21T1020254.DataLayers.SQLServer
{
    public abstract class BaseDAL//abstract ke thua k tao ra doi tuong
    {
        protected string _connectionString="";

        public BaseDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        //tao va mo mot ket noi den sql server
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
    //BaseDAL obj = new BaseDAL();
}
