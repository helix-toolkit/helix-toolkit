namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class FpsCounter : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AveragingInterval
        {
            get { return m_averagingInterval; }
            set
            {
                if (value == m_averagingInterval)
                    return;
                if (value < TimeSpan.FromSeconds(0.1))
                    throw new ArgumentOutOfRangeException();

                m_averagingInterval = value;
                OnPropertyChanged("AveragingInterval");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        public void AddFrame(TimeSpan ts)
        {
            var sec = AveragingInterval;
            var index = m_frames.FindLastIndex(aTS => ts - aTS > sec);
            if (index > -1)
                m_frames.RemoveRange(0, index);
            m_frames.Add(ts);

            UpdateValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            m_frames.Clear();
            UpdateValue();
        }

        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get { return m_value; }
            private set
            {
                if (value == m_value)
                    return;
                m_value = value;
                OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateValue()
        {
            if (m_frames.Count < 2)
            {
                Value = -1;
            }
            else
            {
                var dt = m_frames[m_frames.Count - 1] - m_frames[0];
                Value = dt.Ticks > 100 ? m_frames.Count / dt.TotalSeconds : -1;
            }
        }

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string name)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private double m_value;
        private TimeSpan m_averagingInterval = TimeSpan.FromSeconds(1);
        private List<TimeSpan> m_frames = new List<TimeSpan>();
    }
}
