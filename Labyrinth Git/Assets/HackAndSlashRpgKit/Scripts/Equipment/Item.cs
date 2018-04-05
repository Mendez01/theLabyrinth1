using UnityEngine;

/// <summary>
/// This is simple details class for Weapon or Armor.
/// Author: Hwan Kim
/// </summary>
[System.Serializable]
public class Item
{
    public enum Type
    {
        /// <summary>
        /// improve 3 % faster
        /// </summary>
        Light = -3,

        /// <summary>
        /// No impact on character speed.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// decrease speed 1%
        /// </summary>
        Medium = 1,

        /// <summary>
        /// decrease speed 3%
        /// </summary>
        Heavy = 3
    }

    // Property signatures:

    public Type wightType;

    public int attackPoint;

    public int defencePoint;

    public string description;

    //Do this variables for more detailed game stuff.
    public float blockRate;

    public float dodgeRate;

    public float resistance;

    public bool needBodyPart;
    public string nameOfSet;
    public string id;
    public string equipmentName;
    public bool isSet = false;
    public bool dualweild = false;
    public bool twoHands = false;

    //This values are only used for head armor type.
    //And it is depend on the your choice to show this facial items with the head gear.
    public bool eyeBlowsVisible, facialHairVisible, HairVisible;
}