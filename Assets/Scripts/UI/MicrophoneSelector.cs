using System.Linq;
using TMPro;
using UnityEngine;
using VContainer;
using Whisper.Utils;

public class MicrophoneSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    [Inject] private MicrophoneRecord _microphoneRecord;

    private void Start()
    {
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_microphoneRecord.AvailableMicDevices.Prepend(_microphoneRecord.microphoneDefaultLabel).ToList());
    }

    public void OnSelectChanged()
    {
        if (_microphoneRecord.IsRecording)
            _microphoneRecord.StopRecord();

        _microphoneRecord.SelectedMicDevice = _dropdown.options[_dropdown.value].text;

        if (!_microphoneRecord.IsRecording)
            _microphoneRecord.StartRecord();
    }
}
