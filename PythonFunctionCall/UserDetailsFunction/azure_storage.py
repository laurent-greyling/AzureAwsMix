from azure.storage.blob import BlobServiceClient
from azure.cosmosdb.table.tableservice import TableService
from azure.cosmosdb.table.models import Entity
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
    
    def writeTable(details):
        entity = AzureStorage.create_table_entity(details)
        table = AzureStorage.create_table_client()
        
        table_name = "userdetails"        
        table.insert_entity(table_name, entity)
        
    def readTable(partitionKey, rowKey):
        table = AzureStorage.create_table_client()
        table_name = "userdetails" 
        return table.get_entity(table_name, partitionKey, rowKey)
    
    def create_blob_client(blobName):
        connection_string = os.environ['AzureWebJobsStorage']
        container_name = "userdetails"
        blob_name = "python-user-" + blobName  
        client = BlobServiceClient.from_connection_string(conn_str=connection_string, container_name=container_name, blob_name=blob_name)
        return client.get_blob_client(container_name, blob_name)
    
    def create_table_client():
        connection_string = os.environ['AzureWebJobsStorage']
        table_service_client = TableService(connection_string=connection_string)
        return table_service_client
    
    def create_table_entity(details):
        entity = Entity()
        entity.PartitionKey = "python-" + details.FirstName
        entity.RowKey = details.Surname + str(details.Id)
        entity.FirstName = details.FirstName
        entity.Surname = details.Surname
        entity.FullName = details.FullName
        return entity