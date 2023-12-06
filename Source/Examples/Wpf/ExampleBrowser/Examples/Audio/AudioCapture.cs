using NAudio.Wave;
using System.IO;
using System;
using CommunityToolkit.Diagnostics;

namespace Audio;

internal sealed class AudioCapture : IDisposable
{
    private IWaveIn? captureDevice;
    private MemoryStream? recordedStream;
    private WaveFileWriter? writer;
    private int maxCaptureBytes;

    public AudioCapture()
    {
        CaptureSeconds = 30;
    }

    public bool IsCapturing { get; set; }
    public int CaptureSeconds { get; set; }

    public event EventHandler<SampleEventArgs>? OnSample;
    public event EventHandler? CaptureComplete;

    public void Capture(WaveFormat captureFormat)
    {
        if (IsCapturing)
        {
            ThrowHelper.ThrowInvalidOperationException("Already Recording");
        }

        CreateCaptureStream(captureFormat);
        StartCapture(captureFormat);
    }

    private void StartCapture(WaveFormat captureFormat)
    {
        EnsureDeviceIsCreated();
        captureDevice!.WaveFormat = captureFormat;
        captureDevice!.StartRecording();
        IsCapturing = true;
    }

    private void CreateCaptureStream(WaveFormat captureFormat)
    {
        int maxSeconds = CaptureSeconds == 0 ? 30 : CaptureSeconds;
        int captureBytes = maxSeconds * captureFormat.AverageBytesPerSecond;
        this.maxCaptureBytes = CaptureSeconds == 0 ? 0 : captureBytes;
        recordedStream = new MemoryStream(captureBytes + 50);
        writer = new WaveFileWriter(new IgnoreDisposeStream(recordedStream), captureFormat);
    }

    private void EnsureDeviceIsCreated()
    {
        if (captureDevice == null)
        {
            captureDevice = new WaveIn();
            captureDevice.RecordingStopped += OnRecordingStopped;
            captureDevice.DataAvailable += OnDataAvailable;
        }
    }

    void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (!IsCapturing)
        {
            return;
        }
        // first save the audio
        byte[] buffer = e.Buffer;
        this.writer?.Write(buffer, 0, e.BytesRecorded);

        // now report each sample if necessary
        for (int index = 0; index < e.BytesRecorded; index += 2)
        {
            short sample = (short)((buffer[index + 1] << 8) | buffer[index + 0]);
            /* short sample2 = BitConverter.ToInt16(buffer, index);
            Debug.Assert(sample == sample2, "Oops"); */
            float sample32 = sample / 32768f;
            OnSample?.Invoke(this, new SampleEventArgs(sample32, 0));
        }

        // stop the recording if necessary
        if (maxCaptureBytes != 0 && recordedStream?.Length >= maxCaptureBytes)
        {
            Stop();
        }
    }

    public void CloseRecording()
    {
        captureDevice?.StopRecording();

        if (writer != null)
        {
            // this will fix up the data lengths in the recorded memory stream
            writer.Close();
            writer = null;
            recordedStream?.Seek(0, SeekOrigin.Begin);
            RaiseCaptureStopped();
        }
    }

    void OnRecordingStopped(object? sender, EventArgs e)
    {
        IsCapturing = false;
        CloseRecording();
        captureDevice?.Dispose();
        captureDevice = null;
    }

    public void Stop()
    {
        captureDevice?.StopRecording();
    }

    private void RaiseCaptureStopped()
    {
        CaptureComplete?.Invoke(this, EventArgs.Empty);
    }

    public double RecordVolume { get; set; }

    public void Dispose()
    {
        captureDevice?.Dispose();
        captureDevice = null;
    }
}
