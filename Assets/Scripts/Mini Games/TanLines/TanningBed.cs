using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TanningBed : MonoBehaviour {
	public float tanAmount = 0;
	public TanningSalonAssistant assistant;
	public Character character;
	public GameObject stopTanButton;
	public GameObject continueButton;
	public int visitsRequiredForMole = 2;
	private int salonVisits;
	// Use this for initialization
	void Start () {
		salonVisits = PlayerPrefs.GetInt("tanning salon visits",0);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startTanningSlider() {
	}

	public void stopTanningSlider() {
		//TODO: take into account severity and their risk level
		//TODO: add sun spots
		Time.timeScale = 0f;
		GetComponent<AudioSource> ().Stop ();
		continueButton.SetActive (true);
		stopTanButton.SetActive (false);
		character.gameObject.SetActive(true);
		salonVisits++;
		PlayerPrefs.SetInt ("tanning salon visits", salonVisits);
		assistant.player.newOffer ("exam");
		//LOW RISK 0-2
		//HIGH RISK 3-4
		//SELECTED CONVERSATION
		int selectedConversation = Random.Range(0,1);
		switch((int)tanAmount) {
		case 0:
		case 1:
		case 2:
			switch(selectedConversation) {
			case 0:
				//APB 1
				assistant.GetComponent<PlayerBehavior>().trackEvent(4, "0", "APB1", "Ray", selectedConversation.ToString());
				assistant.dialogue.text = "Looking good! We've got a great promotion until the end of the month if you want to keep it that way.";
				assistant.response.text = "Thanks!";
				break;
			case 1:
				//HB 5
				assistant.GetComponent<PlayerBehavior>().trackEvent(4, "0", "HB5", "Ray", selectedConversation.ToString());
				assistant.dialogue.text = "Hope you enjoyed your stay. Come back and tan all you want, our facilities are completely safe!";
				assistant.response.text = "Thanks!";
				break;
			}
			break;
		case 3:
		case 4:
			switch(selectedConversation) {
			case 0:
				//APB 1
				assistant.GetComponent<PlayerBehavior>().trackEvent(4, "0", "APB1", "RAY", selectedConversation.ToString());
				assistant.dialogue.text = "Great tan, you look super toned! Have a nice day!";
				assistant.response.text = "Thanks!";
				break;
			case 1:
				//HB7
				assistant.GetComponent<PlayerBehavior>().trackEvent(4, "0", "HB7", "RAY", selectedConversation.ToString());
				assistant.dialogue.text = "You might have stayed in there too long, good thing you're young!";
				assistant.response.text = "Good to know!";
				break;
			}
			break;
		}
		int tan = (int)tanAmount;
		//TODO: Send analytics?
		Debug.Log("Setting tan to : " + tan);

//		assistant.player.setTan (tan);
		character.setTone (tan);

		if (salonVisits >= visitsRequiredForMole) {
			character.getMole (salonVisits);
		}
		assistant.player.tan = tan;
		assistant.player.takeAction (false);
		assistant.dialogControls.SetActive (true);

	}

	public void setTanAmount(float t) {
		tanAmount = t;
	}
}
