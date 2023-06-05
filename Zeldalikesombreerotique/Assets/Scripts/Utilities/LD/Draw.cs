using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Utilities.LD
{
    public class Draw : MonoBehaviour
    {
        internal readonly List<(Link link, int index)> Links = new();
        [SerializeField] private bool isEnabledAtStart;
        internal readonly List<TrafficLights> LinkedTrafficLights = new();
        internal bool IsEnabled;
        private bool _isLightedByStreetLight;
        internal bool IsPermanentlyEnabled;
        [SerializeField] internal Transform linkPoint;

        [SerializeField] private AudioSource audioSource1;
        [SerializeField] private AudioSource audioSource2;
        private Coroutine _audioCoroutine;
        private Tweener _audioTweener1;
        private Tweener _audioTweener2;
        [SerializeField] private float audioFadeDuration;
        [SerializeField] private float audioDisablingFadeDuration;
        private int _counterSound;
        
        private LightedElementsManager _lightedElementsManager;
        
        private void Start()
        {
            _lightedElementsManager = LightedElementsManager.Instance;
            
            if (isEnabledAtStart)
            {
                Enable();
            }
        }

        private void FixedUpdate()
        {
            if (IsEnabled && _isLightedByStreetLight && !isEnabledAtStart)
            {
                _lightedElementsManager.RevealedObjects[gameObject] = true;
            }
            
            _lightedElementsManager.CurrentCheckCoroutine ??= StartCoroutine(_lightedElementsManager.CheckDictionariesCoroutine(gameObject));
        }

        internal void Disable(bool isStreetLight = false)
        {
            if (IsPermanentlyEnabled) return;
            if (LinkedTrafficLights.All(x => x.CheckIfLinked())) return;
            
            StopDrawGlowSound();
            
            IsEnabled = false;
            if (isStreetLight) _isLightedByStreetLight = false;
            ChangeLinkValues(false);
        }

        internal void Enable(bool isStreetLight = false)
        {
            if (IsPermanentlyEnabled || IsEnabled) return;
            
            _counterSound = 0;
            _audioCoroutine = StartCoroutine(DrawGlowSoundCoroutine());
            
            IsEnabled = true;
            if (isStreetLight) _isLightedByStreetLight = true;
            LinkedTrafficLights.ForEach(x => x.CheckIfLinked());
            ChangeLinkValues(true);
        }

        private void ChangeLinkValues(bool isEnabled)
        {
            foreach (var link in Links)
            {
                switch (link.index)
                {
                    case 1:
                        link.link.IsObject1Enabled = isEnabled;
                        link.link.CheckIfBothObjectsAreEnabled(1);
                        break;

                    case 2:
                        link.link.IsObject2Enabled = isEnabled;
                        link.link.CheckIfBothObjectsAreEnabled(2);
                        break;
                }
            }
        }
        
        private IEnumerator DrawGlowSoundCoroutine()
        {
            if (isEnabledAtStart) yield break;
            
            if (_counterSound % 2  == 0)
            {
                audioSource1.volume = 0f;
                audioSource1.Play();
                _audioTweener1 = audioSource1.DOFade(1f, audioFadeDuration);

                yield return new WaitForSeconds(audioSource1.clip.length - audioFadeDuration);

                _audioTweener1 = audioSource1.DOFade(0f, audioFadeDuration);
            }
            else
            {
                audioSource2.volume = 0f;
                audioSource2.Play();
                _audioTweener2 = audioSource2.DOFade(1f, audioFadeDuration);

                yield return new WaitForSeconds(audioSource2.clip.length - audioFadeDuration);

                _audioTweener2 = audioSource2.DOFade(0f, audioFadeDuration);
            }
            
            _counterSound++;
            _audioCoroutine = StartCoroutine(DrawGlowSoundCoroutine());
        }
        
        private void StopDrawGlowSound()
        {
            if (isEnabledAtStart) return;
            
            if (_audioCoroutine != null)
            {
                StopCoroutine(_audioCoroutine);
                _audioCoroutine = null;
            }
            
            _audioTweener1?.Kill();
            _audioTweener2?.Kill();
            audioSource1.DOFade(0f, audioDisablingFadeDuration);
            audioSource2.DOFade(0f, audioDisablingFadeDuration);
        }
    }
}
