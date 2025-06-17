namespace iOSEntryBug
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var thread = new Thread(() => {
                while (true)
                {
                    Thread.Sleep(1000);
                    GC.Collect();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                stackLayout.Add(new Entry());
            }
            await Task.Delay(500); // Let the entries render

            // Simulate busy main thread and give time to background the app
            Thread.Sleep(5000);

            // iOS allows this code to finish before backgrounding the app and pausing all threads
            for (int i = 100; i > 0; i--)
            {
                var entry = stackLayout[i] as Entry;
                stackLayout.Remove(entry);
                entry?.Handler?.DisconnectHandler();
            }

            // App will be backgrounded now, GC will run immediately when resumed and crash 
        }
    }
}
