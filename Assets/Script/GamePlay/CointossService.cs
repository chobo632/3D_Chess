using UnityEngine;

namespace Chess.GamePlay
{
    public class CointossService
    {
        private bool playerSelectedPattern;
        private bool lastResult;

        // プレイヤーの表裏選択を保存
        public void SetPlayerChoice(bool choseHeadTail)
        {
            playerSelectedPattern = choseHeadTail;
        }

        // 抽選を実行し、表示用メッセージを返す
        public string Execute()
        {
            lastResult = Random.value >= 0.5f;
            string resultStr = lastResult ? "表" : "裏";
            string whoFirst = playerSelectedPattern == lastResult ? "1Pが先攻(白)" : "2Pが先攻(白)";

            return $"{resultStr} \n{whoFirst}";
        }

        // プレイヤーが白かどうかを返す
        public bool IsPlayerWhite()
        {
            return playerSelectedPattern == lastResult;
        }
    }
}