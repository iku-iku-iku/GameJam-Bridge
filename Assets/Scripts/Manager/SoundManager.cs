using System.Collections.Generic;
using Common;
using Data;
using UnityEngine;

namespace Manager
{
    public class SoundManager : UnitySingleton<SoundManager>
    {
        public List<AudioSource> sources = new List<AudioSource>();
        public SoundSo audioSo;
        private AudioSource _bgmSource;
        private const int SourceCount = 20;
        private const float BgmBaseVolume = 1f;
        private const float EffectBaseVolume = 0.6f;

        public static SoundSo AudioSo => Instance.audioSo;

        private float _effectVolume = 0.5f;

        private float EffectVolume
        {
            get => _effectVolume;
            set
            {
                _effectVolume = value;
                SetEffectVolume(value);
            }
        }


        // 实现单例自动加载到场景中
        [RuntimeInitializeOnLoadMethod]
        private static void Load()
        {
            _ = Instance;
        }

        private void Awake()
        {
            audioSo = Resources.Load<SoundSo>("Data/Sound Data");

            AttachEffectSources();
            AttachBGMSource();

            EffectVolume = 0.5f;
            SetBGMVolume(0.5f);

            DontDestroyOnLoad(gameObject);
        }

        private void AttachBGMSource()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
        }

        private void AttachEffectSources()
        {
            for (var i = 0; i < SourceCount; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                sources.Add(source);
            }
        }

        // 播放bgm
        public void PlayBGM(AudioClip bgm)
        {
            _bgmSource.clip = bgm;
            _bgmSource.time = 0;
            _bgmSource.Play();
        }

        // 停止播放bgm
        public void PauseBGM()
        {
            _bgmSource.Pause();
        }


        // 设置bgm音量
        private void SetBGMVolume(float value) => _bgmSource.volume = value * BgmBaseVolume;

        // 设置音效音量
        private void SetEffectVolume(float value) => sources.ForEach(src => src.volume = value * EffectBaseVolume);

        // 获得空闲的audio source
        private AudioSource GetSource()
        {
            for (var i = 0; i < SourceCount; i++)
            {
                if (!sources[i].isPlaying)
                {
                    var src = sources[i];
                    src.volume = EffectVolume;
                    return src;
                }
            }

            return null;
        }

        // 播放音效
        public void PlayEffect(AudioClip clip)
        {
            var source = GetSource();
            if (source == null) return;
            source.clip = clip;
            source.Play();
        }
    }
}