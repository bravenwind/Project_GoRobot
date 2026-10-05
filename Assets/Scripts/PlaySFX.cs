using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public static PlaySFX Instance;

    [SerializeField]
    private AudioClip moveSound;

    [SerializeField]
    private AudioClip rotateSound;

    [SerializeField]
    private AudioClip collectKeySound;

    [SerializeField]
    private AudioClip levelClearSound;

    [SerializeField]
    private AudioClip levelFailSound;

    private AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        source = GetComponent<AudioSource>();
    }

    public void PlayMove()
    {
        if (moveSound != null) 
        {
            source.PlayOneShot(moveSound);
        }
    }

    public void PlayRotate()
    {
        if (rotateSound != null)
        {
            source.PlayOneShot(rotateSound, 0.5f);
        }
    }

    public void PlayCollectKey()
    {
        if (collectKeySound != null)
        {
            source.PlayOneShot(collectKeySound, 0.5f);
        }
    }

    public void PlayLevelClear()
    {
        if (levelClearSound != null)
        {
            source.PlayOneShot(levelClearSound, 0.5f);
        }
    }

    public void PlayLevelFail()
    {
        if (levelFailSound != null)
        {
            source.PlayOneShot(levelFailSound, 0.5f);
        }
    }

}
