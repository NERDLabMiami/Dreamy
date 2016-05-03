using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dermatologist : MonoBehaviour {
	private int conversationIndex = 0;
	private int selectedConversation;
	private int dermatologistVisits;
	public Text dermatologistDialog;
	public Text playerDialog;
	public Player player;
	public Character avatar;

	// Use this for initialization
	void Start () {
		conversationIndex = 0;
		dermatologistVisits = PlayerPrefs.GetInt("dermatologist visits",0);
		if (dermatologistVisits > 0) {
			dermatologistDialog.text = "You're back! Another skin exam?";
		}
		dermatologistVisits++;
		PlayerPrefs.SetInt("dermatologist visits", dermatologistVisits);
		switch(player.tan) {
		case 0:
		case 1:
		case 2:
			selectedConversation = Random.Range(0,1);
			break;
		case 3:
		case 4:
			selectedConversation = Random.Range(2,3);
			break;
		}

	}

	public void respond() {
		//TODO: Keep track of visits to the dermatologist
		//TODO: With assets in place, transition between close up of dermatologist and player's face
		//TODO: With assets in place, animate transitions
		if (conversationIndex == 2) {
			player.visitDermatologist();
		}
		if (conversationIndex == 5) {
			//add another line?
			player.reduceTan ();
			player.loadSceneNumber (1);
		}
		else {
			switch(player.tan) {
			case 0:
			case 1:
				//LOW RISK
				if (selectedConversation == 0) {
					Debug.Log("USING LOW1");
					dermatologistDialog.text = player.getDermatologistMessage(conversationIndex,"LOW1");
					playerDialog.text = player.getDermatologistResponse(conversationIndex, "LOW1");
					GetComponent<PlayerBehavior>().trackEvent(5,conversationIndex.ToString(), "APH11", "DERMATOLOGIST", selectedConversation.ToString());
					GetComponent<PlayerBehavior>().trackEvent(5,conversationIndex.ToString(), "HT51", "DERMATOLOGIST", selectedConversation.ToString());
				}
				else {
					Debug.Log("USING LOW2");
					dermatologistDialog.text = player.getDermatologistMessage(conversationIndex,"LOW2");
					playerDialog.text = player.getDermatologistResponse(conversationIndex, "LOW2");
					GetComponent<PlayerBehavior>().trackEvent(5,conversationIndex.ToString(), "APH11", "DERMATOLOGIST", selectedConversation.ToString());
					GetComponent<PlayerBehavior>().trackEvent(5,conversationIndex.ToString(), "HT51", "DERMATOLOGIST", selectedConversation.ToString());
				}					
				break;
			case 2:
			case 3:
			case 4:
				//HIGH RISK
				if (selectedConversation == 2) {
					Debug.Log("USING HIGH1");
					dermatologistDialog.text = player.getDermatologistMessage(conversationIndex, "HIGH1");
					playerDialog.text = player.getDermatologistResponse(conversationIndex, "HIGH1");
					GetComponent<PlayerBehavior>().trackEvent(5, conversationIndex.ToString(), "APH12", "DERMATOLOGIST", selectedConversation.ToString());
					GetComponent<PlayerBehavior>().trackEvent(5, conversationIndex.ToString(), "HT52", "DERMATOLOGIST", selectedConversation.ToString());
				}
				else {
					Debug.Log("USING HIGH2");
					dermatologistDialog.text = player.getDermatologistMessage(conversationIndex, "HIGH2");
					playerDialog.text = player.getDermatologistResponse(conversationIndex, "HIGH2");
					GetComponent<PlayerBehavior>().trackEvent(5, conversationIndex.ToString(), "APH12", "DERMATOLOGIST", selectedConversation.ToString());
					GetComponent<PlayerBehavior>().trackEvent(5, conversationIndex.ToString(), "HT52", "DERMATOLOGIST", selectedConversation.ToString());
				}
				break;
			}
			conversationIndex++;
		}
	}

}
