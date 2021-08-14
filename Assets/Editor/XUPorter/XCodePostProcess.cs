using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
		if (target != BuildTarget.iOS) {
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

    //得到xcode工程的路径
    string path = Path.GetFullPath(pathToBuiltProject);

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO implement generic settings as a module option
		//project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");

    // 编辑plist 文件
    EditorPlist(path);

    // 编辑OC代码
    // NOTE:按各SDK的需要添加函数，by lixiaojiang
    EditorCode(path);

		// Finally save the xcode project
		project.Save();
	}

  private static void EditorPlist(string filePath)
  {
    XCPlist list = new XCPlist(filePath);    
    string PlistAdd = @"  
            <key>CFBundleURLTypes</key>
            <array>
              <dict>
                <key>CFBundleURLSchemes</key>
                  <array>
                    <string>cy1407921103977</string>
                  </array>
                </dict>            
            </array>";
    //在plist里面增加一行
    list.AddKey(PlistAdd);
    //在plist里面替换一行
    //list.ReplaceKey("<string>com.yusong.${PRODUCT_NAME}</string>", "<string>" + bundle + "</string>");
    //保存
    list.Save();
  }

  private static void EditorCode(string filePath)
  {
    //读取UnityAppController.mm文件
    XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");
    //添加代码解决键盘语音输入问题
    UnityAppController.WriteAbove("if(_didResignActive)", "\tEAGLContextSetCurrentAutoRestore autoRestore(_mainDisplay->surface.context);\n\t\t");
    //添加MBI初始化代码
    UnityAppController.WriteBelow("#include \"Unity/GlesHelper.h\"", "#import \"Cylib.h\"");    
    string appKey = "1407921103977";
    string channelID = "1010802002";
    string sentence = string.Format("[Cylib onStart:@\"{0}\" withChannelID:@\"{1}\"];", appKey, channelID);
    UnityAppController.WriteAbove("return NO;", sentence);
  }
#endif

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
