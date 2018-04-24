using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersistantData : MonoBehaviour {

    public TextMeshProUGUI textMeshPro;

	// Use this for initialization
	void Start () {
        textMeshPro.text = Application.persistentDataPath;

    }
	
}
