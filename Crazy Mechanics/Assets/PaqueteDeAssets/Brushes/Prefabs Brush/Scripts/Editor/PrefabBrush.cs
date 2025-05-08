//Available on the asset store at https://assetstore.unity.com/packages/tools/utilities/prefab-brush-easy-object-placement-tool-260560?aid=1100lACye&utm_campaign=unity_affiliate&utm_medium=affiliate&utm_source=partnerize-linkmaker
//Harpia Games Studio

#define UNITY_DISABLE_API_UPDATER

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using UnityEditor.EditorTools;
using UnityEngine.Serialization;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using LayerField = UnityEditor.UIElements.LayerField;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Label = UnityEngine.UIElements.Label;
using Button = UnityEngine.UIElements.Button;
using Slider = UnityEngine.UIElements.Slider;
using Toggle = UnityEngine.UIElements.Toggle;
using MaskField = UnityEditor.UIElements.MaskField;
using TextField = UnityEngine.UIElements.TextField;
using ColorField = UnityEditor.UIElements.ColorField;
using ObjectField = UnityEditor.UIElements.ObjectField;
using MinMaxSlider = UnityEngine.UIElements.MinMaxSlider;
using VisualElement = UnityEngine.UIElements.VisualElement;
using DropdownField = UnityEngine.UIElements.DropdownField;
using LayerMaskField = UnityEditor.UIElements.LayerMaskField;
using ClickEvent = UnityEngine.UIElements.ClickEvent;
using DragEnterEvent = UnityEngine.UIElements.DragEnterEvent;
using DragLeaveEvent = UnityEngine.UIElements.DragLeaveEvent;
using DragExitedEvent = UnityEngine.UIElements.DragExitedEvent;
using Foldout = UnityEngine.UIElements.Foldout;
using Application = UnityEngine.Application;

#if UNITY_2022_1_OR_NEWER
using Vector3Field = UnityEngine.UIElements.Vector3Field;
using DefaultEnumField = UnityEngine.UIElements.EnumField;
using FloatField = UnityEngine.UIElements.FloatField;

#else
using UnityEditor.UIElements;
using DefaultEnumField = UnityEditor.UIElements.EnumField; // This works in 2021.3
//using DefaultEnumField = UnityEngine.UIElements.EnumField;
#endif

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable CoVariantArrayConversion
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable PossibleNullReferenceException
// ReSharper disable Unity.InstantiateWithoutParent
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Harpia.PrefabBrush
{
    public class PrefabBrush : EditorWindow
    {
        #region Enums

        public enum PivotMode
        {
            MeshPivot,
            Bounds_Center,
            Bounds_Center_Bottom,
            Bounds_Center_Top,
            Bounds_Center_Forward,
            Bounds_Center_Back,
            Bounds_Center_Left,
            Bounds_Center_Right,
            Prefab_Brush_Pivot_Component
        }

        public enum PaintMode
        {
            Multiple,
            Precision,
            Eraser,
        }

        public enum RaycastMode
        {
            Physical_Collider,
            Mesh,
            Both,
        }

        public enum Vector3Mode
        {
            Fixed,
            Random,
        }

        public enum Vector3ModeUniform
        {
            Fixed,
            Random,
            Random_Uniform,
        }

        public enum RotationTypes
        {
            Fixed,
            Random,
            Scene_Camera_Rotation,
            Look_At_Object
        }

        public enum ParentMode
        {
            No_Parent,
            Fixed_Transform,
            Hit_Surface_Object,
        }

        #endregion

        #region VisualElementsVariables

        private VisualElement _shortcutParent;
        public VisualElement prefabsUiHolder;
        private VisualElement _statusBackground;
        private VisualElement _addSelectedPrefabsPanel;

        private DefaultEnumField pivotMode;
        private DefaultEnumField paintModeDropdown;
        private DefaultEnumField parentModeDropdown;
        private DefaultEnumField raycastModeDropdown;

        private Label _dragAndDropLabel;
        private Label _statusLabel;
        private Label _addSelectedPrefabsLabel;
        private Label _requiredParentWarningLabel;

        public Slider brushRadiusSlider;

        public Slider sliderBrushStrength;
        public Slider clippingToleranceSlider;

        public Toggle useAngleLimitsToggle;
        public Toggle showBoundsToggle;

        public Toggle precisionModeAddMeshToBatch;

        public Toggle _showWorldLines;

        public Toggle toggleAlignToHit;

        public Toggle precisionModeChangePrefabToggle;

        private Foldout _brushFoldout;
        private Foldout _precisionModeFoldout;

        private FloatField angleMinField;
        private FloatField angleMaxField;
        public FloatField precisionModeRotationAngle;
        public FloatField worldLinesDistance;

        public TextField parentNameInput;

        private Button _statusButton;
        private Button _paintModeButton;
        private Button _eraserModeButton;
        private Button createParentButton;
        private Button _addSelectedPrefabsButton;

        public MaskField tagMaskField;
        public ObjectField parentField;
        public MinMaxSlider angleLimitsField;
        public LayerMaskField layerMaskField;
        public DropdownField _templatesDropdown;

        public Camera sceneCamera;
        public bool isRaycastHit;
        public Vector3 firstRaycastHitPoint;
        public RaycastHit lastHitInfo;

        [FormerlySerializedAs("_randomPointInsideDisc")]
        public Vector3 randomPointInsideDisc;

        private string _dragAndDropOriginalText;

        public static PrefabBrush instance;

        public bool _isMouse0Down;
        public bool _isMouse1Down;
        private bool _isShiftDown;
        private bool _isAltDown;
        private bool _isCtrlDown;
        private bool _successPainted;

        private bool isXDown;
        private bool isYDown;
        private bool isZDown;
        private bool isChangeSizeDown;
        private bool isAdjustDistanceFromGroundDown;

        private Vector2 mouse0DownPosition;
        private Vector2 _lastMousePosCtrl;
        private Vector2 _lastMousePosShift;

        public List<string> _currentTagsSelected;
        private List<GameObject> _prefabsToAddFromSelection;
        public List<PbPrefabUI> currentPrefabs => _temporaryTemplate.prefabs;

        #endregion

        #region StaticVariables

        private const float brushSizeIncrement = 0.06f;
        private const string xmlFileName = "PrefabBrushXML";

        public static float deltaTime;
        private static Mesh _sphereMesh;
        private static Material _defaultMaterial;
        private static string _visualTreeGuid;
        private static double LastMouse0DownTime;
        private static double lastRepaintTime;
        private static readonly Color activeButtonColor = new Color(0f, 1f, 0.61f, 0.39f);
        public static readonly Color _styleBackgroundColorGreen = new Color(0f, 1f, 0f, 0.4f);
        public const string DebugLogStart = "[Prefab Brush]";
        //private static double mouse0DownTime => EditorApplication.timeSinceStartup - LastMouse0DownTime;

        private static PaintMode lastMode = PaintMode.Precision;
        private static bool isRaycastTagAllowed;
        private static bool _isTextureAllowed;
        public VisualElement paintModePanel;
        public Toggle _toggleUniformScaleParent;
        public Toggle makeErasableToggle;
        public Toggle makeObjectsStaticToggle;
        public Toggle addToClippingCheckToggle;
        public RotationTypeElement rotationField;
        public Vector3ModeElement scaleField;
        public Vector3ModeElement offsetField;
        public VisualElement spacer;
        public static PrefabBrushTemplate _lastLoadedTemplate;

        private Label notAvailableInPlayModeLabel;
        private VisualElement placementSection;
        public PrefabBrushTemplate _temporaryTemplate;
        public SerializedObject _serializedObject;
        private FloatField _hiddenField;
        private double _lastUpdateListTime;
        public PB_AdvancedSettings advancedSettings;
        private bool _freeRotation;
        private Vector2 _lastMousePos;
        private bool _lastIsMouseOverWindow;
        public Toggle _alignToHitAxisX;
        public Toggle _alignToHitAxisY;
        public Toggle _alignToHitAxisZ;

        #endregion

        #region UnityMessages

        [MenuItem("Tools/Prefab Brush/Open Prefab Brush", false, 1)]
        public static void ShowWindow()
        {
            if (instance != null)
            {
                instance.Focus();
                return;
            }

            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            PrefabBrush wnd = GetWindow<PrefabBrush>(new Type[]{ inspectorType });

            wnd.titleContent = new GUIContent("Prefab Brush");
        }

        private void OnDisable()
        {
#if HARPIA_DEBUG
            Debug.Log($"{DebugLogStart} Prefab Brush Disabled");
#endif
            PrefabBrushTool.isUsingTool = false;
            PB_PhysicsSimulator.Dispose();
            ExitTool();
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                PB_ModularShortcuts.UpdateAssignKey();
                UseCurrentEventPB();
            }
        }

        private void OnDestroy()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] On Destroy called ");
#endif

            PB_ShortcutManager.Dispose();
            instance = null;

            SceneView.duringSceneGui -= OnSceneGUI;

            Undo.undoRedoPerformed -= OnUndoRedo;

            ToolManager.activeContextChanged -= OnActiveContextChanged;
            ToolManager.activeToolChanged -= OnActiveToolChanged;
            ToolManager.activeContextChanging -= OnActiveContextChanging;
            ToolManager.activeToolChanging -= OnActiveToolChanging;

            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
            PrefabStage.prefabStageDirtied -= OnPrefabStageDirtied;

            EditorSceneManager.sceneClosing -= OnSceneClosing;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneSaved -= OnSceneSaved;

            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

#if UNITY_2022_2_OR_NEWER
            EditorApplication.focusChanged -= OnEditorFocusChanged;
#endif

            if (_serializedObject != null) _serializedObject.Dispose();
            if (_temporaryTemplate != null) Object.DestroyImmediate(_temporaryTemplate);

            EditorApplication.update -= OnEditorUpdate;
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnUndoRedo()
        {
            //Get the undo message
            string message = Undo.GetCurrentGroupName();
            PrefabBrushObject.Dispose();
            UpdatePrefabList();

            if (!PB_UndoManager.IsUndoMessage(message)) return;

#if HARPIA_DEBUG
            Debug.Log($"[Preab Brush] Undo/Redo {message}");
#endif
        }

        public void CreateGUI()
        {
            if (Application.isPlaying)
            {
                notAvailableInPlayModeLabel = new Label("\nPrefab Brush is not available in play mode\nPlease close and open it again..."){ style ={ fontSize = 20, unityTextAlign = TextAnchor.MiddleCenter } };
                rootVisualElement.Add(notAvailableInPlayModeLabel);

                //on state changed
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

                return;
            }

            //Temporary Template

            CreateSerializedObject();

            instance = this;
            if (notAvailableInPlayModeLabel != null) notAvailableInPlayModeLabel.RemoveFromHierarchy();
            VisualTreeAsset tree = LoadVisualTreeAsset(xmlFileName, ref _visualTreeGuid);
            tree.CloneTree(rootVisualElement);
            VisualElement root = rootVisualElement;

            //Foldouts
            PrefabBrushTool.InitAllFoldouts(root);
            root.Find<Foldout>("erase-mode", PrefabBrushTemplate.FoldoutEraseMode);
            root.Find<Foldout>("template-section", PrefabBrushTemplate.FoldoutTemplateSection);
            Foldout mainSettingsSection = root.Find<Foldout>("main-settings-foldout", PrefabBrushTemplate.FoldoutMainSettings);
            root.Find<Foldout>("single-section", PrefabBrushTemplate.FoldoutPrecisionSection);
            Foldout parentSection = root.Find<Foldout>("parent-section", PrefabBrushTemplate.FoldoutParenting);
            Foldout brushSection = root.Find<Foldout>("brush-section", PrefabBrushTemplate.FoldoutBrushMultipleMode);
            Foldout placemenSection = root.Find<Foldout>("placement-section", PrefabBrushTemplate.FoldoutPlacement);
            Foldout transformSection = root.Find<Foldout>("transform-section", PrefabBrushTemplate.FoldoutTransforms);
            root.Find<Foldout>("physics-secttion", PrefabBrushTemplate.FoldoutPhysics);
            Foldout advancedSection = root.Find<Foldout>("advanced-options-section", PrefabBrushTemplate.FoldoutAdvancedSettings);

            //Top buttons
            paintModePanel = root.Q<VisualElement>("paint-mode");
            _eraserModeButton = root.Q<Button>("erase-mode-button");
            _paintModeButton = root.Q<Button>("paint-mode-button");
            root.Q<Button>("documentation-button").RegisterCallback<ClickEvent>(_ => OpenDocumentation());
            spacer = root.Q<VisualElement>("spacer");

            _paintModeButton.RegisterCallback<ClickEvent>(OnPaintModeButtonClick);
            _eraserModeButton.RegisterCallback<ClickEvent>(OnEraseModeButtonClick);
            root.Q<Button>("settings-button").RegisterCallback<ClickEvent>(OnSettingsModeButtonClick);

            //Custom props
            CustomPrefabProps.Init(root);

            //Main Panels
            root.Q<VisualElement>("paint-mode");
            root.Q<VisualElement>("settings-mode");

            //Prefabs selected section
            _addSelectedPrefabsPanel = root.Q("add-selected-objects");
            _addSelectedPrefabsButton = _addSelectedPrefabsPanel.Q<Button>("add-selected-objects-button");
            _addSelectedPrefabsLabel = _addSelectedPrefabsPanel.Q<Label>("add-selected-objects-label");
            _addSelectedPrefabsPanel.SetActive(false);
            _addSelectedPrefabsButton.RegisterCallback<ClickEvent>(OnAddSelectedPrefabsButton);

            //Section
            placementSection = root.Q<VisualElement>("placement-section");

            //status
            _statusBackground = root.Q<VisualElement>("status-section");
            _statusLabel = root.Q<Label>("status");
            _statusButton = root.Q<Button>("start-brush");
            _statusButton.RegisterCallback<ClickEvent>(OnStartButton);

            Button revealTemplateButton = root.Q<Button>("reveal-template");
            revealTemplateButton.RegisterCallback<ClickEvent>(OnRevealTemplateButton);

            Button saveTemplateButton = root.Q<Button>("save-template");
            saveTemplateButton.RegisterCallback<ClickEvent>(OnSaveTemplateButton);

            //Prefabs
            prefabsUiHolder = root.Q<VisualElement>("prefabs-holder");
            prefabsUiHolder.Clear();

            Button clearListButton = root.Q<Button>("clear-list");
            clearListButton.RegisterCallback<ClickEvent>(OnClearListButton);

            //Parent
            parentModeDropdown = parentSection.Q<DefaultEnumField>("parenting-mode");
            parentModeDropdown.Init(ParentMode.No_Parent);
            parentModeDropdown.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.ParentMode));
            parentModeDropdown.RegisterValueChangedCallback(OnParentModeChanged);
            _requiredParentWarningLabel = parentSection.Q<Label>("required-parent-label");

            _toggleUniformScaleParent = parentSection.Find<Toggle>("uniform-scaled-parent-only", PrefabBrushTemplate.UniformScaleParent);

            parentField = parentSection.Find<ObjectField>("parent", PrefabBrushTemplate.ParentObject);
            parentField.objectType = typeof(Transform);
            parentField.RegisterValueChangedCallback(OnParentChanged);

            parentNameInput = parentSection.Find<TextField>("parent-name", PrefabBrushTemplate.ParentName);
            createParentButton = parentSection.Q<Button>("create-parent");
            createParentButton.RegisterCallback<ClickEvent>(OnCreateParentButton);

            //Shortcuts
            _shortcutParent = root.Q<Label>("shortcuts-label").parent;

            //Mode
            paintModeDropdown = root.Q<DefaultEnumField>("mode-dropdown");
            paintModeDropdown.Init(lastMode);
            paintModeDropdown.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.PaintMode));
            paintModeDropdown.RegisterValueChangedCallback(OnModeChanged);

            //Texture Mask
            PB_TerrainHandler.ShowAlert();

            //Precision Mode
            _precisionModeFoldout = root.Q<Foldout>("single-section");
            precisionModeChangePrefabToggle = _precisionModeFoldout.Find<Toggle>("single-mode-change-prefab", PrefabBrushTemplate.PrecisionModeChangePrefabAfterPlacing);
            precisionModeAddMeshToBatch = _precisionModeFoldout.Find<Toggle>("single-mode-add-mesh", PrefabBrushTemplate.PrecisionModeAddMesh);

            precisionModeRotationAngle = _precisionModeFoldout.Find<FloatField>("shortcut-rotation-angle", PrefabBrushTemplate.PrecisionModeRotationAngle);
            precisionModeRotationAngle.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.PrecisionModeRotationAngle));

            precisionModeRotationAngle.RegisterValueChangedCallback(OnPrecisionModeRotationAngleChanged);

            PB_FakePlaneManger.Init(root);

            PB_PhysicsSimulator.Init(root);

            //Drag and drop
            _dragAndDropLabel = root.Q<Label>("drag-drop-text");
            _dragAndDropOriginalText = _dragAndDropLabel.text;
            VisualElement dragAndDropSection = root.Q<VisualElement>("drag-drop-section");
            dragAndDropSection.RegisterCallback<DragEnterEvent>(OnDragAndDropPrefabsEnter);
            dragAndDropSection.RegisterCallback<DragExitedEvent>(OnDragPrefabsExit);
            dragAndDropSection.RegisterCallback<DragLeaveEvent>(OnDragPrefabsLeave);

            //Brush
            _brushFoldout = root.Q<Foldout>("brush-section");

            brushRadiusSlider = _brushFoldout.Find<Slider>("size", PrefabBrushTemplate.BrushSize);
            brushRadiusSlider.RegisterFocusEvents_PB();
            PB_EditorInputSliderMinMax.LoadSliderMinMaxValues(brushRadiusSlider, "pb-radius");
            brushRadiusSlider.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Set Range Values", _ => PB_EditorInputSliderMinMax.Show("Update Slider Range", brushRadiusSlider, "pb-radius"), DropdownMenuAction.AlwaysEnabled); }));

            sliderBrushStrength = _brushFoldout.Find<Slider>("strength", PrefabBrushTemplate.BrushStrength);
            sliderBrushStrength.lowValue = 0.01f;
            sliderBrushStrength.highValue = 1f;
            //brushRadiusSlider.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.BrushRadius));

            //Eraser
            PB_EraserManager.Init(root);

            //Snapping
            PB_SnappingManager.Init(placemenSection, _serializedObject);

            //Raycast mode
            raycastModeDropdown = mainSettingsSection.Q<DefaultEnumField>("raycast-mode");
            raycastModeDropdown.Init(RaycastMode.Mesh);
            raycastModeDropdown.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.RaycastMode));
            raycastModeDropdown.RegisterValueChangedCallback(OnRaycastModeChanged);

            //Pivot mode
            pivotMode = root.Q<DefaultEnumField>("pivot-mode");
            pivotMode.Init(PivotMode.MeshPivot);
            pivotMode.BindProperty(_serializedObject.FindProperty(PrefabBrushTemplate.PivotModeValue));
            pivotMode.RegisterValueChangedCallback(OnPivotModeChanged);

            offsetField = new Vector3ModeElement(transformSection, "offset-type", "offset-fixed", "offset-max", "offset-min", "", false, PrefabBrushTemplate.OffsetMode, PrefabBrushTemplate.FixedOffset, PrefabBrushTemplate.MaxOffset, PrefabBrushTemplate.MinOffset, null, null, null);
            offsetField.RegisterFocusEvents();
            offsetField.enumField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentOffset());
            offsetField.fixedField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentOffset());
            offsetField.maxField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentOffset());
            offsetField.minField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentOffset());

            toggleAlignToHit = transformSection.Find<Toggle>("align-with-ground", PrefabBrushTemplate.AlignWithGround);
            toggleAlignToHit.RegisterValueChangedCallback(OnAlignToHitChanged);
            _alignToHitAxisX = transformSection.Find<Toggle>("align-to-hit-axis-x", PrefabBrushTemplate.AlignWithHitAxisX);
            _alignToHitAxisY = transformSection.Find<Toggle>("align-to-hit-axis-y", PrefabBrushTemplate.AlignWithHitAxisY);
            _alignToHitAxisZ = transformSection.Find<Toggle>("align-to-hit-axis-z", PrefabBrushTemplate.AlignWithHitAxisZ);
            OnAlignToHitChanged(null);

            //Layer
            layerMaskField = placemenSection.Find<LayerMaskField>("layer", PrefabBrushTemplate.LayerMask);
            layerMaskField.RegisterValueChangedCallback(OnLayerMaskChanged);
            layerMaskField.SetValueWithoutNotify(-1);

            root.Q<Button>("physics-debbuger-btn").RegisterCallback<ClickEvent>(_ => PB_PhysicsSimulator.ShowDebugWindow());

            //Tag
            tagMaskField = placemenSection.Find<MaskField>("tags", PrefabBrushTemplate.TagMask);
            tagMaskField.RegisterCallback<ChangeEvent<int>>(OnTagMaskChanged);
            tagMaskField.SetValueWithoutNotify(-1);

            string[] tags = InternalEditorUtility.tags;
            tagMaskField.choices = tags.ToList();

            useAngleLimitsToggle = placemenSection.Find<Toggle>("use-angle-limits", PrefabBrushTemplate.UseAngleLimits);
            useAngleLimitsToggle.RegisterValueChangedCallback(OnUseAngleLimitsChanged);

            angleLimitsField = placemenSection.Find<MinMaxSlider>("slope-limits", PrefabBrushTemplate.AngleLimits);
            angleLimitsField.RegisterValueChangedCallback(OnAngleLimitsChanged);

            angleMaxField = placemenSection.Q<FloatField>("max-slope");
            angleMaxField.value = angleLimitsField.highLimit;
            angleMaxField.RegisterValueChangedCallback(OnAngleMaxChanged);

            angleMinField = placemenSection.Q<FloatField>("min-slope");
            angleMinField.value = angleLimitsField.lowLimit;
            angleMinField.RegisterValueChangedCallback(OnAngleMinChanged);

            //Rotation
            rotationField = new RotationTypeElement(transformSection, "rotation-type", "rotation-fixed", "rotation-max", "rotation-min", null, PrefabBrushTemplate.RotationMode, PrefabBrushTemplate.FixedRotation, PrefabBrushTemplate.MaxRotation, PrefabBrushTemplate.MinRotation, null);
            rotationField.fixedField.RegisterFocusEvents_PB();

            rotationField.enumField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentRotation());
            rotationField.fixedField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentRotation());
            rotationField.maxField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentRotation());
            rotationField.minField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentRotation());

            //scale
            scaleField = new Vector3ModeElement(transformSection, "scale-type", "scale-fixed", "scale-max", "scale-min", "", true, PrefabBrushTemplate.ScaleMode, PrefabBrushTemplate.FixedScale, PrefabBrushTemplate.MaxScale, PrefabBrushTemplate.MinScale, null, PrefabBrushTemplate.MaxScaleUniform, PrefabBrushTemplate.MinScaleUniform);
            scaleField.RegisterFocusEvents();
            scaleField.AddProportions("scale-proportions-toggle-fixed", "scale-proportions-toggle-max", "scale-proportions-toggle-min", root);

            scaleField.fixedField.value = Vector3.one;
            scaleField.minField.value = Vector3.one * 0.8f;
            scaleField.maxField.value = Vector3.one * 1.2f;

            scaleField.enumField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentScale());
            scaleField.fixedField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentScale());
            scaleField.maxField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentScale());
            scaleField.minField.RegisterValueChangedCallback((_) => PB_PrecisionModeManager.UpdateCurrentScale());

            //Advanced options
            makeObjectsStaticToggle = advancedSection.Find<Toggle>("objects-static-toggle", PrefabBrushTemplate.MakeStatic);
            makeErasableToggle = advancedSection.Find<Toggle>("objects-erasable-toggle", PrefabBrushTemplate.MakeErasable);
            addToClippingCheckToggle = advancedSection.Find<Toggle>("objects-clipping-test-toggle", PrefabBrushTemplate.AddToClippingTest);

            _showWorldLines = advancedSection.Find<Toggle>("show-brush-world-lines", PrefabBrushTemplate.ShowWorldLines);
            _showWorldLines.RegisterValueChangedCallback(OnShowBrushGuideLinesChanged);
            _showWorldLines.RegisterFocusEvents_PB();

            worldLinesDistance = advancedSection.Find<FloatField>("guide-lines-distance", PrefabBrushTemplate.WorldLinesDistance);
            worldLinesDistance.RegisterFocusEvents_PB();

            showBoundsToggle = advancedSection.Find<Toggle>("show-bounds", PrefabBrushTemplate.ShowClippingBounds);
            showBoundsToggle.RegisterValueChangedCallback(_ => UpdateUITool());

            //Clipping
            clippingToleranceSlider = brushSection.Find<Slider>("clipping-strength", PrefabBrushTemplate.ClippingStrenght);
            clippingToleranceSlider.lowValue = 0.01f;
            clippingToleranceSlider.highValue = 1f;
            clippingToleranceSlider.RegisterFocusEvents_PB();
            clippingToleranceSlider.RegisterValueChangedCallback(OnClippingToleranceChanged);

            //Template
            _templatesDropdown = root.Find<DropdownField>("select-template", PrefabBrushTemplate.Template);
            _templatesDropdown.RegisterValueChangedCallback(OnTemplateChanged);
            UpdateTemplatesDropdown();

            //Shortcuts
            PB_ModularShortcuts.Init(root);

            //Events
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            ToolManager.activeContextChanged += OnActiveContextChanged;
            ToolManager.activeToolChanged += OnActiveToolChanged;
            ToolManager.activeContextChanging += OnActiveContextChanging;
            ToolManager.activeToolChanging += OnActiveToolChanging;

            Selection.selectionChanged += OnSelectionChanged;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageDirtied += OnPrefabStageDirtied;

            EditorApplication.update += OnEditorUpdate;

            //scene change event
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaved += OnSceneSaved;

            //Editor lost focus
#if UNITY_2022_2_OR_NEWER
            EditorApplication.focusChanged += OnEditorFocusChanged;
#endif

            //Playmode
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            //Compilation
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            //Hidden element
            _hiddenField = new FloatField();
            _hiddenField.style.display = DisplayStyle.None;
            root.Add(_hiddenField);

            PB_TextureMaskHandler.Init(root);
            advancedSettings = new PB_AdvancedSettings(root);
            PB_PressurePen.Init(root);
            PB_ThumbnailGenerator.Init();
            PB_AttractorManager.Init(root);
            UpdateUITool();
            OnAlignToHitChanged(null);
            UpdatePrefabList();

            Selection.objects = Array.Empty<Object>();
            PrefabBrushTool.isUsingTool = false;
        }

        private void OnAlignToHitChanged(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] On Align To Hit Changed {(evt != null ? evt.newValue : toggleAlignToHit.value)}");
#endif
            bool val = toggleAlignToHit.value;
            _alignToHitAxisX.parent.parent.SetActive(val);
        }

        private void CreateSerializedObject()
        {
            if (_serializedObject != null) return;
            if (_temporaryTemplate == null)
            {
                _temporaryTemplate = CreateInstance<PrefabBrushTemplate>();
                _temporaryTemplate.name = "Prefab Brush - Temporary Template";
            }

            _serializedObject = new SerializedObject(_temporaryTemplate);
        }

        private void UpdateTemplatesDropdown()
        {
            _templatesDropdown ??= rootVisualElement.Q<DropdownField>("select-template", PrefabBrushTemplate.Template);
            if (_templatesDropdown == null) return;
            _templatesDropdown.choices = PrefabBrushTemplate.GetAllPresetsNames();
            _templatesDropdown.value = (PrefabBrushTemplate.GetCurrentTemplateName());
        }

        private void OnEditorFocusChanged(bool obj)
        {
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brsuh] OnEditorFocusChanged {obj}");
#endif

            DisposeKeysVariables();
            UpdatePrefabList();
            UpdatePrefabList();

            if (!obj)
            {
                PB_PhysicsSimulator.Dispose();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
#if HARPIA_DEBUG
            Debug.Log($"OnPlayModeStateChanged {obj} - Has instance {instance != null}", this);
#endif

            if (obj == PlayModeStateChange.EnteredEditMode && instance != null)
            {
                //Bug with serializtion object
                //CreateGUI();
                return;
            }

            ResetStaticVariables();
            ExitTool();
        }

        private void OnEditorUpdate()
        {
            PB_PhysicsSimulator.Update(deltaTime);
        }

        private void OnFocus()
        {
            instance = this;
            CreateSerializedObject();
            if (Application.isPlaying) return;

#if HARPIA_DEBUG
            Debug.Log($"{DebugLogStart} On Focus", this);
#endif

            PrefabBrushObject.Dispose(PB_PhysicsSimulator.IsUsingPhysics());
            PB_PrecisionModeManager.HideObject();
            UpdateTemplatesDropdown();
            _serializedObject.UpdateIfRequiredOrScript();
            UpdatePrefabList();
            UpdateShortcuts();
        }

        private void UpdatePrefabList()
        {
            //Time Check
            double time = EditorApplication.timeSinceStartup;
            double diff = time - _lastUpdateListTime;
            if (diff < .5f) return;

            //Null Check
            if (prefabsUiHolder == null) return;

            prefabsUiHolder.Clear();

            //Check if the template is null
            if (_temporaryTemplate.prefabs == null) return;

            //Instantiate All UI
            foreach (PbPrefabUI prefab in _temporaryTemplate.prefabs)
            {
                prefab.InstantiateUI();
            }

            _lastUpdateListTime = time;
        }

        private void OnLostFocus()
        {
            if (Application.isPlaying) return;
#if HARPIA_DEBUG
            Debug.Log($"{DebugLogStart} OnLostFocus -", this);
#endif

            ResetStaticVariables();

            if (advancedSettings != null)
                advancedSettings.SaveValues();

            PB_ModularShortcuts.Dispose();

            //Using tool
            if (!PrefabBrushTool.isUsingTool) PB_ShortcutManager.Dispose();

            UpdatePrefabList();
            Repaint();
        }

        private void OnAddSelectedPrefabsButton(ClickEvent evt)
        {
            if (_prefabsToAddFromSelection == null) return;
            if (_prefabsToAddFromSelection.Count == 0) return;

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] OnAddSelectedPrefabsButton {_prefabsToAddFromSelection.Count}", this);
#endif

            foreach (GameObject go in _prefabsToAddFromSelection)
            {
                AddPrefab(go);
            }
        }

        #endregion

        #region CompilationEvents

        private void OnAfterAssemblyReload()
        {
#if HARPIA_DEBUG
            Debug.Log($"On After Assembly Reload", this);
#endif

            PB_PrecisionModeManager.DisposePrecisionMode();
            PB_TextureMaskHandler.Init(rootVisualElement);
        }

        private void OnBeforeAssemblyReload()
        {
#if HARPIA_DEBUG
            Debug.Log($"On Before Assembly Reload", this);
#endif

            PB_PrecisionModeManager.DisposePrecisionMode();
            PB_PhysicsSimulator.Dispose();
        }

        #endregion

        #region ModeButtonsCallbacks

        private void OnPivotModeChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] On Pivot Mode changed ");
#endif
            PB_AttractorManager.UpdateUI();
        }

        private void OnSettingsModeButtonClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"On Settings Mode Button Click", this);
#endif
            PrefabBrush.instance.advancedSettings.SetActive(true);
            ExitEraserMode();
            paintModePanel.SetActive(false);
            PB_EraserManager.ToggleUI(false);
            PB_ShortcutManager.ClearShortcuts();
        }

        private void OnPaintModeButtonClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"On Paint Mode Button Click - Last Mode {lastMode}", this);
#endif
            Selection.activeObject = null;
            ExitEraserMode();

            advancedSettings.SetActive(false);
            paintModePanel.SetActive(true);
            paintModeDropdown.SetValueWithoutNotify(lastMode);

            if (GetPaintMode() == PaintMode.Precision)
            {
                PB_PrecisionModeManager.SetPrefabToPaint();
            }

            OnModeChanged(null);

            UpdateUITool();
        }

        private void OnEraseModeButtonClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] On Erase Mode Button Click", this);
#endif

            EnterEraserMode();
        }

        #endregion

        #region SceneEvents

        private void OnSceneSaved(Scene scene)
        {
#if HARPIA_DEBUG
            Debug.Log($"OnSceneSaved - scene {scene.name}", this);
#endif
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] OnSceneOpened - mode {mode}", this);
#endif
            ExitTool();
        }

        private void OnSceneClosing(Scene scene, bool removingScene)
        {
#if HARPIA_DEBUG
            Debug.Log($"On Scene Closing - removingScene {removingScene}", this);
#endif
        }

        #endregion

        #region PrefabStageEvents

        private void OnPrefabStageDirtied(PrefabStage obj)
        {
            ExitTool();
        }

        private void OnPrefabStageOpened(PrefabStage obj)
        {
            ExitTool();
        }

        private void OnPrefabStageClosing(PrefabStage obj)
        {
            UpdateUITool();
        }

        #endregion

        private void OnPrecisionModeRotationAngleChanged(ChangeEvent<float> evt)
        {
            float v = Mathf.Clamp(evt.newValue, 0.01f, 180);
            precisionModeRotationAngle.SetValueWithoutNotify(v);
        }

        private void OnShowBrushGuideLinesChanged(ChangeEvent<bool> evt)
        {
            UpdateUITool();
        }

        private void OnUseAngleLimitsChanged(ChangeEvent<bool> evt)
        {
            UpdateUITool();
        }

        private void OnClippingToleranceChanged(ChangeEvent<float> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"OnClippingToleranceChanged", this);
#endif
            PrefabBrushObject.SetClippingSize(1 - clippingToleranceSlider.value, true);
        }

        private void OnParentModeChanged(ChangeEvent<Enum> evt)
        {
            ParentMode newValue = (ParentMode)evt.newValue;
            RaycastMode raycastMode = GetRaycastMode();
            if (newValue == ParentMode.Hit_Surface_Object && raycastMode == RaycastMode.Mesh)
            {
                ParentMode oldValue = (ParentMode)evt.previousValue;
                DisplayError($"Parenting to hit surface object is not supported when using Mesh Raycast Mode Yet." + $"Please use the physical collider raycast mode in order to parent objects to hit surface" + $"\n\nChanging back to {oldValue}");
                parentModeDropdown.SetValueWithoutNotify(oldValue);
            }

            UpdateUITool();
        }

        private void OnModeChanged(ChangeEvent<Enum> evt)
        {
            PaintMode newMode = evt == null ? GetPaintMode() : (PaintMode)evt.newValue;
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brush] Mode Changed to {newMode}");
#endif

            PB_ModularShortcuts.Dispose();
            PB_PrecisionModeManager.DisposePrecisionMode();
            PrefabBrushObject.Dispose();
            PB_AttractorManager.Dispose();
            //new mode
            if (newMode == PaintMode.Precision)
            {
                PbPrefabUI.SelectFirst();
                PB_PrecisionModeManager.SetPrefabToPaint();
            }

            if (newMode == PaintMode.Multiple)
            {
            }

            UpdateEraserModeUI();
            UpdateUITool();
            UpdateShortcuts();
        }

        private void OnAngleMaxChanged(ChangeEvent<float> evt)
        {
            Vector2 limits = angleLimitsField.value;
            limits.y = evt.newValue;
            _temporaryTemplate.angleLimits = limits;
        }

        private void OnAngleMinChanged(ChangeEvent<float> evt)
        {
            Vector2 limits = angleLimitsField.value;
            limits.x = evt.newValue;
            _temporaryTemplate.angleLimits = limits;
        }

        private void OnAngleLimitsChanged(ChangeEvent<Vector2> evt)
        {
            angleMinField.SetValueWithoutNotify(evt.newValue.x);
            angleMaxField.SetValueWithoutNotify(evt.newValue.y);
        }

        private static bool CanUseTool()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isOnPrefabStage = prefabStage != null;
            return !isOnPrefabStage || PrefabBrushTool.isUsingTool;
        }

        //Start using tool
        public void OnStartButton(ClickEvent evt)
        {
            if (CanUseTool() == false) return;

#if HARPIA_DEBUG
            Debug.Log($"{DebugLogStart} Start button clicked - isUsingTool {PrefabBrushTool.isUsingTool} {GetPaintMode()}");
#endif

            DisposeKeysVariables();

            PrefabBrushTool.isUsingTool = !PrefabBrushTool.isUsingTool;

            if (PrefabBrushTool.isUsingTool)
            {
                if (Application.isPlaying)
                {
                    DisplayError("Cannot use Prefab Brush Tool while in Play Mode.");
                    PrefabBrushTool.isUsingTool = false;
                    return;
                }

                PrefabBrushTool.EnableAutoRefresh();
                PB_ShortcutManager.ChangeProfile();
                PB_PhysicsSimulator.TryToStartPhysics();

                Selection.objects = Array.Empty<Object>();
                _addSelectedPrefabsPanel.SetActive(false);
                PaintMode mode = GetPaintMode();

                if (mode == PaintMode.Eraser)
                {
                    UpdateUITool();
                    return;
                }

                if (currentPrefabs.Count == 0)
                {
                    PrefabBrushTool.isUsingTool = false;
                    DisplayError("No Prefabs Added. Please add at least one prefab to the list.");
                    return;
                }

                if (mode == PaintMode.Precision && PbPrefabUI.HasAnySelected() == false)
                {
                    PbPrefabUI.SelectFirst();
                }

                if (mode == PaintMode.Multiple && !PbPrefabUI.HasAnySelected())
                {
                    PbPrefabUI.SelectAll();
                }
            }
            else
            {
                //Exit tool
                PB_MeshBatcher.Dispose(true);
                PB_PrecisionModeManager.DisposePrecisionMode();
                PrefabBrushObject.Dispose();
                PB_PhysicsSimulator.Dispose();
                PB_ShortcutManager.Dispose();
                PB_AttractorManager.Dispose();
            }

            UpdateUITool();
        }

        private static void OnRevealTemplateButton(ClickEvent evt)
        {
            //log
#if HARPIA_DEBUG
            Debug.Log("Reveal template button clicked");
#endif
            PrefabBrushTemplate.RevealCurrentTemplate();
        }

        private void OnSaveTemplateButton(ClickEvent evt)
        {
            //log
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] Save template button clicked");
#endif

            if (_temporaryTemplate == null)
            {
                _temporaryTemplate = CreateInstance<PrefabBrushTemplate>();

                if (_temporaryTemplate == null)
                {
                    DisplayError("Could not save template. Temporary Template is null");
                    return;
                }
            }

            bool result = _temporaryTemplate.SaveScriptableObject();

            if (!result) return;

            string currentName = PrefabBrushTemplate.GetCurrentTemplateName();
            DisplayStatus($"Template [{currentName}] saved");

            //Update dropdown
            _templatesDropdown.choices = PrefabBrushTemplate.GetAllPresetsNames();
            _templatesDropdown.SetValueWithoutNotify(currentName);
        }

        private static void DisplayStatus(string msg)
        {
            EditorUtility.DisplayDialog("Prefab Brush", msg, "Ok");
        }

        private void OnTemplateChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == PrefabBrushTemplate.NoPresetsFound) return;

#if HARPIA_DEBUG
            Debug.Log($"{DebugLogStart} Template changed to " + evt.newValue);
#endif

            _lastLoadedTemplate = PrefabBrushTemplate.LoadTemplate(evt.newValue);
            if (_lastLoadedTemplate == null) return;

            if (_lastLoadedTemplate == null)
            {
                if (evt.newValue != "None") DisplayError($"Could not load template {evt.newValue}");
                return;
            }

            LoadTemplate(_lastLoadedTemplate);
        }

        #region DragAndDrop

        private void OnDragPrefabsLeave(DragLeaveEvent evt)
        {
            //log
#if HARPIA_DEBUG
            Debug.Log("Drag leave");
#endif

            _dragAndDropLabel.text = _dragAndDropOriginalText;
        }

        private void OnDragPrefabsExit(DragExitedEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] Drag exited - Trying to add prefabs - qtd {DragAndDrop.objectReferences.Length}");
#endif
            List<GameObject> toAdd = new List<GameObject>();
            foreach (Object o in DragAndDrop.objectReferences)
            {
                //Check If it is a GO
                GameObject originalGO = o as GameObject;
                if (originalGO == null)
                {
                    string path = AssetDatabase.GetAssetPath(o);
                    Debug.LogError($"{DebugLogStart} Not a game object at {path}");
                    continue;
                }

                //Find The Prefab Asset
                GameObject assetGo = PrefabBrushTool.GetPrefabAsset(originalGO);

                //If it is a scene object
                if (assetGo == null)
                {
#if HARPIA_DEBUG
                    Debug.LogError($"Could not get prefab asset for {originalGO.gameObject.name}", originalGO);
#endif
                    toAdd.Add(originalGO);
                    continue;
                }

                //It is a prefab asset
                toAdd.Add(assetGo);
            }

            AddPrefab(toAdd.ToArray());

            _dragAndDropLabel.text = _dragAndDropOriginalText;
        }

        private void OnDragAndDropPrefabsEnter(DragEnterEvent dragEnterEvent)
        {
            //log
#if HARPIA_DEBUG
            Debug.Log($"OnDragPerformedEvent {DragAndDrop.objectReferences.Length} {Event.current.type}");
#endif

            if (DragAndDrop.objectReferences.Length == 0) return;

            _dragAndDropLabel.text = $"Drop {DragAndDrop.objectReferences.Length} files here";
        }

        #endregion

        private void AddPrefab(PbPrefabUI objectList)
        {
            if (objectList == null) return;
            if (currentPrefabs.Contains(objectList)) return;
            currentPrefabs.Add(objectList);
            objectList.InstantiateUI();
        }

        private void AddPrefab(params Object[] objectList)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] Adding Prefabs to List {objectList.Length}");
#endif

            //Select the prefabs to add
            List<GameObject> toAdd = new List<GameObject>();
            foreach (Object o in objectList)
            {
                //Null checks
                if (o == null) continue;

                //GO Check
                GameObject gameObject = o as GameObject;
                if (gameObject == null) continue;

                //Add to the list
                toAdd.Add(gameObject);
            }

            //Save the object
            Undo.RecordObject(_temporaryTemplate, "Prefab Brush - Add  Prefab");

            //Add The Prefabs
            foreach (GameObject o in toAdd)
            {
                PbPrefabUI newObj = new PbPrefabUI(o);
                _temporaryTemplate.prefabs.Add(newObj);
            }

            //Update The Object
            _serializedObject.Update();
        }

        #region MenuItem_TopMenu

        [MenuItem("Tools/Prefab Brush/Documentation", false, 2)]
        private static void OpenDocumentation()
        {
            Application.OpenURL("https://harpiagames.gitbook.io/prefab-brush-documentation/");
        }

        [MenuItem("Tools/Prefab Brush/Rate This asset", false, 12)]
        private static void OpenRatePage()
        {
            Application.OpenURL("https://u3d.as/37hF");
        }

        [MenuItem("Tools/Prefab Brush/Discord", false, 11)]
        private static void OpenOpenDiscord()
        {
            Application.OpenURL("https://discord.gg/Tr952uhsqb");
        }

        [MenuItem("Tools/Prefab Brush/More Tools/Low Poly Color Changer", false, 12)]
        private static void ToolsLowPoly()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/level-design/low-poly-color-changer-easy-color-changing-variations-248562?aid=1100lACye");
        }

        [MenuItem("Tools/Prefab Brush/More Tools/Prefab Icons - Icon Creator", false, 12)]
        private static void ToolsIconCreator()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/game-toolkits/icon-creator-generate-fast-easy-complete-icons-generator-198488?aid=1100lACye&utm_campaign=unity_affiliate&utm_medium=affiliate&utm_source=partnerize-linkmaker");
        }

        #endregion

        #region MenuItem_Assets

        [MenuItem("GameObject/Prefab Brush/Add Prefabs", false)]
        [MenuItem("Assets/Prefab Brush/Add Prefabs", false)]
        private static void AddPrefabs()
        {
            if (instance == null) ShowWindow();

            GameObject[] selectedGameObjects = Selection.gameObjects;
            instance.AddPrefab(selectedGameObjects);
        }

        [MenuItem("GameObject/Prefab Brush/Use These Prefabs", false)]
        [MenuItem("Assets/Prefab Brush/Use These Prefabs", false)]
        private static void UseThesePrefabs()
        {
            List<GameObject> selectedGameObjects = PrefabBrushTool.GetPrefabs(Selection.objects);

            if (instance == null)
            {
                ShowWindow();
            }

            instance.OnClearListButton(null);
            instance.AddPrefab(selectedGameObjects.ToArray());
            instance.parentField.value = null;
        }

        [MenuItem("Assets/Prefab Brush/Add Folder Prefabs", false)]
        private static void AddFolderPrefabs()
        {
            string selectedFolder = PB_FolderUtils.GetSelectedPathOrFallback();
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] AddFolderPrefabs - selectedFolder: {selectedFolder}");
#endif
            if (instance == null) ShowWindow();
            List<GameObject> prefabs = PB_FolderUtils.GetPrefabs(selectedFolder);
            instance.AddPrefab(prefabs.ToArray());
        }

        [MenuItem("Assets/Prefab Brush/Add Folder Prefabs", true)]
        private static bool AddFolderPrefabsValidate()
        {
            string selectedFolder = PB_FolderUtils.GetSelectedPathOrFallback();
            bool hasAnyPrefab = PB_FolderUtils.HasAnyPrefab(selectedFolder);
            return !string.IsNullOrEmpty(selectedFolder) && hasAnyPrefab;
        }

        [MenuItem("GameObject/Prefab Brush/Use These Prefabs", true)]
        [MenuItem("Assets/Prefab Brush/Use These Prefabs", true)]
        private static bool ValidateUseThesePrefabs()
        {
            return PrefabBrushTool.HasAnyPrefab(Selection.objects);
        }

        [MenuItem("GameObject/Prefab Brush/Add Prefabs", true)]
        [MenuItem("Assets/Prefab Brush/Add Prefabs", true)]
        private static bool ValidateAddPrefabs()
        {
            return PrefabBrushTool.HasAnyPrefab(Selection.objects);
        }

        #endregion

        private void OnSelectionChanged()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] OnSelectionChanged");
#endif

            UpdatePrefabList();
            PB_PhysicsSimulator.Dispose(!PrefabBrushTool.isUsingTool);
            PB_PhysicsSimulator.UpdateRigidbodiesObjects();
            PB_MeshBatcher.Dispose();
            _addSelectedPrefabsPanel.SetActive(false);

            if (Selection.objects.Length > 0 && PB_UndoManager.IsLastUndoTimeGreaterThan(0.5))
            {
                ExitTool();
            }

            if (GetPaintMode() == PaintMode.Eraser) return;
            if (Selection.objects.Length == 0) return;
            if (!PrefabBrushTool.HasAnyPrefab(Selection.objects)) return;

            _prefabsToAddFromSelection = PrefabBrushTool.GetPrefabs(Selection.objects);
            if (_prefabsToAddFromSelection.Count == 0) return;

            _addSelectedPrefabsLabel.text = $"Add {_prefabsToAddFromSelection.Count} Selected Prefab(s) ";
            _addSelectedPrefabsPanel.SetActive(true);
        }

        private void OnActiveToolChanging()
        {
            //log the tool
#if HARPIA_DEBUG
            Debug.Log($"Active tool changing from {ToolManager.activeToolType}");
#endif
            ExitTool();
            UpdateUITool();
        }

        private void OnActiveContextChanging()
        {
            //log the tool
#if HARPIA_DEBUG
            Debug.Log($"Active context changing to {ToolManager.activeContextType}");
#endif
            UpdateUITool();
        }

        private void OnActiveToolChanged()
        {
            //log the tool
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brush] Active tool changed to {ToolManager.activeToolType}");
#endif

            bool isUsingPrefabTool = ToolManager.activeToolType.ToString().Equals(nameof(PrefabBrushTool));
            PrefabBrushTool.isUsingTool = isUsingPrefabTool;

            if (isUsingPrefabTool)
            {
                Selection.activeGameObject = null;
            }

            UpdateUITool();
        }

        private void UpdateUITool()
        {
            bool isUsingTool = PrefabBrushTool.isUsingTool;

            if (!CanUseTool())
            {
                _statusLabel.text = "Cannot Use The Tool On Prefab Mode";
                _statusBackground.style.backgroundColor = new Color(1f, 0f, 0f, 0.4f);
                _statusButton.text = "Start";
                _statusButton.SetActive(false);
            }

            else if (isUsingTool)
            {
                _statusLabel.text = "Brush is running";
                _statusBackground.style.backgroundColor = _styleBackgroundColorGreen;
                _statusButton.text = "Stop";
                _statusButton.SetActive(true);
            }
            else
            {
                _statusLabel.text = "Brush is not running";
                _statusBackground.style.backgroundColor = new Color(1f, 0f, 0f, 0.4f);
                _statusButton.text = "Start";
                _statusButton.SetActive(true);
            }

            bool isInMultipleMode = GetPaintMode() == PaintMode.Multiple;
            bool isInPrecisionMode = GetPaintMode() == PaintMode.Precision;
            bool isInEraserMode = GetPaintMode() == PaintMode.Eraser;

            PB_SnappingManager.UpdateUI();
            PB_FakePlaneManger.UpdateUI();
            PB_AttractorManager.UpdateUI();

            _brushFoldout.SetActive(isInMultipleMode);
            _precisionModeFoldout.SetActive(isInPrecisionMode);
            placementSection.SetActive(isInMultipleMode || isInPrecisionMode);

            ParentMode parentMode = GetParentMode();
            parentField.SetActive(parentMode == ParentMode.Fixed_Transform);
            parentNameInput.SetActive(parentMode == ParentMode.Fixed_Transform);
            createParentButton.SetActive(parentMode == ParentMode.Fixed_Transform);
            _requiredParentWarningLabel.SetActive(parentMode == ParentMode.Fixed_Transform && parentField.value == null);
            _toggleUniformScaleParent.SetActive(parentMode == ParentMode.Hit_Surface_Object);

            tagMaskField.SetActive(isInMultipleMode || isInPrecisionMode);
            precisionModeAddMeshToBatch.SetActive(GetRaycastMode() == RaycastMode.Mesh);

            bool angleLimits = useAngleLimitsToggle.value;
            angleLimitsField.parent.SetActive(angleLimits);
            angleMinField.parent.SetActive(angleLimits);

            bool showGuideLines = _showWorldLines.value;
            worldLinesDistance.SetActive(showGuideLines);

            PrefabBrushObject.SetClippingSize(clippingToleranceSlider, PB_EraserManager.eraserClippingRadiusSlider);
            PB_TextureMaskHandler.UpdateUITerrain();

            PB_EraserManager.ToggleUI(isInEraserMode);
            _eraserModeButton.SetBackgroundColor(isInEraserMode ? activeButtonColor : Color.clear);
            _paintModeButton.SetBackgroundColor(!isInEraserMode ? activeButtonColor : Color.clear);

            angleLimitsField.value = new Vector2(angleMinField.value, angleMaxField.value);

            scaleField.UpdateUIVec3Element();
            rotationField.UpdateUIVec3Element();
            offsetField.UpdateUIVec3Element();

            UpdateShortcuts();

            PB_PhysicsSimulator.UpdateUIPhysics();
        }

        private void UpdateShortcuts()
        {
            PB_ShortcutManager.ClearShortcuts();
            PB_ModularShortcuts.Dispose();

            if (_shortcutParent == null) return;

            switch (GetPaintMode())
            {
                case PaintMode.Eraser:
                    PB_EraserManager.AddShortcuts();
                    break;
                case PaintMode.Precision:
                    PB_PrecisionModeManager.AddShortcuts();
                    break;
                default:
                    PB_MultipleModeManager.AddShortcuts();
                    break;
            }

            PB_ShortcutManager.ApplyTo(_shortcutParent);
        }

        public PaintMode GetPaintMode()
        {
            if (paintModeDropdown == null) return PaintMode.Multiple;
            return (PaintMode)paintModeDropdown.value;
        }

        public ParentMode GetParentMode()
        {
            return (ParentMode)parentModeDropdown.value;
        }

        public PivotMode GetPivotMode()
        {
            return (PivotMode)pivotMode.value;
        }

        #region OnSceneGUI

        private void OnSceneGUI(SceneView obj)
        {
            if (!PrefabBrushTool.isUsingTool) return;

            sceneCamera = obj.camera;

            //Scene view check
            bool isMouseOverWindow = PrefabBrushTool.IsMouseOverAnySceneView();
            if (_lastIsMouseOverWindow != isMouseOverWindow)
            {
                if (isMouseOverWindow)
                {
#if HARPIA_DEBUG
                    //Entered the scene view
                    Debug.Log($"[Prefab Brush] Entered Scene View", this);
#endif
                }
                else
                {
#if HARPIA_DEBUG
                    //Exited the scene view
                    Debug.Log($"[Prefab Brush] Exited Scene View", this);
#endif

                    //Exited the scene view
                    DisposeKeysVariables();
                }
            }

            _lastIsMouseOverWindow = isMouseOverWindow;

            //Mouse not over window
            if (!isMouseOverWindow)
            {
                PB_OnSceneGuiFocusInteraction();

                return;
            }

            EventHandlerBoth();

            if (_isShiftDown || _isAltDown || _isCtrlDown)
            {
#if HARPIA_DEBUG
                //Debug.Log($"Returning Here - shift down {_isShiftDown} = alt down {_isAltDown} - ctrl down {_isCtrlDown} - {Event.current.keyCode} - {Event.current.type}");
#endif

                if (_isCtrlDown && Event.current.keyCode == KeyCode.Z && Event.current.type == EventType.KeyUp)
                {
                    PB_UndoManager.PerformUndo();
                    isChangeSizeDown = false;
                }

                PB_HandlesExtension.Update(deltaTime);
                return;
            }

            switch (GetPaintMode())
            {
                case PaintMode.Precision:
                    if (currentPrefabs.Count == 0) return;
                    OnSceneGuiPrecisionMode();
                    PB_HandlesExtension.Update(deltaTime);
                    PB_PhysicsSimulator.DrawArrowHandles();
                    PB_AttractorManager.DrawHandles();
                    break;

                case PaintMode.Multiple:
                    if (currentPrefabs.Count == 0) return;
                    OnSceneGUIMultiple();
                    PB_HandlesExtension.Update(deltaTime);
                    PB_PhysicsSimulator.DrawArrowHandles();
                    break;

                case PaintMode.Eraser:
                    OnSceneGuiEraser();
                    PB_HandlesExtension.Update(deltaTime);
                    break;
            }
        }

        private void OnSceneGUIMultiple()
        {
            int mouseButton = Event.current.button;

            switch (Event.current.type)
            {
                case EventType.MouseDrag:

                    if (mouseButton is 1 or 2)
                    {
                        isRaycastHit = false;
                        return;
                    }

                    if (isRaycastTagAllowed && mouseButton == 0 && GetPaintMode() == PaintMode.Multiple)
                    {
                        //Radius Check
                        if (brushRadiusSlider.value == 0)
                        {
                            PB_HandlesExtension.WriteTextErrorTemp("Brush Radius is 0", lastHitInfo, 1f);
                            return;
                        }

                        //Parent Check
                        if (GetParentMode() == ParentMode.Fixed_Transform && parentField.value == null)
                        {
                            PB_HandlesExtension.WriteTextErrorTemp("No Parent Selected", lastHitInfo, 1f);
                            return;
                        }

                        PB_PressurePen.Update();
                        UpdateMouseRaycast();

                        int callTimes = (int)brushRadiusSlider.value;
                        if (callTimes <= 0) callTimes = 1;
                        for (int i = 0; i < callTimes; i++)
                        {
                            PaintDrag();
                        }
                    }

                    break;

                case EventType.MouseDown:

                    if (mouseButton is 1 or 2)
                    {
                        isRaycastHit = false;
                        return;
                    }

                    UpdateMouseRaycast();

                    if (isRaycastTagAllowed && Event.current.button == 0)
                    {
                        PB_MultipleModeManager.OnPaintStart();
                        PB_PressurePen.OnMouseDown();
                    }

                    UseCurrentEventPB();
                    break;

                case EventType.MouseUp:
                    if (mouseButton == 0)
                    {
                        PB_UndoManager.RegisterUndo();
                        PrefabBrushObject.Dispose(false);
                        PB_PressurePen.OnMouseUp();
                    }

                    break;

                case EventType.Used:
                    UseCurrentEventPB();
                    break;

                case EventType.MouseMove:
                    UpdateMouseRaycast();

                    break;

                case EventType.Repaint:

                    if (layerMaskField.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Layer Mask Is Nothing", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (tagMaskField.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Tag Mask Is Nothing", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (!isRaycastHit)
                    {
                        PB_HandlesExtension.WriteNoRaycastHit(deltaTime);
                        break;
                    }

                    DrawPrefabCircleMultiple();
                    PB_SnappingManager.DrawGrid(lastHitInfo, false);
                    if (showBoundsToggle.value)
                    {
                        float d = Vector3.Distance(sceneCamera.transform.position, lastHitInfo.point) + 2f;
                        PrefabBrushObject.DrawBoundsCircles(lastHitInfo.point, d, PB_EraserManager.eraserRadiusSlider.value, true);
                    }

                    break;

                case EventType.KeyDown:
                    OnKeyDownBrushMode();
                    break;
            }
        }

        private void OnSceneGuiEraser()
        {
            int mouseButton = Event.current.button;

            switch (Event.current.type)
            {
                case EventType.Repaint:

                    if (PB_EraserManager.eraserMask.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Eraser Layer Mask Is None", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (PB_EraserManager.eraserTagMask.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Eraser Tag Mask Is None", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (PB_EraserManager.eraserRadiusSlider.value == 0)
                    {
                        PB_HandlesExtension.WriteTextErrorTemp("Eraser Radius is 0", lastHitInfo);
                        return;
                    }

                    if (!isRaycastHit)
                    {
                        PB_HandlesExtension.WriteNoRaycastHit(deltaTime);
                        return;
                    }

                    PB_EraserManager.DrawCircleEraser(lastHitInfo, PB_EraserManager.eraserRadiusSlider.value);
                    PrefabBrushTool.DrawGuideLines(lastHitInfo);
                    break;

                case EventType.MouseDrag:
                    UpdateMouseRaycast();
                    if (mouseButton is 1 or 2) isRaycastHit = false;
                    else if (mouseButton == 0)
                    {
                        PB_EraserManager.Eraser(lastHitInfo, PB_EraserManager.eraserRadiusSlider.value);
                        UseCurrentEventPB();
                    }

                    break;

                case EventType.MouseMove:
                    UpdateMouseRaycast();
                    PB_TerrainHandler.Init();
                    break;

                case EventType.MouseDown:
                    UpdateShortcuts();
                    if (mouseButton is 1 or 2) isRaycastHit = false;
                    else if (mouseButton == 0)
                    {
                        if (focusedWindow.GetType() != typeof(SceneView)) return;
                        mouse0DownPosition = Event.current.mousePosition;
                        UseCurrentEventPB();
                    }

                    break;

                case EventType.MouseUp:
                    if (mouseButton == 0)
                    {
                        PB_UndoManager.RegisterUndo();
                        RaycastMode getMode = GetRaycastMode();
                        if (getMode == RaycastMode.Mesh)
                        {
                            PB_MeshBatcher.Dispose();
                        }
                    }

                    break;

                case EventType.KeyDown:
                    OnKeyDownEraserMode();
                    break;

                case EventType.Used:
                    UseCurrentEventPB();
                    break;
            }
        }

        private void OnSceneGuiPrecisionMode()
        {
            int mouseButton = Event.current.button;
            switch (Event.current.type)
            {
                case EventType.MouseDrag:
                    if (mouseButton is 1 or 2) isRaycastHit = false;
                    break;

                case EventType.MouseDown:

                    UpdateShortcuts();
                    if (mouseButton is 1 or 2)
                    {
                        isRaycastHit = false;
                        return;
                    }

                    UseCurrentEventPB();
                    if (mouseButton == 0)
                    {
                        _successPainted = false;
                        if (focusedWindow != null && focusedWindow.GetType() != typeof(SceneView)) return;

                        if (PB_PrecisionModeManager.HasObject() == false) return;

                        if (!IsParentOk())
                        {
                            PB_HandlesExtension.WriteTextErrorTemp("Check Parent Settings", lastHitInfo, .8f);
                            return;
                        }

                        if (!isRaycastHit)
                        {
                            Debug.LogError($"{DebugLogStart} No raycast hit. Check your allowed layers and tags. ");
                            return;
                        }

                        if (!IsAngleValid(lastHitInfo.normal))
                        {
                            PB_HandlesExtension.WriteTextErrorTemp("Invalid Angle", lastHitInfo, .8f);
                            return;
                        }

                        if (PB_PrecisionModeManager.IsScaleZero())
                        {
                            PB_HandlesExtension.WriteTextErrorTemp("Scale Is Zero", lastHitInfo, .8f);
                            return;
                        }

                        Transform p = GetParent();
                        if (IsParentScaleOk(p))
                        {
                            _successPainted = PB_PrecisionModeManager.PaintPrefab(p);
                        }

                        mouse0DownPosition = Event.current.mousePosition;
                    }

                    break;

                case EventType.MouseUp:
                    if (mouseButton == 0 && _successPainted)
                    {
                        PB_UndoManager.RegisterUndo();
                        PB_PrecisionModeManager.SetPrefabToPaint();
                        _isMouse0Down = false;
                    }

                    break;

                case EventType.Used:
                    UseCurrentEventPB();
                    break;

                case EventType.MouseMove:
                    if (Event.current.control || Event.current.command || Event.current.shift) break;

                    if (_freeRotation)
                    {
                        Vector2 diff = Event.current.mousePosition - _lastMousePos;
                        PB_PrecisionModeManager.UpdateFreeRotation(diff);
                        _lastMousePos = Event.current.mousePosition;

                        return;
                    }

                    UpdateMouseRaycast();

                    //Lets check vertex snapping
                    if (PB_SnappingManager.IsUsingVertexSnap())
                    {
                        Ray r = PrefabBrushTool.GetMouseGuiRay();
                        bool anyClose = PB_SnappingManager.IsAnyVertexClose(firstRaycastHitPoint, r, out Vector3 vertex);
                        if (anyClose) lastHitInfo.point = vertex;
                    }

                    //Let's check the attractor
                    if (PB_AttractorManager.HasAnyAttractorClose(lastHitInfo.point, out PrefabBrushAttractor foundedAttractor))
                    {
                        Debug.Log($"Founded Attractor {foundedAttractor.name}");
                        lastHitInfo.point = foundedAttractor.transform.position;
                    }

                    break;

                case EventType.Repaint:
                    if (layerMaskField.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Layer Mask Is None", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (tagMaskField.value == 0)
                    {
                        PB_HandlesExtension.WriteTempTextAtMousePos("Tag Mask Is None", PB_HandlesExtension.errorColor, 4);
                        return;
                    }

                    if (!isRaycastHit)
                    {
                        PB_PrecisionModeManager.SetCurrentObjectActive(false);
                        if (mouseButton != 2 && mouseButton != 1 && !_isMouse1Down)
                        {
                            PB_HandlesExtension.WriteNoRaycastHit(deltaTime);
                        }

                        return;
                    }

                    PB_SnappingManager.DrawGrid(lastHitInfo, false);
                    PB_PrecisionModeManager.SetCurrentObjectActive(true);
                    PB_PrecisionModeManager.DrawTemporaryPrefab(lastHitInfo);
                    PB_HandlesExtension.WriteAngle(lastHitInfo);
                    break;

                case EventType.KeyDown:
                    OnKeyDownPrecisionMode();
                    break;

                case EventType.MouseLeaveWindow:
                    PB_PrecisionModeManager.HideObject();
                    break;

                case EventType.MouseEnterWindow:
                    PB_PrecisionModeManager.ShowObject();
                    break;

                case EventType.ScrollWheel:

                    if (GetPaintMode() != PaintMode.Precision) return;

                    //Scroll wheel rotate object
                    float scrollValue = Event.current.delta.y * advancedSettings.scrollRotationSpeed.value;

                    if (isXDown)
                    {
                        PB_PrecisionModeManager.RotateCurrentObject(new Vector3(scrollValue, 0, 0));
                        UseCurrentEventPB();
                    }
                    else if (isYDown)
                    {
                        PB_PrecisionModeManager.RotateCurrentObject(new Vector3(0, scrollValue, 0));
                        UseCurrentEventPB();
                    }
                    else if (isZDown)
                    {
                        PB_PrecisionModeManager.RotateCurrentObject(new Vector3(0, 0, scrollValue));
                        UseCurrentEventPB();
                    }
                    else if (isChangeSizeDown)
                    {
                        scrollValue *= .01f;
                        PB_PrecisionModeManager.AddScale(scrollValue);
                        UseCurrentEventPB();
                    }
                    else if (isAdjustDistanceFromGroundDown)
                    {
                        PB_PrecisionModeManager.AdjustDistanceY(scrollValue);
                        UseCurrentEventPB();
                    }

                    break;
            }
        }

        private void PB_OnSceneGuiFocusInteraction()
        {
            if (VisualElementsExtension.focusElement == null) return;
            if (Event.current.type != EventType.Repaint) return;

            VisualElement focus = VisualElementsExtension.focusElement;
            if (focus == brushRadiusSlider || focus == advancedSettings.brushBaseColor || focus == advancedSettings.brushBorderColor)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PrefabBrushTool.DrawCircle(hitInfo, brushRadiusSlider.value, true);
                return;
            }

            if (focus == clippingToleranceSlider)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                Debug.Log($"here", this);
                float d = Vector3.Distance(sceneCamera.transform.position, hitInfo.point) + 2f;
                PrefabBrushObject.DrawBoundsCircles(hitInfo.point, d, clippingToleranceSlider.value, true);
                return;
            }

            if (focus == scaleField.maxField)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PrecisionModeManager.DrawTemporaryPrefab(hitInfo, scaleField.maxField.value);
                return;
            }

            if (focus == scaleField.minField)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PrecisionModeManager.DrawTemporaryPrefab(hitInfo, scaleField.minField.value);
                return;
            }

            if (focus == offsetField.fixedField)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PrefabBrushTool.DrawDottedLines(hitInfo, offsetField.fixedField.value);
                PB_PrecisionModeManager.DrawTemporaryPrefab(hitInfo, offsetField.fixedField.value);
                return;
            }

            if (focus == _showWorldLines || focus == worldLinesDistance)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PrefabBrushTool.DrawGuideLines(hitInfo);
                return;
            }

            if (focus == PB_EraserManager.eraserRadiusSlider || focus == advancedSettings.eraserColor)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_EraserManager.DrawCircleEraser(hitInfo, PB_EraserManager.eraserRadiusSlider.value);
                return;
            }

            if (focus == PB_EraserManager.eraserClippingRadiusSlider)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                float d = Vector3.Distance(sceneCamera.transform.position, hitInfo.point) + 2f;
                PrefabBrushObject.DrawBoundsCircles(hitInfo.point, d, PB_EraserManager.eraserRadiusSlider.value, false);
                return;
            }

            if (focus == PB_SnappingManager.gridSnapValueField || focus == advancedSettings.gridColor || focus == PB_SnappingManager.gridOffsetField)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_SnappingManager.DrawGrid(hitInfo, true);
                return;
            }

            if (focus == PB_PhysicsSimulator.impulseFieldVec3.fixedField || focus == PB_PhysicsSimulator.fixedButton)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PhysicsSimulator.DrawArrowHandles(PB_PhysicsSimulator.impulseFieldVec3.fixedField.value, hitInfo.point);
                return;
            }

            if (focus == PB_PhysicsSimulator.impulseFieldVec3.minField || focus == PB_PhysicsSimulator.minButton)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PhysicsSimulator.DrawArrowHandles(PB_PhysicsSimulator.impulseFieldVec3.minField.value, hitInfo.point);
                return;
            }

            if (focus == PB_SnappingManager._vertexSnapMinDistance)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PhysicsSimulator.DrawSphereHandles(PB_SnappingManager._vertexSnapMinDistance.value, hitInfo.point);
                return;
            }

            if (focus == PB_PhysicsSimulator.impulseFieldVec3.maxField || focus == PB_PhysicsSimulator.maxButton)
            {
                RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, GetRaycastMode(), layerMaskField.value);
                PB_PhysicsSimulator.DrawArrowHandles(PB_PhysicsSimulator.impulseFieldVec3.maxField.value, hitInfo.point);
            }

            if (focus == PB_FakePlaneManger.fakePlaneYPosField)
            {
                PB_FakePlaneManger.DrawFakePlane();
            }
        }

        #endregion

        #region KeyDownEvents

        private void OnKeyDownEraserMode()
        {
            if (PB_ModularShortcuts.increaseRadius.IsShortcut())
            {
                PB_EraserManager.IncreaseRadius(brushSizeIncrement);
            }
            else if (PB_ModularShortcuts.decreaseRadius.IsShortcut())
            {
                PB_EraserManager.IncreaseRadius(-brushSizeIncrement);
            }
        }

        private void OnKeyDownPrecisionMode()
        {
            if (PB_ModularShortcuts.rotateRight.IsShortcut())
            {
                Vector3 rotY = PB_PrecisionModeManager.AddToRotation(precisionModeRotationAngle.value);
                PB_HandlesExtension.WriteVector3(rotY, lastHitInfo, "Rotation", "°");
            }

            else if (PB_ModularShortcuts.rotateLeft.IsShortcut())
            {
                Vector3 rotY2 = PB_PrecisionModeManager.AddToRotation(-precisionModeRotationAngle.value);
                PB_HandlesExtension.WriteVector3(rotY2, lastHitInfo, "Rotation", "°");
            }

            else if (PB_ModularShortcuts.randomRotation.IsShortcut())
            {
                RotationTypes rotationMode = rotationField.GetMode();
                if (rotationMode != RotationTypes.Random)
                {
                    PB_HandlesExtension.WriteTextErrorTemp("Random Rotation is only available\nwhen using Random Rotation Mode", lastHitInfo, 1f);
                }
                else
                {
                    Vector3 randomRot = rotationField.GetValue(true, lastHitInfo);
                    PB_PrecisionModeManager.SetRotation(randomRot);
                    PB_HandlesExtension.WriteVector3(randomRot, lastHitInfo, "Rotation", "°");
                }
            }

            else if (PB_ModularShortcuts.decreaseRadius.IsShortcut())
            {
                Vector3 newSize = PB_PrecisionModeManager.IncreaseSize(-.1f);
                PB_HandlesExtension.WriteTempText("Size: ", newSize, lastHitInfo);
            }

            else if (PB_ModularShortcuts.increaseRadius.IsShortcut())
            {
                Vector3 newSize2 = PB_PrecisionModeManager.IncreaseSize(.1f);
                PB_HandlesExtension.WriteTempText("Size: ", newSize2, lastHitInfo);
            }

            else if (PB_ModularShortcuts.normalizeSize.IsShortcut())
            {
                Vector3 normalizedSize = PB_PrecisionModeManager.NormalizeSize();
                PB_HandlesExtension.WriteTempText("Size: ", normalizedSize, lastHitInfo);
            }

            else if (PB_ModularShortcuts.nextPrefab.IsShortcut())
            {
                PbPrefabUI prefab = GetNextPrefab(PB_PrecisionModeManager.PrefabToPaint);
                PB_PrecisionModeManager.SetPrefabToPaint(prefab);
                PB_HandlesExtension.WriteTempText($"Changed Prefab to: {prefab.GetName()}");
            }

            else if (PB_ModularShortcuts.previousPrefab.IsShortcut())
            {
                PbPrefabUI prefab = GetNextPrefab(PB_PrecisionModeManager.PrefabToPaint, true);
                PB_PrecisionModeManager.SetPrefabToPaint();
                PB_HandlesExtension.WriteTempText($"Changed Prefab to: {prefab.GetName()}");
            }

            else if (PB_ModularShortcuts.freeRotation.IsShortcut())
            {
                _freeRotation = true;
                _lastMousePos = Event.current.mousePosition;
                string val = PB_HandlesExtension.Vec3ToReadable(PB_PrecisionModeManager.GetCurrentRotationVec3());
                PB_HandlesExtension.WriteTempText($"Free Rotation Enabled\n{val}");
            }
            else if (PB_ModularShortcuts.nextPivot.IsShortcut())
            {
                PB_AttractorManager.OnNextPivot(null);
            }
            else if (PB_ModularShortcuts.previousPivot.IsShortcut())
            {
                PB_AttractorManager.OnPreviousPivot(null);
            }
        }

        private void OnKeyDownBrushMode()
        {
            if (PB_ModularShortcuts.exitTool.IsShortcut())
            {
                ExitTool();
            }
            else if (PB_ModularShortcuts.increaseRadius.IsShortcut())
            {
                IncreaseBrushSize(brushSizeIncrement);
            }
            else if (PB_ModularShortcuts.decreaseRadius.IsShortcut())
            {
                IncreaseBrushSize(-brushSizeIncrement);
            }
        }

        #endregion

        private PbPrefabUI GetNextPrefab(GameObject prefabToPaint, bool reverse = false)
        {
            int index = currentPrefabs.FindIndex(e => e.prefabToPaint == prefabToPaint);

            if (!reverse)
            {
                index++;
                if (index >= currentPrefabs.Count) index = 0;
            }
            else
            {
                index--;
                if (index < 0) index = currentPrefabs.Count - 1;
            }

            currentPrefabs[index].Select();
            return currentPrefabs[index];
        }

        private void EventHandlerBoth()
        {
            EventType evtType = Event.current.type;

            switch (evtType)
            {
                case EventType.Repaint:
                {
                    double time = EditorApplication.timeSinceStartup;
                    deltaTime = (float)(time - lastRepaintTime);
                    lastRepaintTime = time;

                    if (_isMouse1Down) return;

                    PB_BoundsManager.DrawBounds(new Bounds(), true);

                    if (!isRaycastTagAllowed)
                    {
                        if (lastHitInfo.transform != null)
                            PB_HandlesExtension.WriteTextErrorTemp($"Tag not allowed: {lastHitInfo.transform.tag}", lastHitInfo);
                    }

                    if (!_isTextureAllowed && GetPaintMode() != PaintMode.Eraser)
                        PB_HandlesExtension.WriteTextErrorTemp($"Texture Not Allowed", lastHitInfo);

                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.changeMode.IsShortcut(Event.current.keyCode):
                {
                    if (PB_EraserManager.IsOnEraseMode()) ExitEraserMode();
                    else EnterEraserMode();
                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.rotationXShortcut.IsShortcut():
                {
                    isXDown = true;
                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.rotationYShortcut.IsShortcut() && !_isCtrlDown:
                {
                    isYDown = true;
                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.rotationZShortcut.IsShortcut() && !_isCtrlDown:
                {
                    isZDown = true;
                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.changeScaleShortcut.IsShortcut():
                {
                    isChangeSizeDown = true;
                    if (GetPaintMode() == PaintMode.Precision) UseCurrentEventPB();
                    return;
                }
                case EventType.KeyDown when PB_ModularShortcuts.yDisplacementShortcut.IsShortcut():
                {
                    isAdjustDistanceFromGroundDown = true;
                    if (GetPaintMode() == PaintMode.Precision) UseCurrentEventPB();
                    return;
                }
                case EventType.KeyDown:

                    switch (Event.current.keyCode)
                    {
                        case KeyCode.LeftShift or KeyCode.RightShift:
                            if (!_isShiftDown) _lastMousePosShift = Event.current.mousePosition;
                            _isShiftDown = true;
                            PB_PrecisionModeManager.HideObject();
                            break;

                        case KeyCode.LeftAlt or KeyCode.RightAlt or KeyCode.AltGr:
                            _isAltDown = true;
                            PB_PrecisionModeManager.HideObject();
                            break;

                        case KeyCode.RightControl or KeyCode.LeftControl or KeyCode.LeftCommand or KeyCode.RightCommand or KeyCode.LeftApple or KeyCode.RightApple:
                            if (!_isCtrlDown) _lastMousePosCtrl = Event.current.mousePosition;
                            _isCtrlDown = true;
                            PB_PrecisionModeManager.HideObject();
                            break;
                    }

                    return;
                case EventType.KeyUp when PB_ModularShortcuts.rotationXShortcut.IsShortcut():
                    isXDown = false;
                    return;
                case EventType.KeyUp when PB_ModularShortcuts.rotationYShortcut.IsShortcut():
                    isYDown = false;
                    return;
                case EventType.KeyUp when PB_ModularShortcuts.rotationZShortcut.IsShortcut():
                    isZDown = false;
                    return;
                case EventType.KeyUp when PB_ModularShortcuts.changeScaleShortcut.IsShortcut():
                    isChangeSizeDown = false;
                    return;
                case EventType.KeyUp when PB_ModularShortcuts.yDisplacementShortcut.IsShortcut():
                    isAdjustDistanceFromGroundDown = false;
                    return;
                case EventType.KeyUp when PB_ModularShortcuts.freeRotation.IsShortcut():
                    _freeRotation = false;
                    PB_HandlesExtension.WriteTempText("Free Rotation Disabled");
                    return;

                case EventType.KeyUp:
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.LeftShift or KeyCode.RightShift:
                            _isShiftDown = false;
                            break;
                        case KeyCode.RightControl or KeyCode.LeftControl or KeyCode.LeftCommand or KeyCode.RightCommand:
                            _isCtrlDown = false;
                            break;

                        case KeyCode.LeftAlt or KeyCode.RightAlt or KeyCode.AltGr:
                            _isAltDown = false;
                            break;
                    }

                    if (PB_ModularShortcuts.exitTool.IsShortcut()) ExitTool();

                    return;
                }
                case EventType.MouseDown:
                    PrefabBrushTool.EnableAutoRefresh();
                    UpdateShortcuts();
                    switch (Event.current.button)
                    {
                        case 0:
                            mouse0DownPosition = Event.current.mousePosition;
                            LastMouse0DownTime = EditorApplication.timeSinceStartup;
                            _isMouse0Down = true;
                            UpdateShortcuts();
                            break;
                        case 1:
                            _isMouse1Down = true;
                            break;
                    }

                    return;
                case EventType.MouseUp:
                    switch (Event.current.button)
                    {
                        case 0:
                            _isMouse0Down = false;
                            LastMouse0DownTime = double.PositiveInfinity;
                            break;
                        case 1:
                            _isMouse1Down = false;
                            break;
                    }

                    UpdateShortcuts();
                    break;
                case EventType.MouseEnterWindow when !PrefabBrushTool.IsMouseOverAnySceneView():
                    return;
                case EventType.MouseEnterWindow:
                {
                    SceneView sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null) sceneView.Focus();
                    FocusWindowIfItsOpen<SceneView>();
                    break;
                }
            }
        }

        private void EnterEraserMode()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] EnterEraserMode", this);
#endif
            Selection.activeObject = null;
            lastMode = GetPaintMode();
            paintModeDropdown.SetValueWithoutNotify(PaintMode.Eraser);

            PB_SnappingManager.UpdateUI();
            advancedSettings.SetActive(false);
            PB_PhysicsSimulator.impulseFieldVec3.SetActive(false);
            PB_PhysicsSimulator.impulseForceSlider.parent.SetActive(false);

            PB_PrecisionModeManager.SetCurrentObjectActive(false);
            UpdateUITool();
            UpdateEraserModeUI();
            PB_BoundsManager.DisposeBounds();
            PrefabBrushObject.ForceInit();
            CustomPrefabProps.Hide();
            PB_HandlesExtension.WriteTempTextAtMousePos($"Entered {GetPaintMode()} Mode", Color.white, 2, 1f);
        }

        private void ExitEraserMode()
        {
#if HARPIA_DEBUG
            Debug.Log($"ExitEraserMode", this);
#endif
            PB_PrecisionModeManager.SetCurrentObjectActive(true);
            PB_PrecisionModeManager.SetCurrentObjectPos(lastHitInfo.point);
            paintModeDropdown.SetValueWithoutNotify(lastMode);

            PB_PhysicsSimulator.impulseFieldVec3.SetActive(true);
            PB_PhysicsSimulator.impulseForceSlider.parent.SetActive(true);

            UpdateEraserModeUI();
            PB_HandlesExtension.WriteTempTextAtMousePos($"Entered {GetPaintMode()} Mode", Color.white, 2, 1f);
            PrefabBrushObject.Dispose(false);

            UpdateUITool();
        }

        private void UpdateEraserModeUI()
        {
            bool onEraserMode = GetPaintMode() == PaintMode.Eraser;
            _precisionModeFoldout.SetActive(!onEraserMode);
            parentField.parent.SetActive(!onEraserMode);
            pivotMode.parent.SetActive(!onEraserMode);
            scaleField.enumField.parent.SetActive(!onEraserMode);

            prefabsUiHolder.parent.SetActive(!onEraserMode);
            _templatesDropdown.parent.parent.SetActive(!onEraserMode);
            _dragAndDropLabel.parent.SetActive(!onEraserMode);
            _addSelectedPrefabsPanel.SetActive(false);

            PB_EraserManager.ToggleUI(onEraserMode);
            UpdateShortcuts();
        }

        private void IncreaseBrushSize(float p0)
        {
            float value = brushRadiusSlider.value;
            value += p0 * value;
            value = Mathf.Clamp(value, brushRadiusSlider.lowValue, brushRadiusSlider.highValue);
            brushRadiusSlider.value = value;

            string format = value < 1 ? "f2" : "f1";
            string t1 = "Radius: " + value.ToString(format);
            PB_HandlesExtension.WriteTempTextAtMousePos(t1, Color.white, 3);
        }

        private void OnRaycastModeChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"On Raycast Mode Changed {evt.newValue}", this);
#endif

            RaycastMode newMode = (RaycastMode)evt.newValue;
            if (newMode == RaycastMode.Mesh)
            {
                if (!SystemInfo.supportsComputeShaders)
                {
                    DisplayError("Your system does not support compute shaders :(\n\nReverting mode to Physical Colliders");
                    raycastModeDropdown.SetValueWithoutNotify(RaycastMode.Physical_Collider);
                    return;
                }

                if (PB_TerrainHandler.HasAnyTerrain())
                {
                    DisplayStatus("Attention: Mesh Raycast mode is not compatible with Terrains. If you are facing issues, please switch to Physical Collider mode.");
                }
            }

            precisionModeAddMeshToBatch.SetActive(newMode == RaycastMode.Mesh);
            PB_TextureMaskHandler.UpdateUITerrain();
        }

        public void ExitTool()
        {
            if (PrefabBrushTool.isUsingTool == false) return;
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] Exiting tool");
#endif

            PrefabBrushTool.isUsingTool = false;
            PrefabBrushObject.Dispose();
            PB_PrecisionModeManager.DisposePrecisionMode();
            PB_MeshRaycaster.Dispose();
            PB_VertexFinder.Dispose();
            PB_TerrainHandler.Dispose();
            PB_TextureMaskHandler.Dispose();
            PB_PhysicsSimulator.Dispose();
            PB_ThumbnailGenerator.Dispose();
            PB_ShortcutManager.Dispose();
            PbPrefabUI.Dispose(currentPrefabs);

            UpdateShortcuts();
            UpdateUITool();
        }

        private void DisposeKeysVariables()
        {
            isXDown = false;
            isYDown = false;
            isZDown = false;
            _isShiftDown = false;
            _isAltDown = false;
            _isCtrlDown = false;
            isAdjustDistanceFromGroundDown = false;
            isChangeSizeDown = false;
        }

        private void ResetStaticVariables()
        {
            mouse0DownPosition = Vector2.zero;
            _lastMousePosCtrl = Vector2.zero;
            _lastMousePosShift = Vector2.zero;
            LastMouse0DownTime = 0;

            _isMouse0Down = false;
            _isMouse1Down = false;
            isRaycastHit = false;
            lastHitInfo = new RaycastHit();
        }

        public bool IsParentOk()
        {
            return GetParentMode() != ParentMode.Fixed_Transform || parentField.value != null;
        }

        public bool IsParentScaleOk(Transform parent)
        {
            ParentMode mode = GetParentMode();
            if (mode == ParentMode.Hit_Surface_Object && _toggleUniformScaleParent.value && !PrefabBrushTool.IsVector3Uniform(parent.lossyScale))
            {
                return false;
            }

            return true;
        }

        public Transform GetParent()
        {
            return GetParentMode() switch{
                ParentMode.Fixed_Transform => parentField.value as Transform,
                ParentMode.Hit_Surface_Object => lastHitInfo.transform,
                ParentMode.No_Parent => null,
                _ => null
            };
        }

        private void PaintDrag()
        {
            //Strenght Random value
            bool chance = Random.value <= (sliderBrushStrength.value);
            if (!chance) return;

            //Random Hit check
            randomPointInsideDisc = PrefabBrushTool.GetRandomPointInsideDisc(lastHitInfo, brushRadiusSlider.value);
            bool hitRandomPoint = PrefabBrushTool.RaycastToWorldPoint(sceneCamera, randomPointInsideDisc, layerMaskField.value, out RaycastHit hitInfoRandomPoint, GetRaycastMode(), out Ray r);
            if (!hitRandomPoint) return;

            //Grid Check
            if (PB_SnappingManager.IsUsingGridSnap())
            {
                randomPointInsideDisc = PB_SnappingManager.TryToGetPositionOnGrid(randomPointInsideDisc);
            }

            //Angle check
            if (useAngleLimitsToggle.value)
            {
                float angle = Vector3.Angle(hitInfoRandomPoint.normal, Vector3.up);
                if (!IsAngleValid(angle)) return;
            }

            //Clip Test
            PbPrefabUI prefab = GetRandomPrefab(false);
            PrefabBrushObject.RadiusBounds bounds = PB_BoundsManager.GetRadiusBound(prefab.prefabToPaint);
            if (PB_BoundsManager.BoundsIntersects(randomPointInsideDisc, bounds.clippingRadiusPlacing, true))
            {
                return;
            }

            //Texture Check
            if (!PB_TextureMaskHandler.IsTextureValid(hitInfoRandomPoint.point)) return;

            //Paint
            PB_MultipleModeManager.PaintPrefabMultiple(hitInfoRandomPoint, prefab, r);
        }

        public bool IsAngleValid(float a)
        {
            if (!useAngleLimitsToggle.value) return true;
            if (a > angleLimitsField.value.y) return false;
            if (a < angleLimitsField.value.x) return false;
            return true;
        }

        private bool IsAngleValid(Vector3 normal)
        {
            if (!useAngleLimitsToggle.value) return true;
            float a = Vector3.Angle(normal, Vector3.up);
            return IsAngleValid(a);
        }

        public static Quaternion GetAlignedRotation(RaycastHit hit)
        {
            return Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        public Vector3 GetLocalPosition(GameObject childObject, PivotMode pivotModeParam)
        {
            if (showBoundsToggle.value)
            {
                Bounds boundsToDraw = PB_BoundsManager.GetBounds(childObject);
                PB_BoundsManager.DrawBounds(boundsToDraw, false);
            }

            if (pivotModeParam == PivotMode.MeshPivot) return Vector3.zero;

            childObject.transform.position = Vector3.zero;

            Quaternion rotation = childObject.transform.rotation;
            Quaternion oldRot = rotation;

            //Bounds can only be calculated with identity rotation
            childObject.transform.rotation = Quaternion.identity;
            Bounds boundsToCalc = PB_BoundsManager.GetBounds(childObject);
            rotation = oldRot;

            childObject.transform.rotation = rotation;
            return CalculatePivot(childObject, pivotModeParam, boundsToCalc);
        }

        private static Vector3 CalculatePivot(GameObject childObject, PivotMode pivotModeParam, Bounds boundsToCalc)
        {
            switch (pivotModeParam)
            {
                case PivotMode.Bounds_Center:
                {
                    return childObject.transform.position - boundsToCalc.center;
                }
                case PivotMode.Bounds_Center_Bottom:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(0, boundsToCalc.extents.y, 0);
                }
                case PivotMode.Bounds_Center_Top:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(0, -boundsToCalc.extents.y, 0);
                }
                case PivotMode.Bounds_Center_Forward:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(0, 0, boundsToCalc.extents.z);
                }
                case PivotMode.Bounds_Center_Back:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(0, 0, -boundsToCalc.extents.z);
                }
                case PivotMode.Bounds_Center_Right:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(boundsToCalc.extents.x, 0, 0);
                }
                case PivotMode.Bounds_Center_Left:
                {
                    return childObject.transform.position - boundsToCalc.center + new Vector3(-boundsToCalc.extents.x, 0, 0);
                }
                case PivotMode.Prefab_Brush_Pivot_Component:
                {
                    //We need to change the position after
                    return Vector3.zero;
                }
                default:
                    return Vector3.zero;
            }
        }

        private void UseCurrentEventPB()
        {
            Event.current.Use();
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
        }

        private void DrawPrefabCircleMultiple()
        {
            if (!isRaycastHit) return;
            if (!isRaycastTagAllowed) return;

            bool validAngle = IsAngleValid(lastHitInfo.normal);
            PrefabBrushTool.DrawCircle(lastHitInfo, brushRadiusSlider.value, validAngle);

            if (!validAngle)
            {
                PB_HandlesExtension.WriteTextErrorTemp("Invalid Angle", lastHitInfo, deltaTime);
                return;
            }

            if (offsetField.GetMode() == Vector3Mode.Fixed)
            {
                if (offsetField.fixedField.value == Vector3.zero) return;
                PrefabBrushTool.DrawPoint(lastHitInfo.point + offsetField.fixedField.value);
                PrefabBrushTool.DrawDottedLines(lastHitInfo, offsetField.fixedField.value);
                return;
            }

            PrefabBrushTool.DrawPoint(lastHitInfo.point + offsetField.minField.value);
            PrefabBrushTool.DrawPoint(lastHitInfo.point + offsetField.maxField.value);
        }

        private void UpdateMouseRaycast()
        {
            void CalculateHitOnPlane(Ray ray)
            {
                if (!PB_FakePlaneManger.IsUsingFakePlane()) return;
                isRaycastHit = PB_FakePlaneManger.Raycast(ray, out float _, out lastHitInfo);
                firstRaycastHitPoint = lastHitInfo.point;
            }

            isRaycastTagAllowed = true;
            _isTextureAllowed = true;
            var raycastMode = GetRaycastMode();
            bool calculateMeshRaycast = raycastMode is RaycastMode.Mesh or RaycastMode.Both;
            bool calculatePhysicsRaycast = raycastMode is RaycastMode.Physical_Collider or RaycastMode.Both;

            bool meshRaycastHit = false;
            bool physicsRaycastHit = false;
            RaycastHit meshRaycastHitInfo = new RaycastHit();
            RaycastHit physicsRaycastHitInfo = new RaycastHit();
            Ray ray = PrefabBrushTool.GetMouseGuiRay();

            //Mesh Raycast
            if (calculateMeshRaycast)
            {
                Mesh batch = PB_MeshBatcher.BatchForRaycast();
                meshRaycastHit = PB_MeshRaycaster.Raycast(ray, batch, out MeshRaycastResult meshHit);
                meshRaycastHitInfo = meshHit.ToHitInfo();

                if (!calculatePhysicsRaycast)
                {
                    if (!meshRaycastHit)
                    {
                        Ray mouseRay = PrefabBrushTool.GetMouseGuiRay();
                        CalculateHitOnPlane(mouseRay);
                        return;
                    }

                    isRaycastHit = meshRaycastHit;
                    lastHitInfo = meshRaycastHitInfo;
                    firstRaycastHitPoint = lastHitInfo.point;
                    return;
                }
            }

            //Physics Raycast
            if (calculatePhysicsRaycast)
            {
                physicsRaycastHit = PrefabBrushTool.RaycastPhysicsGUI(sceneCamera.farClipPlane, layerMaskField.value, out physicsRaycastHitInfo, ray);
                if (!calculateMeshRaycast)
                {
                    if (!physicsRaycastHit)
                    {
                        CalculateHitOnPlane(ray);
                        return;
                    }

                    isRaycastHit = physicsRaycastHit;
                    lastHitInfo = physicsRaycastHitInfo;

                    if (!isRaycastHit) return;
                    string tag = lastHitInfo.collider.gameObject.tag;
                    isRaycastTagAllowed = IsTagAllowed(tag, _currentTagsSelected);
                    _isTextureAllowed = PB_TextureMaskHandler.IsTextureValid(lastHitInfo.point);
                    return;
                }
            }

            //Both Raycast

            switch (physicsRaycastHit)
            {
                case false when !meshRaycastHit:
                    CalculateHitOnPlane(ray);
                    return;
                case false:
                    lastHitInfo = meshRaycastHitInfo;
                    isRaycastHit = true;
                    return;
            }

            if (!meshRaycastHit)
            {
                lastHitInfo = physicsRaycastHitInfo;
                isRaycastHit = true;
                return;
            }

            lastHitInfo = meshRaycastHitInfo.distance < physicsRaycastHitInfo.distance ? meshRaycastHitInfo : physicsRaycastHitInfo;
            isRaycastHit = true;
        }

        public RaycastMode GetRaycastMode()
        {
            return (RaycastMode)raycastModeDropdown.value;
        }

        private void OnActiveContextChanged()
        {
#if HARPIA_DEBUG
            Debug.Log($"Active context changed to {ToolManager.activeContextType}");
#endif
            UpdateUITool();
        }

        private void OnTagMaskChanged(ChangeEvent<int> evt)
        {
#if HARPIA_DEBUG
            if (evt != null)
                Debug.Log($"Tag mask changed to {evt.newValue} | {tagMaskField.choices.Count} ");
#endif

            _currentTagsSelected = tagMaskField.GetSelectedChoices();

#if HARPIA_DEBUG
            Debug.Log($"Tag selected {string.Join(", ", _currentTagsSelected)}");
#endif
            PB_MeshBatcher.Dispose();
        }

        private void OnLayerMaskChanged(ChangeEvent<int> changeEvent)
        {
#if HARPIA_DEBUG
            Debug.Log($"Layer mask changed to {changeEvent.newValue}");
#endif
            PB_MeshBatcher.Dispose();
        }

        private void OnClearListButton(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log("Clear list button clicked");
#endif
            currentPrefabs.Clear();
            prefabsUiHolder.Clear();
        }

        private void OnParentChanged(ChangeEvent<Object> evt)
        {
#if HARPIA_DEBUG
            Debug.Log("Parent changed");
#endif
            UpdateUITool();
        }

        private void OnCreateParentButton(ClickEvent evt)
        {
            string parentBar = parentNameInput.value;

            if (string.IsNullOrEmpty(parentBar))
            {
                DisplayError("Parent name cannot be empty");
                return;
            }

            //Check if parent already exists
            GameObject foundedParent = GameObject.Find(parentBar);

            if (foundedParent != null)
            {
                const string skipParentKey = "prefab-brush-skip-parent-popup";

                if (!EditorPrefs.GetBool(skipParentKey, false))
                {
                    int option = EditorUtility.DisplayDialogComplex("Prefab Brush",
                        $"There's already a object named {parentBar}.\nDo you want ot use it as parent?",
                        "Yes and don't ask me again", "No", "Yes");

                    // 0 -> yes and dont ask
                    // 2 -> Yes
                    // 1 -> no

                    //If no
                    if (option == 1) return;

                    //If save
                    if (option == 0) EditorPrefs.SetBool(skipParentKey, true);
                }
            }
            else
            {
                //Create a new parent
                foundedParent = new GameObject(parentBar);
            }

            parentField.value = foundedParent;
        }

        private bool DisplayConfirmation(string msg)
        {
            return EditorUtility.DisplayDialog("Prefab Brush", msg, "Yes", "Cancel");
        }

        public static void DisplayError(string msg)
        {
            EditorUtility.DisplayDialog("Prefab Brush - Error", msg, "Ok");
        }

        public static VisualTreeAsset LoadVisualTreeAsset(string xmlFileNameLocal, ref string visualTreeGuid)
        {
            string xmlFilePath = AssetDatabase.GUIDToAssetPath(visualTreeGuid);
            if (string.IsNullOrEmpty(xmlFilePath))
            {
                string[] foundedGUIDs = AssetDatabase.FindAssets(xmlFileNameLocal);

                if (foundedGUIDs.Length == 0)
                {
                    DisplayError($"Could not find the {xmlFileNameLocal}.uxml, did you renamed the file? If so rename it {xmlFileNameLocal}.uxml");
                    return null;
                }

                //get the first founded path
                visualTreeGuid = foundedGUIDs[0];
                xmlFilePath = AssetDatabase.GUIDToAssetPath(visualTreeGuid);
            }

            VisualTreeAsset loaded = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(xmlFilePath);
            if (loaded == null) Debug.LogError($"{DebugLogStart} Could not load file at {xmlFilePath}");

            return loaded;
        }

        public static void Remove(PbPrefabUI pbPrefabData)
        {
            //Get the main objects
            PrefabBrushTemplate templateObject = instance._temporaryTemplate;
            SerializedObject serializedObject = instance._serializedObject;

            //Record Undo
            Undo.RecordObject(templateObject, "Prefab Brush - Remove Prefab");

            //Remove
            templateObject.prefabs.Remove(pbPrefabData);

            //Update teh serialized object
            serializedObject.Update();

            //Update the UI
            if (instance.currentPrefabs.Count == 0) instance.ExitTool();
        }

        public PbPrefabUI GetRandomPrefab(bool select, params GameObject[] exclude)
        {
            IEnumerable<PbPrefabUI> tempList;
            if (GetPaintMode() == PaintMode.Precision)
                tempList = currentPrefabs.Where(e => !exclude.Contains(e.prefabToPaint));
            else
                tempList = currentPrefabs.Where(e => !exclude.Contains(e.prefabToPaint) && e.selected);

            PbPrefabUI[] pbPrefabUis = tempList as PbPrefabUI[] ?? tempList.ToArray();
            if (!pbPrefabUis.Any()) return null;

            PbPrefabUI randomObj = pbPrefabUis.ElementAt(Random.Range(0, pbPrefabUis.Count()));

            if (select) randomObj.Select();

            return randomObj;
        }

        public List<string> GetPrefabGUIDs()
        {
            List<string> guids = new();

            foreach (PbPrefabUI prefab in currentPrefabs)
            {
                string guid = prefab.GetGuid();
                if (string.IsNullOrEmpty(guid)) continue;
                guids.Add(guid);
            }

            return guids;
        }

        public bool IsTagAllowed(string gameObjectTag, List<string> allowedTags)
        {
            if (string.IsNullOrEmpty(gameObjectTag)) return true;
            if (allowedTags == null || allowedTags.Count == 0) OnTagMaskChanged(null);
            if (allowedTags == null || allowedTags.Count == 0) return true;
            return allowedTags.Contains(gameObjectTag);
        }

        //Also apply physics
        public static PrefabBrushObject RegisterObject(GameObject go)
        {
            if (instance.makeObjectsStaticToggle.value)
                go.isStatic = true;

            //Already has the component
            PrefabBrushObject brushObject = null;
            if (go.TryGetComponent(typeof(PrefabBrushObject), out Component com))
            {
                brushObject = com as PrefabBrushObject;
            }

            if (brushObject == null) brushObject = go.AddComponent<PrefabBrushObject>();

            bool erasable = instance.makeErasableToggle.value;
            bool clippingTest = instance.addToClippingCheckToggle.value;
            bool usingPhysics = PB_PhysicsSimulator.IsUsingPhysics();

            brushObject.Init(usingPhysics, erasable, clippingTest, PB_PhysicsSimulator.GetPhysicalLayer(brushObject.gameObject));
            PB_PhysicsSimulator.ApplyImpulse(go);

            return brushObject;
        }

        public void MovePrefabCard(PbPrefabUI pbPrefabUI, bool isLeft)
        {
            //get the card index in the list
            int index = currentPrefabs.IndexOf(pbPrefabUI);
            int newIndex = isLeft ? index - 1 : index + 1;

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] MovePrefabCard - index {index} - new index {newIndex}");
#endif

            if (newIndex < 0 || newIndex >= currentPrefabs.Count)
            {
                Debug.LogWarning($"[{DebugLogStart}] Could Not Move Prefab Card");
                return;
            }

            //swap the cards
            PbPrefabUI temp = currentPrefabs[newIndex];
            currentPrefabs[index] = temp;
            currentPrefabs[newIndex] = pbPrefabUI;

            //update the ui
            prefabsUiHolder.Clear();
            foreach (PbPrefabUI ui in currentPrefabs)
            {
                ui.InstantiateUI();
            }
        }

        public void LoadTemplate(PrefabBrushTemplate prefabBrushTemplate)
        {
            _lastLoadedTemplate = prefabBrushTemplate;

            PrefabBrushTemplate.SetLastTemplate(_lastLoadedTemplate.name);
            PB_PrecisionModeManager.DisposePrecisionMode();

            _temporaryTemplate.CloneFrom(prefabBrushTemplate);
            if (_temporaryTemplate.paintMode == PaintMode.Eraser) _temporaryTemplate.paintMode = PaintMode.Precision;
            _temporaryTemplate.SetAllFoldoutsOpen(true);

            _serializedObject.Update();

            //Update the UI
            PB_SnappingManager.LoadTemplate(_temporaryTemplate);

            if (_lastLoadedTemplate.prefabs != null)
            {
                foreach (PbPrefabUI prefab in _lastLoadedTemplate.prefabs)
                {
                    if (prefab == null) continue;
                    if (prefab.prefabToPaint == null)
                    {
                        Debug.LogWarning($"[{DebugLogStart}] Could not load prefab from template. Reason: Prefab is null");
                        continue;
                    }

                    AddPrefab(prefab);
                }
            }

            UpdateUITool();
            UpdatePrefabList();

            if (GetPaintMode() == PaintMode.Precision && currentPrefabs.Count > 0)
            {
                currentPrefabs[0].Select();
            }

            _templatesDropdown.SetValueWithoutNotify(_lastLoadedTemplate.name);
        }

        public bool HasAnyPrefabsWithPhysics()
        {
            foreach (PbPrefabUI prefabUI in currentPrefabs)
            {
                if (prefabUI == null) continue;
                if (!prefabUI.AllowsPhysicsPlacement()) continue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// This is useful for forcing the UNDO working making
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void FocusHiddenElement() => _hiddenField.Focus();
    }

    public static class PrefabBrushTool
    {
        public static bool isUsingTool;
        private static Color BrushColor1 => PrefabBrush.instance.advancedSettings.brushBorderColor.value;
        private static Color BrushColor2 => PrefabBrush.instance.advancedSettings.brushBaseColor.value;
        private static Color BrushColorError => PrefabBrush.instance.advancedSettings.invalidLocationColor.value;

        public static string GetColorfulText(string s, Color white)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(white)}>{s}</color>";
        }

        public static void InitAllFoldouts(VisualElement root)
        {
            UQueryBuilder<Foldout> foldouts = root.Query<Foldout>();
            foreach (Foldout foldout in foldouts.ToList()) foldout.StylizeFoldout();
        }

        public static void StylizeFoldout(this Foldout f)
        {
            Toggle toogle = f.Q<Toggle>();
            Color color = new Color(1, 1, 1, .03f);
            toogle.SetBackgroundColor(color);
            toogle.style.marginLeft = 0;
            toogle.style.marginRight = 0;
        }

        public static T Find<T>(this VisualElement root, string name, string propName) where T : VisualElement
        {
            SerializedObject obj = PrefabBrush.instance._serializedObject;

            if (obj == null) Debug.LogError($"[Prefab Brush] Serialized Object is null");

            //Find the visual element
            T retValue;
            if (!string.IsNullOrEmpty(name)) retValue = root.Q<T>(name);
            else retValue = root.Q<T>();
            if (retValue == null) Debug.LogError($"[Prefab Brush] Could not find {nameof(T)} with name {name}");

            //Check if the serialized prop name is valid
            if (!string.IsNullOrEmpty(propName))
            {
                bool foundProp = obj.FindProperty(propName) != null;
                if (!foundProp)
                {
#if HARPIA_DEBUG
                    Debug.LogError($"[Prefab Brush] Could not find property {propName} in the template serialized object");
#endif
                    return retValue;
                }
            }

            //Bind the prop
            if (!string.IsNullOrEmpty(propName))
            {
                SerializedProperty serializedProperty = obj.FindProperty(propName);
                switch (retValue)
                {
                    case FloatField ff:
                        ff.BindProperty(serializedProperty);
                        break;
                    case Vector3Field vector3Field:
                        vector3Field.BindProperty(serializedProperty);
                        break;
                    case Slider slider:
                        slider.BindProperty(serializedProperty);
                        break;
                    case IntegerField integerField:
                        integerField.BindProperty(serializedProperty);
                        break;
                    case Toggle toggle:
                        toggle.BindProperty(serializedProperty);
                        break;
                    case ObjectField objectField:
                        objectField.BindProperty(serializedProperty);
                        break;
                    case SliderInt sliderInt:
                        sliderInt.BindProperty(serializedProperty);
                        break;
                    case ColorField cf:
                        cf.BindProperty(serializedProperty);
                        break;
                    case TextField tf:
                        tf.BindProperty(serializedProperty);
                        break;
                    case LayerField lf:
                        lf.BindProperty(serializedProperty);
                        break;
                    case MaskField mf:
                        mf.BindProperty(serializedProperty);
                        break;
                    case Vector2Field vf:
                        vf.BindProperty(serializedProperty);
                        break;
                    case Vector2IntField vif:
                        vif.BindProperty(serializedProperty);
                        break;
                    case MinMaxSlider minMax:
                        minMax.BindProperty(serializedProperty);
                        break;
                    case DropdownField dp:
                        dp.BindProperty(serializedProperty);
                        break;
                    case Foldout fp:
                        fp.BindProperty(serializedProperty);
                        break;
                    default:
                        Debug.LogError($"[Prefab Brush] Could not bind property to field {name}");
                        break;
                }
            }

            return retValue;
        }

#if HARPIA_DEBUG

        [MenuItem("Tools/Prefab Brush/Debug/View Temporary Object")]
        public static void ViewTemporaryObject()
        {
            EditorUtility.OpenPropertyEditor(PrefabBrush.instance._temporaryTemplate);
        }
#endif

        public static bool RaycastPhysicsGUI(float maxDistance, int mask, out RaycastHit hit, Ray r)
        {
            //Raycast with GUI
            return Physics.Raycast(r.origin, r.direction, out hit, maxDistance, mask, QueryTriggerInteraction.UseGlobal);
        }

        public static Ray GetMouseGuiRay()
        {
            return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        }

        public static bool RaycastToWorldPoint(Camera c, Vector3 refPoint, int layerMask, out RaycastHit hit, PrefabBrush.RaycastMode mode, out Ray ray)
        {
            Transform transform = c.transform;
            Vector3 position = transform.position;
            ray = new Ray{ origin = position, direction = (refPoint - position).normalized };

            float maxDistance = Vector3.Distance(position, refPoint) + 0.2f;

            if (mode == PrefabBrush.RaycastMode.Mesh)
            {
                bool meshModeHit = PB_MeshRaycaster.Raycast(ray, PB_MeshBatcher.BatchForRaycast(), out MeshRaycastResult result);

                if (meshModeHit)
                {
                    hit = result.ToHitInfo();
                    return true;
                }

                bool planeModeHit = PB_FakePlaneManger.Raycast(ray, out float _, out hit);
                if (planeModeHit) return true;

                hit = new RaycastHit();
                return false;
            }

            //Check if the ray hits any collider
            bool physicsModeHit = Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
            if (physicsModeHit) return true;

            //Check if the ray hits the fake plane
            bool planeModeHit2 = PB_FakePlaneManger.Raycast(ray, out float _, out hit);
            if (planeModeHit2) return true;

            //No hit
            hit = new RaycastHit();
            return false;
        }

        public static void DrawGuideLines(RaycastHit hitInfo)
        {
            if (!PrefabBrush.instance._showWorldLines.value) return;
            Handles.zTest = CompareFunction.LessEqual;
            float distance = PrefabBrush.instance.worldLinesDistance.value;
            float size2 = 3f;

            Vector3 drawPoint = PB_SnappingManager.TryToGetPositionOnGrid(hitInfo.point);

            Vector3 p = drawPoint + Vector3.up * 0.02f;

            Handles.color = Color.red;
            Handles.DrawDottedLine(p, p + Vector3.right * distance, size2);
            Handles.DrawDottedLine(p, p - Vector3.right * distance, size2);

            Handles.color = Color.green;
            Handles.DrawDottedLine(p, p + Vector3.up * distance, size2);
            Handles.DrawDottedLine(p, p - Vector3.up * distance, size2);

            Handles.color = Color.blue;
            Handles.DrawDottedLine(p, p + Vector3.forward * distance, size2);
            Handles.DrawDottedLine(p, p - Vector3.forward * distance, size2);
        }

        public static void DrawCircle(RaycastHit hitInfo, float radius, bool validAngle)
        {
            //Draw a solid circle
            Handles.color = validAngle ? BrushColor2 : BrushColorError;
            Handles.DrawSolidDisc(hitInfo.point, hitInfo.normal, radius);

            Handles.color = BrushColor1;
            Handles.DrawWireDisc(hitInfo.point, hitInfo.normal, radius, 5f);
            Handles.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * radius / 2f, 2f);

            DrawGuideLines(hitInfo);
        }

        public static bool IsMouseOverAnySceneView()
        {
            foreach (object sceneView in SceneView.sceneViews)
            {
                if (sceneView.GetType().ToString() != "UnityEditor.SceneView") continue;
                SceneView sv = (SceneView)sceneView;
                bool isInside = sv.rootVisualElement.localBound.Contains(Event.current.mousePosition);
                if (isInside) return true;
            }

            return false;
        }

        public static Vector3 GetRandomPointInsideDisc(RaycastHit hit, float radius)
        {
            float randomAngleRad = Random.value * Mathf.PI * 2;
            float sin = Mathf.Sin(randomAngleRad);
            float cos = Mathf.Cos(randomAngleRad);
            float radiusRandom = Random.Range(0f, radius);
            Vector3 randomPoint = new Vector3(sin, 0, cos) * radiusRandom;

            //Rotate the point to match the normal
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            return hit.point + rotation * randomPoint;
        }

        public static void DrawPoint(Vector3 randomPointInsideDisc)
        {
            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(randomPointInsideDisc, Vector3.up, 0.1f);
        }

        public static bool HasAnyPrefab(Object[] objects)
        {
            if (objects.Length == 0) return false;

            foreach (Object obj in objects)
            {
                if (obj is not GameObject go) continue;
                if (!IsPrefab(go)) continue;
                return true;
            }

            return false;
        }

        public static List<GameObject> GetPrefabs(Object[] objects)
        {
            if (objects.Length == 0)
            {
#if HARPIA_DEBUG
                Debug.LogError($"{PrefabBrush.DebugLogStart} objects array is null");
#endif
                return null;
            }

            List<GameObject> foundPrefabs = new();

            foreach (Object obj in objects)
            {
                //Check if object is GameObject
                if (obj is not GameObject go) continue;

                //Check if its prefab
                if (!IsPrefab(go)) continue;

                // GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go);
                GameObject source = GetPrefabAsset(go);

                //source
                if (source == null) source = go;
                if (foundPrefabs.Contains(source)) continue;

                foundPrefabs.Add(source);
            }

            if (foundPrefabs.Count == 0)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} No prefabs selected");
            }

            return foundPrefabs;
        }

        public static bool IsPrefab(GameObject o)
        {
            if (PrefabUtility.GetPrefabAssetType(o) == PrefabAssetType.NotAPrefab) return false;
            //if (!PrefabUtility.IsAnyPrefabInstanceRoot(o)) return false;
            return true;
        }

        public static GameObject GetPrefabAsset(GameObject o)
        {
            if (!IsPrefab(o)) return o;

            bool isVariant = PrefabUtility.IsPartOfVariantPrefab(o);
            if (isVariant)
            {
                GameObject nearestPrefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(o);
                string variantPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(nearestPrefabInstanceRoot);

#if HARPIA_DEBUG
                Debug.Log($"[{nameof(PrefabBrushTool)}] Getting Prefab Variant {nearestPrefabInstanceRoot.gameObject.name} at path {variantPath}", o);
#endif
                return PrefabUtility.GetCorrespondingObjectFromSourceAtPath(nearestPrefabInstanceRoot, variantPath);
            }

            GameObject obj = PrefabUtility.GetCorrespondingObjectFromOriginalSource(o);
            if (obj != null) return obj;

            GameObject asset = PrefabUtility.GetCorrespondingObjectFromSource(o);
            if (asset != null) return asset;

            return null;
        }

        public static RaycastHit GetCenterRay()
        {
            return GetCenterRay(PrefabBrush.instance.sceneCamera, PrefabBrush.instance.GetRaycastMode(), ~1);
        }

        public static RaycastHit GetCenterRay(Camera cam, PrefabBrush.RaycastMode mode, int mask)
        {
            Transform camTransform = cam.transform;
            Vector3 camPosition = camTransform.position;
            Vector3 camForward = camTransform.forward;

            Ray ray = new(){ origin = camPosition, direction = camForward };

            RaycastHit hitInfo = new(){ normal = Vector3.up, point = camPosition.y < 0 ? camPosition + camForward * 4f : PB_FakePlaneManger.RaycastIntoY0(ray), };

            if (mode == PrefabBrush.RaycastMode.Mesh)
            {
                bool hit = PB_MeshRaycaster.Raycast(ray, PB_MeshBatcher.BatchForRaycast(), out MeshRaycastResult result);
                if (hit) hitInfo = result.ToHitInfo();
            }

            else if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo2, cam.farClipPlane, mask, QueryTriggerInteraction.UseGlobal))
            {
                hitInfo = hitInfo2;
            }

            return hitInfo;
        }

        public static void DrawDottedLines(RaycastHit hitInfo, Vector3 positionOffsetValue)
        {
            float size = 3;

            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawDottedLine(hitInfo.point, hitInfo.point + positionOffsetValue, size);

            Handles.color = Color.red;
            Handles.DrawDottedLine(hitInfo.point, hitInfo.point + new Vector3(positionOffsetValue.x, 0, 0), size);

            Handles.color = Color.green;
            Handles.DrawDottedLine(hitInfo.point, hitInfo.point + new Vector3(0, positionOffsetValue.y, 0), size);

            Handles.color = Color.blue;
            Handles.DrawDottedLine(hitInfo.point, hitInfo.point + new Vector3(0, 0, positionOffsetValue.z), size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RoundTo(float value, float multipleOf)
        {
            return Mathf.Round(value / multipleOf) * multipleOf;
        }

        public static Vector3 RoundTo(Vector3 value, float multipleOf)
        {
            return new Vector3(RoundTo(value.x, multipleOf), RoundTo(value.y, multipleOf), RoundTo(value.z, multipleOf));
        }

        public static string GetStringFormat(float newValue)
        {
            string format = newValue < 1 && newValue != 0 ? "f2" : "f1";
            return newValue.ToString(format);
        }

        public static void EnableAutoRefresh()
        {
            bool changed = false;
            foreach (object sceneView in SceneView.sceneViews)
            {
                SceneView sv = (SceneView)sceneView;
                if (!sv.hasFocus) continue;
                if (sv.sceneViewState.alwaysRefresh && sv.sceneViewState.alwaysRefreshEnabled) continue;

                if (!sv.sceneViewState.fxEnabled) sv.sceneViewState.fxEnabled = true;
                sv.sceneViewState.alwaysRefresh = true;

                changed = true;
            }

            if (changed)
            {
                Debug.Log($"{PrefabBrush.DebugLogStart} Changed Scene view alwaysRefresh to true");
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static List<Rigidbody> GetSelectedRigidbodies()
        {
            List<Rigidbody> rigidbodies = new();
            foreach (Transform transform in Selection.transforms)
            {
                if (transform.TryGetComponent(out Rigidbody rb))
                {
                    if (!PB_PhysicsSimulator.HasPhysicalCollider(rb.gameObject)) continue;
                    rigidbodies.Add(rb);
                }
            }

            return rigidbodies;
        }

        public static Vector3 MultiplyVec3(Vector3 a, Vector3 b)
        {
            float x = a.x * b.x;
            float y = a.y * b.y;
            float z = a.z * b.z;

            return new Vector3(x, y, z);
        }

        public static Quaternion FixRotation(Quaternion rot, Vector3 rotation, float multiplier = 2)
        {
            rot *= Quaternion.AngleAxis(rotation.y * multiplier, Vector3.up);
            rot *= Quaternion.AngleAxis(rotation.x * multiplier, Vector3.right);
            rot *= Quaternion.AngleAxis(rotation.z * multiplier, Vector3.forward);
            return rot;
        }

        public static Vector3 EvaluateLocalScale(Vector3 parentLossyScale, Vector3 desiredScale)
        {
            return new Vector3(desiredScale.x / parentLossyScale.x, desiredScale.y / parentLossyScale.y, desiredScale.z / parentLossyScale.z);
        }

        //Do not delete, used for the pen pressure
        public static float Lerp(Vector2 minMax, float t)
        {
            return Mathf.Lerp(minMax.x, minMax.y, t);
        }

        public static bool IsVector3Uniform(Vector3 parentLossyScale)
        {
            return Mathf.Approximately(parentLossyScale.x, parentLossyScale.y) && Mathf.Approximately(parentLossyScale.y, parentLossyScale.z);
        }

        public static List<string> GetTagsChoices()
        {
            string[] tags = InternalEditorUtility.tags;
            return tags.ToList();
        }

        public static bool IsLayerValid(int gameObjectLayer, LayerMaskField field)
        {
            return field.value == (field.value | (1 << gameObjectLayer));
        }
    }

    public static class PB_EraserManager
    {
        private static RaycastHit Hit => PrefabBrush.instance.lastHitInfo;

        public static Slider eraserRadiusSlider;
        public static Slider eraserClippingRadiusSlider;
        public static Toggle deletePhysicsOnlyToggle;
        public static LayerMaskField eraserMask;
        public static MaskField eraserTagMask;

        private static List<string> _currentEraserTagsSelected;
        private static VisualElement _eraseModePanel;

        private static SerializedObject _serializedObject => PrefabBrush.instance._serializedObject;

        public static void Init(VisualElement root)
        {
            _eraseModePanel = root.Q<VisualElement>("erase-mode");

            deletePhysicsOnlyToggle = _eraseModePanel.Find<Toggle>("erase-physics-only-toggle", PrefabBrushTemplate.DeletePhysicsOnly);
            deletePhysicsOnlyToggle.RegisterValueChangedCallback(OnDeletePhysicsToggle);

            eraserRadiusSlider = _eraseModePanel.Find<Slider>("eraser-radius-slider", PrefabBrushTemplate.EraserSize);
            eraserRadiusSlider.RegisterFocusEvents_PB();
            PB_EditorInputSliderMinMax.LoadSliderMinMaxValues(eraserRadiusSlider, "eraser-radius");
            eraserRadiusSlider.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Set Range Values", _ => PB_EditorInputSliderMinMax.Show("Update Slider Range", eraserRadiusSlider, "eraser-radius"), DropdownMenuAction.AlwaysEnabled); }));
            eraserRadiusSlider.SetActive(false);
            eraserRadiusSlider.RegisterValueChangedCallback(OnEraseRadiusSliderValueChange);

            eraserClippingRadiusSlider = _eraseModePanel.Find<Slider>("eraser-radius-clipping-slider", PrefabBrushTemplate.EraserClipping);
            eraserClippingRadiusSlider.RegisterFocusEvents_PB();
            eraserClippingRadiusSlider.SetActive(false);
            eraserClippingRadiusSlider.RegisterValueChangedCallback(OnEraseRadiusSliderValueChange);

            eraserMask = _eraseModePanel.Find<LayerMaskField>("erase-mask", PrefabBrushTemplate.EraserMask);
            eraserMask.SetValueWithoutNotify(-1);

            eraserTagMask = _eraseModePanel.Find<MaskField>("eraser-tag-mask", PrefabBrushTemplate.EraserTagMask);
            eraserTagMask.RegisterCallback<ChangeEvent<int>>(OnEraserTagMaskChanged);
            eraserTagMask.SetValueWithoutNotify(-1);

            eraserTagMask.choices = PrefabBrushTool.GetTagsChoices();
        }

        private static void OnEraseRadiusSliderValueChange(ChangeEvent<float> evt)
        {
#if HARPIA_DEBUG
            Debug.Log("[Prefab Brush] OnEraseRadiusSliderValueChange");
#endif
            PrefabBrushObject.SetClippingSize(1 - eraserClippingRadiusSlider.value, false);
        }

        private static void OnDeletePhysicsToggle(ChangeEvent<bool> evt)
        {
            if (evt.newValue == false) return;
            if (PB_PhysicsSimulator.IsUsingPhysics()) return;
            EditorUtility.DisplayDialog("Prefab Brush - Warning", "Notice that this option only works when simulate physics is on.", "Ok");
        }

        private static void OnEraserTagMaskChanged(ChangeEvent<int> evt)
        {
            _currentEraserTagsSelected = eraserTagMask.GetSelectedChoices();
#if HARPIA_DEBUG
            Debug.Log($"Eraser Tag selected {string.Join(", ", _currentEraserTagsSelected)}");
#endif
        }

        public static void Eraser(RaycastHit point, float radius)
        {
            PrefabBrushObject[] list = PrefabBrushObject.GetBrushObjects();

            for (int index = 0; index < list.Length; index++)
            {
                PrefabBrushObject brushObject = list[index];
                if (brushObject == null) continue;
                if (brushObject.erasable == false) continue;

                GameObject objectToEraser = brushObject.gameObject;

                //Check if erase mask is valid
                int mask = eraserMask.value;
                bool isInMask = mask == (mask | (1 << objectToEraser.layer));
                if (!isInMask) continue;

                //Check if the tag mask is valid 
                if (_currentEraserTagsSelected != null && !_currentEraserTagsSelected.Contains(objectToEraser.tag)) continue;

                //Check the distance
                PrefabBrushObject.RadiusBounds bounds = brushObject.GetBoundsSphere();
                if (!bounds.Intersects(point.point, eraserRadiusSlider.value, false)) continue;

                //Let's check for physics
                if (PB_PhysicsSimulator.IsUsingPhysics() && deletePhysicsOnlyToggle.value)
                {
                    if (!PB_PhysicsSimulator.HasValidRigidbody(objectToEraser)) continue;
                    if (!PB_PhysicsSimulator.HasPhysicalCollider(objectToEraser)) continue;
                }

                PB_UndoManager.DestroyAndRegister(objectToEraser);
                index--;
            }
        }

        public static void DrawCircleEraser(RaycastHit hitInfo, float radius)
        {
            Handles.color = PrefabBrush.instance.advancedSettings.eraserColor.value;
            Handles.DrawSolidDisc(hitInfo.point, hitInfo.normal, radius);

            Handles.color = new Color(0f, 0f, 0f, 0.39f);
            Handles.DrawWireDisc(hitInfo.point, hitInfo.normal, radius, 5f);
            Handles.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * radius / 2f, 2f);

            Handles.DrawWireArc(hitInfo.point, hitInfo.normal, Vector3.up, 360f, radius);

            if (!PrefabBrush.instance.showBoundsToggle.value) return;
            float d = Vector3.Distance(hitInfo.point, PrefabBrush.instance.sceneCamera.transform.position) + 2f;
            PrefabBrushObject.DrawBoundsCircles(hitInfo.point, d, eraserRadiusSlider.value, false);
        }

        public static void AddShortcuts()
        {
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.increaseRadius);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.decreaseRadius);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.changeMode);
        }

        public static void IncreaseRadius(float val)
        {
            float newValue = eraserRadiusSlider.value;
            newValue += newValue * val;
            Slider s = PrefabBrush.instance.brushRadiusSlider;
            newValue = Mathf.Clamp(newValue, s.lowValue, s.highValue);
            eraserRadiusSlider.value = newValue;
            PB_HandlesExtension.WriteTempText("Eraser Radius: " + PrefabBrushTool.GetStringFormat(newValue), Hit, Color.white);
        }

        public static bool IsOnEraseMode()
        {
            return PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Eraser;
        }

        public static void ToggleUI(bool onEraserMode)
        {
            eraserRadiusSlider.SetActive(onEraserMode);
            deletePhysicsOnlyToggle.SetActive(onEraserMode);
            _eraseModePanel.SetActive(onEraserMode);
            eraserClippingRadiusSlider.SetActive(onEraserMode);
        }
    }

    public static class PB_PrecisionModeManager
    {
        private static GameObject tempGo;
        private static GameObject prefabToPaint;
        private static GameObject lastPlacedObject;
        private static PbPrefabUI selectedPrefabUI;

        private static Vector3 currentScale = Vector3.one;
        public static float currentScaleProps = 1;
        private static Vector3 currentOffset = Vector3.zero;

        //Rotations
        private static Vector3 shortcutRotation = Vector3.zero;
        private static Vector3 currentRotationCamera = Vector3.zero;

        private static RaycastHit hit => PrefabBrush.instance.lastHitInfo;
        private static float deltaTime => PrefabBrush.deltaTime;
        private static bool alignWithNormal => PrefabBrush.instance.toggleAlignToHit.value;

        private static bool randomPrefabAfterPlace => PrefabBrush.instance.precisionModeChangePrefabToggle.value;

        public static GameObject PrefabToPaint
        {
            get
            {
                if (prefabToPaint != null) return prefabToPaint;
                SetPrefabToPaint(PrefabBrush.instance.GetRandomPrefab(true));
                return prefabToPaint;
            }
        }

        public static void DrawTemporaryPrefab(RaycastHit hitPoint, Vector3 size = default, bool lerp = true)
        {
            if (tempGo == null) return;

            PrefabAttributes props = selectedPrefabUI.customProps;

            //Scale
            Vector3 scale = props.useCustomScale ? props.GetScale() * currentScaleProps : PrefabBrushTool.MultiplyVec3(currentScale, prefabToPaint.transform.lossyScale);

            //offset
            Vector3 offset = props.useCustomOffset ? props.GetOffset() : currentOffset;

            //Rotation
            Quaternion finalRot = CalculateFinalRot(hitPoint, props);

            if (props.useCustomOffset)
            {
                finalRot *= Quaternion.Euler(-props.fixedRotation);
            }

            float lerpSpeed = PB_SnappingManager.IsUsingGridSnap() ? 40 : 20;
            finalRot = Quaternion.Lerp(tempGo.transform.rotation, finalRot, deltaTime * lerpSpeed);

            //Position
            Vector3 hitPointPos = PB_SnappingManager.TryToGetPositionOnGrid(hitPoint.point);
            Vector3 pos;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (lerp)
            {
                Vector3 currentPos = tempGo.transform.position;
                Vector3 finalPos = hitPointPos + offset;
                float distanceSqr = (finalPos - currentPos).sqrMagnitude;
                const float maxDistance = 10;

                //Check if the object is too far from the desired position
                if (distanceSqr > maxDistance * maxDistance) pos = finalPos;
                else pos = Vector3.Lerp(currentPos, finalPos, deltaTime * lerpSpeed);
            }
            else pos = hitPointPos + offset;

            tempGo.transform.position = pos;

            Transform meshObj = tempGo.transform.GetChild(0);

            PrefabBrush.PivotMode pivotMode = props.useCustomPivotMode ? props.pivotMode : PrefabBrush.instance.GetPivotMode();
            meshObj.localPosition = PrefabBrush.instance.GetLocalPosition(meshObj.gameObject, pivotMode);
            meshObj.transform.localScale = Vector3.Lerp(meshObj.transform.localScale, scale, deltaTime * lerpSpeed);
            tempGo.transform.rotation = finalRot;

            if (pivotMode == PrefabBrush.PivotMode.Prefab_Brush_Pivot_Component)
            {
                PB_AttractorManager.SetCurrentObject(meshObj.gameObject);

                meshObj.transform.position += PB_AttractorManager.GetOCurrentOffset();
            }

            PrefabBrushTool.DrawDottedLines(hitPoint, offset);
            PrefabBrushTool.DrawGuideLines(hitPoint);

            tempGo.gameObject.SetActive(true);
        }

        private static Quaternion CalculateFinalRot(RaycastHit hitPoint, PrefabAttributes props)
        {
            //Get The Main Values
            RotationTypeElement rotationField = PrefabBrush.instance.rotationField;
            PrefabBrush.RotationTypes rotationType = props.useCustomRotationMode ? props.rotationMode : rotationField.GetMode();

            Vector3 rotVec3Field = props.useCustomRotationMode ? props.GetRotation() : rotationField.GetValue(false, hitPoint);

            //Add the shortcutRotation
            rotVec3Field += shortcutRotation;

            //Change the multiplayer version
            float multiplier = 1f;
            Quaternion defaultEuler = Quaternion.identity;

            switch (rotationType)
            {
                case PrefabBrush.RotationTypes.Fixed:
                    multiplier = 1f;
                    break;
                case PrefabBrush.RotationTypes.Scene_Camera_Rotation:
                    multiplier = 2;
                    rotVec3Field += currentRotationCamera;
                    defaultEuler = Quaternion.Euler(rotVec3Field);
                    break;
                default:
                    multiplier = .5f;
                    break;
            }

            //Align with normal
            bool alingToHit = alignWithNormal;
            Quaternion finalRot = alingToHit ? PrefabBrush.GetAlignedRotation(hitPoint) : defaultEuler;

            if (alingToHit)
            {
                finalRot = PrefabBrush.GetAlignedRotation(hitPoint);

                if (!PrefabBrush.instance._alignToHitAxisX.value) finalRot.x = 0;
                if (!PrefabBrush.instance._alignToHitAxisY.value) finalRot.y = 0;
                if (!PrefabBrush.instance._alignToHitAxisZ.value) finalRot.z = 0;
            }
            else
            {
                finalRot = defaultEuler;
            }

            //Fix rotation
            finalRot = PrefabBrushTool.FixRotation(finalRot, rotVec3Field, multiplier);

            return finalRot;
        }

        public static void SetPrefabToPaint(PbPrefabUI ui)
        {
            if (ui == null)
            {
#if HARPIA_DEBUG
                Debug.LogError($"(SetPrefabToPaint) ui param is null");
#endif
                return;
            }

            Quaternion newObjectRotation = Quaternion.Euler(shortcutRotation);

            //destroy temp mesh
            if (tempGo != null)
            {
                newObjectRotation = tempGo.transform.rotation;
                Object.DestroyImmediate(tempGo);
            }

            if (lastPlacedObject != null) newObjectRotation = lastPlacedObject.transform.rotation;

            selectedPrefabUI = ui;
            selectedPrefabUI.Select();

            //Create a new tempMesh
            GameObject meshObj = Object.Instantiate(selectedPrefabUI.prefabToPaint);
            meshObj.name = "Prefab Brush Preview Mesh";
            meshObj.hideFlags = HideFlags.HideAndDontSave;

            //Create a new Temp GO
            if (tempGo != null) Object.DestroyImmediate(tempGo);
            tempGo = new GameObject{ name = "Prefab Brush Preview Object", hideFlags = HideFlags.HideAndDontSave };

            //Set the parent
            meshObj.transform.SetParent(tempGo.transform);
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localRotation = Quaternion.identity;
            meshObj.transform.localScale = currentScale;

            //Update
            tempGo.transform.position = hit.point + currentOffset;
            tempGo.transform.rotation = newObjectRotation;

            //Update physics
            foreach (Collider col in tempGo.GetComponentsInChildren<Collider>()) col.enabled = false;
            foreach (Rigidbody rb in tempGo.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;

            //Enable temp go
            if (tempGo != null)
            {
                bool boolVal = false;
                if (EditorWindow.mouseOverWindow != null) boolVal = EditorWindow.mouseOverWindow.GetType() == typeof(SceneView);
                tempGo.SetActive(boolVal);
            }

            selectedPrefabUI.customProps.Dispose();
            prefabToPaint = selectedPrefabUI.prefabToPaint;

            UpdateTransformValues();
            PB_AttractorManager.SetCurrentObject(tempGo);
        }

        public static void UpdateTransformValues()
        {
            UpdateCurrentScale();
            UpdateCurrentRotation();
            UpdateCurrentOffset();
        }

        public static void SetPrefabToPaint()
        {
#if HARPIA_DEBUG
            Debug.Log($"[Precision Mode] Setting prefab to paint...");
#endif
            lastPlacedObject = null;

            selectedPrefabUI = PbPrefabUI.GetSelectedPrefab();
            if (selectedPrefabUI != null) selectedPrefabUI.Select();

            //Deselect all UI
            foreach (PbPrefabUI prefabs in PrefabBrush.instance.currentPrefabs)
            {
                if (prefabs == selectedPrefabUI) continue;
                prefabs.Deselect();
            }

            SetPrefabToPaint(selectedPrefabUI);
        }

        public static bool PaintPrefab(Transform parent)
        {
#if HARPIA_DEBUG
            Debug.Log($"[Precision Mode] Painting prefab... prefabToPaint is null {prefabToPaint == null}");
#endif

            //Find Prefab To Paint
            if (prefabToPaint == null)
            {
                selectedPrefabUI = PbPrefabUI.GetSelectedPrefab();
                if (selectedPrefabUI != null) prefabToPaint = selectedPrefabUI.prefabToPaint;
            }

            if (prefabToPaint == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} No prefab to paint, please select one");
                return false;
            }

            //Instantiate
            lastPlacedObject = PrefabUtility.InstantiatePrefab(prefabToPaint) as GameObject;
            if (lastPlacedObject == null) lastPlacedObject = Object.Instantiate(prefabToPaint);
            if (lastPlacedObject == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} [Code PMM01] Could not instantiate a new Object, Please contact support. harpiagamesstudio@gmail.com");
                return false;
            }

            //Get Reference Object
            if (tempGo == null) SetPrefabToPaint();
            if (tempGo == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} [Code PMM02] Could Not Paint Prefab - Please contact support. harpiagamesstudio@gmail.com");
                return false;
            }

            //Transform
            Transform meshObject = tempGo.transform.GetChild(0);
            if (meshObject == null) meshObject = tempGo.transform;
            if (meshObject == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} [Code PMM03] Could Not Paint Prefab - Please contact support. harpiagamesstudio@gmail.com");
                return false;
            }

            lastPlacedObject.transform.position = meshObject.transform.position;
            lastPlacedObject.transform.rotation = meshObject.rotation;

            Vector3 localScale = meshObject.lossyScale;

            //Parenting
            if (parent != null)
            {
                localScale = PrefabBrushTool.EvaluateLocalScale(parent.lossyScale, localScale);
                lastPlacedObject.transform.SetParent(parent);
            }

            //Scale
            lastPlacedObject.transform.localScale = localScale;

            //Register
            PrefabBrush.RegisterObject(lastPlacedObject);

            //Add mesh to batcher
            PB_MeshBatcher.AddMeshToBatch(lastPlacedObject);
            PB_MeshBatcher.AddMeshToVertexBatch(lastPlacedObject);

            //Undo
            RegisterUndo();

            //Change prefab
            if (randomPrefabAfterPlace) SetPrefabToPaint(PrefabBrush.instance.GetRandomPrefab(true));

            Object.DestroyImmediate(tempGo);

#if HARPIA_DEBUG
            Debug.Log($"[Precision Mode] Prefab painted...", lastPlacedObject);
#endif
            PB_AttractorManager.Dispose();

            return true;
        }

        public static Vector3 AddToRotation(float value)
        {
            PrefabAttributes customProps = selectedPrefabUI.customProps;
            bool useCustomRotation = customProps.useCustomRotationMode;

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PrecisionModeManager)}] Add to rotation {shortcutRotation.y} | useCustomRotation {useCustomRotation}  | InputValue {value}");
#endif
            if (float.IsNaN(value) || value == 0) value = 15;
            if (float.IsNaN(shortcutRotation.y)) shortcutRotation.y = 0;

            if (useCustomRotation)
            {
                customProps.AddToRotation(value);
                return customProps.GetRotation();
            }

            PrefabBrush.RotationTypes rotationMode = PrefabBrush.instance.rotationField.GetMode();
            float rotationYValue = (shortcutRotation.y + value);

            //shortcutRotation.y = rotationMode == PrefabBrush.RotationTypes.Fixed ? PrefabBrushTool.RoundTo(rotationYValue, value) : rotationYValue;
            shortcutRotation.y = rotationYValue;

            if (tempGo != null) DrawTemporaryPrefab(hit, default, false);

            //UI
            PrefabBrush.instance.rotationField.TrySetValue(PrefabBrush.RotationTypes.Fixed, shortcutRotation);

            return tempGo.transform.rotation.eulerAngles;
        }

        public static Vector3 IncreaseSize(float p0)
        {
            PrefabAttributes customProps = selectedPrefabUI.customProps;
            if (customProps.useCustomScale)
            {
                currentScaleProps += p0;
                currentScaleProps = Mathf.Clamp(currentScaleProps, 0.1f, 1000f);
                return tempGo.transform.localScale * currentScaleProps;
            }

            currentScale += currentScale * p0;

            const float min = 0.05f;
            const float max = 1000f;

            //clamp
            currentScale = new Vector3(Mathf.Clamp(currentScale.x, min, max), Mathf.Clamp(currentScale.y, min, max), Mathf.Clamp(currentScale.z, min, max));

            PrefabBrush.instance.scaleField.TrySetValue(PrefabBrush.Vector3Mode.Fixed, currentScale);

            return currentScale;
        }

        //Called by the shortcuts with scroll
        public static void RotateCurrentObject(Vector3 worldRot)
        {
            shortcutRotation += worldRot;
            shortcutRotation = new Vector3(FixRotationValue(shortcutRotation.x), FixRotationValue(shortcutRotation.y), FixRotationValue(shortcutRotation.z));

            PrefabBrush brush = PrefabBrush.instance;
            Vector3 displayRotation = tempGo.transform.GetChild(0).rotation.eulerAngles;
            PB_HandlesExtension.WriteVector3(displayRotation, brush.lastHitInfo, "Rotation", "°");
            return;

            float FixRotationValue(float v)
            {
                //v %= 360f;
                v = PrefabBrushTool.RoundTo(v, 0.001f);
                return v;
            }
        }

        public static void AddScale(float p)
        {
            if (tempGo == null) return;
            if (tempGo.transform.childCount == 0) return;

            Vector3 newScale = currentScale + currentScale * p;

            if (newScale.x <= 0.0001f) return;
            if (newScale.y <= 0.0001f) return;
            if (newScale.z <= 0.0001f) return;

            PrefabBrush brush = PrefabBrush.instance;
            currentScale = newScale;
            PB_HandlesExtension.WriteVector3(tempGo.transform.GetChild(0).lossyScale, brush.lastHitInfo, "Scale", "");
        }

        private static void RegisterUndo()
        {
            PB_UndoManager.AddToUndo(lastPlacedObject);
            PB_UndoManager.RegisterUndo();
        }

        public static Vector3 GetNewObjectPosition()
        {
            GameObject go = lastPlacedObject == null ? tempGo : lastPlacedObject;
            return go.transform.position;
        }

        public static void DisposePrecisionMode()
        {
            if (tempGo != null)
                Object.DestroyImmediate(tempGo);

            lastPlacedObject = null;
            prefabToPaint = null;
            PbPrefabUI.Dispose(null);
            currentScaleProps = 1;

            if (tempGo != null)
            {
                foreach (Transform o in tempGo.transform)
                {
                    Object.DestroyImmediate(o.gameObject);
                }
            }
        }

        public static void HideObject()
        {
            if (tempGo != null) tempGo.gameObject.SetActive(false);
        }

        public static void ShowObject()
        {
            if (tempGo != null)
            {
                tempGo.gameObject.SetActive(true);
                return;
            }

            SetPrefabToPaint();
        }

        public static void SetRotation(Vector3 newRotation)
        {
            shortcutRotation = newRotation;
        }

        public static void AddShortcuts()
        {
            if (PrefabBrush.instance.GetPaintMode() != PrefabBrush.PaintMode.Precision) return;

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.increaseRadius);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.decreaseRadius);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.rotateRight, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.rotateLeft);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.randomRotation);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.nextPrefab, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.previousPrefab);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.normalizeSize);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.nextPivot, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.previousPivot);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.rotationXShortcut, true, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.rotationYShortcut, false, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.rotationZShortcut, false, true);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.changeScaleShortcut, true, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.yDisplacementShortcut, false, true);

            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.changeMode, true);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.exitTool);
        }

        public static Vector3 NormalizeSize()
        {
            currentScale = Vector3.one;
            if (tempGo != null) tempGo.transform.GetChild(0).localScale = currentScale;
            return currentScale;
        }

        public static void SetCurrentObjectActive(bool b)
        {
            if (tempGo == null) return;
            tempGo.SetActive(b);
        }

        public static void SetCurrentObjectPos(Vector3 point)
        {
            if (tempGo == null) return;
            tempGo.transform.position = point;
        }

        public static void UpdateCurrentOffset() => currentOffset = PrefabBrush.instance.offsetField.GetValue(true, hit);
        public static void UpdateCurrentScale() => currentScale = PrefabBrush.instance.scaleField.GetValue(true, hit);

        public static void UpdateCurrentRotation()
        {
            PrefabBrush.RotationTypes rotationMode = PrefabBrush.instance.rotationField.GetMode();
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brush - Precision mode] Updating current rotation to {rotationMode}");
#endif

            switch (rotationMode)
            {
                case PrefabBrush.RotationTypes.Fixed:
                    PB_PrecisionModeManager.ResetRotations();
                    break;

                case PrefabBrush.RotationTypes.Scene_Camera_Rotation: return;
            }

            currentRotationCamera = PrefabBrush.instance.rotationField.GetValue(true, hit);
        }

        public static void UpdateCurrentRotation(Vector3 value)
        {
            shortcutRotation = value;
        }

        public static void AdjustDistanceY(float scrollValue)
        {
            scrollValue *= PrefabBrush.instance.advancedSettings.scrollRotationSpeed.value * .05f;
            currentOffset += new Vector3(0, scrollValue, 0);
            PB_HandlesExtension.WriteVector3(currentOffset, PrefabBrush.instance.lastHitInfo, "Offset", "");
        }

        public static GameObject GetDrawingObject()
        {
            return tempGo;
        }

        public static bool IsScaleZero() => tempGo.transform.GetChild(0).localScale == Vector3.zero;

        public static bool HasObject()
        {
            if (tempGo == null) return false;
            return tempGo.transform.childCount > 0;
        }

        public static void ResetRotations()
        {
            shortcutRotation = Vector3.zero;
        }

        public static void UpdateFreeRotation(Vector2 mouseDelta)
        {
            Transform cameraTransform = PrefabBrush.instance.sceneCamera.transform;
            Transform objectTransform = tempGo.transform.GetChild(0);

            const float RotationSpeed = 1f;
            float rotSpeed = Mathf.Max(0.1f, RotationSpeed);

            Vector3 rot = mouseDelta * rotSpeed;
            rot.x = -rot.x;
            rot.z = 0;

            Vector3 relativeUp = cameraTransform.TransformDirection(Vector3.up);
            Vector3 relativeRight = cameraTransform.TransformDirection(Vector3.right);

            Vector3 objectRelativeUp = objectTransform.InverseTransformDirection(relativeUp);
            Vector3 objectRelativeRight = objectTransform.InverseTransformDirection(relativeRight);

            Quaternion rotateBy = Quaternion.AngleAxis(rot.x, objectRelativeUp) * Quaternion.AngleAxis(-rot.y, objectRelativeRight);

            // Bounds bounds = GetBounds(objectTransform);
            objectTransform.Rotate(rotateBy.eulerAngles);
            // Bounds bounds2 = GetBounds(objectTransform);
            // Vector3 offset = bounds.center - bounds2.center;
            // objectTransform.position += offset;
        }

        public static Vector3 GetCurrentRotationVec3()
        {
            return tempGo.transform.GetChild(0).rotation.eulerAngles;
        }
    }

    public static class PB_MultipleModeManager
    {
        private static RaycastHit Hit => PrefabBrush.instance.lastHitInfo;
        private static float Radius => PrefabBrush.instance.brushRadiusSlider.value;
        private static bool IsHit => PrefabBrush.instance.isRaycastHit;

        private static readonly List<PrefabBrushObject> _addedGameObjects = new List<PrefabBrushObject>();

        public static void PaintPrefabMultiple(RaycastHit raycastHit, PbPrefabUI prefabToSpawn, Ray r)
        {
            if (prefabToSpawn == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} No Prefab to spawn. Please Check your Prefabs list");
                PB_HandlesExtension.WriteTempText("Select at least 1 object on your prefab list", 2f, 5);
                return;
            }

            Vector3 hitPoint = raycastHit.point;

            //Let's check if any bounds intersects with the hit point
            if (PB_BoundsManager.BoundsIntersects(hitPoint, 0.01f, true))
                return;

            if (_addedGameObjects.Any(brushObject => brushObject != null && brushObject.meshClippingChecks && brushObject.BoundsIntersect(hitPoint, 0.01f, true)))
                return;

            //Vertex Snap Check
            if (PB_SnappingManager.IsUsingVertexSnap())
            {
                hitPoint = PB_SnappingManager.IsAnyVertexClose(raycastHit.point, r, out Vector3 vertex) ? vertex : raycastHit.point;
            }

            //Grid Snap Check
            else if (PB_SnappingManager.IsUsingGridSnap())
            {
                hitPoint = PB_SnappingManager.TryToGetPositionOnGrid(hitPoint);
            }

            //Let's check if the parent is ok
            PrefabBrush pb = PrefabBrush.instance;
            Transform parent = pb.GetParent();
            if (PrefabBrush.instance.IsParentScaleOk(parent) == false) return;

            //Instantiate prefab instance
            GameObject go = PrefabUtility.InstantiatePrefab(prefabToSpawn.prefabToPaint) as GameObject;

            if (go == null)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} Could not instantiate Prefab");
                return;
            }

            PrefabAttributes props = prefabToSpawn.customProps;

            Vector3 offset = props.useCustomOffset ? props.GetOffset() : pb.offsetField.GetValue(true, raycastHit);

            PrefabBrush.PivotMode pivotMode = props.useCustomPivotMode ? props.pivotMode : pb.GetPivotMode();

            //Rotation

            Vector3 rotVec3Field = props.useCustomRotationMode ? props.GetRotation() : pb.rotationField.GetValue(true, raycastHit);
            Quaternion rot = pb.toggleAlignToHit.value ? PrefabBrush.GetAlignedRotation(raycastHit) : Quaternion.Euler(rotVec3Field);
            rot = PrefabBrushTool.FixRotation(rot, rotVec3Field);

            //Position
            go.transform.position = hitPoint + offset + PrefabBrush.instance.GetLocalPosition(go.gameObject, pivotMode);
            go.transform.rotation = rot;

            Vector3 scale = props.useCustomScale ? props.GetScale() : PrefabBrushTool.MultiplyVec3(pb.scaleField.GetValue(true, raycastHit), prefabToSpawn.prefabToPaint.transform.lossyScale);

            if (PrefabBrush.instance.IsParentOk() && parent != null)
            {
                scale = PrefabBrushTool.EvaluateLocalScale(parent.lossyScale, scale);
                go.transform.SetParent(parent);
            }

            go.transform.localScale = scale;

            //Fix the position with the custom pivot - need to do this after scaling it
            if (pivotMode == PrefabBrush.PivotMode.Prefab_Brush_Pivot_Component)
            {
                go.transform.position += PB_AttractorManager.GetFirstPivotOffset(go);
            }

            PrefabBrushObject.RadiusBounds radiusBounds = PB_BoundsManager.GetRadiusBound(go);

            //Let's check if the new bounds intersects with any other bounds
            if (_addedGameObjects.Any(brushObject => brushObject != null && brushObject.meshClippingChecks && brushObject.BoundsIntersect(radiusBounds.pivot, radiusBounds.clippingRadiusPlacing, true)))
            {
                Object.DestroyImmediate(go);
                return;
            }

            if (PB_BoundsManager.BoundsIntersect(radiusBounds, PrefabBrush.instance.brushRadiusSlider.value))
            {
                Object.DestroyImmediate(go);
                return;
            }

            //Finally
            PrefabBrushObject pbo = PrefabBrush.RegisterObject(go);
            PB_UndoManager.AddToUndo(go);
            _addedGameObjects.Add(pbo);
            props.Dispose();
        }

        public static void OnPaintStart()
        {
            _addedGameObjects.Clear();

            if (!IsHit) return;

            if (!PrefabBrush.instance.IsParentOk()) return;

            PrefabBrush.instance.randomPointInsideDisc = PrefabBrushTool.GetRandomPointInsideDisc(Hit, Radius);
        }

        public static void SelectAllPrefabs()
        {
            foreach (PbPrefabUI prefab in PrefabBrush.instance.currentPrefabs)
                prefab.Select();
        }

        public static void DeselectAllPrefabs()
        {
            foreach (PbPrefabUI prefab in PrefabBrush.instance.currentPrefabs)
                prefab.Deselect();
        }

        public static void AddShortcuts()
        {
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.increaseRadius);
            PB_ShortcutManager.ShowShortcuts(PB_ModularShortcuts.decreaseRadius);
        }
    }

    public static class PB_HandlesExtension
    {
        private static float timer;
        private static bool useMousePos;
        private static string tempText = "";
        private static int currentPriority = -1;
        public static readonly Color errorColor = new Color(1f, 0.61f, 0.58f);

        private static Color tempColor;
        private static RaycastHit hit;
        private static GUIStyle mainStyle;

        public static void WriteAngle(RaycastHit raycastHit)
        {
            if (timer > 0) return;
            if (!PrefabBrush.instance.useAngleLimitsToggle.value) return;
            float angle = Vector3.Angle(raycastHit.normal, Vector3.up);
            bool isValid = PrefabBrush.instance.IsAngleValid(angle);
            Color color = isValid ? Color.white : errorColor;
            useMousePos = false;
            WriteText(angle.ToString("f0") + "°", color, raycastHit);
        }

        private static void WriteText(string t, Color color, RaycastHit raycastHit)
        {
            //Draw text

            if (mainStyle == null)
            {
                Texture2D transparentTexture = new Texture2D(1, 1);
                transparentTexture.SetPixel(1, 1, new Color(0f, 0f, 0f, .3f));
                transparentTexture.Apply();

                mainStyle = new GUIStyle{
                    normal ={ textColor = color, background = transparentTexture, },
                    fontSize = 14,
                    padding = new RectOffset(3, 3, 1, 1),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
            }

            mainStyle.normal.textColor = color;

            Vector3 pos;
            Camera cam = PrefabBrush.instance.sceneCamera;

            if (!useMousePos)
            {
                float distance = Vector3.Distance(cam.transform.position, raycastHit.point);

                Transform camTransform = cam.transform;
                Vector3 offset = -camTransform.up * distance * .05f;
                offset += camTransform.right * distance * .05f;
                pos = raycastHit.point + offset;
            }
            else
            {
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = cam.pixelHeight - mousePos.y - 20;
                mousePos.x += 20;
                Ray r = cam.ScreenPointToRay(mousePos);
                pos = r.origin + r.direction * 1f;
            }

            Handles.Label(pos, t, mainStyle);
        }

        public static void WriteTempText(string t, Vector3 value, RaycastHit lastHitInfo)
        {
            WriteTempText(t + " " + Vec3ToReadable(value), lastHitInfo, Color.white);
        }

        public static string Vec3ToReadable(Vector3 value)
        {
            float min = Mathf.Min(value.x, value.y, value.z);
            string format = min < 1 ? "f2" : "f1";
            string v = "X " + value.x.ToString(format);
            v += "    Y " + value.y.ToString(format);
            v += "    Z " + value.z.ToString(format);
            return v;
        }

        public static void WriteNoRaycastHit(float deltaTime)
        {
            if (PrefabBrush.instance._isMouse1Down)
            {
                return;
            }

            PB_BoundsManager.DisposeBounds();
            WriteTempTextAtMousePos("No Raycast Hit", Color.white, 1, deltaTime);
        }

        public static void WriteTempTextAtMousePos(string t, Color c, int priority, float time = .8f)
        {
            if (priority < currentPriority) return;

            tempText = t;
            timer = time;
            tempColor = c;
            useMousePos = true;
            currentPriority = priority;
        }

        public static void WriteTempText(string t, float time = 0.8f, int priority = 1)
        {
            WriteTempText(t, PrefabBrush.instance.lastHitInfo, Color.white, time, priority);
        }

        public static void WriteTempText(string t, RaycastHit lastHitInfo, Color c, float time = .8f, int currentPriorityParam = 1)
        {
            if (currentPriorityParam < currentPriority) return;

            hit = lastHitInfo;
            tempText = t;
            timer = time;
            tempColor = c;
            useMousePos = false;
            currentPriority = currentPriorityParam;
        }

        public static void Update(float deltaTime)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (timer < 0)
            {
                currentPriority = -1;
                return;
            }

            timer -= deltaTime;

            WriteText(tempText, tempColor, hit);
        }

        public static void WriteVector3(Vector3 rot, RaycastHit lastHitInfo, string name, string complement)
        {
            if (currentPriority >= 0) return;

            string text = $"{name}\n" + "X   " + PrefabBrushTool.GetStringFormat(rot.x) + $"{complement}     ";
            text += "Y   " + PrefabBrushTool.GetStringFormat(rot.y) + $"{complement}     ";
            text += "Z   " + PrefabBrushTool.GetStringFormat(rot.z) + $"{complement}";
            WriteTempText(text, lastHitInfo, Color.white);
            currentPriority = -1;
        }

        public static void WriteTextErrorTemp(string msg, RaycastHit raycastHit, float t = -1)
        {
            WriteTempText(msg, raycastHit, errorColor, t == -1 ? PrefabBrush.deltaTime : t);
        }
    }

    public static class VisualElementsExtension
    {
        public static VisualElement focusElement;

        public static void SetBackgroundColor(this VisualElement e, Color c)
        {
            if (c == Color.clear)
            {
                e.style.backgroundColor = new StyleColor(StyleKeyword.Null);
                return;
            }

            e.style.backgroundColor = c;
        }

        public static void ChangeColorOnHover(this VisualElement e, Color hoverColor)
        {
            e.RegisterCallback<MouseOverEvent>(_ => { e.SetBackgroundColor(hoverColor); });

            e.RegisterCallback<MouseOutEvent>(_ => { e.SetBackgroundColor(Color.clear); });
        }

        public static void RegisterEditorPrefs(this FloatField e, string keyStart, float defaultValue)
        {
            string key = keyStart + e.name;
            e.SetValueWithoutNotify(EditorPrefs.GetFloat(key, defaultValue));
            e.RegisterCallback<FocusOutEvent>(_ => EditorPrefs.SetFloat(key, e.value));
        }

        public static List<string> GetSelectedChoices(this MaskField field)
        {
            //Everything
            if (field.value == -1) return field.choices;

            //Get the value in bits
            List<string> selectedChoices = new List<string>();
            string binary = Convert.ToString(field.value, 2);
            for (int i = 0; i < binary.Length; i++)
            {
                char c = binary[i];
                if (c == '1') selectedChoices.Add(field.choices[binary.Length - i - 1]);
            }

            return selectedChoices;
        }

        public static void SetBackgroundTexture(this VisualElement e, Texture n)
        {
#if HARPIA_DEBUG
            if (n == null) Debug.LogError($"Texture is null for {e.name}");
#endif
            e.style.backgroundImage = (StyleBackground)n;
        }

        public static void SetBackgroundTexture(this VisualElement e, Texture2D n)
        {
#if HARPIA_DEBUG
            if (e == null) Debug.LogError($"visual element is null");
            if (n == null) Debug.LogError($"Texture2D is null for {e.name}");
#endif
            e.style.backgroundImage = n;
        }

        public static void Destroy(this VisualElement e)
        {
            if (e == null) return;
            if (e.parent == null) return;
            e.parent.Remove(e);
        }

        public static Texture GetBackgroundTexture(this VisualElement e)
        {
            Background img = e.style.backgroundImage.value;
            if (img == null) return null;
            if (img.texture != null) return img.texture;
            if (img.sprite != null) return img.sprite.texture;
            if (img.renderTexture != null) return img.renderTexture;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive(this VisualElement e, bool n)
        {
            e.style.display = n ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetVisible(this VisualElement e, bool n)
        {
            e.style.visibility = new StyleEnum<Visibility>(n ? Visibility.Visible : Visibility.Hidden);
        }

        public static bool IsActive(this VisualElement e)
        {
            return e.style.display == DisplayStyle.Flex;
        }

        public static void SetBorderColor(this VisualElement element, Color c, float width = 2f)
        {
            element.style.borderBottomWidth = width;
            element.style.borderTopWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;

            element.style.borderBottomColor = c;
            element.style.borderTopColor = c;
            element.style.borderLeftColor = c;
            element.style.borderRightColor = c;
        }

        public static void SetBorderRadius(this VisualElement element, float r)
        {
            element.style.borderBottomLeftRadius = r;
            element.style.borderBottomRightRadius = r;
            element.style.borderTopLeftRadius = r;
            element.style.borderTopRightRadius = r;
        }

        public static void SetBorderPadding(this VisualElement element, float r)
        {
            element.style.paddingBottom = r;
            element.style.paddingTop = r;
            element.style.paddingLeft = r;
            element.style.paddingRight = r;
        }

        public static void BorderColorOnHover(this VisualElement e, Color c)
        {
            e.RegisterCallback<MouseEnterEvent>(_ => e.SetBorderColor(c));
            e.RegisterCallback<MouseOutEvent>(_ => e.SetBorderColor(Color.clear));
        }

        public static void SetMinValue(this FloatField e, float minValue)
        {
            e.RegisterValueChangedCallback(evt => {
                if (evt.newValue < minValue) e.SetValueWithoutNotify(minValue);
            });
        }

        public static void RegisterFocusEvents_PB(this VisualElement element)
        {
            element.RegisterCallback<FocusInEvent>(_ => {
#if HARPIA_DEBUG
                Debug.Log($"[{nameof(VisualElementExtensions)}] RegisterFocusEvents_PB {element.name}");
#endif
                focusElement = element;
            });

            element.RegisterCallback<FocusOutEvent>(_ => {
                if (focusElement == null) return;
                if (focusElement == element) focusElement = null;
            });
        }
    }

    public static class PB_ShortcutManager
    {
        private static List<VisualElement> toAdd;
        private static VisualElement lastParent;
        private static string _previousProfile;
        private const string shortcutProfileId = "Prefab Brush Shortcuts";

        public static void ClearShortcuts()
        {
            toAdd?.Clear();
            if (lastParent != null) lastParent.Clear();
        }

        public static void ShowShortcuts(PB_ModularShortcuts.ShortcutData data, bool addSpace = false, bool useScrollWheel = false)
        {
            if (data == null) return;
            VisualElement shortCutElement = new VisualElement{ style ={ flexDirection = FlexDirection.Row, marginTop = addSpace ? 10 : 5 }, pickingMode = PickingMode.Ignore, };

            CreateShortcutLabel(data, shortCutElement);

            if (useScrollWheel) AddScrollWheel(shortCutElement);

            Label label = new Label{ text = data.shortCutName, style ={ unityTextAlign = TextAnchor.MiddleLeft }, pickingMode = PickingMode.Ignore, };

            shortCutElement.Add(label);

            toAdd ??= new List<VisualElement>();
            toAdd.Add(shortCutElement);
        }

        public static void ApplyTo(VisualElement parent)
        {
            parent.Clear();

            if (toAdd == null) return;
            if (toAdd.Count == 0) return;

            parent.Add(new Label("<b>Shortcuts</b>"));

            foreach (VisualElement element in toAdd)
            {
                parent.Add(element);
            }

            toAdd.Clear();
        }

        public static void ChangeProfile()
        {
            IShortcutManager sm = ShortcutManager.instance;
            //if (sm == null) return;

            _previousProfile = sm.activeProfileId;
            IEnumerable<string> availableProfileIds = sm.GetAvailableProfileIds();
            if (!availableProfileIds.Contains(shortcutProfileId)) sm.CreateProfile(shortcutProfileId);

            sm.activeProfileId = shortcutProfileId;

            //ID: Tools/Toggle Pivot Orientation      binding: X
            //ID: Tools/Toggle Pivot Position      binding: Z
            sm.RebindShortcut("Tools/Toggle Pivot Orientation", new ShortcutBinding());
            sm.RebindShortcut("Tools/Toggle Pivot Position", new ShortcutBinding());
            sm.RebindShortcut("Tools/Transform", new ShortcutBinding());
            sm.RebindShortcut("Tools/View", new ShortcutBinding());
            sm.RebindShortcut("Tools/Move", new ShortcutBinding());
            sm.RebindShortcut("Tools/Scale", new ShortcutBinding());
            sm.RebindShortcut("Tools/Rect", new ShortcutBinding());

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            EditorApplication.quitting -= Dispose;
            EditorApplication.quitting += Dispose;

            AssemblyReloadEvents.beforeAssemblyReload -= Dispose;
            AssemblyReloadEvents.beforeAssemblyReload += Dispose;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            Dispose();
        }

        public static void Dispose()
        {
            if (ShortcutManager.instance.activeProfileId != shortcutProfileId)
            {
#if HARPIA_DEBUG
                Debug.Log($"[Prefab brush shortcuts] - Could not dispose shortcut.. Reason {nameof(ShortcutManager.instance.activeProfileId)}" +
                          $" is the the same as {shortcutProfileId}");
#endif
                return;
            }

            if (_previousProfile == shortcutProfileId || string.IsNullOrEmpty(_previousProfile))
            {
                _previousProfile = "Default";
            }

            ShortcutManager.instance.activeProfileId = _previousProfile;
#if HARPIA_DEBUG
            Debug.Log($"Prefab brush shortcuts - Disposed. Profile reset to {_previousProfile}");
#endif
        }

        private static void AddScrollWheel(VisualElement parent)
        {
            Label plus = new Label(){ text = "+", style ={ marginRight = 5, } };

            parent.Add(plus);

            Label shortcutLabel = new Label{ text = "ScrollWheel", style ={ unityTextAlign = TextAnchor.MiddleCenter, marginRight = 5 }, pickingMode = PickingMode.Ignore };

            shortcutLabel.SetBorderColor(Color.grey);
            shortcutLabel.SetBorderRadius(5);
            shortcutLabel.SetBorderPadding(2);
            shortcutLabel.style.minWidth = 20;
            shortcutLabel.style.height = 20;

            parent.Add(shortcutLabel);
            lastParent = parent;
        }

        private static void CreateShortcutLabel(PB_ModularShortcuts.ShortcutData data, VisualElement parent)
        {
            Label shortcutLabel = new Label{ text = data.GetKeyText(), style ={ unityTextAlign = TextAnchor.MiddleCenter, marginRight = 5 }, pickingMode = PickingMode.Ignore };

            shortcutLabel.SetBorderColor(Color.grey);
            shortcutLabel.SetBorderRadius(5);
            shortcutLabel.SetBorderPadding(2);
            shortcutLabel.style.minWidth = 20;
            shortcutLabel.style.height = 20;

            parent.Add(shortcutLabel);
            lastParent = parent;
        }
    }

    public static class PB_BoundsManager
    {
        private static Bounds nextDrawBounds;

        public static void DisposeBounds()
        {
            nextDrawBounds = new Bounds();
        }

        public static void DrawBounds(Bounds bounds, bool draw)
        {
            if (draw)
            {
                if (!PrefabBrush.instance.showBoundsToggle.value) return;
                if (nextDrawBounds.size == Vector3.zero) return;
                Handles.color = Color.yellow;
                Handles.DrawWireCube(nextDrawBounds.center, nextDrawBounds.size);
                return;
            }

            nextDrawBounds = bounds;
        }

        public static Bounds GetBounds(GameObject go)
        {
            Transform obj = go.transform;

            if (obj.TryGetComponent(typeof(PrefabBrushObject), out Component com))
            {
                return ((PrefabBrushObject)com).GetBounds();
            }

            Renderer[] renderersList = obj.GetComponentsInChildren<Renderer>();
            if (renderersList.Length == 0) return new Bounds(obj.position, Vector3.zero);

            Bounds lastBounds = GetBounds(renderersList);
            return lastBounds;
        }

        private static Bounds GetBounds(Renderer[] renderers)
        {
            Bounds bounds = renderers[0].bounds;
            for (int index = 1; index < renderers.Length; index++)
            {
                Renderer renderer = renderers[index];
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        public static bool BoundsIntersect(PrefabBrushObject.RadiusBounds bounds, float radiusIntersectValue)
        {
            if (PB_PhysicsSimulator.IsUsingPhysics()) return false;

            PrefabBrushObject[] allBrushObjects = PrefabBrushObject.GetBrushObjects();

            foreach (PrefabBrushObject brushObject in allBrushObjects)
            {
                if (brushObject == null) continue;
                if (brushObject.meshClippingChecks == false) continue;
                if (brushObject.BoundsIntersect(bounds.pivot, radiusIntersectValue, true)) return true;
            }

            return false;
        }

        public static bool BoundsIntersects(Vector3 raycastHitPoint, float radius, bool isPlacing)
        {
            if (PB_PhysicsSimulator.IsUsingPhysics()) return false;

            PrefabBrushObject[] brushObjects = PrefabBrushObject.GetBrushObjects();

            foreach (PrefabBrushObject brushObject in brushObjects)
            {
                if (brushObject == null) continue;
                if (!brushObject.meshClippingChecks) continue;
                if (brushObject.BoundsIntersect(raycastHitPoint, radius, isPlacing)) return true;
            }

            return false;
        }

        public static PrefabBrushObject.RadiusBounds GetRadiusBound(GameObject go)
        {
            Bounds bounds = GetBounds(go);
            return new PrefabBrushObject.RadiusBounds(bounds, go.transform);
        }
    }

    public static class PB_TextureMaskHandler
    {
        private static VisualElement terrainPanel;
        public static Toggle useTextureMaskToggle;

        private static Dictionary<Texture, bool> allowedTextures;
        private static VisualElement texturesHolder;

        public static void Init(VisualElement root)
        {
            VisualElement terrainRoot = root.Q<VisualElement>("texture-mask-section");

            terrainPanel = terrainRoot.Q<VisualElement>("allowed-textures-section");
            useTextureMaskToggle = terrainRoot.Find<Toggle>("use-texture-mask-toggle", PrefabBrushTemplate.UseTextureMask);

            texturesHolder = terrainRoot.Q<VisualElement>("allowed-textures-holder");
            texturesHolder.Clear();

            useTextureMaskToggle.RegisterValueChangedCallback(OnUseTextureMaskToggle);

            UpdateUITerrain();
        }

        private static void OnUseTextureMaskToggle(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"toggle value changed {evt.newValue}");
#endif
            bool e = PrefabBrush.instance.GetRaycastMode() == PrefabBrush.RaycastMode.Physical_Collider;
            terrainPanel.SetActive(evt.newValue && e);
            UpdateUITerrain();
        }

        public static void UpdateUITerrain()
        {
            //Need to check paint mode - makes no sense to use texture mask in precision mode since it's only one object

            bool raycastMode = PrefabBrush.instance.GetRaycastMode() == PrefabBrush.RaycastMode.Physical_Collider;
            bool paintMode = PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Multiple;

            useTextureMaskToggle.SetActive(raycastMode && paintMode);
            terrainPanel.SetActive(raycastMode && useTextureMaskToggle.value);

            AddAllowedTexture(PB_TerrainHandler.GetTerrainTextures(), texturesHolder.childCount == 0);
        }

        private static void AddAllowedTexture(IEnumerable<Texture> textures, bool selected)
        {
            textures ??= new List<Texture>();
            foreach (Texture texture in textures) AddAllowedTexture(texture, selected);
        }

        private static void AddAllowedTexture(Texture newTexture, bool selected)
        {
            if (newTexture == null) return;

            allowedTextures ??= new Dictionary<Texture, bool>();
            if (allowedTextures.ContainsKey(newTexture)) return;

            const float notSelectedOpacity = .3f;
            const float selectedOpacity = 1f;

            const int size = 35;
            VisualElement textureElement = new VisualElement{
                style ={
                    width = size,
                    height = size,
                    marginLeft = 2,
                    marginRight = 2,
                    opacity = selected ? selectedOpacity : notSelectedOpacity,
                }
            };
            textureElement.SetBorderRadius(1f);
            textureElement.SetBorderColor(selected ? Color.yellow : Color.clear);

            //hover
            textureElement.SetBackgroundTexture(newTexture);

            //Click event
            textureElement.RegisterCallback<ClickEvent>(e => {
                VisualElement element = e.target as VisualElement;
                bool isSelected = element.style.opacity.value == 1f;
                isSelected = !isSelected;
                element.style.opacity = isSelected ? selectedOpacity : notSelectedOpacity;
                element.SetBorderRadius(1f);
                element.SetBorderColor(isSelected ? Color.yellow : Color.clear);

                //tooltip
                element.tooltip = newTexture.name + "\n";
                element.tooltip += isSelected ? "Selected" : "Not Selected";
                allowedTextures[newTexture] = isSelected;

#if HARPIA_DEBUG
                Debug.Log($"[Prefab Brush] Texture clicked {newTexture.name} | allowedTexturesLenght {allowedTextures.Count}");
#endif
            });

            RegisterContextMenu(textureElement);
            texturesHolder.Add(textureElement);
            allowedTextures.Add(newTexture, selected);
        }

        private static void RegisterContextMenu(VisualElement textureElement)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Reveal In Project"), false, () => {
                Texture2D texture = textureElement.style.backgroundImage.value.texture;
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = texture;
            });

            textureElement.RegisterCallback<ContextClickEvent>(_ => { menu.ShowAsContext(); });
        }

        public static bool IsTextureValid(Vector3 hitPoint)
        {
            if (!useTextureMaskToggle.value) return true;
            if (allowedTextures == null) return true;
            if (PB_TerrainHandler.HasAnyTerrain() == false) return true;

            List<Texture> texturesUnderMouse = PB_TerrainHandler.GetTexturesAtPosition(hitPoint);
            if (texturesUnderMouse.Count == 0) return true;
#if HARPIA_DEBUG
            //  string allNames = string.Join(", ", textures.Select(x => x.name));
            //  Debug.Log($"[Prefab Brush] Texture Under mouse {textures.Count} {allNames}");
#endif

            foreach (Texture textureUnderMouse in texturesUnderMouse)
            {
                bool hasKey = allowedTextures.TryGetValue(textureUnderMouse, out bool selected);
                if (!hasKey) continue;
                if (selected) return true;
            }

            return false;
        }

        public static void Dispose()
        {
            texturesHolder.Clear();
            allowedTextures?.Clear();
        }

        public static void AddTextures(List<Texture> textures, bool selected)
        {
            if (textures == null) return;
            if (textures.Count == 0) return;
            foreach (Texture texture in textures) AddAllowedTexture(texture, selected);
        }

        public static List<Texture> GetAllowedTextures()
        {
            if (allowedTextures == null) return null;
            return allowedTextures.Where(x => x.Value).Select(x => x.Key).ToList();
        }
    }

    public static class PB_FileManager
    {
        public static bool IsInsideAssetFolder(string path)
        {
            //log path
#if HARPIA_DEBUG
            Debug.Log("IsInsideAssetFolder - path " + path);
#endif
            return path.Contains(Application.dataPath);
        }

        public static void RemoveFileNamesFromPath(ref string lastPath)
        {
            lastPath = lastPath.Replace(GetFileName(lastPath), "");
        }

        private static string GetFileName(string path)
        {
            string fileName = Path.GetFileName(path);
            return fileName;
        }
    }

    public static class PB_PhysicsSimulator
    {
        //https://forum.unity.com/threads/separating-physics-scenes.597697/

        private static bool shouldSimulate;
        public static Slider physicsStepSlider;
        public static DefaultEnumField affectObjectsEnum;
        public static DefaultEnumField simulationModeEnum;

        public static Toggle useCustomLayerToggle;
        public static LayerField customLayerField;

        private static Toggle simulatePhysicsToggle;
#if UNITY_2022_2_OR_NEWER
        private static UnityEngine.SimulationMode defaultPhysicsSimulationMode;
#endif

        private static bool isPhysicsActive;
        private static VisualElement physicsStatus;
        public static Vector3ModeElement impulseFieldVec3;
        public static Slider impulseForceSlider;
        public static Button fixedButton;
        public static Button maxButton;
        public static Button minButton;
        private static double lastUpdateTime;
        private static float deltaTimeUpdate;
        private static Label noRigidbodyLabel;
        private static Toggle _impulseToggle;
        private static VisualElement _impulseSection;
        private static Label collideWithLabel;

        private static void OnSimulationModeChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] Simulation mode changed {evt.newValue}");
#endif
            Dispose();
        }

        public static AffectMode GetAffectMode()
        {
            if (affectObjectsEnum == null) return AffectMode.Only_New_Spawned_Objects;
            return affectObjectsEnum.value as AffectMode? ?? AffectMode.All;
        }

        public static Vector3 GetImpulseForce() => impulseFieldVec3.useToggle.value ? impulseFieldVec3.GetValue(true, PrefabBrush.instance.lastHitInfo) : Vector3.zero;

        public enum SimulationMode
        {
            Auto,
            Step,
        }

        public enum AffectMode
        {
            Only_New_Spawned_Objects,

            //Only_Selected,
            All,
        }

        public static void Init(VisualElement root)
        {
            if (Application.isPlaying) return;

#if UNITY_2022_2_OR_NEWER
            defaultPhysicsSimulationMode = Physics.simulationMode;
#else
            isPhysicsActive = Physics.autoSimulation;
#endif

            physicsStatus = root.Q<VisualElement>("status-physics-section");
            physicsStatus.SetActive(false);
            physicsStatus.SetBackgroundColor(PrefabBrush._styleBackgroundColorGreen);
            physicsStatus.Q<Button>("stop-physics-button").RegisterCallback<ClickEvent>(OnStopPhysicsButton);

            Foldout physicsFoldout = root.Q<Foldout>("physics-secttion");

            simulatePhysicsToggle = physicsFoldout.Find<Toggle>("simulate-physics-toggle", "");
            simulatePhysicsToggle.text = "Simulate Physics";
            simulatePhysicsToggle.RegisterValueChangedCallback(OnSimulateToggle);

            useCustomLayerToggle = physicsFoldout.Find<Toggle>("physics-use-custom-toggle", PrefabBrushTemplate.PhysicsUseCustomLayer);
            useCustomLayerToggle.RegisterValueChangedCallback(OnUseCustomLayerToggle);

            customLayerField = physicsFoldout.Find<LayerField>("physics-use-custom-layer", PrefabBrushTemplate.PhysicsCustomLayer);
            customLayerField.SetActive(useCustomLayerToggle.value);
            customLayerField.RegisterValueChangedCallback(OnCustomLayerChanged);

            collideWithLabel = physicsFoldout.Q<Label>("physics-collider-with-text");
            collideWithLabel.text = "";
            collideWithLabel.SetActive(useCustomLayerToggle.value);
            if (useCustomLayerToggle.value) OnCustomLayerChanged(null);

            physicsFoldout.Q<Button>("physics-reset-bodies").RegisterCallback<ClickEvent>(OnResetBodiesClick);

            fixedButton = physicsFoldout.Q<Button>("align-with-scene-view-fixed-impulse");
            fixedButton.RegisterFocusEvents_PB();

            maxButton = physicsFoldout.Q<Button>("align-with-scene-view-max-impulse");
            maxButton.RegisterFocusEvents_PB();

            minButton = physicsFoldout.Q<Button>("align-with-scene-view-min-impulse");
            minButton.RegisterFocusEvents_PB();

            //Impulse
            _impulseSection = physicsFoldout.Q<VisualElement>("add-force");
            _impulseToggle = physicsFoldout.Find<Toggle>("custom-props-impulse-toggle", PrefabBrushTemplate.UseImpulse);
            _impulseToggle.RegisterValueChangedCallback(OnImpulseToggle);

            impulseFieldVec3 = new Vector3ModeElement(root, "custom-props-impulse-type", "custom-props-impulse-fixed", "custom-props-impulse-max", "custom-props-impulse-min", null, false, PrefabBrushTemplate.ImpulseMode, PrefabBrushTemplate.ImpulseFixed, PrefabBrushTemplate.ImpulseMax, PrefabBrushTemplate.ImpulseMin, null, null, null);
            impulseFieldVec3.useToggle.RegisterValueChangedCallback(OnImpulseChanged);
            impulseFieldVec3.RegisterFocusEvents();
            impulseFieldVec3.SetButtons(fixedButton, maxButton, minButton);

            impulseForceSlider = physicsFoldout.Find<Slider>("impulse-force-slider", PrefabBrushTemplate.PhysicsImpulseForce);
            impulseForceSlider.SetActive(_impulseToggle.value);
            PB_EditorInputSliderMinMax.LoadSliderMinMaxValues(impulseForceSlider, "pb-impulse-force");
            impulseForceSlider.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Set Range Values", _ => PB_EditorInputSliderMinMax.Show("Update Slider Range", impulseForceSlider, "pb-impulse-force"), DropdownMenuAction.AlwaysEnabled); }));

            fixedButton.RegisterCallback<ClickEvent>(_ => OnAlignWithImpulse(PrefabBrush.instance.sceneCamera, impulseFieldVec3.fixedField));
            maxButton.RegisterCallback<ClickEvent>(_ => OnAlignWithImpulse(PrefabBrush.instance.sceneCamera, impulseFieldVec3.maxField));
            minButton.RegisterCallback<ClickEvent>(_ => OnAlignWithImpulse(PrefabBrush.instance.sceneCamera, impulseFieldVec3.minField));

            //step
            physicsFoldout.Q<Button>("step-physics-button").RegisterCallback<ClickEvent>(OnStepPhysicsClick);

            //Physics settings
            physicsFoldout.Q<Button>("btn-physics-settings").RegisterCallback<ClickEvent>(_ => SettingsService.OpenProjectSettings("Project/Physics"));

            //Info
            simulationModeEnum = physicsFoldout.Q<DefaultEnumField>("physics-simulation-mode");
            simulationModeEnum.Init(SimulationMode.Auto);
            simulationModeEnum.BindProperty(PrefabBrush.instance._serializedObject.FindProperty(PrefabBrushTemplate.PhysicsSimulationMode));
            simulationModeEnum.RegisterValueChangedCallback(OnSimulationModeChanged);

            noRigidbodyLabel = physicsFoldout.Q<Label>("no-rigidbody-selected");

            affectObjectsEnum = physicsFoldout.Q<DefaultEnumField>("physics-affect-objects");
            affectObjectsEnum.Init(AffectMode.All);
            affectObjectsEnum.BindProperty(PrefabBrush.instance._serializedObject.FindProperty(PrefabBrushTemplate.PhysicsAffectMode));
            affectObjectsEnum.RegisterValueChangedCallback(OnAffectsObjectsChanged);
            OnAffectsObjectsChanged(null);

            physicsStepSlider = physicsFoldout.Find<Slider>("physics-step-slider", PrefabBrushTemplate.PhysicsStepValue);
        }

        private static void OnCustomLayerChanged(ChangeEvent<int> evt)
        {
            int value = customLayerField.value;
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] Custom Layer Changed to {value}");
#endif
            string collideWith = "Collide With Layers: ";
            bool allLayers = true;
            bool none = true;

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName)) continue;

                bool col = !Physics.GetIgnoreLayerCollision(value, i);
                if (!col)
                {
                    allLayers = false;
                    continue;
                }

                none = false;
                collideWith += layerName + ", ";
            }

            collideWith = collideWith.Remove(collideWith.Length - 2);
            collideWith = allLayers ? "Collide With All Layers" : collideWith;
            collideWith = none ? "Collide With None" : collideWith;

            collideWithLabel.text = collideWith;
            collideWithLabel.tooltip = collideWith;
        }

        private static void OnStopPhysicsButton(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] OnStopPhysicsButton");
#endif
            Dispose();
            simulatePhysicsToggle.value = false;
        }

        private static void OnImpulseToggle(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] OnImpulseToggle {evt.newValue}");
#endif
            UpdateUIPhysics();
        }

        private static void OnUseCustomLayerToggle(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"OnUseCustomLayerToggle {evt.newValue}");
#endif
            customLayerField.SetActive(evt.newValue);
            collideWithLabel.SetActive(evt.newValue);
            if (evt.newValue)
            {
                OnCustomLayerChanged(null);
            }
        }

        /// <summary>
        /// Checks if the prefab has a non trigger collider
        /// </summary>
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

        /// <summary>
        /// Check If the Object has a non Kinematic rigidbody
        /// </summary>
        public static bool HasValidRigidbody(GameObject o)
        {
            if (o == null) return false;

            //Copy this method to prefabBrushObject
            foreach (Rigidbody rigidbody in o.GetComponentsInChildren<Rigidbody>())
                if (rigidbody.isKinematic == false)
                    return true;

            return false;
        }

        private static void OnStepPhysicsClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PrefabBrush)}] OnStepPhysicsClick");
#endif
            if (GetSimulationMode() != SimulationMode.Step) return;

            ChangePhysicsMode(true);
            IEnumerable<Rigidbody> all = GetAllBodies();
            PB_UndoManager.RegisterUndoTransforms(all, "Before Step Physics Simulation");
            Physics.Simulate(physicsStepSlider.value);
            shouldSimulate = false;
        }

        private static void OnImpulseChanged(ChangeEvent<bool> evt)
        {
            // if (evt.newValue == false) return;
            // if (PrefabBrush.instance.currentPrefabs.Count == 0) return;
            // if (PrefabBrush.instance.currentPrefabs.Any(e => PB_PhysicsSimulator.HasValidRigidbody(e.prefabToPaint))) return;
            // PrefabBrush.DisplayError("None of the selected prefabs have a Rigidbody component. You will not be able to use the impulse feature.");
        }

        private static void OnAlignWithImpulse(Camera cam, Vector3Field field)
        {
            if (cam == null)
            {
                PrefabBrush.DisplayError("No scene view found, please add a scene view to your layout.");
                return;
            }

            //round to 3 decimals
            const float round = 1000f;
            Vector3 forward = cam.transform.forward;
            forward.x = Mathf.Round(forward.x * round) / round;
            forward.y = Mathf.Round(forward.y * round) / round;
            forward.z = Mathf.Round(forward.z * round) / round;

            field.SetValueWithoutNotify(forward);
        }

        private static void OnAffectsObjectsChanged(ChangeEvent<Enum> evt)
        {
            UpdateRigidbodiesObjects();
            UpdateUIPhysics();
        }

        public static void UpdateRigidbodiesObjects()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] Update Rigidbodies Objects - Is using the tool {PrefabBrushTool.isUsingTool}");
#endif
            //This dosent seems to work in untiy 6

            //all rb in scene
            RigidbodyData.ClearList();
            AffectMode mode = GetAffectMode();

            if (mode == AffectMode.All)
            {
                IEnumerable<Rigidbody> all = GetAllBodies().ToList();
                foreach (Rigidbody rigidbody in all) RigidbodyData.AddRigidbody(rigidbody, false);

                //Create an undo with all the transforms positions
                PB_UndoManager.RegisterUndoTransforms(all, "Before Physics Simulation");

                UpdateUIPhysics();
                return;
            }

            if (mode == AffectMode.Only_New_Spawned_Objects)
            {
                IEnumerable<Rigidbody> allRBs = GetAllBodies().ToList();
                foreach (Rigidbody rB in allRBs) RigidbodyData.AddRigidbody(rB, true);
                PB_UndoManager.RegisterUndoTransforms(allRBs, "Before Physics Simulation");
                UpdateUIPhysics();
                return;
            }

            foreach (Rigidbody rigidbody in GetAllBodies()) RigidbodyData.AddRigidbody(rigidbody, true);

            UpdateUIPhysics();
        }

        private static IEnumerable<Rigidbody> GetAllBodies()
        {
#if UNITY_2022_2_OR_NEWER
            IEnumerable<Rigidbody> allRBs = Object.FindObjectsByType<Rigidbody>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            IEnumerable<Rigidbody> allRBs = Object.FindObjectsOfType<Rigidbody>();
#endif

            return allRBs;
        }

        private static void OnResetBodiesClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log("OnResetBodiesClick");
#endif

#if UNITY_2022_2_OR_NEWER
            Rigidbody[] list = Object.FindObjectsByType<Rigidbody>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            Rigidbody[] list = Object.FindObjectsOfType<Rigidbody>();
#endif

            foreach (Rigidbody rb in list)
            {
                if (rb.isKinematic) continue;

                //If any compilation error occurs, try changing velocity to linearVelocity or vice versa
#if !UNITY_6000_0_OR_NEWER
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
#else
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
#endif
            }
        }

        private static void OnSimulateToggle(ChangeEvent<bool> changeEvent)
        {
            shouldSimulate = (changeEvent == null ? true : changeEvent.newValue);
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PhysicsSimulator)}] On simulate toggle {shouldSimulate}");
#endif

            if (Application.isPlaying)
            {
                PrefabBrush.DisplayError("Can't simulate physics while in play mode");
                return;
            }

            if (shouldSimulate)
            {
                ChangePhysicsMode(true);
                bool anyPrefabWithPhysics = PrefabBrush.instance.HasAnyPrefabsWithPhysics();
                noRigidbodyLabel.SetActive(!anyPrefabWithPhysics);
                lastUpdateTime = EditorApplication.timeSinceStartup;
                deltaTimeUpdate = 0;
                UpdateRigidbodiesObjects();
            }
            else
            {
                ChangePhysicsMode(false);
                noRigidbodyLabel.SetActive(false);
                RigidbodyData.ClearList();
            }

            UpdateUIPhysics();
        }

        public static void ChangePhysicsMode(bool isScript)
        {
#if UNITY_2022_2_OR_NEWER
            Physics.simulationMode = isScript ? UnityEngine.SimulationMode.Script : defaultPhysicsSimulationMode;
#else
                        Physics.autoSimulation = isScript? true : isPhysicsActive;
#endif
        }

        public static void UpdateUIPhysics()
        {
            if (noRigidbodyLabel == null) return;
            SimulationMode simulationMode = GetSimulationMode();

            bool isOnEraseMode = PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Eraser;

            //Top
            physicsStatus?.SetActive(shouldSimulate);

            simulatePhysicsToggle.SetValueWithoutNotify(IsUsingPhysics());

            //Main
            physicsStepSlider?.parent.SetActive(simulationMode == SimulationMode.Step);
            simulatePhysicsToggle.SetActive(simulationMode == SimulationMode.Auto);
            noRigidbodyLabel.SetActive(simulatePhysicsToggle.value && !PrefabBrush.instance.HasAnyPrefabsWithPhysics() && !isOnEraseMode);

            //Impulse
            bool allowImpulse = simulationMode == SimulationMode.Auto;
            _impulseSection.SetActive(allowImpulse);
            _impulseToggle.SetActive(allowImpulse);
            impulseFieldVec3.SetActive(_impulseToggle.value);
            impulseForceSlider.SetActive(_impulseToggle.value);

            useCustomLayerToggle.SetActive(!isOnEraseMode);
            customLayerField.SetActive(useCustomLayerToggle.value && useCustomLayerToggle.IsActive());

            //Margin top
            PrefabBrush.instance.spacer.style.marginTop = !shouldSimulate ? 55 : 90;
        }

        public static void Dispose(bool stopSimulation = true)
        {
            if (Application.isPlaying) return;

#if HARPIA_DEBUG
            Debug.Log($"[Physics] Dispose  - stopSimulation {stopSimulation}");
#endif

            if (stopSimulation) shouldSimulate = false;

            RigidbodyData.ClearList();
#if UNITY_2022_2_OR_NEWER
            Physics.simulationMode = defaultPhysicsSimulationMode;
#else
            Physics.autoSimulation = isPhysicsActive;
#endif
            if (Selection.transforms.Length > 0) UpdateRigidbodiesObjects();

            //Update UI
            UpdateUIPhysics();
        }

        public static void Update(double deltaTime)
        {
            if (!shouldSimulate) return;
            if (GetSimulationMode() != SimulationMode.Auto)
            {
                //Only unity 6
                //Physics.Simulate((float)deltaTime);
                return;
            }

            deltaTimeUpdate = (float)(EditorApplication.timeSinceStartup - lastUpdateTime);
            lastUpdateTime = EditorApplication.timeSinceStartup;
            if (deltaTimeUpdate > .5f) return;

            ChangePhysicsMode(true);
            Physics.Simulate(deltaTimeUpdate);
        }

        [Serializable]
        public struct RigidbodyData
        {
            [SerializeField]
            private Rigidbody rb;

            [SerializeField]
            private bool IsKinematic;

            [SerializeField]
            private CollisionDetectionMode CollisionMode;

            [SerializeField]
            private int originalLayer;

            public static List<RigidbodyData> allRigidbodies = new List<RigidbodyData>();
            public static int Count => allRigidbodies.Count;

            public static void AddRigidbody(Rigidbody body, bool kinematic)
            {
                if (body.isKinematic) return;

                bool hasPhysicalCollider = HasPhysicalCollider(body.gameObject);

                allRigidbodies.Add(new RigidbodyData(body));

                SetAllBodiesLayer(body.gameObject, GetPhysicalLayer(body.gameObject));
                body.isKinematic = kinematic || !hasPhysicalCollider;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            private static void SetAllBodiesLayer(GameObject target, int layer)
            {
                if (layer is < 0 or > 31) return;

                Rigidbody[] bodies = target.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody body in bodies)
                {
                    body.gameObject.layer = layer;
                }
            }

            private RigidbodyData(Rigidbody body)
            {
                rb = body;
                CollisionMode = body.collisionDetectionMode;
                IsKinematic = body.isKinematic;
                originalLayer = body.gameObject.layer;
            }

            public void RestoreRigidBody()
            {
                if (rb == null)
                {
                    allRigidbodies.Remove(this);
                    return;
                }

                rb.collisionDetectionMode = CollisionMode;
                rb.isKinematic = IsKinematic;
                SetAllBodiesLayer(rb.gameObject, originalLayer);

                EditorUtility.SetDirty(rb);
                allRigidbodies.Remove(this);
            }

            public static void ClearList()
            {
                while (allRigidbodies.Count > 0) allRigidbodies[0].RestoreRigidBody();
            }
        }

        public static SimulationMode GetSimulationMode()
        {
            return simulationModeEnum.value as SimulationMode? ?? SimulationMode.Auto;
        }

        public static void ApplyImpulse(GameObject go)
        {
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            if (rbs.Length == 0) return;

            //Impulse
            bool addImpulse = _impulseToggle.value && simulatePhysicsToggle.value;
            float force = impulseForceSlider.value;

            //Impulse checks
            if (addImpulse)
            {
                if (force == 0)
                    PB_HandlesExtension.WriteTextErrorTemp("Impulse force is 0", PrefabBrush.instance.lastHitInfo, .8f);

                else if (impulseFieldVec3.GetValue(true, PrefabBrush.instance.lastHitInfo) == Vector3.zero)
                    PB_HandlesExtension.WriteTextErrorTemp("Impulse direction is (0,0,0)", PrefabBrush.instance.lastHitInfo, .8f);
            }

            foreach (Rigidbody rb in rbs)
            {
                RigidbodyData.AddRigidbody(rb, false);
                if (addImpulse) rb.AddForce(impulseFieldVec3.GetValue(true, PrefabBrush.instance.lastHitInfo).normalized * force, ForceMode.Impulse);
            }
        }

        public static void DrawArrowHandles()
        {
            if (!simulatePhysicsToggle.value) return;
            if (!_impulseToggle.value) return;
            if (!PrefabBrush.instance.isRaycastHit) return;
            if (GetSimulationMode() == SimulationMode.Step) return;

            Vector3 hit = PrefabBrush.instance.lastHitInfo.point + Vector3.up;

            Handles.color = Color.cyan;
            if (impulseFieldVec3.GetMode() == PrefabBrush.Vector3Mode.Fixed)
            {
                Quaternion rot = Quaternion.LookRotation(impulseFieldVec3.fixedField.value);
                Handles.ArrowHandleCap(-1, hit, rot, 1, EventType.Repaint);
                return;
            }

            Handles.color = Color.cyan;
            Handles.ArrowHandleCap(-1, hit, Quaternion.LookRotation(impulseFieldVec3.minField.value), 1, EventType.Repaint);
        }

        public static void DrawArrowHandles(Vector3 val, Vector3 hitInfoPoint)
        {
            Handles.color = Color.cyan;
            Vector3 hit = hitInfoPoint + Vector3.up;
            Handles.ArrowHandleCap(-1, hit, Quaternion.LookRotation(val), 1, EventType.Repaint);
        }

        public static bool IsUsingPhysics()
        {
            if (simulatePhysicsToggle == null) return false;
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brush] IsUsingPhysics {simulatePhysicsToggle.value}");
#endif
            if (Physics.simulationMode != UnityEngine.SimulationMode.Script) return false;
            return simulatePhysicsToggle.value;
        }

        public static void ShowDebugWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/Analysis/Physics Debugger");
        }

        public static int GetPhysicalLayer(GameObject refGO)
        {
            PrefabBrushObject prefabBrushComponent = refGO.GetComponent<PrefabBrushObject>();

            //Let's check if the prefab has a custom layer
            if (prefabBrushComponent != null && prefabBrushComponent.customLayer != -1)
                return prefabBrushComponent.customLayer;

            if (useCustomLayerToggle == null) return -1;

            //If the prefab does not have a custom layer, we will use the custom layer from the physics simulator
            return useCustomLayerToggle.value ? customLayerField.value : -1;
        }

        public static void DrawSphereHandles(float value, Vector3 hitInfoPoint)
        {
            //draw a sphere
            Handles.color = Color.cyan;
            Handles.SphereHandleCap(-1, hitInfoPoint, Quaternion.identity, value, EventType.Repaint);
        }

        public static void TryToStartPhysics()
        {
            if (simulatePhysicsToggle.value == false) return;
            OnSimulateToggle(null);
        }
    }

    public static class PB_MeshBatcher
    {
        private static Mesh combinedMeshRaycast;
        private static Mesh combinedMeshVertexFinder;
        private static List<MeshFilter> validList;
        private static Vector3 chacedCameraPos;

        const float maxDistance = 100;
        const float maxDistanceSqr = maxDistance * maxDistance;

        //Used for vertex snapping
        public static Mesh BatchForVertex()
        {
            if (combinedMeshVertexFinder != null && !CameraMovedTooFar())
            {
                return combinedMeshVertexFinder;
            }

            //Get the valid objects
            if (NeedToFindMeshesAgain())
            {
                validList = GetMeshFilters().Where(IsValidForVertexFinding).ToList();
            }

            //Distance check
            chacedCameraPos = PrefabBrush.instance.sceneCamera.transform.position;
            MeshFilter[] distanceList = FilterByDistance(validList);

            //batch
            CombineInstance[] combine = new CombineInstance[distanceList.Length];
            for (int i = 0; i < distanceList.Length; i++)
            {
                combine[i].mesh = distanceList[i].sharedMesh;
                combine[i].transform = distanceList[i].transform.localToWorldMatrix;
            }

            //create new mesh
            combinedMeshVertexFinder = new Mesh{ indexFormat = IndexFormat.UInt32 };
            combinedMeshVertexFinder.CombineMeshes(combine);

#if HARPIA_DEBUG
            if (distanceList.Length == 0) Debug.LogError($"Error batcher list is empty");
            Debug.Log($"[PB Mesh Batcher] Generating new mesh {distanceList.Length}");
#endif

            return combinedMeshVertexFinder;
        }

        private static bool NeedToFindMeshesAgain()
        {
            if (validList == null) return true;
            if (validList.Count == 0) return true;

            return false;
        }

        private static bool CameraMovedTooFar()
        {
            Vector3 camPos = PrefabBrush.instance.sceneCamera.transform.position;
            const float minDistance = maxDistance / 2f;
            const float minDistanceSQR = minDistance * minDistance;
            float cameraOffset = (camPos - chacedCameraPos).sqrMagnitude;
            return cameraOffset > minDistanceSQR;
        }

        private static MeshFilter[] FilterByDistance(List<MeshFilter> meshFilters)
        {
            const int objectLimits = 1000;
            Vector3 camPos = PrefabBrush.instance.sceneCamera.transform.position;
            IOrderedEnumerable<MeshFilter> orderedList = meshFilters.OrderBy(x => (x.transform.position - camPos).sqrMagnitude);

            MeshFilter[] distanceList = new MeshFilter[objectLimits];

            int index = 0;
            foreach (MeshFilter filter in orderedList)
            {
                distanceList[index] = filter;
                index++;
                if (index >= distanceList.Length) break;
            }

            //Resize the array
            Array.Resize(ref distanceList, index);

            return distanceList;
        }

        private static bool IsValidForBoth(MeshFilter meshFilter, List<string> allowedTags, LayerMaskField layerMaskField)
        {
            if (meshFilter.sharedMesh == null) return false;

            //Layer Check
            if (!PrefabBrushTool.IsLayerValid(meshFilter.gameObject.layer, layerMaskField))
            {
                return false;
            }

            //Tag check
            if (!PrefabBrush.instance.IsTagAllowed(meshFilter.gameObject.tag, allowedTags))
            {
                string tag = meshFilter.gameObject.tag;
                PB_HandlesExtension.WriteTextErrorTemp($"Tag not allowed: {tag}", PrefabBrush.instance.lastHitInfo);
                return false;
            }

            //Renderer check
            if (!meshFilter.gameObject.TryGetComponent(typeof(Renderer), out Component r)) return false;
            if (r is SkinnedMeshRenderer) return false;

            MeshRenderer renderer = (MeshRenderer)r;
            if (renderer.enabled == false) return false;
            return true;
        }

        private static bool IsValidForVertexFinding(MeshFilter meshFilter)
        {
            return IsValidForBoth(meshFilter, PB_SnappingManager.GetAllowedTags(), PB_SnappingManager._vertexSnappingLayersField);
        }

        //Used by vertex placing
        public static Mesh BatchForRaycast()
        {
            if (combinedMeshRaycast != null && !CameraMovedTooFar())
            {
                return combinedMeshRaycast;
            }

            //get all mesh filter
            if (NeedToFindMeshesAgain())
            {
                validList = GetMeshFilters().Where(IsValidForMeshRaycast).ToList();
#if HARPIA_DEBUG
                Debug.Log($"[Prefab Brush] Rebuilding valid list {validList.Count}");
#endif
            }

            //distance check
            chacedCameraPos = PrefabBrush.instance.sceneCamera.transform.position;
            MeshFilter[] distanceList = FilterByDistance(validList);

            //batch
            CombineInstance[] combine = new CombineInstance[distanceList.Length];

            for (int i = 0; i < distanceList.Length; i++)
            {
                combine[i].mesh = distanceList[i].sharedMesh;
                combine[i].transform = distanceList[i].transform.localToWorldMatrix;
            }

            //create new mesh
            if (combinedMeshRaycast != null) Object.DestroyImmediate(combinedMeshRaycast);
            combinedMeshRaycast = new Mesh{ indexFormat = IndexFormat.UInt32 };
            combinedMeshRaycast.CombineMeshes(combine);
            PB_MeshRaycaster.UpdateMesh(combinedMeshRaycast);

#if HARPIA_DEBUG
            if (distanceList.Length == 0) Debug.LogError($"Error batcher list is empty");
            Debug.Log($"[PB Mesh Batcher] Generating new mesh - {distanceList.Length} objects");
#endif

            return combinedMeshRaycast;
        }

        private static MeshFilter[] GetMeshFilters()
        {
#if UNITY_2022_2_OR_NEWER
            return Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<MeshFilter>(false);
#endif
        }

        private static bool IsValidForMeshRaycast(MeshFilter meshFilter)
        {
            return IsValidForBoth(meshFilter, PrefabBrush.instance._currentTagsSelected, PrefabBrush.instance.layerMaskField);
        }

        public static void AddMeshToVertexBatch(GameObject m)
        {
#if HARPIA_DEBUG
            Debug.Log($"[PB Mesh Batcher] Add Mesh To Vertex Batch {combinedMeshVertexFinder == null}");
#endif

            if (combinedMeshVertexFinder == null) return;

            //Get the valid filters
            MeshFilter[] allFilters = m.GetComponentsInChildren<MeshFilter>();
            List<MeshFilter> validFilters = new List<MeshFilter>();
            foreach (MeshFilter meshFilter in allFilters)
            {
                if (!IsValidForVertexFinding(meshFilter)) continue;
                validFilters.Add(meshFilter);
            }

            if (validFilters.Count == 0) return;

            int beforeVerticesLength = combinedMeshVertexFinder.vertices.Length;

            CombineInstance[] combineInstances = new CombineInstance[validFilters.Count + 1];

            combineInstances[0].mesh = combinedMeshVertexFinder;
            combineInstances[0].transform = Matrix4x4.identity;

            for (int i = 1; i < combineInstances.Length; i++)
            {
                MeshFilter filter = validFilters[i - 1];
                combineInstances[i].mesh = filter.sharedMesh;
                combineInstances[i].transform = filter.transform.localToWorldMatrix;
                Debug.Log($"[PB Mesh Batcher] Adding new combineInstances {filter.name}");
            }

            Mesh newCombinedMesh = new(){ indexFormat = IndexFormat.UInt32, };

            newCombinedMesh.CombineMeshes(combineInstances, true, true, false);
            if (combinedMeshVertexFinder != null) Object.DestroyImmediate(combinedMeshVertexFinder);
            combinedMeshVertexFinder = newCombinedMesh;

            if (beforeVerticesLength == combinedMeshVertexFinder.vertices.Length)
            {
                //Combine failed for some reason
                //DisposeVertex();
                //return;
            }

            //we need this to re-send data to the GPU
            PB_VertexFinder.Dispose();
        }

        //Called by precision mode
        public static void AddMeshToBatch(GameObject mesh)
        {
            if (PrefabBrush.instance.GetRaycastMode() != PrefabBrush.RaycastMode.Mesh) return;
            if (!PrefabBrush.instance.precisionModeAddMeshToBatch.value) return;

#if HARPIA_DEBUG
            int vertexCount = combinedMeshRaycast == null ? 0 : combinedMeshRaycast.vertexCount;
#endif

            MeshFilter[] allFilters = mesh.GetComponentsInChildren<MeshFilter>();
            List<MeshFilter> validFilters = new List<MeshFilter>();

            foreach (MeshFilter meshFilter in allFilters)
            {
                if (!IsValidForMeshRaycast(meshFilter)) continue;
                validFilters.Add(meshFilter);
            }

            if (validFilters.Count == 0) return;

            CombineInstance[] combineInstances = new CombineInstance[validFilters.Count + 1];
            combineInstances[0].mesh = combinedMeshRaycast == null ? new Mesh() : combinedMeshRaycast;
            combineInstances[0].transform = Matrix4x4.identity;

            for (int i = 1; i < combineInstances.Length; i++)
            {
                MeshFilter filter = validFilters[i - 1];
                combineInstances[i].mesh = filter.sharedMesh;
                combineInstances[i].transform = filter.transform.localToWorldMatrix;
#if HARPIA_DEBUG
                if (combineInstances[i].mesh == null) Debug.LogError($"Combine instance is null {i}");
#endif
            }

            Mesh newCombinedMesh = new Mesh{ indexFormat = IndexFormat.UInt32 };
            newCombinedMesh.CombineMeshes(combineInstances);

            if (combinedMeshRaycast != null) Object.DestroyImmediate(combinedMeshRaycast);
            combinedMeshRaycast = newCombinedMesh;

            PB_MeshRaycaster.Dispose();

#if HARPIA_DEBUG
            Debug.Log($"[PB Mesh Batcher] Adding new mesh {validFilters.Count}");
            Assert.IsFalse(vertexCount == combinedMeshRaycast.vertexCount, $"Vertex count is the same {vertexCount}");
#endif
        }

        public static void Dispose(bool clearList = false)
        {
            //destroy mesh
            if (combinedMeshRaycast != null) Object.DestroyImmediate(combinedMeshRaycast);

            combinedMeshRaycast = null;
            DisposeVertex();

            if (validList != null) validList.Clear();

            PB_MeshRaycaster.Dispose();
        }

        public static Mesh BatchObject(GameObject gameObject)
        {
            //get all the mesh filter
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(false);
            List<MeshFilter> validMeshFilters = new List<MeshFilter>();

            foreach (MeshFilter filter in meshFilters)
            {
                if (filter.sharedMesh == null) continue;

                if (!filter.gameObject.TryGetComponent(typeof(Renderer), out Component r)) continue;

                MeshRenderer renderer = (MeshRenderer)r;

                if (renderer.enabled == false) continue;

                validMeshFilters.Add(filter);
            }

            //batch
            CombineInstance[] combine = new CombineInstance[validMeshFilters.Count];

            for (int index = 0; index < validMeshFilters.Count; index++)
            {
                MeshFilter filter = validMeshFilters[index];
                combine[index].mesh = filter.sharedMesh;
                combine[index].transform = filter.transform.localToWorldMatrix;
            }

            //create new mesh
            Mesh newCombinedMesh = new Mesh{ indexFormat = IndexFormat.UInt32 };
            newCombinedMesh.CombineMeshes(combine);

            return newCombinedMesh;
        }

        private static void DisposeVertex()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_MeshBatcher)}] Dispose Vertex");
#endif
            if (combinedMeshVertexFinder != null) Object.DestroyImmediate(combinedMeshVertexFinder);
            combinedMeshVertexFinder = null;
            PB_VertexFinder.Dispose();
        }
    }

    [Serializable]
    public class PbPrefabUI
    {
        private const string XMLFileName = "PrefabBrushPrefabXML";

        private static VisualTreeAsset _tree;
        private static string _visualTreeGuid;

        private static Texture2D _customPropsIconUnity;
        private static Texture2D _rigidbodyIconUnity;
        private static Texture2D _colliderIconUnity;
        private static Texture2D _attractorIconUnity;
        private static Texture2D _customPivotIconUnity;

        private static PbPrefabUI _contextSelected;
        private static PbPrefabUI _lastSelectedCard;

        public VisualElement iconElement;
        private TemplateContainer _rootElement;

        public GameObject prefabToPaint;

        public bool selected;
        public PrefabAttributes customProps;

        [NonSerialized]
        public Texture2D loadedIcon;

        private VisualElement _parentElement;
        private VisualElement _customPropsIcon;
        private VisualElement _rigidbodyIconElement;
        private Button _deleteButton;
        private VisualElement _colliderIconElement;
        private VisualElement _attractorIconElement;
        private VisualElement _customPivotIconElement;
        private Label _nameLabel;
        private static PbPrefabUI _lastPrefabClickedPrecision;
        public const string UiName = "PrefabUI";
        private VisualElement parent => PrefabBrush.instance.prefabsUiHolder;

        public PbPrefabUI(GameObject prefab)
        {
            prefabToPaint = prefab;
            InstantiateUI();
            customProps = new PrefabAttributes();
        }

        private void LoadIcon()
        {
            if (loadedIcon == null)
            {
                LoadThumbnail();
                return;
            }

            iconElement.SetBackgroundTexture(loadedIcon);
        }

        public void InstantiateUI()
        {
            _tree ??= PrefabBrush.LoadVisualTreeAsset(XMLFileName, ref _visualTreeGuid);
            _parentElement = parent;

            _rootElement = _tree.Instantiate();
            _rootElement.RegisterCallback<ClickEvent>(OnPrefabClick);

            _parentElement.Add(_rootElement);
            _rootElement.name = UiName;

            iconElement = _rootElement.Q<VisualElement>("icon");
#if UNITY_2022_2_OR_NEWER
            iconElement.style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Top));
#endif

            //Unity In Editor Icons
            if (_customPropsIconUnity == null) _customPropsIconUnity = EditorGUIUtility.IconContent("d_SceneViewTools").image as Texture2D;
            if (_rigidbodyIconUnity == null) _rigidbodyIconUnity = EditorGUIUtility.IconContent("d_Rigidbody Icon").image as Texture2D;
            if (_colliderIconUnity == null) _colliderIconUnity = EditorGUIUtility.IconContent("d_BoxCollider Icon").image as Texture2D;
            if (_attractorIconUnity == null) _attractorIconUnity = EditorGUIUtility.IconContent("ToolHandlePivot").image as Texture2D;
            if (_customPivotIconUnity == null) _customPivotIconUnity = EditorGUIUtility.IconContent("d_AvatarPivot").image as Texture2D;

            _customPropsIcon = _rootElement.Q<VisualElement>("custom-props-icon-pencil");
            _customPropsIcon.SetBackgroundTexture(_customPropsIconUnity);
            _customPropsIcon.SetActive(false);

            _colliderIconElement = _rootElement.Q<VisualElement>("icon-collider");
            _colliderIconElement.SetBackgroundTexture(_colliderIconUnity);
            _colliderIconElement.SetActive(false);

            _attractorIconElement = _rootElement.Q<VisualElement>("attractor-icon");
            _attractorIconElement.SetBackgroundTexture(_attractorIconUnity);
            _attractorIconElement.SetActive(false);

            _customPivotIconElement = _rootElement.Q<VisualElement>("custom-pivot-icon");
            _customPivotIconElement.SetBackgroundTexture(_customPivotIconUnity);
            _customPivotIconElement.SetActive(false);

            LoadIcon();

            _nameLabel = _rootElement.Q<Label>("name");
            _nameLabel.text = GetName();

            _deleteButton = _rootElement.Q<Button>("remove");
            _deleteButton.RegisterCallback<ClickEvent>(OnDeleteButton);
            _deleteButton.SetActive(false);

            _rootElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            _rootElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            _rootElement.SetBorderColor(Color.clear);

            _rootElement.RegisterCallback<ContextClickEvent>(OnContextClick);

            _rigidbodyIconElement = _rootElement.Q<VisualElement>("rigidbody-icon");
            _rigidbodyIconElement.SetBackgroundTexture(_rigidbodyIconUnity);

            if (selected) Select();

            UpdateUIPrefab();
        }

        public void UpdateUIPrefab()
        {
            if (_customPropsIcon == null) return;

            bool hasCustomProps = customProps != null && customProps.HasAnyChange();
            _customPropsIcon.SetActive(hasCustomProps);

            //name
            _nameLabel.text = GetName();

            //Rb
            bool hasRigidbody = PB_PhysicsSimulator.HasValidRigidbody(prefabToPaint);
            _rigidbodyIconElement.SetActive(hasRigidbody);

            //Collider
            bool hasCollider = PB_PhysicsSimulator.HasPhysicalCollider(prefabToPaint);
            _colliderIconElement.SetActive(hasCollider);

            //Has attractor
            bool hasAttractor = PB_AttractorManager.HasAttractor(prefabToPaint);
            _attractorIconElement.SetActive(hasAttractor);

            //Has custom pivot
            bool hasCustomPivot = PB_AttractorManager.HasCustomPivot(prefabToPaint);
            _customPivotIconElement.SetActive(hasCustomPivot);

            //Tooltip
            string tooltip = prefabToPaint.name + "\n";
            if (hasCustomProps) tooltip += "\nHas custom properties";
            if (hasRigidbody) tooltip += "\nHas a Non Kinematic Rigidbody";
            if (hasCollider) tooltip += "\nHas a Physical Collider";
            if (hasAttractor) tooltip += "\nHas a Pivot Attractor";
            if (hasCustomPivot) tooltip += "\nHas a Custom Pivot";

            _rootElement.tooltip = tooltip;
            _rootElement.SetBorderColor(selected ? Color.yellow : Color.clear);
            LoadIcon();
        }

        #region ContextMenu

        private void OnContextClick(ContextClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}] OnContextClick ");
#endif

            _contextSelected = this;
            PrefabBrush.PaintMode mode = PrefabBrush.instance.GetPaintMode();

            //Create the menu
            GenericMenu menu = new GenericMenu();

            //Add the options
            menu.AddItem(new GUIContent($"{GetName()}"), false, null);
            menu.AddItem(new GUIContent("Custom Props"), false, OnCustomSpawnProps);

            if (!selected)
                menu.AddItem(new GUIContent("Select this"), false, Select);
            else
                menu.AddItem(new GUIContent("Deselect"), false, Deselect);

            if (mode == PrefabBrush.PaintMode.Multiple)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Select All"), false, PB_MultipleModeManager.SelectAllPrefabs);
                menu.AddItem(new GUIContent("Deselect All"), false, PB_MultipleModeManager.DeselectAllPrefabs);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Rename"), false, OnRename);
            menu.AddItem(new GUIContent("Show Prefab In Project"), false, OnShowPrefab);
            //menu.AddItem(new GUIContent("Show Prefab In Inspector"), false, OnShowInInspector);

            if (PrefabBrushTool.IsPrefab(prefabToPaint))
                menu.AddItem(new GUIContent("Open Prefab"), false, OnOpenPrefab);

            menu.AddItem(new GUIContent("Remove From List"), false, OnRemoveFromList);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Move Left"), false, onMoveLeft);
            menu.AddItem(new GUIContent("Move Right"), false, OnMoveRight);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Reload Thumbnail"), false, OnReloadThumbnail);

            //Show the menu
            menu.ShowAsContext();
        }

        private void OnReloadThumbnail()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}] On Reload Thumbnail");
#endif
            //Destroy the current icon

            Object.DestroyImmediate(loadedIcon);
            loadedIcon = null;
            LoadIcon();
        }

        private void onMoveLeft()
        {
            PrefabBrush.instance.MovePrefabCard(this, true);
        }

        private void OnMoveRight()
        {
            PrefabBrush.instance.MovePrefabCard(this, false);
        }

        private void OnRename()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}] On Rename");
#endif

            //Show input dialog
            string newName = PB_EditorInputDialog.Show("Rename Prefab", "Enter the new name:", GetName(), "Rename");

            if (string.IsNullOrEmpty(newName)) return;

            if (string.IsNullOrWhiteSpace(newName))
            {
                PrefabBrush.DisplayError("Invalid Name! Try another name.");
                return;
            }

            customProps ??= new PrefabAttributes();
            customProps.customName = newName;

            //update the serialized object
            PrefabBrush.instance._serializedObject.UpdateIfRequiredOrScript();

            UpdateUIPrefab();
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}]PrefabBrushPrefab OnRename {prefabToPaint.name} to {newName}");
#endif
        }

        public string GetName()
        {
            if (prefabToPaint == null) return "Null Prefab";
            if (customProps == null) return prefabToPaint.name;

            if (string.IsNullOrEmpty(customProps.customName)) return prefabToPaint.name;
            return customProps.customName;
        }

        private void OnOpenPrefab()
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabToPaint);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AssetDatabase.OpenAsset(asset);
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        }

        private void OnShowInInspector()
        {
            Selection.activeObject = prefabToPaint;
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        }

        private void OnCustomSpawnProps()
        {
            int index = PrefabBrush.instance.currentPrefabs.IndexOf(this);
            CustomPrefabProps.Show(this, index);
        }

        private void OnRemoveFromList()
        {
            OnDeleteButton(null);
        }

        private void OnShowPrefab()
        {
#if HARPIA_DEBUG
            Debug.Log("[Prefab Brush] Clicked on show prefab in project");
#endif

            if (_contextSelected == null) return;
            if (_contextSelected.prefabToPaint == null) return;

            EditorGUIUtility.PingObject(_contextSelected.prefabToPaint);

            string path = AssetDatabase.GetAssetPath(_contextSelected.prefabToPaint);
            path = Path.GetDirectoryName(path);
            PB_FolderUtils.ShowFolder(path);
        }

        #endregion

        private void OnPrefabClick(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}] PrefabBrushPrefab OnPrefabClick is selected {selected} {PrefabBrush.instance.GetPaintMode()}");
#endif

            PB_AttractorManager.Dispose();
            if (PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Precision) _lastPrefabClickedPrecision = this;
            UpdateUIPrefab();

            switch (PrefabBrush.instance.GetPaintMode())
            {
                case PrefabBrush.PaintMode.Precision:
                    SetSelectedCard(this);
                    PB_PrecisionModeManager.SetPrefabToPaint(this);
                    break;

                case PrefabBrush.PaintMode.Multiple:
                    if (selected) Deselect();
                    else Select();
                    break;
            }

            if (!PrefabBrushTool.isUsingTool) PrefabBrush.instance.OnStartButton(null);
        }

        private static void SetSelectedCard(PbPrefabUI newValue)
        {
            PrefabBrush.PaintMode mode = PrefabBrush.instance.GetPaintMode();

            if (mode == PrefabBrush.PaintMode.Multiple)
            {
                newValue.selected = true;
                newValue.UpdateUIPrefab();
                return;
            }

            //Precision Mode
            PB_MultipleModeManager.DeselectAllPrefabs();
            _lastSelectedCard = newValue;
            _lastSelectedCard.selected = true;
            _lastSelectedCard.UpdateUIPrefab();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            _deleteButton.SetActive(false);
            _rootElement.SetBackgroundColor(Color.clear);
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _deleteButton.SetActive(true);
            _rootElement.SetBackgroundColor(new Color(0f, 0f, 0f, 0.39f));
        }

        private void LoadThumbnail()
        {
            SetThumbnail(PB_ThumbnailGenerator._defaultThumbPrefabUnity, false);
            PB_ThumbnailGenerator.GenerateThumbnail(this);
        }

        private void OnDeleteButton(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PbPrefabUI)}] On Delete Button");
#endif

            DisposeIcon();
            PrefabBrush.Remove(this);
            _parentElement.Remove(_rootElement);
            PrefabBrush.instance.FocusHiddenElement();
        }

        public string GetGuid()
        {
            if (prefabToPaint == null) return "";

            string path = AssetDatabase.GetAssetPath(prefabToPaint);
            return AssetDatabase.AssetPathToGUID(path);
        }

        public static void Dispose(List<PbPrefabUI> currentList)
        {
            if (currentList != null && PrefabBrush.instance == null)
            {
                foreach (PbPrefabUI ui in currentList)
                {
                    if (ui == null) continue;
                    ui.DisposeIcon();
                }
            }

            if (_lastSelectedCard != null && PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Precision)
                _lastSelectedCard.Deselect();

            _lastSelectedCard = null;
        }

        private void DisposeIcon()
        {
            if (loadedIcon == null) return;
            Object.DestroyImmediate(loadedIcon);
            loadedIcon = null;
        }

        public void Select()
        {
            SetSelectedCard(this);
        }

        public static PbPrefabUI GetSelectedPrefab()
        {
            if (_lastSelectedCard != null) return _lastSelectedCard;
            if (_lastPrefabClickedPrecision != null) return _lastPrefabClickedPrecision;
            return null;
        }

        public static bool HasAnySelected()
        {
            return PrefabBrush.instance.currentPrefabs.Any(e => e.selected);
        }

        public static void SelectFirst()
        {
            if (_lastPrefabClickedPrecision != null)
            {
                _lastPrefabClickedPrecision.Select();
                return;
            }

            List<PbPrefabUI> list = PrefabBrush.instance.currentPrefabs;
            if (list.Count == 0)
            {
                return;
            }

            PB_MultipleModeManager.DeselectAllPrefabs();

            list[0].Select();
        }

        public void Deselect()
        {
            selected = false;
            UpdateUIPrefab();
        }

        public static void SelectAll()
        {
            foreach (PbPrefabUI prefab in PrefabBrush.instance.currentPrefabs)
                prefab.Select();
        }

        public void SetBackgroundColor(Color c)
        {
            _rootElement.SetBackgroundColor(c);
        }

        public bool AllowsPhysicsPlacement()
        {
            if (prefabToPaint == null) return false;
            if (!PB_PhysicsSimulator.HasValidRigidbody(prefabToPaint)) return false;
            if (!PB_PhysicsSimulator.HasPhysicalCollider(prefabToPaint)) return false;
            return true;
        }

        public void SetThumbnail(Texture2D thumbnail, bool isLoadedFromAssetPreview)
        {
            if (thumbnail == null) return;
            if (isLoadedFromAssetPreview)
            {
                RenderTexture rt = new RenderTexture(thumbnail.width, thumbnail.height, 24);
                RenderTexture.active = rt;
                Graphics.Blit(thumbnail, rt);

                loadedIcon = new Texture2D(thumbnail.width, thumbnail.height);
                loadedIcon.ReadPixels(new Rect(0, 0, thumbnail.width, thumbnail.height), 0, 0);
                loadedIcon.Apply();

                RenderTexture.active = null;
                rt.Release();
                Object.DestroyImmediate(rt);
            }

            iconElement.SetBackgroundTexture(thumbnail);
        }

        public VisualElement GetVisualRoot() => _rootElement;

        public bool IsOnUI()
        {
            if (_rootElement == null) return false;
            if (_rootElement.panel == null) return false;
            if (_rootElement.parent == null) return false;
            return true;
        }
    }

    [Serializable]
    public class PrefabAttributes
    {
        public string customName;
        public bool useCustomPivotMode;
        public PrefabBrush.PivotMode pivotMode;

        public bool useCustomOffset;
        public PrefabBrush.Vector3Mode offsetMode;
        public Vector3 offsetMinValue = Vector3.one * -.2f;
        public Vector3 offsetMaxValue = Vector3.one * 0.2f;
        public Vector3 offsetFixedValue = Vector3.zero;

        public bool useCustomRotationMode;
        public PrefabBrush.RotationTypes rotationMode;
        public Vector3 rotationMinValue = Vector3.zero;
        public Vector3 rotationMaxValue = Vector3.up * 360f;
        public Vector3 fixedRotation = Vector3.zero;

        public bool useCustomScale;

        public PrefabBrush.Vector3ModeUniform scaleMode;
        public Vector3 scaleMinValue = Vector3.one * 0.8f;
        public Vector3 scaleMaxValue = Vector3.one * 1.2f;

        public Vector3 scaleMaxUniformValue = Vector3.one;
        public Vector3 scaleMinUniformValue = Vector3.one;

        public Vector3 fixedScale = Vector3.one;

        private Vector3 _currentRandomScale;
        private Vector3 _currentRandomRotation;
        private Vector3 _currentRotationOffset;
        private Vector3 _currentRandomOffset;

        public bool HasAnyChange()
        {
            return useCustomOffset || useCustomPivotMode || useCustomRotationMode || useCustomScale;
        }

        public void SetScale(Vector3ModeElement e)
        {
            useCustomScale = e.useToggle?.value ?? false;
            scaleMode = (PrefabBrush.Vector3ModeUniform)e.enumField.value;
            scaleMinValue = e.minField.value;
            scaleMaxValue = e.maxField.value;
            fixedScale = e.fixedField.value;

            scaleMaxUniformValue = e.maxFieldUniform.value.x * Vector3.one;
            scaleMinUniformValue = e.minFieldUniform.value.x * Vector3.one;
        }

        public void SetRotation(RotationTypeElement e)
        {
            useCustomRotationMode = e.useToggle?.value ?? false;
            rotationMode = (PrefabBrush.RotationTypes)e.enumField.value;
            rotationMinValue = e.minField.value;
            rotationMaxValue = e.maxField.value;
            fixedRotation = e.fixedField.value;
        }

        public void SetOffset(Vector3ModeElement e)
        {
            useCustomOffset = e.useToggle?.value ?? false;
            offsetMode = (PrefabBrush.Vector3Mode)e.enumField.value;
            offsetMinValue = e.minField.value;
            offsetMaxValue = e.maxField.value;
            offsetFixedValue = e.fixedField.value;
        }

        public void SetPivotMode(bool value, PrefabBrush.PivotMode p)
        {
            useCustomPivotMode = value;
            pivotMode = p;
        }

        public Vector3 GetOffset()
        {
            if (offsetMode == PrefabBrush.Vector3Mode.Fixed) return offsetFixedValue;

            if (_currentRandomOffset != Vector3.zero) return _currentRandomOffset;
            _currentRandomOffset = GetRandomVec3(offsetMinValue, offsetMaxValue);

            return _currentRandomOffset;
        }

        public Vector3 GetRotation()
        {
            switch (rotationMode)
            {
                case PrefabBrush.RotationTypes.Fixed:
                    return fixedRotation;
                case PrefabBrush.RotationTypes.Scene_Camera_Rotation:
                    return RotationTypeElement.CalculateSceneCameraRotation() + _currentRotationOffset;
            }

            if (_currentRandomRotation != Vector3.zero) return _currentRandomRotation;
            _currentRandomRotation = GetRandomVec3(rotationMinValue, rotationMaxValue);

            return _currentRandomRotation;
        }

        public Vector3 GetScale()
        {
            if (scaleMode == PrefabBrush.Vector3ModeUniform.Fixed) return fixedScale;

            if (scaleMode == PrefabBrush.Vector3ModeUniform.Random_Uniform)
            {
                return Vector3.one * Random.Range(scaleMinUniformValue.x, scaleMaxUniformValue.x);
            }

            if (_currentRandomScale != Vector3.zero) return _currentRandomScale;
            _currentRandomScale = GetRandomVec3(scaleMinValue, scaleMaxValue);

            return _currentRandomScale;
        }

        private Vector3 GetRandomVec3(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);
            return new Vector3(x, y, z);
        }

        public void Dispose()
        {
            _currentRandomOffset = Vector3.zero;
            _currentRandomRotation = Vector3.zero;
            _currentRandomScale = Vector3.zero;
        }

        public void AddToRotation(float value)
        {
            switch (rotationMode)
            {
                case PrefabBrush.RotationTypes.Fixed:
                    fixedRotation += Vector3.up * value;
                    return;
                case PrefabBrush.RotationTypes.Scene_Camera_Rotation:
                    _currentRotationOffset += Vector3.up * value;
                    return;
                default:
                    _currentRandomRotation += Vector3.up * value;
                    break;
            }
        }
    }

    public static class CustomPrefabProps
    {
        private static Label labelName;

        private static VisualElement _customPropsPanel;
        private static VisualElement _iconStatic;

        private static Vector3ModeElement _offsetField;
        private static RotationTypeElement _rotationField;
        private static Vector3ModeElement _scaleField;

        private static PbPrefabUI _selectedPbPrefabObj;
        private static Toggle _useCustomPivotToggle;
        private static DefaultEnumField _customPivotEnum;
        private static Button _prefabScaleButton;

        public static void Init(VisualElement root)
        {
            _customPropsPanel = root.Q<VisualElement>("custom-props-section");

            _iconStatic = root.Q<VisualElement>("custom-props-icon");
#if UNITY_2022_2_OR_NEWER
            _iconStatic.style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Top));
#endif
            _iconStatic.SetBackgroundTexture(Texture2D.blackTexture);

            labelName = root.Q<Label>("custom-props-prefab-name");

            _offsetField = new Vector3ModeElement(root, "custom-props-offset-type", "custom-props-offset-fixed", "custom-props-offset-max", "custom-props-offset-min", "custom-props-offset-toggle", false, PrefabBrushTemplate.CustomPropsOffsetMode, PrefabBrushTemplate.CustomPropsOffsetFixed, PrefabBrushTemplate.CustomPropsOffsetMax, PrefabBrushTemplate.CustomPropsOffsetMin, null, null, null);

            _rotationField = new RotationTypeElement(root, "custom-props-rotation-type", "custom-props-rotation-fixed", "custom-props-rotation-max", "custom-props-rotation-min", "custom-props-rotation-toggle", PrefabBrushTemplate.CustomPropsRotationMode, PrefabBrushTemplate.CustomPropsRotationFixed, PrefabBrushTemplate.CustomPropsRotationMax, PrefabBrushTemplate.CustomPropsRotationMin, null);

            _scaleField = new Vector3ModeElement(root, "custom-props-scale-type", "custom-props-scale-fixed", "custom-props-scale-max", "custom-props-scale-min", "custom-props-scale-toggle", true, PrefabBrushTemplate.CustomPropsScaleMode, PrefabBrushTemplate.CustomPropsScaleFixed, PrefabBrushTemplate.CustomPropsScaleMax, PrefabBrushTemplate.CustomPropsScaleMin, null, PrefabBrushTemplate.CustomPropsScaleMaxUniform, PrefabBrushTemplate.CustomPropsScaleMinUniform);

            _prefabScaleButton = _customPropsPanel.Q<Button>("custom-prop-scale-use-prefab");
            _prefabScaleButton.RegisterCallback<ClickEvent>(OnUsePrefabScale);

            _customPropsPanel.Q<Button>("custom-props-scale-multiply-max").RegisterCallback<ClickEvent>(_ => OnMultiplyScale(_scaleField.maxField));
            _customPropsPanel.Q<Button>("custom-props-scale-multiply-min").RegisterCallback<ClickEvent>(_ => OnMultiplyScale(_scaleField.minField));

            Button prefabRotationButton = _customPropsPanel.Q<Button>("custom-prop-rot-use-prefab");
            prefabRotationButton.RegisterCallback<ClickEvent>(OnUsePrefabRotation);

            root.Q<Button>("custom-props-back").RegisterCallback<ClickEvent>(OnBackClick);

            _useCustomPivotToggle = root.Q<Toggle>("custom-props-pivot-toggle");
            _customPivotEnum = root.Q<DefaultEnumField>("custom-props-pivot-type");
            _customPivotEnum.Init(PrefabBrush.PivotMode.MeshPivot);
            _customPivotEnum.BindProperty(PrefabBrush.instance._serializedObject.FindProperty(PrefabBrushTemplate.CustomPivotMode));

            _useCustomPivotToggle.RegisterValueChangedCallback(evt => {
                _customPivotEnum.SetActive(evt.newValue);
                if (_selectedPbPrefabObj == null) return;
                OnAnyChange();
            });

            _customPivotEnum.RegisterValueChangedCallback(_ => {
                if (_selectedPbPrefabObj == null) return;
                OnAnyChange();
            });

            _scaleField.RegisterEventAll(OnAnyChange);
            _rotationField.RegisterEventAll(OnAnyChange);
            _offsetField.RegisterEventAll(OnAnyChange);

            _customPropsPanel.SetActive(false);
        }

        private static void OnMultiplyScale(Vector3Field scaleInput)
        {
#if HARPIA_DEBUG
            Debug.Log($"Clicked multiply max");
#endif
            Vector3 scale = _selectedPbPrefabObj.prefabToPaint.transform.lossyScale;

            if (scale.magnitude == 0)
            {
                PrefabBrush.DisplayError("Prefab Scale is 0");
                return;
            }

            float x = scale.x * scaleInput.value.x;
            float y = scale.y * scaleInput.value.y;
            float z = scale.z * scaleInput.value.z;
            scaleInput.value = new Vector3(x, y, z);
        }

        private static void OnUsePrefabRotation(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(CustomPrefabProps)}] Clicked use prefab rotation");
#endif

            _rotationField.fixedField.value = _selectedPbPrefabObj.prefabToPaint.transform.rotation.eulerAngles;

            //log
            string log = $"{PrefabBrush.DebugLogStart} Prefab Rotation: {_rotationField.fixedField.value}";
            Debug.Log(log, _selectedPbPrefabObj.prefabToPaint);
        }

        private static void OnUsePrefabScale(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"Clicked use prefab scale");
#endif

            _scaleField.fixedField.value = _selectedPbPrefabObj.prefabToPaint.transform.lossyScale;
        }

        private static void OnAnyChange()
        {
            if (_selectedPbPrefabObj == null) return;
            PrefabAttributes props = _selectedPbPrefabObj.customProps;

            props.SetRotation(_rotationField);
            props.SetOffset(_offsetField);
            props.SetScale(_scaleField);
            props.Dispose();

            props.SetPivotMode(_useCustomPivotToggle.value, (PrefabBrush.PivotMode)_customPivotEnum.value);

            PB_PrecisionModeManager.currentScaleProps = 1;

            _selectedPbPrefabObj.UpdateUIPrefab();
        }

        private static void OnBackClick(ClickEvent evt)
        {
            _customPropsPanel.SetActive(false);
            PrefabBrush.instance.paintModePanel.SetActive(true);
        }

        public static void Show(PbPrefabUI obj, int index)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(CustomPrefabProps)}] Showing custom props for {obj.prefabToPaint.name} - Index {index}", obj.prefabToPaint);
#endif

            _selectedPbPrefabObj = obj;
            if (obj.loadedIcon != null)
            {
                _iconStatic.SetBackgroundTexture(obj.loadedIcon);
                _iconStatic.SetActive(true);
                _iconStatic.SetVisible(true);
                _iconStatic.SetEnabled(true);
            }
            else
            {
                _iconStatic.SetActive(false);
            }

            PrefabBrush.instance.paintModePanel.SetActive(false);
            _customPropsPanel.SetActive(true);

            labelName.text = "Custom Properties: " + obj.GetName();

            PrefabAttributes p = obj.customProps;

            Debug.LogError($"here");

            _offsetField.SetValues(p.useCustomOffset, (int)p.offsetMode, p.offsetMinValue, p.offsetMaxValue, p.offsetFixedValue, false);
            _rotationField.SetValues(p.useCustomRotationMode, (int)p.rotationMode, p.rotationMinValue, p.rotationMaxValue, p.fixedRotation, false);
            _scaleField.SetValues(p.useCustomScale, (int)p.scaleMode, p.scaleMinValue, p.scaleMaxValue, p.fixedScale, true);

            SerializedObject s = PrefabBrush.instance._serializedObject;

            //prefabs.Array.data[0].customProps.useCustomOffset

            //Offset
            _offsetField.useToggle.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.useCustomOffset"));
            _offsetField.minField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.offsetMinValue"));
            _offsetField.maxField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.offsetMaxValue"));
            _offsetField.fixedField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.offsetFixedValue"));
            _offsetField.enumField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.offsetMode"));

            //Rortation
            _rotationField.useToggle.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.useCustomRotationMode"));
            _rotationField.minField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.rotationMinValue"));
            _rotationField.maxField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.rotationMaxValue"));
            _rotationField.fixedField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.fixedRotation"));

            //Scale
            _scaleField.useToggle.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.useCustomScale"));
            _scaleField.minField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.scaleMinValue"));
            _scaleField.maxField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.scaleMaxValue"));
            _scaleField.fixedField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.fixedScale"));
            _scaleField.enumField.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.scaleMode"));

            //Uniform fileds
            _scaleField.maxFieldUniform.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.scaleMaxUniformValue"));
            _scaleField.minFieldUniform.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.scaleMinUniformValue"));

            //pivot
            _useCustomPivotToggle.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.useCustomPivotMode"));
            _customPivotEnum.BindProperty(s.FindProperty($"prefabs.Array.data[{index}].customProps.pivotMode"));

            _useCustomPivotToggle.value = p.useCustomPivotMode;
            _customPivotEnum.value = p.pivotMode;
            _customPivotEnum.SetActive(p.useCustomPivotMode);
            _prefabScaleButton.tooltip = "Prefab Scale: " + _selectedPbPrefabObj.prefabToPaint.transform.lossyScale;
        }

        public static void Hide()
        {
            _customPropsPanel.SetActive(false);
            PrefabBrush.instance.paintModePanel.SetActive(true);
        }
    }

    public static class PB_TerrainHandler
    {
        private static Texture[] terrainTextures;

        public static void ShowAlert()
        {
            if (Terrain.activeTerrain == null) return;
            Debug.LogWarning($"{PrefabBrush.DebugLogStart} Attention: " + $"Since you have a terrain in your scene, ensure you select the 'Physical Raycast' mode for object placement on 'Raycast mode' dropdown ");
        }

        public static void Init()
        {
            if (!PrefabBrush.instance.isRaycastHit) return;
            if (Terrain.activeTerrain == null) return;
        }

        private static Vector3Int ConvertToSplatCoordinates(Vector3 pos, Terrain t)
        {
            Vector3 positionOnTerrain = pos - t.transform.position;

            TerrainData terrainData = t.terrainData;
            float indexX = positionOnTerrain.x / terrainData.size.x;
            float indexZ = positionOnTerrain.z / terrainData.size.z;

            int x = Mathf.FloorToInt(terrainData.alphamapWidth * indexX);
            int z = Mathf.FloorToInt(terrainData.alphamapHeight * indexZ);
            return new Vector3Int(x, 0, z);
        }

        private static List<Texture> GetTexturesAt(Vector3 position, Terrain terrain, float threshold = .5f)
        {
            var _splatCoordinates = ConvertToSplatCoordinates(position, terrain);

            //Check if the coordinates are valid
            if (_splatCoordinates.x < 0 || _splatCoordinates.x >= terrain.terrainData.alphamapWidth) return new List<Texture>();
            if (_splatCoordinates.z < 0 || _splatCoordinates.z >= terrain.terrainData.alphamapHeight) return new List<Texture>();

            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(_splatCoordinates.x, _splatCoordinates.z, 1, 1);

            List<Texture> textures = new();
            for (int i = 0; i < alphaMap.GetLength(2); i++)
            {
                if (alphaMap[0, 0, i] > threshold)
                {
                    textures.Add(terrain.terrainData.terrainLayers[i].diffuseTexture);
                }
            }

            return textures;
        }

        private static void GetTerrainProps()
        {
            var lastTerrain = Terrain.activeTerrain;
            var lastTerrainData = lastTerrain.terrainData;

            //get the textures
            terrainTextures = new Texture[lastTerrainData.terrainLayers.Length];

            for (int i = 0; i < lastTerrainData.terrainLayers.Length; i++)
            {
                if (lastTerrainData.terrainLayers[i] == null) continue;
                if (lastTerrainData.terrainLayers[i].diffuseTexture == null) continue;
                terrainTextures[i] = lastTerrainData.terrainLayers[i].diffuseTexture;
            }
        }

        public static Texture[] GetTerrainTextures(Terrain t)
        {
            var data = t.terrainData;

            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < data.terrainLayers.Length; i++)
            {
                if (data.terrainLayers[i] == null) continue;
                if (data.terrainLayers[i].diffuseTexture == null) continue;
                textures.Add(data.terrainLayers[i].diffuseTexture);
            }

            return textures.ToArray();
        }

        public static Texture[] GetTerrainTextures()
        {
            if (Terrain.activeTerrain == null) return null;

            terrainTextures = GetTerrainTextures(Terrain.activeTerrain);

            if (terrainTextures == null || terrainTextures.Length == 0)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} Terrain textures are null or empty.");
            }

            return terrainTextures;
        }

        public static void Dispose()
        {
        }

        public static bool HasAnyTerrain()
        {
            return Terrain.activeTerrain != null;
        }

        public static List<Texture> GetTexturesAtPosition(Vector3 hitPoint)
        {
            if (Terrain.activeTerrain == null) return new List<Texture>();
            return GetTexturesAt(hitPoint, Terrain.activeTerrain);
        }
    }

    public static class PB_UndoManager
    {
        private static List<GameObject> _currentUndoList;
        private static double _lastUndoTime;
        private static string _lastGroupName;

        private const string baseUndoMsg = "Prefab Brush - ";
        private const string undoMessage = baseUndoMsg + "Painted Game Objects";
        //private const string undoMessageTransform = baseUndoMsg + "Saved Transforms";

        public static void AddToUndo(GameObject o)
        {
            _currentUndoList ??= new List<GameObject>();
            _currentUndoList.Add(o);
        }

        public static void RegisterUndo()
        {
            if (_currentUndoList == null) return;
            if (_currentUndoList.Count == 0) return;

            Undo.RecordObject(_currentUndoList[0], undoMessage);
            int undoID = Undo.GetCurrentGroup();

            int undoCount = 0;
            foreach (GameObject go in _currentUndoList)
            {
                if (go == null) continue;
                Undo.RegisterCreatedObjectUndo(go, undoMessage);
                Undo.CollapseUndoOperations(undoID);
                undoCount++;
            }

            Undo.SetCurrentGroupName($"Prefab Brush Undo - Object Count:{undoCount} - ID: {undoID}");

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_UndoManager)}] RegisterUndo - {undoCount} objects - Undo ID {undoID}");
#endif

            Undo.FlushUndoRecordObjects();
            _currentUndoList.Clear();
        }

        public static bool IsUndoMessage(string msg)
        {
            return msg.Contains(baseUndoMsg);
        }

        public static void DestroyAndRegister(GameObject objectToEraser)
        {
            int undoID = Undo.GetCurrentGroup();
            Undo.DestroyObjectImmediate(objectToEraser);
            Undo.CollapseUndoOperations(undoID);
        }

        public static void RegisterUndoTransforms(IEnumerable<Rigidbody> selectedRigidbodies, string message)
        {
            if (selectedRigidbodies == null) return;

            Transform[] allTransforms = selectedRigidbodies.Select(rb => rb.transform).ToArray();
            if (allTransforms.Length == 0) return;

            Undo.RecordObjects(allTransforms, "Prefab Brush " + message);
        }

        public static void PerformUndo()
        {
            //Return in some cases
            if (_lastUndoTime == EditorApplication.timeSinceStartup) return;

#if UNITY_2022_2_OR_NEWER
            bool isPrecessing = Undo.isProcessing;
            if (isPrecessing) return;
#endif

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_UndoManager)}] Perform undo | Name: {Undo.GetCurrentGroupName()}");
#endif

            //Check if undo is not working
            string currentGroupName = Undo.GetCurrentGroupName();
            if (currentGroupName == _lastGroupName)
            {
                PrefabBrush.instance.ExitTool();

                //Get current control
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                GUIUtility.ExitGUI();

                //log
                Debug.LogError($"{PrefabBrush.DebugLogStart} Exiting the tool to undo the last operation. Some Unity Versions undo through code does not works. We are working to fix this.");
                return;
            }

            //Save the last undo time
            _lastUndoTime = EditorApplication.timeSinceStartup;
            _lastGroupName = currentGroupName;
        }

        public static bool IsLastUndoTimeGreaterThan(double time)
        {
            return EditorApplication.timeSinceStartup - _lastUndoTime > time;
        }
    }

    public static class EditorPrefsExtension
    {
        public static void SaveColor(string key, Color c)
        {
            EditorPrefs.SetFloat(key + "_r", c.r);
            EditorPrefs.SetFloat(key + "_g", c.g);
            EditorPrefs.SetFloat(key + "_b", c.b);
            EditorPrefs.SetFloat(key + "_a", c.a);
        }

        public static Color GetColor(string key, Color defaultValue)
        {
            if (EditorPrefs.HasKey(key + "_r") == false) return defaultValue;

            float r = EditorPrefs.GetFloat(key + "_r");
            float g = EditorPrefs.GetFloat(key + "_g");
            float b = EditorPrefs.GetFloat(key + "_b");
            float a = EditorPrefs.GetFloat(key + "_a");

            return new Color(r, g, b, a);
        }

        public static void DeleteColorKey(string pbBrushBaseColor)
        {
            EditorPrefs.DeleteKey(pbBrushBaseColor + "_r");
            EditorPrefs.DeleteKey(pbBrushBaseColor + "_g");
            EditorPrefs.DeleteKey(pbBrushBaseColor + "_b");
            EditorPrefs.DeleteKey(pbBrushBaseColor + "_a");
        }
    }

    public abstract class PB_Vector3Element
    {
        public Toggle useToggle;
        public DefaultEnumField enumField;
        public Vector3Field fixedField;
        public Vector3Field maxField;
        public Vector3Field minField;
        private Vector3 lastRandomValue;

        public virtual void RegisterEventAll(Action action)
        {
            enumField.RegisterValueChangedCallback((_) => action());
            minField.RegisterValueChangedCallback((_) => action());
            maxField.RegisterValueChangedCallback((_) => action());
            fixedField.RegisterValueChangedCallback((_) => action());
            useToggle?.RegisterValueChangedCallback((_) => action());
        }

        protected void AddManipulators(Vector3Field f)
        {
            if (f == null) return;
            f.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Zero (0,0,0)", _ => f.value = Vector3.zero, DropdownMenuAction.AlwaysEnabled); }));
            f.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("One (1,1,1)", _ => f.value = Vector3.one, DropdownMenuAction.AlwaysEnabled); }));
        }

        protected void Init(VisualElement root, string enumFieldParam, string fixedFieldParam, string maxFieldParam, string minFieldParam, string useToggleParam, string enumFieldSerializableName, string fixedFieldSerializableName, string maxFieldSerializableName, string minFieldSerializableName, string useToggleSerializableName)
        {
            enumField = root.Q<DefaultEnumField>(enumFieldParam);
            enumField.BindProperty(PrefabBrush.instance._serializedObject.FindProperty(enumFieldSerializableName));

            fixedField = root.Find<Vector3Field>(fixedFieldParam, fixedFieldSerializableName);
            maxField = root.Find<Vector3Field>(maxFieldParam, maxFieldSerializableName);
            minField = root.Find<Vector3Field>(minFieldParam, minFieldSerializableName);

            AddManipulators(fixedField);
            AddManipulators(minField);
            AddManipulators(maxField);

            if (!string.IsNullOrEmpty(useToggleParam))
            {
                useToggle = root.Find<Toggle>(useToggleParam, useToggleSerializableName);
                useToggle.RegisterValueChangedCallback(OnUseToggleValueChanged);
            }
        }

        public abstract Vector3 GetValue(bool canGenerateRandom, RaycastHit objectPoint);

        protected void OnUseToggleValueChanged(ChangeEvent<bool> evt)
        {
            bool value = useToggle?.value ?? true;
            enumField.SetActive(value);
            UpdateUIVec3Element();
        }

        public abstract void UpdateUIVec3Element();

        protected Vector3 GetRandomValue(bool canGenerateRandom)
        {
            if (!canGenerateRandom) return lastRandomValue;
            Vector3 min = minField.value;
            Vector3 max = maxField.value;
            lastRandomValue = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            return lastRandomValue;
        }
    }

    public class RotationTypeElement : PB_Vector3Element
    {
        private VisualElement lookAtSection;
        private ObjectField lookAtReferenceTransform;
        public Toggle lookAtRotX;
        public Toggle lookAtRotY;
        public Toggle lookAtRotZ;

        public RotationTypeElement(VisualElement root, string enumField, string fixedField, string maxField, string minField, string toggle, string enumFieldSerializableName, string fixedFieldSerializableName, string maxFieldSerializableName, string minFieldSerializableName, string useToggleSerializableName)
        {
            Init(root, enumField, fixedField, maxField, minField, toggle, enumFieldSerializableName, fixedFieldSerializableName, maxFieldSerializableName, minFieldSerializableName, useToggleSerializableName);
            this.enumField.Init(PrefabBrush.RotationTypes.Fixed);
            this.enumField.RegisterValueChangedCallback(OnEnumValueChanged);

            lookAtSection = root.Q<VisualElement>("rotation-look-at-section");
            lookAtRotX = lookAtSection.Find<Toggle>("rotation-look-at-axis-x", PrefabBrushTemplate.LookAtRotX);
            lookAtRotY = lookAtSection.Find<Toggle>("rotation-look-at-axis-y", PrefabBrushTemplate.LookAtRotY);
            lookAtRotZ = lookAtSection.Find<Toggle>("rotation-look-at-axis-z", PrefabBrushTemplate.LookAtRotZ);
            lookAtReferenceTransform = lookAtSection.Find<ObjectField>("rotation-look-at-transform", PrefabBrushTemplate.LookAtRotObject);
            lookAtSection.SetActive(false);
        }

        private void OnEnumValueChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[Prefab Brush] Changed rotaiton mode to {enumField.value}");
#endif

            bool toggleValue = useToggle?.value ?? true;

            int modeInt = Convert.ToInt32(enumField.value);

            enumField.SetActive(toggleValue);
            enumField.value = (PrefabBrush.RotationTypes)modeInt;

            bool enableFixed = modeInt == (int)PrefabBrush.Vector3Mode.Fixed && toggleValue;
            fixedField.SetActive(enableFixed);

            bool enableMax = modeInt == (int)PrefabBrush.Vector3Mode.Random && toggleValue;
            maxField.SetActive(enableMax);

            bool enableMin = modeInt == (int)PrefabBrush.Vector3Mode.Random && toggleValue;
            minField.SetActive(enableMin);

            lookAtSection.SetActive(modeInt == (int)PrefabBrush.RotationTypes.Look_At_Object);

            switch (modeInt)
            {
                case (int)PrefabBrush.RotationTypes.Scene_Camera_Rotation:
                    PB_PrecisionModeManager.UpdateCurrentRotation(Vector3.zero);
                    break;

                case (int)PrefabBrush.RotationTypes.Look_At_Object:
                {
                    //beta warning
                    const string skipLookAtObjBetaWarning = "prefab-brush-skip-look-at-rot-beta-warning";
                    if (!EditorPrefs.GetBool(skipLookAtObjBetaWarning, false))
                    {
                        bool option = EditorUtility.DisplayDialog("Prefab Brush",
                            $"Attention: Look_At_Object rotation mode still in beta and may not work correctly. ",
                            "Ok. Don't show me this again", "Ok");
                        if (option) EditorPrefs.SetBool(skipLookAtObjBetaWarning, true);
                    }

                    break;
                }
            }
        }

        public override Vector3 GetValue(bool canGenerateRandom, RaycastHit objectPoint)
        {
            int mode = Convert.ToInt16(enumField.value);

            switch (mode)
            {
                case (int)PrefabBrush.RotationTypes.Scene_Camera_Rotation: return CalculateSceneCameraRotation();

                case (int)PrefabBrush.RotationTypes.Look_At_Object:
                    Debug.Log($"here ");
                    Vector3 d = GetLookAtPos(objectPoint);
                    return Quaternion.LookRotation(d).eulerAngles;

                case (int)PrefabBrush.RotationTypes.Fixed: return fixedField.value;

                case (int)PrefabBrush.RotationTypes.Random:
                {
                    return GetRandomValue(canGenerateRandom);
                }
            }

            return Vector3.zero;
        }

        public static Vector3 CalculateSceneCameraRotation()
        {
            Camera cam = PrefabBrush.instance.sceneCamera;
            Vector3 posOnCamera = cam.WorldToScreenPoint(PrefabBrush.instance.lastHitInfo.point);
            posOnCamera = new Vector3(posOnCamera.x / cam.pixelWidth, posOnCamera.y / cam.pixelHeight, 0);
            float x = (posOnCamera.x - .5f) * cam.fieldOfView;
            Vector3 camOriginalRot = cam.transform.rotation.eulerAngles;
            return new Vector3(0, x + camOriginalRot.y / Mathf.PI, 0);
        }

        public void TrySetValue(PrefabBrush.RotationTypes desiredMode, Vector3 newVal)
        {
            PrefabBrush.RotationTypes current = (PrefabBrush.RotationTypes)enumField.value;
            if (current != desiredMode) return;
            fixedField.SetValueWithoutNotify(newVal);
        }

        public override void UpdateUIVec3Element()
        {
            OnEnumValueChanged(null);
        }

        public PrefabBrush.RotationTypes GetMode()
        {
            return (PrefabBrush.RotationTypes)enumField.value;
        }

        public void SetValues(bool isUsing, int mode, Vector3 minValue, Vector3 maxValue, Vector3 fixedValue, bool useUniform)
        {
            minField.SetValueWithoutNotify(minValue);
            maxField.SetValueWithoutNotify(maxValue);
            fixedField.SetValueWithoutNotify(fixedValue);
            enumField.SetValueWithoutNotify(useUniform ? (PrefabBrush.Vector3ModeUniform)mode : (PrefabBrush.Vector3Mode)mode);
            UpdateUIVec3Element();
        }

        public Vector3 GetLookAtPos(RaycastHit ray)
        {
            bool useX = lookAtRotX.value;
            bool useY = lookAtRotY.value;
            bool useZ = lookAtRotZ.value;

            //Safety checks
            if (!useX && !useY && !useZ)
            {
                PB_HandlesExtension.WriteTextErrorTemp("No Axis Selected", ray);
                return ray.point;
            }

            if (lookAtReferenceTransform.value == null)
            {
                PB_HandlesExtension.WriteTextErrorTemp("No Look At Transform Selected", ray);
                return ray.point;
            }

            //Initialize values
            Vector3 lookAtPos = ((Transform)lookAtReferenceTransform.value).position;
            Vector3 p = ray.point;
            if (!useX) p.x = lookAtPos.x;
            if (!useY) p.y = lookAtPos.y;
            if (!useZ) p.z = lookAtPos.z;
            Vector3 ret = lookAtPos - p;

            //Ray drawing
            if (PrefabBrush.instance.GetPaintMode() == PrefabBrush.PaintMode.Precision)
                Debug.DrawRay(ray.point, ret, Color.red, .05f);
            else
                Debug.DrawRay(ray.point, ret, Color.red, .4f);

            //Return
            return ret;
        }
    }

    public class Vector3ModeElement : PB_Vector3Element
    {
        public readonly Vector3Field minFieldUniform;
        public readonly Vector3Field maxFieldUniform;

        public bool proportionsFixed;
        public bool proportionsMin;
        public bool proportionsMax;
        private readonly bool allowUniform;

        private Button buttonFixedField;
        private Button buttonMaxField;
        private Button buttonMinField;

        private static Texture linkedIcon;
        private static Texture unlinkedIcon;
        private VisualElement _constrainedElementFixed;
        private VisualElement _constrainedElementMax;
        private VisualElement _constrainedElementMin;

        public Vector3ModeElement(VisualElement root, string enumField, string fixedField, string maxField, string minField, string useToggle, bool allowUniform, string enumFieldSerializableName, string fixedFieldSerializableName, string maxFieldSerializableName, string minFieldSerializableName, string useToggleSerializableName, string maxFieldUniformSerializableName, string minFieldUniformSerializableName)
        {
            Init(root, enumField, fixedField, maxField, minField, useToggle, enumFieldSerializableName, fixedFieldSerializableName, maxFieldSerializableName, minFieldSerializableName, useToggleSerializableName);

            this.allowUniform = allowUniform;

            if (this.allowUniform)
            {
                string maxFieldName = maxField + "-uniform";
                string minFieldName = minField + "-uniform";

                maxFieldUniform = root.Find<Vector3Field>(maxFieldName, maxFieldUniformSerializableName);
                minFieldUniform = root.Find<Vector3Field>(minFieldName, minFieldUniformSerializableName);

#if HARPIA_DEBUG
                if (maxFieldUniform == null) Debug.LogError($"Could not find field {maxFieldName}");
                if (minFieldUniform == null) Debug.LogError($"Could not find field {minFieldName}");
#endif

                minFieldUniform.RegisterValueChangedCallback(OnUniformFieldChanged);
                maxFieldUniform.RegisterValueChangedCallback(OnUniformFieldChanged);
            }

            this.enumField.Init(this.allowUniform ? PrefabBrush.Vector3ModeUniform.Fixed : PrefabBrush.Vector3Mode.Fixed);
            this.enumField.RegisterValueChangedCallback(OnEnumValueChanged);

            OnUseToggleValueChanged(null);
            UpdateUIVec3Element();

            AddManipulators(maxFieldUniform);
            AddManipulators(minFieldUniform);
        }

        private void OnUniformFieldChanged(ChangeEvent<Vector3> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"{PrefabBrush.DebugLogStart} Changed uniform field");
#endif

            Vector3 old = evt.previousValue;
            Vector3 newValue = evt.newValue;
            if (old == newValue && newValue.x == newValue.y && newValue.y == newValue.z) return;

            Vector3Field field = (Vector3Field)evt.target;

            //Z
            if (old.x != newValue.x)
            {
                field.value = Vector3.one * newValue.x;
                PB_PrecisionModeManager.UpdateTransformValues();
                return;
            }

            //Y
            if (old.y != newValue.y)
            {
                field.value = Vector3.one * newValue.y;
                PB_PrecisionModeManager.UpdateTransformValues();
                return;
            }

            //z
            field.value = Vector3.one * newValue.z;
            PB_PrecisionModeManager.UpdateTransformValues();
        }

        void UpdateConstrainedUI(VisualElement e, bool value)
        {
            const string tooltipEnable = "Enable Constrained Proportions";
            const string tooltipDisable = "Disable Constrained Proportions";

            if (linkedIcon == null)
            {
                linkedIcon = EditorGUIUtility.IconContent("d_Linked").image;
                unlinkedIcon = EditorGUIUtility.IconContent("d_Unlinked").image;
            }

            e.SetBackgroundTexture(value ? linkedIcon : unlinkedIcon);
            e.tooltip = value ? tooltipDisable : tooltipEnable;
        }

        public void AddProportions(string elementNameFixed, string elementNameMax, string elementNameMin, VisualElement root)
        {
            _constrainedElementFixed = root.Q<VisualElement>(elementNameFixed);
            UpdateConstrainedUI(_constrainedElementFixed, proportionsFixed);

            _constrainedElementMax = root.Q<VisualElement>(elementNameMax);
            UpdateConstrainedUI(_constrainedElementMax, proportionsMax);

            _constrainedElementMin = root.Q<VisualElement>(elementNameMin);
            UpdateConstrainedUI(_constrainedElementMin, proportionsMin);

            _constrainedElementFixed.RegisterCallback<ClickEvent>(_ => {
                proportionsFixed = !proportionsFixed;
                UpdateConstrainedUI(_constrainedElementFixed, proportionsFixed);
            });

            _constrainedElementMax.RegisterCallback<ClickEvent>(_ => {
                proportionsMax = !proportionsMax;
                UpdateConstrainedUI(_constrainedElementMax, proportionsMax);
            });

            _constrainedElementMin.RegisterCallback<ClickEvent>(_ => {
                proportionsMin = !proportionsMin;
                UpdateConstrainedUI(_constrainedElementMin, proportionsMin);
            });

            fixedField.RegisterValueChangedCallback(evt => {
                if (!proportionsFixed) return;
                fixedField.SetValueWithoutNotify(GetConstrainedValue(evt.previousValue, evt.newValue));
            });

            maxField.RegisterValueChangedCallback(evt => {
                if (!proportionsMax) return;
                maxField.SetValueWithoutNotify(GetConstrainedValue(evt.previousValue, evt.newValue));
            });

            minField.RegisterValueChangedCallback(evt => {
                if (!proportionsMin) return;
                minField.SetValueWithoutNotify(GetConstrainedValue(evt.previousValue, evt.newValue));
            });

            Vector3 GetConstrainedValue(Vector3 oldValue, Vector3 newValue)
            {
                if (oldValue is{ x: 0f, y: 0f, z: 0f })
                {
                    float firstDiff = newValue.x + newValue.y + newValue.z;
                    return Vector3.one * firstDiff;
                }

                bool isX = oldValue.x != newValue.x;
                bool isY = oldValue.y != newValue.y;
                bool isZ = oldValue.z != newValue.z;
                if (isX && isY && isZ) return newValue;

                float increase = 1;

                if (isX) increase = newValue.x / oldValue.x;
                if (isY) increase = newValue.y / oldValue.y;
                if (isZ)
                {
                    increase = newValue.z / oldValue.z;
                }

                if (isX) newValue = new Vector3(newValue.x, newValue.y * increase, newValue.z * increase);
                if (isY) newValue = new Vector3(newValue.x * increase, newValue.y, newValue.z * increase);
                if (isZ) newValue = new Vector3(newValue.x * increase, newValue.y * increase, newValue.z);

                if (float.IsNaN(newValue.x) || float.IsInfinity(newValue.x)) return oldValue;
                if (float.IsNaN(newValue.y) || float.IsInfinity(newValue.y)) return oldValue;
                if (float.IsNaN(newValue.z) || float.IsInfinity(newValue.z)) return oldValue;

                const float round = 0.001f;
                newValue = new Vector3(PrefabBrushTool.RoundTo(newValue.x, round), PrefabBrushTool.RoundTo(newValue.y, round), PrefabBrushTool.RoundTo(newValue.z, round));

                return newValue;
            }
        }

        public void SetButtons(Button fixedButton, Button minButton, Button maxButton)
        {
            buttonFixedField = fixedButton;
            buttonMaxField = maxButton;
            buttonMinField = minButton;

            OnUseToggleValueChanged(null);
            UpdateUIVec3Element();
        }

        private void OnEnumValueChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            // Debug.Log($"[{nameof(Vector3ModeElement)}] {nameof(OnEnumValueChanged)} ");
#endif

            bool toggleValue = useToggle?.value ?? true;

            int modeInt = Convert.ToInt32(enumField.value);

            enumField.SetActive(toggleValue);

            bool enableFixed = modeInt == (int)PrefabBrush.Vector3Mode.Fixed && toggleValue;
            fixedField.SetActive(enableFixed);
            buttonFixedField?.SetActive(enableFixed);

            bool enableMax = modeInt == (int)PrefabBrush.Vector3Mode.Random && toggleValue;
            maxField.SetActive(enableMax);
            buttonMaxField?.SetActive(enableMax);

            bool enableMin = modeInt == (int)PrefabBrush.Vector3Mode.Random && toggleValue;
            minField.SetActive(enableMin);
            buttonMinField?.SetActive(enableMin);

            if (allowUniform)
            {
                bool enableUniform = modeInt == 2 && toggleValue;
                maxFieldUniform.SetActive(enableUniform);
                minFieldUniform.SetActive(enableUniform);
            }
        }

        public override Vector3 GetValue(bool canGenerateRandom, RaycastHit hit)
        {
            int mode = Convert.ToInt16(enumField.value);

            switch (mode)
            {
                //Fixed
                case (int)PrefabBrush.Vector3Mode.Fixed:
                    return fixedField.value;
                //Random
                case (int)PrefabBrush.Vector3ModeUniform.Random:
                {
                    return GetRandomValue(canGenerateRandom);
                }
                default:
                {
                    //Random Uniform
                    float randomValue = Random.Range(minFieldUniform.value.x, maxFieldUniform.value.x);
                    return new Vector3(randomValue, randomValue, randomValue);
                }
            }
        }

        public Vector3 GetValueFixed()
        {
            return fixedField.value;
        }

        public void RegisterFocusEvents()
        {
            minField.RegisterFocusEvents_PB();
            maxField.RegisterFocusEvents_PB();
            fixedField.RegisterFocusEvents_PB();
        }

        public void TrySetValue(PrefabBrush.Vector3Mode desiredMode, Vector3 newVal)
        {
            PrefabBrush.Vector3Mode current = (PrefabBrush.Vector3Mode)enumField.value;
            if (current != desiredMode) return;
            fixedField.SetValueWithoutNotify(newVal);
        }

        public void SetValues(bool isUsing, int mode, Vector3 minValue, Vector3 maxValue, Vector3 fixedValue, bool useUniform)
        {
            minField.SetValueWithoutNotify(minValue);
            maxField.SetValueWithoutNotify(maxValue);
            fixedField.SetValueWithoutNotify(fixedValue);
            enumField.SetValueWithoutNotify(useUniform ? (PrefabBrush.Vector3ModeUniform)mode : (PrefabBrush.Vector3Mode)mode);
            if (useToggle != null) useToggle.SetValueWithoutNotify(isUsing);

            UpdateUIVec3Element();
        }

        public void SerializeFor(SerializedObject o, IBindable b)
        {
            b.BindProperty(o);
        }

        public void SetValueFixed(Vector3 newValue, bool changeModeToFixed)
        {
            fixedField.SetValueWithoutNotify(newValue * PrefabBrush.instance.advancedSettings.scrollRotationSpeed.value);
            if (changeModeToFixed) enumField.value = PrefabBrush.Vector3Mode.Fixed;
        }

        public PrefabBrush.Vector3Mode GetMode()
        {
            return (PrefabBrush.Vector3Mode)enumField.value;
        }

        public void SetActive(bool b)
        {
            if (b)
            {
                enumField.SetActive(true);
                useToggle?.SetActive(true);
                UpdateUIVec3Element();
                return;
            }

            minField.SetActive(false);
            maxField.SetActive(false);
            fixedField.SetActive(false);
            enumField.SetActive(false);
            useToggle?.SetActive(false);

            buttonFixedField?.SetActive(false);
            buttonMaxField?.SetActive(false);
            buttonMinField?.SetActive(false);
        }

        public sealed override void UpdateUIVec3Element()
        {
            OnEnumValueChanged(null);
        }

        public void SetConstrainedProportions(bool constrainedScaleFixed, bool constrainedScaleMin, bool constrainedScaleMax)
        {
            proportionsFixed = constrainedScaleFixed;
            proportionsMin = constrainedScaleMin;
            proportionsMax = constrainedScaleMax;

            UpdateConstrainedUI(_constrainedElementFixed, proportionsFixed);
            UpdateConstrainedUI(_constrainedElementMax, proportionsMax);
            UpdateConstrainedUI(_constrainedElementMin, proportionsMin);
        }
    }

    public class PB_AdvancedSettings
    {
        public readonly ColorField brushBorderColor;
        public readonly ColorField brushBaseColor;
        public readonly ColorField invalidLocationColor;
        public readonly ColorField gridColor;
        public readonly ColorField eraserColor;

        private VisualElement settingsPanel;
        private Color originalInvalidLocationColor1;
        private Color originalGridColor1;
        private Color originalEraserColor1;
        private Color originalBrushBorderColor1;
        private Color originalBrushBaseColor1;
        public readonly FloatField scrollRotationSpeed;

        public PB_AdvancedSettings(VisualElement root)
        {
            settingsPanel = root.Q("settings-section");

            brushBaseColor = settingsPanel.Q<ColorField>("settings-brush-color");
            brushBorderColor = settingsPanel.Q<ColorField>("settings-brush-border-color");
            eraserColor = settingsPanel.Q<ColorField>("settings-eraser-color");
            gridColor = settingsPanel.Q<ColorField>("settings-grid-color");
            invalidLocationColor = settingsPanel.Q<ColorField>("settings-invalid-location-color");

            originalBrushBaseColor1 = brushBaseColor.value;
            originalBrushBorderColor1 = brushBorderColor.value;
            originalEraserColor1 = eraserColor.value;
            originalGridColor1 = gridColor.value;
            originalInvalidLocationColor1 = invalidLocationColor.value;

            brushBaseColor.RegisterFocusEvents_PB();
            brushBorderColor.RegisterFocusEvents_PB();
            eraserColor.RegisterFocusEvents_PB();
            gridColor.RegisterFocusEvents_PB();
            invalidLocationColor.RegisterFocusEvents_PB();

            scrollRotationSpeed = settingsPanel.Find<FloatField>("settings-scroll-rotation", PrefabBrushTemplate.ScrollRotationSpeed);
            scrollRotationSpeed.SetMinValue(0.01f);
            scrollRotationSpeed.RegisterEditorPrefs("PB", 1);

            brushBaseColor.value = EditorPrefsExtension.GetColor("pb_brush_base_color", brushBaseColor.value);
            brushBorderColor.value = EditorPrefsExtension.GetColor("pb_brush_border_color", brushBorderColor.value);
            eraserColor.value = EditorPrefsExtension.GetColor("pb_eraser_color", eraserColor.value);
            gridColor.value = EditorPrefsExtension.GetColor("pb_grid_color", gridColor.value);
            invalidLocationColor.value = EditorPrefsExtension.GetColor("pb_invalid_location_color", invalidLocationColor.value);

            settingsPanel.Q<Button>("settings-reset").RegisterCallback<ClickEvent>(OnResetButton);
            SetActive(false);
        }

        public PB_AdvancedSettings(FloatField scrollRotationSpeed)
        {
            this.scrollRotationSpeed = scrollRotationSpeed;
        }

        public void SetActive(bool n)
        {
            if (settingsPanel == null)
            {
                settingsPanel = PrefabBrush.instance.rootVisualElement.Q<VisualElement>("settings-section");
            }

            settingsPanel.SetActive(n);
        }

        private void OnResetButton(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log("Resetting PB settings");
#endif
            //show confirmation dialog
            bool r = EditorUtility.DisplayDialog("Reset Prefab Brush Settings", "Are you sure you want to reset all Prefab Brush settings to their default values?", "Yes", "No");

            if (!r) return;

            DeleteKeys();

            brushBaseColor.value = originalBrushBaseColor1;
            brushBorderColor.value = originalBrushBorderColor1;
            eraserColor.value = originalEraserColor1;
            gridColor.value = originalGridColor1;
            invalidLocationColor.value = originalInvalidLocationColor1;
        }

        public void SaveValues()
        {
            if (brushBaseColor != null) EditorPrefsExtension.SaveColor("pb_brush_base_color", brushBaseColor.value);
            if (brushBorderColor != null) EditorPrefsExtension.SaveColor("pb_brush_border_color", brushBorderColor.value);
            if (eraserColor != null) EditorPrefsExtension.SaveColor("pb_eraser_color", eraserColor.value);
            if (gridColor != null) EditorPrefsExtension.SaveColor("pb_grid_color", gridColor.value);
            if (invalidLocationColor != null) EditorPrefsExtension.SaveColor("pb_invalid_location_color", invalidLocationColor.value);
        }

        private static void DeleteKeys()
        {
            EditorPrefsExtension.DeleteColorKey("pb_brush_base_color");
            EditorPrefsExtension.DeleteColorKey("pb_brush_border_color");
            EditorPrefsExtension.DeleteColorKey("pb_eraser_color");
            EditorPrefsExtension.DeleteColorKey("pb_grid_color");
            EditorPrefsExtension.DeleteColorKey("pb_invalid_location_color");
        }

        static void RegisterEvent(FloatField baseField, Slider target)
        {
            baseField.RegisterValueChangedCallback((evt) => {
                if (evt.newValue < .1f)
                {
                    baseField.SetValueWithoutNotify(.1f);
                    return;
                }

                target.highValue = evt.newValue;
                float v = Mathf.Clamp(target.value, target.lowValue, target.highValue);
                target.SetValueWithoutNotify(v);
            });
        }
    }

    public class PB_EditorInputSliderMinMax : EditorWindow
    {
        private Vector3 ret;
        private bool shouldClose;
        private Slider targetSlider1;
        private bool initializedPosition;
        public float _max;
        public float _min;
        public string prefsKey;

        private void OnGUI()
        {
            if (shouldClose)
            {
                Close();
                return;
            }

            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Escape:
                        shouldClose = true;
                        break;

                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        ApplyAndClose();
                        break;
                }
            }

            Rect rect = EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(12);

            //High Value
            _max = EditorGUILayout.FloatField("Max Value", _max);
            EditorGUILayout.Space(12);
            if (_max < 0.001f) _max = 0.001f;
            if (_max <= _min) _max = _min + 0.001f;

            //Low Value
            _min = EditorGUILayout.FloatField("Min Value", _min);
            if (_min < 0.001f) _min = 0.001f;
            if(_min >= _max) _min = _max - 0.001f;
            

            EditorGUILayout.Space(12);

            Rect r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if (GUI.Button(r, "Cancel"))
            {
                shouldClose = true;
                return;
            }

            r.x += r.width;
            if (GUI.Button(r, "Update Slider Values"))
            {
                ApplyAndClose();
                return;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size) minSize = maxSize = rect.size;

            if (!initializedPosition)
            {
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x + 32, mousePos.y, position.width, position.height);
                initializedPosition = true;
            }
        }

        private void ApplyAndClose()
        {
            if (_min > _max)
            {
                EditorUtility.DisplayDialog("Error", "Min value must be less than Max value", "Ok");
                return;
            }
            
            if (_max < _min)
            {
                EditorUtility.DisplayDialog("Error", "Max value must be greater than Min value", "Ok");
                return;
            }
            
            targetSlider1.lowValue = _min;
            targetSlider1.highValue = _max;
            targetSlider1.value = Mathf.Clamp(targetSlider1.value, targetSlider1.lowValue, targetSlider1.highValue);

            if (!string.IsNullOrEmpty(prefsKey))
            {
                EditorPrefs.SetFloat(prefsKey + "_min", _min);
                EditorPrefs.SetFloat(prefsKey + "_max", _max);
            }

            shouldClose = true;
        }

        public static void LoadSliderMinMaxValues(Slider target, string prefsKey)
        {
            string minKey = prefsKey + "_min";
            string maxKey = prefsKey + "_max";

            if (EditorPrefs.HasKey(minKey)) target.lowValue = EditorPrefs.GetFloat(minKey);
            if (EditorPrefs.HasKey(maxKey)) target.highValue = EditorPrefs.GetFloat(maxKey);
        }

        public static void Show(string title, Slider value, string prefsKey)
        {
            string ret = null;
            PB_EditorInputSliderMinMax currentWindow = CreateInstance<PB_EditorInputSliderMinMax>();

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_EditorInputSliderMinMax)}] Showing window");
#endif

            currentWindow.targetSlider1 = value;
            currentWindow._min = value.lowValue;
            currentWindow._max = value.highValue;
            currentWindow.prefsKey = prefsKey;

            currentWindow.titleContent = new GUIContent(title);
            currentWindow.ShowModal();
        }
    }

    public class PB_EditorInputDialog : EditorWindow
    {
        private string okButton;
        private string inputText;
        private string description;
        private string cancelButton;
        private bool initializedPosition;
        private Action onOKButton;

        private bool shouldClose;

        private void OnGUI()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Escape:
                        shouldClose = true;
                        break;

                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        onOKButton?.Invoke();
                        shouldClose = true;
                        break;
                }
            }

            if (shouldClose)
            {
                Close();
            }

            Rect rect = EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(12);

            EditorGUILayout.LabelField(description);
            EditorGUILayout.Space(8);
            GUI.SetNextControlName("inText");
            inputText = EditorGUILayout.TextField("", inputText);
            GUI.FocusControl("inText");
            EditorGUILayout.Space(12);

            Rect r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if (GUI.Button(r, okButton))
            {
                onOKButton?.Invoke();
                shouldClose = true;
            }

            r.x += r.width;
            if (GUI.Button(r, cancelButton))
            {
                inputText = null; // Cancel - delete inputText
                shouldClose = true;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size) minSize = maxSize = rect.size;

            if (!initializedPosition)
            {
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x + 32, mousePos.y, position.width, position.height);
                initializedPosition = true;
            }
        }

        public static string Show(string title, string description, string inputText, string okButton = "OK", string cancelButton = "Cancel")
        {
            string ret = null;
            PB_EditorInputDialog wnd = CreateInstance<PB_EditorInputDialog>();
            wnd.titleContent = new GUIContent(title);
            wnd.description = description;
            wnd.inputText = inputText;
            wnd.okButton = okButton;
            wnd.cancelButton = cancelButton;
            wnd.onOKButton += () => ret = wnd.inputText;
            wnd.ShowModal();

            return ret;
        }
    }

    public static class PB_ShaderProps
    {
        public static readonly int Epsilon = Shader.PropertyToID("epsilon");
        public static readonly int WorldToLocalMatrix = Shader.PropertyToID("worldToLocalMatrix");
        public static readonly int VertexBuffer = Shader.PropertyToID("vertexBuffer");
        public static readonly int TriangleBuffer = Shader.PropertyToID("triangleBuffer");
        public static readonly int ResultHits = Shader.PropertyToID("resultHits");
        public static readonly int RayOrigin = Shader.PropertyToID("rayOrigin");
        public static readonly int RayDirection = Shader.PropertyToID("rayDirection");
        public static readonly int ResultNormals = Shader.PropertyToID("resultNormals");
        public static readonly int ClosestPoint = Shader.PropertyToID("closestPoint");
        public static readonly int DistanceToCamera = Shader.PropertyToID("hitDistanceToCamera");
        public static readonly int MinDistance = Shader.PropertyToID("minDistance");

        public static void FindShader(string shaderName, ref ComputeShader computeShader)
        {
            //find an asset called raycastShaderName
            string[] guids = AssetDatabase.FindAssets(shaderName);

            if (guids.Length == 0)
            {
                Debug.LogError($"{PrefabBrush.DebugLogStart} {shaderName} shader not found");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
        }
    }

    public static class PB_VertexFinder
    {
        private static int kernelIndexVertex;
        private static uint threadGroupSizeX;
        private static uint threadGroupSizeY;
        private static uint threadGroupSizeZ;

        private static ComputeBuffer trianglesBuffer;
        private static ComputeBuffer vertexBuffer;
        private static ComputeBuffer resultClosestPointsBuffer;
        private static ComputeBuffer resultDistancesToCameraBuffer;

        private static Vector3[] resultClosestPointsArray;
        private static float[] resultDistanceToCameraArray;

        private static ComputeShader vertexShader;
        private const string VertexShaderName = "PrefabBrush_VertexFinder";
        private static int sizeX1;

        //We need the ray, so we do not get invisible vertices
        public static bool GetClosestVertex(Vector3 hitPoint, float minDistance, Ray r, Mesh m, Matrix4x4 matrix, out Vector3 result)
        {
            InitVertex();

            if (m.vertices.Length == 0)
            {
                result = Vector3.zero;
                return false;
            }

#if HARPIA_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample("Prefab Brush - Vertex Finder");
#endif

            //Inputs
            if (vertexBuffer == null)
            {
#if HARPIA_DEBUG
                Debug.Log($"[{nameof(PB_VertexFinder)}] Initializing vertex Shader - {m.vertices.Length} vertices");
#endif

                vertexShader.SetFloat(PB_ShaderProps.Epsilon, Mathf.Epsilon);
                vertexShader.SetMatrix(PB_ShaderProps.WorldToLocalMatrix, matrix);

                vertexBuffer = new ComputeBuffer(m.vertices.Length, sizeof(float) * 3);
                vertexBuffer.SetData(m.vertices);
                vertexShader.SetBuffer(kernelIndexVertex, PB_ShaderProps.VertexBuffer, vertexBuffer);

                trianglesBuffer = new ComputeBuffer(m.triangles.Length, sizeof(int));
                trianglesBuffer.SetData(m.triangles);
                vertexShader.SetBuffer(kernelIndexVertex, PB_ShaderProps.TriangleBuffer, trianglesBuffer);

                int length = m.triangles.Length / 3;

                resultDistanceToCameraArray = new float[length];
                resultDistancesToCameraBuffer = new ComputeBuffer(length, sizeof(float));
                resultDistancesToCameraBuffer.SetData(resultDistanceToCameraArray);
                vertexShader.SetBuffer(kernelIndexVertex, PB_ShaderProps.DistanceToCamera, resultDistancesToCameraBuffer);

                resultClosestPointsArray = new Vector3[length];
                resultClosestPointsBuffer = new ComputeBuffer(length, sizeof(float) * 3);
                resultClosestPointsBuffer.SetData(resultClosestPointsArray);
                vertexShader.SetBuffer(kernelIndexVertex, PB_ShaderProps.ClosestPoint, resultClosestPointsBuffer);

                sizeX1 = Mathf.CeilToInt(((float)length / threadGroupSizeX) + 1);
            }

            vertexShader.SetVector(PB_ShaderProps.RayOrigin, r.origin);
            vertexShader.SetVector(PB_ShaderProps.RayDirection, r.direction);
            vertexShader.SetFloat(PB_ShaderProps.MinDistance, minDistance);

            //Error here
            vertexShader.Dispatch(kernelIndexVertex, sizeX1, (int)threadGroupSizeY, (int)threadGroupSizeZ);

            resultClosestPointsBuffer.GetData(resultClosestPointsArray);
            resultDistancesToCameraBuffer.GetData(resultDistanceToCameraArray);

            float maxDistanceToCamera = Vector3.Distance(r.origin, hitPoint) + 0.1f;
            int resultIndex = -1;
            for (int index = resultClosestPointsArray.Length - 1; index >= 0; index--)
            {
                Vector3 point = resultClosestPointsArray[index];
                if (point.x >= 9_000_000)
                {
                    continue;
                }

                float distance = resultDistanceToCameraArray[index];

                if (distance < maxDistanceToCamera)
                {
                    maxDistanceToCamera = distance;
                    resultIndex = index;
                }
            }

            //No vertex found
            if (resultIndex == -1)
            {
                result = Vector3.zero;
                return false;
            }

            result = resultClosestPointsArray[resultIndex];

#if HARPIA_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif

            return true;
        }

        public static void Dispose()
        {
            trianglesBuffer?.Dispose();
            vertexBuffer?.Dispose();
            resultClosestPointsBuffer?.Dispose();
            resultDistancesToCameraBuffer?.Dispose();

            resultClosestPointsArray = null;
            resultDistanceToCameraArray = null;
            trianglesBuffer = null;
            vertexBuffer = null;
        }

        private static void InitVertex()
        {
            if (vertexShader != null) return;

            PB_ShaderProps.FindShader(VertexShaderName, ref vertexShader);
            kernelIndexVertex = vertexShader.FindKernel("FindVertexCS");
            vertexShader.GetKernelThreadGroupSizes(kernelIndexVertex, out threadGroupSizeX, out threadGroupSizeY, out threadGroupSizeZ);

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_VertexFinder)}] {vertexShader.name} index {kernelIndexVertex} ");
#endif
        }
    }

    public static class PB_MeshRaycaster
    {
        private static int kernelIndexRaycast;
        private static uint threadGroupSizeX;
        private static uint threadGroupSizeY;
        private static uint threadGroupSizeZ;

        private static ComputeShader raycastShader;
        private const string RaycastShaderName = "PrefabBrush_MeshRaycaster";

        private static ComputeBuffer trianglesBuffer;
        private static ComputeBuffer vertexBuffer;
        private static ComputeBuffer resultBufferHits;
        private static ComputeBuffer resultNormalsBuffer;
        private static float[] hitDistances;
        private static Vector3[] resultNormals;
        private static int sizeX1;

        public static bool Raycast(Ray r, Mesh batchMesh, out MeshRaycastResult result)
        {
            InitRaycast();

            if (batchMesh.vertices.Length == 0)
            {
                result = new MeshRaycastResult();
                return false;
            }

#if HARPIA_DEBUG
            UnityEngine.Profiling.Profiler.BeginSample("Prefab Brush - Mesh Raycaster");
#endif

            //Inputs
            if (vertexBuffer == null)
            {
#if HARPIA_DEBUG
                Debug.Log($"[PB Mesh Raycaster] Initializing raycastShader - {batchMesh.vertices.Length} vertices");
#endif

                UpdateMesh(batchMesh);
            }

            raycastShader.SetVector(PB_ShaderProps.RayOrigin, r.origin);
            raycastShader.SetVector(PB_ShaderProps.RayDirection, r.direction);

            raycastShader.Dispatch(kernelIndexRaycast, sizeX1, (int)threadGroupSizeY, (int)threadGroupSizeZ);

            resultBufferHits.GetData(hitDistances);
            resultNormalsBuffer.GetData(resultNormals);

            float maxDistance = Mathf.Infinity;
            int resultIndex = -1;
            for (int index = hitDistances.Length - 1; index >= 0; index--)
            {
                float distance = hitDistances[index];
                if (distance >= 1_000_000) continue;
                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    resultIndex = index;
                }
            }

            //No collision ray
            if (resultIndex == -1)
            {
                result = new MeshRaycastResult();
                return false;
            }

            Vector3 pos = r.origin + r.direction * maxDistance;

            result = new MeshRaycastResult(pos, resultNormals[resultIndex], maxDistance);

#if HARPIA_DEBUG
            UnityEngine.Profiling.Profiler.EndSample();
#endif

            return true;
        }

        public static void Dispose()
        {
            trianglesBuffer?.Dispose();
            vertexBuffer?.Dispose();
            resultBufferHits?.Dispose();
            resultNormalsBuffer?.Dispose();

            hitDistances = null;
            trianglesBuffer = null;
            vertexBuffer = null;
            resultBufferHits = null;
            resultNormalsBuffer = null;
        }

        private static void InitRaycast()
        {
            if (raycastShader != null) return;

            PB_ShaderProps.FindShader(RaycastShaderName, ref raycastShader);
            kernelIndexRaycast = raycastShader.FindKernel("MeshRaycastCS");
            raycastShader.GetKernelThreadGroupSizes(kernelIndexRaycast, out threadGroupSizeX, out threadGroupSizeY, out threadGroupSizeZ);
        }

        public static void UpdateMesh(Mesh batchMesh)
        {
            if (batchMesh == null) return;

            InitRaycast();

            raycastShader.SetFloat(PB_ShaderProps.Epsilon, Mathf.Epsilon);
            raycastShader.SetMatrix(PB_ShaderProps.WorldToLocalMatrix, Matrix4x4.identity);

            vertexBuffer = new ComputeBuffer(batchMesh.vertices.Length, sizeof(float) * 3);
            vertexBuffer.SetData(batchMesh.vertices);
            raycastShader.SetBuffer(kernelIndexRaycast, PB_ShaderProps.VertexBuffer, vertexBuffer);

            trianglesBuffer = new ComputeBuffer(batchMesh.triangles.Length, sizeof(int));
            trianglesBuffer.SetData(batchMesh.triangles);
            raycastShader.SetBuffer(kernelIndexRaycast, PB_ShaderProps.TriangleBuffer, trianglesBuffer);

            hitDistances = new float[batchMesh.triangles.Length / 3];
            resultBufferHits = new ComputeBuffer(hitDistances.Length, sizeof(float));
            resultBufferHits.SetData(hitDistances);
            raycastShader.SetBuffer(kernelIndexRaycast, PB_ShaderProps.ResultHits, resultBufferHits);

            resultNormals = new Vector3[hitDistances.Length];
            resultNormalsBuffer = new ComputeBuffer(hitDistances.Length, sizeof(float) * 3);
            resultNormalsBuffer.SetData(resultNormals);
            raycastShader.SetBuffer(kernelIndexRaycast, PB_ShaderProps.ResultNormals, resultNormalsBuffer);

            sizeX1 = Mathf.CeilToInt((float)hitDistances.Length / threadGroupSizeX + 1);
        }
    }

    public readonly struct MeshRaycastResult
    {
        private readonly Vector3 position;
        private readonly Vector3 normal;
        private readonly float distance;

        //constructor
        public MeshRaycastResult(Vector3 position, Vector3 normal, float distance)
        {
            this.position = position;
            this.normal = normal;
            this.distance = distance;
        }

        public RaycastHit ToHitInfo()
        {
            RaycastHit hit = new RaycastHit{ point = position, normal = normal, distance = distance };
            return hit;
        }
    }

    public static class PB_PressurePen
    {
        public enum PenPressureUseMode
        {
            Affect_Brush_Strength,
            Affect_Brush_Radius,
        }

        private static DefaultEnumField penPressureMode;
        private static Toggle usePressureToggle;

        private const string usePressureKey = "PB-use-pressure";
        private const string penPressureModeKey = "PB-pressure-mode";

        private static Vector2 radiusMinMax;
        private static Vector2 strengthMinMax;
        private static float oldStrength;
        private static float oldBrushRadius;

        public static void Init(VisualElement root)
        {
            usePressureToggle = root.Find<Toggle>("use-pressure-toggle", PrefabBrushTemplate.UsePenToogle);

            penPressureMode = root.Q<DefaultEnumField>("pen-pressure-mode");
            penPressureMode.Init(PenPressureUseMode.Affect_Brush_Radius);
            penPressureMode.BindProperty(PrefabBrush.instance._serializedObject.FindProperty(PrefabBrushTemplate.PenPressureMode));

#if !UNITY_2022_1_OR_NEWER
            penPressureMode.SetActive(false);
            usePressureToggle.SetActive(false);
#else
            Label msg = root.Q<Label>("pen-pressure-message");
            msg.SetActive(false);

            usePressureToggle.value = EditorPrefs.GetBool(usePressureKey, false);
            usePressureToggle.RegisterValueChangedCallback(OnToggleChanged);
            usePressureToggle.SetActive(true);

            penPressureMode.Init(PenPressureUseMode.Affect_Brush_Radius);
            penPressureMode.SetValueWithoutNotify((PenPressureUseMode)EditorPrefs.GetInt(penPressureModeKey, (int)PenPressureUseMode.Affect_Brush_Strength));
            penPressureMode.RegisterValueChangedCallback(OnDropdownChanged);

            Slider sliderRadius = PrefabBrush.instance.brushRadiusSlider;
            Slider strengthSlider = PrefabBrush.instance.sliderBrushStrength;

            radiusMinMax = new Vector2(sliderRadius.lowValue, sliderRadius.highValue);
            strengthMinMax = new Vector2(strengthSlider.lowValue, strengthSlider.highValue);
#endif
        }

        private static void OnDropdownChanged(ChangeEvent<Enum> changeEvent)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_PressurePen)}] Changed dropdown");
#endif

            EditorPrefs.SetInt(penPressureModeKey, (int)(PenPressureUseMode)changeEvent.newValue);
        }

        private static void OnToggleChanged(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"Toggle changed to {evt.newValue}");
#endif
            EditorPrefs.SetBool(usePressureKey, usePressureToggle.value);
            penPressureMode.SetActive(usePressureToggle.value);
        }

        public static void Update()
        {
#if UNITY_2022_1_OR_NEWER
            if (!usePressureToggle.value) return;

            float pressure = Event.current.pressure * Event.current.pressure;
            PenPressureUseMode mode = (PenPressureUseMode)penPressureMode.value;

            PB_HandlesExtension.WriteTempText("Pressure: " + pressure.ToString("f3"));

            switch (mode)
            {
                case PenPressureUseMode.Affect_Brush_Radius:
                    PrefabBrush.instance.brushRadiusSlider.value = PrefabBrushTool.Lerp(radiusMinMax, pressure);
                    break;
                case PenPressureUseMode.Affect_Brush_Strength:
                    PrefabBrush.instance.sliderBrushStrength.value = PrefabBrushTool.Lerp(strengthMinMax, pressure);
                    break;
            }
#endif
        }

        public static void OnMouseDown()
        {
#if !UNITY_2022_1_OR_NEWER
            return;
#else
            if (!usePressureToggle.value) return;
            oldBrushRadius = PrefabBrush.instance.brushRadiusSlider.value;
            oldStrength = PrefabBrush.instance.sliderBrushStrength.value;
#endif
        }

        public static void OnMouseUp()
        {
#if !UNITY_2022_1_OR_NEWER
            return;
#else
            if (!usePressureToggle.value) return;
            PrefabBrush.instance.brushRadiusSlider.value = oldBrushRadius;
            PrefabBrush.instance.sliderBrushStrength.value = oldStrength;
#endif
        }
    }

    public static class PB_ModularShortcuts
    {
        private const string KeyStart = "pb-shortcut-";
        private static VisualElement holder;

        public static ShortcutData increaseRadius;
        public static ShortcutData decreaseRadius;
        public static ShortcutData rotateRight;
        public static ShortcutData rotateLeft;
        public static ShortcutData randomRotation;
        public static ShortcutData nextPrefab;
        public static ShortcutData previousPrefab;
        public static ShortcutData normalizeSize;
        public static ShortcutData changeMode;
        public static ShortcutData exitTool;
        public static ShortcutData freeRotation;
        public static ShortcutData nextPivot;
        public static ShortcutData previousPivot;

        public static ShortcutData rotationXShortcut;
        public static ShortcutData rotationYShortcut;
        public static ShortcutData rotationZShortcut;
        public static ShortcutData changeScaleShortcut;
        public static ShortcutData yDisplacementShortcut;

        private static List<ShortcutData> allShortCuts;
        private static ShortcutData selectedData1;
        private static Label selectedButton;
        private const string TapKeyText = "Tap Desired Key";

        private static readonly Dictionary<KeyCode, string> cautionShortcuts = new Dictionary<KeyCode, string>(){
            { KeyCode.W, "This key is used on the SceneView to move the camera" },
            { KeyCode.A, "This key is used on the SceneView to move the camera" },
            { KeyCode.S, "This key is used on the SceneView to move the camera" },
            { KeyCode.D, "This key is used on the SceneView to move the camera" },
            { KeyCode.Alpha2, "This key toggle 3D /3 2D mode" },
            // { KeyCode.V, "This code is used on the scene to move objects" },
            // { KeyCode.Z, "This code is used on the scene to toggle gizmos on Pivot / Center" },
            // { KeyCode.X, "This code is used on the scene to toggle gizmos on Local / World view" },
        };

        public class ShortcutData
        {
            public DefaultEnumField shortCutField;
            public readonly string shortCutName;
            private readonly KeyCode _defaultShortcut;
            string EditorPrefsKey => KeyStart + shortCutName;

            public ShortcutData(string shortCutName, KeyCode shortCut)
            {
                _defaultShortcut = shortCut;

                this.shortCutName = shortCutName;
            }

            public KeyCode GetSavedShortcut()
            {
                return (KeyCode)EditorPrefs.GetInt(EditorPrefsKey, (int)_defaultShortcut);
            }

            public bool IsShortcut()
            {
                return IsShortcut(Event.current.keyCode);
            }

            public bool IsShortcut(KeyCode code)
            {
                return (KeyCode)shortCutField.value == code;
            }

            public void ResetShortcut()
            {
                EditorPrefs.DeleteKey(EditorPrefsKey);
                shortCutField.SetValueWithoutNotify(_defaultShortcut);
            }

            public string GetKeyText()
            {
                KeyCode value = (KeyCode)shortCutField.value;
                switch (value)
                {
                    case KeyCode.Escape: return "esc";
                    case KeyCode.Plus: return "+";
                    case KeyCode.Minus: return "-";
                    case KeyCode.Greater: return ">";
                    case KeyCode.Less: return "<";
                    case KeyCode.Backslash: return @"\";
                    case KeyCode.Dollar: return "$";
                    case KeyCode.Hash: return "#";
                    case KeyCode.DoubleQuote: return @"""";
                    case KeyCode.Question: return "?";
                    case KeyCode.Equals: return "=";
                    case KeyCode.Period: return ".";
                    case KeyCode.Comma: return ",";
                }

                string SplitCamelCase(string input)
                {
                    return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                }

                return SplitCamelCase(value.ToString());
            }
        }

        public static void Init(VisualElement root)
        {
            holder = root.Q<VisualElement>("modular-shortcuts-holder");

            increaseRadius = new("Increase Size", KeyCode.Equals);
            ShowShortcut(increaseRadius);
            decreaseRadius = new("Decrease Size", KeyCode.Minus);
            ShowShortcut(decreaseRadius);

            rotateRight = new("Rotate Right", KeyCode.Period);
            ShowShortcut(rotateRight, true);
            rotateLeft = new("Rotate Left", KeyCode.Comma);
            ShowShortcut(rotateLeft);
            randomRotation = new("Random Rotation", KeyCode.M);
            ShowShortcut(randomRotation);

            nextPrefab = new("Next Prefab", KeyCode.F);
            ShowShortcut(nextPrefab, true);

            previousPrefab = new("Previous Prefab", KeyCode.G);
            ShowShortcut(previousPrefab);

            freeRotation = new ShortcutData("Free Rotation", KeyCode.R);
            ShowShortcut(freeRotation, false, "Allows you to free rotate your object in precision mode");

            nextPivot = new ShortcutData("Next Pivot", KeyCode.E);
            ShowShortcut(nextPivot, true, "In precision mode, go to the next pivot of the object");

            previousPivot = new ShortcutData("Previous Pivot", KeyCode.Q);
            ShowShortcut(previousPivot, false, "In precision mode, go to the previous pivot of the object");

            normalizeSize = new ShortcutData("Normalize Size", KeyCode.L);
            ShowShortcut(normalizeSize, true, "Set the object size to 1,1,1");

            changeMode = new("Change Mode", KeyCode.B);
            ShowShortcut(changeMode);

            exitTool = new ShortcutData("Exit Tool", KeyCode.Escape);
            ShowShortcut(exitTool);

            rotationXShortcut = new ShortcutData("Rotate X with ScrollWheel", KeyCode.X);
            ShowShortcut(rotationXShortcut, true);

            rotationYShortcut = new ShortcutData("Rotate Y with ScrollWheel", KeyCode.Y);
            ShowShortcut(rotationYShortcut);

            rotationZShortcut = new ShortcutData("Rotate Z with ScrollWheel", KeyCode.Z);
            ShowShortcut(rotationZShortcut);

            changeScaleShortcut = new ShortcutData("Change Size with ScrollWheel", KeyCode.C);
            ShowShortcut(changeScaleShortcut, true);

            yDisplacementShortcut = new ShortcutData("Y Offset with ScrollWheel", KeyCode.V);
            ShowShortcut(yDisplacementShortcut);

            root.Q<Button>("reset-shortcuts").RegisterCallback<ClickEvent>(ResetShortcuts);
            root.Q<Button>("keycode-button").RegisterCallback<ClickEvent>(OpenKeycodeDoc);
        }

        private static void ShowShortcut(ShortcutData data, bool addSpace = false, string tooltip = null)
        {
            allShortCuts ??= new List<ShortcutData>();
            allShortCuts.Add(data);

            VisualElement newShortcutUI = new(){ style ={ flexDirection = FlexDirection.Row, marginTop = addSpace ? 5 : 0 } };
            if (!string.IsNullOrEmpty(tooltip)) newShortcutUI.tooltip = tooltip;

            Label shortcutName = new(data.shortCutName){ style ={ width = new StyleLength(new Length(180, LengthUnit.Pixel)), marginLeft = new StyleLength(new Length(3, LengthUnit.Pixel)) } };
            newShortcutUI.Add(shortcutName);

            DefaultEnumField keycodeField = new(){ style ={ flexGrow = 1, } };

            data.shortCutField = keycodeField;
            keycodeField.Init(data.GetSavedShortcut());
            keycodeField.RegisterValueChangedCallback(e => OnShortcutEnumChanged(data, keycodeField, e));
            newShortcutUI.Add(keycodeField);

            //Button Get Key
            Label getKeyButton = new Label(){
                text = TapKeyText,
                style ={
                    backgroundColor = new StyleColor(new Color(0.4039216f, 0.4039216f, 0.4039216f, .5f)),
                    borderBottomLeftRadius = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    borderTopLeftRadius = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    borderBottomRightRadius = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    borderTopRightRadius = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    paddingRight = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    paddingLeft = new StyleLength(new Length(3f, LengthUnit.Pixel)),
                    fontSize = 12,
                    marginBottom = 1,
                    marginTop = 1
                }
            };

            getKeyButton.SetBorderColor(new Color(0.1294118f, 0.1294118f, 0.1294118f, 0.5f), 1f);

            getKeyButton.RegisterCallback<ClickEvent>(e => OnAssignKeyButton(data, e));

            newShortcutUI.Add(getKeyButton);
            newShortcutUI.ChangeColorOnHover(new Color(1, 1, 1, .3f));

            holder.Add(newShortcutUI);
        }

        private static void OnAssignKeyButton(ShortcutData data, ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"On assign key button for {data.shortCutName}");
#endif
            Label pressedButton = ((Label)evt.target);

            if (selectedButton != null)
            {
                if (pressedButton == selectedButton)
                {
                    Dispose();
                    return;
                }

                selectedButton.text = TapKeyText;
            }

            selectedButton = pressedButton;
            selectedData1 = data;
            selectedButton.text = "Waiting For Input...";
        }

        private static void OnShortcutEnumChanged(ShortcutData data, DefaultEnumField keycodeField, ChangeEvent<Enum> changeEvent)
        {
            KeyCode keyCode = (KeyCode)keycodeField.value;
            string key = KeyStart + data.shortCutName;

#if HARPIA_DEBUG
            Debug.Log($"Changed shortcut {data.shortCutName} | new value {keyCode} | key = {key} ");
#endif
            Dispose();

            //Check if it can use
            if (cautionShortcuts.TryGetValue(keyCode, out string shortcut))
            {
                PrefabBrush.DisplayError($"Cannot use shortcut {keyCode.ToString()}: {shortcut}");
                Fallback();
                return;
            }

            //Find same shortcut
            foreach (ShortcutData allShortCut in allShortCuts)
            {
                if (allShortCut == data) continue;
                if ((KeyCode)allShortCut.shortCutField.value != keyCode) continue;

                PrefabBrush.DisplayError($"Shortcut Conflict! This shortcut ({keyCode}) is already being used by {allShortCut.shortCutName}. Please select another one");
                Fallback();
                return;
            }

            EditorPrefs.SetInt(key, (int)keyCode);

            void Fallback()
            {
                KeyCode oldValue = (KeyCode)changeEvent.previousValue;
                keycodeField.SetValueWithoutNotify(oldValue);
            }
        }

        public static void Dispose()
        {
            if (selectedButton != null) selectedButton.text = TapKeyText;
            selectedData1 = null;
            selectedButton = null;
        }

        private static void OpenKeycodeDoc(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"Clicked on open keycode docs");
#endif
            Application.OpenURL("https://docs.unity3d.com/ScriptReference/KeyCode.html");
        }

        private static void ResetShortcuts(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"Clicked to reset shortcuts");
#endif

            bool r = EditorUtility.DisplayDialog("Prefab Brush - Reset Shortcuts", "Are you sure you want to reset all shortcuts? This action cannot be undone.", "Ok", "Cancel");

            if (!r) return;

            Dispose();

            foreach (ShortcutData data in allShortCuts)
            {
                data.ResetShortcut();
            }
        }

        public static void UpdateAssignKey()
        {
            if (selectedData1 == null) return;
            KeyCode key = Event.current.keyCode;
            if (key == KeyCode.None) return;

            selectedData1.shortCutField.value = key;
            Dispose();
        }
    }

    public static class PB_FolderUtils
    {
        public static string GetSelectedPathOrFallback()
        {
            string path = "";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                bool fileExists = File.Exists(path);
                bool isNull = string.IsNullOrEmpty(path);
#if HARPIA_DEBUG
                Debug.Log($"[{nameof(PB_FolderUtils)}] Found path {path}| fileExists {fileExists} | isNull {isNull} | Obj {obj.name}");
#endif
                if (isNull || !fileExists) continue;
                path = Path.GetDirectoryName(path);
                break;
            }

            return path;
        }

        public static void ShowFolder(string path)
        {
            EditorUtility.FocusProjectWindow();

            Object folder = AssetDatabase.LoadAssetAtPath(path, typeof(object));

            if (folder == null)
            {
                Debug.Log($"{PrefabBrush.DebugLogStart} Could not find folder {path}");
                return;
            }

            Type pt = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            object ins = pt.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            MethodInfo showDirMeth = pt.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

            if (obj == null)
            {
                Debug.LogError("[Prefab Brush] Could not find any folder tab");
                return;
            }

            showDirMeth.Invoke(ins, new object[]{ obj.GetInstanceID(), true });
        }

        public static bool HasAnyPrefab(string selectedFolder)
        {
            string[] assets = AssetDatabase.FindAssets("t:Prefab", new[]{ selectedFolder });
            return assets.Length > 0;
        }

        public static List<GameObject> GetPrefabs(string selectedFolder)
        {
            if (string.IsNullOrEmpty(selectedFolder)) return new List<GameObject>();

            string[] assets = AssetDatabase.FindAssets("t:Prefab", new[]{ selectedFolder });

            List<GameObject> objects = new List<GameObject>();

            foreach (string s in assets)
            {
                if (string.IsNullOrEmpty(s)) continue;
                string path = AssetDatabase.GUIDToAssetPath(s);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj == null) continue;
                objects.Add(obj);
            }

            return objects;
        }
    }

    public static class PB_SnappingManager
    {
        public enum SnapModeEnum
        {
            None,
            Grid_Snapping,
            Vertex_Snapping
        }

        public static FloatField gridSnapValueField;
        public static Vector2Field gridOffsetField;

        private static Vector3 _gridPos;
        private static DefaultEnumField _snapModeDropdown;
        public static LayerMaskField _vertexSnappingLayersField;
        public static MaskField _vertexSnappingTags;
        private static VisualElement _gridSnapSection;
        private static VisualElement _vertexSnappingSection;
        public static FloatField _vertexSnapMinDistance;
        private static Color GridColor => PrefabBrush.instance.advancedSettings.gridColor.value;

        public static void Init(VisualElement root, SerializedObject obj)
        {
            _snapModeDropdown = root.Q<DefaultEnumField>("snap-mode-dropdown");
            _snapModeDropdown.Init(SnapModeEnum.None);
            _snapModeDropdown.BindProperty(obj.FindProperty(PrefabBrushTemplate.SnapMode));
            _snapModeDropdown.RegisterValueChangedCallback(OnSnapModeChanged);

            _gridSnapSection = root.Q<VisualElement>("grid-snapping-section");

            gridSnapValueField = root.Find<FloatField>("grid-snap-value", PrefabBrushTemplate.GridSnapValue);
            gridSnapValueField.RegisterFocusEvents_PB();
            gridSnapValueField.RegisterValueChangedCallback(OnGridSnapFloatChanged);
            gridSnapValueField.SetMinValue(0.01f);

            gridOffsetField = root.Find<Vector2Field>("grid-offset", PrefabBrushTemplate.GridOffset);
            gridOffsetField.RegisterFocusEvents_PB();
            gridOffsetField.RegisterValueChangedCallback(OnGridOffsetChanged);

            //Vertex snapping stuff
            _vertexSnappingSection = root.Q<VisualElement>("vertex-snapping-section");

            _vertexSnappingLayersField = _vertexSnappingSection.Find<LayerMaskField>("vertex-snapping-layer-mask", PrefabBrushTemplate.VertexSnapLayerMask);
            _vertexSnappingLayersField.RegisterValueChangedCallback(OnVertexSnappingLayerMaskChanged);
            _vertexSnappingLayersField.value = -1;

            _vertexSnappingTags = _vertexSnappingSection.Find<MaskField>("vertex-snapping-tag-mask", PrefabBrushTemplate.VertexSnapTagMask);
            _vertexSnappingTags.RegisterValueChangedCallback(OnVertexSnappingTagMaskChanged);
            _vertexSnappingTags.choices = PrefabBrushTool.GetTagsChoices();
            _vertexSnappingTags.value = -1;

            _vertexSnapMinDistance = root.Find<FloatField>("vertex-snapping-snap-distance", PrefabBrushTemplate.VertexSnapDistance);
            _vertexSnapMinDistance.SetMinValue(0.0001f);
            _vertexSnapMinDistance.RegisterFocusEvents_PB();
        }

        private static void OnVertexSnappingTagMaskChanged(ChangeEvent<int> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_SnappingManager)}] Vertex snapping tag mask changed");
#endif
            PB_VertexFinder.Dispose();
            PB_MeshBatcher.Dispose();
        }

        private static void OnVertexSnappingLayerMaskChanged(ChangeEvent<int> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_SnappingManager)}] Vertex snapping layer mask changed");
#endif
            PB_VertexFinder.Dispose();
        }

        private static void OnSnapModeChanged(ChangeEvent<Enum> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_SnappingManager)}] Snap mode changed");
#endif
            UpdateUI();
        }

        private static void OnGridSnapFloatChanged(ChangeEvent<float> changeEvent)
        {
            float v = Mathf.Max(0.001f, gridSnapValueField.value);
            gridSnapValueField.SetValueWithoutNotify(v);
        }

        private static bool CanUseGrid()
        {
            PrefabBrush.PaintMode paintMode = PrefabBrush.instance.GetPaintMode();
            return paintMode != PrefabBrush.PaintMode.Eraser;
        }

        private static Vector3 GetGridOffset()
        {
            return new Vector3(gridOffsetField.value.x, 0, gridOffsetField.value.y);
        }

        public static void DrawGrid(RaycastHit hitInfo, bool forceDraw)
        {
            switch (forceDraw)
            {
                case false when !CanUseGrid():
                case false when !IsUsingGridSnap():
                    return;
            }

            float snapValue = gridSnapValueField.value;
            float radius = snapValue * 10;
            float arcRadius = snapValue / 10;
            Handles.color = GridColor;

            _gridPos = new Vector3(PrefabBrushTool.RoundTo(hitInfo.point.x, snapValue), hitInfo.point.y, PrefabBrushTool.RoundTo(hitInfo.point.z, snapValue)) + GetGridOffset();

            Vector3 start = new(_gridPos.x - radius, _gridPos.y, _gridPos.z - radius);
            Vector3 end = new(_gridPos.x + radius, _gridPos.y, _gridPos.z + radius);
            Vector3 center = new(_gridPos.x, _gridPos.y, _gridPos.z);

            Vector3 offset = Vector3.up * 0.01f;

            Handles.zTest = CompareFunction.LessEqual;
            for (float x = start.x; x <= end.x; x += snapValue)
            {
                for (float z = start.z; z <= end.z; z += snapValue)
                {
                    Vector3 pos = new(x, _gridPos.y, z);

                    Color c = Handles.color;
                    float distanceFromCenter = Vector3.Distance(pos, center) / radius;
                    c.a = (1 - distanceFromCenter) * 0.3f;

                    if (c.a < 0.1f) continue;

                    Handles.color = PrefabBrush.instance.advancedSettings.gridColor.value;
                    Handles.DrawSolidDisc(pos + offset, Vector3.up, arcRadius);
                }
            }
        }

        private static void OnGridOffsetChanged(ChangeEvent<Vector2> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_SnappingManager)}] Grid offset changed");
#endif
            float size = gridSnapValueField.value;
            Vector2 v = evt.newValue;
            v.x = Mathf.Clamp(v.x, 0, size);
            v.y = Mathf.Clamp(v.y, 0, size);
            gridOffsetField.SetValueWithoutNotify(v);
        }

        public static void UpdateUI()
        {
            //harpia log
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_SnappingManager)}] UpdateUI");
#endif

            bool canUseGrid = CanUseGrid();
            bool isUsingGrid = IsUsingGridSnap();
            bool isUsingVertex = IsUsingVertexSnap();

            //UI
            _gridSnapSection.SetActive(canUseGrid && isUsingGrid);
            _vertexSnappingSection.SetActive(isUsingVertex);

            //UX
            if (gridSnapValueField.value <= 0) gridSnapValueField.SetValueWithoutNotify(0.1f);
        }

        public static bool IsUsingVertexSnap() => GetSnapMode() == SnapModeEnum.Vertex_Snapping;

        public static void LoadTemplate(PrefabBrushTemplate lastLoadedTemplate)
        {
            _snapModeDropdown.value = lastLoadedTemplate.snapMode;
            gridOffsetField.value = lastLoadedTemplate.gridOffset;

            _vertexSnappingTags.value = lastLoadedTemplate.vertexSnapTagMask;
            _vertexSnappingLayersField.value = lastLoadedTemplate.vertexSnapLayerMask;
            _vertexSnapMinDistance.value = lastLoadedTemplate.vertexSnapDistance;
        }

        public static Vector3 TryToGetPositionOnGrid(Vector3 refPos)
        {
            if (!CanUseGrid()) return refPos;
            if (!IsUsingGridSnap()) return refPos;

            Vector3 offset = GetGridOffset();
            float snapValue = gridSnapValueField.value;

            Vector3 roundValue = new Vector3(PrefabBrushTool.RoundTo(refPos.x, snapValue), refPos.y, PrefabBrushTool.RoundTo(refPos.z, snapValue));

            Vector3[] possiblePositions = new Vector3[4];
            possiblePositions[0] = roundValue + new Vector3(offset.x, 0, offset.z);
            possiblePositions[1] = roundValue + new Vector3(-offset.x, 0, offset.z);
            possiblePositions[2] = roundValue + new Vector3(offset.x, 0, -offset.z);
            possiblePositions[3] = roundValue + new Vector3(-offset.x, 0, -offset.z);

            //Find the closes to ref pos
            float d = float.MaxValue;
            int index = -1;
            for (int i = 0; i < possiblePositions.Length; i++)
            {
                float cd = Vector3.SqrMagnitude(refPos - possiblePositions[i]);
                if (cd > d) continue;
                d = cd;
                index = i;
            }

            return possiblePositions[index];
        }

        public static bool IsUsingGridSnap() => GetSnapMode() == SnapModeEnum.Grid_Snapping;

        public static SnapModeEnum GetSnapMode() => (SnapModeEnum)_snapModeDropdown.value;

        public static bool IsAnyVertexClose(Vector3 hitPoint, Ray r, out Vector3 result)
        {
            bool find = PB_VertexFinder.GetClosestVertex(hitPoint, _vertexSnapMinDistance.value, r, PB_MeshBatcher.BatchForVertex(), Matrix4x4.identity, out result);
            return find;
        }

        public static List<string> GetAllowedTags()
        {
            return _vertexSnappingTags.GetSelectedChoices();
        }
    }

    public static class PB_FakePlaneManger
    {
        public static Toggle fakePlaneNoRaycastToggle;
        public static FloatField fakePlaneYPosField;
        private static Plane _planeYZero;
        private static float _lastPlaneProsY = float.NegativeInfinity;

        private static void OnFakePlaneToggle(ChangeEvent<bool> evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_FakePlaneManger)}] OnPlaceOnYZeroToggleChanged");
#endif
            fakePlaneYPosField.SetActive(fakePlaneNoRaycastToggle.value);
        }

        public static void Init(VisualElement root)
        {
            fakePlaneNoRaycastToggle = root.Find<Toggle>("no-hit-y-pos-toggle", PrefabBrushTemplate.UseFakePlaneIfNoHit);
            fakePlaneYPosField = root.Find<FloatField>("fake-plane-y-pos", PrefabBrushTemplate.FakePlaneYPosition);
            fakePlaneYPosField.SetValueWithoutNotify(0);
            fakePlaneYPosField.RegisterFocusEvents_PB();
            fakePlaneNoRaycastToggle.RegisterValueChangedCallback(OnFakePlaneToggle);
            OnFakePlaneToggle(null);
        }

        public static void UpdateUI()
        {
            fakePlaneYPosField.SetActive(fakePlaneNoRaycastToggle.value);
        }

        private static Plane GetFakePlane()
        {
            //if (Math.Abs(_lastPlaneProsY - fakePlaneYPosField.value) < .00001f) return _planeYZero;
            _lastPlaneProsY = fakePlaneYPosField.value;
            Vector3 planeCenter = new(0, _lastPlaneProsY, 0);
            _planeYZero = new Plane(Vector3.up, planeCenter);
            return _planeYZero;
        }

        public static bool Raycast(Ray ray, out float f, out RaycastHit hitInfo)
        {
            hitInfo = new RaycastHit();
            f = -1;
            if (!fakePlaneNoRaycastToggle.value) return false;

            Plane planeYZero = GetFakePlane();
            bool hit = planeYZero.Raycast(ray, out f);
            if (!hit) return false;

            hitInfo.point = ray.GetPoint(f);
            hitInfo.normal = planeYZero.normal;
            return true;
        }

        public static bool IsUsingFakePlane()
        {
            return fakePlaneNoRaycastToggle.value;
        }

        public static void DrawFakePlane()
        {
            Camera sceneCamera = PrefabBrush.instance.sceneCamera;
            PrefabBrush.RaycastMode raycastMode = PrefabBrush.instance.GetRaycastMode();
            LayerMaskField layerMaskField = PrefabBrush.instance.layerMaskField;

            RaycastHit hitInfo = PrefabBrushTool.GetCenterRay(sceneCamera, raycastMode, layerMaskField.value);

            bool hit = hitInfo.point.x != float.MaxValue;
            if (!hit) return;

            Vector3 center = hitInfo.point;
            const float size = 10;
            float yPos = fakePlaneYPosField.value;

            Vector3[] rectVertices = new[]{ new Vector3(center.x - size, yPos, center.z - size), new Vector3(center.x + size, yPos, center.z - size), new Vector3(center.x + size, yPos, center.z + size), new Vector3(center.x - size, yPos, center.z + size) };

            Color faceColor = new(1f, 1f, 1f, 0.3f);
            Handles.zTest = CompareFunction.Less;
            Handles.DrawSolidRectangleWithOutline(rectVertices, faceColor, Color.red);
        }

        public static Vector3 RaycastIntoY0(Ray ray)
        {
            //crete a fake plane at y 0
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(ray, out float f)) return Vector3.zero;
            return ray.GetPoint(f);
        }
    }

    public static class PB_ThumbnailGenerator
    {
        class ThumbnailRequestModel
        {
            public GameObject go;
            public PbPrefabUI ui;
        }

        public static Texture2D _defaultThumbPrefabUnity;

        private static Queue<ThumbnailRequestModel> loadQueue = new();
        private static Dictionary<GameObject, Texture2D> _thumbnails = new();
        private static ThumbnailRequestModel currentLoading;
        private static int _instanceID;
        private static Texture2D currentTex;
        private static bool _isRunningUpdate;

        private const int minPreviewSize = 256;

        public static void Init()
        {
            _thumbnails ??= new Dictionary<GameObject, Texture2D>();
            loadQueue ??= new Queue<ThumbnailRequestModel>();

            if (_defaultThumbPrefabUnity != null) return;
            _defaultThumbPrefabUnity = AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
        }

        public static void Dispose()
        {
            EditorApplication.update -= OnUpdate;
            _isRunningUpdate = false;
        }

        private static void OnUpdate()
        {
            //Nothing to load
            if (loadQueue.Count == 0 && currentLoading == null)
            {
                Dispose();
                return;
            }

            if (currentLoading != null)
            {
                //Wait for load
                if (AssetPreview.IsLoadingAssetPreview(_instanceID)) return;

                //Loaded with error
                if (currentTex == null)
                {
                    ClearUpdate();
                    return;
                }

                //Loaded successfully
                _thumbnails.Remove(currentLoading.go);
                _thumbnails.Add(currentLoading.go, currentTex);

                //Set the thumbnail
                currentLoading.ui.SetThumbnail(currentTex, true);

                ClearUpdate();
                return;
            }

            //Get The next item in the queue
            currentLoading = loadQueue.Dequeue();
            if (currentLoading == null || (_thumbnails.ContainsKey(currentLoading.go) && _thumbnails[currentLoading.go] != null))
            {
                ClearUpdate();
                return;
            }

            //Get the instance id of the current gameobject
            _thumbnails.Remove(currentLoading.go);
            _instanceID = currentLoading.go.GetInstanceID();

            //Start the loading of the asset preview
            currentTex = AssetPreview.GetAssetPreview(currentLoading.go);
            return;

            void ClearUpdate()
            {
                currentLoading = null;
                _instanceID = -1;
                currentTex = null;
            }
        }

        public static void GenerateThumbnail(PbPrefabUI ui)
        {
            GameObject go = ui.prefabToPaint;
            if (go == null) return;

            //Already Loaded
            if (_thumbnails.ContainsKey(go) && _thumbnails[go] != null)
            {
                ui.SetThumbnail(_thumbnails[go], true);
                return;
            }

            //Is Loading
            if (loadQueue.Any(x => x.go == go)) return;

            _thumbnails.Remove(go);
            loadQueue.Enqueue(new ThumbnailRequestModel{ go = go, ui = ui });

            int size = Mathf.Max(minPreviewSize, _thumbnails.Count + 1);
            AssetPreview.SetPreviewTextureCacheSize(size);

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_ThumbnailGenerator)}] Loading Thumbnail {go.name} | Queue size {_thumbnails.Count}");
#endif

            if (_isRunningUpdate) return;
            EditorApplication.update += OnUpdate;
            _isRunningUpdate = true;
        }
    }

    public static class PB_AttractorManager
    {
        private static PrefabBrushAttractor[] _attractors;
        private static PrefabBrushPivot currentPivot;
        private static GameObject lastObject;

        private const float minDistance = 0.2f;
        private static PrefabBrushAttractor closestAttractor;
        private static VisualElement _section;
        private static Label _nameLabel;
        private static Label _numberLabel;

        private static int currentIndex;
        private static PrefabBrushPivot[] _pivotsInChildren;
        private static Button _nextButton;
        private static Button _previousButton;

        public static void Init(VisualElement root)
        {
            _section = root.Q<VisualElement>("custom-pivot-selection");
            _nameLabel = _section.Q<Label>("custom-pivot-selection-name");
            _numberLabel = _section.Q<Label>("custom-pivot-selection-number");
            _previousButton = _section.Q<Button>("custom-pivot-selection-previous");
            _previousButton.RegisterCallback<ClickEvent>(OnPreviousPivot);
            _nextButton = _section.Q<Button>("custom-pivot-selection-next");
            _nextButton.RegisterCallback<ClickEvent>(OnNextPivot);

            UpdateUI();
        }

        public static void OnPreviousPivot(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] OnPreviousPivot ");
#endif

            if (_pivotsInChildren.Length == 0)
            {
                PB_HandlesExtension.WriteTextErrorTemp("No Pivots Found", PrefabBrush.instance.lastHitInfo);
                return;
            }

            currentIndex--;
            if (currentIndex < 0) currentIndex = _pivotsInChildren.Length - 1;
            UpdateUI();
            string t = $"{currentIndex + 1} / {_pivotsInChildren.Length}";
            PB_HandlesExtension.WriteTempText($"Selected Pivot {t}");
        }

        public static void OnNextPivot(ClickEvent evt)
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] OnNextPivot ");
#endif
            if (_pivotsInChildren.Length == 0)
            {
                PB_HandlesExtension.WriteTextErrorTemp("No Pivots Found", PrefabBrush.instance.lastHitInfo);
                return;
            }

            currentIndex++;
            if (currentIndex >= _pivotsInChildren.Length) currentIndex = 0;
            UpdateUI();
            string t = $"{currentIndex + 1} / {_pivotsInChildren.Length}";
            PB_HandlesExtension.WriteTempText($"Selected Pivot {t}");
        }

        public static void UpdateUI()
        {
            PrefabBrush instance = PrefabBrush.instance;
            if (instance == null)
            {
                _section?.SetActive(false);
                return;
            }

            bool show = instance.GetPivotMode() == PrefabBrush.PivotMode.Prefab_Brush_Pivot_Component && instance.GetPaintMode() == PrefabBrush.PaintMode.Precision;

            _section?.SetActive(show);
            if (!show) return;

            if (currentPivot == null)
            {
                _nameLabel.text = "No Pivot Found";
                _numberLabel.text = " - ";
                _previousButton.SetEnabled(false);
                _nextButton.SetEnabled(false);
                return;
            }

            bool enableButtons = _pivotsInChildren.Length > 1;
            _previousButton?.SetEnabled(enableButtons);
            _nextButton?.SetEnabled(enableButtons);

            _nameLabel.text = currentPivot.name;
            currentPivot = _pivotsInChildren[currentIndex];
            _numberLabel.text = $"{currentIndex + 1} / {_pivotsInChildren.Length}";
        }

        public static bool HasAnyAttractorClose(Vector3 hitPos, out PrefabBrushAttractor outFoundedAttractor)
        {
            outFoundedAttractor = null;
            if (currentPivot == null) return false;

            FindAttractors();
            float currentFoundDistance = Mathf.Infinity;
            closestAttractor = null;

            foreach (PrefabBrushAttractor currentAttractor in _attractors)
            {
                //Simple Checks
                if (currentAttractor == null) continue;

                //Distance check
                float sqrDistance = Vector3.SqrMagnitude(hitPos - currentAttractor.transform.position);
                if (sqrDistance > currentFoundDistance) continue;

                closestAttractor = currentAttractor;
                currentFoundDistance = sqrDistance;
            }

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] Searching for attractor in {_attractors.Length} - Found Closest {closestAttractor != null}");
#endif

            if (closestAttractor == null) return false;

            float radius = closestAttractor.GetRadius();
            radius *= radius;

            if (currentFoundDistance > radius) return false;

            outFoundedAttractor = closestAttractor;
            return true;
        }

        private static void FindAttractors()
        {
            if (_attractors != null) return;

            _attractors = Object.FindObjectsByType<PrefabBrushAttractor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            //Let's remove the attractors of the same parent
            PrefabBrushAttractor[] childAttractors = lastObject.GetComponentsInChildren<PrefabBrushAttractor>();

            //Let's keep only the valid attractors 
            List<PrefabBrushAttractor> removeList = new List<PrefabBrushAttractor>();

            foreach (PrefabBrushAttractor attractor in _attractors)
            {
                if (currentPivot.IsAttractedBy(attractor)) continue;
                removeList.Add(attractor);
            }

            _attractors = _attractors.Except(removeList).Except(childAttractors).ToArray();
        }

        public static void DrawHandles()
        {
            if (!PrefabBrush.instance.isRaycastHit) return;

            bool hasAttractor = closestAttractor != null;
            if (!hasAttractor) return;

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(PrefabBrush.instance.lastHitInfo.point, Vector3.up, minDistance);

            Handles.color = Color.green;
            Handles.DrawWireDisc(closestAttractor.transform.position, Vector3.up, closestAttractor.GetRadius());
        }

        public static void Dispose()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] disposing ");
#endif
            _attractors = null;
            currentPivot = null;
            lastObject = null;
            closestAttractor = null;
        }

        public static void SetCurrentObject(GameObject tempGo)
        {
            if (lastObject != null && lastObject == tempGo) return;
            lastObject = tempGo;

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] Searching for pivot in {tempGo.name}");
#endif

            currentPivot = null;

            _pivotsInChildren = tempGo.GetComponentsInChildren<PrefabBrushPivot>();
            if (_pivotsInChildren.Length == 0)
            {
                UpdateUI();
                return;
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, _pivotsInChildren.Length - 1);
            currentPivot = _pivotsInChildren[currentIndex];

#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}] Found pivot {currentPivot?.name}", currentPivot);
#endif

            string[] array = currentPivot.GetAttractedByArray();
            FindAttractors();
            UpdateUI();

            if (array.Length == 0) return;

            foreach (string attractedBy in array)
            {
                if (attractedBy == null) continue;
                bool any = _attractors.Any(attractor => attractor != null && attractedBy == attractor.name);
                if (any) return;
            }

            string list = string.Join(", ", array);
            Debug.LogWarning("Prefab Brush - The pivot is set to be attracted by names that are not in the scene. Please check your pivot.  " + list);
        }

        public static Vector3 CurrentPivotPosition()
        {
            if (currentPivot == null) return Vector3.zero;
            return currentPivot.transform.position;
        }

        public static Vector3 GetOCurrentOffset()
        {
            if (currentPivot == null) return Vector3.zero;
            Vector3 pos1 = lastObject.transform.position;
            Vector3 pos2 = currentPivot.transform.position;
            return pos1 - pos2;
        }

        [MenuItem("GameObject/Prefab Brush/Pivot & Attractors/Create Pivot")]
        public static void MenuItemCreatePivot()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}]  Create pivot");
#endif
            GameObject parent = Selection.activeGameObject;
            GameObject go = new GameObject("Prefab Brush Pivot");
            go.AddComponent<PrefabBrushPivot>();

            if (parent != null)
            {
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Prefab Brush Pivot");
        }

        [MenuItem("GameObject/Prefab Brush/Pivot & Attractors/Create Pivot", true)]
        public static bool MenuItemCreatePivotValidator()
        {
            return Selection.gameObjects.Length == 1 && Selection.activeGameObject != null;
        }

        //-------------------------
        [MenuItem("GameObject/Prefab Brush/Pivot & Attractors/Create Attractor")]
        public static void MenuItemCreateAttractor()
        {
#if HARPIA_DEBUG
            Debug.Log($"[{nameof(PB_AttractorManager)}]  Create pivot");
#endif
            GameObject parent = Selection.activeGameObject;
            GameObject go = new GameObject("Prefab Brush Attractor");
            go.AddComponent<PrefabBrushAttractor>();

            if (parent != null)
            {
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Prefab Brush Pivot");
        }

        [MenuItem("GameObject/Prefab Brush/Pivot & Attractors/Create Attractor", true)]
        public static bool MenuItemCreatePivotAttractor()
        {
            return Selection.gameObjects.Length == 1 && Selection.activeGameObject != null;
        }

        public static bool HasAttractor(GameObject prefabToPaint)
        {
            return prefabToPaint.GetComponentInChildren<PrefabBrushAttractor>() != null;
        }

        public static bool HasCustomPivot(GameObject prefabToPaint)
        {
            return prefabToPaint.GetComponentInChildren<PrefabBrushPivot>() != null;
        }

        public static Vector3 GetFirstPivotOffset(GameObject go)
        {
            PrefabBrushPivot firstPivot = go.GetComponentInChildren<PrefabBrushPivot>();
            if (firstPivot == null) return Vector3.zero;
            Vector3 pos1 = go.transform.position;
            Vector3 pos2 = firstPivot.transform.position;
            return (pos1 - pos2);
        }
    }
}