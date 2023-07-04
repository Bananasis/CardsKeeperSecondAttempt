using System.Collections.Generic;

namespace Utils
{
    public class PlayerPrefTypeList<S> : PlayerPrefList<S>
    {
        public PlayerPrefTypeList(string name, S def, int defSize) : base(name, def, defSize)
        {
        }

        public PlayerPrefTypeList(string name, S def, IReadOnlyList<S> defValues) : base(name, def, defValues)
        {
        }

        protected override PlayerPrefContainer<S> GetContainer(string name, S def)
        {
            return new PlayerPrefComplexContainer<S>(name, def);
        }
    }
}