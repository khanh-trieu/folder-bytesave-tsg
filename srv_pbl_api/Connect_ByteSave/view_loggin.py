from flask import jsonify
from flask_restful import Resource
from pip._vendor import requests

from app_config import *


class bytesave_loggin(Resource):
    def get(self,email,pwd,serial_number,os,ip_public,ip_private,name_computer):
        try:
            myobj = {'email':email,'pwd':pwd,
                                           'os':os,
                                           'serial_number':serial_number,
                                           'ip_public':ip_public,
                                           'ip_private':ip_private,
                                           'name_computer':name_computer,
                                             }
            response = requests.post(API_LOGGIN,data=myobj)
            msg = response.json()['msg']
            status = response.json()['status']
            return jsonify({'msg': msg,'status':status})
        except Exception as e:
            return jsonify({'msg': msg, 'status': status, 'string_error': str(e)})

class check_logged(Resource):
    def get(self,serial_number):
        try:
            myobj = {'serial_number':serial_number }
            response = requests.post(API_LOGGED,data=myobj)
            msg = response.json()['msg']
            status = response.json()['status']
            if status == 'error':
                return jsonify({'msg': msg, 'status': 'false','error_private':str(response.json()['error'])})
            return jsonify({'msg': msg,'status':status})
        except Exception as e:
            return jsonify({'msg': msg, 'status': 'false','error_public':str(e)})

class bytesave_logout(Resource):
    def get(self,serial_number):
        try:
            myobj = {'serial_number':serial_number }
            response = requests.post(API_LOGOUT,data=myobj)
            msg = response.json()['msg']
            status = response.json()['status']
            if status == 'error':
                return jsonify({'msg': msg, 'status': 'false','error_private':str(response.json()['error'])})
            return jsonify({'msg': msg,'status':status})
        except Exception as e:
            return jsonify({'msg': msg, 'status': 'false','error_public':str(e)})

class bytesave_get_ex_date(Resource):
    def get(self,serial_number):
        try:
            response = requests.get(API_EX_DATE + str(serial_number))
            msg = response.json()['msg']
            status = response.json()['status']
            return jsonify({'msg': msg,'status':status})
        except Exception as e:
            return jsonify({'msg': str(e), 'status': 'false'})