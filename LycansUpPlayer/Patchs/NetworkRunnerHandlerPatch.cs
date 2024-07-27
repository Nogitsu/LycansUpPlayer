using HarmonyLib;
using Fusion;
using Fusion.Photon.Realtime;
using UnityEngine;
using System.Collections;

namespace LycansUpPlayer.Patchs
{
    [HarmonyPatch(typeof(NetworkRunnerHandler))]
    public class NetworkRunnerHandlerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StartSession")]
        public static bool StartSessionPrefix(NetworkRunnerHandler __instance, GameMode mode, string sessionName, SceneRef scene, string region, AuthenticationValues authenticationValues, int playerCount, string customLobbyName, bool disableClientSessionCreation)
        {
            Debug.Log("StartSessionPrefix called with sessionName: " + sessionName);

            // Exécution de la méthode patchée pour modifier le comportement
            __instance.StartCoroutine(WaitAndStartNewSession(__instance, mode, sessionName, scene, region, authenticationValues, playerCount, customLobbyName, disableClientSessionCreation));

            return false; // Empêcher l'exécution de la méthode originale
        }

        private static IEnumerator WaitAndStartNewSession(NetworkRunnerHandler instance, GameMode mode, string sessionName, SceneRef scene, string region, AuthenticationValues authenticationValues, int playerCount, string customLobbyName, bool disableClientSessionCreation)
        {
            // Assurez-vous que la méthode ShutdownAll fonctionne comme prévu
            instance.ShutdownAll();
            yield return new WaitForSeconds(2); // Ajustez le délai si nécessaire

            // Assurez-vous que BuildCustomAppSetting est bien appelée
            AppSettings appSettings = BuildCustomAppSetting(instance, region);

            Debug.Log($"Region set to: {appSettings.FixedRegion}");

            // Démarrer la nouvelle session
            var result =  instance._networkRunner.StartGame(new StartGameArgs
            {
                CustomPhotonAppSettings = appSettings,
                GameMode = mode,
                IsVisible = true,
                IsOpen = true,
                SceneManager = instance.GetSceneManager(instance._networkRunner),
                Scene = scene,
                CustomLobbyName = customLobbyName,
                SessionName = sessionName,
                PlayerCount = playerCount,
                DisableClientSessionCreation = disableClientSessionCreation,
                AuthValues = authenticationValues
            });

            Debug.Log($"StartGame result: {result}");
        }



        private static AppSettings BuildCustomAppSetting(NetworkRunnerHandler instance, string region)
        {
            AppSettings copy = PhotonAppSettings.Instance.AppSettings.GetCopy();
            if (!string.IsNullOrEmpty(region))
            {
                copy.FixedRegion = region.ToLower(); // Assurez-vous que la région est bien configurée
                Debug.Log($"Region set to: {copy.FixedRegion}");
            }
            else
            {
                Debug.LogWarning("Region is null or empty, using default region");
                copy.FixedRegion = "us"; // Définir une région par défaut si la région est vide
            }
            return copy;
        }

    }
}
