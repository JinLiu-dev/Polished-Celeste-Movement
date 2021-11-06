using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public Camera Camera1;
    public Camera Camera2;
    public Camera Camera3;
    public Camera Camera4;
    public Camera Camera5;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Camera1.enabled = true;
        Camera2.enabled = false;
        Camera3.enabled = false;
        Camera4.enabled = false;
        Camera5.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        float px = Player.transform.position.x;
        float py = Player.transform.position.y;
        if(px < 9.5) {
            Camera1.enabled = true;
            Camera2.enabled = false;
            Camera3.enabled = false;
            Camera4.enabled = false;
            Camera5.enabled = false;
        }
        if(px > 9.5 && py > 1 && px < 18){
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = false;
            Camera4.enabled = true;
            Camera5.enabled = false;
        }
        if(px > 9.5 && py < 1 && px < 18){
            Camera1.enabled = false;
            Camera2.enabled = true;
            Camera3.enabled = false;
            Camera4.enabled = false;
            Camera5.enabled = false;
        }
        if(px > 18 && py > 2){
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = false;
            Camera4.enabled = false;
            Camera5.enabled = true;
        }
        if(px > 18 && py < 2){
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = true;
            Camera4.enabled = false;
            Camera5.enabled = false;
        }
    }
}
