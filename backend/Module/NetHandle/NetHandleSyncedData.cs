using GTANetworkAPI;

namespace VMP_CNR
{
    public static class NetHandleSyncedData
    {   
        public static void RemoveEntityDataWhenExists(this Entity entity, string key)
        {
            if (entity.HasSharedData(key))
            {
                entity.ResetSharedData(key);
            }
        }
        
        public static void RemoveEntityDataWhenExists(this Player Player, string key)
        {
            if (Player.HasSharedData(key))
            {
                Player.ResetSharedData(key);
            }
        }
    }
}