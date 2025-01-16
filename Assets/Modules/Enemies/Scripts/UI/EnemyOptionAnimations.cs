using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Enemies.UI
{
    public class EnemyOptionAnimations : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Graphic sprite;

        [SerializeField]
        private Graphic shadow;

        [SerializeField]
        private RectTransform graphicsParent;

        [SerializeField]
        private CanvasGroup uiGroup;

        [SerializeField]
        private CanvasGroup targetGroup;

        [SerializeField]
        private RectTransform targetFrame;

        #endregion

        #region Animation

        private Coroutine currentAnimation;

        private void PlayAnimation(IEnumerator anim)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(anim);
        }

        #endregion

        #region Hit

        public void Hit() => PlayAnimation(Hit_Coroutine());

        private IEnumerator Hit_Coroutine()
        {
            Coroutine[] parallel = new Coroutine[] {
                StartCoroutine(Hit_Blank(2)),
                StartCoroutine(Hit_Shake(6)),
                StartCoroutine(Hit_FadeIn()),
            };

            foreach (Coroutine item in parallel)
                yield return item;

            Idle();
        }

        private IEnumerator Hit_Blank(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.05f);

                sprite.SetAlpha(0);

                yield return new WaitForSeconds(0.05f);

                sprite.SetAlpha(1);
            }
        }

        private IEnumerator Hit_Shake(int count)
        {
            int index = 0;
            bool goingLeft = true;

            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.08f);

                graphicsParent.localPosition = new Vector2(
                    index * 5,
                    0
                );

                if (goingLeft)
                    index--;
                else
                    index++;

                if (index < 0 || index > 0)
                    goingLeft = !goingLeft;
            }
        }

        private IEnumerator Hit_FadeIn()
        {
            uiGroup.alpha = 0;

            yield return new WaitForSeconds(0.5f);

            yield return uiGroup.FadeIn(5, 0.017f);
        }

        #endregion

        #region Spawn

        public void Spawn() => PlayAnimation(Spawn_Coroutine());

        private IEnumerator Spawn_Coroutine()
        {
            Coroutine[] parallel = new Coroutine[] {
                StartCoroutine(Spawn_FadeIn()),
                StartCoroutine(Spawn_SlideIn()),
                StartCoroutine(Spawn_FadeUiIn()),
            };

            foreach (Coroutine item in parallel)
                yield return item;

            Idle();
        }

        private IEnumerator Spawn_FadeIn()
        {
            Coroutine[] parallel = new Coroutine[] {
                StartCoroutine(sprite.FadeIn(8, 0.17f)),
                StartCoroutine(shadow.FadeIn(8, 0.17f)),
            };

            foreach (Coroutine item in parallel)
                yield return item;
        }

        private IEnumerator Spawn_SlideIn()
        {
            yield return graphicsParent.TranslateLocal(8, 0.17f, new Vector3(0, 40, 0), new Vector3(0, 0, 0));
        }

        private IEnumerator Spawn_FadeUiIn()
        {
            uiGroup.alpha = 0;
            yield return new WaitForSeconds(1.5f);
            yield return uiGroup.FadeIn(10, 0.017f);
        }

        #endregion

        #region Idle

        public void Idle() => PlayAnimation(IdleCoroutine());

        private IEnumerator IdleCoroutine()
        {
            while (true)
                yield return Idle_Bobbing();
        }

        private IEnumerator Idle_Bobbing()
        {
            graphicsParent.localPosition = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            graphicsParent.localPosition = new Vector3(0, 2.5f, 0);
            yield return new WaitForSeconds(0.5f);
            graphicsParent.localPosition = Vector3.zero;
        }

        #endregion

        #region Death

        public void Death() => PlayAnimation(Death_Coroutine());

        private IEnumerator Death_Coroutine()
        {
            Coroutine[] parallel = new Coroutine[] {
                StartCoroutine(Death_Shake(10)),
                StartCoroutine(Death_FadeOut()),
                StartCoroutine(Death_FadeUiOut()),
            };

            foreach (Coroutine item in parallel)
                yield return item;
        }

        private IEnumerator Death_Shake(int count)
        {
            int index = 0;
            bool goingLeft = true;

            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.05f);

                graphicsParent.localPosition = new Vector2(
                    index * 5,
                    0
                );

                if (goingLeft)
                    index--;
                else
                    index++;

                if (index < 0 || index > 0)
                    goingLeft = !goingLeft;
            }
        }

        private IEnumerator Death_FadeOut()
        {
            yield return new WaitForSeconds(0.25f);

            Coroutine[] parallel = new Coroutine[] {
                StartCoroutine(sprite.FadeOut(40, 0.017f)),
                StartCoroutine(shadow.FadeOut(40, 0.017f)),
            };

            foreach (Coroutine item in parallel)
                yield return item;
        }

        private IEnumerator Death_FadeUiOut()
        {
            yield return uiGroup.FadeOut(30, 0.017f);
        }

        #endregion

        #region Target

        private Coroutine targetAnimation;

        public void EnableTarget()
        {
            if (targetAnimation != null)
                StopCoroutine(targetAnimation);

            targetGroup.alpha = 1;
            targetAnimation = StartCoroutine(Target_Idle());
        }

        public void DisableTarget()
        {
            if (targetAnimation != null)
                StopCoroutine(targetAnimation);

            targetGroup.alpha = 0;
            targetAnimation = null;
        }

        private IEnumerator Target_Idle()
        {
            while (true)
            {
                targetFrame.sizeDelta = Vector2.one * 40;

                yield return new WaitForSeconds(0.5f);

                targetFrame.sizeDelta = Vector2.one * 35;

                yield return new WaitForSeconds(0.5f);
            }
        }

        #endregion
    }
}