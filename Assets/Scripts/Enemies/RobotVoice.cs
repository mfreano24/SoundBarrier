using UnityEngine;

public class RobotVoice : MonoBehaviour
{

    public AudioSource voiceLine;

    public Light speechLight;

    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;
    void Awake()
    {

        if (!voiceLine)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];

    }


    void Start()
    {

    }
    void Update()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            //dont check every frame or this game will literally take up all your RAM.
            currentUpdateTime = 0f;
            voiceLine.clip.GetData(clipSampleData, voiceLine.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength;
            Debug.Log("VOICE LINE LOUDNESS = " + clipLoudness);
            speechLight.intensity = clipLoudness / 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            voiceLine.Play();
        }
    }
}
