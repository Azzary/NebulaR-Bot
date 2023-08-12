---
sidebar_position: 1
---

# Character

L'API du personnage (Character API) dans le combat de NebularBot vous permet d'accéder à certaines fonctionnalités pour interagir avec votre personnage lors des combats. Voici les fonctions disponibles dans l'API du personnage :

| Fonction | Type de retour | Description |
| --- | --- | --- |
| `Character:GetAllies()` | List(Fighter) | Renvoie tous les alliés sur le champ de bataille. |
| `Character:GetEnemies()` | List(Fighter) | Renvoie tous les ennemis sur le champ de bataille. |
| `Character:GetAllEntitys()` | List(Fighter) | Renvoie toutes les entités sur le champ de bataille. |
| `Character:GetCell()` | Cell | Renvoie la cellule actuelle du joueur. |
| `Character:GetLife()` | int | Renvoie la vie actuelle du joueur. |
| `Character:GetMaxLife()` | int | Renvoie la vie maximale du joueur. |
| `Character:GetPA()` | int | Renvoie les points d'action du joueur. |
| `Character:GetPM()` | int | Renvoie les points de mouvement du joueur. |
| `Character:EndTurn()` | void | Termine le tour du joueur. |
| `Character:Move(Cell cell, int nbTry = 0)` | int | Déplace le joueur vers la cellule ciblée. Renvoie la distance restante si le mouvement n'a pas pu être effectué entièrement. |
| `Character:LauchSpell(string key, Cell target)` | bool | Lance un sort sur la cellule ciblée. La touche correspondant au sort doit être fournie. Renvoie true si le sort a été lancé avec succès, false sinon. |

Exemple d'utilisation de l'API du personnage (Character API) en Lua :

```lua
function PlayTurn()
    if Character:GetPA() >= 4 then
        Character:LauchSpell(NUM_2, closerEnemy.Cell)
    end
    Character:EndTurn();
end
```
