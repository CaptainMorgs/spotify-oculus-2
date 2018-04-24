using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using SpotifyAPI.Web.Models;

public class NewPlayModeTest {

    private GameObject spotifyManager;
    private Spotify spotifyManagerScript;


   [Test]
	public void NewPlayModeTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator NewPlayModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}

    [UnityTest]
    public IEnumerator GameObject_WithRigidBody_WillBeAffectedByPhysics()
    {
        var go = new GameObject();
        go.AddComponent<Rigidbody>();
        var originalPosition = go.transform.position.y;

        yield return new WaitForFixedUpdate();

        Assert.AreNotEqual(originalPosition, go.transform.position.y);
    }

    [UnityTest]
    public IEnumerator Test_Spotify_GetTrack()
    {
       spotifyManager = GameObject.Find("SpotifyManager");
       spotifyManagerScript = spotifyManager.GetComponent<Spotify>();

        string trackId = "0Pw6Gg8QChw5iSRRSrcWXP";
        FullTrack fullTrack = spotifyManagerScript.GetTrack(trackId);

        yield return new WaitForFixedUpdate();

        Assert.IsFalse(fullTrack.HasError());
    }
}
