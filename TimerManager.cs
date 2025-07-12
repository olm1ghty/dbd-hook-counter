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

        public List<TimerData>[] timers = new List<TimerData>[4];

        private int hook = 11;
        private int ds = 61;

        public int timerStartXoffset = 235;
        public int timerStartYoffset = -30;
        public int timerDistanceYoffset = 20;

        public TimerManager(TransparentOverlayForm form)
        {
            this.form = form;

            for (int i = 0; i < 4; i++)
            {
                timers[i] = new List<TimerData>();
            }
        }

        public void AddTimer(int seconds, int survivorIndex = -1)
        {
            timers[survivorIndex].Add(new TimerData(seconds, CalculatePosition(survivorIndex)));
        }

        Point CalculatePosition(int survivorIndex = -1)
        {
            int x = form.hookStageCounterStartX - timerStartXoffset;
            int y = form.hookStageCounterStartY + (form.hookStageCounterOffset * survivorIndex) - timerStartYoffset;

            if (survivorIndex == -1)
            {
                survivorIndex = 0;
            }

            if (timers[survivorIndex].Count > 0)
            {
                y += timerDistanceYoffset;
            }

            return new Point(x, y);
        }

        public void RemoveExpiredTimers()
        {
            foreach (var list in timers)
                list.RemoveAll(t => t.IsExpired);
            ArrangeTimers();
        }

        public IEnumerable<TimerData> AllTimers => timers.SelectMany(t => t);

        public void RemoveTimer(int survivorIndex = -1)
        {
            if (TimerExists(survivorIndex))
            {
                List<TimerData> timerList = timers[survivorIndex];

                if (survivorIndex == -1)
                {
                    timerList = timers[0];
                }

                ArrangeTimers();
                timerList.Clear();
            }
        }

        private bool TimerExists(int survivorIndex = -1)
        {
            bool exists = false;

            List<TimerData> timerList = timers[survivorIndex];

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

        //public void Timer_TimerCompleted(object sender, EventArgs e)
        //{
        //    TimerData timer = (TimerData)sender;
        //    timer.TimerCompleted -= Timer_TimerCompleted;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (timers[i].Remove(timer))
        //        {
        //            break;
        //        }
        //    }
        //    //form.Controls.Remove(timer);
        //    ArrangeTimers();
        //}

        public void ClearAllTimers()
        {
            foreach (var timerList in timers)
            {
                //foreach (var timer in timerList)
                //{
                //    timer.Stop();
                //    //form.Controls.Remove(timer);
                //}
                timerList.Clear();
            }
        }

        public void ArrangeTimers()
        {
            for (int i = 0; i < timers.Length; i++)
            {
                foreach (var timer in timers[i])
                {
                    int x = form.hookStageCounterStartX - timerStartXoffset;
                    int y = form.hookStageCounterStartY + (form.hookStageCounterOffset * i) - timerStartYoffset;

                    if (i == -1)
                    {
                        i = 0;
                    }

                    if (timers[i].Count > 1)
                    {
                        if (timers[i][0] == timer)
                        {
                            y -= timerDistanceYoffset;
                        }
                        else if (timers[i][1] == timer)
                        {
                            y += timerDistanceYoffset;
                        }
                    }

                    timer.Position = new Point(x, y);
                }
            }
        }

        public void TriggerTimer(int index)
        {
            RemoveTimer(index);
            AddTimer(ds, index);
            AddTimer(hook, index);
        }

        //public class TimerData : Control
        //{
        //    TransparentOverlayForm form;
        //    public int seconds;
        //    private System.Windows.Forms.Timer timer;

        //    public event EventHandler TimerCompleted;

        //    public TimerData(int seconds, TransparentOverlayForm form)
        //    {
        //        this.form = form;
        //        this.seconds = seconds;
        //        this.AutoSize = true;

        //        this.SetStyle(ControlStyles.SupportsTransparentBackColor |
        //          ControlStyles.UserPaint |
        //          ControlStyles.OptimizedDoubleBuffer |
        //          ControlStyles.AllPaintingInWmPaint, true);

        //        this.BackColor = Color.Transparent;


        //        // Set the size of the control based on the text size
        //        using (Graphics g = this.CreateGraphics())
        //        {
        //            SizeF textSize = g.MeasureString(seconds.ToString(), this.Font);
        //            this.Size = new Size((int)textSize.Width, (int)textSize.Height);
        //        }

        //        this.timer = new System.Windows.Forms.Timer();
        //        this.timer.Interval = 1000; // Timer interval set to 1 second
        //        this.timer.Tick += Timer_Tick;
        //    }

        //    public void Start()
        //    {
        //        timer.Start();
        //    }

        //    public void Stop()
        //    {
        //        timer.Stop();
        //    }

        //    private void Timer_Tick(object sender, EventArgs e)
        //    {
        //        seconds--;

        //        if (seconds <= 0)
        //        {
        //            timer.Stop();
        //            TimerCompleted?.Invoke(this, EventArgs.Empty);
        //        }
        //    }
        //}

        public class TimerData
        {
            public DateTime StartedAt { get; }
            public int DurationSeconds { get; }

            public Point Position { get; set; }

            public int SecondsRemaining =>
                Math.Max(0, DurationSeconds - (int)(DateTime.UtcNow - StartedAt).TotalSeconds);

            public bool IsExpired => SecondsRemaining <= 0;

            public TimerData(int duration, Point pos)
            {
                StartedAt = DateTime.UtcNow;
                DurationSeconds = duration;
                Position = pos;
            }
        }
    }
}
