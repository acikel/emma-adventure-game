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

	public static Avatar playerAvatar;

	public static Avatar helperAvatar;

	public static Transform groundCenter;
	public static AvatarManager Instance
	{
		get
		{

			if (playerAvatar==null)
			{
				
				AvatarManager.instance = new AvatarManager();
				//Debug.Log("hi3"+instance);
				playerAvatar = GameObject.FindWithTag("Player").GetComponent<Emma>();
				helperAvatar = GameObject.FindWithTag("Helper").GetComponent<Helper>();
				helperAvatar.gameObject.SetActive(false);
				backgroundCollider = GameObject.FindWithTag("Ground").GetComponent<PolygonCollider2D>();
				background = GameObject.FindWithTag("Ground");

				currentAvatar = playerAvatar;
				groundCenter = GameObject.FindWithTag("Ground").transform.Find("Center");
			}
			return AvatarManager.instance;

		}

	}

	public void ReloadGround()
    {
		//Debug.Log("hi");
		backgroundCollider = GameObject.FindWithTag("Ground").GetComponent<PolygonCollider2D>();
		background = GameObject.FindWithTag("Ground");
		groundCenter = GameObject.FindWithTag("Ground").transform.Find("Center");
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
