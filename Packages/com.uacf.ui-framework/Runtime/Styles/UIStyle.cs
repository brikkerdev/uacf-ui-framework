using UnityEngine;

namespace UACF.UI.Styles
{
    [CreateAssetMenu(menuName = "UACF UI/Style", fileName = "UIStyle")]
    public class UIStyle : ScriptableObject
    {
        public string styleKey = "default";
        public UIStyle parent;

        public UIStyleState normal = new UIStyleState();
        public UIStyleState hovered = new UIStyleState();
        public UIStyleState pressed = new UIStyleState();
        public UIStyleState disabled = new UIStyleState();
        public UIStyleState focused = new UIStyleState();
        public UIStyleState selected = new UIStyleState();

        public float transitionDuration = 0.15f;
        public AnimationCurve transitionCurve;

        public UIStyleState ResolveState(UIComponentState state)
        {
            var current = GetStateFor(state);
            var merged = new UIStyleState();

            if (parent != null)
            {
                var parentState = parent.ResolveState(state);
                merged = StyleResolver.MergeStates(current, parentState);
            }
            else
            {
                merged = current;
            }

            return merged;
        }

        public UIStyleState GetMergedNormal()
        {
            var current = normal;
            if (parent != null)
            {
                var parentNormal = parent.GetMergedNormal();
                current = StyleResolver.MergeStates(normal, parentNormal);
            }
            return current;
        }

        private UIStyleState GetStateFor(UIComponentState state)
        {
            return state switch
            {
                UIComponentState.Normal => normal,
                UIComponentState.Hovered => hovered,
                UIComponentState.Pressed => pressed,
                UIComponentState.Disabled => disabled,
                UIComponentState.Focused => focused,
                UIComponentState.Selected => selected,
                _ => normal
            };
        }
    }

    public enum UIComponentState
    {
        Normal,
        Hovered,
        Pressed,
        Disabled,
        Focused,
        Selected
    }
}
