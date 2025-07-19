using LLMUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using Whisper;

public class Agent : MonoBehaviour
{
    [Inject] private LLMCharacter _character;
    [Inject] private STTWhisper _whisper;

    private CancellationTokenSource _cancellationTokenSource;
    private string _lastSegment;

    private readonly List<string> _segments = new();
    private readonly object _lock = new(); 
    private readonly SemaphoreSlim _semaphore = new(0);

    public event OnStreamResultUpdatedDelegate OnAnswer;
    public event Action<IEnumerable<ChatMessage>> OnChatUpdate;

    private void Start()
    {
        _whisper.OnSegmentFinished += OnSegmentFinished;

        _cancellationTokenSource = new();
        _ = BrainLoop(_cancellationTokenSource.Token);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _semaphore.Dispose();
    }

    private async Task BrainLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_segments.Count == 0) 
                await _semaphore.WaitAsync(cancellationToken);

            string query;
            lock (_lock)
            {
                query = string.Join(' ', _segments);
                _segments.Clear();
            }

            Debug.Log($"query: {query}");
            OnChatUpdate?.Invoke(_character.chat.Append(new() { role = _character.playerName, content = query }));

            var result = await _character.Chat(query);

            Debug.Log($"answer: {result}");

            OnAnswer?.Invoke(result);
            OnChatUpdate?.Invoke(_character.chat);
        }
    }

    private void OnSegmentFinished(WhisperResult segment)
    {
        if (string.IsNullOrWhiteSpace(segment.Result))
            return;

        if (segment.Result == _lastSegment)
            return;

        lock (_lock)
            _segments.Add(segment.Result);

        _lastSegment = segment.Result; 
        _semaphore.Release();
    }
}
