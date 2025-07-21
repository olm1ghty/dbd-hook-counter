using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBD_Hook_Counter
{
    public class ToastManager
    {
        TransparentOverlayForm mainForm;
        public readonly List<ToastMessage> toasts;

        public ToastManager(TransparentOverlayForm mainForm)
        {
            toasts = new();
            this.mainForm = mainForm;
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

            Rectangle scr = Screen.PrimaryScreen.Bounds;
            Point pos = new(
                scr.X + (scr.Width - size.Width) / 2,
                scr.Y + 50
            );

            var toast = new ToastForm(text, pos);
            toast.Show();
        }
    }
}
