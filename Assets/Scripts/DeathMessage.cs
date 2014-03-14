using UnityEngine;
using System.Collections;

public class DeathMessage : MonoBehaviour {
	public bool dead = false;
	public GameObject go;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void setDead(bool b){
		dead = b;
	}
	void OnGUI() {
		if(dead){
			GameObject.Destroy(go);
			GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2 - 15, 400, 30), "You had a heart attack and died. Try calming down next time");
		}
	}
}
