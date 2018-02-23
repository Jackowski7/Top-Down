using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter : MonoBehaviour
{

    public GameObject _player;

    // Use this for initialization
    private void Start ()
    {
        if (GameManager.hasPlayer == false)
        {
            Vector3 pos = new Vector3(transform.position.x, .5f, transform.position.z) ;
            _player = Instantiate(_player, pos, transform.rotation);
            _player.name = "Player";
            Player.lastEnterPoint = pos;
            GameManager.hasPlayer = true;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 pos = new Vector3(transform.position.x, .5f, transform.position.z);
            player.transform.position = pos;
            Player.lastEnterPoint = pos;
            Debug.Log("moving player");
        }
    }
}
