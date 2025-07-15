using ExCSS;
using System.Diagnostics;
using Properties = DBDtimer.Properties;

public class OverlayMenuForm : Form
{
    TransparentOverlayForm form;

    public OverlayMenuForm(TransparentOverlayForm form)
    {
        this.form = form;

        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Text = "Settings";

        // --- Aspect ratio dropdown -----------------
        var arLabel = new Label { Text = "Aspect ratio:", AutoSize = true };
        var arBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        arBox = AddItems(arBox, form.scaler.aspectRatios);
        arBox.SelectedIndex = Properties.Settings.Default.DropdownAR;
        arBox.SelectedIndexChanged += (_, __) =>
        {
            string value = arBox.SelectedItem!.ToString()!;
            form.scaler.aspectRatio = value;
        };

        // --- UI dropdown ----------------
        var uiLabel = new Label { Text = "UI scale (DBD settings):", AutoSize = true, Margin = new Padding(0, 8, 0, 0) };
        var uiBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        uiBox = AddItems(uiBox, form.scaler.MenuScales);
        uiBox.SelectedIndex = Properties.Settings.Default.DropdownUI;
        uiBox.SelectedIndexChanged += (_, __) =>
        {
            float value = int.Parse(uiBox.SelectedItem!.ToString()!) / 100f;
            form.scaler.MenuScale = value;
        };

        // --- In-game HUD dropdown ----------------
        var hudLabel = new Label { Text = "In-game HUD scale (DBD settings):", AutoSize = true, Margin = new Padding(0, 8, 0, 0) };
        var hudBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        hudBox = AddItems(hudBox, form.scaler.HUDscales);
        hudBox.SelectedIndex = Properties.Settings.Default.DropdownHUD;
        hudBox.SelectedIndexChanged += (_, __) =>
        {
            float value = int.Parse(hudBox.SelectedItem!.ToString()!) / 100f;
            form.scaler.HUDScale = value;
        };

        // --- DS checkbox ----------------
        var dsCheckBox = new CheckBox
        {
            Text = "DS timer",
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 0),
            Checked = form.timerManager.dsTimerEnabled
        };
        dsCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.dsTimerEnabled = dsCheckBox.Checked;
            Properties.Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
        };

        // --- Endurance checkbox ----------------
        var enduranceCheckBox = new CheckBox
        {
            Text = "Off-hook endurance timer",
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 0),
            Checked = form.timerManager.enduranceTimerEnabled
        };
        enduranceCheckBox.CheckedChanged += (_, __) =>
        {
            form.timerManager.enduranceTimerEnabled = enduranceCheckBox.Checked;
            Properties.Settings.Default.enduranceTimerEnabled = enduranceCheckBox.Checked;
        };

        // --- Close button ----------------
        var closeBtn = new Button
        {
            Text = "Save and restart the overlay",
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 0)
        };
        closeBtn.Click += (_, __) =>
        {
            // Save chosen values
            Properties.Settings.Default.AspectRatio = form.scaler.aspectRatio;
            Properties.Settings.Default.MenuScale = form.scaler.MenuScale;
            Properties.Settings.Default.HUDScale = form.scaler.HUDScale;

            Properties.Settings.Default.DropdownAR = arBox.SelectedIndex;
            Properties.Settings.Default.DropdownUI = uiBox.SelectedIndex;
            Properties.Settings.Default.DropdownHUD = hudBox.SelectedIndex;

            Properties.Settings.Default.dsTimerEnabled = dsCheckBox.Checked;
            Properties.Settings.Default.enduranceTimerEnabled = enduranceCheckBox.Checked;

            Properties.Settings.Default.Save();

            // Restart app
            Application.Restart();
            Environment.Exit(0); // ensures full termination
        };


        // --- Layout ------------------------
        var layout = new FlowLayoutPanel
        { 
            AutoSize = true, 
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(12)
        };
        layout.Controls.AddRange(new Control[] {
            arLabel,  arBox,
            uiLabel, uiBox,
            hudLabel, hudBox,
            dsCheckBox,
            enduranceCheckBox,
            closeBtn,
        });
        Controls.Add(layout);
    }

    ComboBox AddItems<T>(ComboBox box, List<T> list)
    {
        foreach (var item in list)
        {
            box.Items.Add(item);
        }
        return box;
    }

    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        this.Close();  // auto-close when user clicks outside
    }
}
