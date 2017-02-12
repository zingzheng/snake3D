using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Game : MonoBehaviour {

	public GameObject anim3;

	//下方的菜单
	public Text textScore;
	public Button pauseButton;
	public Button rankButton;

	public Button frButton;

	//中间的开始菜单
	public Image startImage;
	public Button startButton;
	public Text textLose;
	public Button restartButton;

	//右侧的菜单
	public Image menuImage;
	public Text textRank;
	public Button closeButton;
	public Button freshButton;


	public playerControl player;
	public SpawnFood sf;

	//注册菜单
	public Image regImage;
	public InputField nameInput;
	public Button nameButton;

	//音效
	public Text regText;
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
	//延迟时间
	private float delt = 99.0f;




	// Use this for initialization
	void Start () {
		isStart = false;
		Time.timeScale = 0;

		//添加player的成功、失败、加分的方法
		player.OnLose += OnLose;
		player.OnWin += OnWin;
		player.AddScore += AddScore;
		//监听开始、重新开始
		Button str_btn = startButton.GetComponent<Button>();
		str_btn.onClick.AddListener(startGame);
		Button restr_btn = restartButton.GetComponent<Button>();
		restr_btn.onClick.AddListener(restartGame);
		//监听暂停
		Button pau_btn = pauseButton.GetComponent<Button>();
		pau_btn.onClick.AddListener(pauseGame);
		Button fr_btn = frButton.GetComponent<Button>();
		fr_btn.onClick.AddListener(restartGame);

		//监听榜单
		Button rank_btn = rankButton.GetComponent<Button>();
		rank_btn.onClick.AddListener(showRank);
		Button close_btn = closeButton.GetComponent<Button>();
		close_btn.onClick.AddListener(closeRank);
		Button fresh_btn = freshButton.GetComponent<Button>();
		fresh_btn.onClick.AddListener(showRank);

		//监听设置昵称
		Button name_btn = nameButton.GetComponent<Button>();
		name_btn.onClick.AddListener(submitName);


		//若未设置昵称，弹出设置昵称
		infoall = ReadFile (configPath);
		if (infoall == null) {
			regImage.gameObject.SetActive (true);
		} else {
			userName = (string)infoall [0];
		}
		startImage.gameObject.SetActive (true);
		startButton.gameObject.SetActive (true);
	}

	//开始游戏
	void startGame(){
		
		//激活蛇和食物脚本
		sf.gameObject.SetActive (true);
		player.gameObject.SetActive (true);

		//隐藏开始菜单
		startImage.gameObject.SetActive (false);
		startButton.gameObject.SetActive (false);

		//播放背景英语
		bgm.Play();
		Time.timeScale = 1;
		isStart = true;
		pauseButton.gameObject.SetActive (true);

//		delt = 3.0f;
//		GameObject anim = Instantiate (anim3);

	}

	//重新开始游戏
	void restartGame(){
		SceneManager.LoadScene ("snake_scene");
		Time.timeScale = 0;
		isStart = false;
	}

	//暂停游戏
	void pauseGame(){
		if (isStart == true) {
			isStart = false;
			Time.timeScale = 0;
			pauseButton.GetComponentInChildren<Text> ().text = "恢复游戏";

			//暂停背景音乐
			bgm.Stop ();
		} else {
			isStart = true;
			Time.timeScale = 1;
			pauseButton.GetComponentInChildren<Text> ().text = "暂停游戏";
			bgm.Play ();
		}
	}

	//排行榜菜单
	void showRank(){
		if(isStart == true){
			Time.timeScale = 0;
			bgm.Stop ();
		}
		pauseButton.enabled=false;
		//刷新榜单并显示菜单
		StartCoroutine (initRank ());
		menuImage.gameObject.SetActive (true);

	}

	//关闭排行榜菜单
	void closeRank(){
		if(isStart == true){
			Time.timeScale = 1;
			bgm.Play ();
		}
		pauseButton.enabled=true;
		menuImage.gameObject.SetActive (false);

	}


	
	// Update is called once per frame
//	void Update () {
//		if (delt < 5.0f){
//			delt -= Time.deltaTime;
//			if (delt <= 0.0f) {
//				Time.timeScale = 1;
//				isStart = true;
//				pauseButton.gameObject.SetActive (true);
//				delt = 99.0f;
//			}
//		}
//	}

	//失败触发
	void OnLose(){
		isStart = false;
		pauseButton.gameObject.SetActive (false);
		//失败音效
		bgm.Stop ();
		failm.Play ();
		//提交分数
		StartCoroutine(submitScore ());
		//展示中间菜单
		startImage.gameObject.SetActive (true);
		textLose.text = "You lose!";
		textLose.gameObject.SetActive(true);
		restartButton.gameObject.SetActive (true);

		Time.timeScale = 0;
	}

	//成功触发
	void OnWin(){
		isStart = false;
		pauseButton.gameObject.SetActive (false);
		//成功音效
		bgm.Stop ();
		//提交分数
		StartCoroutine(submitScore ());
		//展示中间菜单
		startImage.gameObject.SetActive (true);
		textLose.text = "You win!";
		textLose.gameObject.SetActive(true);
		restartButton.gameObject.SetActive (true);

		Time.timeScale = 0;


	}

	//加分
	void AddScore(){
		//加分音效
		scorem.Play ();
		//更新分数
		score += 1;
		textScore.text = "score: " + score.ToString();
		if (score == (10+10-1) * (15+18-1)) {
			OnWin ();
		}

	}

	//提交昵称
	void submitName(){
		string nameStr = nameInput.GetComponentInChildren<Text> ().text;
		if (nameStr == "") {
			regText.text = "请输出昵称！";
		} else {
			StartCoroutine(checkName (nameStr));
		}


	}

	//注册姓名
	IEnumerator checkName(string name){
		WWW www = new WWW ("http://127.0.0.1:8000/check_name/"+name);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
			if (www.text.ToString ().Contains ("ok")) {
				WriteFile (configPath, name);
				userName = name;
				regImage.gameObject.SetActive (false);
			} else {
				regText.text = "该昵称已被占用！";
			}

		} else {
			Debug.Log (www.error);
			regText.text = "服务器异常！";
		}
	}

	//获取排行榜
	IEnumerator initRank () {
		WWW www = new WWW ("http://127.0.0.1:8000/list_top/");
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log (www.text);
			string res = www.text.ToString();
			int bIndex = 23;
			int eIndex = res.LastIndexOf("\"");
			res = res.Substring (bIndex, eIndex - bIndex);
			res = res.Replace ("\\n", "\n");
			textRank.text = res;
		} else {
			Debug.Log (www.error);
			textRank.text = "load ranklist fail! \nplease try later!";
			//myText.text = myText.text.Replace ("\\n", "\n");
		}
	}

	//提交分数到服务器
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
		

	//写文件
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
		
	//读文件
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
