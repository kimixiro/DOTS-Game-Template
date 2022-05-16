namespace DOTSTemplate
{
    public interface IGameService
    {
        void Fire(GameTrigger gameTrigger);
      
    }

    public enum GameTrigger
    {
        Play,
        MainMenu
    }
}