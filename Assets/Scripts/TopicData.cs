using System.Collections.Generic;

/// <summary>
/// お題データを管理する静的クラス
/// </summary>
public static class TopicData
{
    // お題リスト
    private static readonly List<string> topics = new List<string>
    {
        // 日常生活系
        "未読のLINEの件数",
        "今日の歩数",
        "好きな数字",
        "持っている鍵の数",
        "今の気分の点数(1-10)",
        "今日は何度寝しましたか？",
        "一日の平均食事回数",
        "今月の外食回数",
        "昨日の睡眠時間（時間）",
        "持っている靴の数",
        "スマホの通知数",
        "スマホの充電残量(%)",
        "今財布に入っている小銭の枚数",
        
        // 趣味・嗜好系
        "契約しているサブスクの数",
        "納豆の好きさの度合(0-100)",
        "最長で続いた趣味の継続年数",
        "自分にとって縁起の良い数字",
        "個人的に好きな時代の西暦",
        "好きなアーティストの曲数",
        "今年読んだ本の数",
        "今年見た映画の数",
        
        // 人生経験系
        "今年の旅行に行った回数",
        "人生で海外旅行に行った回数",
        "人生で引っ越しした回数",
        "今年、歯医者に行った回数",
        "人生で転職した回数",
        
        // お金系
        "人生で一番高い買い物の値段(万円)",
        "今月使ったコンビニの回数",
        
        // ネット・SNS系
        "自分の名前の画数",
        "登録しているYouTubeチャンネルの数",
        "フォローしているアカウントの数",
        
        // その他
        "自分の体重(kg)",
        "自分の身長(cm)"
    };

    /// <summary>
    /// ランダムでお題を取得
    /// </summary>
    public static string GetRandomTopic()
    {
        int idx = UnityEngine.Random.Range(0, topics.Count);
        return topics[idx];
    }

    /// <summary>
    /// お題リストを取得
    /// </summary>
    public static List<string> GetTopics()
    {
        return new List<string>(topics);
    }
}
