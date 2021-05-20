import itertools
import os
from azure.storage.blob import BlobClient, BlobServiceClient
import time
import json
from datetime import datetime
import sys

from data_api import save_log_agent
# connect_string = 'DefaultEndpointsProtocol=https;AccountName=tsgblobtestbykhanh;AccountKey=46BUwKMiWg\u002BhJx9NSdB46dbL46RmkSLRWnEOkY8aXjASLBFIojsBchdLBdvJCW\u002B2iH91riWgN76gr3ljCXZjdQ==;EndpointSuffix=core.windows.net'


# time_string: day_of_week|hours
def is_allow(str_timer):
    try:
        # timer_array = str_timer.Split('|')
        timer_array = str_timer.split('|')
        day_of_week = timer_array[0].split(',')
        today_of_week = int(datetime.today().weekday())
        print(today_of_week)
        print(day_of_week)
        print(timer_array)
        if str(today_of_week) in day_of_week:
            list_hour = timer_array[1].split(',')
            print(list_hour)
            for item in list_hour:
                now = datetime.now()
                hour = item.split(':')[0]
                minute = item.split(':')[1]
                print(now.hour)
                print(now.minute)
                if int(hour) == now.hour and int(minute) == now.minute:
                    return True
    except Exception as e:
        save_log_agent( e,'is_allow(' + str_timer +')',0)
        print(e)
    return False


def get_file_json():
    ret = dict()
    try:
        with open('c:\\pytest\\appsettings.json') as file_json:
            data = json.load(file_json)
            temp = data['backup_bytesave']
            file_json.close()
            ret['status'] = True
            ret['data'] = temp
    except Exception as e:
        print(e)
        ret['status'] = False
        ret['data'] = []

    return ret


def get_serial_number():
    os_type = sys.platform.lower()
    if "win" in os_type:
        command = "wmic bios get serialnumber"
    elif "linux" in os_type:
        command = "hal-get-property --udi /org/freedesktop/Hal/devices/computer --key system.hardware.uuid"
    elif "darwin" in os_type:
        command = "ioreg -l | grep IOPlatformSerialNumber"
    return os.popen(command).read().replace("\n", "").replace("	", "").replace(" ", "").replace("SerialNumber", "")


async def compare():
    return


def upload_file_to_blob(path_file,file_name, container_name, connect_string):
    ret = dict()
    try:
        blob_service_client = BlobServiceClient.from_connection_string(connect_string)
        blob_client = blob_service_client.get_blob_client(container=container_name, blob=file_name)
        with open(path_file, "rb") as data:
            blob_client.upload_blob(data, blob_type="BlockBlob", length=None, metadata=None, overwrite=True)
        ret['status'] = True
    except Exception as e:
        save_log_agent(e, 'upload_file_to_blob('+path_file + ',' + file_name + ',' + container_name +','+connect_string +')',0)
        ret['status'] = False
    return ret


def move_blob_to_lastversion(connection_string, source_container_name, blob_name):
    try:
        # Create client
        blob_service_client = BlobServiceClient.from_connection_string(connection_string)
        # Create blob client for source blob
        source_blob = BlobClient(
            blob_service_client.url,
            container_name=source_container_name,
            blob_name=blob_name,
        )
        # Create new blob and start copy operation.
        new_blob = blob_service_client.get_blob_client(source_container_name, '.LastVersion/' + blob_name)
        new_blob.start_copy_from_url(source_blob.url)
        return True
    except Exception as e:
        save_log_agent(e, 'move_blob_to_lastversion(' +connection_string+','+source_container_name +','+blob_name+ ')', 0)
        return False


def get_list_file_local(path):
    ret = dict()
    try:
        # path = 'c:\\pytest\\'
        files = []
        # r=root, d=directories, f = files
        for r, d, f in os.walk(path):
            for file in f:
                files.append({
                    'name': os.path.join(r, file).replace(path, '', 1),
                    'last_modified': time.mktime(datetime.strptime(
                        time.strftime('%d/%m/%Y', time.gmtime(os.path.getmtime(os.path.join(r, file)))),
                        "%d/%m/%Y").timetuple()),
                    'create_time': time.mktime(datetime.strptime(
                        time.strftime('%d/%m/%Y', time.gmtime(os.path.getctime(os.path.join(r, file)))),
                        "%d/%m/%Y").timetuple()),
                    'size': os.path.getsize(os.path.join(r, file)),
                })
                print(files)
        ret['status'] = True
        ret['data'] = files
    except Exception as e:
        save_log_agent(e, 'get_list_file_local(' + path + ')', 0)
        ret['status'] = False
        ret['data'] = []
    return ret


def get_list_file_cloud(connect_string, container_name):
    ret = dict()
    try:
        files = []
        blob_service_client = BlobServiceClient.from_connection_string(connect_string)
        container_client = blob_service_client.get_container_client(container_name)
        blob_list = container_client.list_blobs()
        for blob in blob_list:
            if blob['name'].split('/')[0] != '.LastVersion':
                files.append({
                    'name': blob.name.replace('/', '\\'),
                    'last_modified': blob.last_modified.timestamp(),
                    'create_time': blob.creation_time.timestamp(),
                    'size': blob.size,
                })
        ret['status'] = True
        ret['data'] = files
    except Exception as e:
        save_log_agent(e, 'get_list_file_cloud('+connect_string+','+container_name+')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret

def get_list_file_cloud_in_LastVersion(connect_string, container_name):
    ret = dict()
    try:
        files = []
        blob_service_client = BlobServiceClient.from_connection_string(connect_string)
        container_client = blob_service_client.get_container_client(container_name)
        blob_list = container_client.list_blobs()
        for blob in blob_list:
            if blob['name'].split('/')[0] == '.LastVersion':
                files.append({
                    'name': blob.name.replace('/', '\\'),
                    'last_modified': blob.last_modified.timestamp(),
                    'create_time': blob.creation_time.timestamp(),
                    'size': blob.size,
                })
        ret['status'] = True
        ret['data'] = files
    except Exception as e:
        save_log_agent(e, 'get_list_file_cloud('+connect_string+','+container_name+')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret

def get_list_file_cloud(connect_string, container_name):
    ret = dict()
    try:
        files = []
        blob_service_client = BlobServiceClient.from_connection_string(connect_string)
        container_client = blob_service_client.get_container_client(container_name)
        blob_list = container_client.list_blobs()
        for blob in blob_list:
            if blob['name'].split('/')[0] != '.LastVersion':
                files.append({
                    'name': blob.name.replace('/', '\\'),
                    'last_modified': blob.last_modified.timestamp(),
                    'create_time': blob.creation_time.timestamp(),
                    'size': blob.size,
                })
        ret['status'] = True
        ret['data'] = files
    except Exception as e:
        save_log_agent(e, 'get_list_file_cloud('+connect_string+','+container_name+')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret

def get_list_name_file_cloud(list_cloud):
    ret = dict()
    try:
        files = []
        for blob in list_cloud:
            files.append(blob['name'])
        ret['status'] = True
        ret['data'] = files
    except Exception as e:
        save_log_agent( e, 'get_list_name_file_cloud(' + list_cloud + ')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret

def dff_cloud_client(list_client, list_cloud):
    ret = dict()
    try:
        list_difference = [item for item in list_client if item not in list_cloud]

        list_dff = list_client not in list_cloud

        if list_dff == None:
            ret['status'] = False
        else:
            ret['status'] = True
            ret['data'] = list_dff
    except Exception as e:
        print(e)
    return ret

def delete_file_blob(connection_string,source_container_name,blob_name):
    try:
        # Create client
        blob_service_client = BlobServiceClient.from_connection_string(connection_string)
        # # delete blob
        container_client = blob_service_client.get_container_client(source_container_name)
        # await container_client.delete_blobs(blob_name)
        container_client.delete_blobs(blob_name)
    except Exception as e:
        save_log_agent(e, 'delete_file_blob(' + connection_string + ','+source_container_name+','+blob_name+')', 0)
    return

def rename_file(path_file,file_name):
    try:
        path_file = path_file + file_name
        file_name =file_name.split('\\')[len(file_name.split('\\'))-1]
        os.rename(path_file,os.path.dirname(os.path.abspath(path_file))+'\\[DelCloud]_'+file_name)
    except Exception as e:
        return
    return True

