import pymysql
import json

# Connexion à la base de données
connection = pymysql.connect(
    host='localhost',
    user='root',
    password='',
    db='leafworld'
)

# Création d'un curseur
cursor = connection.cursor()

# Exécution de la requête SQL pour obtenir toutes les lignes de la table
cursor.execute("SELECT * FROM scriptedcells")

# Récupération de toutes les lignes
rows = cursor.fetchall()

# Dictionnaire pour stocker les résultats
result = {}

for row in rows:
    MapID, CellID, ActionID, EventID, ActionsArgs, Conditions = row

    # Conversion des conditions
    Conditions = False if Conditions == "-1" else True

    # Ajout des données dans le dictionnaire
    if MapID not in result:
        result[MapID] = {}

    result[MapID][CellID] = {
        "ActionsArgs": ActionsArgs,
        "Conditions": Conditions
    }

# Écriture des données dans un fichier JSON
with open('output.json', 'w') as f:
    json.dump(result, f, ensure_ascii=False)

print("Les données ont été exportées avec succès dans le fichier 'output.json'.")

# Fermeture de la connexion à la base de données
connection.close()
