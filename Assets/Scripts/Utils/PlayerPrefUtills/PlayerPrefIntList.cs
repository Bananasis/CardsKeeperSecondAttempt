using System.Collections.Generic;

namespace Utils
{
    public class PlayerPrefIntList : PlayerPrefList<int>
    {
        public PlayerPrefIntList(string name, int def, int defSize) : base(name, def, defSize)
        {
        }

        public PlayerPrefIntList(string name, int def, IReadOnlyList<int> defValues) : base(name, def, defValues)
        {
        }

        protected override PlayerPrefContainer<int> GetContainer(string name, int def)
        {
            return new PlayerPrefInt(name, def);
        }
    }
}