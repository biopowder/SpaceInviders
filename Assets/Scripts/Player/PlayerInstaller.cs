using UnityEngine;

namespace ShootEmUp
{
    public class PlayerInstaller : MonoBehaviour
    {
        [SerializeField]
        private Ship character;

        [SerializeField]
        private BulletManager bulletManager;
        
        private void Start()
        {
            character.Setup(bulletManager);
        }
    }
}