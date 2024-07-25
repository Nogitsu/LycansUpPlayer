using BepInEx;
using UnityEngine;

namespace LycansUpPlayer
{

    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInProcess("Lycans.exe")]
    public class LycansUpPlayerPlugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = $"{PLUGIN_AUTHOR}.{PLUGIN_NAME}";
        public const string PLUGIN_AUTHOR = "NM & QM";
        public const string PLUGIN_NAME = "LycansUpPlayer";
        public const string PLUGIN_VERSION = "1.0.0";

        private void Awake()
        {
            Log.Init(Logger);
        }

        private void Update()
        {
        }
    }
}