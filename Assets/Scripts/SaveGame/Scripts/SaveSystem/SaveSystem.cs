using UnityEngine;
using System.Collections;
using System.IO;

// Modifiche apportate: creato currentData, le get vengono fatte dai dati del file, le set su un insieme di dati temporaneo che poi 
// viene reso permanente quando viene chiamata la funzione che salva tutto su disco.

public class SaveSystem : MonoBehaviour {
	
	private string file;
	private bool loaded;
	private DataState fileData;
    private DataState currentData;

    public static SaveSystem Instance;
    
    void Awake()
    {
        Instance = this;
    }


    public void Clear()
    {
        fileData = new DataState();
        SerializatorBinary.SaveBinary(fileData, GetPath());
    }
    
	public void Initialize(string fileName)
    {
		if(!loaded)
		{
			file = fileName;

            if (File.Exists(GetPath()))
            {
                Load();
            }
            else
            {
                fileData = new DataState();
                currentData = new DataState();
            }

			loaded = true;
		}
	}

	public string GetPath()
	{
		return Application.persistentDataPath + "/" + file;
	}

	public void Load()
	{
		fileData = SerializatorBinary.LoadBinary(GetPath());
        currentData = new DataState(fileData);
	}

    public void ReplaceItem(string name, string item)
	{
		bool j = false;
		for(int i = 0; i < currentData.items.Count; i++)
		{
			if(string.Compare(name, currentData.items[i].Key) == 0)
			{
                currentData.items[i].Value = Crypt(item);
				j = true;
				break;
			}
		}

		if(!j) currentData.AddItem(new SaveData(name, Crypt(item)));
	}

	public bool HasKey(string name) // check for a key
    {
		if(string.IsNullOrEmpty(name)) return false;

		foreach(SaveData k in fileData.items)
		{
			if(string.Compare(name, k.Key) == 0)
			{
				return true;
			}
		}

		return false;
	}

	public void SetVector3(string name, Vector3 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y + "|" + val.z);
	}

	public void SetVector2(string name, Vector2 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y);
	}

	public void SetColor(string name, Color val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.r + "|" + val.g + "|" + val.b + "|" + val.a);
	}

	public void SetBool(string name, bool val) // set the key and value
    {
		if(string.IsNullOrEmpty(name)) return;
		string tmp = string.Empty;
		if(val) tmp = "1"; else tmp = "0";
		ReplaceItem(name, tmp);
	}

	public void SetFloat(string name, float val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public void SetInt(string name, int val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public void SetString(string name, string val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val);
	}

	public void SaveToDisk() // write data to file
    {
		if(currentData.items.Count == 0) return;
		SerializatorBinary.SaveBinary(currentData, GetPath());
        fileData = currentData;
		Debug.Log("[SaveGame] --> Save game data: " + GetPath());
	}

	public Vector3 GetVector3(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector3.zero;
		return iVector3(name, Vector3.zero);
	}

	public Vector3 GetVector3(string name, Vector3 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector3(name, defaultValue);
	}

    public Vector3 iVector3(string name, Vector3 defaultValue)
	{
		Vector3 vector = Vector3.zero;

		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				string[] t = Crypt(fileData.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 3)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					vector.z = floatParse(t[2]);
					return vector;
				}
				break;
			}
		}   

		return defaultValue;
	}

	public Vector2 GetVector2(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector2.zero;
		return iVector2(name, Vector2.zero);
	}

	public Vector2 GetVector2(string name, Vector2 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector2(name, defaultValue);
	}

    public Vector2 iVector2(string name, Vector2 defaultValue)
	{
		Vector2 vector = Vector2.zero;

		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				string[] t = Crypt(fileData.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 2)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					return vector;
				}
				break;
			}
		}

		return defaultValue;
	}

	public Color GetColor(string name)
	{
		if(string.IsNullOrEmpty(name)) return Color.white;
		return iColor(name, Color.white);
	}

	public Color GetColor(string name, Color defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iColor(name, defaultValue);
	}

    public Color iColor(string name, Color defaultValue)
	{
		Color color = Color.clear;

		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				string[] t = Crypt(fileData.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 4)
				{
					color.r = floatParse(t[0]);
					color.g = floatParse(t[1]);
					color.b = floatParse(t[2]);
					color.a = floatParse(t[3]);
					return color;
				}
				break;
			}
		}

		return defaultValue;
	}

	public bool GetBool(string name) // get value by key
    {
		if(string.IsNullOrEmpty(name)) return false;
		return iBool(name, false);
	}

	public bool GetBool(string name, bool defaultValue) // with the default setting
    {
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iBool(name, defaultValue);
	}

    public bool iBool(string name, bool defaultValue)
	{
		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				if(string.Compare(Crypt(fileData.items[i].Value), "1") == 0) return true; else return false;
			}
		}

		return defaultValue;
	}

	public float GetFloat(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iFloat(name, 0);
	}

	public float GetFloat(string name, float defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iFloat(name, defaultValue);
	}

    public float iFloat(string name, float defaultValue)
	{
		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				return floatParse(Crypt(fileData.items[i].Value));
			}
		}

		return defaultValue;
	}

	public int GetInt(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iInt(name, 0);
	}

	public int GetInt(string name, int defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iInt(name, defaultValue);
	}

    public int iInt(string name, int defaultValue)
	{
        if (fileData != null)
        {
            for (int i = 0; i < fileData.items.Count; i++)
            {
                if (string.Compare(name, fileData.items[i].Key) == 0)
                {
                    return intParse(Crypt(fileData.items[i].Value));
                }
            }
        }

		return defaultValue;
	}

	public string GetString(string name)
	{
		if(string.IsNullOrEmpty(name)) return string.Empty;
		return iString(name, string.Empty);
	}

	public string GetString(string name, string defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iString(name, defaultValue);
	}

    public string iString(string name, string defaultValue)
	{
		for(int i = 0; i < fileData.items.Count; i++)
		{
			if(string.Compare(name, fileData.items[i].Key) == 0)
			{
				return Crypt(fileData.items[i].Value);
			}
		}

		return defaultValue;
	}

    public int intParse(string val)
	{
		int value;
		if(int.TryParse(val, out value)) return value;
		return 0;
	}

    public float floatParse(string val)
	{
		float value;
		if(float.TryParse(val, out value)) return value;
		return 0;
	}

    public string Crypt(string text)
	{
		string result = string.Empty;
		foreach(char j in text) result += (char)((int)j ^ 42);
		return result;
	}
}
