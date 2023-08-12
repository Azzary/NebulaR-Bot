---
sidebar_position: 0
---

# Script

Les scripts dans NebularBot sont écrits en Lua et sont utilisés pour personnaliser le comportement du bot dans différentes situations. Chaque script peut contenir une fonction `start`, qui est appelée une fois lors du chargement du script.

Voici un exemple de structure de base pour un script :

```lua
-- Définition de la fonction start
function start()
    -- Code à exécuter lors du chargement du script
    print("Script chargé !")
end

-- Autres fonctions personnalisées pour le script
function customFunction()
    -- Code pour une fonction personnalisée
    print("Ceci est une fonction personnalisée.")
end

-- Code à exécuter lors du d'un changement de map
function move()
        return {
        { map = "671", changeMap = "left" },
        { x = 4 , y = -19 , changeMap = "right" },
    }
end

-- Code à exécuter si il n'y a pas de move
function lost()
    return World:GoTo(4, -19)
end


```
