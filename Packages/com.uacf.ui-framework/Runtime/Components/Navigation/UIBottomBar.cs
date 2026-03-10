using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [System.Serializable]
    public class BottomBarItem
    {
        public string iconKey;
        public string label;
        public string id;
    }

    [RequireComponent(typeof(RectTransform))]
    public class UIBottomBar : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIHorizontalLayout _itemsLayout;
        [SerializeField] private List<BottomBarItem> items = new List<BottomBarItem>();
        [SerializeField] private int selectedIndex;
        [SerializeField] private string backgroundColorToken = "surface";
        [SerializeField] private string selectedColorToken = "primary";
        [SerializeField] private string unselectedColorToken = "onBackgroundSecondary";
        [SerializeField] private string elevationToken = "high";

        public event Action<int> OnItemSelected;

        public override string ComponentType => "UIBottomBar";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = theme.GetColor(backgroundColorToken);
        }

        public void SetItems(List<BottomBarItem> i) { items = i; }
        public void SelectItem(int index) { selectedIndex = index; OnItemSelected?.Invoke(index); }
    }
}
