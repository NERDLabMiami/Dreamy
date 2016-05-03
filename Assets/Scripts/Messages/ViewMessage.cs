using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewMessage : MonoBehaviour {
	public Image profilePic;
	public Character character;
	public Text alias;
	public Text body;
	public Transform responseContainer;
	public GameObject responseTemplate;

	private Player player;
	private string characterEpiloguePath;
	private string epilogueStoryPath;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

	}
	private void startEpilogue() {
		
	}

	public void addResponse(Response r) {
		GameObject response = Instantiate(responseTemplate);
		response.GetComponent<ResponseOption>().response.text = r.text;
		int type = offerType (response, r);
		if (type == 5) {
			//TODO: parse path sender and cue epilogue
			response.GetComponent<Button> ().onClick.AddListener (() => {
				player.removeMessage (r.messageIndex);
				Inbox inbox = GameObject.FindGameObjectWithTag("Inbox").GetComponent<Inbox>();
				string characterPath = getStringFromResponse(r.path,0);
				if (characterPath.Equals("ignore")) {
				}
				else {
					
					string storyPath = getStringFromResponse(r.path, 2);
					inbox.populateEpilogue(characterPath, storyPath, character.name);
					inbox.epilogue.npc.characterAssignment = character.characterAssignment;
					inbox.epilogue.cue ();
					//TODO: Track Epilogue
					//BELIEF ID = Unhealthy, Healthy, 
					GetComponent<PlayerBehavior>().trackEvent(6, inbox.epilogue.endingType.ToString(),r.belief, characterPath,"none");

				}

			});

		}
		else if(type == 1) {
			response.GetComponent<Button> ().onClick.AddListener (() => {

				player.removeMessage (r.messageIndex);
				string p = getStringFromResponse(r.path,1);
				GetComponent<PlayerBehavior>().trackEvent(2, "IGNORE","none", p, "none");
				Debug.Log("Removing offer with path: " + p);
				int offerCount = PlayerPrefs.GetInt(p + "_offers", 0);
				PlayerPrefs.SetInt(p + "_offers", offerCount - 1);
				player.previewInbox.messageContainer.SetActive(true);
				Destroy (gameObject);
				player.refreshInbox();
				player.previewInbox.checkIfEmpty();

			});
		
		}
		else if (type != -1) {
			response.GetComponent<Button> ().onClick.AddListener (() => {
				//TODO: Track Event Accepting Offer

				player.removeMessage (r.messageIndex);
				string p = getStringFromResponse(r.path,0);
				GetComponent<PlayerBehavior>().trackEvent(2, "ACCEPT", "none", p, "none");
				Debug.Log("Removing offer with path: " + p);
				int offerCount = PlayerPrefs.GetInt(p + "_offers", 0);
				PlayerPrefs.SetInt(p + "_offers", offerCount - 1);
				player.previewInbox.checkIfEmpty();
				player.takeAction(true);
				//TODO: Check for piercing, hair offer types
				if (type == 6) {
					//piercing
					player.avatar.getPierced();
					player.previewInbox.messageContainer.SetActive(true);
					Destroy (gameObject);
					player.refreshInbox();
					player.previewInbox.checkIfEmpty();
					transform.parent.gameObject.SetActive(false);

				}
				else if (type == 7) {
					//haircut
					player.avatar.getHaircut();
					player.previewInbox.messageContainer.SetActive(true);
					Destroy (gameObject);
					player.refreshInbox();
					player.previewInbox.checkIfEmpty();
					transform.parent.gameObject.SetActive(false);
				}
				else {
					player.loadSceneNumber(type);
				}
			});
		} else {
			response.GetComponent<Button> ().onClick.AddListener (() => {
				//dialog_belief_id
				if (r.dialog_id != null) {
					respond (r.path, r.messageIndex, r.belief, r.dialog_id);
				}
				else {
					respond (r.path, r.messageIndex, r.belief, "none");

				}
					player.previewInbox.checkIfEmpty();
			});	

			}
		response.transform.SetParent(responseContainer, false);

	}

	int offerType(GameObject obj, Response r) {
		string[] pathArray = StringArrayFunctions.getMessage(r.path);
		if (pathArray[0] == "remove") {
			return 1;
		}
		if (pathArray [0] == "tanning") {
			return 2;
		}
		if (pathArray [0] == "love") {
			return 3;
		}
		
		if (pathArray [0] == "dermatologist") {
			return 4;
		}
		if (pathArray.Length > 1) {
			//has a character attached to it
			if (pathArray [1] == "epilogue") {

				Debug.Log("Epilogue: " + r.path);

				return 5;
			}
		}
		if (pathArray[0] == "piercing") {
			return 6;
		}
		if (pathArray[0] == "haircut") {
			return 7;
		}
	return -1;

	}

	string getStringFromResponse(string path, int index) {
		string[] pathArray = StringArrayFunctions.getMessage(path);
		Debug.Log("PATH IS " + pathArray.Length + " LONG");
		Debug.Log("PATH IS " + path);

		return pathArray[index];
	}
	/*
	bool isOffer(GameObject obj, Response r) {
		string[] pathArray = StringArrayFunctions.getMessage(r.path);
		if (pathArray [0] == "tanning") {
			obj.GetComponent<Button> ().onClick.AddListener (() => {
				player.removeMessage (r.messageIndex);
				player.loadSceneNumber(1);
			});
			return true;
		}
		if (pathArray [0] == "love") {
			obj.GetComponent<Button> ().onClick.AddListener (() => {
				player.removeMessage (r.messageIndex);
				player.loadSceneNumber(2);
			});
			return true;
		}

		if (pathArray [0] == "dermatologist") {
			Debug.Log("Skin Exam");
			obj.GetComponent<Button> ().onClick.AddListener (() => {
				player.removeMessage (r.messageIndex);
				player.loadSceneNumber(3);
			});
			return true;
		}


		return false;
	}
*/
	void respond(string path, int index, string belief, string dialog) {
		//TODO: Path update for just character
		GetComponent<PlayerBehavior>().trackEvent(2, "DLG", belief, StringArrayFunctions.getMessage(path)[0],dialog);
		player.takeAction (true);
		player.removeMessage(index);
		//TODO: Check threshold requirements if needed in mid conversation to add/remove response time
		if(StringArrayFunctions.getMessage(path)[1].Contains("deadend")) {
			//Player chose a dead end, unhook to allow for a new conversation
			player.unhook();
		}
		else {
			Debug.Log("NOT A DEADEND: " + path);
		}
		player.addMessage(path);
		player.refreshInbox();
		player.previewInbox.messageContainer.SetActive(true);
		Destroy (gameObject);

	}

	public void ignore() {
		player.previewInbox.messageContainer.SetActive(true);
		Destroy (gameObject);
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDisable() {
//		GameObject.FindGameObjectWithTag("Inbox").GetComponent<Inbox>().messageContainer.SetActive(true);
		Destroy (this.gameObject);
	}
}
