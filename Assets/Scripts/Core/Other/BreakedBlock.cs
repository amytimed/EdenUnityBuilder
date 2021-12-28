using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BreakedBlock : MonoBehaviour
{
    private ParticleSystem _particle;
    public Block Block;
    void Start()
    {
        _particle = gameObject.GetComponent<ParticleSystem>();
        if (Block != null)
        {
            _particle.GetComponent<ParticleSystemRenderer>().mesh = InitMesh();
        }
    }

    private Mesh InitMesh()
    {
        ParticleSystem.MainModule psmain = _particle.main;
        psmain.startColor = new ParticleSystem.MinMaxGradient(Block.GetColor());
        Mesh _mesh = new Mesh();

        _mesh.Clear();

        _mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0) };

        List<Vector2> uv = new List<Vector2>();
        BlockSet.BlockSettings blocksettings;

        if (BlockSet.Blocks.TryGetValue(Block.BlockType, out blocksettings))
        {
            uv.AddRange(GetUVs((int)blocksettings.TexLeft));

            _mesh.uv = uv.ToArray();
        }
        // _mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) }; //Old

        _mesh.triangles = new int[] { 0, 1, 2 };

        return _mesh;
    }

    void Update()
    {

    }

    private Vector2[] GetUVs(float id)
    {
        List<Vector2> uv = new List<Vector2>();

        float tUnit = World.Instance.TerrainMaterialSettings.Tiling;
        uv.Add(new Vector2(tUnit * 1 + tUnit, tUnit * id));
        uv.Add(new Vector2(tUnit * 1 + tUnit, tUnit * id + tUnit));
        uv.Add(new Vector2(tUnit * 1, tUnit * id + tUnit));
        // uv.Add(new Vector2(tUnit * 1, tUnit * id));

        return uv.ToArray();
    }
}
