using DBDtimer.Properties;

public class OverlayKeybindsForm : Form
{
    TransparentOverlayForm form;

    public OverlayKeybindsForm(TransparentOverlayForm form)
    {
        this.form = form;

        // Window style
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Text = "Hot keys";
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;

        var labelMargin = new Padding(0, 10, 0, 0);

        // Layout
        var layout = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(16),
            BackColor = Color.FromArgb(30, 30, 30),
            Margin = new Padding(0)
        };

        // Keybinds dictionary - update these as needed
        var keybinds = new Dictionary<string, string>
        {
            { "Shift + H", "Hot keys" },
            { "Shift + M", "Settings" },
            { "Shift + K", "Exit the app" },
            { "Shift + R", "Restart the app" },
            { "Shift + P", "Pause/Unpause the app (if playing survivor, for example)" },
            { "1/2/3/4", "Manually add a hook stage. If the hook counter is at 2, sets it to 0." },
            { "5", "Clear all hook stages" },
            { "Shift + 1/2/3/4", "Manually trigger unhook timers" },
            { "Shift + 5", "Clear all timers" }
        };

        foreach (var kvp in keybinds)
        {
            var label = new Label
            {
                Text = $"{kvp.Key}:  {kvp.Value}",
                AutoSize = true,
                Margin = labelMargin,
                ForeColor = Color.White
            };
            layout.Controls.Add(label);
        }

        // Close button
        var closeBtn = new Button
        {
            Text = "Close",
            AutoSize = true,
            BackColor = Color.FromArgb(45, 140, 240),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 16, 0, 0),
            Padding = new Padding(8, 4, 8, 4),
            Cursor = Cursors.Hand
        };
        closeBtn.FlatAppearance.BorderSize = 0;
        closeBtn.Click += (_, __) => Close();

        layout.Controls.Add(closeBtn);
        Controls.Add(layout);
    }

    protected override void OnDeactivate(EventArgs e)
    {
        form.hotkeyActions.UnpauseApp();
        base.OnDeactivate(e);
        this.Close();
    }
}
