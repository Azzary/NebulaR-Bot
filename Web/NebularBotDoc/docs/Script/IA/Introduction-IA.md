---
sidebar_position: 1
---

# Introduction à l'IA

Dans NebularBot, l'IA de combat est conçue pour vous aider à gérer vos combats de manière efficace. Pour utiliser l'IA de combat, il est faut avoir une fonction PlayTurn qui servira de point d'entrée.

Voici un exemple d'IA:

```lua

function PlayTurn()
    local turn = Fight:GetTurn()
    local playerCell = Character:GetCell()

    if turn % 5 == 0 or turn == 1 then
        Character:LauchSpell("NUM_3", playerCell)
    end

    local closestEnemy = Fight:GetClosestEnemy()
    local closestEnemyDistance = Character:Move(closestEnemy:Cell())

    if Character:GetPA() >= 4 and closestEnemyDistance <= 1 then
        Character:LauchSpell("NUM_2", closestEnemy:Cell())
    end
    Character:EndTurn()
end
```

Dans cet exemple, l'IA vérifie si c'est le 5eme tours ou le premier tour pour lancer un sort. Ensuite, elle sélectionne l'ennemi le plus proche et se déplace vers lui. Si le personnage a au moins 4 points d'action et que l'ennemi est à une distance de 1 case ou moins, l'IA lance un autre sort (ici, le sort associé à la touche "2") sur l'ennemi.

Cet exemple est simple mais efficace, et vous pouvez le personnaliser en fonction de vos sorts préférés et de votre style de jeu. Vous pouvez également ajouter davantage de conditions et de stratégies pour rendre l'IA encore plus puissante dans les combats.

Nous espérons que cette IA de combat vous sera utile dans vos aventures sur Dofus Retro avec NebularBot !