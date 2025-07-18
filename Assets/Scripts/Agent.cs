using LLMUnity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using Whisper;

public class Agent : MonoBehaviour
{
    [Inject] private LLMCharacter _character;
    [Inject] private STTWhisper _whisper;

    private readonly List<string> _history = new();
    private readonly List<string> _prepared = new();

    public event OnStreamResultUpdatedDelegate OnAnswer;
    public event Action<IList<ChatMessage>> OnChatUpdate;

    private void Start()
    {
        _whisper.OnSegmentFinished += OnSegmentFinished;

        BrainLoop();
    }

    private async Task BrainLoop()
    {
        while (true)
        {
            while (_prepared.Count == 0)
                await Task.Delay(300);

            var query = string.Join(' ', _prepared);
            _history.AddRange(_prepared);
            _prepared.Clear();

            Debug.LogWarning($"query: {query}");
            OnChatUpdate?.Invoke(_character.chat);

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
