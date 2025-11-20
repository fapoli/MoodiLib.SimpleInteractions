using UnityEngine;

namespace MoodyLib.Interactions {

    /// <summary>
    /// Trigger that plays a one-shot audio clip using a local AudioSource.
    /// Can be invoked manually, by area activation, or as a post-interaction action.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : AbstractTrigger {

        private AudioSource _audioSource;

        /// <summary>
        /// Audio clip played when the trigger is activated.
        /// </summary>
        public AudioClip audioClip;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void OnTriggered() {
            if (audioClip == null) return;
            _audioSource.PlayOneShot(audioClip);
        }
    }
}