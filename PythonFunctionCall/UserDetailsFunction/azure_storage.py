from azure.storage.blob import BlobServiceClient
from UserDetailsFunction.json_encoder import JEncoder
import os
import json

class AzureStorage:
    def writeBlob(details):
        connection_string = os.environ['AzureWebJobsStorage']
        encoded_details = JEncoder().encode(details)
        content = json.dumps(encoded_details)
        blob_name = "python-user-" + str(details.Id)
        container_name = "userdetails"
        
        client = BlobServiceClient.from_connection_string(conn_str=connection_string, container_name=container_name, blob_name=blob_name)
        blob = client.get_blob_client(container_name, blob_name) 
        blob.upload_blob(content)