using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum ConstraintType
{
    None,
    Min,
    Max,
    MinMax
}

public enum ContrainedObject
{
    Transforms,
    AnyChild
}
[Serializable]
public class LevelConstraint 
{
    public ConstraintType type = ConstraintType.None;
    public ContrainedObject contrainedObjectType;
    public Transform[] transformsToContraint;



}
