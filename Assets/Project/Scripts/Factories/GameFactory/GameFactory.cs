using Boosters.Views;
using Hexes;
using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Lean.Pool;
using UnityEngine;

namespace Factories.GameFactory
{
    public class GameFactory : IService
    {
        private CameraService _cameraService;

        private Transform _cellsParent;
        private GlobalBlackboard _globalBlackboard;
        private StaticDataService _staticData;

        public void Initialize(GlobalBlackboard globalBlackboard, StaticDataService staticData,
            CameraService cameraService)
        {
            _globalBlackboard = globalBlackboard;
            _staticData = staticData;
            _cameraService = cameraService;

            _cellsParent = new GameObject("Cells").transform;
        }

        public HexCellView CreateCellView(int index)
        {
            var prefab = _staticData.Prefabs.HexCellView;
            var model = LeanPool.Spawn(prefab, Vector3.zero, Quaternion.identity, _cellsParent);
            model.Initialize(index);

            return model;
        }

        public Hex CreateHex(ColorID color)
        {
            return new Hex(CreateHexModelView(color), color);
        }

        public HexPile CreateHexPile()
        {
            var prefab = _staticData.Prefabs.HexPile;
            var pile = LeanPool.Spawn(prefab);

            return pile;
        }

        public BrushView CreateBrushView()
        {
            var prefab = _staticData.Prefabs.BrushView;
            var model = LeanPool.Spawn(prefab, Vector3.zero, Quaternion.identity);

            model.transform.position = _cameraService.CameraGroup.TargetPoint.position + Vector3.up * 1;

            return model;
        }

        private HexModelView CreateHexModelView(ColorID id)
        {
            var material = GetColorMaterial(id);
            var color = _globalBlackboard.ColorsByID[id];
            var prefab = _staticData.Prefabs.HexModelView;
            var model = LeanPool.Spawn(prefab, Vector3.zero, Quaternion.identity);

            model.SetMaterial(material);
            model.SetVFXColor(color);
            model.Initialize();

            return model;
        }

        public HammerView CreateHammerView()
        {
            var prefab = _staticData.Prefabs.HammerView;
            var model = LeanPool.Spawn(prefab, Vector3.zero, Quaternion.identity);

            model.transform.position = _cameraService.CameraGroup.TargetPoint.position + Vector3.up * 1;

            return model;
        }

        public Material GetColorMaterial(ColorID id)
        {
            return _globalBlackboard.MaterialsByColorID[id];
        }

        public Color GetColor(ColorID id)
        {
            return _globalBlackboard.ColorsByID[id];
        }
    }
}