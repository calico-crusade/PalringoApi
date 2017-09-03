using System;
using System.Reflection;

namespace PalringoApi.Networking.PacketHandling.PacketMap
{
    /// <summary>
    /// The packet mapper
    /// </summary>
    public static class PacketMapper
    {
        /// <summary>
        /// Maps a packet to any given object using the <see cref="PacketHeader"/> and <see cref="PacketPayload"/> attributes
        /// </summary>
        /// <typeparam name="T">The object to map to</typeparam>
        /// <param name="packet">The packet to map</param>
        /// <returns>The mapped object</returns>
        public static T Map<T>(Packet packet)
        {
            var instance = Activator.CreateInstance<T>();

            foreach(var prop in instance.GetType().GetProperties())
            {
                if (!Attribute.IsDefined(prop, typeof(PacketHeader)) && !Attribute.IsDefined(prop, typeof(PacketPayload)))
                    continue;

                if (Attribute.IsDefined(prop, typeof(PacketPayload)))
                {
                    SetProperty(instance, prop, packet.Payload);
                    continue;
                }

                var atr = prop.GetCustomAttribute<PacketHeader>();
                if (!packet.Headers.ContainsKey(atr.Key.ToUpper()))
                    continue;

                SetProperty(instance, prop, packet[atr.Key.ToUpper()]);
            }

            return instance;
        }

        private static void SetProperty<T>(T item, PropertyInfo prop, object value)
        {
            try
            {
                if (value == null)
                    return;
                var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (t.IsEnum)
                {
                    prop.SetValue(item, Enum.ToObject(t, Convert.ChangeType(value, typeof(int))), null);
                    return;
                }
                if (t == typeof(GroupId))
                {
                    prop.SetValue(item, (GroupId)(int)Convert.ChangeType(value, typeof(int)), null);
                    return;
                }
                if (t == typeof(UserId))
                {
                    prop.SetValue(item, (UserId)(int)Convert.ChangeType(value, typeof(int)), null);
                    return;
                }
                var s = Convert.ChangeType(value, t);
                prop.SetValue(item, s, null);
            }
            catch { }
        }
    }
}
