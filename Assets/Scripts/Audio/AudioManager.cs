using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        [Header("BGM")]
        private AudioSource bgmSource;
        [SerializeField] private List<AudioClip> bgmClips = new();

        [Header("SFX")]
        private AudioSource seSource;
        private bool isUninterruptibleSEPlaying;
        private Coroutine bgmLoopCoroutine;

        protected override bool DontDestroy => true;

        protected override void OnAwake()
        {
            if (bgmSource == null)
                bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = false;

            if (seSource == null)
            {
                seSource = gameObject.AddComponent<AudioSource>();
            }
            seSource.playOnAwake = false;
        }
    
        public void PlayRandomBGM()
        {
            if (bgmClips.Count == 0) return;
            var clip = bgmClips[Random.Range(0, bgmClips.Count)];
            bgmSource.clip = clip;
            bgmSource.Play();
            
            if (bgmLoopCoroutine != null)
                StopCoroutine(bgmLoopCoroutine);
            bgmLoopCoroutine = StartCoroutine(WaitAndPlayNextBGM());
        }

        public void StopBGM(float fadeDuration = 1f)
        {
            if (bgmLoopCoroutine != null)
            {
                StopCoroutine(bgmLoopCoroutine);
                bgmLoopCoroutine = null;
            }
            StartCoroutine(FadeOutAndStopBGM(fadeDuration));
        }
        
       
        private IEnumerator FadeOutAndStopBGM(float duration)
        {
            var startVolume = bgmSource.volume;
            var time        = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }
            bgmSource.Stop();
            bgmSource.volume = startVolume; // 恢复音量，方便下次播放
        }

        private IEnumerator WaitAndPlayNextBGM()
        {
            yield return new WaitWhile(() => bgmSource.isPlaying);
            PlayRandomBGM();
        }

        public void PlaySE(string sfxName, bool canInterrupt = true)
        {
            if (isUninterruptibleSEPlaying && canInterrupt && seSource.isPlaying)
            {
                return;
            }

            var clip = ResourceManager<AudioClip>.GetResource(sfxName);
            if (clip == null)
            {
                Debug.LogError($"未找到音效资源: {sfxName}");
                return;
            }
            seSource.clip = clip;
            seSource.Play();
            isUninterruptibleSEPlaying = !canInterrupt;

            if (!canInterrupt)
            {
                StartCoroutine(ResetUninterruptibleFlagWhenDone());
            }
        }

        private IEnumerator ResetUninterruptibleFlagWhenDone()
        {
            yield return new WaitWhile(() => seSource.isPlaying);
            isUninterruptibleSEPlaying = false;
        }

        public void SetBGMVolume(float volume)
        {
            if (volume is < 0f or > 1f) { Debug.LogWarning("BGM音量必须在0到1之间"); return; }
            bgmSource.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            if (volume is < 0f or > 1f) { Debug.LogWarning("BGM音量必须在0到1之间"); return; }
            seSource.volume = volume;
        }
    }
}