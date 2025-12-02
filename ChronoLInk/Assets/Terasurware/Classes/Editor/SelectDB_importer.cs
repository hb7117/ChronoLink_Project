using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class SelectDB_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/ExcelDB/SelectDB.xlsx";
	private static readonly string exportPath = "Assets/Resources/ExcelDB/SelectDB.asset";
	private static readonly string[] sheetNames = { "selectData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_SelectData data = (Entity_SelectData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_SelectData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_SelectData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_SelectData.Sheet s = new Entity_SelectData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_SelectData.Param p = new Entity_SelectData.Param ();
						
					cell = row.GetCell(0); p.index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.selectAmount = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.selectMain = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.select_01 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.select_02 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.select_03 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.nextindex_01 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.nextindex_02 = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.nextindex_03 = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
