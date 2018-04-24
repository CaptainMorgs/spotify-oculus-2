using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecordPlayer : MonoBehaviour
{
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------

    public bool recordPlayerActive = false;

    public GameObject disc;
    public GameObject arm;

    public int mode;
    float armAngle;
    float discAngle;
    float discSpeed;

    private List<Collider> colliderList = new List<Collider>();

    private GameObject spotifyManager;
    private Spotify spotifyScript;

    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    void Awake()
    {
        //  TODO make it the vinyl that the user places on it
        disc = gameObject.transform.Find("teller").gameObject;
        arm = gameObject.transform.Find("arm").gameObject;
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    void Start()
    {
        mode = 0;
        armAngle = 0.0f;
        discAngle = 0.0f;
        discSpeed = 0.0f;
        spotifyManager = GameObject.Find("SpotifyManager");
        spotifyScript = spotifyManager.GetComponent<Spotify>();
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    void Update()
    {
        //-- Mode 0: player off
        if (mode == 0)
        {
            if (recordPlayerActive == true)
                mode = 1;
        }
        //-- Mode 1: activation
        else if (mode == 1)
        {
            if (recordPlayerActive == true)
            {
                armAngle += Time.deltaTime * 30.0f;
                if (armAngle >= 30.0f)
                {
                    armAngle = 30.0f;
                    mode = 2;
                }
                discAngle += Time.deltaTime * discSpeed;
                discSpeed += Time.deltaTime * 80.0f;
            }
            else
                mode = 3;
        }
        //-- Mode 2: running
        else if (mode == 2)
        {
            if (recordPlayerActive == true)
                discAngle += Time.deltaTime * discSpeed;
            else
                mode = 3;
        }
        //-- Mode 3: stopping
        else
        {
            if (recordPlayerActive == false)
            {
                armAngle -= Time.deltaTime * 30.0f;
                if (armAngle <= 0.0f)
                    armAngle = 0.0f;

                discAngle += Time.deltaTime * discSpeed;
                discSpeed -= Time.deltaTime * 80.0f;
                if (discSpeed <= 0.0f)
                    discSpeed = 0.0f;

                if ((discSpeed == 0.0f) && (armAngle == 0.0f))
                    mode = 0;
            }
            else
                mode = 1;
        }

        //-- update objects
        arm.transform.localEulerAngles = new Vector3(0.0f, armAngle, 0.0f);
        disc.transform.localEulerAngles = new Vector3(0.0f, discAngle, 0.0f);
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "vinyl")
        {

            Debug.Log("Vinyl entering recordplayer");

            collider.gameObject.transform.Find("Canvas").gameObject.SetActive(false);

            StartCoroutine(Lerp(collider));
        }
    }
    //pause playback when user picks up record from record player
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "vinyl")
        {
            Debug.Log("Vinyl exiting recordplayer");

            collider.gameObject.transform.Find("Canvas").gameObject.SetActive(true);

            spotifyScript.PausePlayback();
            recordPlayerActive = false;
        }
    }

    IEnumerator Lerp(Collider collider)
    {
        Debug.LogError("Lerping");
        float t = 0f;
        float duration = 2f;

        Vector3 startPostition = collider.transform.position;
        Vector3 endPostition = disc.transform.position;

        Quaternion startRotation = collider.transform.rotation;
        Quaternion endRotation = disc.transform.rotation;


        while (t < 1)
        {
            t += Time.deltaTime / duration;

            collider.transform.position = Vector3.Lerp(startPostition, endPostition, t);
            collider.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }

        VinylScript vinylScript = collider.gameObject.GetComponent<VinylScript>();
        PlaylistScript playlistScript = vinylScript.playlistScript;
        playlistScript.PlaySomething();
        recordPlayerActive = true;
    }
}
