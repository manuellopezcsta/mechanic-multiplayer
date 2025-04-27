using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CarListSO : ScriptableObject
{
    [System.Serializable]
    public class CarsPerLevelList
    {
        public int level;
        public List<GameObject> cars;
    }

    public List<CarsPerLevelList> levelList;
    // Por si no se encuentra el nivel que use ese.
    public List<GameObject> allTheCars;
}
