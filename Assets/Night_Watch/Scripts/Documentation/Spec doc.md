# **Sistemi necessari in Unity**

### ***(Night Watch 3D – Unity Architecture)***

## **1\. Game State**

Gestisce le fasi:

* Giorno

* Notte

* Nave

* Game Over

### **In Unity**

* `GameManager` (Singleton)

### **Responsabilità**

* Cambio fase

* Reset timer

* Trigger spawn / despawn

* Notifica agli altri sistemi (eventi)

---

## **2\. Timer System**

Gestisce il countdown per Giorno / Notte.

### **In Unity**

* Coroutine:

`StartCoroutine(PhaseTimer());`

### **Responsabilità**

* Decremento tempo

* Trigger `OnPhaseEnd`

---

## **3\. Player Controller System**

Controllo movimento \+ input.

### **Componenti**

* `PlayerController`

* `CharacterController` **oppure** movimento manuale

* Input System (New Input System consigliato)

### **Responsabilità**

* WASD movement

* Clamp area di gioco

* Facing direction

* Action input (SPACE)

---

## **4\. Interaction System**

Sistema di interazione contestuale.

### **Funzione**

* Giorno → raccogli semi

* Notte → pianta semi

### **In Unity**

* Trigger Collider

`public interface IInteractable`  
`{`  
    `void Interact();`  
`}`

---

## **5\. Seed System** 

## Gestisce spawn, movimento e raccolta dei semi.

### **Componenti**

* `SeedSpawner`

* `Seed` (MonoBehaviour)

### **Funzioni**

* Spawn casuale

* Movimento random (livelli \>1)

* Raccolta

* Scaling difficoltà

### **Unity tools**

* Rigidbody (Kinematic) o movimento manuale

* Random Nav-less movement

---

## **6\. Planting Zones System**

Zone fisse dove piantare.

### **Componenti**

* `PlantingZone`

* Collider \+ stato `isPlanted`

### **Responsabilità**

* Validazione piantagione

* Consumo seme

* Spawn fiore

---

## **7\. Flower System**

Gestisce i fiori come **risorsa strutturale**, non solo visiva.

### **Componenti**

* `Flower`

* `FlowerManager`

### **Responsabilità**

* Spawn

* Conteggio

* Notifica Gate System

---

## **8\. Gate System (Dynamic Geometry)**

Sistema chiave del gioco.

### **Componenti**

* `GateController`

* Due pilastri laterali

### **Responsabilità**

* Apertura dinamica

* Interpolazione (Lerp)

* Riceve input dal Flower System

`targetGap = baseGap + flowers * expansion;`

---

## **9\. Ship System (Win/Lose Validator)**

Il “test finale” di ogni livello.

### **Componenti**

* `ShipController`

* Collider \+ trigger zone

### **Responsabilità**

* Scaling dimensioni nave per livello

* Movimento forward

* Collision check con gate

* Trigger Game Over o Success

---

## **10\. Difficulty Scaling System**

Non è un MonoBehaviour unico, ma **logica centralizzata**.

### **Parametri che scalano:**

* Numero semi

* Velocità semi

* Larghezza nave

### **In Unity**

* `DifficultyConfig` (ScriptableObject)

➡️ **Consigliato** per bilanciamento rapido.

---

## **11\. Level Progression System**

Gestisce avanzamento livello.

### **Componenti**

* `LevelManager`

### **Responsabilità**

* Incremento livello

* Reset zone

* Cleanup scena

* Trigger nuovo ciclo

---

## **13\. Feedback & VFX System**

Migliora leggibilità e feel.

### **Componenti**

* ParticleSystem

* Tween (DOTween consigliato)

### **Eventi**

* Raccolta

* Piantagione

* Game Over

* Passaggio livello

---

## **14\. Input System**

Gestione input modulare.

### **Opzioni**

* Unity Input System (Actions)

* Mappature:

  * Move

  * Action

---

## **16\. Audio System (facoltativo ma consigliato)**

Feedback sonoro.

### **Componenti**

* AudioManager

* One-shot SFX

---

# **Architettura Consigliata (High-Level)**

`GameManager`  
 `├─ PhaseSystem`  
 `├─ TimerSystem`  
 `├─ LevelManager`  
 `├─ DifficultyConfig (SO)`  
 `├─ UIManager`  
 `└─ EventBus`

`Gameplay`  
 `├─ PlayerController`  
 `├─ SeedSpawner → Seed`  
 `├─ PlantingZone`  
 `├─ FlowerManager`  
 `├─ GateController`  
 `└─ ShipController`

---

## **(lista rapida)**

Per farlo in Unity servono almeno:

* ✅ Game State / Phase System

* ✅ Player Controller

* ✅ Seed (spawn \+ movimento)

* ✅ Planting Zones

* ✅ Flower → Gate coupling

* ✅ Ship \+ collision test

* ✅ Difficulty Scaling

* ✅ Level Reset / Progression

* ✅ UI / Feedback

