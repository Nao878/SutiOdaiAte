using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 起動時にすべてのUIを動的に生成し、ゲームフローを制御するクラス
/// </summary>
public class GameController : MonoBehaviour
{
    // カラーパレット
    private readonly Color primaryColor = new Color(0.2f, 0.5f, 0.9f, 1f);
    private readonly Color successColor = new Color(0.2f, 0.75f, 0.4f, 1f);
    private readonly Color warningColor = new Color(0.95f, 0.6f, 0.2f, 1f);
    private readonly Color dangerColor = new Color(0.9f, 0.3f, 0.3f, 1f);
    private readonly Color bgColor = new Color(0.12f, 0.12f, 0.18f, 1f);
    
    // UI参照
    private Canvas mainCanvas;
    private GameObject playerCountPanel;
    private GameObject coverPanel;
    private GameObject topicPanel;
    private GameObject hintInputPanel;
    private GameObject answerPanel;
    private GameObject resultPanel;

    // UI要素への参照
    private TMP_Text playerCountText;
    private TMP_Text coverMessageText;
    private TMP_Text topicDisplayText;
    private TMP_Text topicViewersText;
    private TMP_Text hintPlayerText;
    private TMP_InputField hintInputField;
    private TMP_Text answerHintsText;
    private TMP_InputField answerInputField;
    private TMP_Text resultText;

    void Start()
    {
        // GameManagerを作成
        if (GameManager.Instance == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }

        // UIを生成
        CreateCanvas();
        CreateAllPanels();
        
        // 初期画面を表示
        ShowPanel(playerCountPanel);
    }

    #region キャンバス作成
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("MainCanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystemがなければ作成
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
    #endregion

    #region パネル作成
    private void CreateAllPanels()
    {
        CreatePlayerCountPanel();
        CreateCoverPanel();
        CreateTopicPanel();
        CreateHintInputPanel();
        CreateAnswerPanel();
        CreateResultPanel();
    }

    /// <summary>
    /// プレイヤー人数選択画面
    /// </summary>
    private void CreatePlayerCountPanel()
    {
        playerCountPanel = CreatePanel("PlayerCountPanel");

        // タイトル
        TMP_Text title = CreateText(playerCountPanel.transform, "🎯 数値お題当てゲーム", 52);
        SetRectTransform(title.rectTransform, new Vector2(0.5f, 0.8f), new Vector2(0, 0), new Vector2(900, 120));
        title.color = new Color(1f, 0.85f, 0.3f);

        // サブタイトル
        TMP_Text subtitle = CreateText(playerCountPanel.transform, "みんなで遊ぶパーティーゲーム", 28);
        SetRectTransform(subtitle.rectTransform, new Vector2(0.5f, 0.72f), new Vector2(0, 0), new Vector2(800, 60));
        subtitle.color = Color.gray;

        // 説明
        TMP_Text instruction = CreateText(playerCountPanel.transform, "プレイヤー人数を選んでください", 30);
        SetRectTransform(instruction.rectTransform, new Vector2(0.5f, 0.58f), new Vector2(0, 0), new Vector2(800, 60));

        // 人数表示
        playerCountText = CreateText(playerCountPanel.transform, "4 人", 72);
        SetRectTransform(playerCountText.rectTransform, new Vector2(0.5f, 0.48f), new Vector2(0, 0), new Vector2(300, 100));
        playerCountText.color = successColor;
        playerCountText.fontStyle = FontStyles.Bold;

        // 減らすボタン
        Button decreaseBtn = CreateButton(playerCountPanel.transform, "◀", 48, new Vector2(130, 130), primaryColor);
        SetRectTransform(decreaseBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.48f), new Vector2(-180, 0), new Vector2(130, 130));
        decreaseBtn.onClick.AddListener(OnDecreasePlayer);

        // 増やすボタン
        Button increaseBtn = CreateButton(playerCountPanel.transform, "▶", 48, new Vector2(130, 130), primaryColor);
        SetRectTransform(increaseBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.48f), new Vector2(180, 0), new Vector2(130, 130));
        increaseBtn.onClick.AddListener(OnIncreasePlayer);

        // 範囲説明
        TMP_Text range = CreateText(playerCountPanel.transform, "（2〜5人）", 22);
        SetRectTransform(range.rectTransform, new Vector2(0.5f, 0.38f), new Vector2(0, 0), new Vector2(300, 50));
        range.color = Color.gray;

        // 開始ボタン
        Button startBtn = CreateButton(playerCountPanel.transform, "🎮 ゲーム開始", 36, new Vector2(450, 120), successColor);
        SetRectTransform(startBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.22f), new Vector2(0, 0), new Vector2(450, 120));
        startBtn.onClick.AddListener(OnStartGame);
    }

    /// <summary>
    /// 伏せ画面（プレイヤー交代用）
    /// </summary>
    private void CreateCoverPanel()
    {
        coverPanel = CreatePanel("CoverPanel");

        // メッセージ
        coverMessageText = CreateText(coverPanel.transform, "次のプレイヤーに\n渡してください", 42);
        SetRectTransform(coverMessageText.rectTransform, new Vector2(0.5f, 0.55f), new Vector2(0, 0), new Vector2(900, 400));
        coverMessageText.color = Color.white;

        // OKボタン
        Button okBtn = CreateButton(coverPanel.transform, "✅ 準備OK", 40, new Vector2(450, 130), successColor);
        SetRectTransform(okBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.2f), new Vector2(0, 0), new Vector2(450, 130));
        okBtn.onClick.AddListener(OnCoverOK);
    }

    /// <summary>
    /// お題表示画面
    /// </summary>
    private void CreateTopicPanel()
    {
        topicPanel = CreatePanel("TopicPanel");

        // タイトル
        TMP_Text title = CreateText(topicPanel.transform, "📋 お題を確認してください", 34);
        SetRectTransform(title.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0, 0), new Vector2(800, 80));

        // お題表示
        topicDisplayText = CreateText(topicPanel.transform, "お題", 48);
        SetRectTransform(topicDisplayText.rectTransform, new Vector2(0.5f, 0.65f), new Vector2(0, 0), new Vector2(900, 200));
        topicDisplayText.color = new Color(1f, 0.9f, 0.3f);
        topicDisplayText.fontStyle = FontStyles.Bold;

        // 確認者リスト
        topicViewersText = CreateText(topicPanel.transform, "確認者:\nプレイヤー2\nプレイヤー3", 26);
        SetRectTransform(topicViewersText.rectTransform, new Vector2(0.5f, 0.42f), new Vector2(0, 0), new Vector2(600, 250));
        topicViewersText.color = new Color(0.7f, 0.7f, 0.7f);

        // 確認完了ボタン
        Button confirmBtn = CreateButton(topicPanel.transform, "✅ 確認完了", 36, new Vector2(450, 120), successColor);
        SetRectTransform(confirmBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.15f), new Vector2(0, 0), new Vector2(450, 120));
        confirmBtn.onClick.AddListener(OnTopicConfirmed);
    }

    /// <summary>
    /// ヒント入力画面
    /// </summary>
    private void CreateHintInputPanel()
    {
        hintInputPanel = CreatePanel("HintInputPanel");

        // プレイヤー名表示
        hintPlayerText = CreateText(hintInputPanel.transform, "プレイヤー2の番です", 38);
        SetRectTransform(hintPlayerText.rectTransform, new Vector2(0.5f, 0.75f), new Vector2(0, 0), new Vector2(800, 100));
        hintPlayerText.fontStyle = FontStyles.Bold;

        // 説明
        TMP_Text instruction = CreateText(hintInputPanel.transform, "お題に関する数値を入力してください", 26);
        SetRectTransform(instruction.rectTransform, new Vector2(0.5f, 0.65f), new Vector2(0, 0), new Vector2(800, 60));
        instruction.color = Color.gray;

        // 入力フィールド
        hintInputField = CreateInputField(hintInputPanel.transform, "数値を入力...");
        SetRectTransform(hintInputField.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(500, 100));

        // 送信ボタン
        Button submitBtn = CreateButton(hintInputPanel.transform, "📤 送信", 36, new Vector2(400, 120), successColor);
        SetRectTransform(submitBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.3f), new Vector2(0, 0), new Vector2(400, 120));
        submitBtn.onClick.AddListener(OnHintSubmit);
    }

    /// <summary>
    /// 回答入力画面
    /// </summary>
    private void CreateAnswerPanel()
    {
        answerPanel = CreatePanel("AnswerPanel");

        // タイトル
        TMP_Text title = CreateText(answerPanel.transform, "🎯 お題を当ててください！", 36);
        SetRectTransform(title.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0, 0), new Vector2(800, 80));
        title.fontStyle = FontStyles.Bold;

        // ヒント一覧
        answerHintsText = CreateText(answerPanel.transform, "ヒント一覧:\nプレイヤー2: 10\nプレイヤー3: 5", 28);
        SetRectTransform(answerHintsText.rectTransform, new Vector2(0.5f, 0.65f), new Vector2(0, 0), new Vector2(700, 300));

        // 入力フィールド
        answerInputField = CreateInputField(answerPanel.transform, "お題を入力...");
        SetRectTransform(answerInputField.GetComponent<RectTransform>(), new Vector2(0.5f, 0.4f), new Vector2(0, 0), new Vector2(600, 100));

        // 回答ボタン
        Button answerBtn = CreateButton(answerPanel.transform, "🎯 回答する", 36, new Vector2(450, 120), successColor);
        SetRectTransform(answerBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.2f), new Vector2(0, 0), new Vector2(450, 120));
        answerBtn.onClick.AddListener(OnAnswerSubmit);
    }

    /// <summary>
    /// 結果画面
    /// </summary>
    private void CreateResultPanel()
    {
        resultPanel = CreatePanel("ResultPanel");

        // 結果テキスト
        resultText = CreateText(resultPanel.transform, "結果", 40);
        SetRectTransform(resultText.rectTransform, new Vector2(0.5f, 0.6f), new Vector2(0, 0), new Vector2(900, 400));

        // 次のラウンドボタン
        Button nextBtn = CreateButton(resultPanel.transform, "▶ 次のラウンド", 34, new Vector2(450, 110), primaryColor);
        SetRectTransform(nextBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.28f), new Vector2(0, 0), new Vector2(450, 110));
        nextBtn.onClick.AddListener(OnNextRound);

        // 最初からボタン
        Button restartBtn = CreateButton(resultPanel.transform, "🔄 最初から", 30, new Vector2(400, 90), warningColor);
        SetRectTransform(restartBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.13f), new Vector2(0, 0), new Vector2(400, 90));
        restartBtn.onClick.AddListener(OnRestart);
    }
    #endregion

    #region UI生成ヘルパー
    private GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(mainCanvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = bgColor;

        panel.SetActive(false);
        return panel;
    }

    private TMP_Text CreateText(Transform parent, string text, int fontSize)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);

        TMP_Text tmpText = obj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.enableWordWrapping = true;

        return tmpText;
    }

    private Button CreateButton(Transform parent, string text, int fontSize, Vector2 size, Color color)
    {
        GameObject btnObj = new GameObject("Button");
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        // ボタンテキスト
        TMP_Text btnText = CreateText(btnObj.transform, text, fontSize);
        btnText.rectTransform.anchorMin = Vector2.zero;
        btnText.rectTransform.anchorMax = Vector2.one;
        btnText.rectTransform.offsetMin = Vector2.zero;
        btnText.rectTransform.offsetMax = Vector2.zero;
        btnText.fontStyle = FontStyles.Bold;

        return btn;
    }

    private TMP_InputField CreateInputField(Transform parent, string placeholder)
    {
        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(parent, false);

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 80);

        Image bg = inputObj.AddComponent<Image>();
        bg.color = new Color(0.25f, 0.25f, 0.3f, 1f);

        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

        // テキストエリア
        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(15, 10);
        textAreaRect.offsetMax = new Vector2(-15, -10);
        textArea.AddComponent<RectMask2D>();

        // プレースホルダー
        TMP_Text placeholderText = CreateText(textArea.transform, placeholder, 28);
        placeholderText.rectTransform.anchorMin = Vector2.zero;
        placeholderText.rectTransform.anchorMax = Vector2.one;
        placeholderText.rectTransform.offsetMin = Vector2.zero;
        placeholderText.rectTransform.offsetMax = Vector2.zero;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f);
        placeholderText.alignment = TextAlignmentOptions.MidlineLeft;

        // 入力テキスト
        TMP_Text inputText = CreateText(textArea.transform, "", 28);
        inputText.rectTransform.anchorMin = Vector2.zero;
        inputText.rectTransform.anchorMax = Vector2.one;
        inputText.rectTransform.offsetMin = Vector2.zero;
        inputText.rectTransform.offsetMax = Vector2.zero;
        inputText.alignment = TextAlignmentOptions.MidlineLeft;

        inputField.textViewport = textAreaRect;
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;

        return inputField;
    }

    private void SetRectTransform(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private void ShowPanel(GameObject panel)
    {
        playerCountPanel.SetActive(panel == playerCountPanel);
        coverPanel.SetActive(panel == coverPanel);
        topicPanel.SetActive(panel == topicPanel);
        hintInputPanel.SetActive(panel == hintInputPanel);
        answerPanel.SetActive(panel == answerPanel);
        resultPanel.SetActive(panel == resultPanel);
    }
    #endregion

    #region イベントハンドラ
    private void OnDecreasePlayer()
    {
        if (GameManager.Instance.playerCount > 2)
        {
            GameManager.Instance.SetPlayerCount(GameManager.Instance.playerCount - 1);
            playerCountText.text = $"{GameManager.Instance.playerCount} 人";
        }
    }

    private void OnIncreasePlayer()
    {
        if (GameManager.Instance.playerCount < 5)
        {
            GameManager.Instance.SetPlayerCount(GameManager.Instance.playerCount + 1);
            playerCountText.text = $"{GameManager.Instance.playerCount} 人";
        }
    }

    private void OnStartGame()
    {
        GameManager.Instance.InitializePlayerNames();
        GameManager.Instance.SetupNewRound();
        GameManager.Instance.CurrentState = GameState.AnswererCover;

        // 回答者に見せない画面を表示
        coverMessageText.text = $"⚠️ {GameManager.Instance.GetAnswererName()}さんは\n画面を見ないでください！\n\n他のプレイヤーがお題を確認します";
        ShowPanel(coverPanel);
    }

    private void OnCoverOK()
    {
        switch (GameManager.Instance.CurrentState)
        {
            case GameState.AnswererCover:
                // お題表示へ
                ShowTopicScreen();
                break;
            case GameState.PlayerChange:
                // ヒント入力へ
                ShowHintInputScreen();
                break;
            case GameState.AnswerInput:
                // 回答入力へ
                ShowAnswerScreen();
                break;
        }
    }

    private void ShowTopicScreen()
    {
        GameManager.Instance.CurrentState = GameState.TopicDisplay;
        topicDisplayText.text = $"📋 {GameManager.Instance.currentTopic}";

        string viewers = "確認者:\n";
        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            if (i != GameManager.Instance.answererIndex)
            {
                viewers += $"・{GameManager.Instance.playerNames[i]}\n";
            }
        }
        topicViewersText.text = viewers;
        ShowPanel(topicPanel);
    }

    private void OnTopicConfirmed()
    {
        // 最初のヒント提供者への交代画面
        string nextPlayer = GameManager.Instance.GetCurrentHintGiverName();
        coverMessageText.text = $"📱 デバイスを\n{nextPlayer}さんに\n渡してください";
        GameManager.Instance.CurrentState = GameState.PlayerChange;
        ShowPanel(coverPanel);
    }

    private void ShowHintInputScreen()
    {
        GameManager.Instance.CurrentState = GameState.HintInput;
        hintPlayerText.text = $"🎯 {GameManager.Instance.GetCurrentHintGiverName()}さんの番です";
        hintInputField.text = "";
        ShowPanel(hintInputPanel);
    }

    private void OnHintSubmit()
    {
        string value = hintInputField.text.Trim();
        if (string.IsNullOrEmpty(value)) return;

        GameManager.Instance.AddHintValue(value);

        if (GameManager.Instance.IsAllHintsGiven())
        {
            // 回答者の番
            coverMessageText.text = $"🎯 {GameManager.Instance.GetAnswererName()}さんの番です！\n\nヒントを見てお題を当ててください";
            GameManager.Instance.CurrentState = GameState.AnswerInput;
            ShowPanel(coverPanel);
        }
        else
        {
            // 次のヒント提供者へ
            string nextPlayer = GameManager.Instance.GetCurrentHintGiverName();
            coverMessageText.text = $"📱 デバイスを\n{nextPlayer}さんに\n渡してください";
            GameManager.Instance.CurrentState = GameState.PlayerChange;
            ShowPanel(coverPanel);
        }
    }

    private void ShowAnswerScreen()
    {
        GameManager.Instance.CurrentState = GameState.AnswerInput;
        answerHintsText.text = $"📊 ヒント一覧:\n{GameManager.Instance.GetHintsDisplayText()}";
        answerInputField.text = "";
        ShowPanel(answerPanel);
    }

    private void OnAnswerSubmit()
    {
        string answer = answerInputField.text.Trim();
        if (string.IsNullOrEmpty(answer)) return;

        bool isCorrect = GameManager.Instance.CheckAnswer(answer);
        ShowResultScreen(isCorrect);
    }

    private void ShowResultScreen(bool isCorrect)
    {
        GameManager.Instance.CurrentState = GameState.Result;

        if (isCorrect)
        {
            resultText.text = $"🎉 正解！ 🎉\n\nお題:\n{GameManager.Instance.currentTopic}\n\n{GameManager.Instance.GetAnswererName()}さん、お見事！";
            resultText.color = successColor;
        }
        else
        {
            resultText.text = $"❌ 残念！\n\n正解は:\n「{GameManager.Instance.currentTopic}」\n\n次回頑張りましょう！";
            resultText.color = dangerColor;
        }
        ShowPanel(resultPanel);
    }

    private void OnNextRound()
    {
        GameManager.Instance.NextRound();
        GameManager.Instance.CurrentState = GameState.AnswererCover;
        coverMessageText.text = $"⚠️ {GameManager.Instance.GetAnswererName()}さんは\n画面を見ないでください！\n\n他のプレイヤーがお題を確認します";
        ShowPanel(coverPanel);
    }

    private void OnRestart()
    {
        GameManager.Instance.ResetGame();
        playerCountText.text = $"{GameManager.Instance.playerCount} 人";
        ShowPanel(playerCountPanel);
    }
    #endregion
}
