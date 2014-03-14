using UnityEngine;
using System.Collections;

public class TestDialogueServer : MonoBehaviour {
	public TextAsset ta;
	public string testXML;
	public DialogueHelper dh;
	// Use this for initialization
	void Start () {
		testXML = ta.text;
		dh.parseDialogueTree(testXML);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
