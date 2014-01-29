import logging
import subprocess

logging.basicConfig(level=logging.DEBUG)


def log_process(command):
    command = command.split()
    process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    for line in process.stdout:
        logging.info(line.strip('\n'))

if __name__ == "__main__":
    log_process('ls -l')
