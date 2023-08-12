---
sidebar_position: 2
---

# PacketHandler

| Nom                             | Type de retour | Description |
|---------------------------------|----------------|-------------|
| `PacketHandler:Add(Packet, CallBack)`    | void            | Ajoute un gestionnaire de paquet pour le paquet spécifié par `Packet` qui appelle la fonction `CallBack`. Si un gestionnaire pour ce paquet existe déjà, aucun gestionnaire supplémentaire n'est ajouté. |
| `PacketHandler:Remove(Packet)`           | void            | Supprime le gestionnaire de paquet pour le paquet spécifié par `Packet`. |
| `PacketHandler:callCallback(Packet)`     | bool            | Appelle la fonction de rappel du gestionnaire de paquet pour le paquet spécifié par `Packet`. Si un gestionnaire pour ce paquet existe, la fonction de rappel est appelée et la méthode renvoie `true`, sinon elle renvoie `false`. |
                                   |



```lua
    function start()
        PacketHandler:Add("GA0;1;" .. tostring(Character:ID()), "PrintHello")
    end

    function PrintHello(packet)
        print("HELLO WORLD")
        print(Character:Name())
    end
```

Dans ce script, nous avons une fonction start (qui est appeler une fois au chargement du script) qui ajoute un handler pour le paquet "GA0;1;" suivi de l'ID du personnage, qui déclenchera la fonction PrintHello lorsque ce paquet sera reçu (Deplacement du joueur).