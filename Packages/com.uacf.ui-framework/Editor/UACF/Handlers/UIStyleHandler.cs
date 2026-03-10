using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Editor.Creation;
using UACF.UI.Styles;
using UnityEngine;
using UnityEditor;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIStyleHandler
    {
        private const string StylesPath = "Assets/Resources/UACF_UI/Styles";

        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/style/create", HandleCreateStyle);
            router.Register("PUT", "/api/ui/style/apply", HandleApplyStyle);
            router.Register("GET", "/api/ui/style/list", HandleListStyles);
        }

        private static async Task HandleCreateStyle(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var key = body.TryGetValue("key", out var k) ? k?.ToString() ?? "new_style" : "new_style";
                if (string.IsNullOrWhiteSpace(key))
                    return new { success = false, error = "key required" };

                var style = ScriptableObject.CreateInstance<UIStyle>();
                style.styleKey = key;

                if (body.TryGetValue("parent", out var parentObj) && parentObj != null)
                {
                    var parentKey = parentObj.ToString();
                    var parentStyle = LoadStyleByKeyOrPath(parentKey);
                    if (parentStyle != null)
                        style.parent = parentStyle;
                }

                ApplyStateFromDict(style.normal, body, "normal");
                ApplyStateFromDict(style.hovered, body, "hovered");
                ApplyStateFromDict(style.pressed, body, "pressed");
                ApplyStateFromDict(style.disabled, body, "disabled");

                PrefabFactory.EnsureDirectory(StylesPath);
                var assetPath = $"{StylesPath}/{key}Style.asset";
                AssetDatabase.CreateAsset(style, assetPath);
                AssetDatabase.SaveAssets();

                return new { success = true, key, path = assetPath };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static void ApplyStateFromDict(UIStyleState state, Dictionary<string, object> body, string prefix)
        {
            if (!body.TryGetValue(prefix, out var stateObj) || stateObj is not Dictionary<string, object> stateDict)
                return;

            if (stateDict.TryGetValue("backgroundColorToken", out var v) && v != null)
            {
                state.backgroundColorToken = new OptionalToken<string>(v.ToString());
            }
            if (stateDict.TryGetValue("textColorToken", out var vt) && vt != null)
            {
                state.textColorToken = new OptionalToken<string>(vt.ToString());
            }
            if (stateDict.TryGetValue("typographyToken", out var vty) && vty != null)
            {
                state.typographyToken = new OptionalToken<string>(vty.ToString());
            }
            if (stateDict.TryGetValue("shapeToken", out var vs) && vs != null)
            {
                state.shapeToken = new OptionalToken<string>(vs.ToString());
            }
            if (stateDict.TryGetValue("elevationToken", out var ve) && ve != null)
            {
                state.elevationToken = new OptionalToken<string>(ve.ToString());
            }
            if (stateDict.TryGetValue("borderColorToken", out var vb) && vb != null)
            {
                state.borderColorToken = new OptionalToken<string>(vb.ToString());
            }
            if (stateDict.TryGetValue("paddingToken", out var vp) && vp != null)
            {
                state.paddingToken = new OptionalToken<string>(vp.ToString());
            }
        }

        private static async Task HandleApplyStyle(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (!body.TryGetValue("target", out var targetObj) || targetObj is not Dictionary<string, object> target)
                    return new { success = false, error = "target required" };

                if (!body.TryGetValue("style", out var styleObj) || styleObj == null)
                    return new { success = false, error = "style required (styleKey or asset path)" };

                var go = UIElementHandler.ResolveTarget(target);
                if (go == null)
                    return new { success = false, error = "Target not found" };

                var styleBinding = go.GetComponent<StyleBinding>();
                if (styleBinding == null)
                    styleBinding = go.AddComponent<StyleBinding>();

                var styleKeyOrPath = styleObj.ToString();
                var loadedStyle = LoadStyleByKeyOrPath(styleKeyOrPath);
                if (loadedStyle == null)
                    return new { success = false, error = $"Style not found: {styleKeyOrPath}" };

                styleBinding.style = loadedStyle;
                styleBinding.useThemeDefault = false;
                styleBinding.ReapplyStyle();

                EditorUtility.SetDirty(go);
                return new { success = true, applied = true, style_key = loadedStyle.styleKey };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static UIStyle LoadStyleByKeyOrPath(string keyOrPath)
        {
            if (string.IsNullOrEmpty(keyOrPath)) return null;

            if (keyOrPath.Contains("/") || keyOrPath.EndsWith(".asset"))
            {
                return AssetDatabase.LoadAssetAtPath<UIStyle>(keyOrPath);
            }

            var guids = AssetDatabase.FindAssets($"t:{nameof(UIStyle)} {keyOrPath}");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var style = AssetDatabase.LoadAssetAtPath<UIStyle>(path);
                if (style != null && style.styleKey == keyOrPath)
                    return style;
            }

            guids = AssetDatabase.FindAssets("t:UIStyle");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var style = AssetDatabase.LoadAssetAtPath<UIStyle>(path);
                if (style != null && style.styleKey == keyOrPath)
                    return style;
            }

            return null;
        }

        private static async Task HandleListStyles(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var guids = AssetDatabase.FindAssets("t:UIStyle");
                var styles = new List<object>();

                foreach (var g in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(g);
                    var style = AssetDatabase.LoadAssetAtPath<UIStyle>(path);
                    if (style == null) continue;

                    styles.Add(new
                    {
                        key = style.styleKey,
                        path,
                        has_parent = style.parent != null,
                        parent_key = style.parent != null ? style.parent.styleKey : (string)null
                    });
                }

                return new { styles };
            });
            ctx.RespondOk(data);
        }
    }
}
