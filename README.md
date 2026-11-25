# 3D UNO-Style Card Game (Unity)

A 3D multiplayer-ready card game inspired by UNO, built in Unity with C#. The project focuses on tactile, responsive 3D card handling—featuring a dynamic parabolic hand fan, perspective-aware drag-and-drop table physics, and a complete 108-card rules engine.

---

## Features

### 3D Hand & Interaction Mechanics
* **Dynamic Parabolic Hand Fan (`HandManager`):** Automatically calculates card spacing, curvature, and rotation based on the current number of cards in hand.
* **Z-Fighting-Free Hovering:** Cards lift smoothly along their local angle on hover using raycast priority weighting, preventing hitbox flickering and ensuring adjacent cards remain easy to select.
* **Zone-Based Drag & Drop (`CardController`):**
  * **Hand Zone (`GreenZone`):** Cards orient toward the camera while held in hand.
  * **Table Surface (`OrangeZone`):** Cards lay flat on the circular table and smoothly clamp to the table's edge using screen-space projection when the cursor moves off-table.
  * **Discard Pile (`RedZone`):** Validates played cards on drop and stacks them incrementally (`pileHeightOffset`) to prevent Z-fighting on the table.

### Deck & Gameplay Rules (`DeckController`)
* **Standard 108-Card Deck Composition:**
  * **76 Number Cards:** Red, Yellow, Green, and Blue (one `0` and two `1–9` cards per color).
  * **24 Action Cards:** Skip, Reverse, and Draw Two (`+2`) — two of each per color.
  * **8 Wild Cards:** 4× Wild (`cc`) and 4× Wild Draw Four (`pl4`).
* **Fisher-Yates Shuffle & Auto-Recycling:** Automatically shuffles the draw pool at the start of the game and recycles discarded cards back into the deck (keeping the top card on the table) whenever the draw pile runs out.
* **Move Validation:** Ensures the starting table card is strictly a number card (`0–9`). Played cards must match the active color or symbol (or be a Wild card); invalid plays automatically return to the player's hand.
* **Smart Draw Rules:** Players draw from the 3D deck until they obtain a playable card, after which they may optionally draw one additional card before the draw pile locks for that turn.

---

## Core Architecture

| Script | Responsibility |
| :--- | :--- |
| `DeckController.cs` | Generates the 108-card pool from card materials, handles shuffling/recycling, validates legal moves, and enforces draw limits. |
| `HandManager.cs` | Manages the player's hand list, computes the parabolic fan layout, and handles hover elevation. |
| `CardController.cs` | Handles raycast hover detection, 3D drag-and-drop across spatial zones, material/card metadata (`bojaKarte`, `znakKarte`), and Photon PUN networking hooks. |

---

## Getting Started

### Prerequisites
* **Unity** (2021.3 LTS or newer recommended)
* **Photon PUN 2** (imported into the project for `Photon.Pun` multiplayer support)

### Setup
1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/your-repo-name.git](https://github.com/your-username/your-repo-name.git)
   ```
2. Open the project folder via **Unity Hub**.
3. Open the main game scene and verify the following scene references:
   * **`DeckController` (3D Deck Object):** Assign `CardTemplate` prefab, `allCardMaterials` array, `HandManager`, and `RedZone` Transform.
   * **Table Colliders:** Ensure `GreenZone`, `OrangeZone`, and `RedZone` objects are active with their respective colliders configured.
4. Press **Play** in the Unity Editor to test drawing, hovering, and playing cards.

---

## Roadmap
- [x] Parabolic 3D hand layout & raycast hover stabilization
- [x] Circular table & discard pile spatial drag-and-drop
- [x] Full 108-card deck generation, shuffling, and discard recycling
- [x] Move validation & draw-until-playable turn logic
- [ ] Color selection UI overlay when playing Wild (`cc`) and Wild Draw Four (`pl4`) cards
- [ ] Action card effects (Skip, Reverse, +2, +4) and turn order manager
- [ ] Top-card UI indicator for improved visibility across multiplayer camera angles
- [ ] Full Photon PUN room synchronization for 2–4 players
