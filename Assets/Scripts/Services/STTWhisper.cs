using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Whisper;
using Whisper.Utils;


public class STTWhisper : IAsyncStartable, IDisposable
{
    private readonly MicrophoneRecord _microphoneRecord;
    private readonly WhisperManager _whisper;

    public event OnStreamResultUpdatedDelegate OnResultUpdated;
    public event OnStreamSegmentFinishedDelegate OnSegmentFinished;

    private WhisperStream _stream;

    [Inject]
    public STTWhisper(MicrophoneRecord microphoneRecord, WhisperManager whisper)
    {
        _microphoneRecord = microphoneRecord;
        _whisper = whisper;
    }

    async Awaitable IAsyncStartable.StartAsync(CancellationToken cancellation)
    {
        _stream = await _whisper.CreateStream(_microphoneRecord);
        _stream.OnResultUpdated += OnResultUpdated;
        _stream.OnSegmentFinished += OnSegmentFinished;
        _stream.StartStream();
    }

    public void Dispose()
    {
        _stream?.StopStream();
    }
}