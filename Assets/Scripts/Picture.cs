using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture {

    public Texture2D texture;
    public PictureTypes type;

    public Enemy[] frozenEnemies;
    public GameObject[] capturedGameObjects;

    public Picture( Texture2D texture2D, PictureTypes pictureType )
    {
        texture = texture2D;
        type = pictureType;
    }
}

public enum PictureTypes
{
    Freeze,
    Capture
}