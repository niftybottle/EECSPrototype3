using UnityEngine;
using System.Collections;

public class GoodPersonController : MonoBehaviour {

	public PlayerController pc;
	public float x;
	public float y;
	public Component c;
	public TextAsset ta;
	public bool talking = false;
	public float dist;
	public GameObject controller;
	public DialogueHelper dh;
	public SceneControlScript scs;
	// Use this for initialization
	void Start () {
		pc = GameObject.Find("Character").GetComponent("PlayerController") as PlayerController;
		controller = GameObject.Find ("SceneControl");
		dh = (DialogueHelper)controller.GetComponent(typeof(DialogueHelper));
		scs = (SceneControlScript)controller.GetComponent(typeof(SceneControlScript));
		x = transform.position.x;
		y = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(!talking){
			dist = Mathf.Abs(pc.x - x) + Mathf.Abs(pc.y - y);
			if(dist<10){
				talking = true;
				scs.SetConvo(true);
				dh.parseDialogueTree(ta.text);
				dh.start = true;
			}
		}else if(scs.despawnTrigger){
			despawn();
		}
	}
	void despawn() {
		dh.Restart();
		scs.SetDespawn(false);
		scs.SetConvo(false);
		DestroyObject(gameObject);
	}
}
