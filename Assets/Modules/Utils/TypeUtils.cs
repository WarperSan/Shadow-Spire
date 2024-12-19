using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Type = BattleEntity.Type;

namespace UtilsModule
{
    public static class TypeUtils
    {
        public static Type[] GetTypes(this Type type)
        {
            var types = Enum.GetValues(typeof(Type));
            var indexes = new List<Type>();

            for (int i = 0; i < types.Length; i++)
            {
                Type item = (Type)types.GetValue(i);

                if ((type & item) != 0)
                    indexes.Add(item);
            }

            return indexes.ToArray();
        }

        public static string GetIcons(this Type type)
        {
            StringBuilder builder = new();

            var types = Enum.GetValues(typeof(Type));
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

        public static Color GetColor(this Type type)
        {
            var color = type.GetTypes()[0] switch
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