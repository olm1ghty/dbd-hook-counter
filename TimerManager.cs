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

        public int timerStartXoffset = 185;
        public int timerStartYoffset = -20;
        public int timerDistanceYoffset = 20;

        public TimerManager(TransparentOverlayForm form)
        {
            this.form = form;

            timerStartXoffset = form.scaler.Scale(timerStartXoffset);
            timerStartYoffset = form.scaler.Scale(timerStartYoffset);
            timerDistanceYoffset = form.scaler.Scale(timerDistanceYoffset);

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
            int x = (int)(form.hookStageCounterStartX - timerStartXoffset);
            int y = (int)(form.hookStageCounterStartY + (form.hookStageCounterOffset * survivorIndex) - timerStartYoffset);

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

        public void ClearAllTimers()
        {
            foreach (var timerList in timers)
            {
                timerList.Clear();
            }
        }

        public void ArrangeTimers()
        {
            for (int i = 0; i < timers.Length; i++)
            {
                foreach (var timer in timers[i])
                {
                    int x = (int)(form.hookStageCounterStartX - timerStartXoffset);
                    int y = (int)(form.hookStageCounterStartY + (form.hookStageCounterOffset * i) - timerStartYoffset);

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
    }
}
