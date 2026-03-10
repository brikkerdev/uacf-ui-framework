using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public abstract class UIInteractableBase : UIComponentBase,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler
    {
        [SerializeField] private bool interactable = true;

        protected UIComponentState _currentState = UIComponentState.Normal;

        public bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable == value) return;
                interactable = value;
                _currentState = value ? UIComponentState.Normal : UIComponentState.Disabled;
                OnStateChanged?.Invoke(_currentState);
                ReapplyStyle();
            }
        }

        public override UIComponentState CurrentState => _currentState;

        public event Action<UIComponentState> OnStateChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            _currentState = interactable ? UIComponentState.Normal : UIComponentState.Disabled;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Hovered);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Normal);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Hovered);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Focused);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!interactable) return;
            SetState(UIComponentState.Normal);
        }

        protected void SetState(UIComponentState state)
        {
            if (_currentState == state) return;
            _currentState = state;
            OnStateChanged?.Invoke(state);
            ReapplyStyle();
        }
    }
}
