using System.Globalization;
using DG.Tweening;
using IngameDebugConsole;
using Services;
using Tayx.Graphy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugGame.UI
{
    public class UIDebugPageGeneral : UIDebugPage
    {
        public override DebugPage ID => DebugPage.General;

        [SerializeField] private Toggle _fps;
        [SerializeField] private Toggle _hud;
        [SerializeField] private GraphyManager _graphyFPS;
        [SerializeField] private Button _buttonFpsPosition;
        [SerializeField] private TMP_Text _fpsPosition;

        [Space,SerializeField] private Toggle _console;
        [SerializeField] private DebugLogManager _consoleManager;

        [Space, SerializeField] private Toggle _hand;
        [SerializeField] private Texture2D _handTexture;
        
        private WindowService _windowService => AllServices.Container.Single<WindowService>();
        private HandCursorWindow _handCursorWindow => _windowService.GetOrCreateWindow<HandCursorWindow>(WindowId.HandCursor);

        protected override void _Init()
        {
            _fps.isOn = DebugService.FPS_Enable;
            _hud.isOn = DebugService.HUD_Enable;
            _fpsPosition.SetText(DebugService.FPS_Position.ToString());
            _console.isOn = DebugService.Console_Enable;
            _hand.isOn = DebugService.Hand_Cursor_Enable;
        }

        public void FPS_ChangeValue(bool value)
        {
            DebugService.FPS_Enable = value;
            _graphyFPS.gameObject.SetActive(value);

            if (value)
            {
                DOTween.Sequence()
                    .AppendInterval(0.2f)
                    .AppendCallback(() =>
                    {
                        _graphyFPS.GraphModulePosition = DebugService.FPS_Position;
                    });
            }

            DebugService.OnSaveChange();
        }
        
        public void HUD_ChangeValue(bool value)
        {
            DebugService.HUD_Enable = value;
            AllServices.Container.Single<WindowService>().GetOrCreateWindow<HUDWindow>(WindowId.HUD).GetComponent<CanvasGroup>().alpha = value ? 1 : 0;

            DebugService.OnSaveChange();
        }
        
        public void UpdatePositionFPS()
        {
            ref var pos = ref DebugService.FPS_Position;
            pos++;
            if (pos == GraphyManager.ModulePosition.FREE)
                pos = GraphyManager.ModulePosition.TOP_RIGHT;

            if (DebugService.FPS_Enable)
                _graphyFPS.GraphModulePosition = pos;
            _fpsPosition.SetText(pos.ToString());

            DebugService.OnSaveChange();
        }

        public void Console_ChangeValue(bool value)
        {
            DebugService.Console_Enable = value;
            _consoleManager.gameObject.SetActive(value);

            DebugService.OnSaveChange();
        }
        
        public void Hand_ChangeValue(bool value)
        {
            DebugService.Hand_Cursor_Enable = value;

            if (value) _windowService.Open(WindowId.HandCursor);
            else _windowService.Close(WindowId.HandCursor);

            /*
            if(value)
                Cursor.SetCursor(_handTexture, new Vector2(0.4f * _handTexture.width, 0.2f * _handTexture.height), CursorMode.ForceSoftware);
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            */

            DebugService.OnSaveChange();
        }

        public void Hand_ScaleChangeValue(string stringValue)
        {
            if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
            {
                Hand_ScaleChangeValue(floatValue);
            }
        }

        public void Hand_ScaleChangeValue(float value)
        {
            _handCursorWindow.SetHandScale(value);
        }

        public void Hand_ClickAnimation(bool value)
        {
            _handCursorWindow.SetHandClickAnimation(value);
        }
    }
}