from flask import Flask, jsonify
from flask_restful import Resource
from pip._vendor import requests

from app_config import  API_SETTING, API_INFO


class bytesave_setting(Resource):
    def get(self, serial_number,email_loggin):
        try:
            response = requests.get(API_SETTING +'0/'+ str(serial_number)+'/'+ str(email_loggin))
            geodata = response.json()
            load_data = geodata['data']
            countdata = geodata['countdata']
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'countdata': 0, 'data': [], 'error': geodata['error']})
            return jsonify(
                {'status': 'true', 'countdata': countdata, 'data': load_data,'error':'' })
        except Exception as e:
            return jsonify({'status': 'false', 'countdata': 0, 'data': [], 'error': str(e)})

class create_bytesave_setting(Resource):
    def get(self, id,id_agent, serial_number,email_loggin,server_mail,port,mail_send,mail_send_pwd,subject ):
        try:
            myobj = {'id': id,
                     'id_agent': id_agent,
                     'server_mail': server_mail,
                     'port': port,
                     'mail_send': mail_send,
                     'mail_send_pwd': mail_send_pwd,
                     'subject': subject,
                     }
            response = requests.post(API_SETTING +str(id)+'/'+ str(serial_number)+'/'+ str(email_loggin), data=myobj)
            geodata = response.json()
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg': geodata['msg'], 'error': geodata['error']})
            return jsonify({'status': 'true', 'msg': geodata['msg'], 'error': ''})
        except Exception as e:
            return jsonify({'status': 'false', 'msg': 'Lỗi hệ thống', 'error': str(e)})

class bytesave_information(Resource):
    def get(self, serial_number,email_loggin):
        try:
            response = requests.get(API_INFO + str(serial_number)+ '/'+str(email_loggin))
            geodata = response.json()
            load_data = geodata['data']
            return jsonify(
                {'status': geodata['status'], 'countdata': geodata['countdata'], 'data': load_data })
        except Exception as e:
            return jsonify({'status': 'false', 'countdata': 0,'data': []})