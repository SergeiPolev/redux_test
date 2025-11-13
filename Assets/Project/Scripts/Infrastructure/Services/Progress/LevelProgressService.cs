using System.Collections.Generic;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;
using Levels;
using UnityEngine;

namespace Infrastructure.Services.Progress
{
    [System.Serializable]
    public class LevelProgressData
    {
        public int MaxLevel = 99;
        public int CurrentLevel = 1;
    }
    
    public class LevelProgressService : IService, ITick
    {
        private float _timer;
        private StaticDataService _staticDataService;
        private LevelProgressData _data = new LevelProgressData();
        
        public float Timer => _timer;
        public int LevelIndex => CurrentLevelNumber - 1;
        public bool IsMaxLevel => CurrentLevelNumber == MaxLevel;

        public int CurrentLevelNumber
        { 
            get => _data.CurrentLevel;
            private set => _data.CurrentLevel = value;
        }
        public int MaxLevel
        { 
            get => _data.MaxLevel; 
            set => _data.MaxLevel = value;
        }

        public int StageIndex => CurrentStageNumber - 1;
        public int CurrentStageNumber { get; private set; }

        public LevelAsset CurrentLevelConfig { get; private set; }
        public float LevelProgress { get; set; }
        
        public int HexesGoal { get; private set; }
        public int HexesCurrent { get; set; }

        public void Initialize(AllServices services )
        {
            _staticDataService = services.Single<StaticDataService>();
        }
        
        public void Tick()
        {
            _timer += Time.deltaTime;
        }

        #region Level

        public void LevelStarted()
        {
            CurrentStageNumber = 1;
            _timer = 0;
            
            CurrentLevelConfig = _staticDataService.Levels[(CurrentLevelNumber - 1) % _staticDataService.Levels.Count];
            HexesGoal = CurrentLevelConfig.targetBurnedHexes;
            HexesCurrent = 0;
        }

        public void LevelComplete()
        {
            if (IsMaxLevel)
            {
                MaxLevel++;
            }

            CurrentStageNumber = 1;
            LevelProgress = 1;
            SetNextLevel();
        }

        public void SetNextLevel(bool ignoreMax = false)
        {
            if (ignoreMax || IsMaxLevel == false)
            {
                CurrentLevelNumber++;
                if (ignoreMax && CurrentLevelNumber > MaxLevel)
                    MaxLevel = CurrentLevelNumber;
            }
        }
        public void SetPreviousLevel()
        {
            if (CurrentLevelNumber > 1)
            {
                CurrentLevelNumber--;
            }
        }

        #endregion

        #region Stage

        public void Stage_Start()
        {
            _timer = 0;
        }


        public void Stage_Passed()
        {
            CurrentStageNumber++;
        }


        internal void Stage_Lose()
        {

        }



        #endregion

        public List<ColorID> GetColors()
        {
            return CurrentLevelConfig.GetAvailableColors(HexesCurrent);
        }
    }
}