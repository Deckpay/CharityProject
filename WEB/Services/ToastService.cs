namespace WEB.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;
        private System.Timers.Timer _timer = new();

        public void ShowToast(string message, string type = "success")
        {
            OnShow?.Invoke(message, type);
        }
    }
}
