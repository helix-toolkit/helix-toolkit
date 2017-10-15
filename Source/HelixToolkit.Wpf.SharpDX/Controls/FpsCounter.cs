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
            if (!m_frames.Add(ts.TotalMilliseconds))
            {
                m_frames.RemoveFirst();
                m_frames.Add(ts.TotalMilliseconds);
            }
            //TrimFrames();
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
            var target = m_frames.Last - sec;           
            while (m_frames.Count > 10 && Math.Abs(m_frames.Last - m_frames.First) > MinimumUpdateDuration
                && (target > m_frames.First || m_frames.First > m_frames.Last || m_frames.IsFull())) //the second condition happened when switching tabs, the TotalMilliseconds reset to 0 from composite rendering
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
                var dt = m_frames.Last - m_frames.First;
                if(dt > MinimumUpdateDuration)
                    Value = m_frames.Count / (dt/1000);
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
        private readonly SimpleRingBuffer<double> m_frames = new SimpleRingBuffer<double>(250);
    }
}