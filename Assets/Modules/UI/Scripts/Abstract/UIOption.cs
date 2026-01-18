using System;
using UnityEngine;

namespace UI.Abstract
{
	public abstract class UIOptionData
	{
		public Action OnEnter;
		public Action OnEscape;
	}

	public abstract class UIOption<T> : MonoBehaviour where T : UIOptionData
	{
		#region Parent

		protected UIOptions Parent;

		public void SetParent(UIOptions parent) => Parent = parent;

		#endregion

		#region Load

		protected T LoadedOption;
		public T GetOption() => LoadedOption;

		public void LoadOption(T option)
		{
			LoadedOption = option;
			OnLoadOption(option);
		}

		protected virtual void OnLoadOption(T option) { }

		#endregion

		#region Selection

		public abstract void Select();
		public abstract void Deselect();

		#endregion

		#region Input

		public void Enter()  => LoadedOption.OnEnter?.Invoke();
		public void Escape() => LoadedOption.OnEscape?.Invoke();

		#endregion
	}
}