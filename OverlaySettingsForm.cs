using DBD_Hook_Counter.Properties;
using Color = System.Drawing.Color;
using Properties = DBD_Hook_Counter.Properties;

public class OverlaySettingsForm : Form
{
    TransparentOverlayForm form;

    public OverlaySettingsForm(TransparentOverlayForm form)
    {
        this.form = form;

        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Text = "Overlay settings";
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;

        this.Shown += (s, e) =>
        {
            this.Activate();
            this.Focus();
            this.BringToFront();
        };

        // Style template
        var labelMargin = new Padding(0, 10, 0, 0);
        var comboStyle = ComboBoxStyle.DropDownList;

        // --- UI scale
        var uiLabel = new Label { Text = "UI scale (in-game setting)", AutoSize = true, Margin = labelMargin };
        var uiBox = CreateComboBox(form.scaler.MenuScales, Properties.Settings.Default.DropdownUI);
        uiBox.SelectedIndexChanged += (_, __) =>
        {
            form.scaler.MenuScale = int.Parse(uiBox.SelectedItem!.ToString()!) / 100f;
        };

        // --- HUD scale
        var hudLabel = new Label { Text = "HUD scale (in-game setting)", AutoSize = true, Margin = labelMargin };
        var hudBox = CreateComboBox(form.scaler.HUDscales, Properties.Settings.Default.DropdownHUD);
        hudBox.SelectedIndexChanged += (_, __) =>
        {
            form.scaler.HUDScale = int.Parse(hudBox.SelectedItem!.ToString()!) / 100f;
        };

        // --- Checkboxes
        var otrCheckBox = CreateCheckbox("OTR timer", form.timerManager.otrTimerEnabled);
        otrCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.otrTimerEnabled = otrCheckBox.Checked;
            Properties.Settings.Default.otrTimerEnabled = otrCheckBox.Checked;
        };

        var dsCheckBox = CreateCheckbox("DS timer", form.timerManager.dsTimerEnabled);
        dsCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.dsTimerEnabled = dsCheckBox.Checked;
            Properties.Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
        };

        var enduranceCheckBox = CreateCheckbox("Endurance timer", form.timerManager.enduranceTimerEnabled);
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

        // --- Timer font size
        var timerFontSizeLabel = new Label
        {
            Text = "Timer font size",
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 0)
        };

        var timerFontSizeTextBox = new TextBox
        {
            Width = 200,
            Text = Properties.Settings.Default.timerFontSize.ToString(),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        timerFontSizeTextBox.KeyPress += (sender, e) =>
         {
             if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
             {
                 e.Handled = true; // Block non-numeric input
             }
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
            Settings.Default.MenuScale = form.scaler.MenuScale;
            Settings.Default.HUDScale = form.scaler.HUDScale;

            Settings.Default.DropdownUI = uiBox.SelectedIndex;
            Settings.Default.DropdownHUD = hudBox.SelectedIndex;
            Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
            Settings.Default.enduranceTimerEnabled = enduranceCheckBox.Checked;
            Settings.Default.otrTimerEnabled = otrCheckBox.Checked;

            if (int.TryParse(timerFontSizeTextBox.Text, out int intValue))
            {
                Settings.Default.timerFontSize = intValue;
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
                return;
            }

            Settings.Default.Save();

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
            uiLabel, uiBox,
            hudLabel, hudBox,
            otrCheckBox,
            dsCheckBox,
            enduranceCheckBox,
            timerFontSizeLabel, timerFontSizeTextBox,
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
        form.hotkeyManager.ResumeHotkeys();
        form.hotkeyActions.UnpauseApp();
        base.OnDeactivate(e);
        this.Close();
    }
}
