using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace LycansUpPlayer.Patchs
{
    public class GameSettingsMenuPatch
    {

        public static void Hook()
        {
            RegisterTranslations();
            On.GameManager.Start += GameManager_Start;
        }

        private static void RegisterTranslations()
        {
            var localizationTableEn = LocalizationSettings.StringDatabase.GetTable(
                "UI Text",
             LocalizationSettings.AvailableLocales.GetLocale("en")
             );
            var localizationTableFr = LocalizationSettings.StringDatabase.GetTable(
                "UI Text",
                LocalizationSettings.AvailableLocales.GetLocale("fr")
            );


            localizationTableEn.AddEntry("LUP_LABEL", "Player Count Increase");
            localizationTableFr.AddEntry("LUP_LABEL", "Augmentation du nombre de joueurs");

            localizationTableEn.AddEntry("LUP_PLAYER_COUNT", "Player Count");
            localizationTableFr.AddEntry("LUP_PLAYER_COUNT", "Nombre de joueurs");

            LocalizationSettings.Instance.OnSelectedLocaleChanged += _ => RegisterTranslations();

        }


        private static void GameManager_Start(On.GameManager.orig_Start orig, GameManager self)
        {
            orig(self);
            Log.Info("GameManager_Start");

            var gameSettingsMenu = LycansUpPlayerPlugin.settings;
            var playerCount = gameSettingsMenu.getPlayerCount();
            AddSection(self.gameUI, "LUP_LABEL");

            AddSlider(self.gameUI, "LUP_PLAYER_COUNT",
                (value) =>
                {
                    gameSettingsMenu.setPlayerCount((int)value);
                    Log.Info($"Player count set to {value}");
                },
                playerCount,
                gameSettingsMenu.getDefaultPlayerCount()
            );

            MakeSettingsMenuScrollable(self.gameUI);
        }

        private static void AddSection(GameUI gameUi, string label)
        {
            var newSection = Object.Instantiate(
                gameUi.settingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/SettingsGroup/GraphicsTitle"),
                gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/LayoutGroup")
            );
            newSection.GetComponentInChildren<LocalizeStringEvent>().SetEntry(label);
        }

        private static void AddSlider(
            GameUI gameUi,
            string label,
            UnityAction<int> onValueChangedListener,
            int initialValue,
            int defaultValue
        )
        {
            var transform = CreateSettingsEntry(gameUi, "MasterVolumeSetting", label);
            var slider = transform.GetComponentInChildren<Slider>();
            slider.wholeNumbers = true; // Assure que le slider utilise des valeurs entières
            slider.minValue = 1;
            slider.maxValue = 15;
            slider.onValueChanged = new Slider.SliderEvent();
            slider.onValueChanged.AddListener(value => onValueChangedListener((int)value));
            On.GameSettingsUI.ResetSettings += (orig, self) =>
            {
                orig(self);
                slider.SetValueWithoutNotify(defaultValue);
                onValueChangedListener(defaultValue);
            };
            slider.SetValueWithoutNotify(initialValue);
        }

        private static void AddToggle(
            GameUI gameUi,
            string label,
            UnityAction<bool> onValueChangedListener,
            bool initialValue,
            bool defaultValue
        )
        {
            var transform = CreateSettingsEntry(gameUi, "VSyncSettings", label);
            var toggle = transform.GetComponentInChildren<Toggle>();
            toggle.onValueChanged = new Toggle.ToggleEvent();
            toggle.onValueChanged.AddListener(onValueChangedListener);
            On.GameSettingsUI.ResetSettings += (orig, self) =>
            {
                orig(self);
                toggle.SetIsOnWithoutNotify(defaultValue);
                onValueChangedListener(defaultValue);
            };
            toggle.SetIsOnWithoutNotify(initialValue);
        }

        private static Transform CreateSettingsEntry(GameUI gameUi, string originalEntry, string label)
        {
            var transform = Object.Instantiate(
                gameUi.settingsMenu.transform.Find($"LayoutGroup/Body/TaskPanel/Holder/SettingsGroup/{originalEntry}"),
                gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder/LayoutGroup")
            );
            transform.GetComponentInChildren<LocalizeStringEvent>().SetEntry(label);
            return transform;
        }

        private static void MakeSettingsMenuScrollable(GameUI gameUi)
        {
            var holder = gameUi.gameSettingsMenu.transform.Find("LayoutGroup/Body/TaskPanel/Holder");
            holder.gameObject.AddComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(-20, -10);
            holder.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            holder.Find("QuitButton").SetParent(holder.parent);

            var layoutGroup = holder.Find("LayoutGroup");

            var scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(holder);
            scrollView.transform.SetAsFirstSibling();

            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.content = layoutGroup.GetComponent<RectTransform>();
            scrollRect.viewport = holder.GetComponent<RectTransform>();

            layoutGroup.SetParent(scrollView.transform);
            layoutGroup.gameObject.AddComponent<ContentSizeFitter>();
            layoutGroup.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

            foreach (Transform child in layoutGroup)
            {
                child.GetComponent<LayoutElement>().minHeight = 40;
            }
        }
    }
}
