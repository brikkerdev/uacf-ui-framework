using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponentBase : MonoBehaviour, IThemeable
    {
        [SerializeField] private string componentId;
        [SerializeField] private StyleBinding styleBinding;

        private RectTransform _rectTransform;

        public RectTransform RectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
        public abstract string ComponentType { get; }
        public virtual UIComponentState CurrentState => UIComponentState.Normal;

        public string ComponentId
        {
            get
            {
                if (string.IsNullOrEmpty(componentId))
                    componentId = "ui_" + GetInstanceID();
                return componentId;
            }
            set => componentId = value;
        }

        public StyleBinding StyleBinding
        {
            get
            {
                if (styleBinding == null)
                    styleBinding = GetComponent<StyleBinding>();
                return styleBinding;
            }
        }

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (string.IsNullOrEmpty(componentId))
                componentId = "ui_" + GetInstanceID();
        }

        protected virtual void OnEnable()
        {
            ThemeManager.RegisterThemeable(this);
        }

        protected virtual void OnDisable()
        {
            ThemeManager.UnregisterThemeable(this);
        }

        public virtual void OnThemeChanged(Theme newTheme)
        {
            ReapplyStyle();
        }

        public abstract void ApplyStyle(ResolvedStyle style);

        public void ReapplyStyle()
        {
            if (StyleBinding != null)
                StyleBinding.ReapplyStyle();
        }

        public virtual Dictionary<string, object> Serialize()
        {
            return new Dictionary<string, object>
            {
                { "componentId", ComponentId },
                { "componentType", ComponentType }
            };
        }

        public virtual void Deserialize(Dictionary<string, object> data)
        {
            if (data.TryGetValue("componentId", out var id) && id is string s)
                componentId = s;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            StyleBinding?.ReapplyStyle();
        }
#endif
    }
}
