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

    private readonly List<string> _history = new();
    private readonly List<string> _prepared = new();

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
    }

    private async Task BrainLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (_prepared.Count == 0)
                await Task.Delay(300, cancellationToken);

            var query = string.Join(' ', _prepared);
            _history.AddRange(_prepared);
            _prepared.Clear();

            Debug.LogWarning($"query: {query}");
            OnChatUpdate?.Invoke(_character.chat.Append(new() { role = _character.playerName, content = query }));

            var result = await _character.Chat(query);

            Debug.LogWarning($"answer: {result}");

            OnAnswer?.Invoke(result);
            OnChatUpdate?.Invoke(_character.chat);
        }
    }

    private void OnSegmentFinished(WhisperResult segment)
    {
        _prepared.Add(segment.Result);

        Debug.LogWarning(string.Join(' ', _prepared));
    }
}
