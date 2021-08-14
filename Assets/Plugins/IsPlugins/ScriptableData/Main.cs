using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScriptableData
{
    class MainClass
    {
        [STAThread]
		public static void Main(string[] args)
		{
			Dictionary<string, string> encodeTable = new Dictionary<string, string>();
			Dictionary<string, string> decodeTable = new Dictionary<string, string>();
			MainClass.AddCrypto("skill", "@1", encodeTable, decodeTable);
			MainClass.AddCrypto("section", "@2", encodeTable, decodeTable);
			MainClass.AddCrypto("effect", "@3", encodeTable, decodeTable);
			MainClass.AddCrypto("sound", "@4", encodeTable, decodeTable);
			MainClass.AddCrypto("triger", "@5", encodeTable, decodeTable);
			MainClass.AddCrypto("duration", "@6", encodeTable, decodeTable);
			MainClass.AddCrypto("grap", "@7", encodeTable, decodeTable);
			MainClass.AddCrypto("range", "@8", encodeTable, decodeTable);
			MainClass.AddCrypto("animation", "@9", encodeTable, decodeTable);
			MainClass.AddCrypto("impact", "@10", encodeTable, decodeTable);
			MainClass.AddCrypto("hittarget", "@11", encodeTable, decodeTable);
			MainClass.AddCrypto("throw", "@12", encodeTable, decodeTable);
			ScriptableDataFile scriptableDataFile = new ScriptableDataFile();
			scriptableDataFile.Load("test.txt");
			scriptableDataFile.Save("copy.txt");
			string contents = scriptableDataFile.GenerateObfuscatedCode(File.ReadAllText("test.txt"), encodeTable);
			File.WriteAllText("obfuscation.txt", contents);
			scriptableDataFile.ScriptableDatas.Clear();
			string code = File.ReadAllText("obfuscation.txt");
			scriptableDataFile.LoadObfuscatedCode(code, decodeTable);
			scriptableDataFile.Save("unobfuscation.txt");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021AC File Offset: 0x000003AC
		private static void AddCrypto(string s, string d, Dictionary<string, string> encodeTable, Dictionary<string, string> decodeTable)
		{
			encodeTable.Add(s, d);
			decodeTable.Add(d, s);
		}
	}
}
