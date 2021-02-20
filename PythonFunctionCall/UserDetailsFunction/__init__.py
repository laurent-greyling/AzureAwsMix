import logging

import azure.functions as func
from UserDetailsFunction.create_details import CreateDetails
from UserDetailsFunction.azure_storage import AzureStorage

def main(msg: func.ServiceBusMessage):
    userDetails =  CreateDetails.get(msg)
    AzureStorage.writeBlob(userDetails)
    
    logging.info('Python ServiceBus queue trigger processed message: %s',
                 msg)
