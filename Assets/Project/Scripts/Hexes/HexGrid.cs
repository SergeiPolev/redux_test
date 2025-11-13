using System;
using System.Collections.Generic;
using System.Linq;
using Levels;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public class HexGrid
    {
        public int Width;
        public int Height;
        public float HexSize; // radius
        public Vector3 Origin = Vector3.zero; // world offset

        public HexCell[,] Cells;

        private LevelAsset _levelAsset;

        public event Action<int> OnHexesRemoved;
        public event Action<HexCell> OnCellChanged;

        // axial-направления (q,r) – тот же порядок, что и обычные соседства
        private static readonly (int dq, int dr)[] AXIAL_DIRS =
        {
            (1, 0),
            (1, -1),
            (0, -1),
            (-1, 0),
            (-1, 1),
            (0, 1)
        };

        // EVEN-Q (pointy-top) соседства
        private static readonly Vector2Int[] NEIGHBOURS_EVEN =
        {
            new(+1, 0),
            new(-1, -1),
            new(0, +1),
            new(+1, -1),
            new(-1, 0),
            new(0, -1),
        };

        private static readonly Vector2Int[] NEIGHBOURS_ODD =
        {
            new(+1, 0),
            new(+1, +1),
            new(0, +1),
            new(-1, 0),
            new(0, -1),
            new(-1, +1),
        };

        public HexGrid(LevelAsset levelAsset, float hexSize, GameFactory gameFactory)
        {
            _levelAsset = levelAsset;
            Width = levelAsset.width;
            Height = levelAsset.height;
            HexSize = hexSize;
            Origin = levelAsset.origin;

            Cells = new HexCell[Width, Height];
            Build(gameFactory);
        }

        // ------------------------ BUILD ------------------------
        private void Build(GameFactory gameFactory)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = y * Height + x;
                    var cell = new HexCell(index, x, y)
                    {
                        WorldPos = GridPosToWorld(x, y),
                        ModelView = gameFactory.CreateCellView(index)
                    };

                    cell.ModelView.transform.position = cell.WorldPos;
                    cell.ModelView.x = cell.x;
                    cell.ModelView.y = cell.y;

                    cell.SetState(_levelAsset.IsBlocked(x, y)
                        ? HexCell.HexCellState.Disabled
                        : HexCell.HexCellState.Normal);

                    bool SameId(LevelAsset.StackDef stackDef)
                    {
                        return stackDef.cell.x == cell.x && stackDef.cell.y == cell.y;
                    }

                    if (_levelAsset.stacks.Any(SameId))
                    {
                        List<ColorID> ids = _levelAsset.stacks.First(SameId).layers;
                        Hex[] hexes = new Hex[ids.Count];

                        for (int i = 0; i < ids.Count; i++)
                        {
                            ColorID id = ids[i];
                            hexes[i] = gameFactory.CreateHex(id);
                        }

                        cell.AddHexesInstant(hexes);
                    }

                    cell.OnCellChanged += CellChangedForward;
                    cell.OnHexesRemoved += HexesRemovedForward;
                    Cells[x, y] = cell;
                }
            }
        }

        private void HexesRemovedForward(int count) => OnHexesRemoved?.Invoke(count);

        private void CellChangedForward(HexCell cell) => OnCellChanged?.Invoke(cell);

        // ------------------------ BOUNDS ------------------------
        public bool IsInside(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;

        public Vector2Int ClampXY(Vector2Int xy) =>
            new(Mathf.Clamp(xy.x, 0, Width - 1), Mathf.Clamp(xy.y, 0, Height - 1));

        // ------------------- GRID <-> WORLD ---------------------
        // pointy-top, even-q
        // Grid pos as index
        public Vector3 GridPosToWorld(int x, int y)
        {
            float px = HexSize * (1.5f * x);
            float pz = -1 * HexSize * (Mathf.Sqrt(3f) * (y + ((x & 1) * 0.5f)));
            return Origin + new Vector3(px, 0f, pz);
        }

        // x ↔ world.x, y ↔ world.z
        public Vector2Int WorldToGridPos(Vector3 world)
        {
            Vector3 local = world - Origin;

            float colStep = 1.5f * HexSize;
            int x = Mathf.RoundToInt(local.x / colStep);

            float rowBase = local.z / (Mathf.Sqrt(3f) * HexSize);
            float yF = rowBase + ((x & 1) * 0.5f);
            int y = -1 * Mathf.RoundToInt(yF);

            return new Vector2Int(x, y);
        }

        // ------------------------ NEIGHBORS ---------------------
        public List<HexCell> GetNeighbours(int x, int y)
        {
            var result = new List<HexCell>();
            if (!IsInside(x, y)) return result;

            var dirs = (x & 1) == 0 ? NEIGHBOURS_EVEN : NEIGHBOURS_ODD;

            foreach (var d in dirs)
            {
                int nx = x + d.x;
                int ny = y + d.y;
                if (IsInside(nx, ny) && Cells[nx, ny].CellState != HexCell.HexCellState.Disabled)
                    result.Add(Cells[nx, ny]);
            }

            return result;
        }

        // ---------------------- LOOKUPS ------------------------
        public HexCell GetCellByIndex(int index)
        {
            int x = index % Width;
            int y = index / Width;
            return Cells[x, y];
        }

        public HexCell FindClosestCell(Vector3 world, bool onlyEmpty = false)
        {
            var xy = WorldToGridPos(world);
            if (IsInside(xy.x, xy.y))
            {
                var c = Cells[xy.x, xy.y];
                if (!onlyEmpty || c.IsEmpty()) return c;
            }

            return null;

            float bestD2 = float.PositiveInfinity;
            HexCell best = null;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var c = Cells[x, y];
                if (onlyEmpty && !c.IsEmpty()) continue;

                float d2 = (c.WorldPos - world).sqrMagnitude;
                if (d2 < bestD2)
                {
                    bestD2 = d2;
                    best = c;
                }
            }

            return best;
        }
        // ----- КОЛЬЦО РАДИУСА R ОТ (cx,cy) -----
// ----- AXIAL / OFFSET / DISTANCE -----

// axial q,r для pointy-top even-q (как у нас в гриде)
        private (int q, int r) OffsetToAxial(int x, int y)
        {
            int q = x;
            int r = y - (x >> 1); // y - floor(x/2)
            return (q, r);
        }

        private (int x, int y) AxialToOffset(int q, int r)
        {
            int x = q;
            int y = r + (q >> 1); // r + floor(q/2)
            return (x, y);
        }

        // расстояние в шагах между двумя клетками
        public int HexDistance(int x1, int y1, int x2, int y2)
        {
            var a1 = OffsetToAxial(x1, y1);
            var a2 = OffsetToAxial(x2, y2);

            // axial -> cube
            int xA = a1.q;
            int zA = a1.r;
            int yA = -xA - zA;

            int xB = a2.q;
            int zB = a2.r;
            int yB = -xB - zB;

            int dx = xA - xB;
            int dy = yA - yB;
            int dz = zA - zB;

            return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
        }

        public List<HexCell> GetRing(int cx, int cy, int radius)
        {
            var result = new List<HexCell>();

            // радиус 0 — только центральная ячейка
            if (radius == 0)
            {
                if (IsInside(cx, cy))
                    result.Add(Cells[cx, cy]);
                return result;
            }

            // центр в axial
            var center = OffsetToAxial(cx, cy);
            int cq = center.q;
            int cr = center.r;

            // стартовая точка на кольце:
            // идём из центра по направлению AXIAL_DIRS[4] (например, (-1,1)) на radius шагов
            int q = cq + AXIAL_DIRS[4].dq * radius;
            int r = cr + AXIAL_DIRS[4].dr * radius;

            // обходим 6 сторон многоугольника
            for (int side = 0; side < 6; side++)
            {
                var dir = AXIAL_DIRS[side];
                for (int step = 0; step < radius; step++)
                {
                    var off = AxialToOffset(q, r);
                    int ox = off.x;
                    int oy = off.y;

                    if (IsInside(ox, oy))
                        result.Add(Cells[ox, oy]);

                    q += dir.dq;
                    r += dir.dr;
                }
            }

            return result;
        }

        // ----------------------- CLEANUP -----------------------
        public void CleanUp()
        {
            if (Cells == null) return;
            foreach (var cell in Cells)
            {
                if (cell == null) continue;

                cell.CleanUp();
                cell.OnCellChanged = null;
            }

            Cells = null;
        }
    }
}