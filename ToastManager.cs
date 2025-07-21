namespace DBD_Hook_Counter
{
    public class ToastManager
    {
        TransparentOverlayForm form;
        public readonly List<ToastMessage> toasts;

        public ToastManager(TransparentOverlayForm form)
        {
            toasts = new();
            this.form = form;
        }

        public void ShowToast(string text)
        {
            using var tempLabel = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Padding = new Padding(10)
            };

            // Measure size
            tempLabel.CreateControl(); // force handle creation
            Size size = tempLabel.PreferredSize;

            Rectangle scr = form.screen.Bounds;
            Point pos = new(
                scr.X + (scr.Width - size.Width) / 2,
                scr.Y + 50
            );

            var toast = new ToastForm(text, pos);
            toast.Show();
        }
    }
}
