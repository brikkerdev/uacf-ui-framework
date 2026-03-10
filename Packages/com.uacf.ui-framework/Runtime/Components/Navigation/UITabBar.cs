using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [System.Serializable]
    public class TabItem
    {
        public string label;
        public string iconKey;
    }

    [RequireComponent(typeof(RectTransform))]
    public class UITabBar : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIHorizontalLayout _tabsLayout;
        [SerializeField] private UnityEngine.UI.Image _indicator;
        [SerializeField] private List<TabItem> tabs = new List<TabItem>();
        [SerializeField] private int selectedIndex;
        [SerializeField] private string indicatorColorToken = "primary";
        [SerializeField] private float indicatorHeight = 3f;

        public event Action<int> OnTabSelected;

        public override string ComponentType => "UITabBar";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _indicator != null)
                _indicator.color = theme.GetColor(indicatorColorToken);
        }

        public void SetTabs(List<TabItem> t) { tabs = t; }
        public void SelectTab(int index) { selectedIndex = index; OnTabSelected?.Invoke(index); }
    }
}
