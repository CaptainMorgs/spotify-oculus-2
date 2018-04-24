using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecommendationSeed : MonoBehaviour
{
    public GameObject recommenderDeck;

    public TextMeshProUGUI seedText;

    private RecommenderDeck recommenderDeckScript;

    // Use this for initialization
    void Start()
    {
        recommenderDeckScript = recommenderDeck.GetComponent<RecommenderDeck>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "vinyl")
        {

            Debug.Log("Vinyl entering recommenderSeed");

            collider.gameObject.transform.Find("Canvas").gameObject.SetActive(false);

            StartCoroutine(Lerp(collider));
           
            recommenderDeckScript.activeSeeds.Add(collider.gameObject);

            PlaylistScript playlistScript = collider.gameObject.GetComponent<VinylScript>().playlistScript;

            seedText.text = playlistScript.playlistName;

            playlistScript.inRecommender = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "vinyl")
        {
            collider.gameObject.transform.Find("Canvas").gameObject.SetActive(true);

            Debug.Log("Vinyl exiting recommenderSeed");

            recommenderDeckScript.activeSeeds.Remove(collider.gameObject);

            seedText.text = "Add seed";

            PlaylistScript playlistScript = collider.gameObject.GetComponent<VinylScript>().playlistScript;

            playlistScript.inRecommender = false;
        }
    }

    IEnumerator Lerp(Collider collider)
    {
        Debug.LogError("Lerping");
        float t = 0f;
        float duration = 2f;

        Vector3 startPostition = collider.transform.position;
        Vector3 endPostition = transform.position;

        Quaternion startRotation = collider.transform.rotation;
        Quaternion endRotation = transform.rotation;


        while (t < 1)
        {
            t += Time.deltaTime / duration;

            collider.transform.position = Vector3.Lerp(startPostition, endPostition, t);
            collider.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }

    }

    public void ResetText() {
        seedText.text = "Add seed";
    }
}
