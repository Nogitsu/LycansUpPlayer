namespace LycansUpPlayer.Patchs
{
    public class GameUI
    {
        public static void Patch()
        {
            On.GameUI.Awake += OnAwake;
        }

        private static void OnAwake(On.GameUI.orig_Awake orig, global::GameUI self)
        {
            orig(self);
        }
    }
}

