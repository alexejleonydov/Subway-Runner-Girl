using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class MeshToObjExporter
{
    [MenuItem("Assets/Export Mesh to OBJ")]
    static void Export()
    {
        Mesh mesh = Selection.activeObject as Mesh;
        if (mesh == null)
        {
            Debug.LogError("Вибери Mesh у Project!");
            return;
        }

        StringBuilder sb = new StringBuilder();

        foreach (Vector3 v in mesh.vertices)
            sb.AppendFormat("v {0} {1} {2}\n", v.x, v.y, v.z);

        foreach (Vector3 n in mesh.normals)
            sb.AppendFormat("vn {0} {1} {2}\n", n.x, n.y, n.z);

        foreach (Vector2 uv in mesh.uv)
            sb.AppendFormat("vt {0} {1}\n", uv.x, uv.y);

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetIndices(i);
            for (int t = 0; t < tris.Length; t += 3)
            {
                sb.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    tris[t] + 1, tris[t + 1] + 1, tris[t + 2] + 1);
            }
        }

        string path = EditorUtility.SaveFilePanel("Export OBJ", "", mesh.name + ".obj", "obj");
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
    }
}
