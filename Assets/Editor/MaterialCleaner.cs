using UnityEngine;
using UnityEditor;

public class RemoveEmptyMaterialsForScene
{
    [MenuItem("Tools/Cleanup/Remove Empty Material Slots From Scene")]
    private static void RemoveEmptyMaterials()
    {
        // Get all renderers in the scene
        Renderer[] renderers = Object.FindObjectsOfType<Renderer>();

        int totalRemoved = 0;

        foreach (var r in renderers)
        {
            var mats = r.sharedMaterials;
            int originalCount = mats.Length;

            // Keep only non-null materials
            mats = System.Array.FindAll(mats, m => m != null);

            if (mats.Length != originalCount)
            {
                r.sharedMaterials = mats;
                EditorUtility.SetDirty(r);
                totalRemoved += originalCount - mats.Length;
            }
        }

        Debug.Log($"Removed {totalRemoved} empty material slots from the scene!");
    }
}
