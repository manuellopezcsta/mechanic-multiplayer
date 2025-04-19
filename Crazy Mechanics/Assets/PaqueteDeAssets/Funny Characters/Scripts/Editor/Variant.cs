using UnityEngine;

namespace CharacterCustomization
{
    public class Variant
    {
        public readonly GameObject PreviewObject;
        public readonly Mesh Mesh;

        public string Name => Mesh.name;

        public Variant(Mesh mesh, GameObject previewObject)
        {
            Mesh = mesh;
            PreviewObject = previewObject;
        }
    }
}