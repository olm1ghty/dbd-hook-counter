using System.Runtime.InteropServices;
using System.Windows.Forms;

public sealed class HotKeyManager : IDisposable
{
    public record HotKey(uint Mod, uint Vk, Action Action);

    private const int WM_HOTKEY = 0x0312;

    private readonly Form host;
    private readonly List<HotKey> list = new();
    private int nextId = 1;                       // unique id per key

    public HotKeyManager(Form host,
                         Action showMenu,
                         Action exit,
                         Action pause)
    {
        this.host = host ?? throw new ArgumentNullException(nameof(host));

        // ---------- DEFINE ALL HOT‑KEYS HERE ----------
        list.Add(new HotKey(MOD_SHIFT, (uint)Keys.M, showMenu)); // Shift+M
        list.Add(new HotKey(MOD_SHIFT, (uint)Keys.K, exit));     // Shift+K
        list.Add(new HotKey(MOD_SHIFT, (uint)Keys.P, pause));    // Shift+P
        // add more here…
        // ---------------------------------------------

        host.HandleCreated += (_, __) => RegisterAll();
        host.HandleDestroyed += (_, __) => UnregisterAll();
        host.Disposed += (_, __) => Dispose();
        Application.AddMessageFilter(new Filter(this));
    }

    public void Add(uint modifiers, uint vk, Action action)
        => list.Add(new HotKey(modifiers, vk, action));

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
                int index = m.WParam.ToInt32() - 1; // ids start at 1
                if (index >= 0 && index < mgr.list.Count)
                    mgr.list[index].Action();
                return true;                        // swallow
            }
            return false;
        }
    }

    // -------------- Win32 --------------
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // modifier flags
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CTRL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;
}
