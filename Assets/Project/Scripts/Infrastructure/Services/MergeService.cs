using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services;

namespace Infrastructure
{
    public struct MergeQueueElement
    {
        public HexCell Cell;
        public Func<HexCell, UniTask> Action;
    }

    public class MergeService : IService
    {
        private IHexGridService _hexGridService;
        private CancellationAsyncService _cancellationAsyncService;

        public Queue<MergeQueueElement> TaskQueue = new Queue<MergeQueueElement>();

        private bool _taskStarted = false;

        public event Action OnMergeEnded;

        public void Initialize(IHexGridService hexGridService, CancellationAsyncService cancellationAsyncService)
        {
            _hexGridService = hexGridService;
            _cancellationAsyncService = cancellationAsyncService;
        }

        public void OnLevelEnter()
        {
            _hexGridService.GetGrid().OnCellChanged += x =>
            {
                TaskQueue.Enqueue(new MergeQueueElement()
                {
                    Cell = x, Action = OnCellChanged
                });
                
                CheckMergeQueue();
            };
        }

        private async void CheckMergeQueue()
        {
            if (!_taskStarted)
            {
                _taskStarted = true;

                while (TaskQueue.Count > 0)
                {
                    var queueElement = TaskQueue.Dequeue();
                    await queueElement.Action.Invoke(queueElement.Cell);
                }

                _taskStarted = false;
                OnMergeEnded?.Invoke();
            }
        }

        public async UniTask OnCellChanged(HexCell cell)
        {
            if (cell.IsEmpty())
            {
                return;
            }
            
            var neighbors = _hexGridService.GetNeighbors(cell.x, cell.y);
            var placedColor = cell.GetTopColor();
            var sameColoredNeighbours = neighbors.Where(x => !x.IsEmpty() && x.GetTopColor() == placedColor);

            if (!sameColoredNeighbours.Any())
            {
                return;
            }
            
            foreach (var neighbor in sameColoredNeighbours)
            {
                if (cell.IsEmpty())
                {
                    break;
                }

                if (neighbor.IsEmpty())
                {
                    continue;
                }

                if (neighbor.GetTopColor() == placedColor)
                {
                    // connecting neighbors if there are more than one same colored neighbours
                    if (sameColoredNeighbours.Count() > 1 || cell.IsOnlyColor(placedColor))
                    {
                        await Merge(neighbor, cell);
                    }
                    else
                    {
                        await Merge(cell, neighbor);
                    }

                    break;
                }
            }
        }

        private async UniTask Merge(HexCell from, HexCell to)
        {
            var hexes = from.GetTopHexes();

            await to.AddHexesAnimated(hexes, _cancellationAsyncService.Token);

            await OnCellChanged(from);
            await OnCellChanged(to);
        }
    }
}