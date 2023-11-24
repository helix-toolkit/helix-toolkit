using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Windows;
using System.IO;

namespace Audio;

internal sealed partial class ControlPanelViewModel : ObservableObject, IDisposable
{
    private const string AppTitle = "Audio";
    private readonly ISpectrumAnalyser analyzer;
    private readonly AudioGraph audioGraph;
    private readonly IWaveFormRenderer? waveFormRenderer;

    [ObservableProperty]
    private string _title = "AudioDemo";

    [ObservableProperty]
    private int _captureSeconds;

    public ControlPanelViewModel(IWaveFormRenderer? waveFormRenderer, ISpectrumAnalyser analyzer)
    {
        this.waveFormRenderer = waveFormRenderer;
        this.analyzer = analyzer;
        audioGraph = new AudioGraph();
        audioGraph.CaptureComplete += AudioGraph_CaptureComplete;
        audioGraph.MaximumCalculated += AudioGraph_MaximumCalculated;
        audioGraph.FftCalculated += AudioGraph_FftCalculated;
        CaptureSeconds = 10;
        NotificationsPerSecond = 100;
    }

    public double RecordVolume
    {
        get { return audioGraph.RecordVolume; }
        set
        {
            if (audioGraph.RecordVolume != value)
            {
                audioGraph.RecordVolume = value;
                OnPropertyChanged(nameof(RecordVolume));
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
                OnPropertyChanged(nameof(NotificationsPerSecond));
            }
        }
    }

    public void Dispose()
    {
        audioGraph.Dispose();
    }

    private void AudioGraph_FftCalculated(object? sender, FftEventArgs e)
    {
        analyzer?.Update(e.Result);
    }

    private void AudioGraph_MaximumCalculated(object? sender, MaxSampleEventArgs e)
    {
        waveFormRenderer?.AddValue(e.MaxSample, e.MinSample);
    }

    private void AudioGraph_CaptureComplete(object? sender, EventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
    }

    [RelayCommand]
    private void PlayFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "All Supported Files (*.wav;*.mp3)|*.wav;*.mp3|All Files (*.*)|*.*"
        };
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

    [RelayCommand]
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

    private bool HasCapturedAudio()
    {
        return audioGraph.HasCapturedAudio;
    }

    [RelayCommand(CanExecute = nameof(HasCapturedAudio))]
    private void PlayCapturedAudio()
    {
        audioGraph.PlayCapturedAudio();
    }

    [RelayCommand]
    private void SaveCapturedAudio()
    {
        var saveFileDialog = new SaveFileDialog
        {
            DefaultExt = ".wav",
            Filter = "WAVE File (*.wav)|*.wav"
        };
        bool? result = saveFileDialog.ShowDialog();
        if (result.HasValue && result.Value)
        {
            audioGraph.SaveRecordedAudio(saveFileDialog.FileName);
        }
    }

    [RelayCommand]
    private void Stop()
    {
        audioGraph.Stop();
        Title = AppTitle;
    }
}
