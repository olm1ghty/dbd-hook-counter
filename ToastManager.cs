using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class ToastManager
    {
        TransparentOverlayForm mainForm;

        public ToastManager(TransparentOverlayForm mainForm)
        {
            this.mainForm = mainForm;
        }


        public void ShowToast(string text)
        {
            // example position: centered, 50 px from top of the primary screen
            Rectangle scr = Screen.PrimaryScreen.Bounds;
            Point pos = new Point(scr.X + (scr.Width - 300) / 2, scr.Y + 50);

            var toast = new ToastForm(text, pos);
            toast.Show();            // <-- this makes the form appear
        }
    }
}
