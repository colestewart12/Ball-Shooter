using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public string username;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadUsername();
    }

    [System.Serializable]
    class SaveData
    {
        public string username;
    }

    public void SaveUsername()
    {
        SaveData data = new SaveData();
        username = GameObject.Find("Name Input").GetComponent<TMPro.TMP_InputField>().text;
        data.username = username;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savenamefile.json", json);

        SceneManager.LoadScene(1);
    }
    public void LoadUsername()
    {
        string path = Application.persistentDataPath + "/savenamefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            username = data.username;
        }
    }

    public void Exit()
    {
        MenuManager.Instance.SaveUsername();
        if (Application.isEditor)
        {
            EditorApplication.ExitPlaymode();
        }
        else
            Application.Quit();
    }
}
