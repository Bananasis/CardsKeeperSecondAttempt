using System;

namespace Utils
{
    public class PlayerPrefEnumList<E> : PlayerPrefList<E> where E : unmanaged, Enum
    {
        public PlayerPrefEnumList(string name, E def, int defSize) : base(name, def, defSize)
        {
        }

        protected override PlayerPrefContainer<E> GetContainer(string name, E def)
        {
            return new PlayerPrefEnum<E>(name, def);
        }
    }
}