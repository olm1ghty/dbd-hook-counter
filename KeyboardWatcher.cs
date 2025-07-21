using System.Runtime.InteropServices;

public sealed class KeyboardWatcher : IDisposable
{
    public event Action EscPressed;
    public event Action AltTabPressed;
    public event Action WinKeyPressed;       // 🔔 New event

    private readonly Thread thread;
    private volatile bool running = true;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const int VK_ESCAPE = 0x1B;
    private const int VK_MENU = 0x12;        // Alt
    private const int VK_TAB = 0x09;         // Tab
    private const int VK_LWIN = 0x5B;        // Left Win
    private const int VK_RWIN = 0x5C;        // Right Win

    public KeyboardWatcher()
    {
        thread = new Thread(Loop) { IsBackground = true };
        thread.Start();
    }

    private void Loop()
    {
        bool escPrev = false;
        bool tabPrev = false;
        bool winPrev = false;

        while (running)
        {
            bool escDown = (GetAsyncKeyState(VK_ESCAPE) & 0x8000) != 0;
            bool tabDown = (GetAsyncKeyState(VK_TAB) & 0x8000) != 0;
            bool altDown = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;

            bool lWinDown = (GetAsyncKeyState(VK_LWIN) & 0x8000) != 0;
            bool rWinDown = (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0;
            bool winDown = lWinDown || rWinDown;

            // Esc rising edge
            if (escDown && !escPrev)
                EscPressed?.Invoke();

            // Alt+Tab rising edge
            if (tabDown && !tabPrev && altDown)
                AltTabPressed?.Invoke();

            // Win key rising edge
            if (winDown && !winPrev)
                WinKeyPressed?.Invoke();

            escPrev = escDown;
            tabPrev = tabDown;
            winPrev = winDown;

            Thread.Sleep(5); // ~200 Hz
        }
    }

    public void Dispose()
    {
        running = false;
        thread.Join();
    }
}
