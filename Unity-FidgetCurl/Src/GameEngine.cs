using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameEngine : GameEngineBehaviour
{
    public static int RecursionMaxLevel = 0;

    public static new bool Pause
    {
        get
        {
            return GameEngine._instance.Pause;
        }
        set
        {
            GameEngine._instance.Pause = value;
        }
    }

    public static GameEngineWorld GameWorld
    {
        get
        {
            GameEngine.Debug.TraceLog.Update("GameEngineWorld.GameWorld");
            if (GameEngine._instance == null)
            {
                return null;
            }
            return GameEngine._instance.World;
        }
    }

    public static new EngineDebug Debug
    {
        get
        {
            return GameEngine._instance.Debug;
        }
    }

    public static CurlSpinnerBehaviour Spinner
    {
        get
        {
            try
            {
                return GameObject.FindObjectOfType<CurlSpinnerBehaviour>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public static BumperManagerBehaviour Bumpers
    {
        get
        {
            try
            {
                return GameObject.FindObjectOfType<BumperManagerBehaviour>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public static CameraBehaviour GameCamera
    {
        get
        {
            return GameObject.FindObjectOfType<CameraBehaviour>();
        }
    }

    public static BackgroundBehaviour Background
    {
        get
        {
            return GameObject.FindObjectOfType<BackgroundBehaviour>();
        }
    }

    public static SmartVector ScreenRatioToWorldPosition(float x, float y)
    {
        if (GameEngine._instance == null)
        {
            return SmartVector.Zero;
        }
        return GameEngine.GameCamera.ScreenRatioToWorldPosition(x, y);
    }

    public static bool Place(Transform Target, SmartVector Start, SmartVector End, float width, float height)
    {
        GameEngine.Debug.TraceLog.Update("GameEngineWorld.Place");
        return GameEngine.GameWorld.Place(Target, Start, End, width, height);
    }
}