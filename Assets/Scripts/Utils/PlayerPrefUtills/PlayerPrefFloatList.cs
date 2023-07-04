namespace Utils
{
    public class PlayerPrefFloatList : PlayerPrefList<float>
    {
        public PlayerPrefFloatList(string name, float def, int defSize) : base(name, def, defSize)
        {
        }

        protected override PlayerPrefContainer<float> GetContainer(string name, float def)
        {
            return new PlayerPrefFloat(name, def);
        }
    }
}