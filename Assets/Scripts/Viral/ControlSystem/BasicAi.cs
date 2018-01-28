using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.ControlSystem
{
    public class BasicAi : AiMovement
    {
        float maxSpeed = 1f;
        int direction = 1;
        public float acceleration = 0.0125f;

        public override void Act(AiMachine am)
        {
            if(direction == 1){
                if(am.Input.Horizontal >= maxSpeed){
                    direction *= -1;
                }
            }
            else{
                if(am.Input.Horizontal <= maxSpeed * -1){
                    direction *= -1;
                }
            }
            am.Input.Horizontal += direction * acceleration;


        }
    }
}
