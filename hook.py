#!/usr/bin/env python3
import sys
import os
import frida
chemin = os.getcwd()

path = (chemin+"/script.js")
SCRIPT = open(path, "r").read()

def hook(target, info, port=8080, filter="true"):
    session = frida.attach(target)
    script = SCRIPT.replace("PORT", str(port)).replace("FILTER", filter).replace("INFO", info)
    frida_script = session.create_script(script)
    frida_script.load()

def start_hook(): 
    target = sys.argv[1]
    info = "" if len(sys.argv) <= 2 else sys.argv[2]
    port = "8080" if len(sys.argv) <= 3 else sys.argv[3]
    print(target, info, port)
    filter = "true" if len(sys.argv) <= 4 else sys.argv[4]
    if str.isdigit(target):
        target = int(target)
    hook(target, info, port, filter)
    if not sys.flags.interactive:
        sys.stdin.read() # infinite loop
        
        
start_hook()