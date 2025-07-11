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
        public TimerManager(TransparentOverlayForm form)
        {
            this.form = form;
        }

        public void AddTimer(int seconds, int survivorIndex = -1)
        {
            //TimerControl timer = new TimerControl(seconds, this);
            //timer.TimerCompleted += Timer_TimerCompleted;

            //if (survivorIndex == -1)
            //{
            //    timers[0].Add(timer);
            //}
            //else
            //{
            //    timers[survivorIndex].Add(timer);
            //}

            //ArrangeTimers();
            //this.Controls.Add(timer);
            //timer.Start();
        }

        public void RemoveTimer(int survivorIndex = -1)
        {
            //if (TimerExists(survivorIndex))
            {
                //List<TimerControl> timerList = timers[survivorIndex];

                //if (survivorIndex == -1)
                //{
                //    timerList = timers[0];
                //}

                //foreach (var timer in timerList)
                //{
                //    timer.Stop();
                //    this.Controls.Remove(timer);
                //}
                //timerList.Clear();
            }
        }

        //private bool TimerExists(int survivorIndex = -1)
        //{
            //bool exists = false;

            //List<TimerControl> timerList = timers[survivorIndex];

            //if (survivorIndex == -1)
            //{
            //    timerList = timers[0];
            //}

            //if (timerList.Count > 0)
            //{
            //    exists = true;
            //}

            //return exists;
        //}

        public void Timer_TimerCompleted(object sender, EventArgs e)
        {
            //TimerControl timer = (TimerControl)sender;
            //timer.TimerCompleted -= Timer_TimerCompleted;
            //for (int i = 0; i < 4; i++)
            //{
            //    if (timers[i].Remove(timer))
            //    {
            //        break;
            //    }
            //}
            //this.Controls.Remove(timer);
            //ArrangeTimers();
        }

        public void ClearAllTimers()
        {
            //foreach (var timerList in timers)
            //{
            //    foreach (var timer in timerList)
            //    {
            //        timer.Stop();
            //        this.Controls.Remove(timer);
            //    }
            //    timerList.Clear();
            //}
        }

        public void ArrangeTimers()
        {
            //for (int i = 0; i < 4; i++)
            //{
            //    int y = hookStageCounters[i].Location.Y - timerStartYoffset;
            //    int x = hookStageCounters[i].Location.X - timerStartXoffset;

            //    foreach (var timer in timers[i])
            //    {
            //        timer.Location = new Point(x, y);
            //        y += timer.Height + timerDistanceYoffset;
            //    }
            //}
        }

        public void TriggerTimer(int index)
        {
            //RemoveTimer(index);
            //AddTimer(hook, index);
            //AddTimer(ds, index);
        }
    }
}
