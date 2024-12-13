using System.Collections;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField]
        private WeaponOptions options;

        public IEnumerator ShowWeapons()
        {
            // var weapons = new WeaponOptionData[3] {
            //     new 
            // }
            //GameManager.Instance.player.GetWeapon();
            yield return null;
        }
    }
}