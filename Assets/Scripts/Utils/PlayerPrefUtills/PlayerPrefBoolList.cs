namespace Utils
{
    public class PlayerPrefBoolList : PlayerPrefList<bool>
    {
        public PlayerPrefBoolList(string name, bool def, int defSize) : base(name, def, defSize)
        {
        }

        protected override PlayerPrefContainer<bool> GetContainer(string name, bool def)
        {
            return new PlayerPrefBool(name, def);
        }
    }
}