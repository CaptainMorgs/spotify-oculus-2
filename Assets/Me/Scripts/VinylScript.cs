using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylScript : MonoBehaviour
{

    public PlaylistScript playlistScript;
    public GameObject fragments, followCube;
    public AudioSource audioSource;
    public GameObject vinylUIGameobject;
    private VinylUI vinylUI;
    public float animationTime = 1.5f;
    public GameObject spawnedFollowCube;
    public List<GameObject> list;
    public bool isThrown = false;
    public Vector3 pushBack = new Vector3(0, 0, 2);

    // Use this for initialization
    void Start()
    {
        vinylUI = vinylUIGameobject.GetComponent<VinylUI>();

        //Ignore the collisions between layer 8 (grabbable) and layer 9 (player)
       Physics.IgnoreLayerCollision(8, 9);
    }

    void OnCollisionEnter(Collision collision)
    {
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(9, 8);
        Debug.Log(collision.gameObject.name);

        Debug.Log("Layer " + collision.gameObject.layer + " and layer " + gameObject.layer);


        if (collision.gameObject.name == "OVRPlayerController")
        {
            Debug.Log("Ignoring collision with player");
            Debug.Log("Layer " + collision.gameObject.layer + " and layer " + gameObject.layer);
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
         //   gameObject.transform.position = gameObject.transform.position + pushBack;
         //   gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        if (isThrown)
        {
            HandleThowCollision(collision);
        }
        else
        {
            // ignore layers grabbable, player, transparent, default and vinyl
            if (collision.gameObject.layer != 8 && collision.gameObject.layer != 9 && collision.gameObject.layer != 10 && collision.gameObject.layer != 11 && collision.gameObject.layer != 12 && collision.gameObject.layer != 0)
            {
                HandleThowCollision(collision);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "followCube")
        {
            list.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "followCube")
        {
            list.Remove(collider.gameObject);
        }
    }


    private void HandleThowCollision(Collision collision)
    {

        //set gravity of the vinyl to enabled when it hits something
        gameObject.GetComponent<Rigidbody>().useGravity = true;

        //re enable rotation when it hits something
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;

        //spawn fragments
        Instantiate(fragments, gameObject.transform.position, Quaternion.identity);
        Rigidbody[] fragmentRigidBodies = fragments.GetComponentsInChildren<Rigidbody>();
        fragmentRigidBodies[0].AddExplosionForce(5.0f, fragmentRigidBodies[0].transform.position, 5.0f, 5.0f, ForceMode.Force);

        //destroy this gameobject
        Destroy(gameObject);
    }

    public void DisableUI()
    {
        vinylUIGameobject.SetActive(false);
    }

    public void EnableUI()
    {
        vinylUIGameobject.SetActive(true);
    }

    public void AnimateToPlayer(Vector3 vector3)
    {
        DisableUI();
        DisableCollisions();
        Hashtable hashtable = new Hashtable();
        hashtable.Add("x", vector3.x);
        hashtable.Add("y", vector3.y);
        hashtable.Add("z", vector3.z);
        hashtable.Add("time", animationTime);
        hashtable.Add("oncomplete", "AnimateOnComplete");

        iTween.MoveTo(gameObject, hashtable);

        iTween.RotateTo(gameObject, new Vector3(0, 0, 0), animationTime);
    }

    public void AnimateToPlayer(Vector3 vector3, Quaternion playerRotation)
    {
        DisableUI();
        DisableCollisions();
        Hashtable hashtable = new Hashtable();
        hashtable.Add("x", vector3.x);
        hashtable.Add("y", vector3.y);
        hashtable.Add("z", vector3.z);
        hashtable.Add("time", animationTime);
        hashtable.Add("oncomplete", "AnimateOnComplete");

        Vector3 rotation = new Vector3(playerRotation.x, playerRotation.y, playerRotation.z);

        iTween.MoveTo(gameObject, hashtable);

        Debug.Log("Rotating from " + gameObject.transform.rotation.ToString() + " to " + rotation.ToString());
        iTween.RotateTo(gameObject, rotation, animationTime);
    }

    private void DisableCollisions()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private void EnableCollisions()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public void AnimateOnComplete()
    {
        EnableUI();
        vinylUI.FadeInImage();
        vinylUI.FadeInPanel();
        SpawnFollowCube();
        EnableCollisions();
    }

    public void InitializeUI(PlaylistScript playlistScript1)
    {
        vinylUI = vinylUIGameobject.GetComponent<VinylUI>();
        vinylUI.InitializeUI(playlistScript1);
    }

    private void SpawnFollowCube()
    {
        Vector3 v = gameObject.transform.position;
        spawnedFollowCube = Instantiate(followCube, v + new Vector3(-0.5f, 0, 0), Quaternion.identity);
        spawnedFollowCube.GetComponent<FollowCubeScript>().playlistScript = playlistScript;
        spawnedFollowCube.GetComponent<FollowCubeScript>().artistId = playlistScript.artistId;
        spawnedFollowCube.GetComponent<FollowCubeScript>().SetText();
        spawnedFollowCube.SetActive(false);
    }

    public void FollowArtist()
    {
        if (list.Count > 0 && list[0].gameObject.tag == "followCube")
        {
            list[0].gameObject.GetComponent<FollowCubeScript>().HandleCollisionWithVinyl2(gameObject);
            list.Remove(list[0]);
        }
        else
        {
            Destroy(spawnedFollowCube);
            SpawnFollowCube();
        }
    }
}
