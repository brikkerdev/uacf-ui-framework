using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Tokens;
using UnityEditor;
using UACF.UI.Editor.Creation;
using UnityEngine;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UITokenHandler
    {
        private const string DefaultPalettePath = "Assets/Resources/UACF_UI/Tokens/DefaultColorPalette.asset";
        private const string DefaultTypographyPath = "Assets/Resources/UACF_UI/Tokens/DefaultTypography.asset";
        private const string DefaultSpacingPath = "Assets/Resources/UACF_UI/Tokens/DefaultSpacing.asset";

        private static readonly HashSet<string> ColorPaletteKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "primary", "primaryVariant", "secondary", "secondaryVariant", "accent",
            "background", "surface", "surfaceVariant", "error", "warning", "success", "info",
            "onPrimary", "onSecondary", "onBackground", "onBackgroundSecondary", "onSurface", "onError",
            "divider", "disabled", "overlay", "shadow"
        };

        private static readonly HashSet<string> TypographyPresetKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "h1", "h2", "h3", "h4", "subtitle1", "subtitle2", "body1", "body2", "caption", "overline", "button"
        };

        private static readonly HashSet<string> SpacingKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "xxs", "xs", "sm", "md", "lg", "xl", "xxl", "xxxl"
        };

        public static void Register(RequestRouter router)
        {
            router.Register("PUT", "/api/ui/tokens/colors", HandleUpdateColors);
            router.Register("POST", "/api/ui/tokens/colors/add-custom", HandleAddCustomColor);
            router.Register("PUT", "/api/ui/tokens/typography", HandleUpdateTypography);
            router.Register("PUT", "/api/ui/tokens/spacing", HandleUpdateSpacing);
        }

        private static async Task HandleUpdateColors(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var paletteId = body.TryGetValue("palette_id", out var pid) ? pid?.ToString() : "default";
                var palettePath = string.Equals(paletteId, "default", StringComparison.OrdinalIgnoreCase)
                    ? DefaultPalettePath
                    : $"{PrefabFactory.TokensPath}/{paletteId}.asset";

                var palette = AssetDatabase.LoadAssetAtPath<ColorPalette>(palettePath);
                if (palette == null)
                    return new { success = false, error = $"ColorPalette not found: {palettePath}. Run bootstrap first." };

                if (!body.TryGetValue("colors", out var colorsObj) || colorsObj is not Dictionary<string, object> colors)
                    return new { success = false, error = "colors object required" };

                var updated = new List<string>();
                var so = new SerializedObject(palette);

                foreach (var kv in colors)
                {
                    var key = kv.Key;
                    if (!ColorPaletteKeys.Contains(key))
                        continue;

                    var color = ParseColor(kv.Value);
                    if (!color.HasValue)
                        continue;

                    var prop = so.FindProperty(key);
                    if (prop != null)
                    {
                        prop.colorValue = color.Value;
                        updated.Add(key);
                    }
                }

                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(palette);
                AssetDatabase.SaveAssets();

                return new { success = true, updated, theme_reapplied = updated.Count > 0 };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleAddCustomColor(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var key = body.TryGetValue("key", out var k) ? k?.ToString() : null;
                if (string.IsNullOrEmpty(key))
                    return new { success = false, error = "key required" };

                if (!body.TryGetValue("color", out var colorObj))
                    return new { success = false, error = "color required" };

                var color = ParseColor(colorObj);
                if (!color.HasValue)
                    return new { success = false, error = "Invalid color format. Use {r,g,b,a}" };

                var palette = AssetDatabase.LoadAssetAtPath<ColorPalette>(DefaultPalettePath);
                if (palette == null)
                    return new { success = false, error = "ColorPalette not found. Run bootstrap first." };

                palette.SetColor(key, color.Value);
                EditorUtility.SetDirty(palette);
                AssetDatabase.SaveAssets();

                return new { success = true, added = true, key };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleUpdateTypography(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var presetId = body.TryGetValue("preset_id", out var pid) ? pid?.ToString() : "default";
                var typoPath = string.Equals(presetId, "default", StringComparison.OrdinalIgnoreCase)
                    ? DefaultTypographyPath
                    : $"{PrefabFactory.TokensPath}/{presetId}.asset";

                var typo = AssetDatabase.LoadAssetAtPath<TypographySet>(typoPath);
                if (typo == null)
                    return new { success = false, error = $"TypographySet not found: {typoPath}. Run bootstrap first." };

                if (!body.TryGetValue("presets", out var presetsObj) || presetsObj is not Dictionary<string, object> presets)
                    return new { success = false, error = "presets object required" };

                var updated = new List<string>();
                var so = new SerializedObject(typo);

                foreach (var kv in presets)
                {
                    var key = kv.Key;
                    if (!TypographyPresetKeys.Contains(key))
                        continue;

                    if (kv.Value is not Dictionary<string, object> presetDict)
                        continue;

                    var presetProp = so.FindProperty(key);
                    if (presetProp == null)
                        continue;

                    if (presetDict.TryGetValue("fontSize", out var fsObj) && TryParseFloat(fsObj, out var fontSize))
                    {
                        var fsProp = presetProp.FindPropertyRelative("fontSize");
                        if (fsProp != null) { fsProp.floatValue = fontSize; updated.Add($"{key}.fontSize"); }
                    }

                    if (presetDict.TryGetValue("fontStyle", out var styleObj))
                    {
                        var styleStr = styleObj?.ToString() ?? "";
                        var fontStyle = styleStr.ToLowerInvariant() switch
                        {
                            "bold" => TMPro.FontStyles.Bold,
                            "italic" => TMPro.FontStyles.Italic,
                            "bolditalic" or "bold italic" => TMPro.FontStyles.Bold | TMPro.FontStyles.Italic,
                            _ => TMPro.FontStyles.Normal
                        };
                        var styleProp = presetProp.FindPropertyRelative("fontStyle");
                        if (styleProp != null) { styleProp.intValue = (int)fontStyle; updated.Add($"{key}.fontStyle"); }
                    }
                }

                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(typo);
                AssetDatabase.SaveAssets();

                return new { success = true, updated };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleUpdateSpacing(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var scaleId = body.TryGetValue("scale_id", out var sid) ? sid?.ToString() : "default";
                var spacingPath = string.Equals(scaleId, "default", StringComparison.OrdinalIgnoreCase)
                    ? DefaultSpacingPath
                    : $"{PrefabFactory.TokensPath}/{scaleId}.asset";

                var scale = AssetDatabase.LoadAssetAtPath<SpacingScale>(spacingPath);
                if (scale == null)
                    return new { success = false, error = $"SpacingScale not found: {spacingPath}. Run bootstrap first." };

                if (!body.TryGetValue("values", out var valuesObj) || valuesObj is not Dictionary<string, object> values)
                    return new { success = false, error = "values object required" };

                var updated = new List<string>();
                var so = new SerializedObject(scale);

                foreach (var kv in values)
                {
                    var key = kv.Key;
                    if (!SpacingKeys.Contains(key))
                        continue;

                    if (!TryParseFloat(kv.Value, out var val))
                        continue;

                    var prop = so.FindProperty(key);
                    if (prop != null)
                    {
                        prop.floatValue = val;
                        updated.Add(key);
                    }
                }

                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(scale);
                AssetDatabase.SaveAssets();

                return new { success = true, updated };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
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
            if (float.TryParse(obj.ToString(), out result)) return true;
            return false;
        }
    }
}
