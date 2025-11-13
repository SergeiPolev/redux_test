using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infrastructure
{
    public class HexPilesService : IService
    {
        private CameraService _cameraService;
        private GameFactory _gameFactory;
        private InputService _inputService;
        private CancellationAsyncService _cancellationAsyncService;
        private LevelProgressService _levelProgressService;

        private List<HexPile> _hexPiles;

        public void Initialize(CameraService cameraService, GameFactory gameFactory, InputService inputService, CancellationAsyncService cancellationAsyncService,  LevelProgressService levelProgressService)
        {
            _inputService = inputService;
            _cameraService =  cameraService;
            _gameFactory =  gameFactory;
            _cancellationAsyncService =  cancellationAsyncService;
            _levelProgressService = levelProgressService;
        }

        public void OnLevelEnter()
        {
            CreatePiles();
        }

        public HexPile SpawnHexPile(Vector3 position)
        {
            var hexPile = _gameFactory.CreateHexPile();
            
            // TODO: Add setting for game factory spawn hexes in pile
            var count = Random.Range(2, 5);
            hexPile.Hexes = new Hex[count];
            
            ColorID colorID = _levelProgressService.GetColors().GetRandom();
            //ColorID colorID = ColorID.Blue;
            int changeColor = Random.Range(1, 5);
            
            for (int i = 0; i < hexPile.Hexes.Length; i++)
            {
                hexPile.Hexes[i] = _gameFactory.CreateHex(colorID);
                hexPile.Hexes[i].HexModelView.transform.SetParent(hexPile.transform);
                changeColor--;

                if (changeColor == 0)
                {
                    changeColor =  Random.Range(1, 5);
                    colorID = _levelProgressService.GetColors().GetRandom();
                }
            }
            
            hexPile.transform.position = position;
            hexPile.OriginPos = position;
            hexPile.RebuildStack();
            hexPile.OnPileRemoved += CheckPilesEmpty;
            
            return hexPile;
        }

        private void CheckPilesEmpty(HexPile hexPile)
        {
            hexPile.OnPileRemoved -= CheckPilesEmpty;
            
            _hexPiles.Remove(hexPile);
            if (_hexPiles.Count == 0)
            {
                CreatePiles();
            }
        }

        private async void CreatePiles()
        {
            _inputService.SetListenerType(InputType.Idle);
            
            var count = 3;
            _hexPiles = new List<HexPile>(count);
            
            for (int i = 0; i < count; i++)
            {
                Transform pilePoint = _cameraService.CameraGroup.PilePoints[i];

                var pilePosWorld = pilePoint.position;
                var pilePos = pilePoint.localPosition;
                pilePoint.localPosition += Vector3.right * 10;
                
                HexPile hexPile = SpawnHexPile(pilePoint.position);
                hexPile.transform.SetParent(pilePoint.transform);
                hexPile.OriginPos = pilePosWorld;
                _hexPiles.Add(hexPile);

                var tween = pilePoint.DOLocalMove(pilePos, 1f).SetDelay(0.1f * i).SetLink(pilePoint.gameObject);

                if (i == count - 1)
                {
                    await tween.ToUniTask(_cancellationAsyncService.Token);
                }
            }

            _inputService.SetListenerType(InputType.DragAndDrop);
        }

        public void ShufflePiles()
        {
            ClearPiles();
            CreatePiles();
        }

        public void ClearPiles()
        {
            foreach (var hexPile in _hexPiles)
            {
                foreach (var hex in hexPile.Hexes)
                {
                    LeanPool.Despawn(hex.HexModelView);
                }
                
                LeanPool.Despawn(hexPile);
            }
        }
    }
}