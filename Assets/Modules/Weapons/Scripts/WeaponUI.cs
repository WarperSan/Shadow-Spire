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
            var weapons = new WeaponOptionData[3] {
                new() {
                    WeaponInstance = GameManager.Instance.player.GetWeapon()
                },
                new() {
                    WeaponInstance = WeaponInstance.CreateRandom(GameManager.Instance.Level.Index)
                },
                new() {
                    WeaponInstance = WeaponInstance.CreateRandom(GameManager.Instance.Level.Index)
                }
            };

            options.LoadOptions(weapons);
            yield return null;



            yield return null;
        }

        //private
    }
}