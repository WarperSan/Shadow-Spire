using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Weapons.UI
{
	public class WeaponSelectionUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject title;

		public IEnumerator ShowWeapons()
		{
			_hasSelected = false;

			LoadWeapons(GameManager.Instance.player.Weapon);
			title.SetActive(true);

			yield return null; // Wait for weapons to load

			// Enable inputs
			AddInputs();

			// Wait until weapon selected
			while (!_hasSelected)
				yield return null;

			title.SetActive(false);
			options.DestroyOptions();
		}

		private void SelectWeapon()
		{
			// If already selected, skip
			if (_hasSelected)
				return;

			_hasSelected = true;

			// Disable inputs
			RemoveInputs();

			WeaponOptionData selectedOption = options.GetSelection().GetOption();
			GameManager.Instance.player.Weapon = selectedOption.WeaponInstance;
		}

		#region Options

		[SerializeField]
		private WeaponOptions options;

		private bool _hasSelected = false;

		private void LoadWeapons(WeaponInstance currentWeapon)
		{
			List<WeaponOptionData> weapons = new()
			{
				new WeaponOptionData
				{
					WeaponInstance = currentWeapon,
					Subtext = "Keep",
					OnEnter = SelectWeapon
				}
			};

			for (int i = 0; i < 2; i++)
			{
				weapons.Add(new WeaponOptionData
				{
					WeaponInstance = WeaponInstance.CreateRandom(GameManager.Instance.Level.Index),
					Subtext = "Replace",
					OnEnter = SelectWeapon
				});
			}

			options.LoadOptions(weapons.ToArray());
			options.ShowSelection();
		}

		#endregion

		#region Inputs

		private void Move(Vector2 dir) => options.Move(dir);
		private void Enter()           => options.Enter();

		/// <summary>
		/// Adds the inputs for this object
		/// </summary>
		private void AddInputs()
		{
			InputManager.Instance.onMoveUI.AddListener(Move);
			InputManager.Instance.onEnterUI.AddListener(Enter);
		}

		/// <summary>
		/// Removes the inputs for this object
		/// </summary>
		private void RemoveInputs()
		{
			InputManager.Instance.onMoveUI.RemoveListener(Move);
			InputManager.Instance.onEnterUI.RemoveListener(Enter);
		}

		#endregion
	}
}