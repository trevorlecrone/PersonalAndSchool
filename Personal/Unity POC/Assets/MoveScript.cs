using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveScript : MonoBehaviour
{
    // object animator
    Animator animator;

    bool facingLeft = false;

    //default move speed
    private float moveSpeed = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Time.fixedDeltaTime = (float)(1.0 / 120.0);
        gameObject.name = "Cool Guy";
    }

    // FixedUpdate is called once per fixed interval
    void FixedUpdate()
    {
        
        if(Input.GetKey(KeyCode.UpArrow) == true)
        {
            animator.SetBool("Moving", true);
            animator.SetInteger("Facing", 0);
            transform.position = transform.position + (transform.up * moveSpeed);
            facingLeft = false;
        }
        else if (Input.GetKey(KeyCode.DownArrow) == true)
        {
            animator.SetBool("Moving", true);
            animator.SetInteger("Facing", 1);
            transform.position = transform.position - (transform.up * moveSpeed);
            facingLeft = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow) == true)
        {
            animator.SetBool("Moving", true);
            animator.SetInteger("Facing", 2);
            transform.position = transform.position + (transform.right * moveSpeed);
            facingLeft = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) == true)
        {
            animator.SetBool("Moving", true);
            animator.SetInteger("Facing", 3);
            transform.position = transform.position + (transform.right * moveSpeed);
            facingLeft = true;
        }
        else
        {
            animator.SetBool("Moving", false);
        }
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, facingLeft ? 180f : 0f, 0f));
    }
}
