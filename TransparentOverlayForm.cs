using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using DBDtimer;
using Svg;
using Properties = DBDtimer.Properties;
using Color = System.Drawing.Color;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Globalization;

public class TransparentOverlayForm : Form
{
    public ScreenChecker screenChecker;
    public TimerManager timerManager;
    public GameStateManager gameManager;
    public SurvivorManager survivorManager;
    public Scaler scaler;
    public ToastManager toastManager;

    public Survivor[] survivors = new Survivor[4];

    public int hookStageCounterStartX = 264;
    public int hookStageCounterStartY = 650;
    public int hookStageCounterOffset = 120;

    SvgDocument hookCounterSVG;
    Graphics graphics;
    Bitmap bmp;

    float hookSVGscaleX;
    float hookSVGscaleY;

    private OverlayMenuForm menuForm;
    private const int HOTKEY_ID = 1;          // choose key id as before

    private const int WM_HOTKEY = 0x0312;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_SHIFT = 0x0004;
    private readonly List<HotKey> hotKeys;
    private bool overlayVisible = false;

    // A compact record that holds everything we need
    record HotKey(int Id, uint Mod, uint Vk, Action Action);

    private readonly List<ToastMessage> toasts = new();

    public TransparentOverlayForm()
    {
        hotKeys = new List<HotKey>
        {
            new HotKey(1, MOD_SHIFT, (uint)Keys.M, ShowMenu), // method group
            new HotKey(2, MOD_SHIFT, (uint)Keys.K, Exit),
            new HotKey(3, MOD_SHIFT, (uint)Keys.P, TriggerPause)
        };

        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        Rectangle screen = Screen.PrimaryScreen.Bounds;
        this.Bounds = screen;
        this.Text = "DBD Hook Counter";
        this.Icon = Properties.Resources.dbd;
        StartPosition = FormStartPosition.CenterScreen;

        using (var svgStream = new MemoryStream(Properties.Resources.hooks))
        {
            hookCounterSVG = SvgDocument.Open<SvgDocument>(svgStream);
        }

        // Use WS_EX_LAYERED to enable per-pixel alpha
        int initialStyle = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, initialStyle | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT);


        scaler = new ();

        hookStageCounterStartX = scaler.Scale(hookStageCounterStartX);
        hookStageCounterStartY = scaler.ScaleY(hookStageCounterStartY);
        hookStageCounterOffset = scaler.Scale(hookStageCounterOffset);

        survivors = new Survivor[]
        {
            new Survivor(0, this),
            new Survivor(1, this),
            new Survivor(2, this),
            new Survivor(3, this)
        };

        screenChecker = new(this);
        timerManager = new(this);
        gameManager = new(this);
        survivorManager = new(this);
        toastManager = new(this);

        bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        graphics = Graphics.FromImage(bmp);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        //hookCounterSVG.Height = new SvgUnit(SvgUnitType.Pixel, 30);

        float desiredWidth = 20;
        float desiredHeight = 38;
        float svgWidth = hookCounterSVG.Bounds.Width;
        float svgHeight = hookCounterSVG.Bounds.Height;
        hookSVGscaleX = desiredWidth / svgWidth;
        hookSVGscaleY = desiredHeight / svgHeight;

        hookCounterSVG.FillOpacity = 0.75f;
    }

    protected override bool ShowWithoutActivation => true;   // prevents focus when shown

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_LAYERED      // stays
                       | NativeMethods.WS_EX_TRANSPARENT  // stays
                       | NativeMethods.WS_EX_NOACTIVATE;  // new: never takes focus
            return cp;
        }
    }


    public void DrawOverlay()
    {
        // wipe everything that was drawn last time ⟵  IMPORTANT
        graphics.Clear(Color.Transparent);

        //if (screenChecker.UIenabled())
        {
            // --- draw hook stages ---
            for (int i = 0; i < survivors.Length; i++)
            {
                hookCounterSVG.Fill = new SvgColourServer(Color.Transparent);

                SvgElement leftHook = hookCounterSVG.GetElementById("leftHook");
                leftHook.Fill = new SvgColourServer(Color.Transparent);

                SvgElement rightHook = hookCounterSVG.GetElementById("rightHook");
                rightHook.Fill = new SvgColourServer(Color.Transparent);

                switch (survivors[i].hookStages)
                {
                    case 0:
                        leftHook.Fill = new SvgColourServer(Color.Black);
                        rightHook.Fill = new SvgColourServer(Color.Black);
                        break;

                    case 1:
                        leftHook.Fill = new SvgColourServer(Color.White);
                        rightHook.Fill = new SvgColourServer(Color.Black);
                        break;

                    case 2:
                        leftHook.Fill = new SvgColourServer(Color.White);
                        rightHook.Fill = new SvgColourServer(Color.White);
                        break;
                }

                var state = graphics.Save();
                graphics.TranslateTransform(hookStageCounterStartX, (hookStageCounterStartY) + (i * hookStageCounterOffset));
                graphics.ScaleTransform(hookSVGscaleX, hookSVGscaleY);
                hookCounterSVG.Draw(graphics);
                graphics.Restore(state);
            }

            // clean up expired timers before drawing
            timerManager.RemoveExpiredTimers();

            // draw all active timers
            foreach (var list in timerManager.timers)
            {
                foreach (var timer in list)
                {
                    string txt = timer.SecondsRemaining.ToString();
                    using (Font f = new Font("Arial", 12, System.Drawing.FontStyle.Bold))
                    using (Brush b = new SolidBrush(Color.Red))
                    {
                        graphics.DrawString(txt, f, b, timer.Position);
                    }
                }
            }
        }

        // ----- draw toasts -----
        toasts.RemoveAll(t => !t.IsAlive);          // drop finished ones

        foreach (var toast in toasts)
        {
            using var f = new Font("Arial", 16, FontStyle.Bold);
            int a = toast.CurrentAlpha;
            using var b = new SolidBrush(Color.FromArgb(a, Color.Yellow));
            graphics.DrawString(toast.Text, f, b, toast.Position);
        }

        NativeMethods.SetBitmapToForm(this, bmp);
    }

    public void ClearOverlay()
    {
        if (graphics != null
            && bmp != null)
        {
            graphics.Clear(Color.Transparent);
            NativeMethods.SetBitmapToForm(this, bmp);
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        foreach (var hk in hotKeys)
                RegisterHotKey(Handle, hk.Id, hk.Mod, hk.Vk);
    }
    
    protected override void OnHandleDestroyed(EventArgs e)
    {
        foreach (var hk in hotKeys)
            UnregisterHotKey(Handle, hk.Id);

        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();
            var hk = hotKeys.FirstOrDefault(h => h.Id == id);
            hk?.Action();                // run the mapped action
            return;                      // eat the message
        }
        base.WndProc(ref m);
    }

    void TriggerPause()
    {
        if (gameManager.screenMonitorTimer.Enabled)
        {
            gameManager.screenMonitorTimer.Stop();
            ClearOverlay();
            toastManager.ShowToast("App paused");
        }
        else
        {
            toastManager.ShowToast("App unpaused");
            gameManager.screenMonitorTimer.Start();
        }
    }

    void Exit()
    {
        Application.Exit();
    }

    private void ToggleOverlay()
    {
        overlayVisible = !overlayVisible;
        this.Visible = overlayVisible;
    }

    private void TakeScreenshot()
    {
        // your capture logic here
    }

    private void ShowMenu()
    {
        EnableInput(true);                    // overlay becomes clickable

        if (menuForm == null || menuForm.IsDisposed)
        {
            menuForm = new OverlayMenuForm(this);

            menuForm.FormClosed += (_, __) => {
                EnableInput(false);           // restore click‑through
            };
        }

        // position near mouse cursor
        Point p = Cursor.Position;
        menuForm.Location = p;
        menuForm.Show();
        menuForm.BringToFront();
    }

    private void EnableInput(bool enable)
    {
        int style = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
        if (enable)
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, style & ~NativeMethods.WS_EX_TRANSPARENT);
        else
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, style | NativeMethods.WS_EX_TRANSPARENT);
    }
}
