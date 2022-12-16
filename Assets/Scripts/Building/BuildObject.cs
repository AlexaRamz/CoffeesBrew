using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    public Build build;
    BuildingSystem buildSys;
    void Start()
    {
        buildSys = FindObjectOfType<BuildingSystem>();
    }

    public void ChooseObject()
    {
        buildSys.ChangeObject(build);
        Debug.Log("changing");
    }
}
