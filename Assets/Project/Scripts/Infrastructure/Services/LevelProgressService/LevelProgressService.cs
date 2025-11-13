using System.Collections.Generic;
using Infrastructure;
using Levels;
using UnityEngine;

namespace Services
{
    [System.Serializable]
    public class LevelProgressData : SaveData
    {
        public int MaxLevel = 1;
        public int CurrentLevel = 1;
    }
    
    public class LevelProgressService : IService, ISaveProgress, ITick
    {
        private float _timer;
        private StaticDataService _staticDataService;
        private SaveLoadService _save;
        private LevelProgressData _data;
        
        public float Timer => _timer;
        public int LevelIndex => CurrentLevelNumber - 1;
        public bool IsMaxLevel => CurrentLevelNumber == MaxLevel;

        public int CurrentLevelNumber
        { 
            get => _data.CurrentLevel;
            set { _data.CurrentLevel = value; _save.SaveProgress(this); } 
        }
        public int MaxLevel
        { 
            get => _data.MaxLevel; 
            set { _data.MaxLevel = value; _save.SaveProgress(this);  } 
        }

        public int StageIndex => CurrentStageNumber - 1;
        public int CurrentStageNumber { get; private set; }

        public LevelAsset CurrentLevelConfig { get; set; }
        public float LevelProgress { get; set; }
        
        public int HexesGoal { get; set; }
        public int HexesCurrent { get; set; }

        public void Initialize(AllServices services )
        {
            _save = services.Single<SaveLoadService>();
            _staticDataService = services.Single<StaticDataService>();
        }

        public void WriteProgress(SaveLoadService saveService)
        {
            saveService.SetData(StringConstants.LevelProgressKey, _data);
        }

        public void LoadProgress(SaveLoadService saveService)
        {
            if (saveService.GetSaveData(StringConstants.LevelProgressKey, out _data) == false)
            {
                _data = new LevelProgressData();
                saveService.SaveProgress(this);
            }
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
            _save.SaveProgress(this);
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