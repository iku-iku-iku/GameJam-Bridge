using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Sound Data", menuName = "Sound Data")]
    public class SoundSo : ScriptableObject
    {
        public AudioClip bgm;
        public AudioClip wind;
        public AudioClip put;
        public AudioClip withdraw;
        public AudioClip transport;
    }
}