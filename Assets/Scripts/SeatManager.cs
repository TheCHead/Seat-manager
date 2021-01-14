using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    [SerializeField] GameObject chairPrefab = null;
    [Header("Chairs")]
    [SerializeField] int numberOfChairs = 10;
    [SerializeField] float chairSpawnRadius = 5f;
    [SerializeField] Transform parentChairs = null;

    [Header("Guests")]
    [SerializeField] GameObject guestPrefab = null;
    [SerializeField] int numberOfGuests = 5;
    [SerializeField] float guestSpawnRadius = 2f;
    [SerializeField] Transform parentGuests = null;
    [Range(0, 100)]
    [SerializeField] float searchSeatChance = 45f;
    Coroutine currentSearch = null;

    void Start()
    {
        SpawnChairs();
        SpawnGuests();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSearch != null)
            {
                StopCoroutine(currentSearch);
            }
            // Used coroutine for guests to be searching for seats one by one
            currentSearch = StartCoroutine(ReserveSeats());
        }
    }


    // Spawns set number of chairs in set radius around zero coordinates. Might be taken in separate class to ensure SRP.
    private void SpawnChairs()
    {
        for (int i = 0; i < numberOfChairs; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfChairs;
            Vector3 spawnPoint = new Vector3(Mathf.Cos(angle) * chairSpawnRadius, 0, Mathf.Sin(angle) * chairSpawnRadius);
            GameObject newChair = Instantiate(chairPrefab.gameObject, spawnPoint, Quaternion.identity, parentChairs);
            newChair.transform.LookAt(Vector3.zero);
        }
    }

    // Spawns set number of guests in set radius around zero point. Might be taken in separate class to ensure SRP.
    private void SpawnGuests()
    {
        for (int i = 0; i < numberOfGuests; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfGuests;
            Vector3 spawnPoint = new Vector3(Mathf.Cos(angle) * guestSpawnRadius, 0, Mathf.Sin(angle) * guestSpawnRadius);
            GameObject newGuest = Instantiate(guestPrefab.gameObject, spawnPoint, Quaternion.identity, parentGuests);
        }
    }

    // Returns a list of chairs not yet reserved by other guests
    private List<Chair> GetAvailableChairs()
    {
        List<Chair> allChairs = FindObjectsOfType<Chair>().ToList();

        List<Chair> availableChairs = new List<Chair>();

        for (int i = 0; i < allChairs.Count; i++)
        {
            if (allChairs[i].IsFree())
            {
                availableChairs.Add(allChairs[i]);
            }
        }
        return availableChairs;
    }

    // Called whenever user clicks "Space"
    private IEnumerator ReserveSeats()
    {
        Guest[] guests = FindObjectsOfType<Guest>();
        foreach (Guest g in guests)
        {
            if (Random.value < searchSeatChance/100 || !g.IsInSearch())
            {
                List<Chair> availableChairs = GetAvailableChairs();
                yield return g.FindSeat(availableChairs);
            }
        }
    }
}
