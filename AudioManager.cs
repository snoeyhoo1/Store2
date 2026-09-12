using System.Collections.Generic;
using UnityEngine;

namespace Store1
{
    public enum Sfx { Coin, Upgrade, Expand, Customer, Sit, Serve, Staff, Mission, Gem, Reward, Shop }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I { get; private set; }
        readonly Dictionary<string, AudioClip> clips = new();
        AudioSource bgmSource, sfxSource, ambienceSource;
        float sfxVolume = 0.85f;
        float lastCoinTime = -10f;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this; DontDestroyOnLoad(gameObject);
            foreach (var clip in Resources.LoadAll<AudioClip>("Sounds")) clips[clip.name] = clip;
            bgmSource = MakeSource("BGM", true, .22f);
            sfxSource = MakeSource("SFX", false, sfxVolume);
            ambienceSource = MakeSource("Ambience", true, .12f);
            PlayBgm();
        }

        AudioSource MakeSource(string name, bool loop, float volume)
        {
            var go = new GameObject(name); go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>(); src.loop = loop; src.playOnAwake = false; src.volume = volume; src.spatialBlend = 0f;
            return src;
        }

        void PlayBgm()
        {
            if (clips.TryGetValue("bgm", out var clip)) { bgmSource.clip = clip; bgmSource.Play(); }
        }

        public void Play(Sfx sfx, float volume = 1f, float pitch = 1f)
        {
            if (sfx == Sfx.Coin && Time.unscaledTime - lastCoinTime < .12f) return;
            if (sfx == Sfx.Coin) lastCoinTime = Time.unscaledTime;
            string key = sfx.ToString().ToLowerInvariant();
            if (!clips.TryGetValue(key, out var clip)) return;
            sfxSource.pitch = Random.Range(pitch - .04f, pitch + .04f);
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
            sfxSource.pitch = 1f;
        }

        public void SetBgmVolume(float value) => bgmSource.volume = Mathf.Clamp01(value);
        public void SetSfxVolume(float value) { sfxVolume = Mathf.Clamp01(value); if (sfxSource != null) sfxSource.volume = sfxVolume; }
        public void SetAmbienceVolume(float value) => ambienceSource.volume = Mathf.Clamp01(value);
    }
}
