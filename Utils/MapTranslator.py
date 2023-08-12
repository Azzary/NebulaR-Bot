import pymysql
import json
import os

# Connexion à la base de données
conn = pymysql.connect(
    host='localhost',
    user='root',
    password='',
    db='ata'
)

# Création d'un curseur
cur = conn.cursor(pymysql.cursors.DictCursor)

# Exécution de la requête pour obtenir toutes les lignes de la table "maps"
cur.execute("SELECT * FROM maps")

# Récupération des résultats
rows = cur.fetchall()

# Parcours des résultats
curentdir = os.getcwd()
for row in rows:
    # Construction du chemin du fichier JSON

    json_file_path = os.path.join(curentdir+ "/maps", f"{row['id']}.json")
    if 'mappos' in row:
        x = int(row['mappos'].split(',')[0])
        y = int(row['mappos'].split(',')[1])
        row['X'] = x
        row['Y'] = y
        del row['mappos']
    with open(json_file_path, 'w') as f:
        json.dump(row, f)

# Fermeture de la connexion
conn.close()
