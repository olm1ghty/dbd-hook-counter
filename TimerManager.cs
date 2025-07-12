using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDtimer
{
    public class TimerManager
    {
        TransparentOverlayForm form;

        public List<TimerControl>[] timers = new List<TimerControl>[4];

        private int hook = 11;
        private int ds = 61;

        private int timerStartXoffset = 235;
        private int timerStartYoffset = -5;
        private int timerDistanceYoffset = 10;

        public TimerManager(TransparentOverlayForm form)
        {
            this.form = form;

            // Initialize timer lists
            for (int i = 0; i < 4; i++)
            {
                timers[i] = new List<TimerControl>();
            }
        }

        public void AddTimer(int seconds, int survivorIndex = -1)
        {
            TimerControl timer = new TimerControl(seconds, form);
            timer.TimerCompleted += Timer_TimerCompleted;

            if (survivorIndex == -1)
            {
                timers[0].Add(timer);
            }
            else
            {
                timers[survivorIndex].Add(timer);
            }

            ArrangeTimers();
            form.Controls.Add(timer);
            timer.Start();
        }

        public void RemoveTimer(int survivorIndex = -1)
        {
            if (TimerExists(survivorIndex))
            {
                List<TimerControl> timerList = timers[survivorIndex];

                if (survivorIndex == -1)
                {
                    timerList = timers[0];
                }

                foreach (var timer in timerList)
                {
                    timer.Stop();
                    form.Controls.Remove(timer);
                }
                timerList.Clear();
            }
        }

        private bool TimerExists(int survivorIndex = -1)
        {
            bool exists = false;

            List<TimerControl> timerList = timers[survivorIndex];

            if (survivorIndex == -1)
            {
                timerList = timers[0];
            }

            if (timerList.Count > 0)
            {
                exists = true;
            }

            return exists;
        }

        public void Timer_TimerCompleted(object sender, EventArgs e)
        {
            TimerControl timer = (TimerControl)sender;
            timer.TimerCompleted -= Timer_TimerCompleted;
            for (int i = 0; i < 4; i++)
            {
                if (timers[i].Remove(timer))
                {
                    break;
                }
            }
            form.Controls.Remove(timer);
            ArrangeTimers();
        }

        public void ClearAllTimers()
        {
            foreach (var timerList in timers)
            {
                foreach (var timer in timerList)
                {
                    timer.Stop();
                    form.Controls.Remove(timer);
                }
                timerList.Clear();
            }
        }

        public void ArrangeTimers()
        {
            for (int i = 0; i < 4; i++)
            {
                int y = form.hookStageCounterStartY + (form.hookStageCounterOffset * i) - timerStartYoffset;
                int x = form.hookStageCounterStartX - timerStartXoffset;

                foreach (var timer in timers[i])
                {
                    timer.Location = new Point(x, y);
                    y += timer.Height + timerDistanceYoffset;
                }
            }
        }

        public void TriggerTimer(int index)
        {
            RemoveTimer(index);
            AddTimer(hook, index);
            AddTimer(ds, index);
        }

        public class TimerControl : Control
        {
            TransparentOverlayForm form;
            public int seconds;
            private System.Windows.Forms.Timer timer;

            public event EventHandler TimerCompleted;

            public TimerControl(int seconds, TransparentOverlayForm form)
            {
                this.form = form;
                this.seconds = seconds;
                this.AutoSize = true;

                this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                  ControlStyles.UserPaint |
                  ControlStyles.OptimizedDoubleBuffer |
                  ControlStyles.AllPaintingInWmPaint, true);

                this.BackColor = Color.Transparent;


                // Set the size of the control based on the text size
                using (Graphics g = this.CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(seconds.ToString(), this.Font);
                    this.Size = new Size((int)textSize.Width, (int)textSize.Height);
                }

                this.timer = new System.Windows.Forms.Timer();
                this.timer.Interval = 1000; // Timer interval set to 1 second
                this.timer.Tick += Timer_Tick;
            }

            public void Start()
            {
                timer.Start();
            }

            public void Stop()
            {
                timer.Stop();
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                seconds--;

                if (seconds <= 0)
                {
                    timer.Stop();
                    TimerCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
