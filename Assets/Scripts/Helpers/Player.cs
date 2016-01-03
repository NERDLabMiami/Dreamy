﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public int tan = 0;
	public int style = 0;
	public int attractiveness = 0;
	public int cancerRisk = 0;
	public int actionsLeft = 0;
	public int daysLeft = 0;
	public int dermatologistVisits = 0;
	public TextAsset potentialMessages;
	public bool reset = false;
	public List<Message> inbox;
	public Inbox previewInbox;
	public Profile profile;
	public Character avatar;
	public Animator progress;
	public GameObject loadingScreen;

	public JSONNode json;
	private List<string> messageList;
	public int daysBetweenChangeInTan = 2;
	// Use this for initialization
	void Start () {
			json = JSON.Parse(potentialMessages.ToString());
			string[] storedMessages = Prefs.PlayerPrefsX.GetStringArray("messages", null, 0);
			messageList = storedMessages.OfType<string>().ToList();
			inbox = new List<Message>();
			refreshInbox();
			populateStats();
			updateProfile();
	}
	public void setGenderPreference(string gender) {
		PlayerPrefs.SetString ("gender preference", gender);

	}
	public string getFinalStory(string character, string story) {
		if (json[character] != null) {
			return json[character]["epilogue"][story];
		}
		else {
			return "no story available";
		}
	}

	public int matches(string characterPath) {
		//Get character's attractiveness, between 0 and 100 to set as the initial response time

		if (json[characterPath] != null) {
		
			int responseTime = json [characterPath] ["requirements"] ["love"].AsInt;

			//Get the difference between the two characters attractiveness. NPC with 50, player with 20, response time is 30. 
			responseTime -= attractiveness;
			Debug.Log ("Accounting for attractiveness, response time is now " + responseTime);

			if (profile.character.wearingHairAccessory() || profile.character.wearingHeadwear()) {
				responseTime += json[characterPath]["requirements"]["accessories"]["headwear"].AsInt;
			}

			if (profile.character.wearingPiercing()) {
				responseTime += json[characterPath]["requirements"]["accessories"]["piercing"].AsInt;
			}


			if (profile.character.wearingGlasses ()) {
				responseTime += json [characterPath] ["requirements"] ["accessories"] ["glasses"].AsInt;
				Debug.Log ("Liking the glasses, response time is now " + responseTime);
			}

			if (profile.character.wearingTie ()) {
				responseTime += json [characterPath] ["requirements"] ["accessories"] ["tie"].AsInt;
				Debug.Log ("Liking the tie, response time is now " + responseTime);

			}


			if (tan < json [characterPath] ["requirements"] ["tan"].AsInt) {
				//must meet tan requirement. If they don't, this gets manually pushed to 9999 to be unresponsive.
				Debug.Log ("TAN IS " + tan + " and requirement is " + json [characterPath] ["requirements"] ["tan"].AsInt);
				responseTime = 9999;
				Debug.Log ("Tan not appropriate, response time is now " + responseTime);

			}


			//If the response time is negative, we just set it to 0 so the NPC responds instantly

			if (responseTime < 0) {
				responseTime = 0;
				Debug.Log ("So attractive, response is immediate");
			}
			Debug.Log ("Match will respond in " + responseTime + " days");
			return responseTime;
		} else {
			return 9999;
		}
	}

	public void refreshInbox() {
		inbox.Clear();
		for (int i = 0; i < messageList.Count; i++) {
			string[] messageParts = StringArrayFunctions.getMessage(messageList[i]);
			if (int.Parse(messageParts[2]) <= 0) {
				//TODO: Check tan requirements. not sure if this is the best place
				if (tan <= json[messageParts[0]]["requirements"]["tan"].AsInt) {
					//Don't add message

				}
				else {
				}
					//new message, add to list
					Message message = new Message();
					message.index = i;
					message.path = messageList[i];
					message.sender = messageParts[0];
					message.passage = messageParts[1];
					message.subject = json[message.sender][message.passage]["subject"];
					message.alias = PlayerPrefs.GetString(message.sender, "");
					message.subject = message.subject.Replace("%C", message.alias);
					
					message.body = json[message.sender][message.passage]["message"];
					JSONNode responses = json[message.sender][message.passage]["responses"];
					for (int j = 0; j < responses.Count; j++) {
						Response r = new Response(responses[j]["path"], responses[j]["response"], responses[j]["time"].AsInt, i);
						message.responses.Add(r);
					}

					inbox.Add(message);

			}
		}

		if (previewInbox != null) {
			previewInbox.clear();
			updatePreviewBox();
		}
	}

	public void updatePreviewBox() {
		for (int i = 0; i < inbox.Count; i++) {
			previewInbox.addMessage(inbox[i]);
		}
	}


	private void saveMessageList() {
		Prefs.PlayerPrefsX.SetStringArray("messages", messageList.ToArray());
	}

	public void addMessage(string path) {
		messageList.Add(path);
		saveMessageList();
		Debug.Log("Adding Message : " + path);
	}

	public void removeMessage(int index) {
		messageList.RemoveAt(index);
		saveMessageList();
	}

	public void removeAllMessages() {
		messageList.Clear();
		saveMessageList();
	}
	
	private void saveProgress(int actions, int days) {
		PlayerPrefs.SetInt("actions left", actions);
		PlayerPrefs.SetInt("days left", days);
	}
	
	public void newOffer(string type) {
		JSONNode offers = json [type].AsObject;
		int selectedOffer = Random.Range (0, offers.Count);
		JSONNode offer = offers [selectedOffer].AsObject;
		Debug.Log (type + " offers :" + offers.Count + " selected " + selectedOffer);
		if (offer ["path"] != null) {
			int offerCount = PlayerPrefs.GetInt(offer["path"] + "_offers", 0);
			if (offerCount <= 0) {
				addMessage (offer ["path"]);
				offerCount++;
				PlayerPrefs.SetInt(offer["path"] + "_offers", offerCount);
			}
		} else {
			Debug.Log("Offer path doesn't exist..." + type);
		}
	}


	public string getDermatologistMessage(int index) {
		JSONNode dermatologistMessage = json ["doctor"]["conversation"][index]["doctor"];		
		return dermatologistMessage;
	}

	public string getDermatologistResponse(int index) {
		JSONNode dermatologistResponse = json ["doctor"]["conversation"][index]["patient"];		
		return dermatologistResponse;
	}

	public string getTanningSalonAssistantMessage(int index) {
		JSONNode tanningSalonMessage = json ["salon"] ["conversation"] [index] ["assistant"];
		return tanningSalonMessage;
	}

	public string getTanningSalonAssistantResponse(int index) {
		JSONNode tanningSalonMessage = json ["salon"] ["conversation"] [index] ["response"];
		return tanningSalonMessage;
	}

	private void newDay() {

		//ADD TANNING OFFER
		newOffer("tanning");
		newOffer ("love");

		for(int i = 0; i < messageList.Count; i++) {
				//iterate through inbox, reduce wait time for each message
			Debug.Log("MESSAGE LIST:" + i + " " + messageList[i]);
				string[] message = StringArrayFunctions.getMessage (messageList[i]);
				int currentDuration = int.Parse(message[2]);
				
				if(currentDuration > 0) {
				//TODO: Check tan requirements here and if not met, add 1?
				//int requiredTan = json[message[0]][message[1]]["thresholds"]["tan"].AsInt;
				int requiredTan = json[message[0]]["requirements"]["tan"].AsInt;
				if (requiredTan <= tan) {
						Debug.Log("Meets tan requirements, counting down to conversation");
						currentDuration-=1;

					}
					else {
						Debug.Log("Tan requirement not met, holding off. Required tan of " + requiredTan + " to proceed");
					}
				}
			//TODO: Look at parts of message
				messageList[i] = message[0] + "/" + message[1] + "/" + currentDuration.ToString();
			}
			saveMessageList();
		}

	public void setStyle() {
		takeAction(true);
	}

	public bool takeAction(bool takeTolls) {
		if (actionsLeft > 1) {
			actionsLeft--;
			saveProgress(actionsLeft, daysLeft);
			if (progress) {
				if (actionsLeft == 1) {
					progress.SetTrigger("one");
				}
				if (actionsLeft == 2) {
					progress.SetTrigger("two");
				}
			}
			return true;
		}
		else {
			if (progress) {
				progress.SetTrigger("three");
			}
			daysLeft--;
			newDay();
			if (takeTolls) {
				//TAKE TOLLS ON ATTRACTIVENESS, TAN
				if (attractiveness > 0) {
					setAttractiveness (attractiveness - 1);
				}

				int daysSinceLastChangeInTan = PlayerPrefs.GetInt ("days since last change in tan", 0);
				daysSinceLastChangeInTan++;

				if (daysBetweenChangeInTan >= daysSinceLastChangeInTan) {
					Debug.Log("Changing Tan");
					tan--;
					if (tan < 0) {
						tan = 0;
					}
					avatar.setTone(tan);
					daysSinceLastChangeInTan = 0;
				}

				PlayerPrefs.SetInt("days since last change in tan", daysSinceLastChangeInTan);


			}
			refreshInbox();
			if (daysLeft < 0) {
				//GAME OVER
				Debug.Log("Days have run out");
				GetComponent<Home>().gameOverMessage.SetActive(true);
				return false;
			}
			else {
				actionsLeft = 3;
				saveProgress(actionsLeft, daysLeft);
				return true;
			}
		}

	}

	public void setTan(int amount) {
		if (tan < amount) {
			//tan increased, increase risk by amount tanned
			int riskIncrease = amount - tan;
			cancerRisk += riskIncrease;
			PlayerPrefs.SetInt("cancer risk", cancerRisk);
		}
		tan = amount;
//		profile.character.transform 
		PlayerPrefs.SetInt("tan", tan);
	}

	public void visitDermatologist() {
		if (cancerRisk > 1) {
			cancerRisk--;
		}
		avatar.removeMole ();
		PlayerPrefs.SetInt ("cancer risk", cancerRisk);
	}

	public void setAttractiveness(int amount) {
		//TODO: Decide if attractiveness is additive or just personal best in the love game
		attractiveness = amount;		
		PlayerPrefs.SetInt("attractiveness", amount);

	}

	public void resetStats() {
//		PlayerPrefs.DeleteKey ("messages");
		PlayerPrefs.DeleteAll();
		removeAllMessages();
		//Debug.Log (json.ToString());
		/*
		JSONNode offers = json ["characters"].AsObject;
		
		Debug.Log ("Amount of Offers: " + offers.Count);
		int selectedOffer = Random.Range (0, offers.Count);
		JSONNode offer = offers [selectedOffer].AsObject;



*/
		List<string> list = new List<string>();

		for (int i = 0; i < json["characters"].Count; i++) {
			list.Add(json["characters"][i]);
		}

		Prefs.PlayerPrefsX.SetStringArray("men", list.ToArray());
		Debug.Log("Saved " + list.Count + " characters");
		PlayerPrefs.SetInt("tan", 0);
		PlayerPrefs.SetInt("attractiveness", Random.Range(0,10));
		PlayerPrefs.SetInt("style", Random.Range(0,10));
		PlayerPrefs.SetInt("cancer risk", 0);
		PlayerPrefs.SetInt("dermatologist visits", 0);
		PlayerPrefs.SetInt("actions left", 3);
		PlayerPrefs.SetInt("days left", 30);
		PlayerPrefs.SetInt("tutorial", 0);

	}
	
	private void populateStats() {
		attractiveness = PlayerPrefs.GetInt("attractiveness", 0);
			//TODO: Style becomes an avatar choice with accessories
		style = PlayerPrefs.GetInt("style", 0);
		actionsLeft = PlayerPrefs.GetInt("actions left", 0);
		daysLeft = PlayerPrefs.GetInt("days left", 0);
		cancerRisk = PlayerPrefs.GetInt("cancer risk", 0);
		tan = PlayerPrefs.GetInt("tan",0);
		dermatologistVisits = PlayerPrefs.GetInt("dermatologist visits", 0);

		if (profile) {
//			profile.heart.CrossFadeAlpha (Remap (attractiveness, 0, 100, 0, 1), 3, true);
			avatar.setTone(tan);

			if (attractiveness <= 5) {
				profile.heart.GetComponent<Animator>().SetTrigger("one");
			}

			if(attractiveness > 5 && attractiveness <= 10) {
				profile.heart.GetComponent<Animator>().SetTrigger("two");
			}

			if (attractiveness <= 20 && attractiveness > 10) {
				profile.heart.GetComponent<Animator>().SetTrigger("three");
			}

			if (attractiveness > 20) {
				profile.heart.GetComponent<Animator>().SetTrigger("four");
			}

		}

	}

	public void updateProfile() {
		if (profile) {
//			profile.actionsLeft.text = actionsLeft.ToString();
			profile.daysLeft.text = daysLeft.ToString("0 days left to get a date");
			profile.messageCount.text = inbox.Count.ToString();
			if(inbox.Count <= 0) {
				previewInbox.emptyMailboxMessage.SetActive(true);
			}
			else {
				previewInbox.emptyMailboxMessage.SetActive(false);
			}
			
		}
	}

	public string fetchMaleName() {
		int numNames = json["names"]["men"].Count;
		return json["names"]["men"][Random.Range(0,numNames)];
	}

	public string fetchMotto() {
		int numMottos = json["mottos"].Count;
		return json["mottos"][Random.Range (0, numMottos)];
	}

//	StartCoroutine (loadHome ());

	public void loadSceneNumber(int level) {
		loadingScreen.SetActive(true);
		StartCoroutine (loadLevel(level));
	}

	IEnumerator loadLevel(int level) {
		AsyncOperation async = SceneManager.LoadSceneAsync (level);
		while (!async.isDone) {
			Debug.Log ("ASYNC: " + async.progress);
			yield return async;		
		}
		loadingScreen.SetActive (false);
		Debug.Log ("Loading complete");
	}


	public void returnHome(){
		loadSceneNumber(1);
	}


	public float Remap (this float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
		
}
