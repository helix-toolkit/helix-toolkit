using CommunityToolkit.Diagnostics;
using NAudio.Wave;
using System;

namespace Audio;

/// <summary>
/// Audio Graph
/// </summary>
internal sealed class AudioGraph : IDisposable
{
    private AudioCapture? capture;
    private AudioPlayback? playback;
    private readonly SampleAggregator aggregator;

    public event EventHandler CaptureComplete
    {
        add { capture!.CaptureComplete += value; }
        remove { capture!.CaptureComplete -= value; }
    }

    public event EventHandler<MaxSampleEventArgs> MaximumCalculated
    {
        add { aggregator.MaximumCalculated += value; }
        remove { aggregator.MaximumCalculated -= value; }
    }

    public event EventHandler<FftEventArgs> FftCalculated
    {
        add { aggregator.FftCalculated += value; }
        remove { aggregator.FftCalculated -= value; }
    }

    public AudioGraph()
    {
        playback = new AudioPlayback();
        playback.OnSample += OnSample;
        capture = new AudioCapture();
        capture.OnSample += OnSample;
        aggregator = new SampleAggregator(8)
        {
            NotificationCount = 100,
            PerformFFT = true
        };
    }

    void OnSample(object? sender, SampleEventArgs e)
    {
        aggregator.Add(e.Left);
    }

    public int NotificationsPerSecond
    {
        get { return aggregator.NotificationCount; }
        set { aggregator.NotificationCount = value; }
    }

    public double RecordVolume
    {
        get { return capture!.RecordVolume; }
        set { capture!.RecordVolume = value; }
    }

    public bool HasCapturedAudio { get; private set; }

    public void PlayFile(string fileName)
    {
        CancelCurrentOperation();
        playback?.Load(fileName);
        aggregator.NotificationCount = 882;
        playback?.Play();
    }

    private void CancelCurrentOperation()
    {
        playback?.Stop();
        capture?.Stop();
    }

    public void Stop()
    {
        CancelCurrentOperation();
    }

    public void SaveRecordedAudio(string fileName)
    {
        throw new NotImplementedException();
    }

    public void PlayCapturedAudio()
    {
        throw new NotImplementedException();
    }

    public void StartCapture(int captureSeconds)
    {
        aggregator.NotificationCount = 200;
        capture!.CaptureSeconds = captureSeconds;
        capture!.Capture(new WaveFormat(8000, 1));
    }

    public void Dispose()
    {
        capture?.Dispose();
        capture = null;
        playback?.Dispose();
        playback = null;
    }
}
