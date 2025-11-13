namespace Infrastructure
{
    public interface IInputListener
    {
        public void OnClickDown();
        public void OnDrag();
        public void OnClickUp();
        public void OnCancelInput();
    }
}