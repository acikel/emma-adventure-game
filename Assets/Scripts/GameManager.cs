using UnityEngine;
public enum GameState { INTRO, MAIN_MENU, PAUSED, GAME, CREDITS, HELP }
public class GameManager : Object
{


	protected GameManager() { }
	private static GameManager instance = null;
	public delegate void OnStateChangeHandler();
	public delegate void OnControllerChangeHandler();
	public GameState gameState { get; private set; }
	public event OnStateChangeHandler OnStateChange;
	public event OnControllerChangeHandler OnControllerChange;
	public static Avatar currentAvatar;
	public static GameObject background;
	public static PolygonCollider2D backgroundCollider;

	public static Avatar playerAvatar;

	public static Avatar helperAvatar;

	public static Transform groundCenter;
	public static GameManager Instance
	{
		get
		{

			if (GameManager.instance == null)
			{
				GameManager.instance = new GameManager();
				playerAvatar = GameObject.FindWithTag("Player").GetComponent<Emma>();
				helperAvatar = GameObject.FindWithTag("Helper").GetComponent<Helper>();
				backgroundCollider = GameObject.FindWithTag("Ground").GetComponent<PolygonCollider2D>();
				background = GameObject.FindWithTag("Ground");

				currentAvatar = playerAvatar;
				groundCenter = GameObject.FindWithTag("Ground").transform.Find("Center");
			}
			return GameManager.instance;

		}

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
		GameManager.instance = null;
	}

}
