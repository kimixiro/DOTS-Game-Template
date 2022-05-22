namespace DOTSTemplate
{
    public interface IInputServices
    {
        public Controls.DebugActions Debug { get; }
        public Controls.PlayerActions Player { get; }
        public Controls.UIActions UI { get; }
    }
}