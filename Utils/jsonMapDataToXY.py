import os
import json
from collections import defaultdict

def consolidate_json_files_in_folder(path):
    result = defaultdict(list)

    for filename in os.listdir(path):
        if filename.endswith('.json'):
            file_path = os.path.join(path, filename)
            with open(file_path, 'r') as file:
                data = json.load(file)
                coordinate_key = f"{data['X']},{data['Y']}"
                result[coordinate_key].append(data['ID'])

    consolidated_json = {f"{key}": value for key, value in result.items()}
    return consolidated_json

# Exemple d'utilisation avec un chemin de dossier
folder_path = r"C:\Users\remic\source\repos\NebulaR Bot\Nebular.Bot\bin\Debug\maps"  # Remplacez par le chemin de votre dossier contenant les fichiers JSON
consolidated_data = consolidate_json_files_in_folder(folder_path)

with open("PosToMapIDs.json", "w") as outfile:
    json.dump(consolidated_data, outfile, indent=2)
