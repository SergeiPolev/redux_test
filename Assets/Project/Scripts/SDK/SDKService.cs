using System.Collections.Generic;
using System;
//using LionStudios.Suite.Analytics;
using Services;
/*using Io.AppMetrica;
using Io.AppMetrica.Ecommerce;*/
using Newtonsoft.Json;
using UnityEngine;
using Data;

[Serializable]
public class LionAnalyticsProgress
{
    public int run_id;
    public string mission_type;
    public string mission_name;
    public string mission_id;
    public int mission_attempt;
    public int step_count;
    public int player_level;

    public List<MainMissionAttempt> MainMissionAttempts;

    public LionAnalyticsProgress()
    {
        MainMissionAttempts = new List<MainMissionAttempt>();
    }


    [Serializable]
    public class MainMissionAttempt
    {
        public int LevelNumber;
        public int MissionAttempt;

        public MainMissionAttempt(int levelNumber)
        {
            LevelNumber = levelNumber;
        }
    }
}

public class SDKService : IService
{
    private bool _isLevelStarted;
    private int level_number => AllServices.Container.Single<LevelProgressService>().CurrentLevelNumber; //FOR FIREBASE
    private LevelProgressService LevelProgressService => AllServices.Container.Single<LevelProgressService>();
    private GameWalletService GameWallet => AllServices.Container.Single<GameWalletService>();
    private StaticDataService _staticData => AllServices.Container.Single<StaticDataService>();
    //private LevelRewardService LevelRewardService => AllServices.Container.Single<LevelRewardService>();

    protected List<int> _levelCompleteIndexes = new List<int>(3) { 1, 3, 5 };
    
    private LionAnalyticsProgress _lionAnalyticsProgress = new LionAnalyticsProgress();

    private SaveLoadService _saveService;

    public void Initialize(AllServices services)
    {
        _saveService = services.Single<SaveLoadService>();
    }

    public void Level_Start_Event()
    {
        SetStartParameters();
        _lionAnalyticsProgress.run_id++;
        AddMissionAttempt();
        UpdateAdditionalData();
        MainMissionStartedEvent();

        AppmetricaLevelStartEvent();
    }

    private void AppmetricaLevelStartEvent()
    {
        int level_num = LevelProgressService.CurrentLevelNumber;
        int try_count = GetMissionAttempt();
        string eventParameters = $"{{\"level_num\":\"{level_num}\", \"try_count\":\"{try_count}\"}}";
        /*AppMetrica.ReportEvent("level_start", eventParameters);
        AppMetrica.SendEventsBuffer();*/
    }

    private void AppmetricaLevelEndEvent(LevelResult levelResult)
    {
        int level_num = LevelProgressService.CurrentLevelNumber;
        int try_count = GetMissionAttempt();
        string result;

        switch (levelResult)
        {
            case LevelResult.WIN:
                result = "win";
                break;
            case LevelResult.LOSE:
                result = "lose";
                break;
            case LevelResult.LEAVE:
                result = "leave";
                break;
            default:
                result = "leave";
                break;
        }

        string eventParameters = $"{{\"level_num\":\"{level_num}\", \"result\":\"{result}\", \"try_count\":\"{try_count}\"}}";
        /*AppMetrica.ReportEvent("level_finish", eventParameters);
        AppMetrica.SendEventsBuffer();*/
    }

    public void SetStartParameters()
    {
        _isLevelStarted = true;
    }

    public void UpdateAdditionalData()
    {
        _lionAnalyticsProgress.mission_type = GetMissionType();
        _lionAnalyticsProgress.mission_name = GetMissionName();
        _lionAnalyticsProgress.mission_id = GetMissionId();
        _lionAnalyticsProgress.mission_attempt = GetMissionAttempt();
        
      //  AllServices.Container.Single<SaveLoadService>().SaveProgress();
    }

    public void Level_Step_Event()
    {
        MissionStepEvent();
    }

    public void Level_End_Event(LevelResult levelResult)
    {
        _isLevelStarted = false;
        UpdateAdditionalData();

        switch (levelResult)
        {
            case LevelResult.WIN:
                MissionCompleteEvent();
                break;
            case LevelResult.LOSE:
                MainMissionFailedEvent();
                break;
            case LevelResult.LEAVE:
                MainMissionFailedEvent();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(levelResult), levelResult, null);
        }

        AppmetricaLevelEndEvent(levelResult);
    }

    private void MainMissionFailedEvent()
    {
        // Uncomment for LionSDK
        /*LionAnalytics.MissionFailed(false, GetMissionType(), GetMissionName(),
            GetMissionId(), GetUserScore(), GetMissionAttempt(), failReason: "out_of_time");*/
    }

    private void MainMissionStartedEvent()
    {
        // Uncomment for LionSDK
        /*LionAnalytics.MissionStarted(false, GetMissionType(), GetMissionName(),
            GetMissionId(), GetMissionAttempt());*/
    }

    private void MissionStepEvent()
    {
        // Uncomment for LionSDK
        /*LionAnalytics.MissionStep(false, GetMissionType(), GetMissionName(),
            GetMissionId(), GetUserScore(), GetMissionAttempt(), stepName: GetStepName(),
            additionalData: new Dictionary<string, object>()
            {
                {"step_count", LevelProgressService.CurrentStage}
            });*/
    }

    private string GetStepName()
    {
        return $"stage_{LevelProgressService.CurrentStageNumber}";
    }

    private void MissionCompleteEvent()
    {
        // Uncomment for LionSDK
        /*LionAnalytics.MissionCompleted(false, GetMissionType(), GetMissionName(),
            GetMissionId(), GetUserScore(), GetPrevAttemptData().MissionAttempt);*/
    }

    private int GetUserScore() => Mathf.FloorToInt(GameWallet.GetValue(GameCurrencyID.Gold));

    private string GetMissionType() => "main";

    private string GetMissionName() => $"main_{GetMissionId()}";


    private string GetMissionId()
    {
        return LevelProgressService.CurrentLevelNumber.ToString();
    }

    private int GetMissionAttempt()
    {
        var mainMissionAttempt = GetMainMissionAttemptData();
        
        var missionAttempt = mainMissionAttempt.MissionAttempt;

        return missionAttempt;
    }

    private void AddMissionAttempt()
    {
        var mainMissionAttempt = GetMainMissionAttemptData();
        mainMissionAttempt.MissionAttempt++;

        //AllServices.Container.Single<SaveLoadService>().SaveProgress();
    }

    private LionAnalyticsProgress.MainMissionAttempt GetMainMissionAttemptData()
    {
        int levelNumber = LevelProgressService.CurrentLevelNumber;
        var mainMissionAttempt = _lionAnalyticsProgress.MainMissionAttempts.Find(x => x.LevelNumber == levelNumber);

        if (mainMissionAttempt is null)
        {
            mainMissionAttempt = new LionAnalyticsProgress.MainMissionAttempt(levelNumber);
            _lionAnalyticsProgress.MainMissionAttempts.Add(mainMissionAttempt);
           // AllServices.Container.Single<SaveLoadService>().SaveProgress();
        }

        return mainMissionAttempt;
    }
    private LionAnalyticsProgress.MainMissionAttempt GetPrevAttemptData()
    {
        int levelNumber = LevelProgressService.CurrentLevelNumber - 1;
        var mainMissionAttempt = _lionAnalyticsProgress.MainMissionAttempts.Find(x => x.LevelNumber == levelNumber);

        if (mainMissionAttempt is null)
        {
            mainMissionAttempt = new LionAnalyticsProgress.MainMissionAttempt(levelNumber);
            _lionAnalyticsProgress.MainMissionAttempts.Add(mainMissionAttempt);
            //AllServices.Container.Single<SaveLoadService>().SaveProgress();
        }

        return mainMissionAttempt;
    }
    private string FormatString(string textId)
    {
        textId = textId.ToLower().Replace(" ", "_");
        return textId;
    }
}
