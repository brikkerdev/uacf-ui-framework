using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;
using UACF.UI.Screens;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UITabContainer : UIComponentBase
    {
        [SerializeField] private UITabBar _tabBar;
        [SerializeField] private List<RectTransform> _tabPages = new List<RectTransform>();
        [SerializeField] private int selectedIndex;
        [SerializeField] private TransitionPreset _tabTransition;

        public event Action<int> OnTabChanged;

        public override string ComponentType => "UITabContainer";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _tabBar?.ReapplyStyle();
        }

        public void AddTab(string label, string iconKey, RectTransform content)
        {
            _tabPages.Add(content);
        }

        public void SelectTab(int index)
        {
            selectedIndex = index;
            OnTabChanged?.Invoke(index);
        }

        public void RemoveTab(int index)
        {
            _tabPages.RemoveAt(index);
        }
    }
}
