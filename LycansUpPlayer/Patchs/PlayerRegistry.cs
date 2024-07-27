using System.Linq;

namespace LycansUpPlayer.Patchs
{
    public class PlayerRegistry
    {
        public static void Hook() {
            On.PlayerRegistry.GetAvailable += PlayerRegistry_GetAvailable;
        }

        private static bool PlayerRegistry_GetAvailable(On.PlayerRegistry.orig_GetAvailable orig, global::PlayerRegistry self, out byte index)
        {
            // If there are no players, we can use the first index
            if (self.ObjectByRef.Count == 0)
            {
                index = 0;
                return true;
            }

            // If server is full, we can't use any index
            if (self.ObjectByRef.Count == LycansUpPlayerPlugin.MAX_PLAYERS)
                {
                    index = 0;
                    return false;
                }

            // If server is not full, we can use the next available index
            byte[] array = (from kvp in self.ObjectByRef
                orderby kvp.Value.Index
                select kvp.Value.Index).ToArray<byte>();
                
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i + 1] > array[i] + 1)
                {
                    index = (byte)(array[i] + 1);
                    return true;
                }
            }
            index = (byte)(array[array.Length - 1] + 1);
            
            return true;
        }
    }

}