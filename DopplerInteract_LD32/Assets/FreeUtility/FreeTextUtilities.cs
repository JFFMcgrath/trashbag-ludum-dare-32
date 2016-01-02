using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FreeTextUtilities
{

	const char ROW_DIVIDER = '\n';
	const char CELL_DIVIDER = '\t';
	const char SUB_CELL_DIVIDER = '|';
	static StringBuilder _stringBuilder = new StringBuilder();
	public const int NONE = -1;

	public static string[] TextToRows(string text){
	
		return TextToRows(text,true);
	
	}

	public static string[] TextToRows(string text, bool ignoreHeader){
		
		string[] splitText = text.Split(ROW_DIVIDER);
		
		if(ignoreHeader){
		
			string[] resultText = new string[splitText.Length-1];
			
			Array.Copy(splitText,1,resultText,0,resultText.Length);
			
			return resultText;
			
		}
		else{
		
			return splitText;
		
		}
		
	}
	
	public static string RowsToText(List<string> rows){
	
		_stringBuilder.Remove(0,_stringBuilder.Length);
		
		for(int i = 0; i < rows.Count; i++){
		
			_stringBuilder.Append(rows[i]);
			
			if(i != rows.Count - 1){
			
				_stringBuilder.Append(ROW_DIVIDER);
			
			}
		
		}
		
		return _stringBuilder.ToString();
	
	}

	public static string[] RowToColumns(string row){
	
		string[] columns = row.Split(CELL_DIVIDER);
	
		return columns;
	
	}
	
	public static string ColumnsToString(List<string> columns){
	
		_stringBuilder.Remove(0,_stringBuilder.Length);
	
		for(int i = 0; i < columns.Count; i++){
			
			_stringBuilder.Append(columns[i]);
			
			if(i != columns.Count - 1){
				
				_stringBuilder.Append(CELL_DIVIDER);
				
			}
			
		}
		
		return _stringBuilder.ToString();
	
	}
	
	public static bool WordStringToBoolean(string str){
	
		if(str == "YES"){
			return true;
		}
		else{
			return false;
		}
	
	}
	
	public static string BooleanToString(bool b){
	
		if(b){
			return "1";
		}
		else{
			return "0";
		}
	
	}
	
	public static int StringToInt(string str){
	
		int ret = NONE;
		
		if(!int.TryParse(str,out ret)){
		
			ret = NONE;
		
		}
		
		return ret;
	
	}
	
	public static string IntToString(int i){
	
		return i.ToString();
	
	}
	
	public static float StringToFloat(string str){
	
		float ret = NONE;
		
		if(!float.TryParse(str,out ret)){
			
			Debug.LogWarning("Cannot parse string to float: " + str);
			ret = NONE;
			
		}
		
		return ret;
	
	}
	
	public static string FloatToString(float f){
	
		return f.ToString();
	
	}
	
	public static FreeCoordinate StringToFreeCoordinate(string str){
	
		FreeCoordinate ret = new FreeCoordinate(NONE,NONE);
	
		string[] cStr = str.Split(SUB_CELL_DIVIDER);
		
		if(cStr.Length != 2){
			
			Debug.LogWarning("Cannot parse string to coordinate: " + str);
						
		}
		else{
		
			int x = NONE,y = NONE;
		
			if(!int.TryParse(cStr[0],out x)){
				Debug.LogWarning("Cannot parse string to coordinate: " + str + " X component: " + cStr[0]);
			}
			
			if(!int.TryParse(cStr[1],out y)){
				Debug.LogWarning("Cannot parse string to coordinate: " + str + " Y component: " + cStr[1]);
			}
			
			ret.Reset(x,y);
		
		}
		
		return ret;
	
	}
	
	public static string FreeCoordinateToString(FreeCoordinate c){
	
		_stringBuilder.Remove(0,_stringBuilder.Length);
	
		_stringBuilder.Append(c.x);
		_stringBuilder.Append(SUB_CELL_DIVIDER);
		_stringBuilder.Append(c.y);
	
		return _stringBuilder.ToString();
	
	}
}


