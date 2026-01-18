using BattleEntity;
using TMPro;
using UI.Abstract;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Weapons;

namespace Enemies.UI
{
	/// <summary>
	/// Data used for the enemy selection menu
	/// </summary>
	public class EnemyOptionData : UIOptionData
	{
		public WeaponInstance Weapon;
		public BattleEnemyEntity Entity;
	}

	/// <summary>
	/// Option used for the enemy selection menu
	/// </summary>
	public class EnemyOption : UIOption<EnemyOptionData>
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private Image sprite;

		[SerializeField]
		private Image shadow;

		[SerializeField]
		private TextMeshProUGUI types;

		#endregion

		#region API

		/// <inheritdoc/>
		protected override void OnLoadOption(EnemyOptionData option)
		{
			SetEntity(option.Entity);
			SetEffectiveness(option.Weapon, option.Entity);
			animations.Spawn();
		}

		/// <inheritdoc/>
		public override void Select()
		{
			SetTarget(true);
		}

		/// <inheritdoc/>
		public override void Deselect()
		{
			SetTarget(false);
		}

		#endregion

		#region Entity

		private void SetEntity(BattleEnemyEntity entity)
		{
			entity.Hit.AddListener(OnHit);
			entity.Death.AddListener(OnDeath);

			types.text = entity.Type.GetIcons();

			SetHealth((uint)entity.Health, 0);

			EnemySo enemy = entity.Enemy.GetRaw();
			sprite.sprite = enemy.fightSprite;

			shadow.sprite = enemy.fightShadowSprite;
			shadow.enabled = shadow.sprite != null;
		}

		private void OnHit(int damage)
		{
			SetDamage((uint)damage);
			SetHealth((uint)LoadedOption.Entity.Health, 0);
			animations.Hit();
		}

		private void OnDeath(int damage)
		{
			SetDamage((uint)damage);
			SetHealth(0, 0);
			animations.Death();

			(Parent as EnemyOptions)?.FindNextValid(Vector2.right);
		}

		#endregion

		#region Health

		[Header("Health")]
		[SerializeField]
		private HealthBar healthBar;

		private void SetHealth(uint health, uint maxHealth) => healthBar.SetHealth(health, maxHealth);
		private void SetDamage(uint damage) => healthBar.TakeDamage(damage);

		#endregion

		#region Animations

		[Header("Animations")]
		[SerializeField]
		private EnemyOptionAnimations animations;

		#endregion

		#region Target

		[Header("Target")]
		[SerializeField]
		private TextMeshProUGUI targetEffectiveness;

		private void SetEffectiveness(WeaponInstance weapon, BattleEnemyEntity entity)
		{
			float percent = entity.CalculateEffectiveness(weapon.GetTypes());

			targetEffectiveness.text = string.Format(
				"<sprite name={0}> <color={1}>{2}</color>%",
				weapon.GetIcon().name,
				GetEffectivenessColor(percent),
				percent
			);
		}

		private void SetTarget(bool isTarget)
		{
			if (isTarget)
				animations.EnableTarget();
			else
				animations.DisableTarget();
		}

		private static string GetEffectivenessColor(float percent)
		{
			if (percent >= 450)
				return "#FF6CC6";

			if (percent >= 250)
				return "orange";

			if (percent >= 150)
				return "purple";

			if (percent > 100)
				return "green";

			if (Mathf.Approximately(percent, 100))
				return "white";

			if (percent >= 75)
				return "#A0A0A0";

			return "#505050";
		}

		#endregion
	}
}