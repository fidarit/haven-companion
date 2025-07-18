using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Whisper;
using Whisper.Utils;


public class STTWhisper : IAsyncStartable
{
    private readonly MicrophoneRecord _microphoneRecord;
    private readonly WhisperManager _whisper;

    public event OnStreamResultUpdatedDelegate OnResultUpdated;
    public event OnStreamSegmentFinishedDelegate OnSegmentFinished;

    [Inject]
    public STTWhisper(MicrophoneRecord microphoneRecord, WhisperManager whisper)
    {
        _microphoneRecord = microphoneRecord;
        _whisper = whisper;
    }

    async Awaitable IAsyncStartable.StartAsync(CancellationToken cancellation)
    {
        var stream = await _whisper.CreateStream(_microphoneRecord);
        stream.OnResultUpdated += OnResultUpdated;
        stream.OnSegmentFinished += OnSegmentFinished;
        stream.StartStream();
    }
}