namespace Beacon.Screens
{
    public abstract class ConsoleWindow
    {
        public abstract ConsoleWindow DoWork();
        public abstract void OnEntered();

        public virtual ConsoleWindow ReplaceStateIfNeeded()
        {
            return this;
        }
    }
}
