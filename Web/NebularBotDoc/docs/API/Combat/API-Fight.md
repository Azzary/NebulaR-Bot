---
sidebar_position: 3
---

# Fight


| Fonction | Type de retour | Description |
| --- | --- | --- |
| `Fight:GetTurn()` | int | Renvoie le tour actuel du combat. |
| `Fight:GetPathDistanceBetween(Cell start, Cell end)` | int | Renvoie la distance en nombre de cases entre deux cellules. |
| `Fight:GetSafestCell()` | Cell | Renvoie la cellule la plus sûre sur la carte en fonction de la distance aux ennemis. |
| `Fight:GetClosestEnemy()` | Fighter | Renvoie l'ennemi le plus proche du joueur. |



Voici un exemple d'utilisation de l'API de combat (Fight API) en Lua :

```lua
function PlayTurn()
    local turn = Fight:GetTurn()
    print("Tour actuel: " .. turn)

    local startCell = Character:GetCell()
    local endCell = Fight:GetClosestEnemy()
    local distance = Fight:GetPathDistanceBetween(startCell, endCell)
    print("Distance entre les cellules: " .. distance)
        print("Ennemi le plus proche: " .. closestEnemy.Name)
    end
end
```
