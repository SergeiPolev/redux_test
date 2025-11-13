using System;
using System.Collections.Generic;
using Infrastructure.Services.Gameplay;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Hex/Level Asset")]
    public class LevelAsset : ScriptableObject
    {
        #region Serialized Fields

        [Header("Grid")] public int width = 8;
        public int height = 8;
        public float hexSize = 1f;
        public Vector3 origin = Vector3.zero;

        [Header("Стопки (стартовые)")] public List<StackDef> stacks = new();

        [Header("Заблокированные клетки")] public List<CellRef> blockedCells = new();

        // ------------- ЦЕЛЬ УРОВНЯ: СЖИГАНИЕ ----------------

        [Header("Burn Goal")] [Tooltip("Сколько гексов нужно сжечь, чтобы пройти уровень")]
        public int targetBurnedHexes = 30;

        [Tooltip("На каком количестве сожжённых гексов какие цвета начинают появляться")]
        public List<ColorUnlockRule> colorUnlocks = new();

        #endregion

        // --------------------- HELPERS ------------------------

        // Цель достигнута?
        public bool IsLevelCompleted(int burnedCount)
        {
            return burnedCount >= targetBurnedHexes;
        }

        // Можно ли этот цвет уже использовать при таком количестве сожжённых?
        public bool IsColorAvailable(ColorID color, int burnedCount)
        {
            var hasRule = false;
            var minUnlock = int.MaxValue;

            foreach (var rule in colorUnlocks)
            {
                if (rule.color != color)
                {
                    continue;
                }

                hasRule = true;
                if (rule.unlockAtBurned < minUnlock)
                {
                    minUnlock = rule.unlockAtBurned;
                }
            }

            if (!hasRule)
            {
                return false; // нет правил — цвет не используется на этом уровне
            }

            return burnedCount >= minUnlock;
        }

        // Все доступные цвета при данном количестве сожжённых
        public List<ColorID> GetAvailableColors(int burnedCount)
        {
            var result = new List<ColorID>();

            foreach (var rule in colorUnlocks)
            {
                if (burnedCount >= rule.unlockAtBurned)
                {
                    if (!result.Contains(rule.color))
                    {
                        result.Add(rule.color);
                    }
                }
            }

            return result;
        }

        public bool IsBlocked(int x, int y)
        {
            foreach (var c in blockedCells)
            {
                if (c.x == x && c.y == y)
                {
                    return true;
                }
            }

            return false;
        }

        #region Nested type: ${0}

        // --------------------- STRUCTS ------------------------

        [Serializable]
        public struct CellRef
        {
            public int x;
            public int y;
        }

        [Serializable]
        public struct StackDef
        {
            public CellRef cell;

            [Tooltip("Список цветов: [0] — верхний, последний — нижний")]
            public List<ColorID> layers;
        }

        [Serializable]
        public struct ColorUnlockRule
        {
            public ColorID color;

            [Tooltip("Минимальное количество сожжённых, с которого этот цвет можно спавнить")]
            public int unlockAtBurned;
        }

        #endregion
    }
}