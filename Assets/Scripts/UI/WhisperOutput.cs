using TMPro;
using UnityEngine;
using VContainer;
using Whisper;
using Whisper.Utils;

public class WhisperOutput : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [Inject] private MicrophoneRecord _microphoneRecord;
    [Inject] private WhisperManager _whisper;

    private async void Start()
    {
        _whisper.OnNewSegment += segment => _text.text += segment.Text;

        var stream = await _whisper.CreateStream(_microphoneRecord);
        stream.OnResultUpdated += Stream_OnResultUpdated;
        stream.StartStream();
    }

    private void Stream_OnResultUpdated(string updatedResult)
    {
        _text.text = updatedResult;
    }
}
