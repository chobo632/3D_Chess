using UnityEngine;
using TMPro;
using Chess.Core;
using Chess.Rules;

namespace Chess.UI
{
    public class PieceStatusPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private TextMeshProUGUI pieceNameText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI abilityText;

        void Start()
        {
            Hide();
        }

        public void Show(PieceModel piece, BattleStats stats)
        {
            panelGroup.alpha = 1;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;

            pieceNameText.text = piece.PieceType.ToString();
            hpText.text = $"HP：{stats.CurrentHP} / {stats.MaxHP}";
            atkText.text = $"ATK：{stats.ATK}";
            abilityText.text = GetAbilityText(piece.PieceType);
        }

        public void Hide()
        {
            panelGroup.alpha = 0;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }

        // HPが変化した時に更新
        public void UpdateHP(int current, int max)
        {
            hpText.text = $"HP：{current} / {max}";
        }

        private string GetAbilityText(PieceType type) => type switch
        {
            PieceType.Knight => "再行動（撃破時・上限2回）",
            PieceType.Bishop => "貫通ダメージ（経路の敵へ20→10）",
            PieceType.Rook => "肩代わり（隣接味方への攻撃を代受け）",
            PieceType.Queen => "範囲攻撃（移動後・周囲8マスへ10ダメージ）",
            PieceType.King => "HP回復（2ターン無被弾で+20）",
            _ => "なし"
        };
    }
}