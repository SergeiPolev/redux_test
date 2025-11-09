using Data;
using UnityEngine;

namespace Services
{
    /* ¬се сохрон€емые данные должны быть наследниками этого класса дл€ сохранени€ версии.
     * ¬ерси€ обновл€тьс€ при сохранение данных на текущую версию приложени€
     */
    [System.Serializable]
    public abstract class SaveData
    {
        public string Version;
    }

    public class SaveLoadService: IService
    {
        private const string SAVE_PROGRESS = "PlayerProgress";

        private ISaverData<PlayerProgress> _saveSystem;
        private PlayerProgress _progress;
        private AllServices _services;

        //ќбщий флаг дл€ первого запуска приложени€
        public bool IsFirstPlay { get; private set; }

        public void Initialize(AllServices services)
        {
            _services = services;
            _saveSystem = new BinarySaverData<PlayerProgress>(SAVE_PROGRESS);
            _progress = null;
        }

        public void LoadProgressAndInformReaders()
        {
            _progress = LoadProgress();
            IsFirstPlay = _progress == null || _progress.IsEmpty();
            if (IsFirstPlay)
                _progress = new PlayerProgress();
            InformPrgressReaders();
        }

        private PlayerProgress LoadProgress()
        {
            if (_saveSystem.HasData())
            {
                var data = _saveSystem.Load();
                return data;
            }
            return null;
        }

        private void InformPrgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _services.ProgressReaders)
            {
                progressReader.LoadProgress(this);
            }
        }


        public void SaveProgressAllService()
        {
            foreach (ISaveProgressWriter writer in _services.ProgressWirters)
            {
                writer.WriteProgress(this);
            }
            _saveSystem.Save(_progress);
        }

        public void SaveProgress(ISaveProgressWriter progressWriter)
        {
            progressWriter.WriteProgress(this);
            _saveSystem.Save(_progress);
        }


        public void DeleteSave()
        {
            _saveSystem.DeleteSave();
        }


        public void SetData<T>(string ID, T data) where T : SaveData
        {
            data.Version = Application.version;
            _progress.SetData(ID, data);
            _saveSystem.Save(_progress);
        }

        public bool GetSaveData<T>(string ID, out T data) where T : SaveData
        {
            return _progress.GetSaveData(ID, out data);
        }
    }
}