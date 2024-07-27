using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using LycansUpPlayer.Patchs;

namespace LycansUpPlayer
{
    public class LycansUpPlayerUI
    {
        private const string REGION_DEFAULT = "us";
        private const string SETTINGS_SERVER_REGION = "SETTINGS_SERVER_REGION";
        private const string MAIN_MENU_PATH = "GameUI/Canvas/MainMenu/LayoutGroup/Body/LayoutGroup/ActionsContainer/LayoutGroup/";
        private const string PLAY_MENU_PATH = "GameUI/Canvas/PlayMenu/LayoutGroup/Body/TaskPanel/Holder/LayoutGroup/";

        public static void Hook()
        {
            On.GameUI.ShowPlayMenu += OnShowPlayMenu;
            On.GameUI.ShowMainMenu += OnShowMainMenu;
            Log.Info("Hook method called and event handlers attached");
        }

        private static void OnShowMainMenu(On.GameUI.orig_ShowMainMenu orig, global::GameUI self, bool active)
        {
            Log.Info("OnShowMainMenu called");
            orig(self, active);

            Log.Info($"Max players set to {LycansUpPlayerPlugin.MAX_PLAYERS}");

            GameObject hostObj = GameObject.Find(MAIN_MENU_PATH + "HostButton");
            GameObject joinObj = GameObject.Find(MAIN_MENU_PATH + "JoinButton");

            if (hostObj == null || joinObj == null)
            {
                Log.Error("Host or Join button not found in Main Menu");
                return;
            }

            Log.Info("Host and Join buttons found");

            Button hostBtn = hostObj.GetComponent<Button>();
            Button joinBtn = joinObj.GetComponent<Button>();

            hostBtn.onClick.RemoveAllListeners();
            hostBtn.onClick.AddListener(() =>
            {
                Log.Info("Host button clicked");
                hostBtn.interactable = false;
                joinBtn.interactable = false;

                NetworkRunnerHandler instance = Object.FindFirstObjectByType<NetworkRunnerHandler>();
                if (instance == null)
                {
                    Log.Error("NetworkRunnerHandler instance not found");
                    return;
                }

                Log.Info("NetworkRunnerHandler instance found");

                string sessionName = RoomCode.Create(5);
                string region = PlayerPrefs.HasKey(SETTINGS_SERVER_REGION) ? PlayerPrefs.GetString(SETTINGS_SERVER_REGION) : REGION_DEFAULT;

                Log.Info($"Calling StartSession with sessionName: {sessionName}, region: {region}");
                instance.StartSession(GameMode.Host, sessionName, SceneManager.GetActiveScene().buildIndex, region, SteamAuth.Instance.InitToken(), LycansUpPlayerPlugin.MAX_PLAYERS, "Default", true);
            });

            On.GameUI.UpdatePlayerCount += (On.GameUI.orig_UpdatePlayerCount orig, global::GameUI self, int count) =>
            {
                Log.Info($"UpdatePlayerCount called with count: {count}");
                self.playerCount.text = $"{count}/{LycansUpPlayerPlugin.MAX_PLAYERS}";
            };
        }

        private static void OnShowPlayMenu(On.GameUI.orig_ShowPlayMenu orig, global::GameUI self)
        {
            Log.Info("OnShowPlayMenu called");
            orig(self);

            GameObject codeJoinObj = GameObject.Find(PLAY_MENU_PATH + "JoinButton");
            if (codeJoinObj == null)
            {
                Log.Error("Join button not found in Play Menu");
                return;
            }

            Log.Info("Join button found in Play Menu");

            Button codeJoinBtn = codeJoinObj.GetComponent<Button>();

            codeJoinBtn.onClick.RemoveAllListeners();
            codeJoinBtn.onClick.AddListener(() =>
            {
                TMP_InputField codeInput = GameObject.Find(PLAY_MENU_PATH + "CodeInput").GetComponent<TMP_InputField>();
                if (codeInput == null)
                {
                    Log.Error("CodeInput field not found");
                    return;
                }

                Log.Info("CodeInput field found");

                string sessionName = codeInput.text;
                if (string.IsNullOrWhiteSpace(sessionName)) return;

                Log.Info($"Joining session with sessionName: {sessionName}");
                codeJoinBtn.interactable = false;

                NetworkRunnerHandler instance = Object.FindFirstObjectByType<NetworkRunnerHandler>();
                if (instance == null)
                {
                    Log.Error("NetworkRunnerHandler instance not found");
                    return;
                }

                Log.Info("NetworkRunnerHandler instance found");

                string region = PlayerPrefs.HasKey(SETTINGS_SERVER_REGION) ? PlayerPrefs.GetString(SETTINGS_SERVER_REGION) : REGION_DEFAULT;
                Log.Info($"Region: {region}, Max Players: {LycansUpPlayerPlugin.MAX_PLAYERS}");

                instance.StartSession(GameMode.Client, sessionName, SceneManager.GetActiveScene().buildIndex, region, SteamAuth.Instance.InitToken(), LycansUpPlayerPlugin.MAX_PLAYERS, "Default", true);
            });
        }
    }
}
