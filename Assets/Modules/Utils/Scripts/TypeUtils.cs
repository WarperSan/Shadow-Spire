using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Type = BattleEntity.Type;

namespace UtilsModule
{
    public static class TypeUtils
    {
        /// <summary>
        /// Finds all the different types in the given type
        /// </summary>
        public static Type[] GetTypes(this Type type)
        {
            Array types = Enum.GetValues(typeof(Type));
            List<Type> indexes = new();

            for (int i = 0; i < types.Length; i++)
            {
                Type item = (Type)types.GetValue(i);

                if ((type & item) != 0)
                    indexes.Add(item);
            }

            if (indexes.Count == 0)
                indexes.Add(Type.NONE);

            return indexes.ToArray();
        }

        /// <summary>
        /// Finds all the icons of every types in the given type
        /// </summary>
        public static string GetIcons(this Type type)
        {
            StringBuilder builder = new();

            Array types = Enum.GetValues(typeof(Type));
            foreach (Type item in types)
            {
                if ((type & item) == 0)
                    continue;

                builder.Append(
                    string.Format(
                        "<sprite name=\"icon_type_{0}\" color={1}>",
                        item.ToString().ToLower(),
                        "#" + ColorUtility.ToHtmlStringRGB(item.GetColor())
                    )
                );
            }


            return builder.ToString();
        }

        /// <summary>
        /// Finds the color of the given type
        /// </summary>
        public static Color GetColor(this Type type)
        {
            string color = type.GetTypes()[0] switch
            {
                Type.NONE => "#D3D3D3",
                Type.NORMAL => "#B9E5EB",
                Type.UNDEAD => "#6BD36B",
                Type.GHOST => "#AA74FF",
                Type.GIANT => "#FF5E47",
                Type.ANIMAL => "#FF8B00",
                Type.AIR => "#00FFD1",
                _ => "white"
            };

            return ColorUtility.TryParseHtmlString(color, out Color clr) ? clr : Color.white;
        }
    }
}