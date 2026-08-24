using UnityEngine;

namespace Chess.Core
{
    public class BattleStats
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; }
        public int ATK { get; }

        public BattleStats(int maxHP, int atk, int? currentHP = null)
        {
            MaxHP = maxHP;
            CurrentHP = currentHP.HasValue ? Mathf.Clamp(currentHP.Value, 1, maxHP) : maxHP;
            ATK = atk;
        }

        public void TakeDamage(int damage)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - damage);
        }

        public void Heal(int amount)
        {
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        }

        public bool IsDefeated => CurrentHP <= 0;

        // 駒種ごとのデフォルトステータス
        public static BattleStats CreateDefault(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => new BattleStats(30, 10),
                PieceType.Knight => new BattleStats(50, 20),
                PieceType.Bishop => new BattleStats(50, 30),
                PieceType.Rook => new BattleStats(150, 10),
                PieceType.Queen => new BattleStats(100, 30),
                PieceType.King => new BattleStats(100, 30),
                _ => new BattleStats(0, 0)
            };
        }
    }
}