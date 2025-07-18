using TMPro;
using UnityEngine;
using VContainer;

public class WhisperOutput : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [Inject] private STTWhisper _whisper;

    private void Start()
    {
        _whisper.OnResultUpdated += OnResultUpdated;
    }

    private void OnResultUpdated(string updatedResult)
    {
        _text.text = updatedResult;
    }
}
