using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

namespace LycansUpPlayer.Patchs
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
        }

        private static void OnShowMainMenu(On.GameUI.orig_ShowMainMenu orig, global::GameUI self, bool active)
        {
            Log.Info("OnShowMainMenu");
            orig(self, active);

            int maxPlayers = 20;

            GameObject hostObj = GameObject.Find(MAIN_MENU_PATH + "HostButton");
            GameObject joinObj = GameObject.Find(MAIN_MENU_PATH + "JoinButton");

            if (hostObj == null || joinObj == null)
            {
                Log.Error("Host or Join button not found in Main Menu");
                return;
            }

            Button hostBtn = hostObj.GetComponent<Button>();
            Button joinBtn = joinObj.GetComponent<Button>();

            hostBtn.onClick.RemoveAllListeners();
            hostBtn.onClick.AddListener(() =>
            {
                Log.Info("Starting host session...");
                hostBtn.interactable = false;
                joinBtn.interactable = false;

                NetworkRunnerHandler instance = Object.FindFirstObjectByType<NetworkRunnerHandler>();
                if (instance == null)
                {
                    Log.Error("NetworkRunnerHandler instance not found");
                    return;
                }

                string sessionName = RoomCode.Create(5);
                string region = PlayerPrefs.HasKey(SETTINGS_SERVER_REGION) ? PlayerPrefs.GetString(SETTINGS_SERVER_REGION) : REGION_DEFAULT;

                instance.StartSession(GameMode.Host, sessionName, SceneManager.GetActiveScene().buildIndex, region, SteamAuth.Instance.InitToken(), maxPlayers, "Default", true);
            });

            On.GameUI.UpdatePlayerCount += (On.GameUI.orig_UpdatePlayerCount orig, global::GameUI self, int count) =>
            {
                self.playerCount.text = $"{count}/{maxPlayers}";
            };
        }

        private static void OnShowPlayMenu(On.GameUI.orig_ShowPlayMenu orig, global::GameUI self)
        {
            Log.Info("OnShowPlayMenu");
            orig(self);

            GameObject codeJoinObj = GameObject.Find(PLAY_MENU_PATH + "JoinButton");
            if (codeJoinObj == null)
            {
                Log.Error("Join button not found in Play Menu");
                return;
            }

            Button codeJoinBtn = codeJoinObj.GetComponent<Button>();

            codeJoinBtn.onClick.RemoveAllListeners();
            codeJoinBtn.onClick.AddListener(() =>
            {
                TMP_InputField codeInput = GameObject.Find(MAIN_MENU_PATH + "CodeInput").GetComponent<TMP_InputField>();
                if (codeInput == null)
                {
                    Log.Error("CodeInput field not found");
                    return;
                }

                string sessionName = codeInput.text;
                if (string.IsNullOrWhiteSpace(sessionName)) return;

                Log.Info($"Joining session {sessionName}...");
                codeJoinBtn.interactable = false;

                NetworkRunnerHandler instance = Object.FindFirstObjectByType<NetworkRunnerHandler>();
                if (instance == null)
                {
                    Log.Error("NetworkRunnerHandler instance not found");
                    return;
                }

                string region = PlayerPrefs.HasKey(SETTINGS_SERVER_REGION) ? PlayerPrefs.GetString(SETTINGS_SERVER_REGION) : REGION_DEFAULT;

                instance.StartSession(GameMode.Client, sessionName, SceneManager.GetActiveScene().buildIndex, region, SteamAuth.Instance.InitToken(), 1, "Default", true);
            });
        }
    }
}
