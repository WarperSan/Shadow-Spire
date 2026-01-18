using UnityEngine;

namespace UI.Abstract
{
	public abstract class UIOptions : UIComponent
	{
		#region Load

		/// <summary>
		/// Loads the given options to the UI
		/// </summary>
		public virtual void LoadOptions(UIOptionData[] options) { }

		#endregion

		#region Selection

		/// <summary>
		/// Shows the current selection of this element
		/// </summary>
		public virtual void ShowSelection() { }

		/// <summary>
		/// Hides the current selection of this element
		/// </summary>
		public virtual void HideSelection() { }

		/// <summary>
		/// Obtains the current selection of this element
		/// </summary>
		public virtual TU GetSelection<T, TU>() where T : UIOptionData where TU : UIOption<T> => null;

		#endregion

		#region Inputs

		/// <summary>
		/// Moves the cursor to the next option depending on the given direction
		/// </summary>
		public virtual void Move(Vector2 dir) { }

		/// <summary>
		/// Calls '<see cref="ActionOption.OnEnter"/>' of the selected option
		/// </summary>
		public virtual void Enter() { }

		/// <summary>
		/// Calls '<see cref="ActionOption.OnEscape"/>' of the selection option
		/// </summary>
		public virtual void Escape() { }

		#endregion
	}

	public abstract class UIOptions<T, TU> : UIOptions where T : UIOption<TU> where TU : UIOptionData
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		protected T optionPrefab;

		#endregion

		#region Load

		protected T[] LoadedOptions;
		protected int SelectedIndex = -1;

		/// <inheritdoc/>
		public override void LoadOptions(UIOptionData[] options)
		{
			if (options is not TU[] typedOptions)
				return;

			DestroyOptions();

			LoadedOptions = new T[options.Length];
			Transform[] elements = new Transform[options.Length];

			for (int i = 0; i < options.Length; i++)
			{
				GameObject newOption = Instantiate(optionPrefab.gameObject, transform);

				elements[i] = newOption.transform;

				if (newOption.TryGetComponent(out T battleOption))
				{
					battleOption.SetParent(this);
					battleOption.LoadOption(typedOptions[i]);
					LoadedOptions[i] = battleOption;
				}
			}

			SelectedIndex = 0;

			AlignOptions(elements);
		}

		/// <summary>
		/// Aligns the loaded options
		/// </summary>
		protected virtual void AlignOptions(Transform[] elements) { }

		#endregion

		#region Destroy

		/// <summary>
		/// Destroys all the loaded options
		/// </summary>
		public void DestroyOptions()
		{
			if (SelectedIndex != -1)
				HideSelection();

			// Destroy all options
			foreach (Transform option in transform)
				Destroy(option.gameObject);

			LoadedOptions = null;
			SelectedIndex = -1;
		}

		#endregion

		#region Selection

		/// <inheritdoc/>
		public override void ShowSelection() => LoadedOptions[SelectedIndex].Select();

		/// <inheritdoc/>
		public override void HideSelection() => LoadedOptions[SelectedIndex].Deselect();

		/// <inheritdoc/>
		public override TU1 GetSelection<T1, TU1>() => LoadedOptions[SelectedIndex] as TU1 ?? base.GetSelection<T1, TU1>();

		public T GetSelection() => LoadedOptions[SelectedIndex];

		#endregion

		#region Inputs

		/// <inheritdoc/>
		public override void Move(Vector2 dir)
		{
			HideSelection();
			OnMoveSelected(dir);
			ShowSelection();
		}

		protected virtual void OnMoveSelected(Vector2 dir)
		{
			dir = dir.normalized;

			if (dir.x < 0)
				SelectedIndex--;
			else if (dir.x > 0)
				SelectedIndex++;

			if (SelectedIndex < 0)
				SelectedIndex = LoadedOptions.Length - 1;

			if (SelectedIndex >= LoadedOptions.Length)
				SelectedIndex = 0;
		}

		/// <inheritdoc/>
		public override void Enter() => LoadedOptions[SelectedIndex].Enter();

		/// <inheritdoc/>
		public override void Escape() => LoadedOptions[SelectedIndex].Escape();

		#endregion
	}
}