using System.Runtime.InteropServices;

public sealed class KeyboardWatcher : IDisposable
{
    public event Action EscPressed;
    public event Action AltTabPressed;          // 🔔 new event

    private readonly Thread thread;
    private volatile bool running = true;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const int VK_ESCAPE = 0x1B;
    private const int VK_MENU = 0x12;          // Alt
    private const int VK_TAB = 0x09;          // Tab

    public KeyboardWatcher()
    {
        thread = new Thread(Loop) { IsBackground = true };
        thread.Start();
    }

    private void Loop()
    {
        bool escPrev = false;
        bool tabPrev = false;

        while (running)
        {
            // 0x8000 bit means "key currently down"
            bool escDown = (GetAsyncKeyState(VK_ESCAPE) & 0x8000) != 0;
            bool tabDown = (GetAsyncKeyState(VK_TAB) & 0x8000) != 0;
            bool altDown = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

            // Esc rising edge
            if (escDown && !escPrev)
                EscPressed?.Invoke();

            // Tab rising edge *while* Alt held
            if (tabDown && !tabPrev && altDown)
                AltTabPressed?.Invoke();

            escPrev = escDown;
            tabPrev = tabDown;

            Thread.Sleep(5); // 200 Hz poll
        }
    }

    public void Dispose()
    {
        running = false;
        thread.Join();
    }
}
