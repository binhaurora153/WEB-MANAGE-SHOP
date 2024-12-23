using _21T1020254.DomainModels;
using Dapper;
using System.Data;

namespace _21T1020254.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL, IcommonDAL<Category>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if exists (select *from Categories where CategoryName=@CategoryName)
                        select -1;
                        else
                            begin
                            insert into Categories(CategoryName,Description)
                              values (@CategoryName,@Description);

                              select SCOPE_IDENTITY();
                        end;";
                var parameter = new
                {

                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
                   
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: CommandType.Text);
                // Thực thi câu lệnh
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue.Trim()}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Categories
                             where (CategoryName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Categories where CategoryID  = @CategoryID";
                var parameter = new
                {
                    CategoryID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;

                connection.Close();
            }
            return result;
        }

        public Category? Get(int id)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryID  = @CategoryID";
                var parameters = new
                {
                    CategoryID = id,
                };
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where CategoryID = @CategoryID)

                        select 1
                            else
                        select 0";
                var parameters = new
                {
                    CategoryID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return result;
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
                                select *, 
                                         row_number() over(order by CategoryName) as RowNumber
                                from Categories
                                where (CategoryName like @searchValue)
                                    ) as t
                              where (@pageSize = 0)
                                      or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                              order by RowNumber;";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue//ben trai la ten tham so trong cau lenh sql, ben phai la value truyen cho tham so
                };
                data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                     if not exists (select *from Categories where CategoryID <> @CategoryID and CategoryName=@CategoryName)
                        begin
                UPDATE Categories 
                    SET CategoryName = @CategoryName,
                        Description = @Description
                    WHERE CategoryID = @CategoryID
            end;";
                var parameter = new
                {
                    CategoryID = data.CategoryID,
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "", // Thêm dòng này
                   

                };
                result = connection.Execute(sql: sql, param: parameter, commandType: CommandType.Text) > 0;
                connection.Close();
            };
            return result;
        }
    }
}
