from flask import Flask, jsonify
from flask_cors import CORS
from flask_restful import Resource, Api
from pip._vendor import requests

from view_backup import *
from view_setting import bytesave_setting, create_bytesave_setting, bytesave_information
from view_loggin import bytesave_loggin, check_logged, bytesave_logout, bytesave_get_ex_date
from app_config import API_LIST_CONNECT, API_DETAIL_CONNECT, API_AGENT_CREATE_UPDATE_METRIC_SERVICE, \
    API_AGENT_CREATE_UPDATE_METRIC_SERVICE_CHECK, API_LIST_BACKUP, API_CHECK_CONNECT, API_DEL_CONNECT

app = Flask(__name__)
api = Api(app)
CORS(app)



# danh sách connection bytesave tai agent
class bytesave_connect(Resource):
    def get(self, serial_number,email_loggin):
        try:
            response = requests.get(API_LIST_CONNECT + str(serial_number)+'/'+str(email_loggin))
            geodata = response.json()
            load_data = geodata['data']
            countdata = geodata['countdata']
            return jsonify({'status': 'true', 'countdata': countdata, 'data': load_data})
        except Exception as e:
            return jsonify({'status': 'false', 'countdata': 0, 'data': []})


# Chi tiết của connection bytesave
class bytesave_connect_detail(Resource):
    def get(self, id):
        try:
            response = requests.get(API_DETAIL_CONNECT + str(id))
            geodata = response.json()
            load_data = geodata['data']
            return jsonify({'status': 'true', 'data': load_data})
        except Exception as e:
            return jsonify({'status': 'false', 'data': [], 'error_public': str(e), })


# kiểm tra  connection bytesave khi thêm mới
class bytesave_connect_check(Resource):
    def get(self, serial_number,name):
        try:
            response = requests.get(API_CHECK_CONNECT + str(serial_number) +'/'+name)
            geodata = response.json()
            if geodata['status'] == 'error':
                return jsonify({'status': 'error', 'error_private': geodata['error']})
            return jsonify({'status': 'true', 'msg':geodata['msg'] })
        except Exception as e:
            return jsonify({'status': 'false', 'msg': 'Lỗi hệ thống!', 'error_public': str(e), })
# Xóa conect bytesave
class bytesave_connect_delete(Resource):
    def get(self, id):
        try:
            response = requests.get(API_DEL_CONNECT + str(id))
            geodata = response.json()
            if geodata['status'] == 'error':
                return jsonify({'status': 'error', 'error_private': geodata['error']})
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg': geodata['msg']})
            return jsonify({'status': 'true', 'msg':geodata['msg'] })
        except Exception as e:
            return jsonify({'status': 'error', 'msg': 'Lỗi hệ thống!', 'error_public': str(e), })


# thêm mới hoặc chỉnh sửa connection bytesave
class bytesave_connect_create_or_update(Resource):
    def get(self, id, serial_number,email_loggin, id_service, information_connect, max_storage, username_account, name):
        try:
            # response = requests.get(API_LIST_CONNECT + str(serial_number)
            myobj = {'id': id,
                     'id_service': id_service,
                     'information_connect': information_connect,
                     'max_storage': max_storage,
                     'username_account': username_account,
                     'type': 0,
                     'name': name,
                     }
            response = requests.post(API_LIST_CONNECT + str(serial_number) +'/'+str(email_loggin), data=myobj)
            geodata = response.json()
            if geodata['status'] == 'false':
                return jsonify({'status': 'false', 'msg': geodata['msg'], 'error_private': geodata['error']})
            return jsonify({'status': 'true', 'msg': geodata['msg']})
        except Exception as e:
            return jsonify({'status': 'false','msg':'Lỗi hệ thống',  'error_private':str(e)})

class del_connect_bytesave():
    def get(self):
        return

# thêm mới hoặc chỉnh sửa metric_service được tạo dưới agent
class bytesave_create_or_update_metric_service(Resource):
    def post(self, id_ser, serial_number, id_service, information_connect, max_storage, username_account):
        myobj = {'id_ser': id_ser, 'serial_number': serial_number,
                 'id_service_s': id_service,
                 'information_connect': information_connect,
                 'max_storage': max_storage,
                 'username_account': username_account
                 }

        check_response = requests.post(API_AGENT_CREATE_UPDATE_METRIC_SERVICE_CHECK, data=myobj)
        if check_response.json()['status'] == 'false':
            msg = check_response.json()['msg']
            return jsonify({'msg': msg})
        response = requests.post(API_AGENT_CREATE_UPDATE_METRIC_SERVICE, data=myobj)
        msg = response.json()['msg']
        status = response.json()['status']
        return jsonify({'msg': msg, 'status': status})


class Tracks(Resource):
    def get(self):
        return jsonify()


class Employees_Name(Resource):
    def get(self, employee_id):
        result = []
        # query = conn.execute("select * from employees where EmployeeId =%d " % int(employee_id))
        # result = {'data': [dict(zip(tuple(query.keys()), i)) for i in query.cursor]}
        return jsonify(result)

class home(Resource):
    def get(self):
        return "hello world!"
api.add_resource(home, '/hello')

api.add_resource(bytesave_get_ex_date, '/ngay-het-han/<string:serial_number>')

api.add_resource(bytesave_information, '/thong-tin/<string:serial_number>/<string:email_loggin>')

api.add_resource(bytesave_setting, '/cai-dat-chung/<string:serial_number>/<string:email_loggin>')
api.add_resource(create_bytesave_setting, '/cai-dat-chung/them-moi/<int:id>/<int:id_agent>/<string:serial_number>/<string:email_loggin>/<string:server_mail>/<string:port>/<string:mail_send>/<string:mail_send_pwd>/<string:subject>')


api.add_resource(bytesave_backup, '/sao-luu/danh-sach/<string:serial_number>/<string:emailloggin>')
api.add_resource(bytesave_backup_check, '/sao-luu/kiem-tra/<string:serial_number>/<string:name>')
api.add_resource(bytesave_backup_delete, '/sao-luu/xoa/<int:id>')
# id=0: thêm mới; id != 0: chỉnh sửa
api.add_resource(bytesave_backup_create_or_update,'/sao-luu/them-moi/<int:id>/<string:serial_number>/<string:emailloggin>/<int:id_connect_bytesave>/<string:name>/<string:local_path>/<string:container_name>/<string:email>/<string:time_delete>/<string:time_delete_file_in_LastVersion>/<string:time>/<string:is_folder>')


api.add_resource(bytesave_connect, '/ket-noi/danh-sach/<string:serial_number>/<string:email_loggin>')
api.add_resource(bytesave_connect_detail, '/ket-noi/chi-tiet/<int:id>')
api.add_resource(bytesave_connect_check, '/ket-noi/kiem-tra/<string:serial_number>/<string:name>')
api.add_resource(bytesave_connect_delete, '/ket-noi/xoa/<int:id>')
# id=0: thêm mới; id != 0: chỉnh sửa
api.add_resource(bytesave_connect_create_or_update,'/ket-noi/them-moi/<int:id>/<int:id_service>/<string:serial_number>/<string:email_loggin>/<string:information_connect>/<string:max_storage>/<string:username_account>/<string:name>')

api.add_resource(bytesave_loggin,'/dang-nhap/<string:email>/<string:pwd>/<string:os>/<string:serial_number>/<string:ip_public>/<string:ip_private>/<string:name_computer>')
api.add_resource(check_logged, '/dang-nhap/kiem-tra-logged/<string:serial_number>')
api.add_resource(bytesave_logout, '/dang-xuat/<string:serial_number>')

api.add_resource(bytesave_create_or_update_metric_service, '/them-moi-dich-vu-luu-tru/<int:id_ser>/<string:serial_number>/<int:id_service>/<string:information_connect>/<int:max_storage>/<string:username_account>')

api.add_resource(Tracks, '/tracks')  # Route_2
api.add_resource(Employees_Name, '/employees/<employee_id>')  # Route_3

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8082)
