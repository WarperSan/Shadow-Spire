using UnityEngine;

namespace Battle.Minigame
{
    public class MinigameProjectile : MonoBehaviour
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer sprite;

        public void SetColor(Color color) => sprite.color = color;

        #endregion

        private void Update() => Move();

        protected virtual void Move()
        {
            transform.Translate(5f * Time.deltaTime * Vector3.down);
        }
    }
}