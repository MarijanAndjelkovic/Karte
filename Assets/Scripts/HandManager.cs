using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<GameObject> cardsInHand = new List<GameObject>();

    public float maxHandWidth = 8f;
    public float maxSpacing = 1.2f;

    public float curveHeight = 0.3f;
    public float maxCardAngle = 15f;
    public float hoverLift = 0.5f;

    public void AddCard(GameObject card)
    {
        cardsInHand.Add(card);
        card.transform.SetParent(this.transform);
        ReorganizeHand();
    }

    public void RemoveCard(GameObject card)
    {
        if (cardsInHand.Contains(card))
        {
            cardsInHand.Remove(card);
            ReorganizeHand();
        }
    }

    public void ReorganizeHand()
    {
        int count = cardsInHand.Count;
        if (count == 0) return;

        float currentSpacing = Mathf.Min(maxSpacing, maxHandWidth / count);
        float startX = -(currentSpacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject card = cardsInHand[i];
            CardController cc = card.GetComponent<CardController>();

            if (cc != null && cc.isDragging) continue;

            float normalizedPosition = (count <= 1) ? 0f : ((float)i / (count - 1)) * 2f - 1f;

            float xPos = startX + (i * currentSpacing);
            float yPos = -(normalizedPosition * normalizedPosition) * curveHeight;
            float zPos = -(i * 0.004f);
            float zRotation = -normalizedPosition * maxCardAngle;

          
            Vector3 cardPos = new Vector3(xPos, yPos, zPos);
            Quaternion cardRot = Quaternion.Euler(0, 0, zRotation);

            if (cc != null && cc.isHovering)
            {
               
                cardPos += (cardRot * Vector3.up) * hoverLift;
            }

          
            card.transform.localPosition = cardPos;
            card.transform.localRotation = cardRot;
        }
    }
}