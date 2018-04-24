using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using VRKeyboard.Utils;
using TMPro;

public class Raycast : MonoBehaviour
{

    // Use this for initialization

    private LineRenderer lineRenderer;
    public int raycastDistance = 20;
    private GameObject ovrPlayerController;
    private bool movementStarted = false;
    private RaycastHit newPosition;
    Material material;
    public GameObject vinyl, rightHandAnchor, vinylContainer;
    public bool playOnClick = false;
    private GameObject spawnedVinyl;
    public GameObject hoverUIGameObject;
    private HoverUI hoverUI;
    public GameObject keyboardGameObject;
    private KeyboardManager keyboardManagerScript;
    public GameObject spriteGameObject;
    private Sprite sprite;
    public UnityEngine.UI.Image pointerImage;
    public Vector3 pointerUIScale = new Vector3(0.001f, 0.001f, 0.001f);
    public Vector3 pointerWorldScale = new Vector3(0.005f, 0.005f, 0.005f);
    public Vector3 pointerWorldScaleZOffset = new Vector3(0f, 0f, 0.05f);
    private LeftHandUI leftHandUIHit;
    public Vector3 spawnPosition;
    public Vector3 spawnPosition2 = new Vector3(-0.5f, 0, 2.0f);
    void Start()
    {

        material = new Material(Shader.Find("Particles/Additive"));

        hoverUI = hoverUIGameObject.GetComponent<HoverUI>();

        keyboardManagerScript = keyboardGameObject.GetComponent<KeyboardManager>();

        sprite = spriteGameObject.GetComponent<Sprite>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetColors(Color.white, Color.white);
        lineRenderer.material = material;

        ovrPlayerController = GameObject.Find("OVRPlayerController");

    }

    /// <summary>
    /// If trigger is pressed, change line renderer's colour to blue if the raycast hits the floor, 
    /// if the raycast is pointing to the floor on trigger release, teleport player to location.
    /// </summary>
    void Update()
    {
        RayCast();

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            RayCastInput();
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            newPosition = RayCastMovement();
        }

        else
        {
            lineRenderer.SetColors(Color.white, Color.white);
            lineRenderer.material.color = Color.white;


            if (movementStarted && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                Debug.Log("Moving to " + newPosition.transform.ToString());

                ovrPlayerController.transform.position = new Vector3(newPosition.point.x, newPosition.point.y + 0.5f, newPosition.point.z);

                lineRenderer.material.color = Color.white;
                lineRenderer.SetColors(Color.white, Color.white);

                movementStarted = false;
            }
        }





    }

    void FixedUpdate()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, transform.forward * raycastDistance + transform.position);
    }

    /// <summary>
    /// Logic for when a raycast line collides with a collider and and the input button is pressed
    /// </summary>
    void RayCastInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
            Debug.Log(hit.transform.name);

            ResumePlayback resumePlayback = hit.transform.GetComponent<ResumePlayback>();
            PausePlayback pausePlayback = hit.transform.GetComponent<PausePlayback>();
            PlaylistScript playlistScript = hit.transform.GetComponent<PlaylistScript>();
            LeftHandUI leftHandUI = hit.transform.GetComponent<LeftHandUI>();

            if (hit.transform.gameObject.tag == "key")
            {
                Text text = hit.transform.gameObject.GetComponentInChildren<Text>();

                keyboardManagerScript.GenerateInput(text.text);
            }

            if (resumePlayback != null)
            {
                resumePlayback.ResumePlaybackFunction();
            }
            else if (leftHandUI != null)
            {
                leftHandUI.OnRayCastHit();
            }
            else if (pausePlayback != null)
            {
                pausePlayback.PausePlaybackFunction();
            }
            else if (playlistScript != null)
            {
                if (playOnClick)
                {
                    playlistScript.PlaySomething();
                }

                if (GameObject.FindWithTag("vinyl") != null)
                {
                    Destroy(GameObject.FindWithTag("vinyl"));
                }

                if (GameObject.FindWithTag("followCube") != null)
                {
                    Destroy(GameObject.FindWithTag("followCube"));
                }

                InstansiateVinylWorking(hit, playlistScript);

                Debug.Log("Spawning Vinyl");
            }

        }
    }

    private void InstansiateVinylWorking(RaycastHit hit, PlaylistScript playlistScript)
    {
        spawnedVinyl = Instantiate(vinyl, hit.transform.position + new Vector3(0, 0, -0.5f), Quaternion.Euler(-90f, 0, 0));


        spawnedVinyl.GetComponent<VinylScript>().playlistScript = playlistScript;
        // spawnedVinyl.GetComponent<VinylScript>().AnimateToPlayer(rightHandAnchor.transform.position + new Vector3(-0.5f, 0, 1.0f), ovrPlayerController.transform.rotation);
        spawnedVinyl.GetComponent<VinylScript>().AnimateToPlayer(ovrPlayerController.transform.position + spawnPosition2, ovrPlayerController.transform.rotation);
        spawnedVinyl.GetComponent<VinylScript>().InitializeUI(playlistScript);
    }

    public void InstansiateVinylWorking(PlaylistScript playlistScript)
    {
        spawnedVinyl = Instantiate(vinyl, new Vector3(10, 2, 0), Quaternion.Euler(-90f, 0, 0));


        spawnedVinyl.GetComponent<VinylScript>().playlistScript = playlistScript;
        spawnedVinyl.GetComponent<VinylScript>().AnimateToPlayer(rightHandAnchor.transform.position + new Vector3(-0.5f, 0, 1.0f), ovrPlayerController.transform.rotation);
        spawnedVinyl.GetComponent<VinylScript>().InitializeUI(playlistScript);
    }

    private void InstansiateVinyl(RaycastHit hit, PlaylistScript playlistScript)
    {
        Debug.Log("Instansiating at " + hit.transform.position + " plus " + spawnPosition);
        GameObject spawnedVinylContatainer = Instantiate(vinylContainer, hit.transform.position + new Vector3(0, 0, -0.5f), Quaternion.Euler(-90f, 0, 0));
        spawnedVinyl = spawnedVinylContatainer.transform.Find("vinyl").gameObject;
        GameObject spawnedFollowCube = spawnedVinylContatainer.transform.Find("Follow Cube").gameObject;
        spawnedFollowCube.GetComponent<FollowCubeScript>().playlistScript = playlistScript;

        spawnedVinyl.GetComponent<VinylScript>().playlistScript = playlistScript;
        spawnedVinylContatainer.GetComponent<VinylContainerScript>().AnimateToPlayer(rightHandAnchor.transform.position + new Vector3(-0.5f, 0, 1.0f));
        spawnedVinyl.GetComponent<VinylScript>().InitializeUI(playlistScript);
    }

    RaycastHit RayCastMovement()
    {

        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance) && hit.transform.tag == "Floor")
        {

            lineRenderer.SetColors(Color.blue, Color.blue);
            lineRenderer.material.color = Color.blue;

            movementStarted = true;

        }
        else
        {
            lineRenderer.material.color = Color.white;
            lineRenderer.SetColors(Color.white, Color.white);
            movementStarted = false;
        }

        return hit;
    }

    void RayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, raycastDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            pointerImage.transform.position = hit.point;
            pointerImage.material.renderQueue = 4000;

            if (leftHandUIHit != null)
            {
                leftHandUIHit.OnHoverExit();
            }

            //11 is hand UI layer
            if (hit.collider.gameObject.layer == 11)
            {
                pointerImage.transform.localScale = pointerUIScale;

                LeftHandUI leftHandUI = hit.transform.GetComponent<LeftHandUI>();


                if (leftHandUI != null)
                {
                    leftHandUIHit = leftHandUI;
                    leftHandUI.OnHover();
                }

            }
            //5 is UI layer
            else if (hit.collider.gameObject.layer == 5)
            {
                pointerImage.transform.localScale = pointerWorldScale;
                pointerImage.transform.position = hit.point - pointerWorldScaleZOffset;
            }
            else
            {
                pointerImage.transform.localScale = new Vector3(0f, 0f, 0f);
            }

            if (hit.transform.gameObject.tag == "song" || hit.transform.gameObject.tag == "playlist" || hit.transform.gameObject.tag == "artist")
            {
                GameObject playlistGameObject = hit.transform.gameObject;
                hoverUI.updateHoverUI(playlistGameObject.GetComponent<PlaylistScript>());
            }
            else
            {
                hoverUI.updateHoverUI();
            }
        }
    }

    /// <summary>
    /// prevents memory leak
    /// </summary>
    private void OnDestroy()
    {
        Destroy(GetComponent<Renderer>());
    }
}
