using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFourDirections { none = -1, up, right, down, left };

public class Whisker : MonoBehaviour
{
    OverworldAgent Agent;
    [SerializeField]
    EFourDirections WhiskerLocation = EFourDirections.none;

    Dictionary<string, int> Interactions = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindAgent());
    }

    IEnumerator FindAgent()
    {
        while (Agent == null)
        {
            Agent = GetComponentInParent<OverworldAgent>();
            yield return null;
        }
        Agent.SetWhiskerInfo(WhiskerLocation, Interactions);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        ChangeInteraction(collision.tag, 1);
    }

    void ChangeInteraction(string tag, int change)
    {
        if (!Interactions.ContainsKey(tag))
        {
            Interactions.Add(tag, 0);
        }
        Interactions[tag] += change;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        ChangeInteraction(collision.tag, -1);
    }
}
