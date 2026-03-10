using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Tokens;
using UACF.UI.Editor.Creation;
using UnityEngine;
using UnityEditor;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UISetupHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/setup/bootstrap", HandleBootstrap);
            router.Register("POST", "/api/ui/setup/prefabs", HandleCreatePrefabs);
            router.Register("POST", "/api/ui/setup/tokens", HandleCreateTokens);
        }

        private static async Task HandleBootstrap(RequestContext ctx)
        {
            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var created = new List<string>();

                // 1. Create token assets
                PrefabFactory.EnsureDirectory(PrefabFactory.TokensPath);
                PrefabFactory.EnsureDirectory(PrefabFactory.ThemesPath);

                var colorPath = $"{PrefabFactory.TokensPath}/DefaultColorPalette.asset";
                if (AssetDatabase.LoadAssetAtPath<ColorPalette>(colorPath) == null)
                {
                    var palette = ScriptableObject.CreateInstance<ColorPalette>();
                    AssetDatabase.CreateAsset(palette, colorPath);
                    created.Add(colorPath);
                }

                var typoPath = $"{PrefabFactory.TokensPath}/DefaultTypography.asset";
                if (AssetDatabase.LoadAssetAtPath<TypographySet>(typoPath) == null)
                {
                    var typo = ScriptableObject.CreateInstance<TypographySet>();
                    AssetDatabase.CreateAsset(typo, typoPath);
                    created.Add(typoPath);
                }

                var spacingPath = $"{PrefabFactory.TokensPath}/DefaultSpacing.asset";
                if (AssetDatabase.LoadAssetAtPath<SpacingScale>(spacingPath) == null)
                {
                    var spacing = ScriptableObject.CreateInstance<SpacingScale>();
                    AssetDatabase.CreateAsset(spacing, spacingPath);
                    created.Add(spacingPath);
                }

                var shapePath = $"{PrefabFactory.TokensPath}/DefaultShapes.asset";
                if (AssetDatabase.LoadAssetAtPath<ShapeSet>(shapePath) == null)
                {
                    var shapes = ScriptableObject.CreateInstance<ShapeSet>();
                    AssetDatabase.CreateAsset(shapes, shapePath);
                    created.Add(shapePath);
                }

                var elevationPath = $"{PrefabFactory.TokensPath}/DefaultElevation.asset";
                if (AssetDatabase.LoadAssetAtPath<ElevationSet>(elevationPath) == null)
                {
                    var elevation = ScriptableObject.CreateInstance<ElevationSet>();
                    AssetDatabase.CreateAsset(elevation, elevationPath);
                    created.Add(elevationPath);
                }

                // 2. Create theme
                var themePath = $"{PrefabFactory.ThemesPath}/DefaultTheme.asset";
                if (AssetDatabase.LoadAssetAtPath<Theme>(themePath) == null)
                {
                    var theme = ScriptableObject.CreateInstance<Theme>();
                    theme.themeName = "Default";
                    theme.themeId = "default";
                    theme.colorPalette = AssetDatabase.LoadAssetAtPath<ColorPalette>(colorPath);
                    theme.typography = AssetDatabase.LoadAssetAtPath<TypographySet>(typoPath);
                    theme.spacing = AssetDatabase.LoadAssetAtPath<SpacingScale>(spacingPath);
                    theme.shapes = AssetDatabase.LoadAssetAtPath<ShapeSet>(shapePath);
                    theme.elevations = AssetDatabase.LoadAssetAtPath<ElevationSet>(elevationPath);
                    AssetDatabase.CreateAsset(theme, themePath);
                    created.Add(themePath);
                }

                // 3. Create prefabs
                var prefabList = CreateAllPrefabs();
                created.AddRange(prefabList);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return new { success = true, created, total = created.Count };
            });

            ctx.RespondOk(result);
        }

        private static async Task HandleCreatePrefabs(RequestContext ctx)
        {
            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var created = CreateAllPrefabs();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return new { success = true, created, total = created.Count };
            });

            ctx.RespondOk(result);
        }

        private static async Task HandleCreateTokens(RequestContext ctx)
        {
            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var created = new List<string>();
                PrefabFactory.EnsureDirectory(PrefabFactory.TokensPath);
                PrefabFactory.EnsureDirectory(PrefabFactory.ThemesPath);

                var colorPath = $"{PrefabFactory.TokensPath}/DefaultColorPalette.asset";
                if (AssetDatabase.LoadAssetAtPath<ColorPalette>(colorPath) == null)
                {
                    var palette = ScriptableObject.CreateInstance<ColorPalette>();
                    AssetDatabase.CreateAsset(palette, colorPath);
                    created.Add(colorPath);
                }

                var typoPath = $"{PrefabFactory.TokensPath}/DefaultTypography.asset";
                if (AssetDatabase.LoadAssetAtPath<TypographySet>(typoPath) == null)
                {
                    var typo = ScriptableObject.CreateInstance<TypographySet>();
                    AssetDatabase.CreateAsset(typo, typoPath);
                    created.Add(typoPath);
                }

                var spacingPath = $"{PrefabFactory.TokensPath}/DefaultSpacing.asset";
                if (AssetDatabase.LoadAssetAtPath<SpacingScale>(spacingPath) == null)
                {
                    var spacing = ScriptableObject.CreateInstance<SpacingScale>();
                    AssetDatabase.CreateAsset(spacing, spacingPath);
                    created.Add(spacingPath);
                }

                var shapePath = $"{PrefabFactory.TokensPath}/DefaultShapes.asset";
                if (AssetDatabase.LoadAssetAtPath<ShapeSet>(shapePath) == null)
                {
                    var shapes = ScriptableObject.CreateInstance<ShapeSet>();
                    AssetDatabase.CreateAsset(shapes, shapePath);
                    created.Add(shapePath);
                }

                var elevationPath = $"{PrefabFactory.TokensPath}/DefaultElevation.asset";
                if (AssetDatabase.LoadAssetAtPath<ElevationSet>(elevationPath) == null)
                {
                    var elevation = ScriptableObject.CreateInstance<ElevationSet>();
                    AssetDatabase.CreateAsset(elevation, elevationPath);
                    created.Add(elevationPath);
                }

                var themePath = $"{PrefabFactory.ThemesPath}/DefaultTheme.asset";
                if (AssetDatabase.LoadAssetAtPath<Theme>(themePath) == null)
                {
                    var theme = ScriptableObject.CreateInstance<Theme>();
                    theme.themeName = "Default";
                    theme.themeId = "default";
                    theme.colorPalette = AssetDatabase.LoadAssetAtPath<ColorPalette>(colorPath);
                    theme.typography = AssetDatabase.LoadAssetAtPath<TypographySet>(typoPath);
                    theme.spacing = AssetDatabase.LoadAssetAtPath<SpacingScale>(spacingPath);
                    theme.shapes = AssetDatabase.LoadAssetAtPath<ShapeSet>(shapePath);
                    theme.elevations = AssetDatabase.LoadAssetAtPath<ElevationSet>(elevationPath);
                    AssetDatabase.CreateAsset(theme, themePath);
                    created.Add(themePath);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return new { success = true, created, total = created.Count };
            });

            ctx.RespondOk(result);
        }

        private static List<string> CreateAllPrefabs()
        {
            var created = new List<string>();

            string Save(string category, string name, GameObject prefab)
            {
                var path = PrefabFactory.SavePrefab(prefab, category, name);
                if (path != null) created.Add(path);
                return path;
            }

            Save("Display", "UIText", PrefabFactory.CreateUIText());
            Save("Display", "UIImage", PrefabFactory.CreateUIImage());
            Save("Display", "UIIcon", PrefabFactory.CreateUIIcon());
            Save("Display", "UIDivider", PrefabFactory.CreateUIDivider());
            Save("Display", "UIBadge", PrefabFactory.CreateUIBadge());

            Save("Input", "UIButton_Filled", PrefabFactory.CreateUIButton(Components.UIButtonVariant.Filled));
            Save("Input", "UIButton_Outlined", PrefabFactory.CreateUIButton(Components.UIButtonVariant.Outlined));
            Save("Input", "UIButton_Text", PrefabFactory.CreateUIButton(Components.UIButtonVariant.Text));
            Save("Input", "UIIconButton", PrefabFactory.CreateUIIconButton());
            Save("Input", "UIToggle", PrefabFactory.CreateUIToggle());
            Save("Input", "UISlider", PrefabFactory.CreateUISlider());
            Save("Input", "UIDropdown", PrefabFactory.CreateUIDropdown());
            Save("Input", "UIInputField", PrefabFactory.CreateUIInputField());
            Save("Input", "UICheckbox", PrefabFactory.CreateUICheckbox());

            Save("Layout", "UIVerticalLayout", PrefabFactory.CreateUIVerticalLayout());
            Save("Layout", "UIHorizontalLayout", PrefabFactory.CreateUIHorizontalLayout());
            Save("Layout", "UIGridLayout", PrefabFactory.CreateUIGridLayout());
            Save("Layout", "UISpacer", PrefabFactory.CreateUISpacer());

            Save("Containers", "UIPanel", PrefabFactory.CreateUIPanel());
            Save("Containers", "UICard", PrefabFactory.CreateUICard());
            Save("Containers", "UIScrollView", PrefabFactory.CreateUIScrollView());
            Save("Containers", "UIList", PrefabFactory.CreateUIList());
            Save("Containers", "UIListItem", PrefabFactory.CreateUIListItem());
            Save("Containers", "UITabContainer", PrefabFactory.CreateUITabContainer());

            Save("Navigation", "UIHeader", PrefabFactory.CreateUIHeader());
            Save("Navigation", "UIToolbar", PrefabFactory.CreateUIToolbar());
            Save("Navigation", "UIBottomBar", PrefabFactory.CreateUIBottomBar());
            Save("Navigation", "UITabBar", PrefabFactory.CreateUITabBar());
            Save("Navigation", "UISidebar", PrefabFactory.CreateUISidebar());

            Save("Overlay", "UIOverlay", PrefabFactory.CreateUIOverlay());
            Save("Overlay", "UIModal", PrefabFactory.CreateUIModal());
            Save("Overlay", "UIDialog", PrefabFactory.CreateUIDialog());
            Save("Overlay", "UIToast", PrefabFactory.CreateUIToast());
            Save("Overlay", "UITooltip", PrefabFactory.CreateUITooltip());

            Save("Feedback", "UIProgressBar", PrefabFactory.CreateUIProgressBar());
            Save("Feedback", "UISpinner", PrefabFactory.CreateUISpinner());
            Save("Feedback", "UIHealthBar", PrefabFactory.CreateUIHealthBar());

            return created;
        }
    }
}
