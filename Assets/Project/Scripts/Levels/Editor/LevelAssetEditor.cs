using System.Collections.Generic;
using Infrastructure.Services.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Levels.Editor
{
    [CustomEditor(typeof(LevelAsset))]
    public class LevelAssetEditor : UnityEditor.Editor
    {
        // делаем режим и кисть статическими, чтобы они не сбрасывались при смене SO
        private static ToolMode _mode = ToolMode.Block;
        private static readonly List<ColorID> _stackLayers = new();

        #region Event Functions

        // ---------- ENABLE / DISABLE ----------

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
        }

        #endregion

        // ---------- INSPECTOR ----------

        public override void OnInspectorGUI()
        {
            var lvl = (LevelAsset)target;

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            lvl.width = EditorGUILayout.IntField("Width", lvl.width);
            lvl.height = EditorGUILayout.IntField("Height", lvl.height);
            lvl.hexSize = EditorGUILayout.FloatField("Hex Size", lvl.hexSize);
            lvl.origin = EditorGUILayout.Vector3Field("Origin", lvl.origin);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Tool", EditorStyles.boldLabel);
            _mode = (ToolMode)EditorGUILayout.EnumPopup("Mode", _mode);

            if (_mode == ToolMode.PlaceStack)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Stack Layers (top → bottom)", EditorStyles.boldLabel);

                if (GUILayout.Button("Добавить слой"))
                {
                    _stackLayers.Add(ColorID.White);
                }

                for (var i = 0; i < _stackLayers.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    _stackLayers[i] = (ColorID)EditorGUILayout.EnumPopup(_stackLayers[i]);
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        _stackLayers.RemoveAt(i);
                        i--;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (_stackLayers.Count == 0)
                {
                    EditorGUILayout.HelpBox("Добавь хотя бы один слой, чтобы рисовать стопки.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Burn Goal", EditorStyles.boldLabel);

            lvl.targetBurnedHexes = EditorGUILayout.IntField("Target Burned Hexes", lvl.targetBurnedHexes);

            EditorGUILayout.LabelField("Color Unlocks", EditorStyles.boldLabel);
            if (GUILayout.Button("Добавить правило цвета"))
            {
                lvl.colorUnlocks.Add(new LevelAsset.ColorUnlockRule
                {
                    color = ColorID.White,
                    unlockAtBurned = 0,
                });
            }

            for (var i = 0; i < lvl.colorUnlocks.Count; i++)
            {
                var rule = lvl.colorUnlocks[i];
                EditorGUILayout.BeginHorizontal();
                rule.color = (ColorID)EditorGUILayout.EnumPopup(rule.color, GUILayout.Width(100));
                rule.unlockAtBurned = EditorGUILayout.IntField("Unlock @", rule.unlockAtBurned);
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    lvl.colorUnlocks.RemoveAt(i);
                    i--;
                }
                else
                {
                    lvl.colorUnlocks[i] = rule;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Utils", EditorStyles.boldLabel);
            if (GUILayout.Button("Clear Stacks"))
            {
                lvl.stacks.Clear();
            }

            if (GUILayout.Button("Clear Blocked"))
            {
                lvl.blockedCells.Clear();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(lvl);
            }
        }

        // ---------- SCENE VIEW ----------

        private void OnSceneViewGUI(SceneView sv)
        {
            if (!(target is LevelAsset lvl))
            {
                return;
            }

            if (Selection.activeObject != lvl)
            {
                return;
            }

            var e = Event.current;

            // забираем мышь, чтобы не рисовался прямоугольник выделения
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            // сетка
            Handles.color = new Color(1, 1, 1, 0.07f);
            DrawHexGrid(lvl);

            // ЛКМ: и клик, и перетаскивание — кисть
            if (e.type is EventType.MouseDown or EventType.MouseDrag &&
                e.button == 0 && !e.alt)
            {
                var world = GetMouseWorld(lvl.origin.y);
                var xy = WorldToGridPos(world, lvl);

                if (xy.x >= 0 && xy.x < lvl.width && xy.y >= 0 && xy.y < lvl.height)
                {
                    PaintCell(lvl, xy.x, xy.y);
                    EditorUtility.SetDirty(lvl);
                    SceneView.RepaintAll();
                    e.Use();
                }
            }

            // оверлеи поверх сетки
            DrawOverlay(lvl);
        }

        // ---------- ACTIONS ----------

        private void PaintCell(LevelAsset lvl, int x, int y)
        {
            switch (_mode)
            {
                case ToolMode.Block:
                    MarkBlocked(lvl, x, y);
                    break;

                case ToolMode.Unblock:
                    Unblock(lvl, x, y);
                    break;

                case ToolMode.PlaceStack:
                    PlaceStack(lvl, x, y);
                    break;

                case ToolMode.EraseStack:
                    EraseStack(lvl, x, y);
                    break;
            }
        }

        private void MarkBlocked(LevelAsset lvl, int x, int y)
        {
            var exists = lvl.blockedCells.Exists(b => b.x == x && b.y == y);
            if (!exists)
            {
                lvl.blockedCells.Add(new LevelAsset.CellRef { x = x, y = y });
            }
        }

        private void Unblock(LevelAsset lvl, int x, int y)
        {
            lvl.blockedCells.RemoveAll(b => b.x == x && b.y == y);
        }

        private void PlaceStack(LevelAsset lvl, int x, int y)
        {
            if (_stackLayers.Count == 0)
            {
                return;
            }

            lvl.stacks.RemoveAll(s => s.cell.x == x && s.cell.y == y);

            var stack = new LevelAsset.StackDef
            {
                cell = new LevelAsset.CellRef { x = x, y = y },
                layers = new List<ColorID>(_stackLayers),
            };

            lvl.stacks.Add(stack);
        }

        private void EraseStack(LevelAsset lvl, int x, int y)
        {
            lvl.stacks.RemoveAll(s => s.cell.x == x && s.cell.y == y);
        }

        // ---------- DRAW ----------

        private void DrawHexGrid(LevelAsset lvl)
        {
            for (var x = 0; x < lvl.width; x++)
            {
                for (var y = 0; y < lvl.height; y++)
                {
                    DrawHex(GridToWorld(x, y, lvl), lvl.hexSize, new Color(1, 1, 1, 0.08f));
                }
            }
        }

        private void DrawOverlay(LevelAsset lvl)
        {
            // Allowed / Blocked
            for (var x = 0; x < lvl.width; x++)
            {
                for (var y = 0; y < lvl.height; y++)
                {
                    var isBlocked = lvl.blockedCells.Exists(b => b.x == x && b.y == y);
                    var pos = GridToWorld(x, y, lvl);

                    if (!isBlocked)
                    {
                        // Allowed – зелёный
                        DrawHex(pos, lvl.hexSize * 0.96f, new Color(0f, 1f, 0f, 0.18f));
                    }
                    else
                    {
                        // Blocked – красный крест
                        DrawHex(pos, lvl.hexSize * 0.9f, new Color(1f, 0f, 0f, 0.35f));
                        Handles.color = Color.red;
                        var s = lvl.hexSize * 0.4f;
                        Handles.DrawLine(pos + new Vector3(-s, 0, -s), pos + new Vector3(+s, 0, +s));
                        Handles.DrawLine(pos + new Vector3(-s, 0, +s), pos + new Vector3(+s, 0, -s));
                    }
                }
            }

            // Stacks – столбики
            foreach (var st in lvl.stacks)
            {
                if (st.layers == null || st.layers.Count == 0)
                {
                    continue;
                }

                var basePos = GridToWorld(st.cell.x, st.cell.y, lvl);
                var layerHeight = lvl.hexSize * 0.4f;
                var layerSize = lvl.hexSize * 0.7f;

                for (var i = 0; i < st.layers.Count; i++)
                {
                    var id = st.layers[i];
                    var col = ColorFor(id);
                    col.a = 0.9f;

                    var center = basePos + Vector3.up * (i * layerHeight + 0.02f);
                    DrawHex(center, layerSize, col);
                }

                // подпись высоты
                var top = basePos + Vector3.up * (st.layers.Count * layerHeight + 0.05f);
                Handles.color = Color.white;
                Handles.Label(top, $"{st.layers.Count}");
            }
        }

        private void DrawHex(Vector3 center, float size, Color col)
        {
            Handles.color = col;

            var pts = new Vector3[7];
            for (var i = 0; i < 6; i++)
            {
                // FLAT-TOP: углы кратные 60°
                var ang = Mathf.Deg2Rad * (60f * i);
                pts[i] = center + new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * size;
            }

            pts[6] = pts[0];

            Handles.DrawAAConvexPolygon(pts);
            Handles.DrawPolyLine(pts);
        }

        // ---------- GRID / WORLD ----------

        // Формулы совместимы с твоим HexGrid (even-q по координатам)
        private Vector3 GridToWorld(int x, int y, LevelAsset lvl)
        {
            var px = lvl.hexSize * (1.5f * x);
            var pz = -1 * lvl.hexSize * (Mathf.Sqrt(3f) * (y + (x & 1) * 0.5f));
            return lvl.origin + new Vector3(px, 0f, pz);
        }

        private Vector2Int WorldToGridPos(Vector3 world, LevelAsset lvl)
        {
            var local = world - lvl.origin;

            var colStep = 1.5f * lvl.hexSize;
            var x = Mathf.RoundToInt(local.x / colStep);

            var rowBase = local.z / (Mathf.Sqrt(3f) * lvl.hexSize);
            var yF = -1 * (rowBase + (x & 1) * 0.5f);
            var y = Mathf.RoundToInt(yF);

            return new Vector2Int(x, y);
        }

        private Vector3 GetMouseWorld(float planeY)
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var t = (planeY - ray.origin.y) / ray.direction.y;
            return ray.origin + ray.direction * t;
        }

        // ---------- COLOR MAPPING ----------

        private Color ColorFor(ColorID id)
        {
            switch (id)
            {
                default:
                case ColorID.White: return Color.white;
                case ColorID.Black: return Color.black;
                case ColorID.Blue: return new Color(0.2f, 0.4f, 1f);
                case ColorID.Yellow: return new Color(1f, 0.9f, 0.2f);
                case ColorID.Red: return new Color(1f, 0.2f, 0.2f);
                case ColorID.Cyan: return new Color(0.2f, 1f, 1f);
                case ColorID.Violet: return new Color(0.7f, 0.3f, 1f);
                case ColorID.Orange: return new Color(1f, 0.6f, 0.2f);
            }
        }

        #region Nested type: ${0}

        private enum ToolMode
        {
            Block, // пометить клетку как заблокированную
            Unblock, // снять блок
            PlaceStack, // поставить стопку
            EraseStack, // стереть стопку
        }

        #endregion
    }
}