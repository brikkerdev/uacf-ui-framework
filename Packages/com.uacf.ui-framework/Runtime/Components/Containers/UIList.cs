using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIList : UIComponentBase
    {
        [SerializeField] private UIScrollView _scrollView;
        [SerializeField] private UIVerticalLayout _layout;
        [SerializeField] private UIListItem _itemTemplate;
        [SerializeField] private bool showDividers = true;
        [SerializeField] private string dividerColorToken = "divider";

        public event Action<int> OnItemClicked;

        public override string ComponentType => "UIList";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _scrollView?.ReapplyStyle();
            _layout?.ReapplyStyle();
        }

        public void SetItems<T>(List<T> data, Action<UIListItem, T> binder)
        {
            // Implementation would instantiate items from template
        }

        public void AddItem(Action<UIListItem> configure) { }
        public void RemoveItem(int index) { }
        public void Clear() { }
        public int Count => 0;
    }
}
