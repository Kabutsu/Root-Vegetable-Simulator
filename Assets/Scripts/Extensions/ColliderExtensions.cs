using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static Vector3 GetRandomPointInsideCollider(this BoxCollider2D boxCollider)
        {
            Vector2 extents = boxCollider.size / 2f;
            Vector2 point = new Vector3(
                Random.Range(-extents.x, extents.x),
                Random.Range(-extents.y, extents.y)
            );

            return boxCollider.transform.TransformPoint(point);
        }
    }
}
