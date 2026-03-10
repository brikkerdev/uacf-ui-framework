using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Components;
using UACF.UI.Styles;

namespace UACF.UI.Editor.Creation
{
    public static class PrefabFactory
    {
        public const string ResourcesPath = "Assets/Resources/UACF_UI";
        public const string PrefabsPath = ResourcesPath + "/Prefabs";
        public const string TokensPath = ResourcesPath + "/Tokens";
        public const string ThemesPath = ResourcesPath + "/DefaultTheme";

        public static GameObject CreateUIText()
        {
            var go = CreateBase("UIText");
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 40);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "Text";
            tmp.fontSize = 16;

            var comp = go.AddComponent<UIText>();
            comp.SetText("Text");

            AddStyleBinding(go, "UIText");
            return go;
        }

        public static GameObject CreateUIImage()
        {
            var go = CreateBase("UIImage");
            go.AddComponent<Image>().color = Color.white;
            go.AddComponent<UIImage>();
            AddStyleBinding(go, "UIImage");
            return go;
        }

        public static GameObject CreateUIIcon()
        {
            var go = CreateBase("UIIcon");
            var img = go.AddComponent<Image>();
            img.color = Color.white;
            img.raycastTarget = false;
            go.AddComponent<UIIcon>();
            AddStyleBinding(go, "UIIcon");
            return go;
        }

        public static GameObject CreateUIDivider()
        {
            var go = CreateBase("UIDivider");
            var img = go.AddComponent<Image>();
            img.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            go.AddComponent<UIDivider>();
            AddStyleBinding(go, "UIDivider");
            return go;
        }

        public static GameObject CreateUIBadge()
        {
            var go = CreateBase("UIBadge");
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = labelRt.anchorMax = new Vector2(0.5f, 0.5f);
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "0";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 12;
            var badge = go.AddComponent<UIBadge>();
            SetPrivateField(badge, "_background", bg);
            SetPrivateField(badge, "_label", tmp);
            AddStyleBinding(go, "UIBadge");
            return go;
        }

        public static GameObject CreateUIButton(Components.UIButtonVariant variant)
        {
            var go = CreateBase("UIButton");
            var img = go.AddComponent<Image>();
            img.color = new Color(0.38f, 0f, 0.93f, 1f);
            img.raycastTarget = true;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(16, 16, 8, 8);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Button";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 14;

            var btn = go.AddComponent<UIButton>();
            btn.SetLabel("Button");
            SetPrivateField(btn, "variant", variant);
            SetPrivateField(btn, "_background", img);
            SetPrivateField(btn, "_label", tmp);

            var unityBtn = go.AddComponent<Button>();
            unityBtn.targetGraphic = img;

            AddStyleBinding(go, "UIButton");
            return go;
        }

        public static GameObject CreateUIIconButton()
        {
            var go = CreateBase("UIIconButton");
            var img = go.AddComponent<Image>();
            img.color = new Color(0.38f, 0f, 0.93f, 1f);
            go.AddComponent<UIIconButton>();
            go.AddComponent<Button>().targetGraphic = img;
            AddStyleBinding(go, "UIIconButton");
            return go;
        }

        public static GameObject CreateUIToggle()
        {
            var go = CreateBase("UIToggle");
            var track = go.AddComponent<Image>();
            track.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            var thumbGo = new GameObject("Thumb");
            thumbGo.transform.SetParent(go.transform, false);
            var thumb = thumbGo.AddComponent<Image>();
            thumb.color = Color.white;
            var toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = track;
            toggle.graphic = thumb;
            var uiToggle = go.AddComponent<UIToggle>();
            SetPrivateField(uiToggle, "_track", track);
            SetPrivateField(uiToggle, "_thumb", thumb);
            SetPrivateField(uiToggle, "_toggle", toggle);
            AddStyleBinding(go, "UIToggle");
            return go;
        }

        public static GameObject CreateUISlider()
        {
            var go = CreateBase("UISlider");
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            var fillRt = fillArea.AddComponent<RectTransform>();
            fillRt.anchorMin = new Vector2(0, 0.25f);
            fillRt.anchorMax = new Vector2(1, 0.75f);
            fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
            var fill = fillArea.AddComponent<Image>();
            fill.color = new Color(0.38f, 0f, 0.93f, 1f);
            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(go.transform, false);
            var handleGo = new GameObject("Handle");
            handleGo.transform.SetParent(handleArea.transform, false);
            handleGo.AddComponent<Image>().color = Color.white;
            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillRt;
            slider.handleRect = handleGo.GetComponent<RectTransform>();
            slider.direction = Slider.Direction.LeftToRight;
            var uiSlider = go.AddComponent<UISlider>();
            SetPrivateField(uiSlider, "_trackBackground", bg);
            SetPrivateField(uiSlider, "_trackFill", fill);
            SetPrivateField(uiSlider, "_thumb", handleGo.GetComponent<Image>());
            SetPrivateField(uiSlider, "_slider", slider);
            AddStyleBinding(go, "UISlider");
            return go;
        }

        public static GameObject CreateUIDropdown()
        {
            var go = CreateBase("UIDropdown");
            go.AddComponent<Image>().color = Color.white;
            go.AddComponent<UIDropdown>();
            AddStyleBinding(go, "UIDropdown");
            return go;
        }

        public static GameObject CreateUIInputField()
        {
            var go = CreateBase("UIInputField");
            var bg = go.AddComponent<Image>();
            bg.color = Color.white;
            var textArea = new GameObject("Text Area");
            textArea.transform.SetParent(go.transform, false);
            var textRt = textArea.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(12, 8);
            textRt.offsetMax = new Vector2(-12, -8);
            var text = textArea.AddComponent<TextMeshProUGUI>();
            text.fontSize = 16;
            var placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(textArea.transform, false);
            var phRt = placeholder.AddComponent<RectTransform>();
            phRt.anchorMin = phRt.anchorMax = Vector2.zero;
            phRt.offsetMin = Vector2.zero;
            phRt.offsetMax = textRt.rect.size;
            var phText = placeholder.AddComponent<TextMeshProUGUI>();
            phText.text = "Enter text...";
            phText.fontStyle = FontStyles.Italic;
            phText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            var inputField = go.AddComponent<TMP_InputField>();
            inputField.textViewport = textRt;
            inputField.textComponent = text;
            inputField.placeholder = phText;
            var uiInput = go.AddComponent<UIInputField>();
            SetPrivateField(uiInput, "_inputField", inputField);
            SetPrivateField(uiInput, "_background", bg);
            SetPrivateField(uiInput, "_inputText", text);
            SetPrivateField(uiInput, "_placeholder", phText);
            AddStyleBinding(go, "UIInputField");
            return go;
        }

        public static GameObject CreateUICheckbox()
        {
            var go = CreateBase("UICheckbox");
            var box = go.AddComponent<Image>();
            box.color = Color.white;
            var checkGo = new GameObject("Checkmark");
            checkGo.transform.SetParent(go.transform, false);
            var check = checkGo.AddComponent<Image>();
            check.color = new Color(0.38f, 0f, 0.93f, 1f);
            var toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            var uiCheck = go.AddComponent<UICheckbox>();
            SetPrivateField(uiCheck, "_box", box);
            SetPrivateField(uiCheck, "_checkmark", check);
            SetPrivateField(uiCheck, "_toggle", toggle);
            AddStyleBinding(go, "UICheckbox");
            return go;
        }

        public static GameObject CreateUIVerticalLayout()
        {
            var go = CreateBase("UIVerticalLayout");
            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 16;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childForceExpandWidth = true;
            go.AddComponent<UIVerticalLayout>();
            AddStyleBinding(go, "UIVerticalLayout");
            return go;
        }

        public static GameObject CreateUIHorizontalLayout()
        {
            var go = CreateBase("UIHorizontalLayout");
            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 16;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandHeight = true;
            go.AddComponent<UIHorizontalLayout>();
            AddStyleBinding(go, "UIHorizontalLayout");
            return go;
        }

        public static GameObject CreateUIGridLayout()
        {
            var go = CreateBase("UIGridLayout");
            var glg = go.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(100, 100);
            glg.spacing = new Vector2(16, 16);
            go.AddComponent<UIGridLayout>();
            AddStyleBinding(go, "UIGridLayout");
            return go;
        }

        public static GameObject CreateUISpacer()
        {
            var go = CreateBase("UISpacer");
            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = 16;
            le.preferredHeight = 16;
            go.AddComponent<UISpacer>();
            AddStyleBinding(go, "UISpacer");
            return go;
        }

        public static GameObject CreateUIPanel()
        {
            var go = CreateBase("UIPanel");
            var img = go.AddComponent<Image>();
            img.color = new Color(0.98f, 0.98f, 0.98f, 1f);
            var content = new GameObject("Content");
            content.transform.SetParent(go.transform, false);
            content.AddComponent<RectTransform>();
            go.AddComponent<UIPanel>();
            AddStyleBinding(go, "UIPanel");
            return go;
        }

        public static GameObject CreateUICard()
        {
            var go = CreateBase("UICard");
            go.AddComponent<Image>().color = new Color(0.98f, 0.98f, 0.98f, 1f);
            go.AddComponent<UICard>();
            AddStyleBinding(go, "UICard");
            return go;
        }

        public static GameObject CreateUIScrollView()
        {
            var go = CreateBase("UIScrollView");
            go.AddComponent<Image>().color = Color.clear;
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(go.transform, false);
            var vpRt = viewport.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = vpRt.offsetMax = Vector2.zero;
            viewport.AddComponent<Image>().color = Color.clear;
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            var contentRt = content.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = Vector2.one;
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.sizeDelta = new Vector2(0, 500);
            content.AddComponent<VerticalLayoutGroup>();
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scrollRect = go.AddComponent<ScrollRect>();
            scrollRect.content = contentRt;
            scrollRect.viewport = vpRt;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            var uiScroll = go.AddComponent<UIScrollView>();
            SetPrivateField(uiScroll, "_scrollRect", scrollRect);
            SetPrivateField(uiScroll, "_content", contentRt);
            SetPrivateField(uiScroll, "_background", go.GetComponent<Image>());
            AddStyleBinding(go, "UIScrollView");
            return go;
        }

        public static GameObject CreateUIList()
        {
            var go = CreateBase("UIList");
            go.AddComponent<UIList>();
            AddStyleBinding(go, "UIList");
            return go;
        }

        public static GameObject CreateUIListItem()
        {
            var go = CreateBase("UIListItem");
            go.AddComponent<Image>().color = Color.clear;
            go.AddComponent<UIListItem>();
            AddStyleBinding(go, "UIListItem");
            return go;
        }

        public static GameObject CreateUITabContainer()
        {
            var go = CreateBase("UITabContainer");
            go.AddComponent<UITabContainer>();
            AddStyleBinding(go, "UITabContainer");
            return go;
        }

        public static GameObject CreateUIHeader()
        {
            var go = CreateBase("UIHeader");
            go.AddComponent<Image>().color = new Color(0.98f, 0.98f, 0.98f, 1f);
            go.AddComponent<UIHeader>();
            AddStyleBinding(go, "UIHeader");
            return go;
        }

        public static GameObject CreateUIToolbar()
        {
            var go = CreateBase("UIToolbar");
            go.AddComponent<UIToolbar>();
            AddStyleBinding(go, "UIToolbar");
            return go;
        }

        public static GameObject CreateUIBottomBar()
        {
            var go = CreateBase("UIBottomBar");
            go.AddComponent<UIBottomBar>();
            AddStyleBinding(go, "UIBottomBar");
            return go;
        }

        public static GameObject CreateUITabBar()
        {
            var go = CreateBase("UITabBar");
            go.AddComponent<UITabBar>();
            AddStyleBinding(go, "UITabBar");
            return go;
        }

        public static GameObject CreateUISidebar()
        {
            var go = CreateBase("UISidebar");
            go.AddComponent<UISidebar>();
            AddStyleBinding(go, "UISidebar");
            return go;
        }

        public static GameObject CreateUIOverlay()
        {
            var go = CreateBase("UIOverlay");
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.5f);
            go.AddComponent<UIOverlay>();
            AddStyleBinding(go, "UIOverlay");
            return go;
        }

        public static GameObject CreateUIModal()
        {
            var go = CreateBase("UIModal");
            go.AddComponent<Image>().color = Color.white;
            go.AddComponent<UIModal>();
            AddStyleBinding(go, "UIModal");
            return go;
        }

        public static GameObject CreateUIDialog()
        {
            var go = CreateBase("UIDialog");
            go.AddComponent<UIDialog>();
            AddStyleBinding(go, "UIDialog");
            return go;
        }

        public static GameObject CreateUIToast()
        {
            var go = CreateBase("UIToast");
            go.AddComponent<UIToast>();
            AddStyleBinding(go, "UIToast");
            return go;
        }

        public static GameObject CreateUITooltip()
        {
            var go = CreateBase("UITooltip");
            go.AddComponent<UITooltip>();
            AddStyleBinding(go, "UITooltip");
            return go;
        }

        public static GameObject CreateUIProgressBar()
        {
            var go = CreateBase("UIProgressBar");
            var track = go.AddComponent<Image>();
            track.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(go.transform, false);
            var fillRt = fillGo.AddComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = new Vector2(0.5f, 1f);
            fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
            var fill = fillGo.AddComponent<Image>();
            fill.color = new Color(0.38f, 0f, 0.93f, 1f);
            fill.type = Image.Type.Filled;
            var bar = go.AddComponent<UIProgressBar>();
            SetPrivateField(bar, "_trackBackground", track);
            SetPrivateField(bar, "_fill", fill);
            AddStyleBinding(go, "UIProgressBar");
            return go;
        }

        public static GameObject CreateUISpinner()
        {
            var go = CreateBase("UISpinner");
            go.AddComponent<Image>().color = new Color(0.38f, 0f, 0.93f, 1f);
            go.AddComponent<UISpinner>();
            AddStyleBinding(go, "UISpinner");
            return go;
        }

        public static GameObject CreateUIHealthBar()
        {
            var go = CreateBase("UIHealthBar");
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var damageGo = new GameObject("Damage");
            damageGo.transform.SetParent(go.transform, false);
            var damageRt = damageGo.AddComponent<RectTransform>();
            damageRt.anchorMin = Vector2.zero;
            damageRt.anchorMax = Vector2.one;
            damageRt.offsetMin = damageRt.offsetMax = Vector2.zero;
            var damageImg = damageGo.AddComponent<Image>();
            damageImg.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            damageImg.type = Image.Type.Filled;
            var healthGo = new GameObject("Health");
            healthGo.transform.SetParent(go.transform, false);
            var healthRt = healthGo.AddComponent<RectTransform>();
            healthRt.anchorMin = Vector2.zero;
            healthRt.anchorMax = Vector2.one;
            healthRt.offsetMin = healthRt.offsetMax = Vector2.zero;
            var healthImg = healthGo.AddComponent<Image>();
            healthImg.color = new Color(0.2f, 0.7f, 0.3f, 1f);
            healthImg.type = Image.Type.Filled;
            var bar = go.AddComponent<UIHealthBar>();
            SetPrivateField(bar, "_backgroundBar", bg);
            SetPrivateField(bar, "_healthFill", healthImg);
            SetPrivateField(bar, "_damageFill", damageImg);
            AddStyleBinding(go, "UIHealthBar");
            return go;
        }

        private static GameObject CreateBase(string name)
        {
            var go = new GameObject(name);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(200, 40);
            return go;
        }

        private static void AddStyleBinding(GameObject go, string componentType)
        {
            var sb = go.AddComponent<StyleBinding>();
            sb.useThemeDefault = true;
        }

        public static string SavePrefab(GameObject prefab, string category, string prefabName)
        {
            EnsureDirectory(PrefabsPath + "/" + category);
            var path = $"{PrefabsPath}/{category}/{prefabName}.prefab";
            var saved = PrefabUtility.SaveAsPrefabAsset(prefab, path);
            Object.DestroyImmediate(prefab);
            return saved ? path : null;
        }

        public static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/UACF_UI"))
                AssetDatabase.CreateFolder("Assets/Resources", "UACF_UI");

            var parts = path.Replace("Assets/Resources/UACF_UI/", "").Replace("Assets/Resources/", "").Split('/');
            var current = "Assets/Resources/UACF_UI";
            foreach (var p in parts)
            {
                if (string.IsNullOrEmpty(p) || p.EndsWith(".prefab")) break;
                var next = current + "/" + p;
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, p);
                current = next;
            }
        }

        private static void SetPrivateField(object obj, string name, object value)
        {
            var field = obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
