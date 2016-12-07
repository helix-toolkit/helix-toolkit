// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FpsCounter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class FpsCounter : INotifyPropertyChanged
    {
        /// <summary>
        /// Minimum FPS Update Duration, Unit = Milliseconds.
        /// </summary>
        private const int MinimumUpdateDuration = 200;
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
                if (value < TimeSpan.FromMilliseconds(MinimumUpdateDuration))
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
            m_frames.AddLast(ts.TotalMilliseconds);
            TrimFrames();
            UpdateValue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrimFrames()
        {
            if (m_frames.Count == 0)
            {
                return;
            }
            var sec = AveragingInterval.TotalMilliseconds;
            var target = m_frames.Last.Value - sec;
            while (m_frames.Count > 0 && target > m_frames.First.Value)
            {
                m_frames.RemoveFirst();
            }
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
                var dt = m_frames.Last.Value - m_frames.First.Value;
                Value = dt > MinimumUpdateDuration ? m_frames.Count / (dt/1000) : -1;
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
        private readonly LinkedList<double> m_frames = new LinkedList<double>();
    }
}