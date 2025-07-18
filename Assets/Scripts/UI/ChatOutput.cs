using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VContainer;

public class ChatOutput : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [Inject] private Agent _agent;

    private void Start()
    {
        _agent.OnChatUpdate += OnChatUpdate;
    }

    private void OnChatUpdate(IList<LLMUnity.ChatMessage> chat)
    {
        _text.text = string.Join(Environment.NewLine, chat
            .Where(t => t.role != "system")
            .Reverse()
            .Select(t => $"{t.role}: {t.content}"));
    }
}
