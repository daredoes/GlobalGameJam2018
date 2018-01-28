using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.ControlSystem
{
    public class RegularAi : AiMovement
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

        GameObject absorbingPlayer(){
            GameObject chosenOne = null;
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player")){
                if(obj.GetComponent<PlayerMachine>().CanDash){
                    return obj;
                }
            }
            return chosenOne;
        }

        bool TurnToObject(GameObject obj, AiMachine am){
            if(obj.transform.position.x > am.transform.position.x){
                if(!am.facingRight){
                    am.FlipSprite();
                    return true;
                }
                return false;
            }
            else{
                if(am.facingRight){
                    am.FlipSprite();
                    return false;

                }
                return true;
            }
        }

        public override NoInput Act(AiMachine am)
        {
            NoInput ReturnedInputs = new NoInput()
            {

            };

            GameObject targetedPlayer = absorbingPlayer();
            int accelerationToRight = 1;
            if (targetedPlayer)
            {
                accelerationToRight = TurnToObject(absorbingPlayer(), am) ? 2 : -2;
            }
            am.Input.Horizontal += acceleration * accelerationToRight;


            ReturnedInputs.MoveInput.x = am.Input.Horizontal;
            ReturnedInputs.MoveInput.y = am.Input.Vertical;

            return ReturnedInputs;
        }
    }
}
