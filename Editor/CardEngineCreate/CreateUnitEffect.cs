using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SadSapphicGames.CardEngine;
using System;
using UnityEngine.Windows;

namespace SadSapphicGames.CardEngineEditor {

    public class CreateUnitEffectWindow : EditorWindow {
        static CreateUnitEffectWindow instance = null;
        bool effectIsCompiling = false;
        [SerializeField]string effectName = ""; 
        [SerializeField]string effectsDirectory;
        EffectDatabaseSO effectDatabase;
        bool closeWindow;

        public CreateUnitEffectWindow() : base() {

        }
        private void OnEnable() {
            var settings = SettingsEditor.ReadSettings(); 
            effectDatabase = EffectDatabaseSO.Instance;

            if(effectDatabase == null || settings == null) {
                closeWindow = true;
                Debug.LogWarning("please finish initializing CardEngine before using the CardEngine/Create menu");
            } else {
                effectsDirectory = settings.Directories.Effects;
                if(!Directory.Exists(effectsDirectory)) {
                    closeWindow = true;
                    Debug.LogWarning("selected directory invalid, please select a valid directory to store card effects using the CardEngine/Settings menu");
                } 
            }
        }
        [MenuItem("CardEngine/Create/Unit Effect")]
        static void Init() {
            instance = EditorWindow.CreateInstance<CreateUnitEffectWindow>();
            instance.Show();
        }
        private void OnGUI() {
            if(closeWindow) this.Close();
            if (!effectIsCompiling) {
                GUILayout.Label("Create a new unit effect", EditorStyles.boldLabel);
                effectName = EditorGUILayout.TextField("Enter effect name",effectName);
                GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Create effect",EditorStyles.miniButtonLeft)) {
                        if(effectName == "") {
                            Debug.LogWarning("No type name entered");
                            this.Close();
                        }
                        if(Directory.Exists(effectsDirectory + "/" + effectName)) {
                            this.Close();
                            throw new Exception($"Folder for type {effectName} already exists");
                        }
                        AssetDatabase.CreateFolder(effectsDirectory, effectName);
                        string newEffectDirectory = effectsDirectory + "/" + effectName;
                        TemplateIO.CopyTemplate("UnitEffectTemplate.cs",effectName+".cs",newEffectDirectory);
                        AssetDatabase.ImportAsset($"{newEffectDirectory}/{effectName}.cs");
                        AssetDatabase.Refresh();
                        effectIsCompiling = true;

                        // TypeSO UnitEffectSO = ScriptableObject.CreateInstance<TypeSO>();
                        // UnitEffectSO.name = effectName;
                        // AssetDatabase.CreateAsset(UnitEffectSO,$"{typePath}/{typeName}.asset");
                        // typeDatabase.AddEntry(UnitEffectSO, typePath);
                        // AssetDatabase.SaveAssets();
                    }
                    if(GUILayout.Button("Cancel",EditorStyles.miniButtonRight)) {
                        this.Close();
                    }
                GUILayout.EndHorizontal();
            } else if(effectIsCompiling) {
                GUILayout.Label("Please wait while effect compiles", EditorStyles.boldLabel);
            }
            if(instance == null) {
                Debug.Log($"done compiling effect {effectName}");
                //? We can tell when the generated script has compiled because after that happens the variables of the EditorWindow return to their initial value
                //? However this means we also lose the information such as effectName entered
                //? Which we need to be able to create a scriptable object of the generated type
                Type effectType = Type.GetType(effectName + ",Assembly-CSharp");
                UnitEffectSO effectSO = (UnitEffectSO)ScriptableObject.CreateInstance(effectType);
                effectSO.name = effectName;
                AssetDatabase.CreateAsset(effectSO,$"{effectsDirectory}/{effectName}/{effectName}.asset");
                effectDatabase.AddEntry(effectSO,effectsDirectory);
                this.Close();
            }
        }

    }
}