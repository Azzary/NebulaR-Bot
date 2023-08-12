import os
import mysql.connector
import xml.etree.ElementTree as ET

# Créer le dossier 'items' s'il n'existe pas
if not os.path.exists("items"):
    os.makedirs("items")

# Établir une connexion à la base de données
conn = mysql.connector.connect(
    host="localhost",
    user="root",
    password="",
    database="leafworld"
)
cursor = conn.cursor()

# Sélectionner tous les enregistrements de la table itemtemplate
cursor.execute("SELECT Id, Type, Name, Niveau, Pods, ConditionForUse, InfosWeapon FROM itemtemplate")
items = cursor.fetchall()
curentdir = os.getcwd()
# Pour chaque item, créer un fichier XML dans le dossier 'items'
for item in items:
    Id, Type, Name, Niveau, Pods, ConditionForUse, InfosWeapon = item

    root = ET.Element("item")

    ET.SubElement(root, "ID").text = str(Id)
    ET.SubElement(root, "Type").text = str(Type)
    ET.SubElement(root, "Name").text = Name
    ET.SubElement(root, "Level").text = str(Niveau)
    ET.SubElement(root, "Pods").text = str(Pods)
    ET.SubElement(root, "ETHEREAL").text = "0"
    ET.SubElement(root, "Condition").text = ConditionForUse if ConditionForUse else ""
    ET.SubElement(root, "WeaponStats").text = InfosWeapon if InfosWeapon else ""

    # Enregistrer le fichier dans le dossier 'items'
    tree = ET.ElementTree(root)
    tree.write(os.path.join(curentdir,"items", f"{Id}.xml"))

cursor.close()
conn.close()
