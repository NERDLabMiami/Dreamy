using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour {

	public InputField playerId;

	public IEnumerator submitPlayer(string skin, string hair, string face, string ears, string eyes, string iris, string mouth, string nose) {
		WWWForm form = new WWWForm ();
		form.AddField("qualtrics", playerId.text);
		form.AddField ("skin", skin);
		form.AddField ("hair", hair);
		form.AddField ("face", face);
		form.AddField ("ears", ears);
		form.AddField ("eyes", eyes);
		form.AddField ("iris", iris);
		form.AddField ("mouth", mouth);
		form.AddField ("nose", nose);
		WWW link = new WWW ("http://track.nerdlab.miami/player.php", form);
		yield return link;
		if (!string.IsNullOrEmpty(link.error)) {
			Debug.Log (link.error);
		}
		else {
			Debug.Log ("Submitted New Player");
		}
	}
	public IEnumerator submitEvent(int eventId, string subtype, string beliefId, string characterId, string dialogId = "none") {

		WWWForm form = new WWWForm ();
		form.AddField("qualtrics_id", PlayerPrefs.GetString("qualtrics_id"));
		form.AddField("event_id", eventId);

		form.AddField ("dialog_id", dialogId);

		if (beliefId.Length > 0) {
			form.AddField("belief_id", beliefId);
			Debug.Log("Belief Exists");
		}
		else {
			Debug.Log("Belief length is 0 or less!");
		}
		if (characterId.Length > 0) {
			form.AddField("character_id", characterId);
			Debug.Log("Adding Character Id");

		}
		form.AddField("subtype", subtype);
		form.AddField("days", PlayerPrefs.GetInt("days left",-1));
		form.AddField("risk", PlayerPrefs.GetInt("cancer risk",0));
		form.AddField("tan",PlayerPrefs.GetInt("tan",0));
		form.AddField("attractiveness", PlayerPrefs.GetInt("attractiveness"));
		WWW link = new WWW ("http://track.nerdlab.miami/event.php", form);
		Debug.Log("Getting to the yield part with form : " + form);

		yield return link;

		Debug.Log("ERROR: " + link.error);
		Debug.Log("MESSAGE: " + link.text);
		if (!string.IsNullOrEmpty(link.error)) {
			Debug.Log (link.error);
			Debug.Log("Got an Error");

		}
		else {
			Debug.Log ("Submitted Event from IENumerator");
		}
	}

	public void trackEvent(int eventId, string subtype, string beliefId, string characterId, string dialogId = "none") {
		Debug.Log("Called Track Event");

		StartCoroutine(submitEvent(eventId, subtype, beliefId, characterId, dialogId));	
	}


	public void storeQualtricsID() {
		PlayerPrefs.SetString("qualtrics_id", playerId.text);

	}

	public void newPlayer(string skin, string hair, string face, string ears, string eyes, string iris, string mouth, string nose) {
		StartCoroutine (submitPlayer (skin, hair, face, ears, eyes, iris, mouth, nose));
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
