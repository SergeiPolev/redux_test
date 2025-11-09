using Infrastructure;
using Services;
using Tayx.Graphy;

namespace DebugGame
{
    public class DebugService : IService
    {
        private const string SAVE_ID = "DebugData";
        [System.Serializable]
        private class DebugGameSaveData
        {
            public bool FPS_Enable = true;
            public bool HUD_Enable = true;
            public GraphyManager.ModulePosition FPS_Position = GraphyManager.ModulePosition.TOP_RIGHT;
            public bool Console_Enable = false;
            public bool Hand_Cursor_Enable = false;
            public string LastConfigSelection;

        }
        private DebugGameSaveData _data;
        private WindowService _windows;
        private IGameStateChanger _stateChanger;
        private SaveLoadService _gameSave;
        private BinarySaverData<DebugGameSaveData> _saver;

        public ref bool FPS_Enable => ref _data.FPS_Enable;
        public ref bool HUD_Enable => ref _data.HUD_Enable;
        public ref GraphyManager.ModulePosition FPS_Position => ref _data.FPS_Position;
        public ref bool Console_Enable => ref _data.Console_Enable;
        public ref string LastConfigSelection => ref _data.LastConfigSelection;
        public ref bool Hand_Cursor_Enable => ref _data.Hand_Cursor_Enable;



        public void Initialize(AllServices services, IGameStateChanger stateChanger)
        {
            _gameSave = services.Single<SaveLoadService>();
            _saver = new BinarySaverData<DebugGameSaveData>(SAVE_ID);
            _windows = services.Single<WindowService>();
            _stateChanger = stateChanger;

            _data = _saver.Load();
            if(_data == null)
                _data = new DebugGameSaveData();
        }

        public void OnSaveChange()
        {
            _saver.Save(_data);
        }

        public void SaveNameSelectConfig(string name)
        {
            _data.LastConfigSelection = name;
            OnSaveChange();
        }


        public void GoSelectionConfig()
        {
            _stateChanger.Enter<Game_CleanUpState>();
            _stateChanger.Enter<SelectGoogleSheetState>();
        }


        public void ResetSetting()
        {
            var saveName = _data.LastConfigSelection;
            _data = new DebugGameSaveData();
            _data.LastConfigSelection = saveName;
            OnSaveChange();
        }

        public void OnWinLevel()
        {
            AllServices.Container.Single<ResultService>().Win();
        }

        public void OnLoseLevel()
        {
            AllServices.Container.Single<ResultService>().Lose();
        }
        
        internal void ResetSaveData()
        {
            _gameSave.DeleteSave();
        }
    }
}