using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Harpia.PrefabBrush
{
    /// <summary>
    ///     This script allows any game object to be interacted with by the Prefab Brush.
    /// </summary>
    [DisallowMultipleComponent]
    public class PrefabBrushObject : MonoBehaviour
    {
#if UNITY_EDITOR

        private static float clippingSliderValuePlacing = 1;
        private static float clippingSliderValueErasing = 1;

        public struct RadiusBounds
        {
            public Vector3 pivot;
            public float clippingRadiusPlacing;
            public float clippingRadiusErasing;
            public readonly float originalRadius;

            public RadiusBounds(Bounds cachedBounds, Transform root)
            {
                originalRadius = (cachedBounds.extents.x + cachedBounds.extents.z) / 2;
                clippingRadiusPlacing = originalRadius * clippingSliderValuePlacing;
                clippingRadiusErasing = originalRadius * clippingSliderValuePlacing;

#if HARPIA_DEBUG
                if (clippingSliderValuePlacing <= 0) Debug.Log("clippingSliderValuePlacing is <= 0");
                if (clippingSliderValuePlacing <= 0) Debug.Log("clippingSliderValuePlacing is <= 0");
                if (originalRadius <= 0) Debug.Log("originalRadius is <= 0");
                if (clippingRadiusPlacing <= 0) Debug.Log("clippingRadiusPlacing is <= 0");
                if (clippingRadiusErasing <= 0) Debug.Log("clippingRadiusErasing is <= 0");
#endif

                pivot = root.transform.position;
            }

            public bool Intersects(RadiusBounds other, bool usePlacing)
            {
#if HARPIA_DEBUG
                if (other.pivot == Vector3.zero && clippingRadiusPlacing == 0) Debug.LogError($"Intersects {other.pivot} {other.clippingRadiusPlacing} {pivot} {clippingRadiusPlacing}");
#endif
                float r = usePlacing ? other.clippingRadiusPlacing : other.clippingRadiusErasing;
                return Intersects(other.pivot, r, usePlacing);
            }

            
            public bool Intersects(Vector3 pointPoint, float radiusCheckVal, bool usePlacingRadius)
            {
                float distanceSqr = (pivot - pointPoint).sqrMagnitude;
                float radiusSum1 = ((usePlacingRadius ? clippingRadiusPlacing : clippingRadiusErasing) + radiusCheckVal) / 2f;
                radiusSum1 *= radiusSum1;
                return distanceSqr < radiusSum1;
            }
        }

        //Transforms
        private Vector3 _lastBoundsPosition;
        private Vector3 _lastBoundsScale;
        private Quaternion _lastBoundsRotation;

        //Bounds
        private RadiusBounds _cachedRadiusBounds;
        private Bounds _cachedBounds;

        [Tooltip("Allow this object to be erased by the Prefab Brush.")]
        public bool erasable = true;

        [Tooltip("When enabled, this function assesses if a newly painted object would reside within the boundaries of an existing object. If a potential overlap is detected, it prevents the placement of the new object to ensure objects don't clip into one another.")]
        public bool meshClippingChecks = true;

        [Tooltip("If this object should use a special layer for physics while the user is deleting. Default is -1, which means it will not be used")]
        [Range(-1, 31)]
        public int customLayer = -1;

        private static PrefabBrushObject[] _brushObjectsCache;
        private static List<PrefabBrushObject> _toDispose;

        [SerializeField]
        [HideInInspector]
        private MeshCollider addedCollider;

        [SerializeField]
        [HideInInspector]
        private Rigidbody addedBody;

        /// <summary>
        ///     Initializes the PrefabBrushObject, called by the painter when a new object is painted.
        /// </summary>
        /// <param name="isUsingPhysics">
        ///     If the user wants to paint with physics, int this object will be added collider and
        ///     rigidbody
        /// </param>
        /// <param name="makeErasable">If this object can be erasable by the erase mode</param>
        /// <param name="clippingTest">If this object should be evaluated when checking the clipping </param>
        /// <param name="specialLayer">If this object should use a special layer for physics while the user is deleting</param>
        public void Init(bool isUsingPhysics, bool makeErasable, bool clippingTest, int specialLayer)
        {
            erasable = makeErasable;
            meshClippingChecks = clippingTest;
            customLayer = specialLayer;

            if (isUsingPhysics)
            {
                //Lets add a collider
                if (!HasPhysicalCollider(gameObject))
                {
                    MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        addedCollider = gameObject.AddComponent<MeshCollider>();
                        addedCollider.convex = true;
                    }
                }

                //Let's make sure collider is convex
                else
                {
                    MeshCollider meshCollider = GetComponentInChildren<MeshCollider>();
                    if (meshCollider != null) meshCollider.convex = true;
                }

                //Lets add a RB
                if (!HasValidRigidbody(gameObject))
                {
                    addedBody = gameObject.AddComponent<Rigidbody>();
                    addedBody.isKinematic = false;
                    addedBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }

#if HARPIA_DEBUG
                Debug.Log($"[{nameof(PrefabBrushObject)}] Added collider and rigidbody to {gameObject.name}", gameObject);
#endif

                _toDispose ??= new List<PrefabBrushObject>();
                _toDispose.Add(this);
            }

            hideFlags = HideFlags.DontSaveInBuild;

            InitCachedBounds();
        }

        private void DrawBounds(Vector3 hitPos, float sqrDistance)
        {
            float distanceSqr = (transform.position - hitPos).sqrMagnitude;
            if (distanceSqr > sqrDistance) return;

            Handles.color = Color.yellow;
            Transform transform1 = transform;
            Handles.zTest = CompareFunction.Always;
            Handles.DrawWireArc(_cachedRadiusBounds.pivot, transform1.up, transform1.right, 360, _cachedRadiusBounds.clippingRadiusPlacing);
        }

        private void DrawBoundsEraser2(Vector3 hitPos, float drawBoundsDistanceSqr, float radiusInterectValue, bool usePlacingRadius)
        {
            float distanceSqr = (transform.position - hitPos).sqrMagnitude;
            if (distanceSqr > drawBoundsDistanceSqr) return;

            bool intersects = _cachedRadiusBounds.Intersects(hitPos, radiusInterectValue, usePlacingRadius);
            Handles.color = intersects ? Color.red : Color.yellow;
            Transform transform1 = transform;
            float r = usePlacingRadius ? _cachedRadiusBounds.clippingRadiusPlacing : _cachedRadiusBounds.clippingRadiusErasing;
            Handles.DrawWireArc(_cachedRadiusBounds.pivot, transform1.up, transform1.right, 360, r);
        }

        public static void DrawBoundsCircles(Vector3 hitPos, float drawBoundsDistance, float radiusIntersectValue, bool usePlacingRadius)
        {
            PrefabBrushObject[] all = GetBrushObjects();
            drawBoundsDistance = Mathf.Clamp(drawBoundsDistance, 20f, 130f);
            drawBoundsDistance *= drawBoundsDistance;
            foreach (PrefabBrushObject brushObject in all)
            {
                if (brushObject == null) continue;
                if (brushObject.erasable == false) continue;
                brushObject.DrawBoundsEraser2(hitPos, drawBoundsDistance, radiusIntersectValue, usePlacingRadius);
            }
        }

        private void OnValidate()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }

        public static PrefabBrushObject[] GetBrushObjects()
        {
            if (_brushObjectsCache == null) ForceInit();
            return _brushObjectsCache;
        }

        private static Bounds GetBoundsStatic(Transform obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0) return new Bounds(obj.position, Vector3.zero);

            Bounds bounds = renderers[0].bounds;
            for (int index = 1; index < renderers.Length; index++)
            {
                Renderer renderer = renderers[index];
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        public RadiusBounds GetBoundsSphere()
        {
            if (transform.localScale == _lastBoundsScale &&
                transform.position == _lastBoundsPosition &&
                transform.rotation == _lastBoundsRotation)
                return _cachedRadiusBounds;

            InitCachedBounds();

            return _cachedRadiusBounds;
        }

        public Bounds GetBounds()
        {
            if (transform.localScale == _lastBoundsScale &&
                transform.position == _lastBoundsPosition &&
                transform.rotation == _lastBoundsRotation)
                return _cachedBounds;

            InitCachedBounds();

            return _cachedBounds;
        }

        private void OnDrawGizmosSelected()
        {
            //Draw the bounds
            if (_cachedRadiusBounds.clippingRadiusPlacing == 0 || _cachedBounds.size.sqrMagnitude == 0)
            {
                InitCachedBounds();
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_cachedRadiusBounds.pivot, _cachedRadiusBounds.originalRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_cachedRadiusBounds.pivot, _cachedRadiusBounds.clippingRadiusPlacing);
        }

        [ContextMenu("Initialize Cached Bounds")]
        private void InitCachedBounds()
        {
            _cachedBounds = GetBoundsStatic(transform);

            Transform t = transform;
            Vector3 pos = t.position;

            _cachedRadiusBounds = new RadiusBounds(_cachedBounds, t);

            _lastBoundsPosition = pos;
            _lastBoundsScale = t.localScale;
            _lastBoundsRotation = t.rotation;
        }

        public static bool HasPhysicalCollider(GameObject o)
        {
            //Copy this method to prefabBrushObject
            foreach (Collider collider in o.GetComponentsInChildren<Collider>())
            {
                if (!collider.enabled) continue;
                if (collider.isTrigger) continue;
                return true;
            }

            return false;
        }

        private static bool HasValidRigidbody(GameObject o)
        {
            //Copy this method to prefabBrushObject
            foreach (Rigidbody rigidbody in o.GetComponentsInChildren<Rigidbody>())
                if (rigidbody.isKinematic == false)
                    return true;

            return false;
        }

        public static void Dispose(bool removePhysicalComponents = true)
        {
            if (removePhysicalComponents && _toDispose != null)
            {
                bool removed = false;
                foreach (PrefabBrushObject o in _toDispose)
                {
                    if (o == null) continue;
                    o.DisposeObject();
                    removed = true;
                    
                    
                }

                _toDispose.Clear();
                if (removed) Debug.Log("[Prefab Brush] Colliders and Rigidbodies removed");
                //PB_HandlesExtension.WriteTempText("Colliders and Rigidbodies removed", 1.2f);
#if HARPIA_DEBUG
                Debug.Log("[PrefabBrushObject] Dispose physics objects");
#endif
            }

            if (_brushObjectsCache == null) return;

            foreach (PrefabBrushObject obj in _brushObjectsCache)
            {
                if (obj == null) continue;
                obj.DisposeObject();
            }

            _brushObjectsCache = null;
        }

        private void DisposeObject()
        {
            if (addedCollider != null) DestroyImmediate(addedCollider);
            if (addedBody != null) DestroyImmediate(addedBody);
        }

        public static void ForceInit()
        {
#if UNITY_2022_2_OR_NEWER
            _brushObjectsCache = FindObjectsByType<PrefabBrushObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            _brushObjectsCache = FindObjectsOfType<PrefabBrushObject>();
#endif
            foreach (PrefabBrushObject brushObject in _brushObjectsCache) brushObject.InitCachedBounds();
        }

        public static void SetClippingSize(float value, bool isPlacing)
        {
            if (isPlacing)
            {
                if (Math.Abs(clippingSliderValuePlacing - value) < .001f) return;
                clippingSliderValuePlacing = value;
            }
            else
            {
                if (Math.Abs(clippingSliderValueErasing - value) < 0.001f) return;
                clippingSliderValueErasing = value;
            }
            
            PrefabBrushObject[] all = GetBrushObjects();
            foreach (PrefabBrushObject brushObject in all)
            {
                if (brushObject == null) continue;
                brushObject.UpdateClippingSize(isPlacing);
            }
        }

        private void UpdateClippingSize(bool isPlacing)
        {
            if (isPlacing)
            {
                _cachedRadiusBounds.clippingRadiusPlacing = _cachedRadiusBounds.originalRadius * clippingSliderValuePlacing;
                return;
            }

            _cachedRadiusBounds.clippingRadiusErasing = _cachedRadiusBounds.originalRadius * clippingSliderValueErasing;
        }

        public bool BoundsIntersect(Vector3 raycastHitPoint, float radius, bool isPlacing)
        {
            //Debug.Log($"Bounds interects isPlacing {isPlacing} - meshClippingChecks {meshClippingChecks}");
            if (!meshClippingChecks) return false;
            if (_cachedBounds.center == Vector3.zero && _cachedBounds.size == Vector3.zero) InitCachedBounds();
            return _cachedRadiusBounds.Intersects(raycastHitPoint, radius, isPlacing);
        }
        

        public static void SetClippingSize(Slider clippingToleranceSlider, Slider clippingsliderValueErase)
        {
            clippingSliderValuePlacing = 1 - clippingToleranceSlider.value;
            clippingSliderValueErasing = 1 - clippingsliderValueErase.value;
        }

        public bool IsUsingCustomPhysicsLayer()
        {
            return customLayer != -1;
        }
#endif
    }
}