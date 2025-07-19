using DBDtimer.Properties;
using ExCSS;
using Color = System.Drawing.Color;
using Properties = DBDtimer.Properties;

public class OverlaySettingsForm : Form
{
    TransparentOverlayForm form;

    public OverlaySettingsForm(TransparentOverlayForm form)
    {
        this.form = form;

        // Modern window style
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Text = "Overlay Settings";
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;

        // Style template
        var labelMargin = new Padding(0, 10, 0, 0);
        var comboStyle = ComboBoxStyle.DropDownList;

        // --- Aspect ratio dropdown
        var arLabel = new Label { Text = "Aspect Ratio", AutoSize = true, Margin = labelMargin };
        var arBox = CreateComboBox(form.scaler.aspectRatios, Properties.Settings.Default.DropdownAR);
        arBox.SelectedIndexChanged += (_, __) =>
        {
            form.scaler.aspectRatio = arBox.SelectedItem!.ToString()!;
        };

        // --- UI scale
        var uiLabel = new Label { Text = "UI Scale (in-game setting)", AutoSize = true, Margin = labelMargin };
        var uiBox = CreateComboBox(form.scaler.MenuScales, Properties.Settings.Default.DropdownUI);
        uiBox.SelectedIndexChanged += (_, __) =>
        {
            form.scaler.MenuScale = int.Parse(uiBox.SelectedItem!.ToString()!) / 100f;
        };

        // --- HUD scale
        var hudLabel = new Label { Text = "HUD Scale (in-game setting)", AutoSize = true, Margin = labelMargin };
        var hudBox = CreateComboBox(form.scaler.HUDscales, Properties.Settings.Default.DropdownHUD);
        hudBox.SelectedIndexChanged += (_, __) =>
        {
            form.scaler.HUDScale = int.Parse(hudBox.SelectedItem!.ToString()!) / 100f;
        };

        // --- Checkboxes
        var dsCheckBox = CreateCheckbox("Enable DS Timer", form.timerManager.dsTimerEnabled);
        dsCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.dsTimerEnabled = dsCheckBox.Checked;
            Properties.Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
        };

        var enduranceCheckBox = CreateCheckbox("Enable Endurance Timer", form.timerManager.enduranceTimerEnabled);
        enduranceCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.enduranceTimerEnabled = enduranceCheckBox.Checked;
            Properties.Settings.Default.enduranceTimerEnabled = enduranceCheckBox.Checked;
        };

        var manualCheckBox = CreateCheckbox("Manual Mode (disables auto-check)", Settings.Default.manualMode);
        manualCheckBox.CheckedChanged += (_, __) =>
        {
            form.gameManager.manualMode = manualCheckBox.Checked;
            Settings.Default.manualMode = manualCheckBox.Checked;
        };

        // --- Save + restart button
        var saveBtn = new Button
        {
            Text = "💾 Save and restart overlay",
            AutoSize = true,
            BackColor = Color.FromArgb(45, 140, 240),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 16, 0, 0),
            Padding = new Padding(8, 4, 8, 4)
        };
        saveBtn.FlatAppearance.BorderSize = 0;
        saveBtn.Cursor = Cursors.Hand;

        saveBtn.Click += (_, __) =>
        {
            Properties.Settings.Default.AspectRatio = form.scaler.aspectRatio;
            Properties.Settings.Default.MenuScale = form.scaler.MenuScale;
            Properties.Settings.Default.HUDScale = form.scaler.HUDScale;

            Properties.Settings.Default.DropdownAR = arBox.SelectedIndex;
            Properties.Settings.Default.DropdownUI = uiBox.SelectedIndex;
            Properties.Settings.Default.DropdownHUD = hudBox.SelectedIndex;
            Properties.Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
            Properties.Settings.Default.enduranceTimerEnabled = enduranceCheckBox.Checked;

            Properties.Settings.Default.Save();

            Application.Restart();
            Environment.Exit(0);
        };

        // --- Layout
        var layout = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(16),
            BackColor = Color.FromArgb(30, 30, 30),
            Margin = new Padding(0)
        };

        layout.Controls.AddRange(new Control[] {
            arLabel, arBox,
            uiLabel, uiBox,
            hudLabel, hudBox,
            dsCheckBox,
            enduranceCheckBox,
            manualCheckBox,
            saveBtn
        });

        Controls.Add(layout);
    }

    ComboBox CreateComboBox<T>(List<T> items, int selectedIndex)
    {
        var box = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 200,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White
        };
        foreach (var item in items)
            box.Items.Add(item);
        box.SelectedIndex = selectedIndex;
        return box;
    }

    CheckBox CreateCheckbox(string text, bool initial)
    {
        return new CheckBox
        {
            Text = text,
            AutoSize = true,
            Checked = initial,
            Margin = new Padding(0, 10, 0, 0),
            ForeColor = Color.White
        };
    }

    protected override void OnDeactivate(EventArgs e)
    {
        form.hotkeyActions.UnpauseApp();
        base.OnDeactivate(e);
        this.Close();
    }
}
