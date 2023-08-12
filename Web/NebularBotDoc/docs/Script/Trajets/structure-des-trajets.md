---
sidebar_position: 1
---

# Structure des Trajets

Pour utiliser les trajets dans NebularBot, vous pouvez créer un fichier de trajet avec des variables personnalisables en fonction de vos besoins. Voici les variables que vous pouvez initialiser dans votre trajet :

| Variable            | Description                                                                                   |
|---------------------|-----------------------------------------------------------------------------------------------|
| `min_monsters`      | Le nombre minimum de monstres requis pour lancer un combat.                                   |
| `max_monsters`      | Le nombre maximum de monstres souhaité pour un combat.                                       |
| `required_monsters` | La liste des identifiants des monstres que vous souhaitez combattre.                          |
| `avoid_monsters`    | La liste des identifiants des monstres que vous souhaitez éviter complètement.               |
| `gather_elements`   | La liste des identifiants des éléments que vous souhaitez récolter.                           |
|  `min_life`          | Le pourcentage de vie à partir duquel le bot doit attendre avant de continuer               |
Ensuite, vous pouvez définir les mouvements du trajet dans une fonction `Move()`. Cette fonction doit retourner une liste d'actions à effectuer dans l'ordre. Chaque action est définie sous forme de dictionnaire avec les clés suivantes :

- `mapid`: l'identifiant de la carte où l'action doit être effectuée.
- `X`: Possition X de la carte où l'action doit être effectuée.
- `Y`: Possition Y de la carte où l'action doit être effectuée.
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
min_life = 70

function Move()
    return {
        {mapid = 1001, fight = true, harvest = true, changeMap = "top"},
        {mapid = 1002, fight = true, changeMap = "right|left"},
        {mapid = 1003, harvest = true, changeMap = "bot"},
        {mapid = 1004, changeMap = "top"}
    }
end
```

Pour optimiser le déplacement et permettre à un personnage de passer plusieurs fois sur la même carte mais avec des directions différentes, l'API fournit une fonction clé Character:LastMapID(). Cette fonction renvoie l'identifiant de la dernière carte sur laquelle le personnage a été présent. Cela permet de créer des scripts de déplacement plus sophistiqués et adaptatifs, comme illustré ci-dessous :

```lua
function move()
    -- Si le personnage vient de la carte 690 la map a gauche de 957
    if(Character:LastMapID() == 690) then
        -- Lors du passage suivant, il changera de carte en allant vers le haut, sans effectuer de combat ni de récolte.
        return {{ map = "957" , changeMap = "top" },}
    end
    -- Sinon, le script propose une séquence de déplacements à réaliser
    return {
        { map = "951", changeMap = "218" },
        { map = "957", changeMap = "left", fight = true, harvest = true }, -- Lors du premier passage sur la carte, le personnage va combattre et récolter, puis il va changer de carte en passant par la gauche.
        { map = "690", changeMap = "260" },
        { map = "705", changeMap = "318" },
        { map = "707", changeMap = "452" },
    }
end
```