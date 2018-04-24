using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylContainerScript : MonoBehaviour
{

    public float animationTime = 5f;
    public GameObject VinylGameObject;
    private VinylScript vinylScript;

    public void AnimateToPlayer(Vector3 vector3)
    {
        vinylScript = VinylGameObject.GetComponent<VinylScript>();
        vinylScript.DisableUI();
        Hashtable hashtable = new Hashtable();
        hashtable.Add("x", vector3.x);
        hashtable.Add("y", vector3.y);
        hashtable.Add("z", vector3.z);
        hashtable.Add("time", animationTime);
        hashtable.Add("oncomplete", "AnimateOnComplete");

        iTween.MoveTo(gameObject, hashtable);

        iTween.RotateTo(gameObject, new Vector3(0, 0, 0), animationTime);
    }

    private void AnimateOnComplete()
    {
        vinylScript.AnimateOnComplete();
    }
}
