using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIHealthBar : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _backgroundBar;
        [SerializeField] private UnityEngine.UI.Image _healthFill;
        [SerializeField] private UnityEngine.UI.Image _damageFill;
        [SerializeField] private UIText _label;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private string healthColorToken = "success";
        [SerializeField] private string damageColorToken = "error";
        [SerializeField] private string lowHealthColorToken = "warning";
        [SerializeField] private float lowHealthThreshold = 0.25f;
        [SerializeField] private bool showLabel = true;
        [SerializeField] private string labelFormat = "{0}/{1}";
        [SerializeField] private float damageAnimDuration = 0.5f;

        public override string ComponentType => "UIHealthBar";

        private void Update()
        {
            if (_healthFill != null) _healthFill.fillAmount = currentHealth / maxHealth;
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                if (_healthFill != null) _healthFill.color = currentHealth / maxHealth < lowHealthThreshold ? theme.GetColor(lowHealthColorToken) : theme.GetColor(healthColorToken);
                if (_damageFill != null) _damageFill.color = theme.GetColor(damageColorToken);
            }
        }

        public void SetHealth(float current, float max) { currentHealth = current; maxHealth = max; }
        public void TakeDamage(float amount) { currentHealth = Mathf.Max(0, currentHealth - amount); }
        public void Heal(float amount) { currentHealth = Mathf.Min(maxHealth, currentHealth + amount); }
    }
}
