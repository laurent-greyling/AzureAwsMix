import logging

import azure.functions as func
from UserDetailsFunction.create_details import CreateDetails
from UserDetailsFunction.azure_storage import AzureStorage

def main(msg: func.ServiceBusMessage):
    userDetails =  CreateDetails.get(msg)
    
    #Azure Blob Storage Code
    AzureStorage.writeBlob(userDetails)
    blob_content = AzureStorage.readBlob(userDetails.Id).decode("utf-8")
    
    #Azure Table Storage
    AzureStorage.writeTable(userDetails)
    
    logging.info('Python ServiceBus queue trigger processed message: %s', msg)