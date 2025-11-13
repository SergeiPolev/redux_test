using System;
using Hexes;
using Infrastructure.Services.Core;
using Infrastructure.Services.Progress;

namespace Infrastructure.Services.Gameplay
{
    public class ClearStackService : IService, IDisposable
    {
        private CancellationAsyncService _cancellationAsyncService;
        private IHexGridService _hexGridService;
        private LevelProgressService _levelProgressService;
        private MergeService _mergeService;

        public void Dispose()
        {
            _mergeService.OnMergeEnded -= OnMergeEnded;
        }

        public event Action OnClearCompleted;

        public void Initialize(IHexGridService hexGridService, MergeService mergeService,
            CancellationAsyncService cancellationAsyncService, LevelProgressService levelProgressService)
        {
            _levelProgressService = levelProgressService;
            _cancellationAsyncService = cancellationAsyncService;
            _mergeService = mergeService;
            _hexGridService = hexGridService;

            _mergeService.OnMergeEnded += OnMergeEnded;
        }

        private void OnMergeEnded()
        {
            var cells = _hexGridService.GetGrid().Cells;

            foreach (var cell in cells)
            {
                if (cell.IsEmpty() || cell.CellState != HexCell.HexCellState.Normal)
                {
                    continue;
                }

                ClearCell(cell);
            }
        }

        private async void ClearCell(HexCell cell)
        {
            var topColorCount = cell.GetTopColorCount();

            if (topColorCount >= 10)
            {
                await cell.RemoveTopHexes(_cancellationAsyncService.Token);
            }

            OnClearCompleted?.Invoke();
        }
    }
}