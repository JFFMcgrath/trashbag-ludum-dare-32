using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class Encryption
{
	//const string ENCRYPTION_KEY = "Warduck";
	const string SALT_STRING = "Goobs";
	const string ENCRYPTED_SUFFIX = "_EC";
	private static string _b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijk.mnopqrstuvwxyz-123456789+/=";

	static MD5 _md5;
	static MD5 md5{
		get{
			if(_md5 == null){
				_md5 = new MD5CryptoServiceProvider();
			}
			return _md5;
		}
	}


	static int _MODULO = -1;
	static int _SEED = -1;
	static int SEED{
		get{


			if (_SEED == -1) {

				int s = 0;

				byte[] b = Encoding.ASCII.GetBytes (SystemInfo.deviceUniqueIdentifier);

				for (int i = 0; i < b.Length; i++) {
					s += (int)b [i];
				}

				Debug.LogWarning ("Modulo is: " + s);

				_MODULO = s;

				b = Encoding.UTF8.GetBytes(SystemInfo.deviceType.ToString());

				for (int i = 0; i < b.Length; i++) {
					s += (int)b [i];
				}

				Debug.LogWarning ("Seed is: " + s);

				_SEED = s;

			}

			return _SEED;

		}
	}

	static string GetEncryptionKey(string text){

		if (string.IsNullOrEmpty (text)) {
			return null;
		}

		int length = text.Length;

		return GetKeyForLength (length);

	}

	static string GetDecryptionKey(string text){

		if (string.IsNullOrEmpty (text)) {
			return null;
		}

		int length = text.Length;

		return GetKeyForLength (length);

	}

	public static string GetXORString(string key, string input)
	{

		if (string.IsNullOrEmpty (key) || string.IsNullOrEmpty (input)) {
			return null;
		}

		StringBuilder sb = new StringBuilder();
		for(int i=0; i < input.Length; i++)
			sb.Append((char)(input[i] ^ key[(i % key.Length)]));
		String result = sb.ToString ();

		return result;
	}

	static string GetKeyForLength(int length){

		UnityEngine.Random.seed = SEED;

		int skip = length % _MODULO;

		StringBuilder sb = new StringBuilder ();

		byte[] b = new byte[1];

		for (int i = 0; i < length; i++) {

			int index = UnityEngine.Random.Range (0, _b64.Length);

			char c = _b64[index];

			sb.Append (c);

			UnityEngine.Random.seed = SEED + (skip * i);

		}

		return sb.ToString ();

	}

	public static string EncryptString(string str)
	{

		string xor = GetXORString (GetEncryptionKey (str), str);

		xor = WWW.EscapeURL (xor);

		return (xor);

	}

	public static string DecryptString(string str){

		string toXOR = (str);

		toXOR = WWW.UnEscapeURL (str);

		return GetXORString (GetEncryptionKey (toXOR), toXOR);

	}

	static Dictionary<string, string> hashCache = new Dictionary<string, string>();

	public static string HashString(string str, bool cacheValue)
	{
	
		string originalString = str;
	
		if(cacheValue){
			if(hashCache.ContainsKey(originalString)){
				return hashCache[originalString];
			}
		}
	
		str += SALT_STRING;
		
		//Declarations
		Byte[] originalBytes;
		Byte[] encodedBytes;
		
		originalBytes = ASCIIEncoding.Default.GetBytes(str);
		encodedBytes = md5.ComputeHash(originalBytes);
		
		//Convert encoded bytes back to a 'readable' string
		string returnValue = BitConverter.ToString(encodedBytes);

		if(cacheValue){
			hashCache.Add(originalString,returnValue);
		}

		return returnValue;

	}

	public static string EncodeString(string str){

		int padding = str.Length % 4;

		for (int i = 0; i < padding; i++) {
			str += '=';
		}

		try{

			byte[] encoding = Encoding.UTF8.GetBytes (str);
			string encoded = Convert.ToBase64String (encoding);
			return encoded;

		}catch(Exception e){

			Debug.LogWarning ("Exception: " + e.ToString ());

		}

		return null;

	}

	public static string DecodeString(string str){

		if (str.Length % 4 != 0) {
			Debug.Log ("Skipping invalid b64 string");
			return null;
		}

		try{

			byte[] decoding = Convert.FromBase64String (str);
			string decoded = Encoding.UTF8.GetString (decoding);

			return decoded;

		}
		catch(Exception e){

			Debug.LogWarning ("Exception: " + e.ToString ());

		}

		return null;
		
	}

	/*public static void SaveEncrypted(string key, string value){
	
		string ecKey = key + ENCRYPTED_SUFFIX;

		string encoded = EncodeString (value);

		PreviewLabs.PlayerPrefs.SetString(key,encoded);
		PreviewLabs.PlayerPrefs.SetString(ecKey,EncryptString(value));
		PreviewLabs.PlayerPrefs.Save();
	
	}
	
	public static string LoadEncryptedString(string key){
	
		string ecKey = key + ENCRYPTED_SUFFIX;
		
		if(!PreviewLabs.PlayerPrefs.HasKey(key)){
			Debug.LogWarning("No entry in save for: " + key);
			return "";
		}
		
		if(!PreviewLabs.PlayerPrefs.HasKey(ecKey)){
			Debug.LogWarning("No entry in save for encrypted: " + ecKey);
			return "";
		}
		
		string ecValue = PreviewLabs.PlayerPrefs.GetString(ecKey);
		string value = PreviewLabs.PlayerPrefs.GetString(key);

		string decoded = DecodeString (value);

		Debug.Log ("Decoded: " + decoded);

		string compValue = EncryptString(decoded);
		
		if(ecValue == compValue){
			return decoded;
		}
		else{
			Debug.LogWarning("Encrypted value and comparison value not equal. Deleting value.");
			PreviewLabs.PlayerPrefs.DeleteKey(key);
			PreviewLabs.PlayerPrefs.DeleteKey(ecKey);
			return "";
		}
	
	}*/

}


