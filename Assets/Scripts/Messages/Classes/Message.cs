using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Message {
	public string sender;
	public string alias;
	public string subject;
	public string path;
	public string passage;
	public string body;
	public string belief = "";
	public string dialog_id;
	public int index;
	public List<Response> responses;

	public Message() {
		responses = new List<Response>();

	}
}
