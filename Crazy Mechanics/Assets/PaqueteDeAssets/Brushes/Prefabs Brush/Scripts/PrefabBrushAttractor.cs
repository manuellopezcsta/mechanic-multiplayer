using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
#endif

namespace Harpia.PrefabBrush
{
    [DisallowMultipleComponent]
    public class PrefabBrushAttractor : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private string attractorName = "";
        public string AttractorName => attractorName;

        [SerializeField, Min(0.01f)] private float attractorRadius = 0.25f;
        
        private PrefabBrushObject parent;
        
        private void OnValidate()
        {
            this.hideFlags = HideFlags.DontSaveInBuild;
        }

        public Object GetParent()
        {
            if(parent == null) parent = GetComponentInParent<PrefabBrushObject>();
            return parent;
        }

        private void OnDrawGizmosSelected()
        {
            var allObjs = UnityEditor.Selection.gameObjects;
            if(!allObjs.Contains(this.gameObject)) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attractorRadius);
            
            //draw a line
            Gizmos.color = Color.red;
            
            Vector3 up = Vector3.up * attractorRadius;
            Vector3 right = Vector3.right * attractorRadius;
            Vector3 forward = Vector3.forward * attractorRadius;
            
            Gizmos.DrawLine(transform.position - up, transform.position + up);
            Gizmos.DrawLine(transform.position - right, transform.position + right);
            Gizmos.DrawLine(transform.position - forward, transform.position + forward);
            
        }

        public string GetAttactorName() => attractorName;
        public float GetRadius() => attractorRadius;
        
#endif
    }
}