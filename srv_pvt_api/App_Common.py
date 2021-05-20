import datetime
import hashlib
import random
from calendar import SATURDAY, SUNDAY
from django.utils import timezone
from azure.storage.blob import BlobServiceClient


class Timer(object):
    @classmethod
    def get_timestamp_now(cls):
        now = timezone.now()
        return int(datetime.datetime.timestamp(now))

    @classmethod
    def get_today(cls):
        now = timezone.now()
        return now.date()

    @classmethod
    def is_weekend(cls):
        weekend = timezone.now().weekday()
        if weekend == SATURDAY or weekend == SUNDAY:
            return True
        else:
            return False

    @classmethod
    def get_hour(cls):
        return timezone.now().hour

    @classmethod
    def get_current_month(cls):
        return timezone.now().month

    @classmethod
    def get_current_year(cls):
        return timezone.now().year

    @classmethod
    def get_timestamp(cls,timedate):
        date = datetime.datetime.strptime(timedate, "%Y/%m/%d")
        at = datetime.datetime.timestamp(date)
        return int(at)

def Convert_timestamp(timestamp):
    try:
        ts = datetime.datetime.fromtimestamp(float(timestamp)).strftime("%d/%m/%Y")
    except Exception as e:
        return ''
    return ts

def GetCapacity(conn_str):
    size = 0
    try:
        # conn_str = 'DefaultEndpointsProtocol=https;AccountName=tsgblobtestbykhanh;AccountKey=46BUwKMiWg+hJx9NSdB46dbL46RmkSLRWnEOkY8aXjASLBFIojsBchdLBdvJCW+2iH91riWgN76gr3ljCXZjdQ==;EndpointSuffix=core.windows.net'
        blob_service_client = BlobServiceClient.from_connection_string(conn_str=conn_str.strip())
        all_containers = blob_service_client.list_containers(include_metadata=True)
        for con in all_containers:
            container = blob_service_client.get_container_client(container=con.name)
            for blob in container.list_blobs():
                size += blob.size
    except Exception as e:
        print(e)
        size = 0
    return size

def genpwd(leng):
    stringcode = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
    return (''.join(random.choice(stringcode) for i in range(leng)))

def encryption(text):
    result = hashlib.sha1(text.encode()).hexdigest()
    return result