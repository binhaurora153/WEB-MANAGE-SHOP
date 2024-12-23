namespace _21T1020254.DataLayers
{
    public interface ISimpleQueryDAL<T> where T : class

    {
        /// <summary>
        /// Lấy toàn bộ dữ liệu của 1 bảng
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
