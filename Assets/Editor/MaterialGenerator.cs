using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class MaterialGenerator : MonoBehaviour
{
    [MenuItem("Alati/Generiši Materijale za Karte")]
    static void GenerisiMaterijale()
    {
        string folderSlika = "Assets/CardTextures";
        string folderMaterijala = "Assets/CardMaterials";

        if (!AssetDatabase.IsValidFolder(folderSlika) || !AssetDatabase.IsValidFolder(folderMaterijala))
        {
            Debug.LogError("Greška: Nisi napravio foldere! Moraju postojati 'CardTextures' i 'CardMaterials' direktno u Assets folderu.");
            return;
        }

        string[] sveSlike = Directory.GetFiles(folderSlika, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) ||
                        s.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        if (sveSlike.Length == 0)
        {
            Debug.LogWarning("Skripta nije našla slike! Proveri da li su slike u 'Assets/CardTextures' i da li su PNG/JPG format.");
            return;
        }

        int uspesnoGenerisano = 0;

        foreach (string putanja in sveSlike)
        {
            string unityPutanja = putanja.Replace("\\", "/");
            string imeFajla = Path.GetFileNameWithoutExtension(unityPutanja);

            Texture2D tekstura = AssetDatabase.LoadAssetAtPath<Texture2D>(unityPutanja);

            if (tekstura != null)
            {
                Material noviMaterijal = new Material(Shader.Find("Unlit/Texture"));
                noviMaterijal.mainTexture = tekstura;

                string putanjaMaterijala = folderMaterijala + "/" + imeFajla + ".mat";
                AssetDatabase.CreateAsset(noviMaterijal, putanjaMaterijala);
                uspesnoGenerisano++;
            }
            else
            {
                Debug.LogError("Unity nije uspeo da učita sliku na putanji: " + unityPutanja);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Uspješno generisano " + uspesnoGenerisano + " materijala!");
    }
}