using System;
using Jundroo.SimplePlanes.ModTools.Parts;
using Jundroo.SimplePlanes.ModTools.Parts.Attributes;
using UnityEngine;

[Serializable]
[PartModifierDesignerHeader("GPWS Core")]
public class GPWSCore : PartModifier
{
    [DesignerPropertyToggleButton("1", "2", "3", "4", "5", "6", "7", "8", Label = "Activation Group", Order = 10)]
    public int activationGroup = 8;

    [DesignerPropertySlider(0, 20, 21, Header = "Obstacle Detection", Label = "Prediction Time", Order = 100)]
    public float obstacleRangeSeconds = 10;

    [DesignerPropertyToggleButton(Header = "Verticle Speed Warning", Label = "Enabled", Order = 100)]
    public bool isVerticleSpeedWarningEnabled = true;

    [DesignerPropertyToggleButton(Header = "Angle Warning", Label = "Enabled", Order = 100)]
    public bool isAngleWarningEnabled = true;

    [DesignerPropertyToggleButton(Header = "Stall Warning", Label = "Enabled", Order = 100)]
    public bool isStallWarningEnabled = true;

    public override PartModifierBehaviour Initialize(GameObject partRootObject)
    {
        var behaviour = partRootObject.AddComponent<GPWSCoreBehaviour>();
        return behaviour;
    }
}