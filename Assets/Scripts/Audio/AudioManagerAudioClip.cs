using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
    [Serializable]
    public class AudioManagerAudioClip
    {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("AudioClip")] public AudioClip audioClip;
        [FormerlySerializedAs("Volume")] public float volume = 1;
        [FormerlySerializedAs("Pitch")] public float pitch = 1;
		[FormerlySerializedAs("VolumeIncrementAmount")] public float VolumeIncrementAmount = 0.1f;
	}
}