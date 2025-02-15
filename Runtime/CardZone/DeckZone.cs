using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadSapphicGames.CardEngine {
    public class DeckZone : CardZone
    {
        private Queue<Card> cardQueue;
        private void Start() {
            CardsDraggable = false;
        }

        public void LoadDecklist(DecklistSO deckData,CardActor owner) {
            Debug.Log($"loading decklist {deckData.name}");
            foreach (var entry in deckData.deckList) {
                for (int i = 0; i < entry.count; i++) {
                    CardEngineManager.instance.InstantiateCard(this,entry.card,owner);
                }
            }
            Shuffle();
        }
        public void Shuffle() {
            int deckSize = cards.Count();
            cardQueue = new Queue<Card>();
            // List<int> order = Enumerable.Range(0,deckSize);
            for (int i = deckSize -1; i >= 0; i--) {
                int j = Random.Range(0,i);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            } for (int i = 0; i < deckSize; i++) { //? there is perhaps a way to combine these into one loop
                cardQueue.Enqueue(cards[i]);
                cards[i].transform.SetSiblingIndex(deckSize - 1 - i);    
            }
        }
        public void DrawCard(HandZone playerHand) {
            Card nextCard = cardQueue.Dequeue();
            nextCard.IsVisible = true;
            this.MoveCard(nextCard, playerHand);

        }
        public override void AddCard(Card card)
        {
            base.AddCard(card);
            card.IsVisible = false;
            Shuffle();
        }
    }
}
