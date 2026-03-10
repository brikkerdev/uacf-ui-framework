using UnityEngine;

namespace UACF.UI.Animation
{
    [CreateAssetMenu(menuName = "UACF UI/Tween Preset", fileName = "TweenPreset")]
    public class TweenPreset : ScriptableObject
    {
        public float duration = 0.3f;
        public float delay;
        public AnimationCurve curve;
        public bool useUnscaledTime = true;
    }
}
