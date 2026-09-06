using UnityEngine;

namespace Chess.Core
{
    public class HPBillboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main == null)
            { 
                return; 
            }

            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
