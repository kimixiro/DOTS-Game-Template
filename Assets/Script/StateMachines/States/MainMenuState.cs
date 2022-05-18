using DOTSTemplate.Ui;
using Unity.Assertions;
using UnityEngine;

namespace DOTSTemplate.States
{
    public class MainMenuState : IState
    {
        private MainMenuView view;
        private readonly IGameService gameService;
        private readonly IDatabaseService databaseService;

        public MainMenuState(IGameService gameService, IDatabaseService databaseService)
        {
            this.gameService = gameService;
            this.databaseService = databaseService;
        }

        public void OnEnter()
        {
            view = Object.FindObjectOfType<MainMenuView>();
            Assert.IsNotNull(view, "Main menu view not found");
            view.Play += OnPlay;
        }

        private void OnPlay()
        {
            gameService.ActiveLevel = databaseService.Levels[0];
            gameService.Fire(GameTrigger.Play);
        }

        public void OnExit()
        {
            view.Play -= OnPlay;
        }
    }
}