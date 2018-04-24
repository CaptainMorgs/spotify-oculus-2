using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandUIController : MonoBehaviour
{

    public GameObject panel1, panel2;
    public float animationTime = 0.2f;
    private Vector3 panel1StartPosition, panel2StartPosition;
    private bool panel1InFocus, isAnimating;

    // Use this for initialization
    void Start()
    {
        panel1InFocus = true;
        isAnimating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            if (panel1InFocus)
            {
                panel1.gameObject.SetActive(false);
                panel2.gameObject.SetActive(true);
                panel1InFocus = false;
            }
            else
            {
                panel1.gameObject.SetActive(true);
                panel2.gameObject.SetActive(false);
                panel1InFocus = true;
            }
        }
    }

    private void OnAnimationComplete()
    {
        isAnimating = false;
    }

    private void TweenStuff()
    {

        if (OVRInput.Get(OVRInput.Button.Three) && isAnimating == false)
        {
            Debug.Log("Button Three Pressed");
            isAnimating = true;

            panel1StartPosition = panel1.transform.position;
            panel2StartPosition = panel2.transform.position;

            if (panel1InFocus)
            {
                iTween.MoveTo(panel1, iTween.Hash(panel2StartPosition, animationTime, "onComplete", "OnAnimationComplete", "onCompleteTarget", gameObject));
                iTween.MoveTo(panel2, panel1StartPosition, animationTime);
            }
            else
            {
                iTween.MoveTo(panel1, iTween.Hash(panel2StartPosition, animationTime, "onComplete", "OnAnimationComplete", "onCompleteTarget", gameObject));
                iTween.MoveTo(panel2, panel2StartPosition, animationTime);
            }
        }
    }
}
