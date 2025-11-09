namespace DebugGame.UI
{
    public class UIDebugPageLevel : UIDebugPage
    {
        public override DebugPage ID => DebugPage.Level;

        public void OnWin()
        {
            DebugService.OnWinLevel();
            OnCloseDebug();
        }
        
        public void OnLose()
        {
            DebugService.OnLoseLevel();
            OnCloseDebug();
        }
    }
}