using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSMController : FSM {
    private Dictionary<int, int> treeAppleMap = new Dictionary<int, int>();
    private List<GameObject> treeList = new List<GameObject>();
    private int appleBackpack;
    public int numberApplesOnTrees = 3;
    public void Start() {
        _anim = GetComponent<Animator>();
        FSMNode._anim = _anim;
        FSMNode.controller = this;
        currentNode = gameObject.AddComponent(typeof(Walking)) as Walking;
        ((Walking) currentNode).target = GameObject.Find("Trees").transform.GetChild(0).gameObject;
        initTrees();
    }

    public override void Update() {
        if(currentNode)
            currentNode = currentNode.doActivity();
    }

    public void initTrees(){
        GameObject trees = GameObject.Find("Trees");
        foreach(Transform child in trees.transform){
            treeAppleMap.Add(child.gameObject.GetInstanceID(), numberApplesOnTrees);
            treeList.Add(child.gameObject);
        }
       // Start with 0 apples
       appleBackpack = 0;
    }

    // Return number of apples added to backpack
    public int addApple(int applesToAdd){
        int beforeApplesAdded = appleBackpack;
        appleBackpack += applesToAdd;
        if(appleBackpack > 2)
            appleBackpack = 2;

        return appleBackpack - beforeApplesAdded;
    }

    public Utils.pickingStates pickTree(GameObject target){
        int key = target.GetInstanceID();
        int numAdded = 0;

        int numApples = treeAppleMap[key];
        numAdded = addApple(numApples);
        treeAppleMap[key] -= numAdded;

        Debug.Log("Tree: "+ key + ", Backpack: " + appleBackpack + ", leftOnTree: " + treeAppleMap[key]);

        //if tree empty, remove from treeMap & treeList
        if(treeAppleMap[key] == 0){
            treeAppleMap.Remove(key);
            treeList.Remove(target);               
        }

        //if backpack full or trees empty, return to village
        if(appleBackpack == 2 || treeList.Count == 0)
            return Utils.pickingStates.village;
        //if backpack not full, target random tree
        return Utils.pickingStates.newTree;
    }

    public Utils.pickingStates dropApples(){
        appleBackpack = 0;
        return Utils.pickingStates.newTree;
    }

    public GameObject selectRandomTree(){
        if(treeList.Count > 0){
            int index = Random.Range(0, treeList.Count - 1);
            return treeList[index];
        } else{
            return null;
        }
    }

    public GameObject selectVillage(){
        return GameObject.Find("Viking_Village");
    }
}