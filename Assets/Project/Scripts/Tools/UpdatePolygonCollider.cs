using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Tools
{
    public class UpdatePolygonCollider : MonoBehaviour
    {
        #region Serialized Fields

        public SpriteRenderer SpriteRenderer;

        #endregion

        [Button]
        public void UpdatePolygon()
        {
            var collider = GetComponent<PolygonCollider2D>();
            var sprite = SpriteRenderer.sprite;

            if (collider != null && sprite != null)
            {
                // update count
                collider.pathCount = sprite.GetPhysicsShapeCount();

                // new paths variable
                var path = new List<Vector2>();

                // loop path count
                for (var i = 0; i < collider.pathCount; i++)
                {
                    // clear
                    path.Clear();
                    // get shape
                    sprite.GetPhysicsShape(i, path);
                    // set path
                    collider.SetPath(i, path.ToArray());
                }
            }
        }
    }
}