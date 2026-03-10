using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Tokens;
using UACF.UI.Theming;

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
            ctx.RespondOk(new { theme_id = "new-theme", path = "Assets/Themes/NewTheme.asset", assets_created = new string[0] });
        }

        private static async Task HandleApplyTheme(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            var themeId = body.TryGetValue("theme_id", out var tid) ? tid?.ToString() : null;
            if (string.IsNullOrEmpty(themeId))
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "theme_id required");
                return;
            }
            await MainThreadDispatcher.Enqueue(() =>
            {
                if (UnityEngine.Application.isPlaying)
                    ThemeManager.SetTheme(themeId);
            });
            ctx.RespondOk(new { applied = true });
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
    }
}
