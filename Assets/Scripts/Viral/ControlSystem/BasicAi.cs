using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.ControlSystem
{
    public class BasicAi : AiMovement
    {
        public float acceleration = 1f;
        private Vector3 defaultDash = new Vector3(20f, 10f, 1f);

        GameObject closestPlayer(){
            GameObject chosenOne = null;
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player")){
                chosenOne = obj;
            }
            return chosenOne;
        }

        void TurnToObject(GameObject obj, AiMachine am){
            if(obj.transform.position.x > am.transform.position.x){
                if(!am.facingRight){
                    am.FlipSprite();
                }
            }
            else{
                if(am.facingRight){
                    am.FlipSprite();
                }
            }
        }

        public override NoInput Act(AiMachine am)
        {
            NoInput ReturnedInputs = new NoInput()
            {

            };
            if(am.CanDash){
                TurnToObject(closestPlayer(), am);

                ReturnedInputs.DashInput = defaultDash;
            }
            else{
                ReturnedInputs.DashInput = Vector3.zero;
            }

            //am.Input.Horizontal += direction * acceleration;

            ReturnedInputs.MoveInput.x = am.Input.Horizontal;
            ReturnedInputs.MoveInput.y = am.Input.Vertical;

            return ReturnedInputs;
        }
    }
}
