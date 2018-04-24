/***
 * Author: Yunhan Li 
 * Any issue please contact yunhn.lee@gmail.com
 ***/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace VRKeyboard.Utils
{
    public class KeyboardManager : MonoBehaviour
    {
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = false;
        public int maxInputLength;

        [Header("UI Elements")]
        public Text inputText;

        public TextMeshProUGUI inputTextPro;

        [Header("Essentials")]
        public Transform characters;

        #endregion

        #region Private Variables
        //  private string Input {
        //     get { return inputText.text;  }
        //    set { inputText.text = value;  }
        // }

        private string Input
        {
            get { return inputTextPro.text; }
            set { inputTextPro.text = value; }
        }

        private Dictionary<GameObject, Text> keysDictionary = new Dictionary<GameObject, Text>();

        private bool capslockFlag;

        private GameObject spotifyManager;

        private Spotify spotifyScript;
        #endregion

        #region Monobehaviour Callbacks
        private void Awake()
        {

            for (int i = 0; i < characters.childCount; i++)
            {
                GameObject key = characters.GetChild(i).gameObject;
                Text _text = key.GetComponentInChildren<Text>();
                keysDictionary.Add(key, _text);

                key.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GenerateInput(_text.text);
                });
            }

            capslockFlag = isUppercase;
            CapsLock();

            spotifyManager = GameObject.Find("SpotifyManager");
            spotifyScript = spotifyManager.GetComponent<Spotify>();
        }
        #endregion

        #region Public Methods
        public void Backspace()
        {
            if (Input.Length > 0)
            {
                Input = Input.Remove(Input.Length - 1);
            }
            else
            {
                return;
            }
        }

        public void Clear()
        {
            Input = "";
        }

        public void CapsLock()
        {
            if (capslockFlag)
            {
                foreach (var pair in keysDictionary)
                {
                    pair.Value.text = ToUpperCase(pair.Value.text);
                }
            }
            else
            {
                foreach (var pair in keysDictionary)
                {
                    pair.Value.text = ToLowerCase(pair.Value.text);
                }
            }
            capslockFlag = !capslockFlag;
        }

        public void Search()
        {
            //    spotifyScript.searchSpotify(inputText.text);
            Debug.Log("Search query: " + inputTextPro.text);
            spotifyScript.SearchSpotify(inputTextPro.text);
        }
        #endregion

        #region Private Methods
        public void GenerateInput(string s)
        {
            if (Input.Length > maxInputLength) { return; }
            //added my shitty code here
            if (s.Equals("Caps Lock"))
            {
                CapsLock();
                return;
            }
            if (s.Equals("Backspace"))
            {
                Backspace();
                return;
            }
            if (s.Equals("Clear All"))
            {
                Clear();
                return;
            }
            if (s.Equals("Search"))
            {
                Search();
                return;
            }

            Input += s;

            Debug.Log("Keyboard being pressed!");
        }

        private string ToLowerCase(string s)
        {
            return s.ToLower();
        }

        private string ToUpperCase(string s)
        {
            return s.ToUpper();
        }
        #endregion
    }
}