using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class AnimationsUtils
    {
        #region Curves

        public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
            => AnimationCurve.Linear(timeStart, valueStart, timeEnd, valueEnd);

        public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
            => AnimationCurve.EaseInOut(timeStart, valueStart, timeEnd, valueEnd);

        public static AnimationCurve Tick(float timeStart, float valueStart, float valueEnd, int ticksCount, float delay)
        {
            if (ticksCount <= 0 || delay <= 0)
            {
                Keyframe keyframe = new(timeStart, valueEnd);
                return new AnimationCurve(keyframe);
            }

            float variation = (valueEnd - valueStart) / ticksCount;

            Keyframe[] keys = new Keyframe[ticksCount + 2];
            keys[0] = new Keyframe(timeStart, valueStart, Mathf.Infinity, Mathf.Infinity); // Start
            keys[^1] = new Keyframe(timeStart + ticksCount * delay, valueEnd, Mathf.Infinity, Mathf.Infinity); // End

            for (int i = 1; i < keys.Length - 1; i++)
                keys[i] = new Keyframe(timeStart + i * delay, valueStart + i * variation, Mathf.Infinity, Mathf.Infinity);

            return new AnimationCurve(keys);
        }

        #endregion

        #region Animate

        public static IEnumerator Animate(Action<float[]> setValues, params AnimationCurve[] curves)
        {
            // Find end time
            float endTime = 0;

            foreach (var item in curves)
                endTime = Mathf.Max(item.keys[^1].time, endTime);

            float time = 0;
            float[] values = new float[curves.Length];

            while (time < endTime)
            {
                // Update values
                for (int i = 0; i < values.Length; i++)
                    values[i] = curves[i].Evaluate(time);

                // Set values
                setValues?.Invoke(values);

                // Wait 1 frame
                yield return null;
                time += Time.deltaTime;
            }

            // Update values
            for (int i = 0; i < values.Length; i++)
                values[i] = curves[i].Evaluate(time);

            // Set values
            setValues?.Invoke(values);
        }

        #endregion

        #region Translate

        public static IEnumerator TranslateLocal(this Transform transform, int ticksCount, float delay, Vector3 start, Vector3 end) => Animate(
            values => transform.localPosition = new Vector3(values[0], values[1], values[2]),
            Tick(0, start.x, end.x, ticksCount, delay),
            Tick(0, start.y, end.y, ticksCount, delay),
            Tick(0, start.z, end.z, ticksCount, delay)
        );

        #endregion

        #region Rotate

        public static IEnumerator RotateLocal(this Transform transform, int ticksCount, float delay, Vector3 start, Vector3 end) => Animate(
            values => transform.localRotation = Quaternion.Euler(values[0], values[1], values[2]),
            Tick(0, start.x, end.x, ticksCount, delay),
            Tick(0, start.y, end.y, ticksCount, delay),
            Tick(0, start.z, end.z, ticksCount, delay)
        );

        #endregion

        #region Scale

        public static IEnumerator ScaleLocal(this Transform transform, int ticksCount, float delay, Vector3 start, Vector3 end) => Animate(
            values => transform.localScale = new Vector3(values[0], values[1], values[2]),
            Tick(0, start.x, end.x, ticksCount, delay),
            Tick(0, start.y, end.y, ticksCount, delay),
            Tick(0, start.z, end.z, ticksCount, delay)
        );

        #endregion

        #region Fade

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

            setAlpha?.Invoke(start);
            behaviour.gameObject.SetActive(true);

            yield return Animate(
                values => setAlpha?.Invoke(values[0]),
                Tick(0, start, end, ticksCount, delay)
            );

            behaviour.gameObject.SetActive(end > 0);
            yield return null;
        }

        /// <summary>
        /// Fades in the given element
        /// </summary>
        public static IEnumerator FadeIn(this Behaviour behaviour, int ticksCount, float delay) => behaviour.Fade(ticksCount, delay, 0f, 1f);

        /// <summary>
        /// Fades out the given element
        /// </summary>
        public static IEnumerator FadeOut(this Behaviour behaviour, int ticksCount, float delay) => behaviour.Fade(ticksCount, delay, 1f, 0f);

        #endregion

        #region Graphic

        /// <summary>
        /// Sets the alpha value of this graphic
        /// </summary>
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        #endregion
    }
}