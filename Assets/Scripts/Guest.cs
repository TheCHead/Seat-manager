using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    bool isInSearch = false;
    [SerializeField] float seatingSpeed = 3f;
    public Chair targetChair = null;
    bool isSeated = false;
    Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (targetChair != null)
        {
            TakeSeat();
        }
    }

    // Called by SeatManager
    public Coroutine FindSeat(List<Chair> availableSeats)
    {
        isInSearch = true;
        return StartCoroutine(ReserveSeat(availableSeats));
    }

    // Leave current chair, find free random chair and reserve it,
    // making it unavailable for other guests
    private IEnumerator ReserveSeat(List<Chair> availableSeats)
    {
        if (availableSeats.Count > 0)
        {
            LeaveChair();
            targetChair = SelectRandomChair(availableSeats);
            targetChair.Reserve();
        }      
        yield return null;
    }


    // Leave current chair, making it available for other guests
    private void LeaveChair()
    {
        if (targetChair != null)
        {
            if (isSeated)
            {
                myAnimator.ResetTrigger("OnChair");              
                isSeated = false;
            }
            targetChair.Leave();
            myAnimator.ResetTrigger("JumpUp");
            myAnimator.SetTrigger("Leave");
            targetChair = null;
        }
    }


    private Chair SelectRandomChair(List<Chair> availableSeats)
    {
        return availableSeats[Random.Range(0, availableSeats.Count - 1)];
    }

    // Called in Update to move towards targetChair, if reserved
    private void TakeSeat()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetChair.transform.position, seatingSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetChair.transform.position) < 0.5f)
        {
            isSeated = true;

            
            myAnimator.SetTrigger("OnChair");

        }
    }

    // Used only for the first search, when every guest should find a chair
    public bool IsInSearch()
    {
        return isInSearch;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Chair>() == targetChair)
        {
            myAnimator.ResetTrigger("Leave");
            myAnimator.SetTrigger("JumpUp");
        }
    }
}
