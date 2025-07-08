using System;
using System.Threading;


namespace UpkManager.Models
{

    public class UnrealLoadProgress
    {
        #region Private Fields

        private int current;
        private int lastUpdatePercent = -1;
        private readonly object locker = new();

        #endregion

        #region Properties

        public string Text { get; set; }

        public int Current
        {
            get => current;
            set => current = value;
        }

        public double Total { get; set; }
        public string StatusText { get; set; }
        public bool IsComplete { get; set; }
        public Action<UnrealLoadProgress> Progress { get; set; }

        #endregion

        #region Public Methods

        public void IncrementCurrent()
        {
            Interlocked.Increment(ref current);
            TryUpdate();
        }

        private void TryUpdate(int thresholdPercent = 1)
        {
            if (Total <= 100 || Progress == null)
                return;

            int percent = (int)(Current * 100 / Total);
            bool shouldUpdate = false;

            lock (locker)
            {
                if (percent >= lastUpdatePercent + thresholdPercent)
                {
                    lastUpdatePercent = percent;
                    shouldUpdate = true;
                }
            }

            if (shouldUpdate)
                Progress(this);
        }

        public void Complete()
        {
            IsComplete = true;
            Progress?.Invoke(this);
        }

        public void Update(string text = default)
        {
            if (text != default) Text = text;
            Progress?.Invoke(this);
        }

        #endregion
    }


}
