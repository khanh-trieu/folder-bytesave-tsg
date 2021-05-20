from flask import Flask, jsonify
from flask_cors import CORS
from flask_restful import Resource, Api
from pip._vendor import requests
# from Connect_ByteSave import app_config
# from app_config import API_LIST_BACKUP, API_CHECK_BACKUP, API_DEL_BACKUP
import app_config

# danh sách backup bytesave tai agent
class bytesave_backup(Resource):
    def get(self, serial_number,emailloggin):
        try:
            response = requests.get(app_config.API_LIST_BACKUP + str(serial_number)+'/'+str(emailloggin))
            geodata = response.json()
            load_data = geodata['data']
            countdata = geodata['countdata']
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'countdata': 0, 'data': [], 'error': geodata['error']})
            return jsonify(
                {'status': 'true', 'countdata': countdata, 'data': load_data,'error':'' })
        except Exception as e:
            return jsonify({'status': 'false', 'countdata': 0, 'data': [], 'error': str(e)})


# kiểm tra  connection bytesave khi thêm mới
class bytesave_backup_check(Resource):
    def get(self, serial_number,name):
        try:
            response = requests.get(app_config.API_CHECK_BACKUP + str(serial_number) +'/'+name)
            geodata = response.json()
            if geodata['status'] == 'error':
                return jsonify({'status': 'error', 'msg': '','error': geodata['error']})
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg':geodata['msg'],'error': ''})
            return jsonify({'status': 'true', 'msg':geodata['msg'],'error': '' })
        except Exception as e:
            return jsonify({'status': 'false', 'msg': 'Lỗi hệ thống!', 'error': str(e), })


# thêm mới hoặc chỉnh sửa backup bytesave
class bytesave_backup_create_or_update(Resource):
    def get(self, id, serial_number,emailloggin, id_connect_bytesave, name, local_path, container_name, email,time_delete,time_delete_file_in_LastVersion,time,is_folder):
        try:
            myobj = {'id': id,
                     'id_connect_bytesave': id_connect_bytesave,
                     'name': name,
                     'local_path': local_path,
                     'container_name': container_name,
                     'email': email,
                     'time_delete': time_delete,
                     'time_delete_file_in_LastVersion': time_delete_file_in_LastVersion,
                     'time': time,
                     'is_folder': is_folder,
                     }
            response = requests.post(app_config.API_LIST_BACKUP + str(serial_number)+'/'+str(emailloggin), data=myobj)
            geodata = response.json()
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg': geodata['msg'], 'error': geodata['error']})
            return jsonify({'status': 'true', 'msg': geodata['msg'],'error': ''})
        except Exception as e:
            return jsonify({'status': 'false','msg':'Lỗi hệ thống',  'error':str(e)})

# Xóa backup bytesave
class bytesave_backup_delete(Resource):
    def get(self, id):
        try:
            response = requests.get(app_config.API_DEL_BACKUP + str(id))
            geodata = response.json()
            if geodata['status'] == 'error':
                return jsonify({'status': 'error', 'error': geodata['error']})
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg': geodata['msg']})
            return jsonify({'status': 'true', 'msg':geodata['msg'] })
        except Exception as e:
            return jsonify({'status': 'error', 'msg': 'Lỗi hệ thống!', 'error': str(e), })

