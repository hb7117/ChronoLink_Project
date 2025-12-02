using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_Dialogue : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int index;
		public int speakerUIindex;
		public string name;
		public string dialogue;
		public string characterPath;
		public string BackGroundPath;
		public int tweenType;
		public int nextindex;
		public int selectIndex;
		public string nextScene;
	}
}

