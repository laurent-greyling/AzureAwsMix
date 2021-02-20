from azure.storage.blob import BlobServiceClient
from UserDetailsFunction.json_encoder import JEncoder
import os
import json

class AzureStorage:
    def writeBlob(details):        
        encoded_details = JEncoder().encode(details)
        content = json.dumps(encoded_details)      
        
        blob = AzureStorage.create_blob_client(str(details.Id))
        blob.upload_blob(content)
        
    def readBlob(id):
        blob = AzureStorage.create_blob_client(str(id))
        content = blob.download_blob()
        return content.content_as_bytes()
    
    def create_blob_client(blobName):
        connection_string = os.environ['AzureWebJobsStorage']
        container_name = "userdetails"
        blob_name = "python-user-" + blobName  
        client = BlobServiceClient.from_connection_string(conn_str=connection_string, container_name=container_name, blob_name=blob_name)
        return client.get_blob_client(container_name, blob_name)        