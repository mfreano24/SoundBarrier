using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    AudioSource[] src;
    //GUESS WE HAVE TO DO MULTIPLE LIKE THE GOOD OLD DAYS
    public AudioClip walkingClip;
    public AudioClip runningClip;

    PlayerController pc;


    bool sprinting;
    bool moving;
    int currentClipIndex = 0;

    MainUI mui;

    bool fullMute = false;
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        src = GetComponents<AudioSource>();
        src[0].clip = walkingClip;
        src[1].clip = runningClip;
        src[0].volume = 0;
        src[1].volume = 0;
        src[0].Play();
        src[1].Play();
        Debug.Log("walking: " + src[0].clip.name + "\trunning: " + src[1].clip.name);
        mui = GameObject.Find("Canvas").GetComponent<MainUI>(); //too lazy to serialize atm, idk
    }

    private void OnEnable()
    {
        
    }

    public void MuteFeet()
    {
        src[0].enabled = false;
        src[1].enabled = false;
    }
    public void UnmuteFeet()
    {
        src[0].enabled = true;
        src[1].enabled = true;
        src[0].volume = 0;
        src[1].volume = 0;
        src[0].Play();
        src[1].Play();
    }

    private void Update()
    {
        if (!fullMute)
        {
            if (pc.moving)
            {
                if (src[currentClipIndex].volume == 0)
                {
                    src[currentClipIndex].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.75f; //need this to be a little quieter.
                }
            }
            else
            {
                if (src[currentClipIndex].volume != 0)
                {
                    src[currentClipIndex].volume = 0;

                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("NOW SPRINTING SOUND");
                currentClipIndex = 1;
                src[0].volume = 0;
                src[1].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.75f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Debug.Log("WALKING SOUND NOW");
                currentClipIndex = 0;
                src[1].volume = 0;
                src[0].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.75f;
            }
        }
        
    }
}
