using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;


public class psd2png : MonoBehaviour {

	[MenuItem("Assets/UI/PSD to PNG")]
	private static void Psd2Png(){
		string targetPath;
		
		foreach (Object target in Selection.objects) {
			targetPath = AssetDatabase.GetAssetPath (target.GetInstanceID ());
			if (Path.GetExtension (targetPath) == ".psd") {
				TextureImporter textureImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
				AtlasTextureSetting.EditFormat(textureImporter);
				AssetDatabase.ImportAsset(targetPath);
				TransFormat ((Texture2D)target);
				textureImporter.isReadable = false;
				AssetDatabase.ImportAsset(targetPath);
			}
		}
		AssetDatabase.Refresh();
	}

	private static void TransFormat(Texture2D targetTexture)
	{
		if (targetTexture == null)
			return;
		
		int w = targetTexture.width;
		int h = targetTexture.height;
		Texture2D newTex = new Texture2D (w, h, TextureFormat.ARGB32, false);
		Color[] pixels = targetTexture.GetPixels ();
		int x, y;
		float r, g, b, a;
		for (int i = 0; i < pixels.Length; i++) {
			x = i % w;
			y = i / w;
			
			a = pixels[i].a;
			r = pixels[i].r;
			g = pixels[i].g;
			b = pixels[i].b;
			r *= a;
			g *= a;
			b *= a;

			Color color = new Color (r, g, b, a);
			newTex.SetPixel (x, y, color);
		}
		WriteTexture (targetTexture, newTex);
		DestroyImmediate(newTex);
	}
	
	//Texture2D Asset Overwrite
	private static void WriteTexture(Texture2D srcTex, Texture2D dstTex){
		byte[] texPNG = dstTex.EncodeToPNG();
		if (texPNG != null) {
			string filePath = AssetDatabase.GetAssetPath(srcTex.GetInstanceID());
			string dirName  = Path.GetDirectoryName(filePath);
			string fileName = Path.GetFileNameWithoutExtension(filePath) + ".png";
			string newPath  = Path.Combine(dirName, fileName);
			File.WriteAllBytes(newPath, texPNG);
			
			AssetDatabase.ImportAsset(filePath);
			AssetDatabase.Refresh();
		}
	}
}
