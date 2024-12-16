using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UtilsModule
{
    public static class AnimationsUtils
    {
        /// <summary>
        /// Fades the given element
        /// </summary>
        public static IEnumerator Fade(this Behaviour behaviour, int ticksCount, float delay, float start, float end)
        {
            start = Mathf.Clamp01(start);
            end = Mathf.Clamp01(end);

            Action<float> setAlpha = null;

            if (behaviour is Graphic graphic)
                setAlpha = (a) => graphic.SetAlpha(a);
            else if (behaviour is CanvasGroup canvasGroup)
                setAlpha = (a) => canvasGroup.alpha = a;

            float valuePerTick = (end - start) / ticksCount;
            setAlpha?.Invoke(start);
            behaviour.gameObject.SetActive(true);

            for (int i = 0; i < ticksCount; i++)
            {
                setAlpha?.Invoke(start + valuePerTick * (i + 1));
                yield return new WaitForSeconds(delay);
            }

            behaviour.gameObject.SetActive(end > 0);
        }

        public static IEnumerator FadeIn(this Behaviour behaviour, int ticksCount, float delay) => behaviour.Fade(ticksCount, delay, 0f, 1f);
        public static IEnumerator FadeOut(this Behaviour behaviour, int ticksCount, float delay) => behaviour.Fade(ticksCount, delay, 1f, 0f);

        #region Graphic

        /// <summary>
        /// Sets the alpha value of this graphic
        /// </summary>
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        #endregion
    }
}