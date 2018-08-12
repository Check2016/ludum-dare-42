using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;

public class SpecialCamera : MonoBehaviour
{
    private const float CrystalReleaseDistance = 1;
    private const float CrystalReleaseForce = 10;

    public Player player;
    public Camera FPSCamera;
    public float SwayAmount = 1;
    public float SwaySpeed = 1;

    [Space]
    public RenderTexture CameraVisorRT;

    [Space]
    public float CameraSightCone = 0.15f;
    public float MaxPauseDistance = 1000;

    public Animator animator;

    private Vector3 startPosition;

    private PlayerUI playerUI;

    private Picture[] pictures;
    private int pictureSelection = 0;

    private void Start()
    {
        startPosition = transform.localPosition;

        playerUI = player.playerUI;

        pictures = new Picture[playerUI.PictureSlots.Length];

        UpdateSelectionOutline();
        UpdatePictureSlots();
    }

    private void Update()
    {
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            TakePicture( PictureTypes.Freeze );
        }

        if ( Input.GetMouseButtonDown( 1 ) )
        {
            TakePicture( PictureTypes.Capture );
        }

        if ( Input.GetMouseButtonDown( 2 ) )
        {
            DeletePicture();
        }

        if ( Input.GetAxis( "Mouse ScrollWheel" ) != 0 )
        {
            if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0 )
            {
                pictureSelection--;
            }
            else
            {
                pictureSelection++;
            }

            if ( pictureSelection < 0 )
            {
                pictureSelection = playerUI.PictureSlots.Length - 1;
            }
            else if ( pictureSelection >= playerUI.PictureSlots.Length )
            {
                pictureSelection = 0;
            }

            UpdateSelectionOutline();
        }
    }

    private void FixedUpdate()
    {
        float mouseX = CrossPlatformInputManager.GetAxis( "Mouse X" );
        float mouseY = CrossPlatformInputManager.GetAxis( "Mouse Y" );

        transform.localPosition += new Vector3( mouseX, mouseY, 0 ) * SwayAmount;

        transform.localPosition = Vector3.Lerp( transform.localPosition, startPosition, Time.fixedDeltaTime * SwaySpeed );
    }

    private void TakePicture( PictureTypes pictureType )
    {
        bool hasFreeSlot = false;

        for ( int i = 0; i < pictures.Length; i++ )
        {
            if ( pictures[i] == null )
            {
                hasFreeSlot = true;
                break;
            }
        }

        if ( hasFreeSlot == false ) return;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = CameraVisorRT;

        Texture2D texture2D = new Texture2D( CameraVisorRT.height, CameraVisorRT.height, TextureFormat.RGB24, true );

        int xOffset = ( int )( ( CameraVisorRT.width - CameraVisorRT.height ) / 2.0f );

        texture2D.ReadPixels( new Rect( xOffset, 0, CameraVisorRT.height, CameraVisorRT.height ), 0, 0 );
        texture2D.Apply();

        RenderTexture.active = currentRT;

        Picture picture = new Picture( texture2D, pictureType );

        switch ( pictureType )
        {
            case PictureTypes.Freeze:
                {
                    List<Enemy> frozenEnemies = new List<Enemy>();

                    List<Enemy> enemies = LevelManager.instance.GetEnemies();

                    for ( int i = 0; i < enemies.Count; i++ )
                    {
                        if ( IsInSightOfCamera( enemies[i].transform ) )
                        {
                            frozenEnemies.Add( enemies[i] );
                        }
                    }

                    if ( frozenEnemies.Count > 0 )
                    {
                        picture.frozenEnemies = frozenEnemies.ToArray();
                    }
                    else
                    {
                        picture.type = PictureTypes.None;
                    }
                }
                break;

            case PictureTypes.Capture:
                {
                    GameObject capturedGameObject = null;
                    float smallestDistance = 0;

                    for ( int i = 0; i < LevelManager.instance.Crystals.Length; i++ )
                    {
                        if ( LevelManager.instance.Crystals[i].gameObject.activeSelf == false ) continue;

                        if ( IsInSightOfCamera( LevelManager.instance.Crystals[i].transform ) )
                        {
                            float distance = Vector3.Distance( LevelManager.instance.Crystals[i].transform.position, player.transform.position );

                            if ( capturedGameObject == null || distance < smallestDistance )
                            {
                                capturedGameObject = LevelManager.instance.Crystals[i].gameObject;
                                smallestDistance = distance;
                            }
                        }
                    }

                    if ( capturedGameObject != null )
                    {
                        capturedGameObject.SetActive( false );
                        picture.capturedGameObject = capturedGameObject;
                    }
                    else
                    {
                        picture.type = PictureTypes.None;
                    }
                }
                break;
        }

        for ( int i = 0; i < playerUI.PictureSlots.Length; i++ )
        {
            if ( pictures[i] == null )
            {
                pictures[i] = picture;

                pictureSelection = i;
                UpdateSelectionOutline();

                break;
            }
        }

        UpdatePictureSlots();

        switch( pictureType )
        {
            case PictureTypes.Freeze:
                {
                    UpdateFrozenEnemies();
                }
                break;
            case PictureTypes.Capture:
                {
                    UpdateFrozenEnemies();
                }
                break;
        }
    }

    private void DeletePicture()
    {
        if ( pictures[pictureSelection] == null ) return;

        PictureTypes pictureType = pictures[pictureSelection].type;

        if ( pictureType == PictureTypes.Capture )
        {
            if ( pictures[pictureSelection].capturedGameObject.CompareTag( "Crystal" ) )
            {
                pictures[pictureSelection].capturedGameObject.transform.position = player.transform.position + player.transform.forward * CrystalReleaseDistance;
                pictures[pictureSelection].capturedGameObject.SetActive( true );
                pictures[pictureSelection].capturedGameObject.GetComponent<Rigidbody>().AddForce( player.transform.forward * CrystalReleaseForce + player.GetComponent<CharacterController>().velocity, ForceMode.Impulse );
            }
        }

        pictures[pictureSelection] = null;
        UpdatePictureSlots();

        if ( pictureType == PictureTypes.Freeze )
            UpdateFrozenEnemies();
    }

    private bool IsInSightOfCamera( Transform target )
    {
        Vector3 ToTargetDir = ( target.position - FPSCamera.transform.position ).normalized;

        target.gameObject.layer = 0;

        Ray ray = new Ray( FPSCamera.transform.position, ToTargetDir );

        RaycastHit raycastHit;

        if ( Physics.Raycast( ray, out raycastHit, MaxPauseDistance, 1 << 0 ) )
        {
            target.gameObject.layer = 8;

            if ( raycastHit.transform == target && 1 - Vector3.Dot( FPSCamera.transform.forward.normalized, ToTargetDir ) <= CameraSightCone )
            {
                return true;
            }
        }

        target.gameObject.layer = 8;

        return false;
    }

    private void UpdateFrozenEnemies()
    {
        List<Enemy> enemies = LevelManager.instance.GetEnemies();

        for ( int i = 0; i < enemies.Count; i++ )
        {
            enemies[i].enabled = true;
        }

        for ( int i = 0; i < playerUI.PictureSlots.Length; i++ )
        {
            if ( pictures[i] == null || pictures[i].type != PictureTypes.Freeze ) continue;

            for ( int j = 0; j < pictures[i].frozenEnemies.Length; j++ )
            {
                pictures[i].frozenEnemies[j].enabled = false;
            }
        }
    }

    private void UpdatePictureSlots()
    {
        for ( int i = 0; i < pictures.Length; i++ )
        {
            if ( pictures[i] != null )
            {
                playerUI.PictureSlots[i].texture = pictures[i].texture;

                if ( pictures[i].type != PictureTypes.None )
                    playerUI.PictureTypeTexts[i].text = pictures[i].type.ToString();
                else
                    playerUI.PictureTypeTexts[i].text = "";
            }
            else
            {
                playerUI.PictureSlots[i].texture = null;
                playerUI.PictureTypeTexts[i].text = "";
            }
        }
    }

    public Picture[] GetPictures()
    {
        return pictures;
    }

    private void UpdateSelectionOutline()
    {
        playerUI.SelectionOutline.anchoredPosition = new Vector2( playerUI.PictureSlots[pictureSelection].rectTransform.anchoredPosition.x, playerUI.SelectionOutline.anchoredPosition.y );
    }
}
