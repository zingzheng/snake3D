using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Game : MonoBehaviour {

	//中间的开始菜单
	public Image startImage;
	public Button startButton;
	public Text textLose;
	public Button restartButton;

	//右侧的菜单
	public Text textRank;
	public Text textScore;
	public Button pauseButton;


	public playerControl player;
	public SpawnFood sf;

	//注册菜单
	public Image regImage;
	public InputField nameInput;
	public Button nameButton;

	//音效
	public AudioSource bgm;
	public AudioSource scorem;
	public AudioSource failm;

	private bool isStart = false;
	private Text pause_text;
	private int score;
	private string userName;
	//配置文件
	private string configPath = "config.txt" ;
	//文本中每行的内容
	private ArrayList infoall = null;




	// Use this for initialization
	void Start () {
		StartCoroutine(initRank ());
		//添加player的成功、失败、加分的方法
		player.OnLose += OnLose;
		player.OnWin += OnWin;
		player.AddScore += AddScore;
		//监听开始按钮、暂停/恢复按钮、重新开始按钮的按下事件
		Button str_btn = startButton.GetComponent<Button>();
		str_btn.onClick.AddListener(startGame);
		Button pau_btn = pauseButton.GetComponent<Button>();
		pau_btn.onClick.AddListener(pauseGame);
		pause_text = pauseButton.GetComponentInChildren<Text> ();
		Button restr_btn = restartButton.GetComponent<Button>();
		restr_btn.onClick.AddListener(restartGame);
		Button name_btn = nameButton.GetComponent<Button>();
		name_btn.onClick.AddListener(submitName);


		infoall = ReadFile (configPath);
		if (infoall == null) {
			regImage.gameObject.SetActive (true);
		} else {
			userName = (string)infoall [0];
		}
		startImage.gameObject.SetActive (true);
		startButton.gameObject.SetActive (true);
	}

	void startGame(){
		Time.timeScale = 1;
		sf.gameObject.SetActive (true);
		player.gameObject.SetActive (true);
		pauseButton.gameObject.SetActive (true);

		startImage.gameObject.SetActive (false);
		startButton.gameObject.SetActive (false);

		isStart = true;
		bgm.Play();
		
	}

	void pauseGame(){
		if (isStart == true) {
			isStart = false;
			Time.timeScale = 0;
			pause_text.text = "恢复游戏";
			bgm.Stop ();
		} else {
			isStart = true;
			Time.timeScale = 1;
			pause_text.text = "暂停游戏";
			bgm.Play();
		}

	}

	void restartGame(){
		SceneManager.LoadScene ("snake_scene");
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLose(){
		failm.Play ();
		StartCoroutine(submitScore ());
		startImage.gameObject.SetActive (true);
		textLose.text = "You lose!";
		textLose.gameObject.SetActive(true);
		restartButton.gameObject.SetActive (true);

		pauseButton.gameObject.SetActive (false);

		Time.timeScale = 0;

		bgm.Stop ();
	}

	void OnWin(){
		StartCoroutine(submitScore ());
		startImage.gameObject.SetActive (true);
		textLose.text = "You win!";
		textLose.gameObject.SetActive(true);
		restartButton.gameObject.SetActive (true);

		pauseButton.gameObject.SetActive (false);

		Time.timeScale = 0;

		bgm.Stop ();
	}

	void AddScore(){
		score += 1;
		scorem.Play ();
		textScore.text = "score: " + score.ToString();
		if (score == 17 * 17) {
			OnWin ();
		}

	}

	void submitName(){
		Text nameText = nameInput.GetComponentInChildren<Text> ();
		WriteFile (configPath, nameText.text);
		userName = nameText.text;
		regImage.gameObject.SetActive(false);

	}

	IEnumerator initRank () {
		WWW www = new WWW ("http://127.0.0.1:8000/list_top/");
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
			textRank.text = www.text.ToString();
		} else {
			Debug.Log (www.error);
		}
	}

	IEnumerator submitScore(){
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField ("user", userName);
		wwwForm.AddField("score",score);
		WWW www = new WWW ("http://127.0.0.1:8000/upload_score/",wwwForm);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
		} else {
			Debug.Log (www.error);
		}

	}

	void WriteFile(string name,string info)
	{
		//文件流信息
		StreamWriter sw;
		FileInfo t = new FileInfo(Application.persistentDataPath+"//"+ name);
		if(!t.Exists)
		{
			//如果此文件不存在则创建
			sw = t.CreateText();
		}
		else
		{
			//如果此文件存在则打开
			sw = t.AppendText();
		}
		//以行的形式写入信息
		sw.WriteLine(info);
		//关闭流
		sw.Close();
		//销毁流
		sw.Dispose();
	}


	ArrayList ReadFile(string name)
	{
		//使用流的形式读取
		StreamReader sr =null;
		try{
			sr = File.OpenText(Application.persistentDataPath+"//"+ name);
		}catch(Exception e)
		{
			//路径与名称未找到文件则直接返回空
			Debug.Log("no config");
			return null;
		}
		string line;
		ArrayList arrlist = new ArrayList();
		while ((line = sr.ReadLine()) != null)
		{
			//一行一行的读取
			//将每一行的内容存入数组链表容器中
			arrlist.Add(line);
		}
		//关闭流
		sr.Close();
		//销毁流
		sr.Dispose();
		//将数组链表容器返回
		return arrlist;
	}  



}
