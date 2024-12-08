using System;
using UnityEngine;

namespace Battle.Options
{
    public class UIOptionData
    {
        public Action OnEnter;
        public Action OnEscape;
    }

    public abstract class UIOption<T> : MonoBehaviour where T : UIOptionData
    {
        protected T loadedOption;
        public T GetOption() => loadedOption;

        public void LoadOption(T option)
        {
            loadedOption = option;
            OnLoadOption(option);
        }

        protected virtual void OnLoadOption(T option) { }

        public abstract void Select();
        public abstract void Deselect();

        public void Enter() => loadedOption.OnEnter?.Invoke();
        public void Escape() => loadedOption.OnEscape?.Invoke();
    }
}