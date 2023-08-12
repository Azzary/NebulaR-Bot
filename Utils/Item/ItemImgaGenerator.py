import os
import xml.etree.ElementTree as ET
import subprocess
import shutil
import threading
from concurrent.futures import ThreadPoolExecutor

# Dossiers
DOSSIER_XML = "items"
DOSSIER_SWF = "swf"
DOSSIER_SORTIE = "output"

# Assurez-vous que le dossier de sortie existe
if not os.path.exists(DOSSIER_SORTIE):
    os.makedirs(DOSSIER_SORTIE)

# Dictionnaire pour stocker les compteurs pour chaque type
type_counters = {}
current_directory = os.getcwd()

output_path = os.path.join(current_directory, "output")
swf_path = os.path.join(current_directory, "swf")


def extract_number(filename):
    return int(filename.split(".")[0])

def extract_and_convert(item_id, swf_path_file):
    output_path_folder = os.path.join(output_path, item_id)  # le chemin du dossier
    output_path_svg = os.path.join(output_path_folder, "1.svg")  # le chemin du fichier SVG
    output_path_png = os.path.join(output_path, f"{item_id}.png")  # le chemin du fichier PNG final
    final_output = os.path.join(output_path, f"{item_id}.svg")
    # Appeler JPEXS FFDec pour extraire les formes en SVG
    print('C:\\JPEXS\\ffdec.bat', '-export', 'shape', output_path_folder, swf_path_file)
    try:
        cmd = f'"C:\\Program Files (x86)\\FFDec\\ffdec.bat" -export shape "{output_path_folder}" "{swf_path_file}"'
        subprocess.run(cmd, shell=True, check=True)
        
        # Conversion du SVG en PNG
        if os.path.exists(output_path_svg):
            shutil.move(output_path_svg, final_output)
            shutil.rmtree(output_path_folder)
    except Exception as e:
        print(f"Erreur lors de l'extraction ou de la conversion : {e}")



with ThreadPoolExecutor(max_workers=10) as executor:
    for xml_filename in sorted(os.listdir(DOSSIER_XML), key=extract_number):
        if(os.path.exists(output_path +"\\"+ xml_filename.split('.')[0] + ".svg")):
            continue
        if xml_filename.endswith(".xml"):
            # Charger le contenu XML
            tree = ET.parse(os.path.join(DOSSIER_XML, xml_filename))
            root = tree.getroot()
            
            # Extraire l'ID et le Type
            item_id = root.find("ID").text
            item_type = root.find("Type").text
            
            # Mettre à jour le compteur pour le type spécifique
            if item_type in type_counters:
                type_counters[item_type] += 1
            else:
                type_counters[item_type] = 1

            # Chemin du fichier SWF correspondant basé sur le type et le compteur
            swf_path_file = os.path.join(swf_path, item_type, f"{type_counters[item_type]}.swf")
            
            if os.path.exists(swf_path_file):
                executor.submit(extract_and_convert, item_id, swf_path_file)         