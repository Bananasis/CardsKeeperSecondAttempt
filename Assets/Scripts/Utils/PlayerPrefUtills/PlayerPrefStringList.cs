namespace Utils
{
    public class PlayerPrefStringList : PlayerPrefList<string>
    {
        public PlayerPrefStringList(string name, string def, int defSize) : base(name, def, defSize)
        {
        }

        protected override PlayerPrefContainer<string> GetContainer(string name, string def)
        {
            return new PlayerPrefString(name, def);
        }
    }
}