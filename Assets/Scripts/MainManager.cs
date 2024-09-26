using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public Text HighScoreText;

    public TMP_InputField NameInputField;
    public GameObject NameInputPanel;

    private bool m_Started = false;
    private int m_Points;
    public int highScores = 0;
    public string highScoreName = "Player";
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        LoadScore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (m_Points > highScores)
        {
            NameInputPanel.SetActive(true);
        }
        SaveScore();
    }

    [System.Serializable]
    class SaveHighScore 
    {
        public int highScore;
        public string name;
    }

    public void SaveScore()
    {
        SaveHighScore highScore = new SaveHighScore();
        if (m_Points > highScores)
        {
            highScore.name = "Player";
            highScore.highScore = m_Points;
            HighScoreText.text = $"Best Score : {highScoreName} : {m_Points}";
            string json = JsonUtility.ToJson(highScore);

            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveHighScore highScore = JsonUtility.FromJson<SaveHighScore>(json);
            HighScoreText.gameObject.SetActive(true);
            HighScoreText.text = $"Best Score : {highScore.name} : {highScore.highScore}";
            highScores = highScore.highScore;
            highScoreName = highScore.name;
        } else
        {
            HighScoreText.text = "Best Score : 0";
        }
    }

    public void SubmitName()
    {
        highScoreName = NameInputField.text;
        NameInputPanel.SetActive(false);
        SaveScore();
    }
}
