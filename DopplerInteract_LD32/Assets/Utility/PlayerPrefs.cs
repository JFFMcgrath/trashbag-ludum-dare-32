/*
	PreviewLabs.PlayerPrefs

	Public Domain
	
	To the extent possible under law, PreviewLabs has waived all copyright and related or neighboring rights to this document. This work is published from: Belgium.
	
	http://www.previewlabs.com
	
*/

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace PreviewLabs
{
	public static class PlayerPrefs
	{
		private static Hashtable playerPrefsHashtable = new Hashtable();
		
		private static bool hashTableChanged = false;
		private static string serializedOutput = "";
		private static string serializedInput = "";
		
		private const string PARAMETERS_SEPERATOR = ";";
		private const string KEY_VALUE_SEPERATOR = ":";
		
		private static readonly string fileName = Application.persistentDataPath + "/VData.txt";
				
		static bool initialized = false;
		static bool isSecured = false;

		public const string HASH_PREFIX = "CS_";

		public static void SetSecured(bool secured){

			isSecured = secured;

		}
				
		public static void Initialize(){
			
			if(initialized){
				Debug.LogWarning("PlayerPrefs already initialized. Returning");
				return;
			}
			
			initialized = true;
			
			InitializeFileStream();

		}

		public static void ReloadFromFile(){

			playerPrefsHashtable.Clear ();
			serializedInput = "";
			serializedOutput = "";
			initialized = false;
			Initialize ();

		}
				
		static void InitializeFileStream(){
			
			StreamReader fileReader = null;

			if (File.Exists(fileName))
			{				
				fileReader = new StreamReader(fileName);
				
				serializedInput = fileReader.ReadLine();
				
				Deserialize();
				
				fileReader.Close();
			}

		}

		public static void DeleteAll()
		{
			
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
	
			UnityEngine.PlayerPrefs.DeleteAll();
			playerPrefsHashtable.Clear();

		}

		static string GetHashedKey(string key){

			if(isSecured){

				return Encryption.HashString (key,true);

			}

			return key;

		}
		
		public static bool HasKey(string key)
		{			

			if (string.IsNullOrEmpty (key)) {
				return false;
			}

			key = GetHashedKey (key);

			return playerPrefsHashtable.ContainsKey(key);
		}
		
		public static void SetString(string key, string value)
		{

			if (string.IsNullOrEmpty (key)) {
				return;
			}

			key = GetHashedKey (key);
							
			if(!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetInt(string key, int value)
		{

			if (string.IsNullOrEmpty (key)) {
				return;
			}

			key = GetHashedKey (key);

			if(!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetFloat(string key, float value)
		{

			if (string.IsNullOrEmpty (key)) {
				return;
			}

			key = GetHashedKey (key);

			if(!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static void SetBool(string key, bool value)
		{

			if (string.IsNullOrEmpty (key)) {
				return;
			}

			key = GetHashedKey (key);

			if(!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			
			hashTableChanged = true;
		}
		
		public static string GetString(string key)
		{			

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			
			return null;
		}
		
		public static string GetString(string key, string defaultValue)
		{

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static int GetInt(string key)
		{			

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (int) playerPrefsHashtable[key];
			}
			
			return 0;
		}
		
		public static int GetInt(string key, int defaultValue)
		{

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (int) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static float GetFloat(string key)
		{			

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (float) playerPrefsHashtable[key];
			}
			
			return 0.0f;
		}
		
		public static float GetFloat(string key, float defaultValue)
		{

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (float) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static bool GetBool(string key)
		{			

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (bool) playerPrefsHashtable[key];
			}
			
			return false;
		}
		
		public static bool GetBool(string key, bool defaultValue)
		{

			key = GetHashedKey (key);

			if(playerPrefsHashtable.ContainsKey(key))
			{
				return (bool) playerPrefsHashtable[key];
			}
			else
			{
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}
		
		public static void DeleteKey(string key)
		{

			if (!HasKey (key)) {
				return;
			}

			key = GetHashedKey (key);

			playerPrefsHashtable.Remove(key);
		}
		
		public static void Save()
		{
		
			if(hashTableChanged)
			{
				DateTime beginTime = DateTime.Now;

				Serialize();

				WriteToFile();

				DateTime endTime = DateTime.Now;

				Debug.Log("Time to serialize: "+ (endTime.Subtract(beginTime).TotalMilliseconds)+"\n"+serializedOutput);

				serializedOutput = "";
			
			}

		}
			
		static void WriteToFile(){
									
			StreamWriter fileWriter = null;

			File.Delete(fileName);

			fileWriter = null;//File.CreateText(fileName);
		
			if (fileWriter == null)
			{ 
				UnityEngine.Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
			}
			else{

				fileWriter.WriteLine(serializedOutput);
			
				fileWriter.Close();
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator myEnumerator = playerPrefsHashtable.GetEnumerator();
						
			StringBuilder stringBuilder = new StringBuilder(serializedOutput);
			StringBuilder microStringBuilder = new StringBuilder ();

			List<string> allSaveStrings = new List<string> ();

			while ( myEnumerator.MoveNext() )
			{
			
				string key, value, checksumValue = null, typeString;

				key = myEnumerator.Key.ToString();
				value = myEnumerator.Value.ToString ();

				typeString = myEnumerator.Value.GetType ().ToString ();

				if (isSecured) {

					checksumValue = Encryption.HashString (value,false);
					value = Encryption.EncryptString (value);

					if (value == null) {
						Debug.Log ("Null value serialized for key: " + key);
						continue;
					}

				}

				stringBuilder.Append(EscapeNonSeperators(key));
				stringBuilder.Append(KEY_VALUE_SEPERATOR);
				stringBuilder.Append(EscapeNonSeperators(value));
				stringBuilder.Append(KEY_VALUE_SEPERATOR);
				stringBuilder.Append(IntFromType(typeString));

				if (isSecured) {
				
					stringBuilder.Append(KEY_VALUE_SEPERATOR);
					stringBuilder.Append(checksumValue);

				}

				allSaveStrings.Add (stringBuilder.ToString ());
				stringBuilder.Remove (0, stringBuilder.Length);
							
			}

			//Shuffle so they aren't right next to each other
			allSaveStrings.Sort (ShuffleStrings);

			for (int i = 0; i < allSaveStrings.Count; i++) {

				if (i != 0) {
					stringBuilder.Append (PARAMETERS_SEPERATOR);
				}

				stringBuilder.Append (allSaveStrings [i]);

			}
			
			serializedOutput = stringBuilder.ToString();

			//serializedOutput = Encryption.EncryptString (serializedOutput);
			
		}

		static int ShuffleStrings(string a, string b){

			return UnityEngine.Random.Range (-1, 2);

		}
		
		private static void Deserialize()
		{

			//serializedInput = Encryption.DecryptString (serializedInput);

			if (string.IsNullOrEmpty (serializedInput)) {
				return;
			}

			string[] parameters = serializedInput.Split(new string[]{PARAMETERS_SEPERATOR},StringSplitOptions.None);
			
			if(playerPrefsHashtable != null){
				playerPrefsHashtable.Clear();
			}

			int requiredLength = 3;

			if (isSecured) {
				requiredLength = 4;
			}

			int index = 0;

			foreach(string parameter in parameters)
			{
				index++;

				if (string.IsNullOrEmpty (parameter)) {
					Debug.LogWarning ("Skipping null parameter @ " + index);
					continue;
				}

				string[] parameterContent = parameter.Split(new string[]{KEY_VALUE_SEPERATOR},StringSplitOptions.None);

				if(parameterContent.Length < requiredLength){
					UnityEngine.Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements ("+parameter+") @ " + index +"\n"+serializedInput);
					continue;
				}
				
				int typeInt = -1;
				
				int.TryParse(parameterContent[2],out typeInt);

				string key = DeEscapeNonSeperators (parameterContent [0]);
				string value = DeEscapeNonSeperators (parameterContent [1]);

				if (isSecured) {

					string decryptedValue = Encryption.DecryptString(value);

					if (decryptedValue == null) {
						Debug.Log ("Null value decrypted. Continuing...");
						continue;
					}

					string checksum = parameterContent [3];
					string validateChecksum = Encryption.HashString (decryptedValue,false);

					if (!validateChecksum.Equals (checksum)) {

						Debug.LogError ("Checksum failed for key: " + key + ".");
						continue;

					}

					object typedValue = GetTypeValue (typeInt, decryptedValue);

					if (typedValue != null) {

						playerPrefsHashtable.Add (key, typedValue);

					}

				} else {

					object typedValue = GetTypeValue (typeInt, value);

					if (typedValue != null) {

						playerPrefsHashtable.Add (key, typedValue);

					}

				}

				if(parameterContent.Length > requiredLength)
				{
					UnityEngine.Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
				}
			}
		}
		
		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(KEY_VALUE_SEPERATOR,"\\" + KEY_VALUE_SEPERATOR);
			inputToEscape = inputToEscape.Replace(PARAMETERS_SEPERATOR,"\\" + PARAMETERS_SEPERATOR);
			return inputToEscape;
		}
		
		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\" + KEY_VALUE_SEPERATOR, KEY_VALUE_SEPERATOR);
			inputToDeEscape = inputToDeEscape.Replace("\\" + PARAMETERS_SEPERATOR, PARAMETERS_SEPERATOR);
			return inputToDeEscape;
		}
		
		public const int STRING_TYPE = 0;
		public const int INT_TYPE = 1;
		public const int BOOL_TYPE = 2;
		public const int SINGLE_TYPE = 3;
		public const int NONE = 4;
		
		public static int IntFromType(string typeName){
			
			if(typeName == "System.String"){
				return STRING_TYPE;
			}
			else if(typeName == "System.Int32"){
				return INT_TYPE;
			}
			else if(typeName == "System.Boolean"){
				return BOOL_TYPE;
			}
			else if(typeName == "System.Single"){
				return SINGLE_TYPE;
			}	
			
			return NONE;
			
		}
		
		public static string TypeFromInt(int typeInt){
			
			if(typeInt == STRING_TYPE){
				return "System.String";
			}
			else if(typeInt == INT_TYPE){
				return "System.Int32";
			}
			else if(typeInt == BOOL_TYPE){
				return "System.Boolean";
			}
			else if(typeInt == SINGLE_TYPE){
				return "System.Single";
			}
			
			return "" + typeInt;
		}
		
		public static object GetTypeValue(int typeInt, string value)
		{
			if(typeInt == STRING_TYPE){
				return (object)value.ToString();
			}
			if(typeInt == INT_TYPE){

				int v = 0;

				if (Int32.TryParse (value, out v)) {
					return (object)v;
				}

				Debug.Log ("Could not convert: " + value + " to int32");

			}
			if(typeInt == BOOL_TYPE){

				bool b = false;

				if (Boolean.TryParse (value, out b)) {
					return (object)b;
				}

				Debug.Log ("Could not convert: " + value + " to boolean");

			}
			if(typeInt == SINGLE_TYPE){

				Single s = 0;

				if (Single.TryParse (value, out s)) {
					return (object)s;
				}

				Debug.Log ("Could not convert: " + value + " to single");

			}
			else
			{
				UnityEngine.Debug.LogError("Unsupported type: " + typeInt);
			}	
			
			return null;
		}
	}	
}