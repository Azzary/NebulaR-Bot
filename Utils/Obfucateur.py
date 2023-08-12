import subprocess

def execute_command(input_path):
    command = f'./JIEJIE.NET.Console.exe input="{input_path}" switch=+controlflow,+rename,+strings'
    result = subprocess.run(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if result.returncode != 0:
        print(f'Error executing command for {input_path}')
        print(result.stderr.decode())
    else:
        print(f'Successfully executed command for {input_path}')

# Define the input paths
input_paths = [
    "C:\\Users\\remic\\source\\repos\\NebularBot\\Nebular.Bot\\bin\\Release\\Nebular.Bot.exe",
    "C:\\Users\\remic\\source\\repos\\NebularBot\\Nebular.Bot\\bin\\Release\\Nebular.IO.exe"
]

# Execute the command for each input path
for input_path in input_paths:
    execute_command(input_path)
