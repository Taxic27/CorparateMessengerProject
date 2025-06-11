using Dapper;

namespace Server.Tools
{
    public interface IMainConnector
    {
        public T Get<T>(String sqlCommand, DynamicParameters parameters);

        public List<T> GetList<T>(String sqlCommand, DynamicParameters parameters = null);

        public void Execute(String sqlCommand, DynamicParameters parameters);
    }
}
