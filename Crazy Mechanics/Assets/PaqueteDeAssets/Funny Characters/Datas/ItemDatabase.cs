using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    public Mesh mMesh;
    public GameObject mPrefab;
    public Sprite mSprite;
}


[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/ItemDatabase", order = 1)]
public class ItemDatabase : ScriptableObject
{
    public List<Data> mDatas = new List<Data>();   
}
