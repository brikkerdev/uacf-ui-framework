using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDropdown : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMP_Text _selectedLabel;
        [SerializeField] private UnityEngine.UI.Image _arrow;
        [SerializeField] private RectTransform _dropdownPanel;
        [SerializeField] private string surfaceColorToken = "surface";
        [SerializeField] private List<string> options = new List<string>();
        [SerializeField] private int selectedIndex;

        public UnityEvent<int> onSelectionChanged;

        public override string ComponentType => "UIDropdown";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (_background != null)
                _background.color = theme.GetColor(surfaceColorToken);
        }

        public void SetOptions(List<string> opts) { options = opts; }
        public void SetSelected(int index) { selectedIndex = index; if (_selectedLabel != null && index >= 0 && index < options.Count) _selectedLabel.text = options[index]; }
        public void Open() { if (_dropdownPanel != null) _dropdownPanel.gameObject.SetActive(true); }
        public void Close() { if (_dropdownPanel != null) _dropdownPanel.gameObject.SetActive(false); }
    }
}
