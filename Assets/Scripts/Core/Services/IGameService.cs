namespace DOTSTemplate
{
    public interface IGameService
    {
        void Fire(GameTrigger gameTrigger);
        LevelDefinition ActiveLevel { get; set; }
    }

    public enum GameTrigger
    {
        Play,
        MainMenu
    }
}