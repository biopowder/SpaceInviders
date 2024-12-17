using UnityEngine;

namespace ShootEmUp
{
    public class Factory<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private T prefab;

        [SerializeField]
        private Transform worldTransform;

        public T Create()
        {
            return Instantiate(prefab, worldTransform);
        }
    }
}