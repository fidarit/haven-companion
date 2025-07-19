using TMPro;
using UnityEngine;
using VContainer;

public class MicrophoneSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;

    [Inject] private MicrophoneSettings _microphoneSettings;

    private void Start()
    {
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_microphoneSettings.AvailableDevices);

        var selectedDeviceIndex = _microphoneSettings.AvailableDevices.IndexOf(_microphoneSettings.SelectedDevice);
        if (selectedDeviceIndex != -1)
            _dropdown.value = selectedDeviceIndex;
    }

    public void OnSelectChanged()
    {
        _microphoneSettings.SelectedDevice = _dropdown.options[_dropdown.value].text;
    }
}
