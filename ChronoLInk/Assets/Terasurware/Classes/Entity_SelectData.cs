using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_SelectData : ScriptableObject
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
		public int selectAmount;
		public string selectMain;
		public string select_01;
		public string select_02;
		public string select_03;
		public int nextindex_01;
		public int nextindex_02;
		public int nextindex_03;
	}
}

