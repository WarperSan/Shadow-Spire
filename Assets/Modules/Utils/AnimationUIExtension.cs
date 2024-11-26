using DhafinFawwaz.AnimationUILib;
using System.Collections;

namespace UtilsModule
{
    public static class AnimationUIExtension
    {
        public static IEnumerator PlayAnimation(this AnimationUI animation)
        {
            bool hasAnimationEnded = false;
            void callback() => hasAnimationEnded = true;
            
            animation.OnAnimationEnded += callback;
            animation.Play();

            while (!hasAnimationEnded)
                yield return null;

            animation.OnAnimationEnded -= callback;
        }
    }
}