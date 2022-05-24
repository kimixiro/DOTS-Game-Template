using Cysharp.Threading.Tasks;
using Unity.Assertions;

namespace DOTSTemplate.States
{
    public class LoadLevelState : LoadSceneStateBase, IState
    {
        private IGameService gameService;

        public LoadLevelState(IGameService gameService)
        {
            this.gameService = gameService;
        }
        
        public void OnEnter()
        {
            var activeLevel = gameService.ActiveLevel;
            Assert.IsNotNull(activeLevel, "Active level not defined");
            LoadLevel(activeLevel).Forget();
        }

        private async UniTaskVoid LoadLevel(LevelDefinition levelDefinition)
        {
            await LoadScene(levelDefinition.Scene);
            gameService.Fire(GameTrigger.Play);
        }

        public void OnExit()
        { 
        }
    }
}