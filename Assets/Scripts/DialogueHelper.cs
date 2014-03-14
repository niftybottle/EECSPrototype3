using UnityEngine;
using System.Collections;
//This requires a modified PT_XMLReader, the one included with this project. This PT_XMLReader makes PT_XMLHashList enumerable.
public class DialogueHelper : MonoBehaviour {

	public Hashtable diaHash;
	public string currentText;
	public string[] currentOptions;

	public DoubleHealthScript health;
	private float oldscreenwidth = 820;
	private float oldscreenheight = 495;
	private float screenheight = Screen.height;
	private float screenwidth = Screen.width;

	//Height and width of the textbox
	private float bgHeight = 150;
	private float bgWidth;
	//The background image for the textbox
	public Texture2D TextBoxBG;
	public float upCornery;
	public float upCornerx;
	//Margin between edge of the text box background and anything in the text box
	public float margin = 10;
	
	public PT_XMLReader reader;
	public bool start = false;

	//Get and Set methods for private variables. Currently, dialogue boxes display centered on the screen, at the bottom
	public void setbgHeight(float newHeight){
		bgHeight = newHeight;
		upCornery = screenheight - newHeight;
	}
	public float getbgHeight(){
		return bgHeight;
	}
	public void setbgWidth(float newWidth){
		bgWidth = newWidth;
		upCornerx = screenwidth/2 - newWidth/2;
	}
	public float getbgWidth(){
		return bgWidth;
	}
	public void setScreenWidth(float newWidth){
		screenwidth = newWidth;
		upCornerx = screenwidth/2 - newWidth/2;
	}
	public float getScreenWidth(){
		return screenwidth;
	}
	public void setScreenHeight(float newHeight){
		screenheight = newHeight;
		upCornery = screenheight - newHeight;
	}
	public float getScreenHeight(){
		return screenheight;
	}
	//End Get and Set methods

	//Parses a dialogue tree from an XML string. The resulting dialogues are stored as a hash map in diaHash (<string title>,
	//<DialogueObject>). These are displayed in the OnGUI function - edit there if you want to change their display. This
	//function just creates the hashMap and the Objects inside it.

	//XML tags:
	//<XML> - encloses the document
	//	<DOb name = "name"> - encloses a piece of dialogue; name is how this dialogue piece is referenced
	//		<DText> - encloses the text for this piece of dialogue, it is assummed that DOb will have exactly 1 of these
	//			the text
	//		</DText>
	//		<DOp (GoTo="name")> - encloses a response to the dialogue which is selectable by the player; GoTo is a reference
	//							to a DOb name, it is where the dialogue will go when this response is selected, it is optional
	//			<DResp> - encloses the text for this response, it is assumed that DOp will have exactly 1 of these
	//				the text
	//			</DResp>
	//			<Com  Obj="name" Fn="name" (Par="par" (ParType = "type"))/> - represents a command to be executed
	//					Obj - the name of the GameObject which has the script containing the function attached. This is *not*
	//						the name of the script
	//					Fn - the name of the function to be called (such as "setScreenWidth"). Do not include parentheses.
	//					Par - Optional, a parameter to be passed to the fn. This must be in string form, and convertable to
	//						the desired type from a string
	//					ParType - Optional, what type to convert Par to. If not specified, it assumed that it should remain
	//						a string. Possible values:
	//							"i" - convert to int
	//							"f" - convert to float
	//							"b" - convert to bool
	//		</DOp>
	//	</DOb>
	//</XML>
	public void parseDialogueTree(string XML){
		PT_XMLHashtable xht;
		int tempCount;
		int tempCount2;
		string[][] commands;
		DiaOptObj[] tempdoo;
		DialogueObj tempdob;
		reader.Parse(XML);
		xht = reader.xml["XML"][0];
		foreach(PT_XMLHashtable DOBxht in xht["DOb"]){
			tempdob = new DialogueObj();
			tempdob.text = DOBxht["DText"][0].text;
			if(DOBxht.HasKey("DOp")){
				tempdoo = new DiaOptObj[DOBxht["DOp"].Count];
				tempCount = 0;
				foreach(PT_XMLHashtable DOPxht in DOBxht["DOp"]){
					tempdoo[tempCount] = new DiaOptObj();
					tempdoo[tempCount].response = DOPxht["DResp"][0].text;
					if(DOPxht.ContainsAtt("GoTo")){
						tempdoo[tempCount].GoTo = DOPxht.att("GoTo");
					}
					if(DOPxht.HasKey("Com")){
						commands = new string[DOPxht["Com"].Count][];
						tempCount2 = 0;
						foreach(PT_XMLHashtable option in DOPxht["Com"]){
							if(option.HasAtt("ParType")){
								commands[tempCount2] = new string[4];
								commands[tempCount2][3] = option.att("ParType");
								commands[tempCount2][2] = option.att ("Par");
							}else if(option.HasAtt("Par")){
								commands[tempCount2] = new string[3];
								commands[tempCount2][2] = option.att("Par");
							}else{
								commands[tempCount2] = new string[2];
							}
							commands[tempCount2][0] = option.att("Obj");
							commands[tempCount2][1] = option.att("Fn");
							tempCount2++;
						}
						tempdoo[tempCount].commands = commands;
					}
					tempCount++;
				}
				tempdob.options = tempdoo;
			}
			diaHash.Add(DOBxht.att("name"),tempdob);
		}
	}

	//Set the defaults
	void Start () {
		reader = new PT_XMLReader();
		//This is the entry point for the dialogue, the first selected
		currentText = "Start";
		//Set background width and calculate the upper left hand corner of the background's coordinates.
		bgWidth = screenwidth;
		upCornery = screenheight - bgHeight;
		upCornerx = 0;
		diaHash = new Hashtable();
	}
	public void Restart (){
		currentText = "Start";
		diaHash = new Hashtable();
		start = false;
	}
	// Update is called once per frame, nothing here
	void Update () {
	
	}
	//This is where the display of the current dialogue items is located. 
	void OnGUI() {
		if(start){
			DialogueObj current = diaHash[currentText] as DialogueObj;
			//Draw the dialogue box background (TextBoxBG) and scale it to the height and width designated by bgWidth and bgHeight
			GUI.DrawTexture(new Rect(upCornerx,upCornery,bgWidth,bgHeight), TextBoxBG);
			//The area where all text and options are displayed
			GUILayout.BeginArea(new Rect(margin,upCornery+margin, bgWidth-(margin*2), bgHeight - (margin*2)));
			GUILayout.BeginVertical();
			//Display the current Dialogue Object's text
			GUILayout.Label(current.text);
			//For each option associated with the current object text, display a button
			foreach (DiaOptObj option in current.options){
				if(GUILayout.Button(option.response)){
					//When the button is selected:
					//Go to the designated dialogue option (go nowhere if no designated option)
					if(option.GoTo != null){
						currentText = option.GoTo;
					}
					//Execute any commands associated with the object
					if(option.commands != null){
						foreach(string[] command in option.commands){
							GameObject receiver = GameObject.Find(command[0]) as GameObject;
							//If there is a parameter associated with this string, parse it as specified and call the function with it.
							//If not specified, assume it is a string and pass it on as-is.
							if(command.Length > 3){
								if(command[3] == "i"){
									int par = int.Parse(command[2]);
									receiver.SendMessage(command[1], par);
								}else if (command[3] == "f"){
									float par = float.Parse(command[2]);
									receiver.SendMessage(command[1], par);
								}else if(command[3] == "b"){
									bool par = bool.Parse(command[2]);
									receiver.SendMessage(command[1], par);
								}
							}else if (command.Length == 3){
								receiver.SendMessage(command[1], command[2]);
							}else{
								//No parameter given, call the function without any parameters
								receiver.SendMessage(command[1]);
							}
						}
					}
				}
			}
			
			//End areas and such
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

}
//An object representing a piece of dialogue and any responses to it
public class DialogueObj {
	//The piece of dialogue
	public string text = null;
	//The possible responses
	public DiaOptObj[] options;

}
//An object representing a dialogue response, with where in the dialogue tree to go when this option is selected and any
//commands to execute then
public class DiaOptObj {
	public string response = null;
	public string GoTo = null;
	public string[][] commands;
}
