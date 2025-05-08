using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Harpia.PrefabBrush
{
    [DisallowMultipleComponent]
    public class PrefabBrushPivot : MonoBehaviour
    {
#if UNITY_EDITOR
        
        [SerializeField, Tooltip("When placing this object, this pivot will align with any attractor with the name inside this list")]
        private string[] attractedBy;

        [SerializeField]
        private bool showGizmos = false;
        
        private PrefabBrushObject parent;
        private MeshRenderer _meshRenderer;
        

        private void OnValidate()
        {
            this.hideFlags = HideFlags.DontSaveInBuild;

            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponentInParent<MeshRenderer>();
            }
        }

        public bool IsAttractedBy(PrefabBrushAttractor prefabBrushPivot)
        {
            if (attractedBy == null) return false;
            return attractedBy.Contains(prefabBrushPivot.AttractorName);
        }


        public bool SameParent(PrefabBrushAttractor currentAttractor)
        {
            if (parent == null) parent = GetComponentInParent<PrefabBrushObject>();
            return parent == currentAttractor.GetParent();
        }


        public string[] GetAttractedByArray()
        {
            return attractedBy;
        }

        private void OnDrawGizmosSelected()
        {
         
            if(!showGizmos) return;
            if(_meshRenderer == null) return;
            
            var bounds = _meshRenderer.bounds;
            
            //Draw the bounds
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            
            //Line in the fron
            float height = bounds.size.y / 2;
            float lenght = bounds.size.z / 2;
            float width = bounds.size.x / 2;
            
            //Horizontal line
            Vector3 diff = transform.forward * width;
            Vector3 startPos = bounds.center + Vector3.up * height - diff;
            Vector3 endPos = bounds.center + Vector3.up * height + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            startPos = bounds.center + Vector3.up * -height - diff;
            endPos = bounds.center + Vector3.up * -height + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            //Vertical line
            diff = transform.right * lenght;
            startPos = bounds.center + Vector3.up * height - diff;
            endPos = bounds.center + Vector3.up * height + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            startPos = bounds.center + Vector3.up * -height - diff;
            endPos = bounds.center + Vector3.up * -height + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            //Horizontal line 2
            diff = transform.up * height;
            startPos = bounds.center + Vector3.right * width - diff;
            endPos = bounds.center + Vector3.right * width + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            startPos = bounds.center + Vector3.right * -width - diff;
            endPos = bounds.center + Vector3.right * -width + diff;
            Gizmos.DrawLine(startPos, endPos);
            
            

        }
        
        
        
        
#endif
    }
}