using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public class HexGridService : IHexGridService
    {
        private HexGrid _hexGrid;
        private GameFactory _gameFactory;
        private LevelProgressService _levelProgressService;
        private CameraService _cameraService;

        public void Initialize(AllServices services)
        {
            _gameFactory = services.Single<GameFactory>();
            _levelProgressService = services.Single<LevelProgressService>();
            _cameraService = services.Single<CameraService>();
        }

        public void CreateGrid()
        {
            var levelAsset = _levelProgressService.CurrentLevelConfig;
            _cameraService.SetWidth(levelAsset.width);
            _hexGrid = new HexGrid(levelAsset, 0.5f, _gameFactory);
        }

        public void CleanUpGrid()
        {
            _hexGrid.CleanUp();
        }

        public HexGrid GetGrid()
        {
            return _hexGrid;
        }

        public List<HexCell> GetNeighbors(int x, int y)
        {
            return _hexGrid.GetNeighbours(x, y);
        }

        public HexCell GetCellByIndex(int index)
        {
            return _hexGrid.GetCellByIndex(index);
        }

        public HexCell GetClosestCell(Vector3 worldPos)
        {
            return _hexGrid.FindClosestCell(worldPos);
        }

        public Vector3 GetGridCenterPos()
        {
            return _hexGrid.Origin + (_hexGrid.GridPosToWorld(_hexGrid.Width - 1, _hexGrid.Height - 1) - _hexGrid.Origin) * 0.5f;
        }
    }
}