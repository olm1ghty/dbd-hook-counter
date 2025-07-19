using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using DBD_Hook_Counter;
using Svg;
using Properties = DBD_Hook_Counter.Properties;
using Color = System.Drawing.Color;

public class TransparentOverlayForm : Form
{
    public readonly ScreenChecker screenChecker;
    public readonly TimerManager timerManager;
    public readonly GameStateManager gameManager;
    public readonly SurvivorManager survivorManager;
    public readonly Scaler scaler;
    public readonly ToastManager toastManager;
    public readonly HotKeyManager hotkeyManager;
    public readonly HotKeyActions hotkeyActions;
    public readonly OverlayRenderer overlayRenderer;
    private readonly KeyboardWatcher keyboardWatcher;
    public OverlaySettingsForm settingsForm;
    public OverlayKeybindsForm keybindsForm;

    public TransparentOverlayForm()
    {
        scaler = new();
        timerManager = new(this);

        hotkeyActions = new HotKeyActions(this);
        hotkeyManager = new HotKeyManager(this, hotkeyActions);

        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        Rectangle screen = Screen.PrimaryScreen.Bounds;
        this.Bounds = screen;
        this.Text = "DBD Hook Counter";
        this.Icon = Properties.Resources.dbd;
        StartPosition = FormStartPosition.CenterScreen;

        // Use WS_EX_LAYERED to enable per-pixel alpha
        int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT);

        screenChecker = new(this);
        survivorManager = new(this);
        gameManager = new(this);
        toastManager = new(this);
        overlayRenderer = new(this);

        keyboardWatcher = new KeyboardWatcher();
        keyboardWatcher.AltTabPressed += () => gameManager.TemporaryPause();
        keyboardWatcher.EscPressed += () => gameManager.TemporaryPause();

        toastManager.ShowToast("Hook counter online");
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_LAYERED     
                       | NativeMethods.WS_EX_TRANSPARENT  
                       | NativeMethods.WS_EX_NOACTIVATE; 
            return cp;
        }
    }

    public void EnableInput(bool enable)
    {
        int style = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        if (enable)
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, style & ~NativeMethods.WS_EX_TRANSPARENT);
        else
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, style | NativeMethods.WS_EX_TRANSPARENT);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        keyboardWatcher.Dispose();   // stop background thread
        base.OnFormClosed(e);
    }
}
