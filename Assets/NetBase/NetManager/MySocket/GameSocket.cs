using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net;
public class GameSocket : SocketManager
{
    private static GameSocket _instance;
    public static GameSocket Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameSocket();
                GameSocket._instance.m_name = "Game";

            }
            return _instance;
        }
    }
}
