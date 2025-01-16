using UnityEngine;

namespace Utils
{
    public static class LayoutUtils
    {
        private static void Align(this Transform[] elements, Vector2 start, Vector2 single)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].TryGetComponent(out RectTransform rect))
                    continue;

                rect.anchoredPosition = new Vector2(
                    start.x + single.x * i, 
                    start.y + single.y * i
                );
            }
        }

        /// <summary>
        /// Aligns horizontally the elements within the given container
        /// </summary>
        public static void AlignHorizontally(this Transform[] elements, RectTransform container)
        {
            float singleX = container.rect.width / elements.Length;
            float startX = (elements.Length - 1) / 2f * -singleX;

            elements.Align(
                new Vector2(startX, 0),
                new Vector2(singleX, 0)
            );
        }

        /// <summary>
        /// Aligns vertically the elements witin the given container
        /// </summary>
        public static void AlignVertically(this Transform[] elements, RectTransform container)
        {
            float singleY = -container.rect.height / elements.Length;
            float startY = (elements.Length - 1) / 2f * -singleY;

            elements.Align(
                new Vector2(0, startY), 
                new Vector2(0, singleY)
            );
        }
    }
}