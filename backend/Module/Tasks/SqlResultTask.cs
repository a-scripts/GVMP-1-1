using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using GTANetworkAPI;
namespace VMP_CNR.Module.Tasks
{
    public abstract class SqlResultTask : SynchronizedTask
    {
        public override void Execute()
        {
            NAPI.Task.Run(() =>
            {
                using (var connection = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = GetQuery();
                        using (var reader = command.ExecuteReader())
                        {
                            OnFinished(reader);
                        }
                    }
                }
            });
        }

        public abstract string GetQuery();

        public abstract void OnFinished(MySqlDataReader reader);
    }
}