using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGenerator : MonoBehaviour
{
    public int depth = 5;
    public float minWidth = 5f;
    public float minHeight = 5f;
    public float maxVariation = 0f;

    public GameObject groundPrefab;
    public GameObject wallPrefab;

    //for partitioning the entire cave prefab into different rooms

    public class BSPNode
    {
        public Rect partition;
        public BSPNode leftChild;
        public BSPNode rightChild;

        public BSPNode(Rect partition)
        {
            this.partition = partition;
        }
    }

    //instantiates the first partitioning and prefabs for passages

    void Start()
    {
        BSPNode root = new BSPNode(new Rect(0, 0, 20, 0));

        SplitPartition(root, depth);
    }

    void SplitPartition(BSPNode node, int remainingDepth)
    {

        //at lowest partition, generate wall if space, else floor
        if (remainingDepth <= 0)
        {
            GenerateTerrain(node.partition);
            return;
        }

        //randomized terrain splitting/generation
        bool splitHorizontal = Random.value > 0.5f;
        float splitPosition;
        if (splitHorizontal)
        {
            splitPosition = Random.Range(node.partition.yMin + minHeight, node.partition.yMax - minHeight);

            node.leftChild = new BSPNode(new Rect(node.partition.xMin, node.partition.yMin, node.partition.width, splitPosition - node.partition.yMin));
            node.rightChild = new BSPNode(new Rect(node.partition.xMin, splitPosition, node.partition.width, node.partition.yMax - splitPosition));
        }
        else
        {
            splitPosition = Random.Range(node.partition.xMin + minWidth, node.partition.xMax - minWidth);

            node.leftChild = new BSPNode(new Rect(node.partition.xMin, node.partition.yMin, splitPosition - node.partition.xMin, node.partition.height));
            node.rightChild = new BSPNode(new Rect(splitPosition, node.partition.yMin, node.partition.xMax - splitPosition, node.partition.height));
        }

        SplitPartition(node.leftChild, remainingDepth - 1);
        SplitPartition(node.rightChild, remainingDepth - 1);
    }

    void GenerateTerrain(Rect partition)
    {
        //Vector3 groundPosition = new Vector3(partition.xMin, partition.xMin, partition.max.y);
        Vector3 groundPosition = new Vector3(partition.min.x, 0, 0);
        Vector3 groundScale = new Vector3(100, 100, 100);
        Quaternion rotation = Quaternion.Euler(270f, 0f, 0f);
        Instantiate(groundPrefab, groundPosition, rotation).transform.localScale=groundScale;

        Vector3 leftWallPosition = new Vector3(partition.min.x, 0, 0);
        Vector3 leftWallScale = new Vector3(100, 100, 100);
        Instantiate(wallPrefab, leftWallPosition, rotation).transform.localScale = leftWallScale;
    }
}
