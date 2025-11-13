using System;
using DG.Tweening;
using Services;

namespace Infrastructure
{
    public class ClearStackService : IService, IDisposable
    {
        private IHexGridService _hexGridService;
        private MergeService _mergeService;
        private CancellationAsyncService _cancellationAsyncService;
        private LevelProgressService _levelProgressService;

        public event Action OnClearCompleted;
        
        public void Initialize(IHexGridService hexGridService, MergeService mergeService, CancellationAsyncService cancellationAsyncService, LevelProgressService levelProgressService)
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
            int topColorCount = cell.GetTopColorCount();

            if (topColorCount >= 10)
            {
                await cell.RemoveTopHexes(_cancellationAsyncService.Token);
            }
            
            OnClearCompleted?.Invoke();
        }

        public void Dispose()
        {
            _mergeService.OnMergeEnded -= OnMergeEnded;
        }
    }
}