using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(KillMyself());

    }
	
    IEnumerator KillMyself() {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
