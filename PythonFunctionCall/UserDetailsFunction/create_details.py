import json
from UserDetailsFunction.details import Details

class CreateDetails:
    def get(queueMessage):
        messageBody = queueMessage.get_body().decode('utf-8')
        userProperties = queueMessage.user_properties
        bodyObject = json.loads(messageBody)
        fullName = bodyObject['FirstName'] + " " + bodyObject['Surname']
        return Details(userProperties['Id'], bodyObject['FirstName'], bodyObject['Surname'], fullName)