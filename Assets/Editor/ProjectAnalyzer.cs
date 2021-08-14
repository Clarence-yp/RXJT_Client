




using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
namespace ProjectAnalyzer
{
    public class ProjectAnalyzer : EditorWindow
    {
        enum ActiveType
        {
            Home, Textures, Models, Materials,Audios
        };

        enum ActiveSubType
        {
            Details, Helps,Settings
        };


        private string[] toolbarStrings = {LanguageCfg.HOME,LanguageCfg.TEXTURES,LanguageCfg.MODELS,LanguageCfg.MATERIALS,LanguageCfg.AUDIOS };
        private ActiveType activeType;       

        private ProjectInfo projectInfo = new ProjectInfo();

        #region 插件有关的函数
        [MenuItem("Window/ProjectAnalyzer/ResourceAnalyzer")]
        static void ProjectAnalyzerStart()
        {
            ProjectAnalyzer window = (ProjectAnalyzer)EditorWindow.GetWindow(typeof(ProjectAnalyzer));
            window.title = "Analyzer";
            window.minSize = new Vector2(1000, 500);
            //window.Init();
        }

        [MenuItem("Assets/AssetView")]
        public static void ProjectAnalyzerView()
        {
            UnityEngine.Object asset = Selection.activeGameObject;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            RefInfo refInfo = null;
            if (asset is AudioClip)
            {
                refInfo = new AudioInfo(asset, assetPath);
            }
            else if (asset is Texture)
            {
                refInfo = new TextureInfo(asset as Texture, assetPath);
            }
            else
            {
                ModelImporter mi = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (mi != null)
                {
                    refInfo = new ModelInfo(mi, assetPath);
                }
            }

            if (refInfo != null)
            {
                EditorUtility.DisplayDialog("Tips", refInfo.GetResInfoDetails(), "OK");
            }
        }




        public void Init()
        {
            ProjectCheckSetting.Apply(false);
            projectInfo.Init();
            Repaint();
        }

        void OnGUI()
        {
            if(GUILayout.Button("刷新资源",GUILayout.Width(200)))
            {
                Init();
            }
            activeType = (ActiveType)GUILayout.Toolbar((int)activeType, toolbarStrings);

            switch (activeType)
            {
                case ActiveType.Home:
                    DrawHome();
                    break;
                case ActiveType.Textures:
                    DrawTextures();
                    break;
                case ActiveType.Models:
                    DrawModels();
                    break;
                case ActiveType.Materials:
                    DrawMaterials();
                    break;
                case ActiveType.Audios:
                    DrawAudios();
                    break;
            }
        }


        private void DrawHelpTips(string helpInfos)
        {
            GUILayout.Label(helpInfos, GUILayout.MaxWidth(1500));
        }

        private void DrawProposeTips(RefInfo refInfo)
        {
            if (refInfo.proposeTipCount > 0)
            {
                if (GUILayout.Button("建议", GUILayout.Width(100)))
                {
                    PingAssetInProject(refInfo.path);
                    EditorUtility.DisplayDialog("Tips", refInfo.GetResInfoDetails(), "OK");
                }

            }
        }

        #endregion


        #region 总览
        private Vector2 scrollPosHome = new Vector2(0, 0);
        void DrawHome()
        {
            scrollPosHome = EditorGUILayout.BeginScrollView(scrollPosHome);

            GUILayout.Label("Welcome，使用前请点击上方刷新按钮", GUILayout.Width(400));
            GUILayout.Space(20);
            GUILayout.Label("工程资源列表如下: ", GUILayout.Width(400));
            GUILayout.Space(10);
            GUILayout.Label(string.Format("纹理贴图个数：  {0}", projectInfo.textures.Count), GUILayout.MinWidth(100));
            GUILayout.Label(string.Format("模型FBX个数 ：  {0}", projectInfo.models.Count), GUILayout.MinWidth(100));
            GUILayout.Label(string.Format("材质球个数  ：  {0}", projectInfo.materials.Count), GUILayout.MinWidth(100));
            GUILayout.Label(string.Format("音效资源个数：  {0}", projectInfo.audios.Count), GUILayout.MinWidth(100));

            GUILayout.Space(20);
            GUILayout.Label("大家如果在使用中感觉有更好的改进，请即使联系啊。争取把它做的牛逼轰轰的。", GUILayout.MinWidth(100));

            
            EditorGUILayout.EndScrollView();
        }
        #endregion



        #region 纹理贴图相关
        private Vector2 scrollPosTexture = new Vector2(0, 0);
        private ActiveSubType actSubTypeTexture = ActiveSubType.Details;
        private string[] textureToolStrings = { LanguageCfg.DETAILS, LanguageCfg.HELPS, LanguageCfg.SETTINGS };


        
        void DrawTextures()
        {
            GUILayout.Space(10);
            actSubTypeTexture = (ActiveSubType)GUILayout.Toolbar((int)actSubTypeTexture, textureToolStrings, GUILayout.MaxWidth(200));
            GUILayout.Space(10);

            scrollPosTexture = EditorGUILayout.BeginScrollView(scrollPosTexture);
            if (actSubTypeTexture == ActiveSubType.Details)
            {

                //绘制title
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageCfg.NAME, GUILayout.Width(100)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.Name);
                }
                if(GUILayout.Button(LanguageCfg.MemorySize, GUILayout.Width(100)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.MemorySize);
                }
                if (GUILayout.Button(LanguageCfg.PIX_W, GUILayout.Width(50)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.PixWidth);
                }
                if (GUILayout.Button(LanguageCfg.PIX_H, GUILayout.Width(50)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.PixHeigh);
                }
                if (GUILayout.Button(LanguageCfg.IsRW, GUILayout.Width(50)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.IsRW);
                }
                if (GUILayout.Button(LanguageCfg.OverridePlat, GUILayout.Width(100)))
                {                    
                }
                if (GUILayout.Button(LanguageCfg.Mipmap, GUILayout.Width(100)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.Mipmap);
                }
                if (GUILayout.Button(LanguageCfg.IsLightmap, GUILayout.Width(80)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.IsLightmap);
                }
                if (GUILayout.Button(LanguageCfg.AnisoLevel, GUILayout.Width(80)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.AnisoLevel);
                }

                if (GUILayout.Button(LanguageCfg.PROPOSE, GUILayout.Width(100)))
                {
                    projectInfo.SortTexture(TextureInfo.SortType.Propose);
                }

                GUILayout.EndHorizontal();

                for (int i = 0; i < projectInfo.textures.Count; i++)
                {
                    TextureInfo textureInfo = projectInfo.textures[i];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(textureInfo.name, GUILayout.Width(100)))
                    {
                        PingAssetInProject(textureInfo.path);
                        //Selection.activeObject = textureInfo.texture;
                    }
                    GUILayout.Space(10);
                    GUILayout.Label(textureInfo.GetSize(), GUILayout.MaxWidth(100));

                    GUILayout.Label(textureInfo.width + "x" + textureInfo.height, GUILayout.MaxWidth(100));
                    GUILayout.Label(textureInfo.isRW.ToString(), GUILayout.MaxWidth(50));

                    GUILayout.Label(textureInfo.IsOverridePlatform(), GUILayout.MaxWidth(100));
                    GUILayout.Label(textureInfo.isMipmap.ToString(), GUILayout.MaxWidth(100));
                    GUILayout.Label(textureInfo.isLightmap.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(textureInfo.anisoLevel.ToString(), GUILayout.MaxWidth(80));
                    DrawProposeTips(textureInfo);
                    GUILayout.EndHorizontal();
                }
            }
            else if (actSubTypeTexture == ActiveSubType.Settings)
            {
                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckMemSize = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckMemSize, "检查内存大小", GUILayout.MaxWidth(100));
                ProjectCheckSettingUI.textureCheckMemSizeValue = GUILayout.TextField(ProjectCheckSettingUI.textureCheckMemSizeValue, GUILayout.MaxWidth(80));
                GUILayout.Label("kb", GUILayout.MaxWidth(20));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckPix = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckPix, "检查分辨率", GUILayout.MaxWidth(100));
                ProjectCheckSettingUI.textureCheckPixW = GUILayout.TextField(ProjectCheckSettingUI.textureCheckPixW, GUILayout.MaxWidth(80));
                GUILayout.Label("x", GUILayout.MaxWidth(20));
                ProjectCheckSettingUI.textureCheckPixH = GUILayout.TextField(ProjectCheckSettingUI.textureCheckPixH, GUILayout.MaxWidth(80));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckPix2Pow = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckPix2Pow, "检查2N次幂", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckIsRW = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckIsRW, "检查可读写", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckPlatSetting = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckPlatSetting, "检查平台设置", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.textureCheckIsLightmap = GUILayout.Toggle(ProjectCheckSettingUI.textureCheckIsLightmap, "检查lightmap格式", GUILayout.MaxWidth(150));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("应用", GUILayout.MaxWidth(100)))
                {
                    ProjectCheckSetting.Apply(true);
                    projectInfo.ReCheckTextures();
                }
            }
            else
            {
                DrawHelpTips(LanguageCfg.HELP_TEXTURE);
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion

     

        #region 音效部分
        private Vector2 scrollPosAudio = new Vector2(0, 0);
        private ActiveSubType actSubTypeAudio = ActiveSubType.Details;
        private string[] audioToolStrings = { LanguageCfg.DETAILS, LanguageCfg.HELPS, LanguageCfg.SETTINGS };
    

        void DrawAudios()
        {
            GUILayout.Space(10);
            actSubTypeAudio = (ActiveSubType)GUILayout.Toolbar((int)actSubTypeAudio, audioToolStrings, GUILayout.MaxWidth(200));
            GUILayout.Space(10);


            scrollPosAudio = EditorGUILayout.BeginScrollView(scrollPosAudio);
            if (actSubTypeAudio == ActiveSubType.Details)
            {   
                string info = "Audios counts  {0}";
                GUILayout.Label(string.Format(info, projectInfo.audios.Count), GUILayout.MinWidth(100));
                GUILayout.Space(10);

                //绘制title
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageCfg.NAME, GUILayout.Width(120)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.Name);
                }
                if (GUILayout.Button(LanguageCfg.AudioFormat, GUILayout.Width(120)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.AudioFormat);
                }
                if (GUILayout.Button(LanguageCfg.IsThreeD, GUILayout.Width(50)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.IsThreeD);
                }
                if (GUILayout.Button(LanguageCfg.ForceToMono, GUILayout.Width(80)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.ForceToMono);
                }
                if (GUILayout.Button(LanguageCfg.CompressionBitrate, GUILayout.Width(80)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.CompressionBitrate);
                }

                if (GUILayout.Button(LanguageCfg.PROPOSE, GUILayout.Width(100)))
                {
                    projectInfo.SortAudio(AudioInfo.SortType.Propose);
                }

                GUILayout.EndHorizontal();


                for (int i = 0; i < projectInfo.audios.Count; i++)
                {
                    AudioInfo audioInfo = projectInfo.audios[i];
                    GUILayout.BeginHorizontal();


                    if (GUILayout.Button(audioInfo.name, GUILayout.Width(120)))
                    {
                        PingAssetInProject(audioInfo.path);
                    }
                    GUILayout.Space(10);
                    GUILayout.Label(audioInfo.audioFormat.ToString(), GUILayout.MaxWidth(120));
                    GUILayout.Label(audioInfo.isThreeD.ToString(), GUILayout.MaxWidth(50));
                    GUILayout.Label(audioInfo.forceToMono.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(audioInfo.compressionBitrate.ToString(), GUILayout.MaxWidth(80));
                    DrawProposeTips(audioInfo);
                    GUILayout.EndHorizontal();
                }
            }
            else if (actSubTypeAudio == ActiveSubType.Settings)
            {
                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.audioCheckCompression = GUILayout.Toggle(ProjectCheckSettingUI.audioCheckCompression, "检查压缩格式", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.audioCheckIs3D = GUILayout.Toggle(ProjectCheckSettingUI.audioCheckIs3D, "检查3D音效格式", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();



                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.audioCheckRate = GUILayout.Toggle(ProjectCheckSettingUI.audioCheckRate, "检查音频采样率", GUILayout.MaxWidth(100));
                ProjectCheckSettingUI.audioCheckRateValue = GUILayout.TextField(ProjectCheckSettingUI.audioCheckRateValue, GUILayout.MaxWidth(80));
                GUILayout.Label("kbps", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();


                if (GUILayout.Button("应用", GUILayout.MaxWidth(100)))
                {
                    ProjectCheckSetting.Apply(true);
                    projectInfo.ReCheckAudios();
                }

            }
            else
            {
                DrawHelpTips(LanguageCfg.HELP_AUDIO);
            }

            EditorGUILayout.EndScrollView();
        }
        #endregion 

        #region 材质球部分

        private Vector2 scrollPosMaterial = new Vector2(0, 0);
        void DrawMaterials()
        {
            scrollPosMaterial = EditorGUILayout.BeginScrollView(scrollPosMaterial);

            GUILayout.Space(10);
            string info = "Materials counts  {0}";
            GUILayout.Label(string.Format(info, projectInfo.materials.Count), GUILayout.MinWidth(100));
            GUILayout.Space(10);

            //绘制title
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(LanguageCfg.NAME, GUILayout.Width(120)))
            {
                projectInfo.SortMaterial(MaterialInfo.SortType.Name);
            }
            if (GUILayout.Button(LanguageCfg.ShaderName, GUILayout.Width(500)))
            {
                projectInfo.SortMaterial(MaterialInfo.SortType.ShaderName);
            }
            if (GUILayout.Button(LanguageCfg.PROPOSE, GUILayout.Width(100)))
            {
                projectInfo.SortMaterial(MaterialInfo.SortType.Propose);
            }

            GUILayout.EndHorizontal();


            for (int i = 0; i < projectInfo.materials.Count; i++)
            {
                MaterialInfo materialInfo = projectInfo.materials[i];
                GUILayout.BeginHorizontal();


                if (GUILayout.Button(materialInfo.name, GUILayout.Width(120)))
                {
                    PingAssetInProject(materialInfo.path);
                }
                GUILayout.Space(10);
                GUILayout.Label(materialInfo.shaderName.ToString(), GUILayout.MaxWidth(500));
                DrawProposeTips(materialInfo);
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

        }
        #endregion

        #region 工程模型部分相关
        private ActiveSubType actSubTypeModel = ActiveSubType.Details;
      
        private string[] modelToolStrings = { LanguageCfg.DETAILS, LanguageCfg.HELPS, LanguageCfg.SETTINGS };
      
            
        private Vector2 scrollPosModel = new Vector2(0, 0);
        void DrawModels()
        {
            GUILayout.Space(10);
            actSubTypeModel = (ActiveSubType)GUILayout.Toolbar((int)actSubTypeModel, modelToolStrings, GUILayout.MaxWidth(200));
            GUILayout.Space(10);


            scrollPosModel = EditorGUILayout.BeginScrollView(scrollPosModel);
            if (actSubTypeModel == ActiveSubType.Details)
            {
                string info = "Model counts  {0}";
                GUILayout.Label(string.Format(info, projectInfo.models.Count), GUILayout.MinWidth(100));
                GUILayout.Space(10);

                //绘制title
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(LanguageCfg.NAME, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.Name);
                }
                if (GUILayout.Button(LanguageCfg.Scale, GUILayout.Width(50)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.Scale);
                }
                if (GUILayout.Button(LanguageCfg.MeshCompress, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.MeshCompression);
                }
                if (GUILayout.Button(LanguageCfg.AnimCompress, GUILayout.Width(180)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.AnimCompression);
                }
                if (GUILayout.Button(LanguageCfg.AnimCnt, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.AnimationClipCount);
                }
                if (GUILayout.Button(LanguageCfg.IsRW, GUILayout.Width(50)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.IsRW);
                }
                if (GUILayout.Button(LanguageCfg.Collider, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.Collider);
                }
                if (GUILayout.Button(LanguageCfg.NormalMode, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.NormalImportMode);
                }
                if (GUILayout.Button(LanguageCfg.TangentMode, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.TangentImportMode);
                }
                if (GUILayout.Button(LanguageCfg.BakeIK, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.BakeIK);
                }
                if (GUILayout.Button(LanguageCfg.FileSize, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.FileSize);
                }
                if (GUILayout.Button(LanguageCfg.SkinnedMeshCnt, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.SkinnedMeshCount);
                }

                if (GUILayout.Button(LanguageCfg.MeshFilterCnt, GUILayout.Width(100)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.MeshFilterCount);
                }
                
                if (GUILayout.Button(LanguageCfg.VertexCnt, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.VertexCount);
                }
                if (GUILayout.Button(LanguageCfg.TriangleCnt, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.TriangleCount);
                }
                if (GUILayout.Button(LanguageCfg.BoneCnt, GUILayout.Width(80)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.BoneCount);
                }

                if (GUILayout.Button(LanguageCfg.PROPOSE, GUILayout.Width(100)))
                {
                    projectInfo.SortModel(ModelInfo.SortType.Propose);
                }

                GUILayout.EndHorizontal();


                for (int i = 0; i < projectInfo.models.Count; i++)
                {
                    ModelInfo modelInfo = projectInfo.models[i];
                    GUILayout.BeginHorizontal();


                    if (GUILayout.Button(modelInfo.name, GUILayout.Width(80)))
                    {
                        PingAssetInProject(modelInfo.path);
                    }
                    GUILayout.Space(10);
                    GUILayout.Label(modelInfo.scale.ToString(), GUILayout.MaxWidth(50));
                    GUILayout.Label(modelInfo.meshCompression.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.animCompression.ToString(), GUILayout.MaxWidth(180));
                    GUILayout.Label(modelInfo.animationClipCount.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.isRW.ToString(), GUILayout.MaxWidth(50));
                    GUILayout.Label(modelInfo.isAddCollider.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.normalImportMode.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.tangentImportMode.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.isBakeIK.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.GetFileLenth(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.skinnedMeshCount.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.meshFilterCount.ToString(), GUILayout.MaxWidth(100));
                    
                    GUILayout.Label(modelInfo.vertexCount.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.triangleCount.ToString(), GUILayout.MaxWidth(80));
                    GUILayout.Label(modelInfo.boneCount.ToString(), GUILayout.MaxWidth(80));

                    DrawProposeTips(modelInfo);
                    GUILayout.EndHorizontal();
                }
            }
            else if (actSubTypeModel == ActiveSubType.Settings)
            {
                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.modelCheckScale = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckScale, "检查Scale属性", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.modelCheckMeshCompression = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckMeshCompression, "检查Mesh压缩", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.modelCheckAnimCompression = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckAnimCompression, "检查动画压缩", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.modelCheckMeshIsRW = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckMeshIsRW, "检查可读写", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                ProjectCheckSettingUI.modelCheckCollider = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckCollider, "检查是否生成碰撞器", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //ProjectCheckSettingUI.modelCheckNormals = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckNormals, "检查是否存在法线", GUILayout.MaxWidth(100));
                //GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //ProjectCheckSettingUI.modelCheckTangents = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckTangents, "检查是否存在切线", GUILayout.MaxWidth(100));
                //GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //ProjectCheckSettingUI.modelCheckFileSize = GUILayout.Toggle(ProjectCheckSettingUI.modelCheckFileSize, "检查文件大小", GUILayout.MaxWidth(100));
                //ProjectCheckSettingUI.modelCheckFileSizeValue = GUILayout.TextField(ProjectCheckSettingUI.modelCheckFileSizeValue, GUILayout.MaxWidth(80));
                //GUILayout.Label("kb", GUILayout.MaxWidth(20));
                //GUILayout.EndHorizontal();


              

                if (GUILayout.Button("应用", GUILayout.MaxWidth(100)))
                {
                    ProjectCheckSetting.Apply(true);
                    projectInfo.ReCheckModels();
                }
            }
            else
            {
                DrawHelpTips(LanguageCfg.HELP_MODEL);
            }

            EditorGUILayout.EndScrollView();
        }
        #endregion 


        #region 工具函数部分
        void PingAssetInProject(string file)
        {
            if (!file.StartsWith("Assets/"))
            {
                return;
            }
            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(file);
            if (asset != null)
            {
                GUI.skin = null;
                //EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(file, typeof(Object)));
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }
        #endregion 

    }

    //用于显示的
    public class ProjectCheckSettingUI
    {
        public static bool textureCheckMemSize = true;
        public static string textureCheckMemSizeValue = "512";
        public static bool textureCheckPix = true;
        public static string textureCheckPixW = "1024";
        public static string textureCheckPixH = "1024";
        public static bool textureCheckPix2Pow = true;
        public static bool textureCheckIsRW = true;
        public static bool textureCheckPlatSetting = true;
        public static bool textureCheckIsLightmap = true;


        public static bool modelCheckScale = true;
        public static bool modelCheckMeshCompression = true;
        public static bool modelCheckAnimCompression = true;
        public static bool modelCheckMeshIsRW = true;
        public static bool modelCheckCollider = true;
        //public static bool modelCheckNormals = true;
        //public static bool modelCheckTangents = true;
        //public static bool modelCheckBakeIK = true;
        //public static bool modelCheckFileSize = true;
        //public static string modelCheckFileSizeValue = 1024*1024 +""; 


        public static bool audioCheckCompression = true;
        public static bool audioCheckIs3D = true;
        public static bool audioCheckRate = true;
        public static string audioCheckRateValue = "128";


    }

    #region 用于检测的设置  
    public class ProjectCheckSetting
    {
        public static ProjectCheckSetting instance = new ProjectCheckSetting();

        public bool textureCheckMemSize = true;
        public int textureCheckMemSizeValue = 0;
        public bool textureCheckPix = true;
        public int textureCheckPixW = 0;
        public int textureCheckPixH = 0;
        public bool textureCheckPix2Pow = true;
        public bool textureCheckIsRW = true;
        public bool textureCheckPlatSetting = true;
        public bool textureCheckIsLightmap = true;


        public bool modelCheckScale = true;
        public bool modelCheckMeshCompression = true;
        public bool modelCheckAnimCompression = true;
        public bool modelCheckMeshIsRW = true;
        public bool modelCheckCollider = true;
        //public bool modelCheckNormals = true;
        //public bool modelCheckTangents = true;
        //public bool modelCheckBakeIK = true;
        //public bool modelCheckFileSize = true;
        //public int modelCheckFileSizeValue = 0; 


        public bool audioCheckCompression = true;
        public bool audioCheckIs3D = true;
        public bool audioCheckRate = true;
        public int audioCheckRateValue = 0;

        public static void Apply(bool hasTip )
        {
            try
            {
                
                instance.textureCheckMemSize = ProjectCheckSettingUI.textureCheckMemSize;
                instance.textureCheckMemSizeValue = int.Parse(ProjectCheckSettingUI.textureCheckMemSizeValue);
                instance.textureCheckPix = ProjectCheckSettingUI.textureCheckPix;
                instance.textureCheckPixW = int.Parse(ProjectCheckSettingUI.textureCheckPixW);
                instance.textureCheckPixH = int.Parse(ProjectCheckSettingUI.textureCheckPixH);
                instance.textureCheckPix2Pow = ProjectCheckSettingUI.textureCheckPix2Pow;
                instance.textureCheckIsRW = ProjectCheckSettingUI.textureCheckIsRW;
                instance.textureCheckPlatSetting = ProjectCheckSettingUI.textureCheckPlatSetting;
                instance.textureCheckIsLightmap = ProjectCheckSettingUI.textureCheckIsLightmap;


                instance.modelCheckScale = ProjectCheckSettingUI.modelCheckScale;
                instance.modelCheckMeshCompression = ProjectCheckSettingUI.modelCheckMeshCompression;
                instance.modelCheckAnimCompression = ProjectCheckSettingUI.modelCheckAnimCompression;
                instance.modelCheckMeshIsRW = ProjectCheckSettingUI.modelCheckMeshIsRW;
                instance.modelCheckCollider = ProjectCheckSettingUI.modelCheckCollider;
                //tmpSetting.modelCheckNormals = ProjectCheckSettingUI.modelCheckNormals;
                //tmpSetting.modelCheckTangents = ProjectCheckSettingUI.modelCheckTangents;
                //tmpSetting.modelCheckBakeIK = ProjectCheckSettingUI.modelCheckBakeIK;
                //tmpSetting.modelCheckFileSize = ProjectCheckSettingUI.modelCheckFileSize;
                //tmpSetting.modelCheckFileSizeValue = int.Parse(ProjectCheckSettingUI.modelCheckFileSizeValue);


                instance.audioCheckCompression = ProjectCheckSettingUI.audioCheckCompression;
                instance.audioCheckIs3D = ProjectCheckSettingUI.audioCheckIs3D;
                instance.audioCheckRate = ProjectCheckSettingUI.audioCheckRate;
                instance.audioCheckRateValue = int.Parse(ProjectCheckSettingUI.audioCheckRateValue);


                if (hasTip)
                {
                    EditorUtility.DisplayDialog("Tips", "设置成功，请返回详情查看", "OK");
                }
            }
            catch (Exception e)
            {
                if (hasTip)
                {
                    EditorUtility.DisplayDialog("Tips", "设置失败，请关掉窗口重新打开 " + e.Message, "OK");
                }
            }


        }
        
    }

    #endregion


    #region 资源基类信息
    public class RefInfo
    {
        public int instanceID;
        public string name;
        public string path;
        public long size;
        public long fileSize = -1;

        public int proposeTipCount = 0;
        public string proposeTips = "";

        public string GetSize()
        {
            return FormatBytes(size);
        }
        public string GetAbsolutePath()
        {
            string tmpPath = Application.dataPath;
            tmpPath = tmpPath.Replace('\\', '/');
            tmpPath = tmpPath.Substring(0, tmpPath.Length - 6);
            return tmpPath + path;
        }

        public String GetFileLenth()
        {
            if (fileSize == -1)
            {
                FileInfo file = new FileInfo(GetAbsolutePath());
                fileSize = file.Length;
            }
            return FormatBytes(fileSize);
        }

        protected void ResetProposeTip()
        {
            proposeTipCount = 0;
            proposeTips = "";
        }
        protected void AddProposeTip(string tip)
        {
            proposeTipCount++;
            string format = "{0}. {1} \n";
            proposeTips += string.Format(format, proposeTipCount, tip);
        }

        protected bool Powerof2(int n)
        {
            return ((n & (n - 1)) == 0);
        }


        public string FormatBytes(long size)
        {
            if (size < 1024)
            {
                return size + " byte";
            }
            else if (size < (1024 * 1024))
            {
                return ((float)size / 1024).ToString("0.0") + " KB";
            }
            else
            {
                return ((float)size / (1024 * 1024)).ToString("0.0") + " M";
            }
        }


          virtual public string GetRefInfos()
          {
              return "";
          }
          public string GetResInfoDetails()
          {
              return GetRefInfos() + proposeTips;
          }
    }


    #endregion



    #region 文字语言类
    public class LanguageCfg
    {
        public const string HOME = "总览";
        public const string TEXTURES = "纹理贴图";
        public const string MODELS = "模型";
        public const string MATERIALS = "材质球";
        public const string AUDIOS = "音效";
        public const string DETAILS = "详情";
        public const string HELPS = "帮助";
        public const string SETTINGS = "设置";
        public const string PROPOSE = " 建议";
        public const string HELP_TEXTURE = "1.Texture的大小尽量做成2的幂 \n\n" +
            "2.设置合理的maxSize\n\n" +
            "3.覆盖不同平台Texture的参数\n\n" +
            "4.Texture大小尽量小于1024 * 1024. 建议尽量使用最小尺寸的贴图\n\n" +
            "5.如果贴图的alpha通道是不用的建议使用RGB24位代替RGB32位\n\n";
        public const string NAME = "NAME";
        public const string MemorySize = "内存大小";
        public const string PIX_W = "宽";
        public const string PIX_H = "高";
        public const string IsRW = "可读写";
        public const string OverridePlat = "平台设置";
        public const string Mipmap = "MipMap";
        public const string IsLightmap = "Lightmap";
        public const string AnisoLevel = "AnisoLevel";


        public const string Scale = "Scale";
        public const string MeshCompress = "Mesh压缩";
        public const string AnimCompress = "Anim压缩";
        public const string Collider = "碰撞器";
        public const string NormalMode = "模型法线";
        public const string TangentMode = "模型切线";
        public const string BakeIK = "BakeIK";
        public const string FileSize = "文件大小";
        public const string SkinnedMeshCnt = "蒙皮数";
        public const string MeshFilterCnt = "MeshFilter数";
        public const string AnimCnt = "动画数";
        public const string VertexCnt = "顶点数";
        public const string TriangleCnt = "三角数";
        public const string BoneCnt = "骨骼数";


        public const string HELP_MODEL =
            "1.unity和其它三维建模软件间的单位差异-它将缩放整个模型，建议设置为1\n\n" +
            "2.网格和动画的压缩意味着占用更少的空间，但是会有损游戏的质量\n\n" +
            "3.注意碰撞体的碰撞层，不必要的碰撞检测一定要舍去\n\n" +
            "4.法线使用于灯光。如果你不在你的网格里使用实时照明，你也并不需要存储法线\n\n" +
            "5.切线使用于法线贴图，如果不使用法线贴图，也许并不需要在网格里存储这些切线\n\n" +
            "6.注意是否有多余的动画脚本，模型自动导入到U3D会有动画脚本，大量的话会严重影响消耗CPU计算\n\n" +
            "7.尽量减少每个动画使用的骨骼\n\n" +
            "8.最好把你人物的三角面数量控制在1500以下";

        public const string ShaderName = "ShaderName";


        public const string HELP_AUDIO =
          "1.压缩声音是通过从编辑器导入设置选择compressed选项，在音频数据将很小，但在播放时会消耗CPU周期来解码。最适用于中等长度音效与音乐\n\n" +
          "2.是否为3D的音效。一般背景的音效不会设置为3D\n\n" +
          "3.Compression kpbs一般128就可以了。128Kbps=磁带、手机立体声MP3播放器最佳设定值、低档MP3播放器最佳设定值\n\n";


        public const string AudioFormat = "压缩格式";
        public const string IsThreeD = "3D音效";
        public const string ForceToMono = "单声道";
        public const string CompressionBitrate = "采样频率";


    }


    #endregion


    #region 音效资源信息
    public class AudioInfo : RefInfo
    {
        public enum SortType
        {
            Name,
            AudioFormat,
            IsThreeD,
            ForceToMono,
            CompressionBitrate,
            Propose
        }


        public AudioImporterFormat audioFormat;
        public bool isThreeD;
        public bool forceToMono;
        public int compressionBitrate;
        public string info;

        public AudioInfo(UnityEngine.Object asset, string path)
        {
            this.name = asset.name;
            this.path = path;
            AudioImporter ai = AudioImporter.GetAtPath(path) as AudioImporter;
            if (ai != null)
            {
                //this.audioFormat = ai.format;
                this.isThreeD = ai.threeD;
                this.forceToMono = ai.forceToMono;
                //this.compressionBitrate = ai.compressionBitrate / 1000;
                AudioClip audioClip = asset as AudioClip;
                info = audioClip.length + "  " + audioClip.samples + "  " + audioClip.channels + "  " + audioClip.frequency;
            }
            CheckValid();
        }

        public override string GetRefInfos()
        {
            string info = "";
            info += LanguageCfg.AudioFormat + ":" + this.audioFormat.ToString() +"\n";
            info += LanguageCfg.IsThreeD + ":" + this.isThreeD.ToString() +"\n";
            info += LanguageCfg.ForceToMono + ":" + this.forceToMono.ToString() +"\n";
            info += LanguageCfg.CompressionBitrate + ":" + this.compressionBitrate.ToString() +"\n\n\n";
            return info;
        }

        public void CheckValid()
        {
            ResetProposeTip();
            ProjectCheckSetting setting = ProjectCheckSetting.instance;
            if (setting.audioCheckCompression)
            {
                if (this.audioFormat == AudioImporterFormat.Native)
                {
                    AddProposeTip("建议音频使用压缩格式");
                }
            }
            if (setting.audioCheckIs3D)
            {
                if (this.isThreeD)
                {
                    AddProposeTip("建议背景音效使用2D音效即可");
                }
            }
            if (setting.audioCheckRate)
            {
                if (this.compressionBitrate > setting.audioCheckRateValue || this.compressionBitrate < 0)
                {
                    AddProposeTip("建议Compression  kpbs使用" + setting.audioCheckRateValue);
                }
            }
        }
    }

    #endregion

    public class MaterialInfo : RefInfo
    {
        public enum SortType
        {
            Name,
            ShaderName,
            Propose
        }


        public string shaderName;
        public MaterialInfo(Material material, string path)
        {
            this.name = material.name;
            this.path = path;
            this.shaderName = material.shader.name;


            if (!this.shaderName.Contains("Mobile") && !this.shaderName.Contains("Unlit"))
            {
                AddProposeTip("建议移动设备上使用mobile或者unlit的shader。");
            }
        }
    }
    public class ModelInfo : RefInfo
    {
        public enum SortType
        {
            Name,
            Scale,
            MeshCompression,
            AnimCompression,
            IsRW,
            Collider,
            NormalImportMode,
            TangentImportMode,
            BakeIK,
            FileSize,
            Animation,
            SkinnedMeshCount,
            MeshFilterCount,
            AnimationClipCount,
            VertexCount,
            TriangleCount,
            BoneCount,
            Propose
        }

        public float scale;
        public ModelImporterMeshCompression meshCompression;
        public ModelImporterAnimationCompression animCompression;
        public bool isRW;
        public bool isAddCollider;
        public bool swapUVChannels;
        public ModelImporterTangentSpaceMode normalImportMode;
        public ModelImporterTangentSpaceMode tangentImportMode;
        public bool isBakeIK;        
        public string filePath;

        public int skinnedMeshCount;
        public int meshFilterCount;
        public int animationClipCount;
        public int vertexCount;
        public int triangleCount;

        public int boneCount;
        public ModelInfo(ModelImporter mi, string path)
        {
            //获取模型数据
            GameObject asset = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            this.path = path;
            this.name = asset.name;
            this.scale = mi.globalScale;
            this.meshCompression = mi.meshCompression;
            this.isRW = mi.isReadable;
            this.isAddCollider = mi.addCollider;
            this.swapUVChannels = mi.swapUVChannels;
            this.normalImportMode = mi.normalImportMode;
            this.tangentImportMode = mi.tangentImportMode;
            this.isBakeIK = mi.bakeIK;
            this.animCompression = mi.animationCompression ;            
            
            if (!mi.importAnimation || mi.animationType ==  ModelImporterAnimationType.None)
            {
                this.animationClipCount = 0;
            }
            else
            {
                this.animationClipCount = mi.clipAnimations.Length;
            }
              
            CollectMeshInfo(asset);

            CheckValid();
        }

        override
        public string GetRefInfos()
        {
            string info = "";
            info += LanguageCfg.Scale + ":" + this.scale.ToString() +"\n";
            info += LanguageCfg.MeshCompress + ":" + this.meshCompression.ToString() +"\n";
            info += LanguageCfg.AnimCompress + ":" + this.animCompression.ToString() + "\n";
            info += LanguageCfg.IsRW + ":" + this.isRW.ToString() + "\n";
            info += LanguageCfg.Collider + ":" + this.isAddCollider.ToString() + "\n";            
            info += LanguageCfg.NormalMode + ":" + this.normalImportMode.ToString() + "\n";
            info += LanguageCfg.TangentMode + ":" + this.tangentImportMode.ToString() + "\n";
            info += LanguageCfg.BakeIK + ":" + this.isBakeIK.ToString() + "\n";
            info += LanguageCfg.SkinnedMeshCnt + ":" + this.skinnedMeshCount.ToString() + "\n";
            info += LanguageCfg.AnimCnt + ":" + this.animationClipCount.ToString() + "\n";
            info += LanguageCfg.VertexCnt + ":" + this.vertexCount.ToString() + "\n";
            info += LanguageCfg.TriangleCnt + ":" + this.triangleCount.ToString() + "\n";
            info += LanguageCfg.BoneCnt + ":" + this.boneCount.ToString() + "\n\n\n";
            return info;

        }



        public void CheckValid()
        {
            ResetProposeTip();
            //检测模型
            ProjectCheckSetting setting = ProjectCheckSetting.instance ;
            if (setting.modelCheckScale)
            {
                if (this.scale != 1)
                {
                    AddProposeTip("建议模型的scale为1");
                }
            }
            if (setting.modelCheckMeshCompression)
            {
                if (this.meshCompression == ModelImporterMeshCompression.Off)
                {
                    AddProposeTip("建议模型采用压缩格式");
                }
            }

            if (setting.modelCheckAnimCompression)
            {
                if (this.animCompression ==  ModelImporterAnimationCompression.Off ||
                    this.animCompression == ModelImporterAnimationCompression.KeyframeReduction)
                {
                    if (this.animationClipCount > 0)
                    {
                        AddProposeTip("建议动画采用压缩KeyframeReductionAndCompression格式");
                    }
                }
            }


            if (setting.modelCheckMeshIsRW)
            {
                if (this.isRW)
                {
                    AddProposeTip("建议将非可读写的模型读写操作关掉");
                }
            }
            if (setting.modelCheckCollider)
            {
                if (this.isAddCollider)
                {
                    AddProposeTip("建议检查下当前模型确实需要导入Collider");
                }
            }
            //if (this.animationClipCount > 0)
            //{
            //    AddProposeTip("建议检查当前模型是否需要导入animation");
            //}
        }
        //private void CollectAnimationInfo(GameObject modelObject)
        //{
        //    Animation legacyAnimation = null;
        //    legacyAnimation = modelObject.GetComponent<Animation>();
        //    if (legacyAnimation == null)
        //    {
        //        return;
        //    }
        //    animationClipCount = legacyAnimation.GetClipCount();
        //}

        private void CollectMeshInfo(GameObject modelObject)
        {
            List<SkinnedMeshRenderer> _skinnedMeshes = FindAllSkinnedMesh(modelObject);
            List<MeshFilter> _meshFilters = FindAllMeshFilter(modelObject);

            foreach (SkinnedMeshRenderer _skin in _skinnedMeshes)
            {
                vertexCount += _skin.sharedMesh.vertexCount;
                triangleCount += _skin.sharedMesh.triangles.Length / 3;
                Transform[] bones = _skin.bones;
                boneCount += bones.Length;                
            }
            foreach (MeshFilter _filter in _meshFilters)
            {              
                vertexCount += _filter.sharedMesh.vertexCount;
                triangleCount += _filter.sharedMesh.triangles.Length / 3;                
            }
            skinnedMeshCount = _skinnedMeshes.Count;
            meshFilterCount = _meshFilters.Count;
        }


        public static List<SkinnedMeshRenderer> FindAllSkinnedMesh(GameObject modelObject)
        {
            List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();
            SkinnedMeshRenderer skin = null;

            skin = modelObject.GetComponent<SkinnedMeshRenderer>();
            if (skin != null)
            {
                skinnedMeshes.Add(skin);
            }
            for (int i = 0; i < modelObject.transform.childCount; i++)
            {
                skin = modelObject.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                if (skin != null)
                {
                    skinnedMeshes.Add(skin);
                }
            }
            return skinnedMeshes;
        }

        public static List<MeshFilter> FindAllMeshFilter(GameObject modelObject)
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();
            MeshFilter filter = null;

            filter = modelObject.GetComponent<MeshFilter>();
            if (filter != null)
            {
                meshFilters.Add(filter);
            }
            for (int i = 0; i < modelObject.transform.childCount; i++)
            {
                filter = modelObject.transform.GetChild(i).GetComponent<MeshFilter>();
                if (filter != null)
                {
                    meshFilters.Add(filter);
                }
            }
            return meshFilters;
        }

    }
    public class TextureInfo : RefInfo
    {
        public enum SortType
        {
            Name,
            MemorySize,
            PixWidth,
            PixHeigh,
            IsRW,
            Mipmap,
            IsLightmap,
            AnisoLevel,
            Propose
        }
        public class TexturePlatSetting
        {
            public const string PLAT_ANDROID = "Android";
            public const string PLAT_IPHONE = "iPhone";
            public const string PLAT_STANDALONE = "Standalone";

            public bool isSetting;
            public int maxSize;
            public TextureImporterFormat textureFormat;

            public string ToString()
            {
                return isSetting + " " + maxSize + " " + textureFormat.ToString();
            }
        }

        public Texture texture;
        public int width;
        public int height;
        public bool isRW;
        public bool isMipmap;
        public bool isLightmap;
        public int anisoLevel;

        public TexturePlatSetting androidPlatSetting = new TexturePlatSetting();
        public TexturePlatSetting iosPlatSetting = new TexturePlatSetting();
        public TexturePlatSetting standalonePlatSetting = new TexturePlatSetting();


        public TextureInfo(Texture texture, string path)
        {
            //获取问题贴图的数据
            this.name = texture.name;
            this.path = path;
            this.texture = texture;
            this.width = this.texture.width;
            this.height = this.texture.height;
            this.size = CalculateTextureSizeBytes(this.texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti != null)
            {
                this.isRW = ti.isReadable;

                androidPlatSetting.isSetting = ti.GetPlatformTextureSettings(TexturePlatSetting.PLAT_ANDROID,
                        out androidPlatSetting.maxSize, out androidPlatSetting.textureFormat);

                iosPlatSetting.isSetting = ti.GetPlatformTextureSettings(TexturePlatSetting.PLAT_IPHONE,
                        out iosPlatSetting.maxSize, out iosPlatSetting.textureFormat);

                standalonePlatSetting.isSetting = ti.GetPlatformTextureSettings(TexturePlatSetting.PLAT_STANDALONE,
                        out standalonePlatSetting.maxSize, out standalonePlatSetting.textureFormat);

                this.isMipmap = ti.mipmapEnabled;
                this.isLightmap = ti.lightmap;
                this.anisoLevel = ti.anisoLevel;
            }

            CheckValid();
            
        }
        public override string GetRefInfos()
        {
            string info = "";

            info += LanguageCfg.PIX_W + ":" + this.width.ToString() + "\n";
            info += LanguageCfg.PIX_H + ":" + this.width.ToString() + "\n";
            info += LanguageCfg.IsRW + ":" + this.isRW.ToString() + "\n";
            info += LanguageCfg.Mipmap + ":" + this.isMipmap.ToString() + "\n";
            info += LanguageCfg.IsLightmap + ":" + this.isLightmap.ToString() + "\n";
            info += LanguageCfg.AnisoLevel + ":" + this.anisoLevel.ToString() + "\n\n\n";
            return info;
        }
        //检验纹理贴图的数据合法性
        public void CheckValid()
        {
            ResetProposeTip();
            ProjectCheckSetting setting = ProjectCheckSetting.instance;
            if (setting.textureCheckMemSize)
            {

                if (this.size >= setting.textureCheckMemSizeValue * 1024)
                {
                    AddProposeTip("文件不小啊，大于" + setting.textureCheckMemSizeValue + "KB，还能减小不？？");
                }
            }

            if (setting.textureCheckPix)
            {
                if (this.width > setting.textureCheckPixW || this.height > setting.textureCheckPixH)
                {
                    AddProposeTip("建议纹理像素最大是" + setting.textureCheckPixW + "x" + setting.textureCheckPixH);
                }
            }
            if (setting.textureCheckPix2Pow)
            {
                if (!Powerof2(this.width) || !Powerof2(this.height))
                {
                    AddProposeTip("建议像素大小为2的n次幂(GUI纹理除外) " + this.width + "x" + this.height);
                }
            }

            if (setting.textureCheckPlatSetting)
            {
                if (!androidPlatSetting.isSetting || !iosPlatSetting.isSetting || !standalonePlatSetting.isSetting)
                {
                    AddProposeTip("建议对不同的平台设置不同的参数");
                }
            }
            if (setting.textureCheckIsRW)
            {
                if (this.isRW)
                {
                    AddProposeTip("建议非读写的贴图将这个读写开关关掉");
                }
            }
            if (setting.textureCheckIsLightmap)
            {
                if (this.isLightmap)
                {
                    AddProposeTip("建议检查当前纹理确实是lightmap需要的纹理否？");
                }
            }
        }


        public string IsOverridePlatform()
        {
            return standalonePlatSetting.isSetting
                + " " + androidPlatSetting.isSetting
                + " " + iosPlatSetting.isSetting;
        }

        int CalculateTextureSizeBytes(Texture tTexture)
        {

            int tWidth = tTexture.width;
            int tHeight = tTexture.height;
            if (tTexture is Texture2D)
            {
                Texture2D tTex2D = tTexture as Texture2D;
                int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
                int mipMapCount = tTex2D.mipmapCount;
                int mipLevel = 1;
                int tSize = 0;
                while (mipLevel <= mipMapCount)
                {
                    tSize += tWidth * tHeight * bitsPerPixel / 8;
                    tWidth = tWidth / 2;
                    tHeight = tHeight / 2;
                    mipLevel++;
                }
                return tSize;
            }

            if (tTexture is Cubemap)
            {
                Cubemap tCubemap = tTexture as Cubemap;
                int bitsPerPixel = GetBitsPerPixel(tCubemap.format);
                return tWidth * tHeight * 6 * bitsPerPixel / 8;
            }
            return 0;
        }

        int GetBitsPerPixel(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8: //	 Alpha-only texture format.
                    return 8;
                case TextureFormat.ARGB4444: //	 A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                    return 16;
                case TextureFormat.RGBA4444: //	 A 16 bits/pixel texture format.
                    return 16;
                case TextureFormat.RGB24:	// A color texture format.
                    return 24;
                case TextureFormat.RGBA32:	//Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.ARGB32:	//Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.RGB565:	//	 A 16 bit color texture format.
                    return 16;
                case TextureFormat.DXT1:	// Compressed color texture format.
                    return 4;
                case TextureFormat.DXT5:	// Compressed color with alpha channel texture format.
                    return 8;
                /*
                case TextureFormat.WiiI4:	// Wii texture format.
                case TextureFormat.WiiI8:	// Wii texture format. Intensity 8 bit.
                case TextureFormat.WiiIA4:	// Wii texture format. Intensity + Alpha 8 bit (4 + 4).
                case TextureFormat.WiiIA8:	// Wii texture format. Intensity + Alpha 16 bit (8 + 8).
                case TextureFormat.WiiRGB565:	// Wii texture format. RGB 16 bit (565).
                case TextureFormat.WiiRGB5A3:	// Wii texture format. RGBA 16 bit (4443).
                case TextureFormat.WiiRGBA8:	// Wii texture format. RGBA 32 bit (8888).
                case TextureFormat.WiiCMPR:	//	 Compressed Wii texture format. 4 bits/texel, ~RGB8A1 (Outline alpha is not currently supported).
                    return 0;  //Not supported yet
                */
                case TextureFormat.PVRTC_RGB2://	 PowerVR (iOS) 2 bits/pixel compressed color texture format.
                    return 2;
                case TextureFormat.PVRTC_RGBA2://	 PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
                    return 2;
                case TextureFormat.PVRTC_RGB4://	 PowerVR (iOS) 4 bits/pixel compressed color texture format.
                    return 4;
                case TextureFormat.PVRTC_RGBA4://	 PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
                    return 4;
                case TextureFormat.ETC_RGB4://	 ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                    return 4;
                //case TextureFormat.ETC_RGB4://	 ATC (ATITC) 4 bits/pixel compressed RGB texture format.
                //    return 4;
                case TextureFormat.ETC2_RGBA8://	 ATC (ATITC) 8 bits/pixel compressed RGB texture format.
                    return 8;
                case TextureFormat.BGRA32://	 Format returned by iPhone camera
                    return 32;
                //case TextureFormat.ATF_RGB_DXT1://	 Flash-specific RGB DXT1 compressed color texture format.
                //case TextureFormat.ATF_RGBA_JPG://	 Flash-specific RGBA JPG-compressed color texture format.
                //case TextureFormat.ATF_RGB_JPG://	 Flash-specific RGB JPG-compressed color texture format.
                //    return 0; //Not supported yet
            }
            return 0;
        }

    }


    class ProjectInfo
    {
        public List<AnimationClip> animClips = new List<AnimationClip>();
        public List<AudioInfo> audios = new List<AudioInfo>();
        public List<MaterialInfo> materials = new List<MaterialInfo>();
        public List<Mesh> meshes = new List<Mesh>();
        public List<ModelInfo> models = new List<ModelInfo>();
        public List<TextureInfo> textures = new List<TextureInfo>();


        public void SortModel(ModelInfo.SortType sortType)
        {
            switch (sortType)
            {
                case ModelInfo.SortType.Name:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo1.name.CompareTo(tInfo2.name); });
                    break;
                case ModelInfo.SortType.Scale:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.scale.CompareTo(tInfo1.scale); });
                    break;
                case ModelInfo.SortType.MeshCompression:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.meshCompression.CompareTo(tInfo1.meshCompression); });
                    break;
                case ModelInfo.SortType.AnimCompression:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.animCompression.CompareTo(tInfo1.animCompression); });
                    break;
                case ModelInfo.SortType.IsRW:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.isRW.CompareTo(tInfo1.isRW); });
                    break;
                case ModelInfo.SortType.Collider:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.isAddCollider.CompareTo(tInfo1.isAddCollider); });
                    break;
                case ModelInfo.SortType.NormalImportMode:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.normalImportMode.CompareTo(tInfo1.normalImportMode); });
                    break;
                case ModelInfo.SortType.TangentImportMode:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.tangentImportMode.CompareTo(tInfo1.tangentImportMode); });
                    break;
                case ModelInfo.SortType.BakeIK:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.isBakeIK.CompareTo(tInfo1.isBakeIK); });
                    break;
                case ModelInfo.SortType.FileSize:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.fileSize.CompareTo(tInfo1.fileSize); });
                    break;
                //case ModelInfo.SortType.Animation:
                //    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.isGenerateAnimation.CompareTo(tInfo1.isGenerateAnimation); });
                //    break;
                case ModelInfo.SortType.SkinnedMeshCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.skinnedMeshCount.CompareTo(tInfo1.skinnedMeshCount); });
                    break;
                case ModelInfo.SortType.MeshFilterCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.meshFilterCount.CompareTo(tInfo1.meshFilterCount); });
                    break;
                case ModelInfo.SortType.AnimationClipCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.animationClipCount.CompareTo(tInfo1.animationClipCount); });
                    break;
                case ModelInfo.SortType.VertexCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.vertexCount.CompareTo(tInfo1.vertexCount); });
                    break;
                case ModelInfo.SortType.TriangleCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.triangleCount.CompareTo(tInfo1.triangleCount); });
                    break;
                case ModelInfo.SortType.BoneCount:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.boneCount.CompareTo(tInfo1.boneCount); });
                    break;
                case ModelInfo.SortType.Propose:
                    models.Sort(delegate(ModelInfo tInfo1, ModelInfo tInfo2) { return tInfo2.proposeTipCount.CompareTo(tInfo1.proposeTipCount); });
                    break;

            }
        }
        public void SortTexture(TextureInfo.SortType sortType)
        {
            switch (sortType)
            {
                case TextureInfo.SortType.Name:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo1.name.CompareTo(tInfo2.name); });
                    break;
                case TextureInfo.SortType.MemorySize:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.size.CompareTo(tInfo1.size); });
                    break;
                case TextureInfo.SortType.PixWidth:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.width.CompareTo(tInfo1.width); });
                    break;
                case TextureInfo.SortType.PixHeigh:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.height.CompareTo(tInfo1.height); });
                    break;
                case TextureInfo.SortType.IsRW:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.isRW.CompareTo(tInfo1.isRW); });
                    break;
                case TextureInfo.SortType.Mipmap:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.isMipmap.CompareTo(tInfo1.isMipmap); });
                    break;
                case TextureInfo.SortType.IsLightmap:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.isLightmap.CompareTo(tInfo1.isLightmap); });
                    break;
                case TextureInfo.SortType.AnisoLevel:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.anisoLevel.CompareTo(tInfo1.anisoLevel); });
                    break;
                case TextureInfo.SortType.Propose:
                    textures.Sort(delegate(TextureInfo tInfo1, TextureInfo tInfo2) { return tInfo2.proposeTipCount.CompareTo(tInfo1.proposeTipCount); });
                    break;
            }
        }
        public void SortMaterial(MaterialInfo.SortType sortType)
        {
            switch (sortType)
            {
                case MaterialInfo.SortType.Name:
                    materials.Sort(delegate(MaterialInfo tInfo1, MaterialInfo tInfo2) { return tInfo1.name.CompareTo(tInfo2.name); });
                    break;
                case MaterialInfo.SortType.ShaderName:
                    materials.Sort(delegate(MaterialInfo tInfo1, MaterialInfo tInfo2) { return tInfo1.shaderName.CompareTo(tInfo2.shaderName); });
                    break;
                case MaterialInfo.SortType.Propose:
                    materials.Sort(delegate(MaterialInfo tInfo1, MaterialInfo tInfo2) { return tInfo2.proposeTipCount.CompareTo(tInfo1.proposeTipCount); });
                    break;
            }
        }
        public void SortAudio(AudioInfo.SortType sortType)
        {
            switch (sortType)
            {
                case AudioInfo.SortType.Name:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo1.name.CompareTo(tInfo2.name); });
                    break;
                case AudioInfo.SortType.AudioFormat:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo2.audioFormat.CompareTo(tInfo1.audioFormat); });
                    break;
                case AudioInfo.SortType.IsThreeD:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo2.isThreeD.CompareTo(tInfo1.isThreeD); });
                    break;
                case AudioInfo.SortType.ForceToMono:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo2.forceToMono.CompareTo(tInfo1.forceToMono); });
                    break;
                case AudioInfo.SortType.CompressionBitrate:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo2.compressionBitrate.CompareTo(tInfo1.compressionBitrate); });
                    break;
                case AudioInfo.SortType.Propose:
                    audios.Sort(delegate(AudioInfo tInfo1, AudioInfo tInfo2) { return tInfo2.proposeTipCount.CompareTo(tInfo1.proposeTipCount); });
                    break;
            }
        }

        public void Init()
        {
            reset();
            CreateProejectInfo();
            SortTexture(TextureInfo.SortType.MemorySize);

        }

        public void ReCheckAudios()
        {
            if (audios == null)
            {
                return;
            }
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].CheckValid();
            }
        }


        public void ReCheckModels()
        {
            if (models == null)
            {
                return;
            }
            for (int i = 0; i < models.Count; i++)
            {
                models[i].CheckValid();
            }
        }
        public void ReCheckTextures()
        {
            if(textures == null)
            {
                return;
            }
            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].CheckValid();
            }
        }
        public string ToString()
        {
            string log = "textures size {0}";
            return string.Format(log, textures.Count);
        }
        
        public void CreateProejectInfo()
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();

            for (int i = 0; i < paths.Length; i++)
            {
                UpdateProgress(i, paths.Length);

                string path = paths[i];
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset is Material)
                {
                    materials.Add(new MaterialInfo(asset as Material, path));
                    continue;
                }
                else if (asset is Texture)
                {
                    textures.Add(new TextureInfo(asset as Texture, path));
                    continue;
                }
                else if (asset is AudioClip)
                {
                    audios.Add(new AudioInfo(asset, path));
                    continue;
                }
                ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
                if (mi != null)
                {
                    models.Add(new ModelInfo(mi, path));                    
                    continue;
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void UpdateProgress(int cur, int total)
        {
            if (cur % 10 == 0)
            {
                float progress = (float)cur / (float)total;
                string format = "读取资源：{0}%";
                EditorUtility.DisplayProgressBar("Progress",string.Format(format,(int)(progress * 100)), progress);
            }
        }


        public void reset()
        {
            animClips.Clear();
            audios.Clear();
            materials.Clear();
            meshes.Clear();
            textures.Clear();
            models.Clear();
        }
    }

     
}
