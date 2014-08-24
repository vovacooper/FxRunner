using UnityEngine;
using System.Collections;

public class FXRunnerGUI : MonoBehaviour {
	public static FXRunnerGUI Instance;

	public UnityEngine.UI.Text ScoreGUIText;
	public UnityEngine.UI.Text TimeGUIText;
	public UnityEngine.UI.Text TextGUIText;

	private FXRunnerManager fxm;
	// Use this for initialization
	void Start () {
		Instance = this;

		//INIT
		ScoreGUIText.text = "";
		TimeGUIText.text = "";


	}
	
	// Update is called once per frame
	int lastTime = 0;
	void Update () {
		if(fxm == null){
			fxm = FXRunner.fxRunnerManager;
			return;
		}
		if( (int)fxm.gameTime != lastTime){
			lastTime = (int)fxm.gameTime;
			TimeGUIText.text = lastTime.ToString();
		}
	}

	private float _score = 0;
	public float Score{
		get{
			return _score;
		}set{
			_score = value;
			Instance.ScoreGUIText.text = _score.ToString();
		}
	}
}
