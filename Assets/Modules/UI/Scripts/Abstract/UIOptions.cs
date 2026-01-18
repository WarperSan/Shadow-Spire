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
		public virtual U GetSelection<T, U>() where T : UIOptionData where U : UIOption<T> => null;

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

	public abstract class UIOptions<T, U> : UIOptions where T : UIOption<U> where U : UIOptionData
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		protected T optionPrefab;

		#endregion

		#region Load

		protected T[] loadedOptions;
		protected int selectedIndex = -1;

		/// <inheritdoc/>
		public override void LoadOptions(UIOptionData[] options)
		{
			if (options is not U[] typedOptions)
				return;

			DestroyOptions();

			loadedOptions = new T[options.Length];
			Transform[] elements = new Transform[options.Length];

			for (int i = 0; i < options.Length; i++)
			{
				GameObject newOption = Instantiate(optionPrefab.gameObject, transform);

				elements[i] = newOption.transform;

				if (newOption.TryGetComponent(out T battleOption))
				{
					battleOption.SetParent(this);
					battleOption.LoadOption(typedOptions[i]);
					loadedOptions[i] = battleOption;
				}
			}

			selectedIndex = 0;

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
			if (selectedIndex != -1)
				HideSelection();

			// Destroy all options
			foreach (Transform option in transform)
				Destroy(option.gameObject);

			loadedOptions = null;
			selectedIndex = -1;
		}

		#endregion

		#region Selection

		/// <inheritdoc/>
		public override void ShowSelection() => loadedOptions[selectedIndex].Select();

		/// <inheritdoc/>
		public override void HideSelection() => loadedOptions[selectedIndex].Deselect();

		/// <inheritdoc/>
		public override U1 GetSelection<T1, U1>() => loadedOptions[selectedIndex] as U1 ?? base.GetSelection<T1, U1>();

		public T GetSelection() => loadedOptions[selectedIndex];

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
				selectedIndex--;
			else if (dir.x > 0)
				selectedIndex++;

			if (selectedIndex < 0)
				selectedIndex = loadedOptions.Length - 1;

			if (selectedIndex >= loadedOptions.Length)
				selectedIndex = 0;
		}

		/// <inheritdoc/>
		public override void Enter() => loadedOptions[selectedIndex].Enter();

		/// <inheritdoc/>
		public override void Escape() => loadedOptions[selectedIndex].Escape();

		#endregion
	}
}