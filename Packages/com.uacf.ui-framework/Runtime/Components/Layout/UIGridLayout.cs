using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIGridLayout : UIComponentBase
    {
        [SerializeField] private GridLayoutGroup _layoutGroup;
        [SerializeField] private ContentSizeFitter _sizeFitter;
        [SerializeField] private Vector2 cellSize = new Vector2(100, 100);
        [SerializeField] private string spacingToken = "md";
        [SerializeField] private Vector2 spacingOverride = new Vector2(-1, -1);
        [SerializeField] private string paddingToken = "none";
        [SerializeField] private RectOffset paddingOverride;
        [SerializeField] private GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft;
        [SerializeField] private GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Horizontal;
        [SerializeField] private TextAnchor childAlignment = TextAnchor.UpperLeft;
        [SerializeField] private GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.Flexible;
        [SerializeField] private int constraintCount;

        public override string ComponentType => "UIGridLayout";

        protected override void Awake()
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<GridLayoutGroup>();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<GridLayoutGroup>();
            if (_layoutGroup == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                var s = theme.GetSpacing(spacingToken);
                _layoutGroup.spacing = spacingOverride.x >= 0
                    ? spacingOverride
                    : new Vector2(s, s);
                _layoutGroup.padding = paddingOverride != null ? paddingOverride : theme.spacing.GetPadding(paddingToken);
            }
            _layoutGroup.cellSize = cellSize;
            _layoutGroup.startCorner = startCorner;
            _layoutGroup.startAxis = startAxis;
            _layoutGroup.childAlignment = childAlignment;
            _layoutGroup.constraint = constraint;
            _layoutGroup.constraintCount = constraintCount;
        }

        public void SetCellSize(Vector2 size) { cellSize = size; ReapplyStyle(); }
        public void SetColumns(int count) { constraint = GridLayoutGroup.Constraint.FixedColumnCount; constraintCount = count; ReapplyStyle(); }
        public void SetRows(int count) { constraint = GridLayoutGroup.Constraint.FixedRowCount; constraintCount = count; ReapplyStyle(); }
    }
}
