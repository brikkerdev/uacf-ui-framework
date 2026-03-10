using NUnit.Framework;
using UnityEngine;
using UACF.UI.Tokens;

namespace UACF.UI.Tests
{
    public class TokenResolutionTests
    {
        [Test]
        public void ColorPalette_GetColor_ReturnsSemanticColor()
        {
            var palette = ScriptableObject.CreateInstance<ColorPalette>();
            var color = palette.GetColor("primary");
            Assert.AreNotEqual(Color.magenta, color);
        }

        [Test]
        public void ColorPalette_TryGetColor_ReturnsTrueForValidKey()
        {
            var palette = ScriptableObject.CreateInstance<ColorPalette>();
            var success = palette.TryGetColor("primary", out var color);
            Assert.IsTrue(success);
        }

        [Test]
        public void SpacingScale_GetSpacing_ReturnsValue()
        {
            var scale = ScriptableObject.CreateInstance<SpacingScale>();
            var value = scale.GetSpacing("md");
            Assert.AreEqual(16f, value);
        }

        [Test]
        public void Theme_GetColor_ProxiesToPalette()
        {
            var theme = ScriptableObject.CreateInstance<Theme>();
            theme.colorPalette = ScriptableObject.CreateInstance<ColorPalette>();
            var color = theme.GetColor("primary");
            Assert.AreNotEqual(Color.magenta, color);
        }
    }
}
