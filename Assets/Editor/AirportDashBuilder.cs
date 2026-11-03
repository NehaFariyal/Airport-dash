using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// AirportDashBuilder: Automated Editor tool to generate all Airport Dash assets,
/// materials, 3D prefabs, and 4 scenes (MainMenu, StageSelect, Information, GamePlay).
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class AirportDashBuilder : EditorWindow
{
    private const string SCENE_MAIN_MENU = "Assets/Scenes/MainMenu.unity";
    private const string SCENE_STAGE_SELECT = "Assets/Scenes/StageSelect.unity";
    private const string SCENE_INFORMATION = "Assets/Scenes/Information.unity";
    private const string SCENE_GAMEPLAY = "Assets/Scenes/GamePlay.unity";

    [MenuItem("Airport Dash/Build Complete Game (All Scenes, Prefabs & Settings)")]
    public static void BuildCompleteGame()
    {
        Debug.Log("==== [Airport Dash] Starting Automated Build Process ====");

        EnsureDirectories();
        CreateMaterials();
        CreatePrefabs();
        CreateMainMenuScene();
        CreateStageSelectScene();
        CreateInformationScene();
        CreateGamePlayScene();
        ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("==== [Airport Dash] Complete Game Successfully Built! ====");
        EditorUtility.DisplayDialog("Airport Dash", "Complete Airport Dash Game generated successfully!\n\nScenes:\n1. MainMenu\n2. StageSelect\n3. Information\n4. GamePlay\n\nStudent: Sk. Neha Fariyal (2303A51598)", "OK");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
        if (!Directory.Exists("Assets/Prefabs")) Directory.CreateDirectory("Assets/Prefabs");
        if (!Directory.Exists("Assets/Materials")) Directory.CreateDirectory("Assets/Materials");
        if (!Directory.Exists("Assets/Audio")) Directory.CreateDirectory("Assets/Audio");
    }

    #region Materials Creation
    [MenuItem("Airport Dash/Sub-Tools/1. Generate Materials")]
    public static void CreateMaterials()
    {
        EnsureDirectories();
        Shader standardShader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Legacy Shaders/Diffuse");

        // Helper to create & save a material
        CreateMaterial("Mat_AirportFloor", standardShader, new Color(0.92f, 0.94f, 0.97f), 0.7f, 0.1f);
        CreateMaterial("Mat_AirportLane", standardShader, new Color(0.12f, 0.53f, 0.90f), 0.5f, 0.0f);
        CreateMaterial("Mat_AirportWall", standardShader, new Color(0.82f, 0.86f, 0.90f), 0.3f, 0.2f);
        CreateMaterial("Mat_AirportGlass", standardShader, new Color(0.55f, 0.78f, 0.95f, 0.85f), 0.9f, 0.1f);
        CreateMaterial("Mat_AirportPillar", standardShader, new Color(0.35f, 0.40f, 0.48f), 0.4f, 0.5f);
        CreateMaterial("Mat_AirportSign", standardShader, new Color(0.08f, 0.25f, 0.55f), 0.6f, 0.0f);
        CreateMaterial("Mat_AirportTarmac", standardShader, new Color(0.25f, 0.27f, 0.30f), 0.2f, 0.0f);

        // Airplane Materials
        CreateMaterial("Mat_AirplaneBody", standardShader, new Color(0.98f, 0.98f, 0.98f), 0.8f, 0.2f);
        CreateMaterial("Mat_AirplaneBlue", standardShader, new Color(0.05f, 0.45f, 0.85f), 0.7f, 0.1f);

        // Obstacles: Luggage, Cart, Stanchion
        CreateMaterial("Mat_LuggageRed", standardShader, new Color(0.88f, 0.22f, 0.20f), 0.6f, 0.1f);
        CreateMaterial("Mat_LuggageBlue", standardShader, new Color(0.18f, 0.45f, 0.75f), 0.6f, 0.1f);
        CreateMaterial("Mat_LuggageOrange", standardShader, new Color(0.95f, 0.52f, 0.10f), 0.6f, 0.1f);
        CreateMaterial("Mat_LuggageBlack", standardShader, new Color(0.15f, 0.15f, 0.17f), 0.4f, 0.0f);
        CreateMaterial("Mat_CartMetal", standardShader, new Color(0.75f, 0.78f, 0.82f), 0.85f, 0.85f);
        CreateMaterial("Mat_StanchionPost", standardShader, new Color(0.85f, 0.87f, 0.90f), 0.9f, 0.9f);
        CreateMaterial("Mat_StanchionRope", standardShader, new Color(0.82f, 0.12f, 0.15f), 0.2f, 0.0f);

        // Collectibles: Coins & Gems
        CreateMaterial("Mat_GoldCoin", standardShader, new Color(1.0f, 0.80f, 0.10f), 0.9f, 0.8f);
        CreateMaterial("Mat_RubyGem", standardShader, new Color(0.92f, 0.15f, 0.25f), 0.95f, 0.3f);

        // Player Materials (matching Capstone illustration: green jacket, blue pants, red cap)
        CreateMaterial("Mat_PlayerShirt", standardShader, new Color(0.20f, 0.68f, 0.38f), 0.3f, 0.0f);
        CreateMaterial("Mat_PlayerPants", standardShader, new Color(0.18f, 0.32f, 0.65f), 0.3f, 0.0f);
        CreateMaterial("Mat_PlayerCap", standardShader, new Color(0.85f, 0.18f, 0.20f), 0.3f, 0.0f);
        CreateMaterial("Mat_PlayerSkin", standardShader, new Color(0.94f, 0.76f, 0.62f), 0.2f, 0.0f);
        CreateMaterial("Mat_PlayerShoes", standardShader, new Color(0.95f, 0.95f, 0.95f), 0.4f, 0.0f);

        AssetDatabase.SaveAssets();
        Debug.Log("[Airport Dash] Materials created.");
    }

    private static Material CreateMaterial(string name, Shader shader, Color color, float smoothness, float metallic)
    {
        string path = $"Assets/Materials/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        return mat;
    }

    private static Material GetMat(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/{name}.mat");
    }

    private static AudioClip GetAudio(string name)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Audio/{name}.wav");
    }
    #endregion

    #region Prefabs Creation
    [MenuItem("Airport Dash/Sub-Tools/2. Generate Prefabs")]
    public static void CreatePrefabs()
    {
        EnsureDirectories();

        CreatePlayerPrefab();
        CreateLuggageObstaclePrefab();
        CreateCartObstaclePrefab();
        CreateStanchionObstaclePrefab();
        CreateAirplaneObstaclePrefab();
        CreateFlyingAirplanePrefab();
        CreateCoinPrefab();
        CreateGemPrefab();
        CreateConcourseSegmentPrefab();

        AssetDatabase.SaveAssets();
        Debug.Log("[Airport Dash] All Prefabs created successfully.");
    }

    private static void CreatePlayerPrefab()
    {
        GameObject root = new GameObject("Player");
        root.tag = "Player";

        CharacterController cc = root.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.45f;
        cc.center = new Vector3(0, 0.9f, 0);

        PlayerMovement movement = root.AddComponent<PlayerMovement>();
        movement.laneDistance = 3.0f;
        movement.laneChangeSpeed = 22f;
        movement.currentLane = 1;
        CollisionDetect collision = root.AddComponent<CollisionDetect>();
        AudioSource audioSource = root.AddComponent<AudioSource>();

        movement.jumpSound = GetAudio("jump");
        collision.collisionSound = GetAudio("collision");

        // Visual Model Container
        GameObject visual = new GameObject("VisualModel");
        visual.transform.SetParent(root.transform, false);
        movement.visualModel = visual.transform;

        // 1. Torso (Green Hoodie)
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.name = "Torso";
        torso.transform.SetParent(visual.transform, false);
        torso.transform.localPosition = new Vector3(0, 0.95f, 0);
        torso.transform.localScale = new Vector3(0.65f, 0.7f, 0.4f);
        torso.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerShirt");
        Object.DestroyImmediate(torso.GetComponent<Collider>());

        // 2. Head (Skin Tone)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(visual.transform, false);
        head.transform.localPosition = new Vector3(0, 1.5f, 0);
        head.transform.localScale = new Vector3(0.42f, 0.45f, 0.42f);
        head.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerSkin");
        Object.DestroyImmediate(head.GetComponent<Collider>());

        // 3. Red Cap (Baseball cap)
        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cap.name = "Cap";
        cap.transform.SetParent(visual.transform, false);
        cap.transform.localPosition = new Vector3(0, 1.7f, 0);
        cap.transform.localScale = new Vector3(0.46f, 0.12f, 0.46f);
        cap.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerCap");
        Object.DestroyImmediate(cap.GetComponent<Collider>());

        GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visor.name = "CapVisor";
        visor.transform.SetParent(visual.transform, false);
        visor.transform.localPosition = new Vector3(0, 1.66f, 0.24f);
        visor.transform.localScale = new Vector3(0.35f, 0.04f, 0.22f);
        visor.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerCap");
        Object.DestroyImmediate(visor.GetComponent<Collider>());

        // 4. Arms
        GameObject leftArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftArm.name = "LeftArm";
        leftArm.transform.SetParent(visual.transform, false);
        leftArm.transform.localPosition = new Vector3(-0.45f, 0.95f, 0);
        leftArm.transform.localScale = new Vector3(0.2f, 0.6f, 0.22f);
        leftArm.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerShirt");
        Object.DestroyImmediate(leftArm.GetComponent<Collider>());

        GameObject rightArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightArm.name = "RightArm";
        rightArm.transform.SetParent(visual.transform, false);
        rightArm.transform.localPosition = new Vector3(0.45f, 0.95f, 0);
        rightArm.transform.localScale = new Vector3(0.2f, 0.6f, 0.22f);
        rightArm.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerShirt");
        Object.DestroyImmediate(rightArm.GetComponent<Collider>());

        // 5. Legs & Shoes (Blue pants & white sneakers)
        GameObject leftLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftLeg.name = "LeftLeg";
        leftLeg.transform.SetParent(visual.transform, false);
        leftLeg.transform.localPosition = new Vector3(-0.18f, 0.35f, 0);
        leftLeg.transform.localScale = new Vector3(0.24f, 0.65f, 0.26f);
        leftLeg.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerPants");
        Object.DestroyImmediate(leftLeg.GetComponent<Collider>());

        GameObject rightLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightLeg.name = "RightLeg";
        rightLeg.transform.SetParent(visual.transform, false);
        rightLeg.transform.localPosition = new Vector3(0.18f, 0.35f, 0);
        rightLeg.transform.localScale = new Vector3(0.24f, 0.65f, 0.26f);
        rightLeg.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerPants");
        Object.DestroyImmediate(rightLeg.GetComponent<Collider>());

        // Shoes
        GameObject leftShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftShoe.name = "LeftShoe";
        leftShoe.transform.SetParent(visual.transform, false);
        leftShoe.transform.localPosition = new Vector3(-0.18f, 0.05f, 0.06f);
        leftShoe.transform.localScale = new Vector3(0.26f, 0.12f, 0.38f);
        leftShoe.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerShoes");
        Object.DestroyImmediate(leftShoe.GetComponent<Collider>());

        GameObject rightShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightShoe.name = "RightShoe";
        rightShoe.transform.SetParent(visual.transform, false);
        rightShoe.transform.localPosition = new Vector3(0.18f, 0.05f, 0.06f);
        rightShoe.transform.localScale = new Vector3(0.26f, 0.12f, 0.38f);
        rightShoe.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_PlayerShoes");
        Object.DestroyImmediate(rightShoe.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Player.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateLuggageObstaclePrefab()
    {
        // Stack of suitcases: Red main suitcase + Blue secondary suitcase + Orange briefcase
        GameObject root = new GameObject("Obstacle_Luggage");
        root.tag = "Obstacle";

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.75f, 0);
        col.size = new Vector3(2.4f, 1.5f, 1.2f);
        col.isTrigger = true;

        // Big Red Suitcase
        GameObject redBag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        redBag.name = "RedSuitcase";
        redBag.transform.SetParent(root.transform, false);
        redBag.transform.localPosition = new Vector3(-0.45f, 0.65f, 0);
        redBag.transform.localScale = new Vector3(1.2f, 1.3f, 0.65f);
        redBag.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageRed");
        Object.DestroyImmediate(redBag.GetComponent<Collider>());

        // Red Suitcase Handle
        GameObject redHandle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        redHandle.name = "RedHandle";
        redHandle.transform.SetParent(redBag.transform, false);
        redHandle.transform.localPosition = new Vector3(0, 0.58f, 0);
        redHandle.transform.localScale = new Vector3(0.35f, 0.15f, 0.15f);
        redHandle.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageBlack");
        Object.DestroyImmediate(redHandle.GetComponent<Collider>());

        // Medium Blue Suitcase
        GameObject blueBag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blueBag.name = "BlueSuitcase";
        blueBag.transform.SetParent(root.transform, false);
        blueBag.transform.localPosition = new Vector3(0.55f, 0.55f, 0.1f);
        blueBag.transform.localScale = new Vector3(1.0f, 1.1f, 0.55f);
        blueBag.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageBlue");
        Object.DestroyImmediate(blueBag.GetComponent<Collider>());

        // Small Orange Suitcase
        GameObject orangeBag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        orangeBag.name = "OrangeBag";
        orangeBag.transform.SetParent(root.transform, false);
        orangeBag.transform.localPosition = new Vector3(0.3f, 0.35f, -0.4f);
        orangeBag.transform.localRotation = Quaternion.Euler(0, 15f, 0);
        orangeBag.transform.localScale = new Vector3(0.9f, 0.7f, 0.45f);
        orangeBag.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageOrange");
        Object.DestroyImmediate(orangeBag.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Obstacle_Luggage.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateCartObstaclePrefab()
    {
        // Airport Baggage Push Cart with chrome tubular frame and bags loaded
        GameObject root = new GameObject("Obstacle_Cart");
        root.tag = "Obstacle";

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.85f, 0);
        col.size = new Vector3(2.2f, 1.7f, 1.6f);
        col.isTrigger = true;

        // Base Bed Platform
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "CartPlatform";
        platform.transform.SetParent(root.transform, false);
        platform.transform.localPosition = new Vector3(0, 0.35f, 0);
        platform.transform.localScale = new Vector3(1.8f, 0.12f, 1.4f);
        platform.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_CartMetal");
        Object.DestroyImmediate(platform.GetComponent<Collider>());

        // 4 Wheels
        float[] xOffsets = { -0.75f, 0.75f };
        float[] zOffsets = { -0.55f, 0.55f };
        foreach (float x in xOffsets)
        {
            foreach (float z in zOffsets)
            {
                GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                wheel.name = "Wheel";
                wheel.transform.SetParent(root.transform, false);
                wheel.transform.localPosition = new Vector3(x, 0.18f, z);
                wheel.transform.localRotation = Quaternion.Euler(0, 0, 90f);
                wheel.transform.localScale = new Vector3(0.32f, 0.08f, 0.32f);
                wheel.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageBlack");
                Object.DestroyImmediate(wheel.GetComponent<Collider>());
            }
        }

        // Cart Tubular Side Rails
        GameObject leftRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftRail.name = "LeftRail";
        leftRail.transform.SetParent(root.transform, false);
        leftRail.transform.localPosition = new Vector3(-0.85f, 0.75f, 0);
        leftRail.transform.localScale = new Vector3(0.08f, 0.75f, 1.4f);
        leftRail.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_CartMetal");
        Object.DestroyImmediate(leftRail.GetComponent<Collider>());

        GameObject rightRail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightRail.name = "RightRail";
        rightRail.transform.SetParent(root.transform, false);
        rightRail.transform.localPosition = new Vector3(0.85f, 0.75f, 0);
        rightRail.transform.localScale = new Vector3(0.08f, 0.75f, 1.4f);
        rightRail.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_CartMetal");
        Object.DestroyImmediate(rightRail.GetComponent<Collider>());

        // Push Handle (Back)
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Handle";
        handle.transform.SetParent(root.transform, false);
        handle.transform.localPosition = new Vector3(0, 1.25f, -0.7f);
        handle.transform.localScale = new Vector3(1.8f, 0.12f, 0.12f);
        handle.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_CartMetal");
        Object.DestroyImmediate(handle.GetComponent<Collider>());

        // Bags on Cart
        GameObject cargoBag1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cargoBag1.name = "CargoBlue";
        cargoBag1.transform.SetParent(root.transform, false);
        cargoBag1.transform.localPosition = new Vector3(-0.3f, 0.75f, -0.1f);
        cargoBag1.transform.localScale = new Vector3(0.9f, 0.65f, 0.8f);
        cargoBag1.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageBlue");
        Object.DestroyImmediate(cargoBag1.GetComponent<Collider>());

        GameObject cargoBag2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cargoBag2.name = "CargoOrange";
        cargoBag2.transform.SetParent(root.transform, false);
        cargoBag2.transform.localPosition = new Vector3(0.45f, 0.7f, 0.15f);
        cargoBag2.transform.localScale = new Vector3(0.7f, 0.6f, 0.8f);
        cargoBag2.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageOrange");
        Object.DestroyImmediate(cargoBag2.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Obstacle_Cart.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateStanchionObstaclePrefab()
    {
        // Airport Gate Stanchion / Velvet Retractable Ribbon Barrier
        // Low height so player can jump over it!
        GameObject root = new GameObject("Obstacle_Stanchion");
        root.tag = "Obstacle";

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.5f, 0);
        col.size = new Vector3(3.4f, 1.0f, 0.5f);
        col.isTrigger = true;

        // Left Chrome Post
        GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftPost.name = "LeftPost";
        leftPost.transform.SetParent(root.transform, false);
        leftPost.transform.localPosition = new Vector3(-1.5f, 0.5f, 0);
        leftPost.transform.localScale = new Vector3(0.12f, 0.5f, 0.12f);
        leftPost.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_StanchionPost");
        Object.DestroyImmediate(leftPost.GetComponent<Collider>());

        GameObject leftBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftBase.name = "LeftBase";
        leftBase.transform.SetParent(root.transform, false);
        leftBase.transform.localPosition = new Vector3(-1.5f, 0.04f, 0);
        leftBase.transform.localScale = new Vector3(0.45f, 0.04f, 0.45f);
        leftBase.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_StanchionPost");
        Object.DestroyImmediate(leftBase.GetComponent<Collider>());

        // Right Chrome Post
        GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightPost.name = "RightPost";
        rightPost.transform.SetParent(root.transform, false);
        rightPost.transform.localPosition = new Vector3(1.5f, 0.5f, 0);
        rightPost.transform.localScale = new Vector3(0.12f, 0.5f, 0.12f);
        rightPost.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_StanchionPost");
        Object.DestroyImmediate(rightPost.GetComponent<Collider>());

        GameObject rightBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightBase.name = "RightBase";
        rightBase.transform.SetParent(root.transform, false);
        rightBase.transform.localPosition = new Vector3(1.5f, 0.04f, 0);
        rightBase.transform.localScale = new Vector3(0.45f, 0.04f, 0.45f);
        rightBase.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_StanchionPost");
        Object.DestroyImmediate(rightBase.GetComponent<Collider>());

        // Red Velvet Ribbon
        GameObject rope = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rope.name = "VelvetRibbon";
        rope.transform.SetParent(root.transform, false);
        rope.transform.localPosition = new Vector3(0, 0.72f, 0);
        rope.transform.localScale = new Vector3(3.0f, 0.08f, 0.04f);
        rope.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_StanchionRope");
        Object.DestroyImmediate(rope.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Obstacle_Stanchion.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateAirplaneObstaclePrefab()
    {
        // 3D Airplane Obstacle sitting across the apron/runway path
        GameObject root = new GameObject("Obstacle_Airplane");
        root.tag = "Obstacle";

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 1.1f, 0);
        col.size = new Vector3(4.2f, 2.2f, 4.2f);
        col.isTrigger = true;

        // Fuselage
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Fuselage";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = new Vector3(0, 1.1f, 0);
        body.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        body.transform.localScale = new Vector3(1.1f, 2.2f, 1.1f);
        body.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBody");
        Object.DestroyImmediate(body.GetComponent<Collider>());

        // Cockpit Glass
        GameObject cockpit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cockpit.name = "Cockpit";
        cockpit.transform.SetParent(root.transform, false);
        cockpit.transform.localPosition = new Vector3(0, 1.4f, 1.5f);
        cockpit.transform.localScale = new Vector3(0.6f, 0.35f, 0.6f);
        cockpit.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportGlass");
        Object.DestroyImmediate(cockpit.GetComponent<Collider>());

        // Swept Wings
        GameObject wings = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wings.name = "Wings";
        wings.transform.SetParent(root.transform, false);
        wings.transform.localPosition = new Vector3(0, 1.0f, 0.2f);
        wings.transform.localScale = new Vector3(4.2f, 0.12f, 1.2f);
        wings.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBody");
        Object.DestroyImmediate(wings.GetComponent<Collider>());

        // Winglets
        GameObject leftTip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftTip.name = "LeftWinglet";
        leftTip.transform.SetParent(root.transform, false);
        leftTip.transform.localPosition = new Vector3(-2.05f, 1.25f, 0.2f);
        leftTip.transform.localScale = new Vector3(0.12f, 0.45f, 0.7f);
        leftTip.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(leftTip.GetComponent<Collider>());

        GameObject rightTip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightTip.name = "RightWinglet";
        rightTip.transform.SetParent(root.transform, false);
        rightTip.transform.localPosition = new Vector3(2.05f, 1.25f, 0.2f);
        rightTip.transform.localScale = new Vector3(0.12f, 0.45f, 0.7f);
        rightTip.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(rightTip.GetComponent<Collider>());

        // Twin Jet Turbofan Engines
        float[] engineX = { -1.0f, 1.0f };
        foreach (float ex in engineX)
        {
            GameObject engine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            engine.name = "JetEngine";
            engine.transform.SetParent(root.transform, false);
            engine.transform.localPosition = new Vector3(ex, 0.75f, 0.1f);
            engine.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            engine.transform.localScale = new Vector3(0.42f, 0.65f, 0.42f);
            engine.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_CartMetal");
            Object.DestroyImmediate(engine.GetComponent<Collider>());
        }

        // Tail Fin (Vertical Stabilizer)
        GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tail.name = "TailFin";
        tail.transform.SetParent(root.transform, false);
        tail.transform.localPosition = new Vector3(0, 1.85f, -1.8f);
        tail.transform.localRotation = Quaternion.Euler(20f, 0, 0);
        tail.transform.localScale = new Vector3(0.14f, 1.2f, 0.9f);
        tail.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(tail.GetComponent<Collider>());

        // Landing Gear Wheels
        Vector3[] wheelPositions = { new Vector3(0, 0.18f, 1.3f), new Vector3(-0.75f, 0.18f, -0.4f), new Vector3(0.75f, 0.18f, -0.4f) };
        foreach (var wp in wheelPositions)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "GearWheel";
            wheel.transform.SetParent(root.transform, false);
            wheel.transform.localPosition = wp;
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            wheel.transform.localScale = new Vector3(0.35f, 0.12f, 0.35f);
            wheel.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageBlack");
            Object.DestroyImmediate(wheel.GetComponent<Collider>());
        }

        // Airport Warning Cones around the airplane
        Vector3[] coneOffsets = { new Vector3(-1.6f, 0.25f, 1.6f), new Vector3(1.6f, 0.25f, 1.6f), new Vector3(-1.9f, 0.25f, -1.4f), new Vector3(1.9f, 0.25f, -1.4f) };
        foreach (var cp in coneOffsets)
        {
            GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone.name = "RunwayCone";
            cone.transform.SetParent(root.transform, false);
            cone.transform.localPosition = cp;
            cone.transform.localScale = new Vector3(0.3f, 0.25f, 0.3f);
            cone.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_LuggageOrange");
            Object.DestroyImmediate(cone.GetComponent<Collider>());
        }

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Obstacle_Airplane.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateFlyingAirplanePrefab()
    {
        // Low/mid-altitude aeroplane flying across the airport sky
        GameObject plane = CreateAirplaneModel();
        plane.name = "Flying_Airplane";

        AirplaneFlyover flyover = plane.AddComponent<AirplaneFlyover>();
        flyover.flightSpeed = 40f;
        flyover.climbSpeed = 1.2f;
        flyover.flightDirection = new Vector3(0.2f, 0.05f, 1.0f);

        PrefabUtility.SaveAsPrefabAsset(plane, "Assets/Prefabs/Flying_Airplane.prefab");
        Object.DestroyImmediate(plane);
    }

    private static void CreateCoinPrefab()
    {
        GameObject root = new GameObject("Coin");
        root.tag = "Coin";

        SphereCollider col = root.AddComponent<SphereCollider>();
        col.radius = 0.55f;
        col.isTrigger = true;

        CollectCoin collectCoin = root.AddComponent<CollectCoin>();
        collectCoin.coinValue = 1;
        collectCoin.pickupSound = GetAudio("coin_pickup");

        CollectableRotate rotator = root.AddComponent<CollectableRotate>();
        rotator.rotateSpeed = 160f;
        rotator.bobbingHeight = 0.15f;

        // Visual 3D embossed coin mesh (cylinder oriented facing front)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visual.name = "CoinMesh";
        visual.transform.SetParent(root.transform, false);
        visual.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        visual.transform.localScale = new Vector3(0.85f, 0.1f, 0.85f);
        visual.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_GoldCoin");
        Object.DestroyImmediate(visual.GetComponent<Collider>());

        // Inner ring
        GameObject inner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        inner.name = "CoinRim";
        inner.transform.SetParent(root.transform, false);
        inner.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        inner.transform.localScale = new Vector3(0.65f, 0.12f, 0.65f);
        inner.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_GoldCoin");
        Object.DestroyImmediate(inner.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Coin.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateGemPrefab()
    {
        GameObject root = new GameObject("Gem");
        root.tag = "Gem";

        SphereCollider col = root.AddComponent<SphereCollider>();
        col.radius = 0.65f;
        col.isTrigger = true;

        GemCollect collectGem = root.AddComponent<GemCollect>();
        collectGem.gemValue = 1;
        collectGem.pickupSound = GetAudio("gem_pickup");

        CollectableRotate rotator = root.AddComponent<CollectableRotate>();
        rotator.rotateSpeed = 120f;
        rotator.bobbingHeight = 0.2f;

        // Faceted diamond ruby shape
        GameObject gemTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gemTop.name = "GemTop";
        gemTop.transform.SetParent(root.transform, false);
        gemTop.transform.localRotation = Quaternion.Euler(45f, 45f, 0);
        gemTop.transform.localScale = new Vector3(0.65f, 0.45f, 0.65f);
        gemTop.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_RubyGem");
        Object.DestroyImmediate(gemTop.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Gem.prefab");
        Object.DestroyImmediate(root);
    }

    private static void CreateConcourseSegmentPrefab()
    {
        // 50-meter modular airport terminal concourse track
        GameObject root = new GameObject("Airport_Segment");
        float segmentLength = 50f;

        // 1. Polished Concourse Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "ConcourseFloor";
        floor.transform.SetParent(root.transform, false);
        floor.transform.localPosition = new Vector3(0, -0.25f, segmentLength / 2f);
        floor.transform.localScale = new Vector3(14f, 0.5f, segmentLength);
        floor.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportFloor");

        // 2. Blue Guidance Lane Markings (Dashed stripes for 3 lanes)
        float[] laneDividers = { -2.0f, 2.0f };
        for (float z = 2.5f; z < segmentLength; z += 5f)
        {
            foreach (float lx in laneDividers)
            {
                GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.name = "LaneStripe";
                stripe.transform.SetParent(root.transform, false);
                stripe.transform.localPosition = new Vector3(lx, 0.01f, z);
                stripe.transform.localScale = new Vector3(0.18f, 0.02f, 2.5f);
                stripe.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportLane");
                Object.DestroyImmediate(stripe.GetComponent<Collider>());
            }
        }

        // 3. Side Boundary Curbs
        GameObject leftCurb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftCurb.name = "LeftCurb";
        leftCurb.transform.SetParent(root.transform, false);
        leftCurb.transform.localPosition = new Vector3(-6.5f, 0.15f, segmentLength / 2f);
        leftCurb.transform.localScale = new Vector3(0.6f, 0.3f, segmentLength);
        leftCurb.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportPillar");

        GameObject rightCurb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightCurb.name = "RightCurb";
        rightCurb.transform.SetParent(root.transform, false);
        rightCurb.transform.localPosition = new Vector3(6.5f, 0.15f, segmentLength / 2f);
        rightCurb.transform.localScale = new Vector3(0.6f, 0.3f, segmentLength);
        rightCurb.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportPillar");

        // 4. Large Terminal Panoramic Windows looking out to runway/tarmac
        GameObject leftGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftGlass.name = "TerminalWindowsLeft";
        leftGlass.transform.SetParent(root.transform, false);
        leftGlass.transform.localPosition = new Vector3(-7f, 3.2f, segmentLength / 2f);
        leftGlass.transform.localScale = new Vector3(0.2f, 6.0f, segmentLength);
        leftGlass.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportGlass");

        GameObject rightGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightGlass.name = "TerminalWindowsRight";
        rightGlass.transform.SetParent(root.transform, false);
        rightGlass.transform.localPosition = new Vector3(7f, 3.2f, segmentLength / 2f);
        rightGlass.transform.localScale = new Vector3(0.2f, 6.0f, segmentLength);
        rightGlass.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportGlass");

        // 5. Outside Runway/Tarmac Floor
        GameObject tarmacLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tarmacLeft.name = "TarmacLeft";
        tarmacLeft.transform.SetParent(root.transform, false);
        tarmacLeft.transform.localPosition = new Vector3(-25f, -0.3f, segmentLength / 2f);
        tarmacLeft.transform.localScale = new Vector3(36f, 0.4f, segmentLength);
        tarmacLeft.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportTarmac");
        Object.DestroyImmediate(tarmacLeft.GetComponent<Collider>());

        // 6. 3D Airplane on the Tarmac (Slide 3 artwork: white airliner with blue tail)
        GameObject airplane = CreateAirplaneModel();
        airplane.name = "TarmacAirplane";
        airplane.transform.SetParent(root.transform, false);
        airplane.transform.localPosition = new Vector3(-20f, 1.2f, 25f);
        airplane.transform.localRotation = Quaternion.Euler(0, 35f, 0);
        airplane.transform.localScale = Vector3.one * 1.6f;

        // 7. Structural Concourse Pillars
        for (float pz = 10f; pz <= segmentLength; pz += 25f)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "TerminalPillar";
            pillar.transform.SetParent(root.transform, false);
            pillar.transform.localPosition = new Vector3(-6.2f, 3.0f, pz);
            pillar.transform.localScale = new Vector3(0.8f, 3.0f, 0.8f);
            pillar.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportPillar");
        }

        // 8. Overhead Flight Boarding Sign
        GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sign.name = "FlightGateSign";
        sign.transform.SetParent(root.transform, false);
        sign.transform.localPosition = new Vector3(0, 5.0f, 25f);
        sign.transform.localScale = new Vector3(8.0f, 1.2f, 0.25f);
        sign.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportSign");
        Object.DestroyImmediate(sign.GetComponent<Collider>());

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Airport_Segment.prefab");
        Object.DestroyImmediate(root);
    }

    private static GameObject CreateAirplaneModel()
    {
        GameObject plane = new GameObject("AirplaneModel");

        // Fuselage
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Fuselage";
        body.transform.SetParent(plane.transform, false);
        body.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        body.transform.localScale = new Vector3(1.6f, 6.0f, 1.6f);
        body.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBody");
        Object.DestroyImmediate(body.GetComponent<Collider>());

        // Cockpit windows
        GameObject cockpit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cockpit.name = "Cockpit";
        cockpit.transform.SetParent(plane.transform, false);
        cockpit.transform.localPosition = new Vector3(0, 0.45f, 4.2f);
        cockpit.transform.localScale = new Vector3(0.8f, 0.35f, 0.8f);
        cockpit.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportGlass");
        Object.DestroyImmediate(cockpit.GetComponent<Collider>());

        // Wings
        GameObject wings = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wings.name = "Wings";
        wings.transform.SetParent(plane.transform, false);
        wings.transform.localPosition = new Vector3(0, 0.1f, 0.5f);
        wings.transform.localScale = new Vector3(12.0f, 0.15f, 2.2f);
        wings.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBody");
        Object.DestroyImmediate(wings.GetComponent<Collider>());

        // Blue Wingtips
        GameObject leftTip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftTip.transform.SetParent(plane.transform, false);
        leftTip.transform.localPosition = new Vector3(-5.9f, 0.4f, 0.5f);
        leftTip.transform.localScale = new Vector3(0.2f, 0.7f, 1.0f);
        leftTip.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(leftTip.GetComponent<Collider>());

        GameObject rightTip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightTip.transform.SetParent(plane.transform, false);
        rightTip.transform.localPosition = new Vector3(5.9f, 0.4f, 0.5f);
        rightTip.transform.localScale = new Vector3(0.2f, 0.7f, 1.0f);
        rightTip.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(rightTip.GetComponent<Collider>());

        // Tail Fin (Vertical Stabilizer)
        GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tail.name = "TailFin";
        tail.transform.SetParent(plane.transform, false);
        tail.transform.localPosition = new Vector3(0, 1.6f, -4.5f);
        tail.transform.localRotation = Quaternion.Euler(20f, 0, 0);
        tail.transform.localScale = new Vector3(0.2f, 2.4f, 1.8f);
        tail.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirplaneBlue");
        Object.DestroyImmediate(tail.GetComponent<Collider>());

        return plane;
    }
    #endregion

    #region Scene 1: MainMenu.unity
    [MenuItem("Airport Dash/Sub-Tools/3. Generate MainMenu Scene")]
    public static void CreateMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera & Lighting
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.backgroundColor = new Color(0.15f, 0.25f, 0.40f);
        camObj.transform.position = new Vector3(0, 2.5f, -6f);
        camObj.transform.rotation = Quaternion.Euler(15f, 0, 0);
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;
        light.intensity = 1.2f;
        lightObj.transform.rotation = Quaternion.Euler(40f, -30f, 0);

        // 2. 3D Background Diorama: Miniature Airport Runner Preview
        GameObject diorama = new GameObject("MenuDiorama");
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.SetParent(diorama.transform, false);
        floor.transform.localPosition = new Vector3(0, -0.2f, 5f);
        floor.transform.localScale = new Vector3(15f, 0.4f, 18f);
        floor.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportFloor");

        GameObject plane = CreateAirplaneModel();
        plane.transform.SetParent(diorama.transform, false);
        plane.transform.localPosition = new Vector3(3.5f, 0.8f, 8f);
        plane.transform.localRotation = Quaternion.Euler(-5f, -145f, 0);
        plane.transform.localScale = Vector3.one * 0.9f;

        // Runner standing in menu
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        if (playerPrefab != null)
        {
            GameObject runner = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            runner.transform.SetParent(diorama.transform, false);
            runner.transform.localPosition = new Vector3(0, 0, 1.5f);
            runner.transform.localRotation = Quaternion.Euler(0, 180f, 0); // facing menu camera
            Object.DestroyImmediate(runner.GetComponent<PlayerMovement>());
            Object.DestroyImmediate(runner.GetComponent<CharacterController>());
            Object.DestroyImmediate(runner.GetComponent<CollisionDetect>());
        }

        // 3. UI Canvas (Screen Space Overlay)
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        MainMenuControl menuControl = canvasObj.AddComponent<MainMenuControl>();
        menuControl.clickSound = GetAudio("button_click");

        // Title Header
        CreateText(canvasObj, "Title", "AIRPORT DASH", new Vector2(0, 220), new Vector2(800, 90), 52, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        CreateText(canvasObj, "Subtitle", "URBAN ESCAPE: ENDLESS RUN", new Vector2(0, 160), new Vector2(800, 50), 26, FontStyle.Bold, new Color(0.2f, 0.75f, 1f), TextAnchor.MiddleCenter);
        CreateText(canvasObj, "Author", "Sk. Neha Fariyal · 2303A51598\nSR University - School of CS&AI", new Vector2(0, 105), new Vector2(800, 60), 18, FontStyle.Normal, new Color(0.9f, 0.9f, 0.9f), TextAnchor.MiddleCenter);

        // Stats Box (Top Left / Center Left) matching Capstone Slide 5 & 7
        GameObject statsPanel = CreateUIPanel(canvasObj, "StatsPanel", new Vector2(-280, 0), new Vector2(240, 220), new Color(0.08f, 0.12f, 0.18f, 0.85f));
        CreateText(statsPanel, "StatsHeader", "CAREER STATS", new Vector2(0, 75), new Vector2(220, 40), 18, FontStyle.Bold, Color.yellow, TextAnchor.MiddleCenter);

        CreateText(statsPanel, "GemLabel", "◆ GEMS:", new Vector2(-40, 30), new Vector2(120, 35), 18, FontStyle.Bold, new Color(1f, 0.3f, 0.4f), TextAnchor.MiddleLeft);
        Text gemTxt = CreateText(statsPanel, "GemValue", "0", new Vector2(50, 30), new Vector2(80, 35), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        menuControl.totalGemsText = gemTxt;

        CreateText(statsPanel, "CoinLabel", "● COINS:", new Vector2(-40, -15), new Vector2(120, 35), 18, FontStyle.Bold, new Color(1f, 0.85f, 0.1f), TextAnchor.MiddleLeft);
        Text coinTxt = CreateText(statsPanel, "CoinValue", "0", new Vector2(50, -15), new Vector2(80, 35), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        menuControl.totalCoinsText = coinTxt;

        CreateText(statsPanel, "DistLabel", "» BEST:", new Vector2(-40, -60), new Vector2(120, 35), 18, FontStyle.Bold, new Color(0.3f, 1f, 0.5f), TextAnchor.MiddleLeft);
        Text distTxt = CreateText(statsPanel, "DistValue", "0m", new Vector2(50, -60), new Vector2(80, 35), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        menuControl.bestDistanceText = distTxt;

        // Action Buttons
        Button startBtn = CreateUIButton(canvasObj, "StartButton", "START DASH", new Vector2(180, 20), new Vector2(240, 65), new Color(0.18f, 0.72f, 0.35f), Color.white, 24);
        menuControl.startButton = startBtn;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(startBtn.onClick, menuControl.OnStartGameClicked);

        Button directBtn = CreateUIButton(canvasObj, "QuickPlayButton", "QUICK RUN", new Vector2(180, -55), new Vector2(240, 50), new Color(0.15f, 0.52f, 0.85f), Color.white, 20);
        menuControl.quickPlayButton = directBtn;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(directBtn.onClick, menuControl.OnDirectPlayClicked);

        Button resetBtn = CreateUIButton(canvasObj, "ResetButton", "RESET DATA", new Vector2(180, -125), new Vector2(240, 45), new Color(0.75f, 0.25f, 0.25f), Color.white, 16);
        menuControl.resetButton = resetBtn;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(resetBtn.onClick, menuControl.OnResetProgressClicked);

        EditorSceneManager.SaveScene(scene, SCENE_MAIN_MENU);
        Debug.Log("[Airport Dash] MainMenu scene created.");
    }
    #endregion

    #region Scene 2: StageSelect.unity
    [MenuItem("Airport Dash/Sub-Tools/4. Generate StageSelect Scene")]
    public static void CreateStageSelectScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera & Light
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.12f, 0.20f);
        camObj.transform.position = new Vector3(0, 2.2f, -5f);
        camObj.transform.rotation = Quaternion.Euler(15f, 0, 0);
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.3f;
        lightObj.transform.rotation = Quaternion.Euler(45f, -30f, 0);

        // Revolving 3D Stage Turntable Diorama (matching Slide 5)
        GameObject turntable = new GameObject("StageTurntable");
        turntable.transform.position = new Vector3(0, 0, 1.5f);

        GameObject baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseCube.name = "StageCube";
        baseCube.transform.SetParent(turntable.transform, false);
        baseCube.transform.localPosition = new Vector3(0, -0.5f, 0);
        baseCube.transform.localScale = new Vector3(3.2f, 1.0f, 3.2f);
        baseCube.GetComponent<Renderer>().sharedMaterial = GetMat("Mat_AirportFloor");

        // Diorama props on top of cube
        GameObject cartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Cart.prefab");
        if (cartPrefab != null)
        {
            GameObject c = PrefabUtility.InstantiatePrefab(cartPrefab) as GameObject;
            c.transform.SetParent(turntable.transform, false);
            c.transform.localPosition = new Vector3(-0.6f, 0, 0.2f);
            c.transform.localScale = Vector3.one * 0.7f;
            Object.DestroyImmediate(c.GetComponent<Collider>());
        }

        GameObject luggagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Luggage.prefab");
        if (luggagePrefab != null)
        {
            GameObject l = PrefabUtility.InstantiatePrefab(luggagePrefab) as GameObject;
            l.transform.SetParent(turntable.transform, false);
            l.transform.localPosition = new Vector3(0.7f, 0, -0.4f);
            l.transform.localScale = Vector3.one * 0.7f;
            Object.DestroyImmediate(l.GetComponent<Collider>());
        }

        // UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        StageControls stageControls = canvasObj.AddComponent<StageControls>();
        stageControls.stagePreviewCube = turntable.transform;
        stageControls.clickSound = GetAudio("button_click");

        CreateText(canvasObj, "Header", "STAGE SELECTION", new Vector2(0, 220), new Vector2(700, 80), 44, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        // Stage Card UI
        GameObject stageCard = CreateUIPanel(canvasObj, "StageCard", new Vector2(0, -140), new Vector2(550, 180), new Color(0.08f, 0.15f, 0.25f, 0.9f));
        Text titleTxt = CreateText(stageCard, "StageTitle", "AIRPORT DASH", new Vector2(0, 50), new Vector2(500, 45), 28, FontStyle.Bold, new Color(0.2f, 0.8f, 1f), TextAnchor.MiddleCenter);
        Text descTxt = CreateText(stageCard, "StageDesc", "Theme: Airport Terminal Concourse\nHazards: Luggage piles · Baggage carts · Boarding stanchions\nAuthor: Sk. Neha Fariyal · 2303A51598", new Vector2(0, -5), new Vector2(500, 70), 16, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter);

        stageControls.stageTitleText = titleTxt;
        stageControls.stageDescriptionText = descTxt;

        Button playBtn = CreateUIButton(stageCard, "PlayButton", "PLAY STAGE", new Vector2(120, -55), new Vector2(180, 48), new Color(0.18f, 0.72f, 0.35f), Color.white, 20);
        Button backBtn = CreateUIButton(stageCard, "BackButton", "BACK", new Vector2(-120, -55), new Vector2(180, 48), new Color(0.45f, 0.5f, 0.55f), Color.white, 18);

        stageControls.playButton = playBtn;
        stageControls.backButton = backBtn;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, stageControls.OnPlayStageClicked);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn.onClick, stageControls.OnBackClicked);

        EditorSceneManager.SaveScene(scene, SCENE_STAGE_SELECT);
        Debug.Log("[Airport Dash] StageSelect scene created.");
    }
    #endregion

    #region Scene 3: Information.unity
    [MenuItem("Airport Dash/Sub-Tools/5. Generate Information Scene")]
    public static void CreateInformationScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera & Light
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.15f, 0.25f);
        camObj.transform.position = new Vector3(0, 1.5f, -5f);
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.0f;

        // UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        LoadToStage loader = canvasObj.AddComponent<LoadToStage>();
        loader.clickSound = GetAudio("button_click");

        CreateText(canvasObj, "Title", "HOW TO PLAY", new Vector2(0, 210), new Vector2(600, 70), 42, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        // Key Control Panels (Slide 5 style: [A] or [D] to steer, [SPACE] to jump)
        GameObject controlsPanel = CreateUIPanel(canvasObj, "ControlsPanel", new Vector2(0, 70), new Vector2(650, 160), new Color(0.06f, 0.10f, 0.18f, 0.85f));

        CreateUIPanel(controlsPanel, "KeyA", new Vector2(-180, 20), new Vector2(90, 80), new Color(0.2f, 0.25f, 0.35f, 1f));
        CreateText(controlsPanel, "KeyAText", "A / ←\nLEFT", new Vector2(-180, 20), new Vector2(90, 80), 18, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        CreateUIPanel(controlsPanel, "KeyD", new Vector2(-60, 20), new Vector2(90, 80), new Color(0.2f, 0.25f, 0.35f, 1f));
        CreateText(controlsPanel, "KeyDText", "D / →\nRIGHT", new Vector2(-60, 20), new Vector2(90, 80), 18, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

        CreateUIPanel(controlsPanel, "KeySpace", new Vector2(130, 20), new Vector2(190, 80), new Color(0.2f, 0.25f, 0.35f, 1f));
        CreateText(controlsPanel, "KeySpaceText", "SPACEBAR\nJUMP OVER OBSTACLES", new Vector2(130, 20), new Vector2(190, 80), 16, FontStyle.Bold, new Color(0.3f, 1f, 0.6f), TextAnchor.MiddleCenter);

        CreateText(controlsPanel, "Desc", "Dodge luggage piles & baggage carts. Jump over stanchion barriers!\nCollect coins and gems to achieve the highest score.", new Vector2(0, -50), new Vector2(620, 45), 15, FontStyle.Normal, Color.yellow, TextAnchor.MiddleCenter);

        // Loading Bar & Status
        GameObject progressBg = CreateUIPanel(canvasObj, "ProgressBg", new Vector2(0, -90), new Vector2(450, 26), new Color(0.2f, 0.2f, 0.2f, 0.9f));
        GameObject fillObj = CreateUIPanel(progressBg, "Fill", new Vector2(-110, 0), new Vector2(220, 22), new Color(0.18f, 0.75f, 0.35f, 1f));

        Slider slider = progressBg.AddComponent<Slider>();
        slider.fillRect = fillObj.GetComponent<RectTransform>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        loader.progressBar = slider;

        Text loadingTxt = CreateText(canvasObj, "LoadingText", "PREPARING BOARDING GATE...", new Vector2(0, -135), new Vector2(500, 40), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        loader.loadingText = loadingTxt;

        Button skipBtn = CreateUIButton(canvasObj, "SkipButton", "START DASH NOW", new Vector2(0, -200), new Vector2(220, 50), new Color(0.15f, 0.55f, 0.9f), Color.white, 18);
        loader.skipButton = skipBtn;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(skipBtn.onClick, loader.OnSkipClicked);

        // Fade image
        GameObject fadeObj = CreateUIPanel(canvasObj, "FadePanel", Vector2.zero, new Vector2(2500, 1500), new Color(0, 0, 0, 0));
        fadeObj.SetActive(false);
        loader.fadeImage = fadeObj.GetComponent<Image>();

        EditorSceneManager.SaveScene(scene, SCENE_INFORMATION);
        Debug.Log("[Airport Dash] Information scene created.");
    }
    #endregion

    #region Scene 4: GamePlay.unity
    [MenuItem("Airport Dash/Sub-Tools/6. Generate GamePlay Scene")]
    public static void CreateGamePlayScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Lighting & Environment
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1.0f, 0.98f, 0.95f);
        light.intensity = 1.35f;
        light.shadows = LightShadows.Soft;
        lightObj.transform.rotation = Quaternion.Euler(45f, -35f, 0);

        // Airport Concourse Ambience Audio
        GameObject ambientObj = new GameObject("AmbientSound");
        AudioSource ambSource = ambientObj.AddComponent<AudioSource>();
        ambSource.clip = GetAudio("airport_ambience");
        ambSource.loop = true;
        ambSource.volume = 0.4f;
        ambSource.Play();

        // 2. Instantiate Player
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        GameObject player = null;
        if (playerPrefab != null)
        {
            player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            player.transform.position = new Vector3(0, 0, 0);
        }

        // 3. Camera Setup
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        CameraFollow camFollow = camObj.AddComponent<CameraFollow>();
        if (player != null) camFollow.target = player.transform;
        camFollow.offset = new Vector3(0, 3.8f, -6.5f);
        camFollow.smoothSpeed = 12f;

        // 4. Segment Generator
        GameObject genObj = new GameObject("SegmentGenerator");
        SegmentGenerator generator = genObj.AddComponent<SegmentGenerator>();
        if (player != null) generator.playerTransform = player.transform;
        generator.segmentLength = 50f;
        generator.initialSegmentsOnScreen = 7;
        generator.spawnDistanceThreshold = 200f;

        // Assign Prefabs to generator
        GameObject segPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Airport_Segment.prefab");
        generator.defaultSegmentTemplate = segPrefab;

        GameObject lugObs = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Luggage.prefab");
        GameObject cartObs = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Cart.prefab");
        GameObject stanObs = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Stanchion.prefab");
        GameObject planeObs = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacle_Airplane.prefab");
        generator.obstaclePrefabs = new GameObject[] { lugObs, cartObs, stanObs, planeObs };
        generator.airplaneObstaclePrefab = planeObs;

        GameObject flyPlane = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Flying_Airplane.prefab");
        generator.flyingAirplanePrefab = flyPlane;

        generator.coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");
        generator.gemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gem.prefab");

        // 5. In-Game HUD Canvas
        GameObject canvasObj = new GameObject("InGameHUD");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        MasterLevelInfo levelInfo = canvasObj.AddComponent<MasterLevelInfo>();
        levelInfo.coinSound = GetAudio("coin_pickup");
        levelInfo.gemSound = GetAudio("gem_pickup");

        // Top-Left In-Game Stats Panel (matching Capstone Slide 5 & 7)
        GameObject statsBox = CreateUIPanel(canvasObj, "HUD_StatsBox", new Vector2(-280, 190), new Vector2(210, 150), new Color(0.08f, 0.12f, 0.18f, 0.85f));

        // Red Gem Row
        CreateText(statsBox, "GemIcon", "◆", new Vector2(-75, 45), new Vector2(35, 35), 24, FontStyle.Bold, new Color(1f, 0.25f, 0.35f), TextAnchor.MiddleCenter);
        Text gemTxt = CreateText(statsBox, "GemCount", "0", new Vector2(10, 45), new Vector2(110, 35), 22, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        levelInfo.gemText = gemTxt;

        // Gold Coin Row
        CreateText(statsBox, "CoinIcon", "●", new Vector2(-75, 5), new Vector2(35, 35), 24, FontStyle.Bold, new Color(1f, 0.85f, 0.1f), TextAnchor.MiddleCenter);
        Text coinTxt = CreateText(statsBox, "CoinCount", "0", new Vector2(10, 5), new Vector2(110, 35), 22, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        levelInfo.coinText = coinTxt;

        // Green Distance Row
        CreateText(statsBox, "DistIcon", "»", new Vector2(-75, -35), new Vector2(35, 35), 24, FontStyle.Bold, new Color(0.2f, 1f, 0.4f), TextAnchor.MiddleCenter);
        Text distTxt = CreateText(statsBox, "DistanceCount", "0m", new Vector2(10, -35), new Vector2(110, 35), 22, FontStyle.Bold, Color.white, TextAnchor.MiddleRight);
        levelInfo.distanceText = distTxt;

        // Top-Right Pause Button
        CreateUIButton(canvasObj, "PauseButton", "||", new Vector2(340, 220), new Vector2(50, 50), new Color(0.12f, 0.5f, 0.85f), Color.white, 20);

        // Screen Fade Out Overlay (for collisions)
        GameObject fadeObj = CreateUIPanel(canvasObj, "FadeOutPanel", Vector2.zero, new Vector2(2500, 1500), new Color(0, 0, 0, 0));
        fadeObj.SetActive(false);

        // Game Over Banner
        GameObject gameOverUI = CreateUIPanel(canvasObj, "GameOverBanner", new Vector2(0, 40), new Vector2(450, 160), new Color(0.7f, 0.1f, 0.1f, 0.92f));
        CreateText(gameOverUI, "GOTitle", "FLIGHT MISSED!\n(COLLISION DETECTED)", new Vector2(0, 25), new Vector2(400, 80), 24, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        CreateText(gameOverUI, "GORestart", "Restarting Terminal Run...", new Vector2(0, -35), new Vector2(400, 40), 18, FontStyle.Normal, Color.yellow, TextAnchor.MiddleCenter);
        gameOverUI.SetActive(false);

        // Wire Collision Detection to UI
        if (player != null)
        {
            CollisionDetect colDetect = player.GetComponent<CollisionDetect>();
            if (colDetect != null)
            {
                colDetect.mainCamera = cam;
                colDetect.fadeOutPanel = fadeObj.GetComponent<Image>();
                colDetect.gameOverUI = gameOverUI;
            }
        }

        EditorSceneManager.SaveScene(scene, SCENE_GAMEPLAY);
        Debug.Log("[Airport Dash] GamePlay scene created.");
    }
    #endregion

    #region Build Settings Configuration
    [MenuItem("Airport Dash/Sub-Tools/7. Configure Build Settings")]
    public static void ConfigureBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(SCENE_MAIN_MENU, true),
            new EditorBuildSettingsScene(SCENE_STAGE_SELECT, true),
            new EditorBuildSettingsScene(SCENE_INFORMATION, true),
            new EditorBuildSettingsScene(SCENE_GAMEPLAY, true)
        };

        EditorBuildSettings.scenes = scenes;
        Debug.Log("[Airport Dash] EditorBuildSettings configured with 4 scenes.");
    }

    [MenuItem("Airport Dash/Sub-Tools/8. Build Windows Standalone Executable")]
    public static void BuildWindowsStandalone()
    {
        ConfigureBuildSettings();

        string buildFolder = "Builds/AirportDash";
        if (!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
        }

        string exePath = $"{buildFolder}/AirportDash.exe";
        string[] scenePaths = new string[]
        {
            SCENE_MAIN_MENU,
            SCENE_STAGE_SELECT,
            SCENE_INFORMATION,
            SCENE_GAMEPLAY
        };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenePaths;
        buildPlayerOptions.locationPathName = exePath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[Airport Dash] Windows Standalone Build SUCCEEDED at: {exePath} (Size: {report.summary.totalSize / 1024 / 1024} MB)");
        }
        else
        {
            Debug.LogError($"[Airport Dash] Build FAILED: {report.summary.result}");
        }
    }
    #endregion

    #region UI Helper Utilities
    private static GameObject CreateUIPanel(GameObject parent, string name, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent.transform, false);

        RectTransform rt = panelObj.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image img = panelObj.AddComponent<Image>();
        img.color = color;

        return panelObj;
    }

    private static Text CreateText(GameObject parent, string name, string content, Vector2 anchoredPos, Vector2 size, int fontSize, FontStyle fontStyle, Color color, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Text txt = textObj.AddComponent<Text>();
        txt.text = content;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = fontSize;
        txt.fontStyle = fontStyle;
        txt.color = color;
        txt.alignment = alignment;

        return txt;
    }

    private static Button CreateUIButton(GameObject parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color bgColor, Color textColor, int fontSize)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        img.raycastTarget = true;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock cb = btn.colors;
        cb.highlightedColor = bgColor * 1.15f;
        cb.pressedColor = bgColor * 0.85f;
        btn.colors = cb;

        Text txt = CreateText(btnObj, "Text", label, Vector2.zero, size, fontSize, FontStyle.Bold, textColor, TextAnchor.MiddleCenter);
        txt.raycastTarget = false;

        return btn;
    }
    #endregion
}

