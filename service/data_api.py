import datetime
import os
import sys

import requests

from app_config import BYTESAVE_API_SAVE_LOG
a =os.getenv('BYTESAVE_API_PBL')
API_BYTESAVE_PB = os.getenv('BYTESAVE_API_PBL') if os.getenv('BYTESAVE_API_PBL') != None else 'http://127.0.0.1:8082/'
#API_BYTESAVE_PB = 'http://127.0.0.1:8084/'


def get_serial_number():
    os_type = sys.platform.lower()
    if "win" in os_type:
        command = "wmic bios get serialnumber"
    elif "linux" in os_type:
        command = "hal-get-property --udi /org/freedesktop/Hal/devices/computer --key system.hardware.uuid"
    elif "darwin" in os_type:
        command = "ioreg -l | grep IOPlatformSerialNumber"
    return os.popen(command).read().replace("\n", "").replace("	", "").replace(" ", "").replace("SerialNumber", "")


def get_list_backup():

    ret = dict()
    try:
        print(API_BYTESAVE_PB)
        response = requests.get(API_BYTESAVE_PB + 'sao-luu/danh-sach/' + str(get_serial_number()))
        geodata = response.json()
        load_data = geodata
        ret['status'] = True
        ret['data'] = load_data
    except Exception as e:
        save_log_agent( e, 'get_list_backup', 0)
        # requests.get(
        #     BYTESAVE_API_SAVE_LOG + 'them-log-agent/' + serial_number + '/Customer/' + e + '/' + 'get_list_backup/' + datetime.now().timestamp() + '/0')
        ret['status'] = False
        ret['data'] = []
    return ret

def get_time_ex():
    try:
        response = requests.get(API_BYTESAVE_PB + 'ngay-het-han/' + str(get_serial_number()))
        geodata = response.json()
        load_data = geodata
        return load_data['status']
    except Exception as e:
        return 'false'

def get_detail_connecttion(id):
    ret = dict()
    try:
        response = requests.get(API_BYTESAVE_PB + 'ket-noi/chi-tiet/' + str(id))
        geodata = response.json()
        load_data = geodata
        ret['status'] = True
        ret['data'] = load_data
    except Exception as e:
        # requests.get(BYTESAVE_API_SAVE_LOG+'them-log-agent/' +serial_number+'/Customer/'+e+'/' +'get_detail_connecttion('+id+')/'+datetime.now().timestamp()+'/0')
        save_log_agent(get_serial_number(), e, 'get_detail_connecttion(' + id + ')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret

def save_log_agent( e, function, type):
    try:
        requests.get(
        BYTESAVE_API_SAVE_LOG + 'them-log-agent/' + str(get_serial_number()) + '/Customer/' + str(e).replace('/','\\') + '/' + str(function) + '/0/' + str(datetime.datetime.now().timestamp()) + '/' + str(type))
    except Exception as e:
        print(e)
    return

