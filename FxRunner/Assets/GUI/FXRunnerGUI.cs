using UnityEngine;
using System.Collections;

public class FXRunnerGUI : MonoBehaviour {
	public static FXRunnerGUI Instance;

	public GUIText ScoreGUIText;
	public GUIText TimeGUIText;
	public GUIText TextGUIText;

	private FXRunnerManager fxm;
	// Use this for initialization
	void Start () {
		Instance = this;

		//INIT
		ScoreGUIText.text = "";
		TimeGUIText.text = "";

		fxm = FXRunner.fxRunnerManager;
	}
	
	// Update is called once per frame
	int lastTime = 0;
	void Update () {
		if( (int)fxm.gameTime != lastTime){
			lastTime = (int)fxm.gameTime;
			TimeGUIText.text = lastTime.ToString();
		}

		ScoreGUIText.pixelOffset  = new Vector2( Screen.width / 2 , Screen.height - ScoreGUIText.fontSize );
		TimeGUIText.pixelOffset  =  new Vector2( Screen.width - 50 , Screen.height - ScoreGUIText.fontSize );
		TextGUIText.pixelOffset  =  new Vector2( Screen.width - 100 , Screen.height - TextGUIText.fontSize );
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
