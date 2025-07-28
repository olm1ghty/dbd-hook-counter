namespace DBD_Hook_Counter
{
    public class TimerManager
    {
        TransparentOverlayForm form;

        public List<TimerData>[] timers = new List<TimerData>[4];

        private int unhookEndurance = 11;
        private int ds = 61;
        private int otr = 81;

        public int timerStartXoffset = -215;
        public int timerStartYoffset = -10;
        public int timerSpacingOffset = 34;

        public bool dsTimerEnabled;
        public bool enduranceTimerEnabled;
        public bool otrTimerEnabled;

        public TimerManager(TransparentOverlayForm form)
        {
            this.form = form;

            timerStartXoffset = form.scaler.ScaleOffsetX(timerStartXoffset);
            timerStartYoffset = form.scaler.ScaleOffsetX(timerStartYoffset);
            timerSpacingOffset = form.scaler.ScaleOffsetX(timerSpacingOffset);

            for (int i = 0; i < 4; i++)
            {
                timers[i] = new List<TimerData>();
            }

            dsTimerEnabled = Properties.Settings.Default.dsTimerEnabled;
            enduranceTimerEnabled = Properties.Settings.Default.enduranceTimerEnabled;
            otrTimerEnabled = Properties.Settings.Default.otrTimerEnabled;
        }

        public void AddTimer(int seconds, Color color, int survivorIndex = -1)
        {
            timers[survivorIndex].Add(new TimerData(seconds, CalculateNewTimerPosition(survivorIndex), color));
        }

        Point CalculateNewTimerPosition(int survivorIndex = -1)
        {
            int x = (int)(form.overlayRenderer.hookStageCounterStartX + timerStartXoffset);
            int y = (int)(form.overlayRenderer.hookStageCounterStartY + (form.overlayRenderer.hookStageCounterOffset * survivorIndex) + timerStartYoffset);

            if (survivorIndex == -1)
            {
                survivorIndex = 0;
            }

            if (timers[survivorIndex].Count > 0)
            {
                y += timerSpacingOffset * timers[survivorIndex].Count;
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
            int clusterCenterX = (int)(form.overlayRenderer.hookStageCounterStartX + timerStartXoffset);
            int clusterCenterY = (int)(form.overlayRenderer.hookStageCounterStartY + timerStartYoffset);
            float timerSpacing = timerSpacingOffset;

            for (int i = 0; i < timers.Length; i++)
            {
                List<TimerData> timerGroup = timers[i];
                int count = timerGroup.Count;

                for (int j = 0; j < count; j++)
                {
                    float offsetFromCenter = j - (count - 1) / 2.0f;

                    int x = clusterCenterX;
                    int y = (int)(clusterCenterY + form.overlayRenderer.hookStageCounterOffset * i + offsetFromCenter * timerSpacing);

                    timerGroup[j].Position = new Point(x, y);
                }
            }
        }

        public void TriggerTimer(int index)
        {
            RemoveTimer(index);

            if (otrTimerEnabled)
            {
                AddTimer(otr, Color.White, index);
            }
            if (dsTimerEnabled)
            {
                AddTimer(ds, Color.Red, index);
            }
            if (enduranceTimerEnabled)
            {
                AddTimer(unhookEndurance, Color.White, index);
            }
        }

        public void TriggerTimerManually(int index)
        {
            if (TimerExists(index))
            {
                RemoveTimer(index);
            }
            else
            {
                if (otrTimerEnabled)
                {
                    AddTimer(otr, Color.White, index);
                }
                if (dsTimerEnabled)
                {
                    AddTimer(ds, Color.Red, index);
                }
                if (enduranceTimerEnabled)
                {
                    AddTimer(unhookEndurance, Color.White, index);
                }
            }
        }
    }
}
