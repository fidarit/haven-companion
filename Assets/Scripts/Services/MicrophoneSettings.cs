using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;
using Whisper.Utils;

public class MicrophoneSettings : IStartable, IDisposable
{
    private readonly MicrophoneRecord _microphoneRecord;

    public List<string> AvailableDevices { get; }

    public string SelectedDevice
    {
        get => PlayerPrefs.GetString(nameof(SelectedDevice), _microphoneRecord.microphoneDefaultLabel); 
        set => SetDevice(value);
    }

    public MicrophoneSettings(MicrophoneRecord microphoneRecord)
    {
        _microphoneRecord = microphoneRecord;

        AvailableDevices = microphoneRecord.AvailableMicDevices
            .Prepend(microphoneRecord.microphoneDefaultLabel)
            .ToList();
    }

    public void Start()
    {
        if (_microphoneRecord.IsRecording)
            return;

        SetDevice(SelectedDevice);
    }

    public void Dispose()
    {
        _microphoneRecord.StopRecord();
    }

    private void SetDevice(string device)
    {
        if (string.IsNullOrWhiteSpace(device) || device == _microphoneRecord.microphoneDefaultLabel)
            device = null;

        PlayerPrefs.SetString(nameof(SelectedDevice), device);

        if (_microphoneRecord.IsRecording && _microphoneRecord.RecordStartMicDevice == device)
            return;

        _microphoneRecord.StopRecord();
        _microphoneRecord.SelectedMicDevice = device;
        _microphoneRecord.StartRecord();
    }
}
