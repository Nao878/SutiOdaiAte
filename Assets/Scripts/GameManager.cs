using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ゲームの状態を表すenum
/// </summary>
public enum GameState
{
    PlayerCountSelect,  // プレイヤー人数選択
    AnswererCover,      // 回答者に見せない画面
    TopicDisplay,       // お題表示（ヒント提供者向け）
    PlayerChange,       // プレイヤー交代画面
    HintInput,          // ヒント入力
    AnswerInput,        // 回答者の回答入力
    Result              // 結果表示
}

/// <summary>
/// ゲーム全体のデータと進行を管理するシングルトンクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager Instance { get; private set; }

    [Header("ゲーム設定")]
    public int playerCount = 4;
    public int answererIndex = 0;
    public int currentRound = 1;

    [Header("ゲームデータ")]
    public string currentTopic = "";
    public List<string> playerNames = new List<string>();
    public List<string> hintValues = new List<string>();
    
    // 現在のヒント提供者インデックス（回答者を除いたインデックス）
    private int currentHintGiverIndex = 0;

    // 現在のゲーム状態
    public GameState CurrentState { get; set; } = GameState.PlayerCountSelect;

    void Awake()
    {
        // シングルトン設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// プレイヤー人数を設定（2〜5人）
    /// </summary>
    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 2, 5);
    }

    /// <summary>
    /// プレイヤー名を初期化
    /// </summary>
    public void InitializePlayerNames()
    {
        playerNames.Clear();
        for (int i = 1; i <= playerCount; i++)
        {
            playerNames.Add($"プレイヤー{i}");
        }
    }

    /// <summary>
    /// 新しいラウンドをセットアップ
    /// </summary>
    public void SetupNewRound()
    {
        hintValues.Clear();
        currentHintGiverIndex = 0;
        currentTopic = TopicData.GetRandomTopic();
    }

    /// <summary>
    /// 回答者の名前を取得
    /// </summary>
    public string GetAnswererName()
    {
        if (answererIndex >= 0 && answererIndex < playerNames.Count)
        {
            return playerNames[answererIndex];
        }
        return $"プレイヤー{answererIndex + 1}";
    }

    /// <summary>
    /// 現在のヒント提供者のプレイヤーインデックスを取得
    /// </summary>
    public int GetCurrentHintGiverPlayerIndex()
    {
        int hintGiverCount = 0;
        for (int i = 0; i < playerCount; i++)
        {
            if (i == answererIndex) continue;
            if (hintGiverCount == currentHintGiverIndex)
            {
                return i;
            }
            hintGiverCount++;
        }
        return -1;
    }

    /// <summary>
    /// 現在のヒント提供者の名前を取得
    /// </summary>
    public string GetCurrentHintGiverName()
    {
        int index = GetCurrentHintGiverPlayerIndex();
        if (index >= 0 && index < playerNames.Count)
        {
            return playerNames[index];
        }
        return "";
    }

    /// <summary>
    /// ヒント値を追加
    /// </summary>
    public void AddHintValue(string value)
    {
        hintValues.Add(value);
        currentHintGiverIndex++;
    }

    /// <summary>
    /// 全ヒントが入力済みか判定
    /// </summary>
    public bool IsAllHintsGiven()
    {
        return hintValues.Count >= playerCount - 1;
    }

    /// <summary>
    /// 回答の正誤判定
    /// </summary>
    public bool CheckAnswer(string answer)
    {
        if (string.IsNullOrEmpty(answer)) return false;
        
        string normalizedTopic = currentTopic.Replace(" ", "").Replace("　", "").ToLower();
        string normalizedAnswer = answer.Replace(" ", "").Replace("　", "").ToLower();
        
        return normalizedTopic.Contains(normalizedAnswer) || normalizedAnswer.Contains(normalizedTopic);
    }

    /// <summary>
    /// ヒント一覧を文字列で取得
    /// </summary>
    public string GetHintsDisplayText()
    {
        string result = "";
        int hintIndex = 0;
        for (int i = 0; i < playerCount; i++)
        {
            if (i == answererIndex) continue;
            if (hintIndex < hintValues.Count)
            {
                result += $"{playerNames[i]}: {hintValues[hintIndex]}\n";
                hintIndex++;
            }
        }
        return result;
    }

    /// <summary>
    /// 次のラウンドへ進む
    /// </summary>
    public void NextRound()
    {
        currentRound++;
        answererIndex = (answererIndex + 1) % playerCount;
        SetupNewRound();
    }

    /// <summary>
    /// ゲームを最初からリセット
    /// </summary>
    public void ResetGame()
    {
        currentRound = 1;
        answererIndex = 0;
        hintValues.Clear();
        currentHintGiverIndex = 0;
        CurrentState = GameState.PlayerCountSelect;
    }
}
