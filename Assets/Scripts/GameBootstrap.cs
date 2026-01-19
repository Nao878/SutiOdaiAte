using UnityEngine;

/// <summary>
/// ゲーム初期化を行うスクリプト
/// シーンに空のGameObjectを作成し、このスクリプトをアタッチするだけで動作します
/// </summary>
[DefaultExecutionOrder(-100)]
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        // GameControllerを生成（これがすべてを初期化）
        if (FindFirstObjectByType<GameController>() == null)
        {
            GameObject controllerObj = new GameObject("GameController");
            controllerObj.AddComponent<GameController>();
        }
    }
}
