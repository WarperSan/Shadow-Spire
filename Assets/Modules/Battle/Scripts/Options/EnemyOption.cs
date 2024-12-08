using BattleEntity;
using Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Battle.Options
{
    public class EnemyOptionData : UIOptionData
    {
        public WeaponSO Weapon;
        public BattleEnemyEntity Entity;
    }

    /// <summary>
    /// Script that handles to display a single option
    /// </summary>
    public class EnemyOption : UIOption<EnemyOptionData>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image sprite;

        [SerializeField]
        private Image shadow;

        [Header("Health")]
        [SerializeField]
        private TextMeshProUGUI health;

        [SerializeField]
        private TextMeshProUGUI healthPopup;

        #endregion

        #region API

        /// <inheritdoc/>
        protected override void OnLoadOption(EnemyOptionData option)
        {
            SetData(option.Entity.Enemy);
            SetEffectiveness(option.Weapon, option.Entity);
            SpawnAnimation();
        }

        /// <inheritdoc/>
        public override void Select()
        {
            SetTargetted(true);
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            SetTargetted(false);
        }

        #endregion

        #region Data

        private void SetData(EnemySO enemy)
        {
            sprite.sprite = enemy.FightSprite;
            shadow.sprite = enemy.FightShadowSprite;
        }

        #endregion

        #region Animations

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        private void SpawnAnimation() => animator.SetTrigger("spawnIn");
        public void HitAnimation(int damage)
        {
            animator.SetTrigger("hit");
            healthPopup.text = string.Format("-{0}", damage);
        }
        private void SetTargetAnimation(bool isTargetted) => animator.SetBool("targetted", isTargetted);

        #endregion

        #region Target

        [Header("Target")]
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private TextMeshProUGUI targetEffectiveness;

        private void SetEffectiveness(WeaponSO weapon, BattleEnemyEntity entity)
        {
            float percent = entity.CalculateEffectiveness(weapon.AttackType);

            targetEffectiveness.text = string.Format(
                "<sprite name={0}> <color={1}>{2}</color>%",
                weapon.Icon.name,
                GetEffectivenessColor(percent),
                percent
            );
        }

        private void SetTargetted(bool isTarget)
        {
            target.SetActive(isTarget);
            SetTargetAnimation(isTarget);
        }

        private string GetEffectivenessColor(float percent)
        {
            if (percent >= 250)
                return "orange";

            if (percent >= 150)
                return "purple";

            if (percent > 100)
                return "green";

            if (percent == 100)
                return "white";

            if (percent >= 75)
                return "#A0A0A0";

            return "#505050";
        }

        #endregion
    }
}