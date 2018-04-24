using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInput : MonoBehaviour
{
    public enum HandState
    {
        EMPTY,
        TOUCH,
        HOLD
    };

    public OVRInput.Controller touchController = OVRInput.Controller.LTouch;
    public LineRenderer lineRenderer;
    public HandState handState = HandState.EMPTY;
    public Rigidbody attachedPoint = null;
    public bool ignoreContactPoint = false;
    public Rigidbody objectHeld;
    public FixedJoint temporaryJoint;
    private Vector3 velocityOld;
    public float throwThreshold = 5f;
    public Vector3 scalingThrowSpeed = new Vector3(5, 5, 5);

    // Use this for initialization
    /// <summary>
    // Upon the hands’ initialization we will get the rigid body component within it.  
    //This will be used as an attach point for our hands when we pick up objects,
    //if we have not specified an attach point in the Inspector view
    /// </summary>
    void Start()
    {
        if (attachedPoint == null)
        {
            attachedPoint = GetComponent<Rigidbody>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (handState)
        {
            //If there is no temporary joint and the player is pressing down on the hand trigger with enough pressure (>= 0.5f in this case), 
            //we set the held object’s velocity to zero, and create a temporary joint attached to it. 
            //We then connect that joint to our AttachPoint (by default, the hand itself) 
            //and set the hand state to HOLDING
            case HandState.TOUCH:
                //   Debug.LogWarning(mHandState);

                if (temporaryJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touchController) >= 0.5f)
                {
                    objectHeld.velocity = Vector3.zero;
                    temporaryJoint = objectHeld.gameObject.AddComponent<FixedJoint>();
                    temporaryJoint.connectedBody = attachedPoint;
                    handState = HandState.HOLD;

                    //if you grab a vinyl, disable its UI, enable follow cube
                    if (objectHeld.gameObject.tag == "vinyl")
                    {
                        VinylScript vinylScript = objectHeld.gameObject.GetComponent<VinylScript>();
                        vinylScript.DisableUI();
                        vinylScript.spawnedFollowCube.SetActive(true);
                    }

                }
                break;

            // If the hand state is already in the HOLDING state, 
            //we check that we do have a temporary joint (i.e. that it is not null) 
            //and that the player is releasing enough of the trigger (in this case, < 0.5f).  
            //If so, we immediately destroy the temporary joint, and set it to null signifying that it is no longer in use.  
            //We then throw the object using a throw method (described further below) and set the hand state to EMPTY.

            case HandState.HOLD:
                //    Debug.LogWarning(mHandState);

                //disable line renderer while holding objects
                lineRenderer.gameObject.SetActive(false);

                if (temporaryJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touchController) < 0.5f)
                {
                    Object.DestroyImmediate(temporaryJoint);
                    temporaryJoint = null;


                    //To stop collisions with hands when throwing an object
                    gameObject.GetComponent<SphereCollider>().enabled = false;
                    bool isThrown = throwObject();
                    VinylScript vinylScript = objectHeld.gameObject.GetComponent<VinylScript>();

                    //if object isn't thrown and its a vinyl then re-enable its UI
                    if (!isThrown)
                    {
                        if (objectHeld.gameObject.tag == "vinyl")
                        {
                            vinylScript.isThrown = false;
                            vinylScript.EnableUI();
                            vinylScript.FollowArtist();
                            //handState = HandState.EMPTY;
                        }
                    }
                    else
                    {
                        //if vinyl is thrown then despawn followcube
                        Destroy(vinylScript.spawnedFollowCube);
                        vinylScript.isThrown = true;
                        //  vinylScript.spawnedFollowCube.SetActive(false);
                    }
                    //re enable line renderer when letting go of objects
                    lineRenderer.gameObject.SetActive(true);
                    gameObject.GetComponent<SphereCollider>().enabled = true;
                    handState = HandState.EMPTY;


                }
                else if (objectHeld == null)
                {
                    handState = HandState.EMPTY;
                    //re enable line renderer when letting go of objects
                    lineRenderer.gameObject.SetActive(true);
                }
                velocityOld = OVRInput.GetLocalControllerAngularVelocity(touchController);

                break;
        }
    }

    /// <summary>
    /// handles when a hand collides with an object.  
    /// It checks that we do not have something in this hand already, 
    /// and then ensures that the object is on the grabbable layer and has a rigid body component attached to it.  
    /// We then store it in our held object and change the hand state to touching
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter(Collider collider)
    {
        if (handState == HandState.EMPTY && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touchController) < 0.5f)
        {

            GameObject temp = collider.gameObject;
            if (temp != null && temp.layer == LayerMask.NameToLayer("grabbable") && temp.GetComponent<Rigidbody>() != null)
            {
                objectHeld = temp.GetComponent<Rigidbody>();
                handState = HandState.TOUCH;
            }
        }
    }

    /// <summary>
    /// checking that we were not holding an object,
    /// and that the object we are no longer touching was on the grabbable layer. 
    /// We then set the held object to null and set the hand state to empty.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerExit(Collider collider)
    {
        if (handState != HandState.HOLD)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("grabbable"))
            {
                objectHeld = null;
                handState = HandState.EMPTY;
            }
        }
    }

    private bool throwObject()
    {
        Debug.Log("Vector Distance: " + Vector3.Distance(OVRInput.GetLocalControllerAngularVelocity(touchController), new Vector3(1, 1, 1)));

        //Only throw object if controller velocity exceeds some threshold


        if (Vector3.Distance(OVRInput.GetLocalControllerAngularVelocity(touchController), new Vector3(1, 1, 1)) > throwThreshold)
        {
            objectHeld.velocity = OVRInput.GetLocalControllerVelocity(touchController);
            if (velocityOld != null)
            {
                // mHeldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);

                //Increase throw speed using scalingThrowSpeed
                objectHeld.angularVelocity = Vector3.Scale(OVRInput.GetLocalControllerAngularVelocity(touchController), scalingThrowSpeed);
            }
            objectHeld.maxAngularVelocity = objectHeld.angularVelocity.magnitude;

            // lock the rotation so it looks smooth
            objectHeld.freezeRotation = true;

            return true;
        }
        else
        {
            //if you are not exceeding the threshold for throwing something, set its velocity to 0
            objectHeld.velocity = new Vector3(0, 0, 0);
            objectHeld.useGravity = false;
            return false;
        }
    }
}

