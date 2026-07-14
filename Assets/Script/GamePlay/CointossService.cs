using UnityEngine;

namespace Chess.GamePlay
{
    public class CointossService
    {
        private bool playerSelectedPattern;

        // プレイヤーの表裏選択を保存
        public void SetPlayerChoice(bool choseHeadTail)
        {
            playerSelectedPattern = choseHeadTail;
        }

        // 抽選を実行し、表示用メッセージを返す
        public string Execute()
        {
            bool isHeadOrTail = Random.value >= 0.5f;
            string resultStr = isHeadOrTail ? "表" : "裏";
            string whoFirst = playerSelectedPattern == isHeadOrTail ? "1Pが先攻(白)" : "2Pが先攻(白)";

            return $"{resultStr} \n{whoFirst}";
        }
    }
}