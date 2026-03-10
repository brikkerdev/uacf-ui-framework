using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Editor.Creation;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UnityEditor;
using UnityEngine;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIThemeHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("GET", "/api/ui/themes", HandleGetThemes);
            router.Register("POST", "/api/ui/theme/create", HandleCreateTheme);
            router.Register("PUT", "/api/ui/theme/apply", HandleApplyTheme);
            router.Register("GET", "/api/ui/theme/get", HandleGetTheme);
        }

        private static async Task HandleGetThemes(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue(() =>
            {
                List<Theme> themes;
                Theme active = null;
                if (UnityEngine.Application.isPlaying)
                {
                    themes = ThemeManager.GetAvailableThemes();
                    active = ThemeManager.ActiveTheme;
                }
                else
                {
                    themes = new List<Theme>();
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:UACF.UI.Tokens.Theme");
                    foreach (var g in guids)
                    {
                        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
                        var t = UnityEditor.AssetDatabase.LoadAssetAtPath<Theme>(path);
                        if (t != null) themes.Add(t);
                    }
                }
                var list = new System.Collections.Generic.List<object>();
                foreach (var t in themes)
                {
                    if (t == null) continue;
                    list.Add(new
                    {
                        id = t.themeId,
                        name = t.themeName,
                        path = UnityEditor.AssetDatabase.GetAssetPath(t),
                        is_active = t == active
                    });
                }
                return new { themes = list };
            });
            ctx.RespondOk(data);
        }

        private static async Task HandleCreateTheme(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var themeId = body.TryGetValue("theme_id", out var tid) ? tid?.ToString() ?? "new-theme" : "new-theme";
                var themeName = body.TryGetValue("theme_name", out var tn) ? tn?.ToString() ?? themeId : themeId;
                var paletteId = body.TryGetValue("palette_id", out var pid) ? pid?.ToString() : "default";
                var colors = body.TryGetValue("colors", out var c) && c is Dictionary<string, object> cd ? cd : null;

                var defaultTheme = AssetDatabase.LoadAssetAtPath<Theme>($"{PrefabFactory.ThemesPath}/DefaultTheme.asset");
                if (defaultTheme == null)
                    return new { success = false, error = "Default theme not found. Run bootstrap first." };

                ColorPalette palette = null;
                var assetsCreated = new List<string>();

                if (colors != null && colors.Count > 0)
                {
                    palette = ScriptableObject.CreateInstance<ColorPalette>();
                    var so = new SerializedObject(palette);
                    foreach (var kv in colors)
                    {
                        var key = kv.Key;
                        var color = ParseColor(kv.Value);
                        if (!color.HasValue) continue;
                        var prop = so.FindProperty(key);
                        if (prop != null)
                            prop.colorValue = color.Value;
                    }
                    so.ApplyModifiedPropertiesWithoutUndo();
                    var palettePath = $"{PrefabFactory.TokensPath}/{themeId}ColorPalette.asset";
                    PrefabFactory.EnsureDirectory(PrefabFactory.TokensPath);
                    AssetDatabase.CreateAsset(palette, palettePath);
                    assetsCreated.Add(palettePath);
                }
                else
                {
                    palette = string.Equals(paletteId, "default", System.StringComparison.OrdinalIgnoreCase)
                        ? AssetDatabase.LoadAssetAtPath<ColorPalette>($"{PrefabFactory.TokensPath}/DefaultColorPalette.asset")
                        : AssetDatabase.LoadAssetAtPath<ColorPalette>($"{PrefabFactory.TokensPath}/{paletteId}.asset");
                    if (palette == null)
                        return new { success = false, error = $"Palette not found: {paletteId}" };
                }

                var theme = ScriptableObject.CreateInstance<Theme>();
                theme.themeId = themeId;
                theme.themeName = themeName;
                theme.colorPalette = palette;
                theme.typography = defaultTheme.typography;
                theme.spacing = defaultTheme.spacing;
                theme.shapes = defaultTheme.shapes;
                theme.elevations = defaultTheme.elevations;
                theme.icons = defaultTheme.icons;

                var themePath = $"{PrefabFactory.ThemesPath}/{themeId}Theme.asset";
                PrefabFactory.EnsureDirectory(PrefabFactory.ThemesPath);
                AssetDatabase.CreateAsset(theme, themePath);
                assetsCreated.Add(themePath);

                AssetDatabase.SaveAssets();

                return new { success = true, theme_id = themeId, path = themePath, assets_created = assetsCreated };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        public static Task<object> ExecuteApplyTheme(Dictionary<string, object> body)
        {
            var themeId = body.TryGetValue("theme_id", out var tid) ? tid?.ToString() : null;
            if (string.IsNullOrEmpty(themeId))
                return System.Threading.Tasks.Task.FromResult<object>(new { success = false, error = "theme_id required" });

            return MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (UnityEngine.Application.isPlaying)
                    ThemeManager.SetTheme(themeId);
                return new { success = true, applied = true, theme_id = themeId };
            });
        }

        private static async Task HandleApplyTheme(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            var result = await ExecuteApplyTheme(body);
            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }
            ctx.RespondOk(result);
        }

        private static async Task HandleGetTheme(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                Theme theme = UnityEngine.Application.isPlaying ? ThemeManager.ActiveTheme : null;
                if (theme == null)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:UACF.UI.Tokens.Theme");
                    if (guids.Length > 0)
                        theme = UnityEditor.AssetDatabase.LoadAssetAtPath<Theme>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                if (theme == null) return new { theme_id = "", colors = new object(), typography = new object(), spacing = new object(), shapes = new object() };

                var colors = new Dictionary<string, object>();
                if (theme.colorPalette != null)
                {
                    foreach (var key in theme.colorPalette.GetAllColorKeys())
                    {
                        var c = theme.colorPalette.GetColor(key);
                        colors[key] = new { r = c.r, g = c.g, b = c.b, a = c.a };
                    }
                }

                return new
                {
                    theme_id = theme.themeId,
                    colors,
                    typography = new object(),
                    spacing = new object(),
                    shapes = new object()
                };
            });
            ctx.RespondOk(data);
        }

        private static Color? ParseColor(object obj)
        {
            if (obj is Dictionary<string, object> d)
            {
                if (!TryParseFloat(d.TryGetValue("r", out var r) ? r : null, out var rf)) rf = 0;
                if (!TryParseFloat(d.TryGetValue("g", out var g) ? g : null, out var gf)) gf = 0;
                if (!TryParseFloat(d.TryGetValue("b", out var b) ? b : null, out var bf)) bf = 0;
                if (!TryParseFloat(d.TryGetValue("a", out var a) ? a : null, out var af)) af = 1;
                return new Color(rf, gf, bf, af);
            }
            return null;
        }

        private static bool TryParseFloat(object obj, out float result)
        {
            result = 0;
            if (obj == null) return false;
            if (obj is double d) { result = (float)d; return true; }
            if (obj is float f) { result = f; return true; }
            if (obj is long l) { result = l; return true; }
            if (obj is int i) { result = i; return true; }
            return float.TryParse(obj?.ToString(), out result);
        }
    }
}
