﻿// WraithGUI by Karma Kitten
// For Phasmophobia build #5635423, manifest #4488252479481708564

using MelonLoader;
using PhasmoTestMod;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WraithGUI
{
    public sealed class PhasmoGame : MelonMod
    {
        private static readonly GUIStyle GUIStyle = new GUIStyle();
        public static readonly GUIStyle backgroundStyle = new GUIStyle();
        private GUIEx.DropdownState ghostDropdownState = new GUIEx.DropdownState();
        private bool menuIsEnabled = false;
        private bool mainMenuIsEnabled = false;
        private bool playerMenuIsEnabled = false;
        private bool ghostMenuIsEnabled = false;
        private bool levelMenuIsEnabled = false;
        private bool listGhostsIsEnabled = false;
        private bool ghostsAlwaysVisible = false;
        private bool spawnMenuIsEnabled = false;
        private string ghostNameEdit = "";
        private int levelNum;
        private float playerSpeed = 1.2f;
        private static string[] mapNames = { "Opening Scene", "Lobby", "Tanglewood Street", "Ridgeview Road House", "Edgefield Street House", "Asylum", "Brownstone High School", "Bleasdale Farmhouse" , "Grafton Farmhouse" };
        private static string[] ghostTypeStrings = { "None", "Spirit", "Wraith", "Phantom", "Poltergeist", "Banshee", "Jinn", "Mare", "Revenant", "Shade", "Demon", "Yurei", "Oni"};
        private static GhostTraits.Type[] ghostTypesArr = { GhostTraits.Type.none, GhostTraits.Type.Spirit, GhostTraits.Type.Wraith, GhostTraits.Type.Phantom, GhostTraits.Type.Poltergeist, GhostTraits.Type.Banshee, GhostTraits.Type.Jinn, GhostTraits.Type.Mare, GhostTraits.Type.Revenant, GhostTraits.Type.Shade, GhostTraits.Type.Demon, GhostTraits.Type.Yurei, GhostTraits.Type.Oni };

        public static LevelController levelController;
        public static GhostController ghostController;
        public static Player player;
        public static SetupPhaseController setupPhaseController;

        public override void OnApplicationStart()
        {
            MelonLogger.Log($"Successfully loaded v{typeof(PhasmoGame).Assembly.GetName().Version}. Enjoy!");
            GUIStyle.alignment = TextAnchor.UpperCenter;
            GUIStyle.fontSize = 16;
            GUIStyle.fontStyle = FontStyle.Bold;
            backgroundStyle.alignment = TextAnchor.MiddleCenter;
            backgroundStyle.normal.textColor = Color.white;
        }
        public override void OnLevelWasInitialized(int level)
        {
            levelNum = level; // Save this for checks later.
            MelonLogger.Log($"{mapNames[level]} was initialized");
            InitializeGameObjects(); // This will ensure the level is never null

            if (level == 1) { GUIMenu.ghostText = ""; }
        }
        public override void OnUpdate()
        {
            // Open Menu (F1)
            if (Keyboard.current.f1Key.wasPressedThisFrame) { menuIsEnabled = !menuIsEnabled; }

            GUIMenu.CycleColors(GUIStyle);
            GUIMenu.CycleColors(backgroundStyle, true);

            if (levelNum > 1) { player.firstPersonController.m_WalkSpeed = playerSpeed; }

            // Always visible ghosts
            if (ghostsAlwaysVisible && levelNum > 1 && levelController.currentGhost != null)
            {
                foreach (GhostAI ghost in CustomGhostController.ghosts)
                {
                    if (!ghost.ghostIsAppeared)
                    {
                        ghost.Appear(true);
                    }
                }
            }
        }
        public override void OnGUI()
        {
            if (menuIsEnabled)
            {
                GUI.Box(new Rect(0, 0, Screen.width, 30), "");
                GUI.Box(new Rect(0, 5, Screen.width, 25), "WraithGUI", GUIStyle);

                if (GUI.Button(new Rect(5, 0, 50, 25), "Main", backgroundStyle)) { mainMenuIsEnabled = !mainMenuIsEnabled; }

                if (GUI.Button(new Rect(65, 0, 60, 25), "Maps", backgroundStyle))
                {
                    
                }

                if (GUI.Button(new Rect(135, 0, 65, 25), "Colors", backgroundStyle))
                {

                }

                if (GUI.Button(new Rect(210, 0, 65, 25), "Players", backgroundStyle))
                {

                }

                if (mainMenuIsEnabled)
                {
                    playerMenuIsEnabled = false;
                    levelMenuIsEnabled = false;
                    ghostMenuIsEnabled = false;
                    GUI.Box(new Rect(0, 31, 150, 250), "");

                    if (GUI.Button(new Rect(20, 40, 100, 25), "Player Menu", backgroundStyle)) { playerMenuIsEnabled = !playerMenuIsEnabled; }

                    if (GUI.Button(new Rect(20, 70, 100, 25), "Ghost Menu", backgroundStyle)) { ghostMenuIsEnabled = !ghostMenuIsEnabled; }
                    
                    if (GUI.Button(new Rect(20, 100, 100, 25), "Level Menu", backgroundStyle)) { levelMenuIsEnabled = !levelMenuIsEnabled; }
                }

                if (playerMenuIsEnabled)
                {
                    mainMenuIsEnabled = false;
                    ghostMenuIsEnabled = false;
                    levelMenuIsEnabled = false;
                    GUI.Box(new Rect(0, 31, 150, 250), "");
                    if (GUI.Button(new Rect(20, 40, 100, 25), "player test", backgroundStyle)) {  }
                    playerSpeed = GUI.HorizontalSlider(new Rect(20, 70, 100, 30), playerSpeed, 0.0F, 10.0F);
                }

                if (ghostMenuIsEnabled)
                {
                    mainMenuIsEnabled = false;
                    playerMenuIsEnabled = false;
                    levelMenuIsEnabled = false;
                    GUI.Box(new Rect(0, 31, 150, 250), "");

                    if (GUI.Button(new Rect(20, 40, 100, 25), "List Ghosts", backgroundStyle)) { listGhostsIsEnabled = !listGhostsIsEnabled; }

                    if (GUI.Button(new Rect(20, 70, 100, 25), "Spawn Menu", backgroundStyle) /*&& levelNum > 1*/)
                    {
                        spawnMenuIsEnabled = !spawnMenuIsEnabled;
                    }

                    if (GUI.Button(new Rect(20, 100, 100, 25), "Ghosts Visible", backgroundStyle)) { ghostsAlwaysVisible = !ghostsAlwaysVisible; }

                    if (GUI.Button(new Rect(20, 130, 100, 25), "Start Hunting", backgroundStyle))
                    {
                        setupPhaseController.ForceEnterHuntingPhase();
                        foreach (GhostAI ghost in CustomGhostController.ghosts)
                        {
                            ghost.state = GhostAI.States.hunting;
                            ghost.isHunting = true;
                            ghost.isChasingPlayer = true;
                        }
                    }

                    if (listGhostsIsEnabled) { GUI.TextArea(new Rect(0, 300, 300, 300), GUIMenu.ghostText); }

                    if (spawnMenuIsEnabled)
                    {
                        GUI.Box(new Rect(200, 31, 250, 250), "");
                        GUI.Box(new Rect(200, 33, 250, 250), "Spawn Menu", GUIStyle);
                        GUI.Label(new Rect(220, 60, 300, 250), "Type:");
                        GUI.Label(new Rect(220, 90, 300, 250), "Name:");

                        ghostNameEdit = GUI.TextField(new Rect(290, 92, 140, 20), ghostNameEdit, 25);

                        if (GUI.Button(new Rect(280, 250, 100, 25), "Spawn", backgroundStyle))
                        {
                            ghostController.SpawnGhost(ghostTypesArr[ghostDropdownState.Select], ghostNameEdit);
                            GUIMenu.ghostText += GhostMethods.GhostToString();
                        }
                        
                        var styles = new GUIEx.DropdownStyles("button", "box", Color.white, 24, 8);
                        ghostDropdownState = GUIEx.Dropdown(new Rect(290, 57, 150, 30), ghostTypeStrings, ghostDropdownState, styles);
                    }
                }

                if (levelMenuIsEnabled)
                {
                    mainMenuIsEnabled = false;
                    playerMenuIsEnabled = false;
                    ghostMenuIsEnabled = false;
                    GUI.Box(new Rect(0, 31, 150, 250), "");
                    if (GUI.Button(new Rect(20, 40, 100, 25), "level test", backgroundStyle)) {  }
                }
            }
        }

        private void InitializeGameObjects()
        {
            levelController = UnityEngine.Object.FindObjectOfType<LevelController>();
            ghostController = UnityEngine.Object.FindObjectOfType<GhostController>();
            player = UnityEngine.Object.FindObjectOfType<Player>();
            setupPhaseController = UnityEngine.Object.FindObjectOfType<SetupPhaseController>();
        }

    }
}