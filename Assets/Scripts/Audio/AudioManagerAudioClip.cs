using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
	[CreateAssetMenu]
    public class AudioManagerAudioClip : ScriptableObject
    {
        [FormerlySerializedAs("AudioClip")] public AudioClip audioClip;
        [FormerlySerializedAs("Volume")] public float volume = 1;
        [FormerlySerializedAs("Pitch")] public float pitch = 1;
		[FormerlySerializedAs("VolumeIncrementAmount")] public float volumeIncrementAmount = 0.1f;
		[FormerlySerializedAs("Loop")] public bool loop = false;
	}
}