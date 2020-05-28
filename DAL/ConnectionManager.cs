using Oracle.ManagedDataAccess.Client;
namespace DAL
{
    public class ConnectionManager
    {
        internal OracleConnection _conexion;
        public ConnectionManager(string connectionString)
        {
            _conexion = new OracleConnection(connectionString);
        }
        public void Open()
        {
            _conexion.Open();
        }
        public void Close()
        {
            _conexion.Close();
        }
    }
}

