using Infrastructure.Services.Core;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;
using Infrastructure.States;

namespace Infrastructure.Services.Gameplay
{
    public class ResultService : IService
    {
        private IStateChanger _gameStateChanger;
        private IHexGridService _hexGridService;
        private LevelProgressService _levelProgressService;
        private LevelStateMachine _levelStateMachine;
        private WindowService _windowService;

        public void Initialize(IStateChanger gameStateChanger, LevelProgressService levelProgressService,
            MergeService mergeService, IHexGridService hexGridService)
        {
            _hexGridService = hexGridService;
            _gameStateChanger = gameStateChanger;
            _levelProgressService = levelProgressService;

            mergeService.OnMergeEnded += CheckLose;
        }

        public void OnLevelEnter()
        {
            _hexGridService.GetGrid().OnHexesRemoved += CountHexes;
        }

        public void OnLevelExit()
        {
            _hexGridService.GetGrid().OnHexesRemoved -= CountHexes;
        }

        private void CountHexes(int value)
        {
            _levelProgressService.HexesCurrent += value;

            if (_levelProgressService.HexesCurrent >= _levelProgressService.HexesGoal)
            {
                Win();
            }
        }

        private void CheckLose()
        {
            bool AnyFreeSpace()
            {
                var hexGrid = _hexGridService.GetGrid();
                for (var x = 0; x < hexGrid.Width; x++)
                {
                    for (var y = 0; y < hexGrid.Height; y++)
                    {
                        if (hexGrid.Cells[x, y].IsEmpty())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            if (!AnyFreeSpace())
            {
                Lose();
            }
        }

        public void SetLevelStateChanger(LevelStateMachine levelStateMachine)
        {
            _levelStateMachine = levelStateMachine;
        }

        public void Win()
        {
            _gameStateChanger.Enter<Game_ResultState, LevelResult>(LevelResult.WIN);
        }

        public void Lose()
        {
            _gameStateChanger.Enter<Game_ResultState, LevelResult>(LevelResult.LOSE);
        }
    }
}