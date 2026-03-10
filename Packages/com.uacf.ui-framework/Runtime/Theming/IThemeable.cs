using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Styles;

namespace UACF.UI.Theming
{
    public interface IThemeable
    {
        void OnThemeChanged(Theme newTheme);
        void ApplyStyle(ResolvedStyle style);
        string ComponentType { get; }
    }
}
