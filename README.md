# NebulaR-Bot

## Introduction

Ce projet est un bot pour Dofus Retro développé en C#. Le bot utilise la bibliothèque `user32.dll` pour simuler des clics et des pressions de touches, et interagit avec le client du jeu pour interpréter les paquets réseau. 

## Fonctionnalités

### Informations du Jeu

Le bot récupère les informations suivantes :

- Connexion au jeu
- Données de la carte
- Détails du personnage
- Scripting Deplacement, Combat, Recolte
- Informations relatives aux combats

### Scripting

Le bot utilise Lua pour la partie script et IA. De plus voici un exemple de script pour gérer un paquet et effectuer une séquence d'actions :

exemple:
Script: 
```lua
function start()
    PacketHandler:Add("GA0;1;" .. tostring(Character.ID), "PrintHello")
end
```

### Mouvement

Le script de mouvement est également géré en Lua. Par exemple :

```lua
function move()
    return {
        { map = "10317" , changeMap = "top" },
        ...
    }
end
```

### Intelligence Artificielle (IA)

L'IA du bot est capable de lancer des sorts et de se déplacer. Par exemple :
```lua
function Fight()
    if botInstance:GetTurn() % 5 == 0 or botInstance:GetTurn() == 1 then
        ...
    end
end
```


