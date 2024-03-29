import os
import shutil
import subprocess
import time
import xml.etree.ElementTree as ET
from pathlib import Path
import json

print ("Don't build this.")
exit(0)

# ----- CONFIGURE THESE -----
#SYNC_FOLDERS = ["Blueprints", "Boosterpacks", "Cards", "Icons", "Sounds"] # folders to be synced, such as Cards, Blueprints, Icons, etc.
#COPY_FILES = ["manifest.json", "localization.tsv", "workshop.txt", "icon.png"] # individual files to copy, such as manifest.json, localization.tsv, etc. (the mod dll is copied automatically)
MODS_ROOT = Path(os.environ["userprofile"]) / Path("AppData/LocalLow/sokpop/Stacklands/Mods") # windows only, can be hardcoded with the below line instead
#MODS_ROOT = Path("C:/Users/cyber/AppData/LocalLow/sokpop/Stacklands/Mods").resolve()

MOD_BIN = Path("./bin/Debug/netstandard2.1").resolve()

def sync_folder(src: Path, dst: Path):
    for file in dst.glob("**/*"):
        file_in_src = src / file.relative_to(dst)
        if file.is_dir() and not file_in_src.exists():
            shutil.rmtree(file)
        elif file.is_file() and (not file_in_src.exists() or file.stat().st_mtime < file_in_src.stat().st_mtime):
            os.remove(file)
    for file in src.glob("**/*"):
        file_in_dst = dst / file.relative_to(src)
        if not file_in_dst.exists():
            file_in_dst.parent.mkdir(parents=True, exist_ok=True)
            if file.is_file():
                shutil.copy(file, file_in_dst)
            elif file.is_dir():
                shutil.copytree(file, file_in_dst)

# build
start_time = time.time()
p = subprocess.Popen("dotnet build", stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=True)
stdout, stderr = p.communicate()
if p.returncode != 0:
    print(stdout.decode())
    exit(p.returncode)
print(f"build started {time.strftime('%H:%I:%S', time.localtime(start_time))}, finished in {time.time() - start_time:.2f}s")

# grab metadata
found_csprojs = list(Path(".").glob("*.csproj"))
if len(found_csprojs) != 1:
    raise RuntimeError("Can't find .csproj file")
with open(found_csprojs[0], encoding="utf-8") as f:
    root = ET.parse(f).getroot()
    MOD_ID = json.load(open("manifest.json"))["id"]
    DLL_NAME = f"{root.find('./PropertyGroup/AssemblyName').text}.dll"
    MOD_DLL = MOD_BIN / DLL_NAME
    MOD_PATH = MODS_ROOT / MOD_ID

# check dll has unique name
if DLL_NAME.lower() == "examplemod.dll":
    print("Did you forget to rename the DLL in the project settings?")
    exit(1);

for file in MODS_ROOT.glob("./*"):
    if (file / ".").is_dir:
        try:
            print("Copied to " +  f"{file / DLL_NAME}")
            shutil.copyfile(MOD_DLL, file / DLL_NAME)
        except:
            print("Can't create " +  f"{file / DLL_NAME}")

exit(1);