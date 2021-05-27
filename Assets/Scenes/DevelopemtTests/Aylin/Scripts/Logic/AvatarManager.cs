using System.Collections.Generic;
using UnityEngine;
public enum GameState { INTRO, MAIN_MENU, PAUSED, GAME, CREDITS, HELP }
public class AvatarManager : Object
{


	protected AvatarManager() { instance = this; }
	private static AvatarManager instance = null;
	public delegate void OnStateChangeHandler();
	public delegate void OnControllerChangeHandler();
	public GameState gameState { get; private set; }
	public event OnStateChangeHandler OnStateChange;
	public event OnControllerChangeHandler OnControllerChange;
	public static Avatar currentAvatar;
	public static GameObject background;
	public static PolygonCollider2D backgroundCollider;
	public static List<Collider2D> obstacles = new List<Collider2D>();
	public static Animator helperAnimator;
	public static Animator playerAnimator;

	public static Avatar playerAvatar;

	public static Avatar helperAvatar;

	public static Transform groundCenter;

	private static GameObject[] obstaclesTmp;
	private SceneManager sceneManager = API.SceneManager;

	//private static GameObject player;
	//private static GameObject helper;
	public static AvatarManager Instance
	{
		get
		{

			if (playerAvatar==null)
			{
				
				AvatarManager.instance = new AvatarManager();
				//Debug.Log("hi3"+instance);
				//player = GameObject.FindWithTag("Player");
				//helper = GameObject.FindWithTag("Helper");
				playerAvatar = GameObject.FindWithTag("Player").GetComponent<Emma>();
				helperAvatar = GameObject.FindWithTag("Helper").GetComponent<Helper>();
				playerAnimator = playerAvatar.gameObject.GetComponent<Animator>();
				helperAnimator = helperAvatar.gameObject.GetComponent<Animator>();
				helperAvatar.gameObject.SetActive(false);
				background = GameObject.FindWithTag("Ground");
				backgroundCollider = background?.GetComponent<PolygonCollider2D>();
				
				obstaclesTmp = GameObject.FindGameObjectsWithTag("Obstacle");
				foreach (GameObject gameObject in obstaclesTmp)
				{
					obstacles.AddRange(gameObject.transform.GetComponentsInChildren<Collider2D>());
				}

				currentAvatar = playerAvatar;
				//groundCenter = GameObject.FindWithTag("Ground")?.transform.Find("Center");
				groundCenter = background?.transform.GetChild(0)?.gameObject.transform;
			}
			return AvatarManager.instance;

		}

	}

	//Used by Scene Manager after new scene load.
	public void ReloadGround()
    {
		//Debug.Log("hi");
		backgroundCollider = GameObject.FindWithTag("Ground").GetComponent<PolygonCollider2D>();
		background = GameObject.FindWithTag("Ground");
		groundCenter = GameObject.FindWithTag("Ground").transform.Find("Center");
	}
	//Used Scene Manager after new scene load.
	public void ReloadObstacles()
	{
		//Debug.Log("ReloadObstacles");
		obstacles.Clear();
		obstaclesTmp = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach(GameObject gameObject in obstaclesTmp)
        {
			obstacles.AddRange(gameObject.transform.GetComponentsInChildren<Collider2D>());
        }
	}

	public bool checkForCollisionWithObstacles(Collider2D colliderToCheck)
    {
		if (sceneManager.IsReloading)
			return false;

        foreach (Collider2D obstacleCollider in obstacles)
        {
			if (colliderToCheck.IsTouching(obstacleCollider))
				return true;
		}
		return false;
		
    }
	public void ChangeGroundCenter(Transform newGroundCenter)
	{
		groundCenter = newGroundCenter;
	}

	public void ChangeController(Avatar newAvatar, Avatar previousAvatar)
	{
		currentAvatar = newAvatar;
		currentAvatar.avatarSpriteRenderer.sortingOrder++;
		previousAvatar.avatarSpriteRenderer.sortingOrder--;
		previousAvatar.getCapsuleTrigger().isTrigger = false;
		currentAvatar.getCapsuleTrigger().isTrigger = true;
		currentAvatar.getRigidbody2D().mass = 1;
		previousAvatar.getRigidbody2D().mass = 10000;

		OnControllerChange();
	}

	public void SetGameState(GameState state)
	{
		this.gameState = state;
		OnStateChange();
	}

	public void OnApplicationQuit()
	{
		AvatarManager.instance = null;
	}

}