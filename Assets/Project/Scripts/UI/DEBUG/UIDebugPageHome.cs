using TMPro;
using UnityEngine;

namespace DebugGame.UI
{
    public class UIDebugPageHome : UIDebugPage
    {
        public override DebugPage ID => DebugPage.Home;
        [SerializeField] private TMP_Text _configName;

        protected override void _Init()
        {
            _configName.SetText(DebugService.LastConfigSelection);
        }

        public void OnOpenPage(DebugPage page)
        {
            UIDebug.OpenPage(page);
        }

        public void OnOpenGeneral()
        {
            OnOpenPage(DebugPage.General);
        }

        public void OnOpenLevel()
        {
            OnOpenPage(DebugPage.Level);
        }



        public void OnGoSelectionConfig()
        {
            DebugService.GoSelectionConfig();
            UIDebug.CloseAllPage();
            UIDebug.Close();
        }

        public void OnResetSetting()
        {
            DebugService.ResetSetting();
            UIDebug.ReInit();
        }

        public void OnResetSaveData()
        {
            DebugService.ResetSaveData();
            OnGoSelectionConfig();
        }
    }
}