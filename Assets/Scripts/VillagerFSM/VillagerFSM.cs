using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VillagerFSM : FSM {
    public static Dictionary<int, int> treeAppleMap = new Dictionary<int, int>();
    public static List<GameObject> treeList = new List<GameObject>();
    private List<GameObject> activeTreeList;
    public int appleBackpack;
    public static int numberApplesOnTrees = 3;

    public void Start() {
        _anim = GetComponent<Animator>();

        currentNode = gameObject.AddComponent(typeof(Walking)) as Walking;
        currentNode.controller = this;
        initTrees();
        ((Walking) currentNode).target = selectRandomTree();
    }
    
    public void initTrees(){
        GameObject trees = GameObject.Find("Trees");
        foreach(Transform child in trees.transform){
            if(!treeAppleMap.ContainsKey(child.gameObject.GetInstanceID())){
                treeAppleMap.Add(child.gameObject.GetInstanceID(), numberApplesOnTrees);
                treeList.Add(child.gameObject);
            }
        }

       activeTreeList = new List<GameObject>(treeList);
       // Start with 0 apples
       appleBackpack = 0;
    }

    // Return number of apples added to backpack
    public int addApple(int numApples){
        int beforeApplesAdded = appleBackpack;
        appleBackpack += numApples;
        if(appleBackpack > 2)
            appleBackpack = 2;

        return appleBackpack - beforeApplesAdded;
    }

    public Utils.pickingStates pickTree(GameObject target){
        int key = target.GetInstanceID();

        if(treeAppleMap.ContainsKey(key)){
            int numAdded = 0;

            int numApples = treeAppleMap[key];
            numAdded = addApple(numApples);
            treeAppleMap[key] -= numAdded;

            Debug.Log("Player: "+ gameObject.name +", Tree: "+ key + ", Backpack: " + appleBackpack + ", leftOnTree: " + treeAppleMap[key]);

            //if tree empty, remove from treeMap & treeList
            if(treeAppleMap[key] == 0){
                activeTreeList.Remove(target);           
            }

            //if backpack full or trees empty, return to village
            if(appleBackpack == 2 || activeTreeList.Count == 0)
                return Utils.pickingStates.village;
            //if backpack not full, target random tree
            return Utils.pickingStates.newTree;
        } else{ // tree empty or not found
            return Utils.pickingStates.newTree;
        }
       
    }

    public Utils.pickingStates dropApples(){
        appleBackpack = 0;
        return Utils.pickingStates.newTree;
    }

    public GameObject selectRandomTree(){
        // Select tree from treemap with nonzero apples, find ID in treelist & return
        
        List<int> keys = new List<int>(treeAppleMap.Keys);
        keys = keys.Where(t => treeAppleMap[t] > 0).ToList();

        if(keys.Count == 0) return null;

        int index = Random.Range(0, keys.Count);
        int targetID = keys[index];
        foreach(GameObject tree in treeList){
            if(tree.GetInstanceID() == targetID){
                return tree;
            }
        }
        return null;
    }

    public GameObject selectVillage(){
        return GameObject.Find("Viking_Village");
    }
}