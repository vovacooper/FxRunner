using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadRLV2 : MonoBehaviour {

	/*******************************************************
	 * PUBLIC
	 ********************************************************/
	///the material for the line's
	public Material lineMaterial;
	
	/// The obsticle prefab.
	public GameObject prefab;
	
	/// The player prefab.
	public GameObject Player;



	private float horizontalPlayerSpeed = .7f;
	private float playerRelativePosition = 0;

	//The speed of the camera. 
	// this speed is a dynamic value


	
	/*******************************************************
	 * GAMEPLAY
	 ********************************************************/

	
	///The distance of the object from the camera. measured by the given function
	///if the camera is in f(t) then the object will be at f(t + distanceFromCamera)
	public static float distanceFromCamera = 1;
	///The speed of the camera and the player
	public static float speed = 1f;

	
	private static float _gameScore = 0;
	public static float gameScore{
		get{
			return _gameScore;
		}set{
			_gameScore = value;
			FXRunnerGUI.Instance.Score = _gameScore;
		}
	}


	/// <summary>
	/// The max speed in current game.
	/// </summary>
	public float maxSpeed = 0;
	/// <summary>
	/// The max score in current game.
	/// </summary>
	public float maxScore = 0;



	/*******************************************************
	 * PRIVATE
	 ********************************************************/
	//The size of the line
	private static float lineSizeLarge = 0.3f;
	//The color of the line
	private static Color lineColorLarge = new Color(0,1,1,1f);



	//List of LineFrame which contain all the lines in a Frame
	private static List<LineFrame> FrameLineList;
	//List of point's in a frame
	private static List<PointFrame> FramePointList;
	//List of objects on the road




	//this values will move the camera and the road according to the RoadFunction Class
	private static float posOfCamera;
	private static float posOfRoad;

	private static RoadFunction1 ptFunction;

	//The distance of the road beeng rendered from the camera. 
	private static float roadDistanceFromCamera;
	//The nunmber of points in one frame
	private static int numberOfPointsInFrame;
	//How manu lined remember in the history
	private static int linesToRemember;

	/// <summary>
	/// the size of the road will be twise this size
	/// </summary>
	private static float sideSize = 30;

	/// <summary>
	/// The distance berween Frames.
	/// </summary>
	private static float step;


	/*******************************************************
	 * MonoBehaviour
	 ********************************************************/
	void Start () {
		ptFunction = new RoadFunction1();
		
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.white;
		
		
		posOfCamera = 0f;
		posOfRoad = 0f;
		numberOfPointsInFrame = 7;
		linesToRemember = 80;
		roadDistanceFromCamera = 20;
		step = .25f;

		sideSize = 30;

		FramePointList = new List<PointFrame>();
		FrameLineList = new List<LineFrame>();
		
		Vector3[] v = new Vector3[ numberOfPointsInFrame ];
		PointFrame pf = new PointFrame( v );
		FramePointList.Add( pf );
	
	}


	void Update () {
		////////////////////////
		/// 
		/// SCORE
		/// ////////////////////

		if( speed > maxSpeed ){
			maxSpeed = speed;
		}
		




		/////////////////////////////////////
		speed += Input.GetAxisRaw("Vertical") * Time.deltaTime ;
	

		////////CLEAR
		if(Input.GetKeyDown(KeyCode.C)) {
			this.Clear();
		}
		
		for(   ; posOfRoad < ( (int)posOfCamera + (int)roadDistanceFromCamera ) ; posOfRoad += step){
			addPointFrame( posOfRoad );
		}

		//Camera speed update
		posOfCamera += speed * Time.deltaTime;

		//Move camera to position
		Camera.main.transform.position = ptFunction.Pos( posOfCamera );// + ptFunction.Norm( posOfCamera );
		Camera.main.transform.LookAt( ptFunction.Pos( posOfCamera + .1f) , ptFunction.Norm( posOfCamera )*20 );
		Camera.main.transform.position = ptFunction.Pos( posOfCamera ) + ptFunction.Norm( posOfCamera )*20;
		Camera.main.transform.Rotate(25 , 0 , 0);


		//Move player position
		if(speed >= 1f){
			playerRelativePosition +=  (1f + 5f *   (1 - (1 / ( Mathf.Abs( speed ) ) ) )   ) * horizontalPlayerSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;

		}else{
			playerRelativePosition +=  horizontalPlayerSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
		}

		if( Mathf.Abs( playerRelativePosition ) >= 1){
			playerRelativePosition /= Mathf.Abs( playerRelativePosition );
		}

		Vector3 p = ptFunction.Pos( posOfCamera + distanceFromCamera );
		Player.transform.position = p + ptFunction.Right( posOfCamera + distanceFromCamera ) * sideSize * playerRelativePosition;
		Player.transform.LookAt( ptFunction.Pos( posOfCamera + distanceFromCamera + 0.1f) + ptFunction.Right( posOfCamera + distanceFromCamera ) * sideSize * playerRelativePosition , ptFunction.Norm( posOfCamera + distanceFromCamera + 0.1f) );


	}


	/*******************************************************
	 * GAMEPLAY
	 ********************************************************/
	/// <summary>
	/// Jump the specified strength.
	/// </summary>
	/// <param name="strength">Strength is in [-1,1].</param>
	private bool canjump = false;
	private void jump( float strength ){
		if( !canjump ){
			return;
		}
	
	}

	/// <summary>
	/// Speeds the player.
	/// </summary>
	/// <param name="boosStrength">Boos strength by boosStrength factor.</param>
	private void speedBust( float boosStrength ){

	}

	/// <summary>
	/// slows down the player.
	/// </summary>
	/// <param name="boosStrength">reduce speed by strength factor.</param>
	private void speedBump( float strength ){

	}


	/*******************************************************
	 * GUI
	 ********************************************************/
	GUIStyle labelStyle;
	void OnGUI() {
		int vc = canvasIndex; 
		GUI.Label (new Rect (10, 5, 300, 24), "Drawin " + vc + " lines. 'C' to clear", labelStyle);

		GUI.Label (new Rect (10, 20, 300, 24), "Speed: " + speed , labelStyle);
		GUI.Label (new Rect (10, 35, 300, 24), "Max speed: " + maxSpeed , labelStyle);

		GUI.Label (new Rect (10, 50, 300, 24), "Collision Count: " + FXRunnerPlayer.CollisionCount , labelStyle);

		GUI.Label (new Rect (10, 65, 300, 24), "Score: " + gameScore , labelStyle);

	}



	

	
	/*******************************************************
	 * Logic
	 ********************************************************/
	public void Clear(){
		FrameLineList = new List<LineFrame>();
		FramePointList = new List<PointFrame>();
		
		Vector3[] v = new Vector3[ numberOfPointsInFrame ];
		PointFrame pf = new PointFrame( v );
		FramePointList.Add( pf );
		
		canvasIndex = 0;
	}
	


	//Adding the points and lines
	public void addPointFrame( float t ){

		Vector3[] v = new Vector3[7] ;
		v[3] = ptFunction.Pos(t);
		
		Vector3 dir = ptFunction.Dir( t );
		
		Vector3 sqr = new Vector3(sideSize,0,0);
		Vector3.OrthoNormalize(ref dir ,ref sqr);
		sqr *= sideSize;
		
		Quaternion q = Quaternion.AngleAxis( ptFunction.Ang( t ) ,dir );
		sqr = q * sqr;
		
		
		v[0] = v[3] - sqr;
		v[1] = v[3] - sqr*2/3;
		v[2] = v[3] - sqr/3;
		v[4] = v[3] + sqr*2/3;
		v[5] = v[3] + sqr/3;
		v[6] = v[3] + sqr;

		PointFrame pf = new PointFrame( v );
		pf.createPrefabs( "Good", this.transform, prefab , t );
		this.addPointFrame( pf , lineSizeLarge , lineColorLarge);
	}
	//Adding the lines
	private void addPointFrame(PointFrame pf ,float lineSize, Color c){
		FramePointList.Add(pf);
		createFrameLines( FramePointList[FramePointList.Count - 2] , FramePointList[FramePointList.Count -1], lineSize , c );
		createPrefabsOnFrame( pf );


		if(FrameLineList.Count > linesToRemember){
			FrameLineList[0].Delete();
			FrameLineList.RemoveAt(0);


			FramePointList[0].Delete();
			FramePointList.RemoveAt(0);
		}
	}
	
	//////////////////////////////
	/// 	HELP
	//////////////////////////////
	public class PointFrame{
		public Vector3[] Points;
		public GameObject[] Prefabs;

		public PointFrame( params Vector3[] points ){
			Points = new Vector3[points.Length];
			for(int i = 0 ; i < points.Length ; i++){
				Points[i] = points[i];
			}
		}
		/// <summary>
		/// C////////////////////////////////////////////////////////////////////////////////////////////////////////		/// </summary>
		public void createPrefabs( string name, Transform parent , GameObject prefab ,float t ){
			float pos = ptFunction.posibilityForObject();
			if( Random.value >= pos ){
				return;
			}

			int numOfPrefabs = (int)((numberOfPointsInFrame - 1) * ptFunction.posibilityForObjectInFrame());

			Prefabs = new GameObject[ numOfPrefabs ];

			float height = step / 2 ;
			float width = sideSize * 2/(numberOfPointsInFrame - 1);


			Vector3 n = ptFunction.Norm(t);	
			Vector3 p = ptFunction.Pos(t);	
			Vector3 dir = ptFunction.Dir( t );
			
			Vector3 sqr = new Vector3(sideSize,0,0);
			Vector3.OrthoNormalize(ref dir ,ref sqr);
			sqr *= width;
			
			Quaternion q = Quaternion.AngleAxis( ptFunction.Ang( t ) ,dir );
			sqr = q * sqr;

			Vector3 hDir =  ptFunction.Pos( t + height  ) - ptFunction.Pos( t ) ;

			//hDir - height vector
			//sqr - width vector

			Vector3[] randPosition = new Vector3[numberOfPointsInFrame - 1];

			p = p - sqr/2 - sqr * 2 + hDir + n/8 ;
			//Will work only for even
			for(int i = 0 ; i < (randPosition.Length) ; i ++){
				randPosition[i] = p + sqr * i ;
			}
			
			for (int i = 0; i < (randPosition.Length) ; i++) {
				Vector3 temp = randPosition[i];
				int randomIndex = Random.Range(i, randPosition.Length);
				randPosition[i] = randPosition[randomIndex];
				randPosition[randomIndex] = temp;
			}

			for( int i = 0 ; i < numOfPrefabs ; i++){
				Prefabs[i] = (GameObject)GameObject.Instantiate( prefab );
				Prefabs[i].transform.localPosition = randPosition[i];
				Prefabs[i].transform.LookAt(randPosition[i] + hDir , ptFunction.Norm( t ) );
				Prefabs[i].transform.parent = parent;
				Prefabs[i].name = name;
			}
		}

		public void Delete()  // destructor
		{
			if( Prefabs != null ){
				foreach( GameObject g in Prefabs){
					GameObject.Destroy(g);
				}
			}
		}
	}
	
	public class LineFrame{
		public List<GameObject> Lines;
		public LineFrame(){
			Lines = new List<GameObject>();
		}
		
		public void Delete()  // destructor
		{
			foreach( GameObject g in Lines ){
				GameObject.Destroy(g);
			}
		}
	}
	private void createPrefabsOnFrame( PointFrame pf ){

	}

	private void createFrameLines( PointFrame from , PointFrame to , float lineSize, Color c){
		LineFrame lineFrame = new LineFrame();
		for(int i = 0 ; i < from.Points.Length ; i++){
			lineFrame.Lines.Add( createLine( lineSize , c ,from.Points[i] , to.Points[i]) );
		}
		lineFrame.Lines.Add( createLine( lineSize , c , to.Points[0], to.Points[ numberOfPointsInFrame - 1 ] ) );

		FrameLineList.Add( lineFrame );
	}
	
	//Counts the Lines objects
	private int canvasIndex = 0;
	private GameObject createLine(float lineSize, Color c  ,params Vector3[] points) {
		GameObject canvas = new GameObject("canvas_" + canvasIndex); 
		canvas.transform.parent = transform;
		canvas.transform.rotation = transform.rotation;
		
		LineRenderer lines = (LineRenderer) canvas.AddComponent<LineRenderer>();
		lines.material = lineMaterial;
	//	lines.material.color = c;
		lines.useWorldSpace = false;
		lines.SetWidth(lineSize, lineSize);
		lines.SetVertexCount( points.Length );
		for(int i = 0 ; i < points.Length ; i++){
			lines.SetPosition(i, points[i]);
		}
		
		canvasIndex++;
		return canvas;
	}
	
}
