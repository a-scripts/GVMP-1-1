using MySql.Data.MySqlClient;

namespace VMP_CNR.Module
{
    public abstract class Loadable<T> : Identifiable<T>
    {
        public Loadable(MySqlDataReader reader)
        {
        }

        public Loadable()
        {

        }

        public abstract T GetIdentifier();
    }
}