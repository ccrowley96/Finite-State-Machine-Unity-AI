using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AppleCounter : MonoBehaviour {
    public Dictionary<int, int> treeAppleMap = VillagerFSM.treeAppleMap;
    public List<GameObject> treeList = VillagerFSM.treeList;

    void Update()
    {
        foreach (GameObject tree in treeList)
        {
            transform.Find(tree.name).GetComponent<TextMesh>().text = treeAppleMap[tree.GetInstanceID()].ToString();
        }
    }
}