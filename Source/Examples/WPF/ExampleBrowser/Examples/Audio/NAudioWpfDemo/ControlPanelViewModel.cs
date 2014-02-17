// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlPanelViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace NAudioWpfDemo
{
    internal class ControlPanelViewModel : INotifyPropertyChanged, IDisposable
    {
        private const string AppTitle = "AudioDemo";
        private readonly ISpectrumAnalyser analyzer;
        private readonly AudioGraph audioGraph;
        private readonly IWaveFormRenderer waveFormRenderer;

        private string _title;
        private int captureSeconds;

        public ControlPanelViewModel(IWaveFormRenderer waveFormRenderer, ISpectrumAnalyser analyzer)
        {
            Title = "AudioDemo";
            this.waveFormRenderer = waveFormRenderer;
            this.analyzer = analyzer;
            audioGraph = new AudioGraph();
            audioGraph.CaptureComplete += audioGraph_CaptureComplete;
            audioGraph.MaximumCalculated += audioGraph_MaximumCalculated;
            audioGraph.FftCalculated += audioGraph_FftCalculated;
            captureSeconds = 10;
            NotificationsPerSecond = 100;

            PlayFileCommand = new RelayCommand(
                () => PlayFile(),
                () => true);
            CaptureCommand = new RelayCommand(
                () => Capture(),
                () => true);
            PlayCapturedAudioCommand = new RelayCommand(
                () => PlayCapturedAudio(),
                () => HasCapturedAudio());
            SaveCapturedAudioCommand = new RelayCommand(
                () => SaveCapturedAudio(),
                () => HasCapturedAudio());
            StopCommand = new RelayCommand(
                () => Stop(),
                () => true);
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChangedEvent("Title");
            }
        }

        public ICommand PlayFileCommand { get; private set; }
        public ICommand CaptureCommand { get; private set; }
        public ICommand PlayCapturedAudioCommand { get; private set; }
        public ICommand SaveCapturedAudioCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public int CaptureSeconds
        {
            get { return captureSeconds; }
            set
            {
                if (captureSeconds != value)
                {
                    captureSeconds = value;
                    RaisePropertyChangedEvent("CaptureSeconds");
                }
            }
        }

        public double RecordVolume
        {
            get { return audioGraph.RecordVolume; }
            set
            {
                if (audioGraph.RecordVolume != value)
                {
                    audioGraph.RecordVolume = value;
                    RaisePropertyChangedEvent("RecordVolume");
                }
            }
        }

        public int NotificationsPerSecond
        {
            get { return audioGraph.NotificationsPerSecond; }
            set
            {
                if (NotificationsPerSecond != value)
                {
                    audioGraph.NotificationsPerSecond = value;
                    RaisePropertyChangedEvent("NotificationsPerSecond");
                }
            }
        }

        public void Dispose()
        {
            audioGraph.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void audioGraph_FftCalculated(object sender, FftEventArgs e)
        {
            if (analyzer != null)
                analyzer.Update(e.Result);
        }

        private void audioGraph_MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            if (waveFormRenderer != null)
                waveFormRenderer.AddValue(e.MaxSample, e.MinSample);
        }

        private void audioGraph_CaptureComplete(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void PlayFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Supported Files (*.wav;*.mp3)|*.wav;*.mp3|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string file = openFileDialog.FileName;
                try
                {
                    audioGraph.PlayFile(file);
                    Title = string.Format("{0} - {1}", AppTitle, Path.GetFileNameWithoutExtension(file));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Title = AppTitle;
                }
            }
        }

        private void Capture()
        {
            // todo: return if already capturing
            try
            {
                Title = string.Format("{0} - Capturing", AppTitle);
                audioGraph.StartCapture(CaptureSeconds);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void PlayCapturedAudio()
        {
            audioGraph.PlayCapturedAudio();
        }

        private bool HasCapturedAudio()
        {
            return audioGraph.HasCapturedAudio;
        }

        private void SaveCapturedAudio()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".wav";
            saveFileDialog.Filter = "WAVE File (*.wav)|*.wav";
            bool? result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                audioGraph.SaveRecordedAudio(saveFileDialog.FileName);
            }
        }

        private void Stop()
        {
            audioGraph.Stop();
            Title = AppTitle;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}