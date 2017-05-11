using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour {

    private enum GameStatus {
        StartMenu,
        Demo,
        CharacterSelect,
        Loading,
        Playing,
        Paused,
        Cutscene
    }

    public List<GameObject> player;
    public GameObject enemy;

    private GameStatus _gameStatus;

    private List<GameObject> _players = new List<GameObject>();
    private List<GameObject> _enemies = new List<GameObject>();

    private Texture _textureOverlay;

    // Use this for initialization
    void Awake() {
        _gameStatus = GameStatus.Playing;
        _players.Add(Instantiate(player[0]));
	}

    private bool CanAddEnemy()
    {
        return _enemies.Count < 3;
    }


    void Update(){
        if (Input.GetAxis(("LeftTrigger")) != 0 && CanAddEnemy())
            _enemies.Add(AddEnemy());
        if (_players.Count > 0)
            UpdateCamera(_players[0]);
        //DEBUG ONLY
        if (Input.GetButtonDown("Jump"))
            ResetValues();
    }

    private void ResetValues()
    {
        foreach (GameObject item in _players)
            Destroy(item);
        foreach (GameObject item in _enemies)
            Destroy(item);

        _players = new List<GameObject>();
        _enemies = new List<GameObject>();
    }

    //TEMP SOLUTION IS TO FOLLOW THE FIRST PLAYER; GameController should position itself within a "scene in the level" or follow all players in a centered manner
    private void UpdateCamera(GameObject _player)
    {
        var x = GameObject.FindGameObjectWithTag("MainCamera");
        x.GetComponent<UnityStandardAssets.Utility.FollowTarget>().target = _player.transform;
    }

    private GameObject AddEnemy()
    {
        return Instantiate(enemy);
    }

    private GameObject AddPlayer(int index)
    {
        return Instantiate(player[index]);
    }


    private bool IsGameInCutscene()
    {
        return _gameStatus == GameStatus.Cutscene;
    }

    private bool IsGamePaused()
    {
        return _gameStatus == GameStatus.Paused;
    }

    private bool GameStatusCanAddPlayer()
    {
        return _gameStatus == GameStatus.Playing;
    }

    private bool CanAddPlayerToGame()
    {
        return _players.Count < 2
            && GameStatusCanAddPlayer();
    }
	
}
