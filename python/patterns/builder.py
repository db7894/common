import sys
from datetime import datetime

class LogBuilder(object):
    def __init__(self, name):
        self.name = name

    def build(self):
        def logger(data):
            message = "{}:{}:{}\n".format(self.level, datetime.now(), data)
            self.stream.write(message)
        return logger

class ErrorLogBuilder(LogBuilder):
    def set_stream(self):
        self.stream = sys.stderr
    def set_level(self):
        self.level = "error"

class DebugLogBuilder(LogBuilder):
    def set_stream(self):
        self.stream = sys.stdout
    def set_level(self):
        self.level = "debug"

def director(implementation, name):
    builder = implementation(name)
    builder.set_stream()
    builder.set_level()
    return builder.build()

if __name__ == "__main__":
    log = director(ErrorLogBuilder, "Logger")
    log("this is a system error message")

