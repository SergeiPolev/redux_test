using System;
using System.Collections.Generic;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;
using Levels;
using UnityEngine;

namespace Infrastructure.Services.Progress
{
    [Serializable]
    public class LevelProgressData
    {
        #region Serialized Fields

        public int MaxLevel = 99;
        public int CurrentLevel = 1;

        #endregion
    }

    public class LevelProgressService : IService, ITick
    {
        private readonly LevelProgressData _data = new();
        private StaticDataService _staticDataService;

        public float Timer { get; private set; }

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

        public void Tick()
        {
            Timer += Time.deltaTime;
        }

        public void Initialize(AllServices services)
        {
            _staticDataService = services.Single<StaticDataService>();
        }

        public List<ColorID> GetColors()
        {
            return CurrentLevelConfig.GetAvailableColors(HexesCurrent);
        }

        #region Level

        public void LevelStarted()
        {
            CurrentStageNumber = 1;
            Timer = 0;

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
            if (ignoreMax || !IsMaxLevel)
            {
                CurrentLevelNumber++;
                if (ignoreMax && CurrentLevelNumber > MaxLevel)
                {
                    MaxLevel = CurrentLevelNumber;
                }
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
            Timer = 0;
        }


        public void Stage_Passed()
        {
            CurrentStageNumber++;
        }


        internal void Stage_Lose()
        {
        }

        #endregion
    }
}