using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;

// ReSharper disable Unity.RedundantFormerlySerializedAsAttribute
// ReSharper disable MemberCanBePrivate.Global
#endif

namespace Harpia.PrefabBrush
{
    public class PrefabBrushTemplate : ScriptableObject
    {
        //Lists 
        public const string Prefabs = "prefabs";

        [FormerlySerializedAs(Prefabs), Space]
        public List<PbPrefabUI> prefabs;

        //Strings
        public const string ParentName = "parentName";

        [FormerlySerializedAs(ParentName)]
        public string parentName;

        //Objects
        public const string AllowedTextures = "allowedTextures";

        [FormerlySerializedAs(AllowedTextures)]
        public List<Texture> allowedTextures;

        public const string ParentObject = "parentObject";

        [FormerlySerializedAs(ParentObject)]
        public Transform parentObject;

        public const string Template = "template";

        [FormerlySerializedAs(Template)]
        public PrefabBrushTemplate template;

        //Colors
        public const string ClippingBoundsColor = "clippingBoundsColor";

        [FormerlySerializedAs(ClippingBoundsColor)]
        public Color clippingBoundsColor = Color.yellow;

        // public const string BrushColor = "BrushColor";
        // [FormerlySerializedAs(BrushColor)]
        // public Color brushColor;
        //
        // public const string BrushBorderColor = "BrushBorderColor";
        // [FormerlySerializedAs(BrushBorderColor)]
        // public Color brushBorderColor;
        //
        // public const string EraserColor = "EraserColor";
        // [FormerlySerializedAs(EraserColor)]
        // public Color eraserColor;
        //
        // public const string GridColor = "GridColor";
        // [FormerlySerializedAs(GridColor)]
        // public Color gridColor; 
        //
        // public const string InvalidColor = "InvalidColor";
        // [FormerlySerializedAs(InvalidColor)]
        // public Color invalidColor;

        //Enums
        public const string PivotModeValue = "pivotModeValue";

        [FormerlySerializedAs(PivotModeValue)]
        public PrefabBrush.PivotMode pivotModeValue;

        public const string PaintMode = "paintMode";

        [FormerlySerializedAs(PaintMode)]
        public PrefabBrush.PaintMode paintMode;

        public const string RaycastMode = "raycastMode";

        [FormerlySerializedAs(RaycastMode)]
        public PrefabBrush.RaycastMode raycastMode;

        public const string ParentMode = "parentMode";

        [FormerlySerializedAs(ParentMode)]
        public PrefabBrush.ParentMode parentMode;

        public const string RotationMode = "rotationMode";

        [FormerlySerializedAs(RotationMode)]
        public PrefabBrush.RotationTypes rotationMode;

        public const string ScaleMode = "scaleMode";

        [FormerlySerializedAs(ScaleMode)]
        public PrefabBrush.Vector3ModeUniform scaleMode;

        public const string OffsetMode = "offsetMode";

        [FormerlySerializedAs(OffsetMode)]
        public PrefabBrush.Vector3Mode offsetMode;

        public const string ImpulseMode = "impulseMode";

        [FormerlySerializedAs(ImpulseMode)]
        public PrefabBrush.Vector3Mode impulseMode;

        public const string PhysicsAffectMode = "physicsAffectMode";

        [FormerlySerializedAs(PhysicsAffectMode)]
        public PB_PhysicsSimulator.AffectMode physicsAffectMode;

        public const string PhysicsSimulationMode = "physicsSimulationMode";

        [FormerlySerializedAs(PhysicsSimulationMode)]
        public PB_PhysicsSimulator.SimulationMode physicsSimulationMode;

        public const string SnapMode = "snapMode";

        [FormerlySerializedAs(SnapMode)]
        public PB_SnappingManager.SnapModeEnum snapMode;

        //ints
        public const string TagMask = "tagMask";

        [FormerlySerializedAs(TagMask)]
        public int tagMask;

        public const string LayerMask = "layerMask";

        [FormerlySerializedAs(LayerMask)]
        public int layerMask;

        public const string EraserMask = "eraserMask";

        [FormerlySerializedAs(EraserMask)]
        public int eraserMask;

        public const string EraserTagMask = "eraserTagMask";

        [FormerlySerializedAs(EraserTagMask)]
        public int eraserTagMask;

        public const string ParentInstanceID = "parentInstanceID";

        [FormerlySerializedAs(ParentInstanceID)]
        public int parentInstanceID;

        public const string PhysicsCustomLayer = "physicsCustomLayer";

        [FormerlySerializedAs(PhysicsCustomLayer)]
        public int physicsCustomLayer;

        public const string VertexSnapLayerMask = "vertexSnapLayerMask";

        [FormerlySerializedAs(VertexSnapLayerMask)]
        public int vertexSnapLayerMask = -1;

        public const string VertexSnapTagMask = "vertexSnapTagMask";

        [FormerlySerializedAs(VertexSnapTagMask)]
        public int vertexSnapTagMask = -1;

        //floats
        public const string BrushSize = "brushSize";

        [FormerlySerializedAs(BrushSize)]
        public float brushSize = 1;

        public const string BrushStrength = "brushStrength";

        [FormerlySerializedAs(BrushStrength)]
        public float brushStrength = .5f;

        public const string ClippingStrenght = "clippingStreght";

        [FormerlySerializedAs(ClippingStrenght)]
        public float clippingStreght = .5f;

        public const string EraserSize = "eraserSize";
        [FormerlySerializedAs(EraserSize)]
        public float eraserSize;
        
        public const string EraserClipping = "eraserClipping";
        [FormerlySerializedAs(EraserClipping), Range(0,1f)]
        public float eraseClipping;

        public const string FakePlaneYPosition = "fakePlaneYPosition";

        [FormerlySerializedAs(FakePlaneYPosition)]
        public float fakePlaneYPosition;

        public const string GridSnapValue = "gridSnapValue";

        [FormerlySerializedAs(GridSnapValue)]
        public float gridSnapValue;

        public const string PhysicsImpulseForce = "physicsImpulseForce";

        [FormerlySerializedAs(PhysicsImpulseForce)]
        public float physicsImpulseForce;

        public const string PhysicsStepValue = "physicsStepValue";

        [FormerlySerializedAs(PhysicsStepValue)]
        public float physicsStepValue;

        public const string ScaleMaxUniform = "scaleMaxUniform";

        [FormerlySerializedAs(ScaleMaxUniform)]
        public float scaleMaxUniform = 1;

        public const string ScaleMinUniform = "scaleMinUniform";

        [FormerlySerializedAs(ScaleMinUniform)]
        public float scaleMinUniform;

        public const string VertexSnapDistance = "vertexSnapDistance";

        [FormerlySerializedAs(VertexSnapDistance)]
        public float vertexSnapDistance = 1;

        public const string WorldLinesDistance = "worldLinesDistance";

        [FormerlySerializedAs(WorldLinesDistance)]
        public float worldLinesDistance = 1;

        public const string PrecisionModeRotationAngle = "vertexSnapDistance";

        [FormerlySerializedAs(PrecisionModeRotationAngle)]
        public float precisionModeRotationAngle;

        //bools
        public const string AddToClippingTest = "addToClippingTest";

        [FormerlySerializedAs(AddToClippingTest)]
        public bool addToClippingTest = true;

        public const string AlignWithGround = "alignWithGround";
        [FormerlySerializedAs(AlignWithGround)]
        public bool alignWithGround;
        
        public const string AlignWithHitAxisX = "alignWithHitAxisX";
        [FormerlySerializedAs(AlignWithHitAxisX)]
        public bool alignWithHitAxisX = true;
        
        public const string AlignWithHitAxisY = "alignWithHitAxisY";
        [FormerlySerializedAs(AlignWithHitAxisY)]
        public bool alignWithHitAxisY = true;
        
        public const string AlignWithHitAxisZ = "alignWithHitAxisZ";
        [FormerlySerializedAs(AlignWithHitAxisZ)]
        public bool alignWithHitAxisZ = true;

        public const string ConstrainedScaleFixed = "constrainedScaleFixed";

        [FormerlySerializedAs(ConstrainedScaleFixed)]
        public bool constrainedScaleFixed;

        public const string ConstrainedScaleMax = "constrainedScaleMax";

        [FormerlySerializedAs(ConstrainedScaleMax)]
        public bool constrainedScaleMax;

        public const string ConstrainedScaleMin = "constrainedScaleMin";

        [FormerlySerializedAs(ConstrainedScaleMin)]
        public bool constrainedScaleMin;

        public const string DeletePhysicsOnly = "deletePhysicsOnly";

        [FormerlySerializedAs(DeletePhysicsOnly)]
        public bool deletePhysicsOnly;

        public const string MakeErasable = "makeErasable";

        [FormerlySerializedAs(MakeErasable)]
        public bool makeErasable = true;

        public const string MakeStatic = "makeStatic";

        [FormerlySerializedAs(MakeStatic)]
        public bool makeStatic;

        public const string PhysicsUseCustomLayer = "physicsUseCustomLayer";

        [FormerlySerializedAs(PhysicsUseCustomLayer)]
        public bool physicsUseCustomLayer;

        public const string PrecisionModeAddMesh = "precisionModeAddMesh";

        [FormerlySerializedAs(PrecisionModeAddMesh)]
        public bool precisionModeAddMesh;

        public const string PrecisionModeChangePrefabAfterPlacing = "precisionModeChangePrefabAfterPlacing";

        [FormerlySerializedAs(PrecisionModeChangePrefabAfterPlacing)]
        public bool precisionModeChangePrefabAfterPlacing;
        

        public const string ShowClippingBounds = "showClippingBounds";

        [FormerlySerializedAs(ShowClippingBounds)]
        public bool showClippingBounds;

        public const string ShowWorldLines = "showWorldLines";

        [FormerlySerializedAs(ShowWorldLines)]
        public bool showWorldLines;

        public const string UniformScaleParent = "uniformScaleParent";

        [FormerlySerializedAs(UniformScaleParent)]
        public bool uniformScaleParent;

        public const string UseAngleLimits = "useAngleLimits";

        [FormerlySerializedAs(UseAngleLimits)]
        public bool useAngleLimits;

        public const string UseFakePlaneIfNoHit = "useFakePlaneIfNoHit";

        [FormerlySerializedAs(UseFakePlaneIfNoHit)]
        public bool useFakePlaneIfNoHit;

        public const string UseImpulse = "useImpulse";

        [FormerlySerializedAs(UseImpulse)]
        public bool useImpulse;

        public const string UseTextureMask = "useTextureMask";

        [FormerlySerializedAs(UseTextureMask)]
        public bool useTextureMask;

        //Vectors
        public const string FixedOffset = "fixedOffset";

        [FormerlySerializedAs(FixedOffset), Space]
        public Vector3 fixedOffset;

        public const string MaxRotation = "maxRotation";

        [FormerlySerializedAs(MaxRotation)]
        public Vector3 maxRotation;

        public const string MinScale = "minScale";
        [FormerlySerializedAs(MinScale)]
        public Vector3 minScale;
        
        public const string MinScaleUniform = "minScaleUniform";
        [FormerlySerializedAs(MinScaleUniform)]
        public Vector3 minScaleUniform;


        public const string AngleLimits = "angleLimits";

        [FormerlySerializedAs(AngleLimits)]
        public Vector2 angleLimits;

        public const string GridOffset = "gridOffset";

        [FormerlySerializedAs(GridOffset)]
        public Vector2 gridOffset;

        public const string FixedPhysicsImpulse = "fixedPhysicsImpulse";

        [FormerlySerializedAs(FixedPhysicsImpulse)]
        public Vector3 fixedPhysicsImpulse;

        public const string FixedRotation = "fixedRotation";

        [FormerlySerializedAs(FixedRotation)]
        public Vector3 fixedRotation;

        public const string FixedScale = "fixedScale";

        [FormerlySerializedAs(FixedScale)]
        public Vector3 fixedScale;

        public const string LookAtRotX = "lookAtRotX";

        [FormerlySerializedAs(LookAtRotX)]
        public bool lookAtRotX;
        
        public const string LookAtRotY = "lookAtRotY";

        [FormerlySerializedAs(LookAtRotY)]
        public bool lookAtRotY;
        
        public const string LookAtRotZ = "lookAtRotZ";

        [FormerlySerializedAs(LookAtRotZ)]
        public bool lookAtRotZ;

        public const string LookAtRotObject = "lookAtRotObject";

        [FormerlySerializedAs(LookAtRotObject)]
        public bool lookAtRotObject;
        
        public const string MaxOffset = "maxOffset";

        [FormerlySerializedAs(MaxOffset)]
        public Vector3 maxOffset;

        public const string MaxPhysicsImpulse = "maxPhysicsImpulse";

        [FormerlySerializedAs(MaxPhysicsImpulse)]
        public Vector3 maxPhysicsImpulse;

        public const string MaxScale = "maxScale";
        [FormerlySerializedAs(MaxScale)]
        public Vector3 maxScale;
        
        public const string MaxScaleUniform = "maxScaleUniform";
        [FormerlySerializedAs(MaxScaleUniform)]
        public Vector3 maxScaleUniform;

        public const string MinOffset = "minOffset";

        [FormerlySerializedAs(MinOffset)]
        public Vector3 minOffset;

        public const string MinPhysicsImpulse = "minPhysicsImpulse";

        [FormerlySerializedAs(MinPhysicsImpulse)]
        public Vector3 minPhysicsImpulse;

        public const string MinRotation = "minRotation";

        [FormerlySerializedAs(MinRotation)]
        public Vector3 minRotation;

        //physics

        public const string ImpulseFixed = "impulseFixed";

        [FormerlySerializedAs(ImpulseFixed)]
        public Vector3 impulseFixed;

        public const string ImpulseMax = "impulseMax";

        [FormerlySerializedAs(ImpulseMax)]
        public Vector3 impulseMax;

        public const string ImpulseMin = "impulseMin";

        [FormerlySerializedAs(ImpulseMin)]
        public Vector3 impulseMin;

        // Advanced settings
        




        public const string ScrollRotationSpeed = "scrollRotationSpeed";

        [FormerlySerializedAs(ScrollRotationSpeed)]
        public float scrollRotationSpeed;

        //Pen
        public const string UsePenToogle = "usePenToogle";

        [FormerlySerializedAs(UsePenToogle)]
        public bool usePenToogle;

        public const string PenPressureMode = "penPressureMode";

        [FormerlySerializedAs(PenPressureMode)]
        public PB_PressurePen.PenPressureUseMode penPressureMode;

        //Custom props

        public const string CustomPivotMode = "customPivotMode";

        [FormerlySerializedAs(CustomPivotMode), HideInInspector]
        public PrefabBrush.PivotMode customPivotMode;

        public const string CustomPropsOffsetMode = "customPropsOffsetMode";

        [FormerlySerializedAs(CustomPropsOffsetMode), HideInInspector]
        public PrefabBrush.Vector3Mode customPropsOffsetMode;

        public const string CustomPropsOffsetFixed = "customPropsOffsetFixed";

        [FormerlySerializedAs(CustomPropsOffsetFixed), HideInInspector]
        public Vector3 customPropsOffsetFixed;

        public const string CustomPropsOffsetMax = "customPropsOffsetMax";

        [FormerlySerializedAs(CustomPropsOffsetMax), HideInInspector]
        public Vector3 customPropsOffsetMax;

        public const string CustomPropsOffsetMin = "customPropsOffsetMin";

        [FormerlySerializedAs(CustomPropsOffsetMin), HideInInspector]
        public Vector3 customPropsOffsetMin;

        public const string CustomPropsScaleMode = "customPropsScaleMode";

        [FormerlySerializedAs(CustomPropsScaleMode), HideInInspector]
        public PrefabBrush.Vector3Mode customPropsScaleMode;

        public const string CustomPropsScaleFixed = "customPropsScaleFixed";

        [FormerlySerializedAs(CustomPropsScaleFixed), HideInInspector]
        public Vector3 customPropsScaleFixed;

        public const string CustomPropsScaleMax = "customPropsScaleMax";
        [FormerlySerializedAs(CustomPropsScaleMax), HideInInspector]
        public Vector3 customPropsScaleMax;
        
        public const string CustomPropsScaleMaxUniform = "customPropsScaleMaxUniform";
        [FormerlySerializedAs(CustomPropsScaleMaxUniform), HideInInspector]
        public Vector3 customPropsScaleMaxUniform;


        public const string CustomPropsScaleMin = "customPropsScaleMin";
        [FormerlySerializedAs(CustomPropsScaleMin), HideInInspector]
        public Vector3 customPropsScaleMin;
        
        public const string CustomPropsScaleMinUniform = "customPropsScaleMinUniform";
        [FormerlySerializedAs(CustomPropsScaleMinUniform), HideInInspector]
        public Vector3 customPropsScaleMinUniform;

        public const string CustomPropsRotationMode = "customPropsRotationMode";

        [FormerlySerializedAs(CustomPropsRotationMode), HideInInspector]
        public PrefabBrush.RotationTypes customPropsRotationMode;

        public const string CustomPropsRotationFixed = "customPropsRotationFixed";

        [FormerlySerializedAs(CustomPropsRotationFixed), HideInInspector]
        public Vector3 customPropsRotationFixed;

        public const string CustomPropsRotationMax = "customPropsRotationMax";

        [FormerlySerializedAs(CustomPropsRotationMax), HideInInspector]
        public Vector3 customPropsRotationMax;

        public const string CustomPropsRotationMin = "customPropsRotationMin";

        [FormerlySerializedAs(CustomPropsRotationMin), HideInInspector]
        public Vector3 customPropsRotationMin;

        //Foldouts
        public const string FoldoutMainSettings = "foldoutMainSetings";

        [FormerlySerializedAs(FoldoutMainSettings), HideInInspector]
        public bool foldoutMainSetings = true;

        public const string FoldoutParenting = "foldoutParenting";

        [FormerlySerializedAs(FoldoutParenting), HideInInspector]
        public bool foldoutParenting = true;

        public const string FoldoutBrushMultipleMode = "foldoutBrushMultipleMode";

        [FormerlySerializedAs(FoldoutBrushMultipleMode), HideInInspector]
        public bool foldoutBrushMultipleMode = true;

        public const string FoldoutPlacement = "foldoutPlacement";

        [FormerlySerializedAs(FoldoutPlacement), HideInInspector]
        public bool foldoutPlacement = true;

        public const string FoldoutTransforms = "foldoutTransforms";

        [FormerlySerializedAs(FoldoutTransforms), HideInInspector]
        public bool foldoutTransforms = true;

        public const string FoldoutPhysics = "foldoutPhysics";

        [FormerlySerializedAs(FoldoutPhysics), HideInInspector]
        public bool foldoutPhysics = true;

        public const string FoldoutAdvancedSettings = "foldoutAdvancedSettings";

        [FormerlySerializedAs(FoldoutAdvancedSettings), HideInInspector]
        public bool foldoutAdvancedSettings = true;

        public const string FoldoutEraseMode = "foldoutEraseMode";
        [FormerlySerializedAs(FoldoutEraseMode), HideInInspector]
        public bool foldoutEraseMode = true;

        public const string FoldoutTemplateSelection = "foldoutTemplateSelection";

        [FormerlySerializedAs(FoldoutTemplateSelection), HideInInspector]
        public bool foldoutTemplateSelection = true;

        public const string FoldoutTemplateSection = "foldoutTemplateSection";

        [FormerlySerializedAs(FoldoutTemplateSection), HideInInspector]
        public bool foldoutTemplateSection = true;

        public const string FoldoutPrecisionSection = "foldoutPrecisionSection";

        [FormerlySerializedAs(FoldoutPrecisionSection), HideInInspector]
        public bool foldoutPrecisionSection = true;

        //Static
        private static List<PrefabBrushTemplate> _allPresets;
        private static PrefabBrushTemplate LoadedPreset => PrefabBrush._lastLoadedTemplate;

        //Const
        public const string NoPresetsFound = "No presets found";
        private const string KeyLastTemplate = "PBLastTemplate";
        private const string KeyLastTemplatePath = "PBLastTemplatePath";

        public void CopyDataFrom(PrefabBrush model)
        {
            var fields = typeof(PrefabBrushTemplate).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    FieldInfo modelField = model.GetType().GetField(field.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (modelField == null) continue;
                    field.SetValue(this, modelField.GetValue(model));
                }
                catch (Exception ex)
                {
#if HARPIA_DEBUG
                    Debug.LogError($"Failed to initialize field {field.Name}: {ex.Message}");
#endif
                }
            }

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrushTemplate)}] Preset cloned. If any variables were not saved please check here");
#endif
        }

        public bool SaveScriptableObject()
        {
            string openPath = GetLastPath().Replace("Assets", Application.dataPath) + "/";
            string path = EditorUtility.SaveFilePanelInProject("Save Brush Preset", GetFileName(), "asset", "Save Preset", openPath);

            if (string.IsNullOrEmpty(path)) return false;

            EditorPrefs.SetString(KeyLastTemplate, path);

            PrefabBrushTemplate asset = CreateInstance<PrefabBrushTemplate>();
            asset.name = Path.GetFileNameWithoutExtension(path);
            asset.CloneFrom(this);
            asset.hideFlags = HideFlags.DontSaveInBuild;
            AssetDatabase.CreateAsset(asset, path);

            //load asset at path
            PrefabBrushTemplate loadedAsset = AssetDatabase.LoadAssetAtPath<PrefabBrushTemplate>(path);
            PrefabBrush._lastLoadedTemplate = loadedAsset;
            PrefabBrush.instance._templatesDropdown.value = loadedAsset.name;

            Debug.Log($"Prefab Brush: Preset saved to {path}", loadedAsset);
            return true;

            string GetFileName()
            {
                if (PrefabBrush._lastLoadedTemplate != null)
                    return PrefabBrush._lastLoadedTemplate.name;

                return "Prefab Brush Preset - ";
            }

            string GetLastPath()
            {
                string lastPath = EditorPrefs.GetString(KeyLastTemplatePath, "Assets");
                if (PrefabBrush._lastLoadedTemplate != null) lastPath = AssetDatabase.GetAssetPath(PrefabBrush._lastLoadedTemplate);
                return Path.GetDirectoryName(lastPath);
            }
        }

        public void CloneFrom(PrefabBrushTemplate other)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrushTemplate)}] Clone from {other.name} to {name}");
#endif
            FieldInfo[] fields = typeof(PrefabBrushTemplate).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.Name.Equals("template")) continue;

                try
                {
#if HARPIA_DEBUG
                    //Color c = Color.cyan;
                    //string value = field.GetValue(other).ToString();
                    //Debug.Log($"[{nameof(PrefabBrushTemplate)}]  Cloning field {PrefabBrushTool.GetColorfulText(field.Name, c)} From {PrefabBrushTool.GetColorfulText(other.name, c)} to {PrefabBrushTool.GetColorfulText(name, c)} - {field.Name} {field.FieldType} {value}");
#endif
                    field.SetValue(this, field.GetValue(other));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to copy field {field.Name}: {ex.Message}");
                }
            }

            prefabs = other.prefabs;
            brushSize = other.brushSize;

            //force the reserialization of the object
            EditorUtility.SetDirty(this);
        }

        public static List<string> GetAllPresetsNames()
        {
            //find all scriptable objects in the project of tye PrefabBrushTemplate
            string[] guids = AssetDatabase.FindAssets("t:PrefabBrushTemplate");
            List<string> allNames = new();
            _allPresets = new();

            if (guids.Length == 0)
            {
                allNames.Add(NoPresetsFound);
                return allNames;
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;
                PrefabBrushTemplate template = AssetDatabase.LoadAssetAtPath<PrefabBrushTemplate>(path);
                if (template == null) continue;

                allNames.Add(template.name);
                _allPresets.Add(template);
            }

            return allNames;
        }

        public static void DeleteTemplate(string templatesDropdownValue)
        {
            throw new NotImplementedException();
        }

        public Transform GetParent()
        {
            if (string.IsNullOrEmpty(parentName)) return null;

            //Instance ID
            GameObject possibleParent = EditorUtility.InstanceIDToObject(parentInstanceID) as GameObject;
            if (possibleParent != null) return possibleParent.transform;

            //Name
            possibleParent = GameObject.Find(parentName);
            if (possibleParent == null) return null;

            return possibleParent.transform;
        }

        public static void SetLastTemplate(string evtNewValue)
        {
            EditorPrefs.SetString(KeyLastTemplate, evtNewValue);
        }

        public static void RevealCurrentTemplate()
        {
            if (LoadedPreset == null) return;

            Debug.Log("[Prefab Brush] Make Sure a project tab is available");
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = LoadedPreset;
        }

        public static PrefabBrushTemplate LoadTemplate(string presetName)
        {
            if (string.IsNullOrEmpty(presetName)) return null;
            if (presetName == "None") return null;

            PrefabBrushTemplate t = _allPresets.Find(x => x.name == presetName);

            if (t != null) return t;

            PrefabBrush.DisplayError($"Template {presetName} not found! Did you renamed or delete it?");
            return null;
        }

        public static string GetCurrentTemplateName()
        {
            if (LoadedPreset == null) return "None";
            return LoadedPreset.name;
        }

        [ContextMenu("Open Brush with this template")]
        public void OpenWithThisTemplate()
        {
            PrefabBrush.ShowWindow();
            PrefabBrush.instance.LoadTemplate(this);
        }

        public void SetAllFoldoutsOpen(bool b)
        {
            foldoutParenting = b;
            foldoutMainSetings = b;
            foldoutBrushMultipleMode = b;
            foldoutPlacement = b;
            foldoutTransforms = b;
            foldoutPhysics = b;
            foldoutAdvancedSettings = b;
            foldoutEraseMode = b;
            foldoutTemplateSelection = b;
            foldoutTemplateSection = b;
            foldoutPrecisionSection = b;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PrefabBrushTemplate))]
    public class PrefabBrushTemplateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //button GUI
            if (GUILayout.Button("Open Prefab Brush With this template"))
            {
                PrefabBrushTemplate script = (PrefabBrushTemplate)target;
                script.OpenWithThisTemplate();
            }

            //space
            GUILayout.Space(20);

            DrawDefaultInspector();
        }
    }

#endif
}