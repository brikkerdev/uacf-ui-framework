using UnityEngine;

namespace UACF.UI.Screens
{
    /// <summary>
    /// Lightweight marker for in-scene screens. Used by screen/show API to identify and toggle visibility.
    /// </summary>
    public class UIScreenMarker : MonoBehaviour
    {
        [SerializeField] private string screenId;

        public string ScreenId => screenId;

        public void SetScreenId(string id)
        {
            screenId = id ?? "";
        }
    }
}
