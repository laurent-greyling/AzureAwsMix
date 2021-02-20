import json
from json import JSONEncoder

class JEncoder(JSONEncoder):
    def default(self, o):
        return o.__dict__