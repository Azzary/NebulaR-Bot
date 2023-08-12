---
sidebar_position: 1
---

# Utilisation des Trajets dans NebularBot

Pour utiliser les trajets dans NebularBot, vous pouvez créer un fichier de trajet avec des paramètres personnalisables en fonction de vos besoins. Voici les paramètres que vous pouvez configurer dans votre trajet :

| Paramètre           | Description                                                                                   |
|---------------------|-----------------------------------------------------------------------------------------------|
| `min_monsters`      | Le nombre minimum de monstres requis pour lancer un combat.                                   |
| `max_monsters`      | Le nombre maximum de monstres souhaité pour un combat.                                       |
| `required_monsters` | La liste des identifiants des monstres que vous souhaitez combattre.                          |
| `avoid_monsters`    | La liste des identifiants des monstres que vous souhaitez éviter complètement.               |
| `gather_elements`   | La liste des identifiants des éléments que vous souhaitez récolter.                           |

Ensuite, vous pouvez définir les mouvements du trajet dans une fonction `Move()`. Cette fonction doit retourner une liste d'actions à effectuer. Chaque action est définie sous forme de dictionnaire avec les clés suivantes :

- `mapid`: l'identifiant de la carte où l'action doit être effectuée.
- `fight`: indique si un combat doit avoir lieu sur cette carte (true) ou non (false).
- `harvest`: indique si une récolte doit avoir lieu sur cette carte (true) ou non (false).
- `changeMap`: la direction dans laquelle se déplacer sur cette carte. Vous pouvez spécifier plusieurs directions séparées par `|` (par exemple : "top|right" pour se déplacer en haut ou à droite).

Voici un exemple de code pour un trajet simple :

```lua
min_monsters = 3
max_monsters = 5
required_monsters = {101, 102, 103}
avoid_monsters = {201, 202}
gather_elements = {301, 302, 303}

function Move()
    return {
        {mapid = 1001, fight = true, harvest = true, changeMap = "top"},
        {mapid = 1002, fight = true, changeMap = "right|left"},
        {mapid = 1003, harvest = true, changeMap = "bot"},
        {mapid = 1004, changeMap = "top"}
    }
end
