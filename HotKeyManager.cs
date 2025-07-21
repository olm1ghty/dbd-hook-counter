using System.Runtime.InteropServices;

public sealed class HotKeyManager : IDisposable
{
    public record HotKey(uint Mod, uint Vk, Action Action);

    private const int WM_HOTKEY = 0x0312;

    private readonly Form host;
    private readonly Dictionary<int, Action> hotkeys = new();
    private readonly List<HotKey> list = new();
    private int nextId = 1;

    private const int VK_ESCAPE = 0x1B;

    public HotKeyManager(Form host, HotKeyActions actions)
    {
        Add(MOD_SHIFT, (uint)Keys.M, actions.ShowSettings);
        Add(MOD_SHIFT, (uint)Keys.H, actions.ShowKeybinds);
        Add(MOD_SHIFT, (uint)Keys.K, actions.Exit);
        Add(MOD_SHIFT, (uint)Keys.P, actions.TriggerPause);
        Add(MOD_SHIFT, (uint)Keys.R, actions.Restart);

        Add(0, (uint)Keys.D1, () => actions.AddHookStage(0));
        Add(0, (uint)Keys.D2, () => actions.AddHookStage(1));
        Add(0, (uint)Keys.D3, () => actions.AddHookStage(2));
        Add(0, (uint)Keys.D4, () => actions.AddHookStage(3));
        Add(0, (uint)Keys.D5, actions.ResetHookStages);

        Add(MOD_SHIFT, (uint)Keys.D1, () => actions.TriggerTimerManually(0));
        Add(MOD_SHIFT, (uint)Keys.D2, () => actions.TriggerTimerManually(1));
        Add(MOD_SHIFT, (uint)Keys.D3, () => actions.TriggerTimerManually(2));
        Add(MOD_SHIFT, (uint)Keys.D4, () => actions.TriggerTimerManually(3));
        Add(MOD_SHIFT, (uint)Keys.D5, actions.ClearAllTimers);

        this.host = host ?? throw new ArgumentNullException(nameof(host));

        host.HandleCreated += (_, __) => RegisterAll();
        host.HandleDestroyed += (_, __) => UnregisterAll();
        host.Disposed += (_, __) => Dispose();
        Application.AddMessageFilter(new Filter(this));
    }

    public void Add(uint modifier, uint key, Action action)
    {
        int id = ((int)modifier << 16) | (int)key;
        hotkeys[id] = action;
        list.Add(new HotKey(modifier, key, action));
    }

    public void RegisterAll()
    {
        foreach (var hk in list)
        {
            _ = RegisterHotKey(host.Handle, nextId++, hk.Mod, hk.Vk);
        }
    }

    public void UnregisterAll()
    {
        for (int id = 1; id < nextId; id++)
            UnregisterHotKey(host.Handle, id);
        nextId = 1;
    }

    public void Dispose() => UnregisterAll();

    // ---------------- message filter -----------------
    private class Filter : IMessageFilter
    {
        private readonly HotKeyManager mgr;
        public Filter(HotKeyManager mgr) => this.mgr = mgr;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int index = m.WParam.ToInt32() - 1;
                if (index >= 0 && index < mgr.list.Count)
                    mgr.list[index].Action();
                return true;
            }
            return false;
        }
    }

    public bool IsEscPressedThisFrame()
    {
        // 0x8000 bit = key is currently down
        return (GetAsyncKeyState(VK_ESCAPE) & 0x8000) != 0;
    }

    // -------------- Win32 --------------
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    // modifier flags
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CTRL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;
}
