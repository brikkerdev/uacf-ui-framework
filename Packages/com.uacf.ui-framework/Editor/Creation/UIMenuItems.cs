using UnityEngine;
using UnityEditor;
using UACF.UI.Components;
using UACF.UI.Tokens;
using UACF.UI.Styles;

namespace UACF.UI.Editor.Creation
{
    public static class UIMenuItems
    {
        [MenuItem("GameObject/UACF UI/Display/Text", false, 10)]
        public static void CreateUIText() => CreateFromPrefab("UIText", "Display");

        [MenuItem("GameObject/UACF UI/Display/Image", false, 11)]
        public static void CreateUIImage() => CreateFromPrefab("UIImage", "Display");

        [MenuItem("GameObject/UACF UI/Display/Icon", false, 12)]
        public static void CreateUIIcon() => CreateFromPrefab("UIIcon", "Display");

        [MenuItem("GameObject/UACF UI/Display/Divider", false, 13)]
        public static void CreateUIDivider() => CreateFromPrefab("UIDivider", "Display");

        [MenuItem("GameObject/UACF UI/Display/Badge", false, 14)]
        public static void CreateUIBadge() => CreateFromPrefab("UIBadge", "Display");

        [MenuItem("GameObject/UACF UI/Input/Button (Filled)", false, 20)]
        public static void CreateUIButtonFilled() => CreateFromPrefab("UIButton_Filled", "Input");

        [MenuItem("GameObject/UACF UI/Input/Button (Outlined)", false, 21)]
        public static void CreateUIButtonOutlined() => CreateFromPrefab("UIButton_Outlined", "Input");

        [MenuItem("GameObject/UACF UI/Input/Button (Text)", false, 22)]
        public static void CreateUIButtonText() => CreateFromPrefab("UIButton_Text", "Input");

        [MenuItem("GameObject/UACF UI/Input/Icon Button", false, 23)]
        public static void CreateUIIconButton() => CreateFromPrefab("UIIconButton", "Input");

        [MenuItem("GameObject/UACF UI/Input/Toggle", false, 24)]
        public static void CreateUIToggle() => CreateFromPrefab("UIToggle", "Input");

        [MenuItem("GameObject/UACF UI/Input/Slider", false, 25)]
        public static void CreateUISlider() => CreateFromPrefab("UISlider", "Input");

        [MenuItem("GameObject/UACF UI/Input/Dropdown", false, 26)]
        public static void CreateUIDropdown() => CreateFromPrefab("UIDropdown", "Input");

        [MenuItem("GameObject/UACF UI/Input/Input Field", false, 27)]
        public static void CreateUIInputField() => CreateFromPrefab("UIInputField", "Input");

        [MenuItem("GameObject/UACF UI/Input/Checkbox", false, 28)]
        public static void CreateUICheckbox() => CreateFromPrefab("UICheckbox", "Input");

        [MenuItem("GameObject/UACF UI/Layout/Vertical Layout", false, 30)]
        public static void CreateUIVerticalLayout() => CreateFromPrefab("UIVerticalLayout", "Layout");

        [MenuItem("GameObject/UACF UI/Layout/Horizontal Layout", false, 31)]
        public static void CreateUIHorizontalLayout() => CreateFromPrefab("UIHorizontalLayout", "Layout");

        [MenuItem("GameObject/UACF UI/Layout/Grid Layout", false, 32)]
        public static void CreateUIGridLayout() => CreateFromPrefab("UIGridLayout", "Layout");

        [MenuItem("GameObject/UACF UI/Layout/Spacer", false, 33)]
        public static void CreateUISpacer() => CreateFromPrefab("UISpacer", "Layout");

        [MenuItem("GameObject/UACF UI/Container/Panel", false, 40)]
        public static void CreateUIPanel() => CreateFromPrefab("UIPanel", "Containers");

        [MenuItem("GameObject/UACF UI/Container/Card", false, 41)]
        public static void CreateUICard() => CreateFromPrefab("UICard", "Containers");

        [MenuItem("GameObject/UACF UI/Container/Scroll View", false, 42)]
        public static void CreateUIScrollView() => CreateFromPrefab("UIScrollView", "Containers");

        [MenuItem("Assets/Create/UACF UI/Theme", false, 100)]
        public static void CreateTheme() => CreateAsset<Theme>("Theme");

        [MenuItem("Assets/Create/UACF UI/Color Palette", false, 101)]
        public static void CreateColorPalette() => CreateAsset<ColorPalette>("ColorPalette");

        [MenuItem("Assets/Create/UACF UI/Typography Set", false, 102)]
        public static void CreateTypographySet() => CreateAsset<TypographySet>("TypographySet");

        [MenuItem("Assets/Create/UACF UI/Spacing Scale", false, 103)]
        public static void CreateSpacingScale() => CreateAsset<SpacingScale>("SpacingScale");

        [MenuItem("Assets/Create/UACF UI/Shape Set", false, 104)]
        public static void CreateShapeSet() => CreateAsset<ShapeSet>("ShapeSet");

        [MenuItem("Assets/Create/UACF UI/Elevation Set", false, 105)]
        public static void CreateElevationSet() => CreateAsset<ElevationSet>("ElevationSet");

        [MenuItem("Assets/Create/UACF UI/Style", false, 106)]
        public static void CreateUIStyle() => CreateAsset<UIStyle>("UIStyle");

        [MenuItem("Assets/Create/UACF UI/Style Sheet", false, 107)]
        public static void CreateStyleSheet() => CreateAsset<StyleSheet>("StyleSheet");

        [MenuItem("Assets/Create/UACF UI/Transition Preset", false, 108)]
        public static void CreateTransitionPreset() => CreateAsset<Screens.TransitionPreset>("TransitionPreset");

        [MenuItem("Assets/Create/UACF UI/Screen Registry", false, 109)]
        public static void CreateScreenRegistry() => CreateAsset<Screens.UIScreenRegistry>("ScreenRegistry");

        private static void CreateFromPrefab(string prefabName, string category)
        {
            var path = $"Assets/Resources/UACF_UI/Prefabs/{category}/{prefabName}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                path = $"Packages/com.uacf.ui-framework/Resources/UACF_UI/Prefabs/{category}/{prefabName}.prefab";
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            if (prefab != null)
            {
                var parent = Selection.activeTransform != null ? Selection.activeTransform : GetOrCreateCanvas();
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                Undo.RegisterCreatedObjectUndo(instance, "Create " + prefabName);
                Selection.activeGameObject = instance;
            }
            else
            {
                Debug.LogWarning($"[UACF UI] Prefab not found: {prefabName}. Create UI manually.");
            }
        }

        private static Transform GetOrCreateCanvas()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null) return canvas.transform;
            var go = new GameObject("Canvas");
            go.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<UnityEngine.UI.CanvasScaler>();
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return go.transform;
        }

        private static void CreateAsset<T>(string defaultName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            var path = EditorUtility.SaveFilePanelInProject("Create " + defaultName, defaultName, "asset", "");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                Selection.activeObject = asset;
            }
        }
    }
}
