using UnityEngine;

using RPG.Characters;


namespace RPG.Core
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip clip;
        [SerializeField] float triggerRadius = 5f;
        [SerializeField] bool isOneTimeOnly = true;

        bool hasPlayed = false;
        AudioSource audioSource;

        PlayerControl player;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = clip;

            player = FindObjectOfType<PlayerControl>();
        }

        void Update()
        {
            float targetDistance = Vector3.Distance(transform.position, player.gameObject.transform.position);
            if(targetDistance <= triggerRadius)
            {
                RequestPlayAudioClip();
            }
        }

        void RequestPlayAudioClip()
        {
            if (isOneTimeOnly && hasPlayed)
            {
                return;
            }
            else if (audioSource.isPlaying == false)
            {
                audioSource.Play();
                hasPlayed = true;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 255f, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}
