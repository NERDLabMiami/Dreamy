using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Response  {
	public string path;
	public string text;
	public string belief = "";
	public string dialog_id = "";
	public int responseTime;
	public int messageIndex;

	//	public Player player;
	//	private Inbox inbox;
	//	public Text response;

	public Response(string p, string txt, int rt, int index, string b, string d) {
		responseTime = rt;
//		if p.Contains("deadend");
		path = p + "/" + responseTime;
		text = txt;
		belief = b;
		dialog_id = d;
		messageIndex = index;
	}

}
