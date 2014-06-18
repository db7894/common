#!/usr/bin/env python
from config import config
from shortly.service import create_application
from werkzeug.serving import run_simple

application = create_application(**config)
run_simple('localhost', 8080, application, use_debugger=True, use_reloader=False)
