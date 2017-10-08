using UnityEngine;
using System.Collections.Generic;

public class Splatter : MonoBehaviour {

    public static Splatter instance;


    public LayerMask splatLayers;
    public Transform[] parentsToSplat;
    public Material splatMaterial;
    public int splatmapSize = 1024;

    Texture2D[] _splatmaps;
    Color32[][] _splatmapContents;
    bool[] _modifiedFlags;

    static readonly Color32[] _palette = new Color32[] {
        new Color32(0, 0, 0, 0),
        new Color32(1, 1, 1, 1),
        new Color32(2, 2, 2, 2),
        new Color32(3, 3, 3, 3),
        new Color32(4, 4, 4, 4),
        new Color32(5, 5, 5, 5),
        new Color32(6, 6, 6, 6)
    };
    static readonly Vector3[] _sprayDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back};

    int _colorIndex = 0;


    // Performs an ink bomb at the given position.
    public void Splat(Vector3 position, Vector3 sprayDirection = default(Vector3))
    {
        Color32 color = _palette[_colorIndex];

        // Randomize the splatter directions a little.
        Quaternion tilt;
        if (sprayDirection == default(Vector3))
            tilt = Random.rotation;
        else
            tilt = Quaternion.LookRotation(sprayDirection, Random.onUnitSphere);

        foreach(var direction in _sprayDirections)
        {
            RaycastHit hit;
            if(Physics.Raycast(position, tilt * direction, out hit, 2.0f, splatLayers))
            {                
                var renderer = hit.collider.GetComponent<MeshRenderer>();
                
                if(renderer == null)
                {
                    Debug.LogWarning("Object in the splattable layers has no MeshRenderer: " + hit.collider.gameObject.name);
                    continue;
                }
                
                // Find corresponding spot in the splatmap.
                int mapID = renderer.lightmapIndex;
                if (mapID < 0)
                    continue;

                int uvX = Mathf.RoundToInt(hit.lightmapCoord.x * (splatmapSize - 1));
                int uvY = Mathf.RoundToInt(hit.lightmapCoord.y * (splatmapSize - 1));

                // Scatter ink blobs in a randomish pattern around the hit point.
                // You could also render a set of ink particles into the splatmap for a more art directed look.
                for(int i = 0; i < 20; i++)
                {
                    int x = Mathf.Clamp(uvX + Random.Range(-1, 2) + Random.Range(-1, 2) + Random.Range(-1, 2), 0, splatmapSize - 1);
                    int y = Mathf.Clamp(uvY + Random.Range(-1, 2) + Random.Range(-1, 2) + Random.Range(-1, 2), 0, splatmapSize - 1);
                    _splatmapContents[mapID][x + y * splatmapSize] = color;
                }

                // Mark the texture as updated so we know to upload the changes to the GPU.
                _modifiedFlags[mapID] = true;                
            }
        }
    }

	
	void Start () {
        instance = this;
        SetupReceiverMaterials();
        _colorIndex = Random.Range(1, _palette.Length);
	}

    void LateUpdate()
    {
        if (Input.GetKeyDown("0")) _colorIndex = 0;
        if (Input.GetKeyDown("1")) _colorIndex = 1;
        if (Input.GetKeyDown("2")) _colorIndex = 2;
        if (Input.GetKeyDown("3")) _colorIndex = 3;
        if (Input.GetKeyDown("4")) _colorIndex = 4;
        if (Input.GetKeyDown("5")) _colorIndex = 5;
        if (Input.GetKeyDown("6")) _colorIndex = 6;

        for (int i = 0; i < _splatmaps.Length; i++)
        {
            if(_modifiedFlags[i])
            {
                _splatmaps[i].SetPixels32(_splatmapContents[i]);
                _splatmaps[i].Apply(false);

                _modifiedFlags[i] = false;
            }
        }
    }

    struct CacheEntry { public Material material; public int mapIndex; }
    Dictionary<CacheEntry, Material> _materialCache = new Dictionary<CacheEntry, Material>();
    // Walk through a collection of renderers and reconfigure them to use our splatted material.
    void SetupReceiverMaterials()
    {
        var splatmaps = new List<Texture2D>();
        
        int renderersModified = 0;

        foreach (var parent in parentsToSplat)
        {
            int textureID = Shader.PropertyToID("_Splatmap");

            foreach (var renderer in parent.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.lightmapIndex < 0)
                    continue;

                while (renderer.lightmapIndex >= splatmaps.Count)
                {
                    var texture = new Texture2D(splatmapSize, splatmapSize, TextureFormat.Alpha8, false, true);
                    texture.filterMode = FilterMode.Point;
                    splatmaps.Add(texture);
                }
                
                var materials = renderer.sharedMaterials;
                for(int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                        continue;

                    var lookup = new CacheEntry { material = materials[i], mapIndex = renderer.lightmapIndex };
                    Material result;
                    if (!_materialCache.TryGetValue(lookup, out result))
                    {
                        result = Instantiate<Material>(splatMaterial);
                        result.color = materials[i].color;
                        result.SetTexture(textureID, splatmaps[renderer.lightmapIndex]);
                        // TODO: Copy references to smoothness/metalness parameters, diffuse & normal texture, etc.                   
                        
                        _materialCache.Add(lookup, result);
                    }
                    materials[i] = result;
                }
                
                renderer.sharedMaterials = materials;
                renderersModified++;
            }
        }

        Debug.Log("Created " + _materialCache.Count + " material instances using " + splatmaps.Count + " splatmaps across " + renderersModified + " renderers.");

        _splatmaps = splatmaps.ToArray();
        _splatmapContents = new Color32[splatmaps.Count][];
        _modifiedFlags = new bool[splatmaps.Count];
        
        for (int i = 0; i < splatmaps.Count; i++)
        {
            var contents = new Color32[splatmapSize * splatmapSize];
            _splatmapContents[i] = contents;
            _splatmaps[i].SetPixels32(contents);
            _splatmaps[i].Apply(false);            
        }       
    }

    // Clean up created material instances.
    void OnDestroy()
    {
        foreach (var material in _materialCache.Values)
            Destroy(material);

        foreach (var map in _splatmaps)
            Destroy(map);
    }
}
