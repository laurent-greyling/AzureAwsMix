import logging

import azure.functions as func
from PythonFunc.create_details import CreateDetails

def main(msg: func.ServiceBusMessage):
    userDetails =  CreateDetails.get(msg)
    logging.info('Python ServiceBus queue trigger processed message: %s',
                 msg)
