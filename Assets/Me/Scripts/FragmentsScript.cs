using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(killMySelf());
    }		

    IEnumerator killMySelf() {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
