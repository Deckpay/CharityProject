namespace WEB.Services
{
    /// <summary>
    /// Egyszerű toast értesítések kezelésére szolgáló service.
    /// </summary>
    public class ToastService
    {
        public event Action<string, string>? OnShow;

        public void ShowToast(string message, string type = "success")
        {
            OnShow?.Invoke(message, type);
        }
    }
}
