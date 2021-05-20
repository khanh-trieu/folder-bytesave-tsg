import os
import threading
from azure.storage.blob import BlobClient, BlobServiceClient
import time
import json
import requests
import smtplib, ssl
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from datetime import datetime
import sys
import win32serviceutil  # ServiceFramework and commandline helper
import win32service  # Events
import servicemanager  # Simple setup and logging

BYTESAVE_API_SAVE_LOG = 'http://13.212.85.136:5000/'

email_loggin = ''

file_local = 'C:\\Program Files (x86)\\ByteSave\\ByteSave\\'
file_path_local_history = file_local + 'historys.json'
file_path_local_appsettings = file_local + 'appsettings.json'
file_path_local_logs = file_local + 'log_service.txt'


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
        save_log_agent(e, 'is_allow(' + str_timer + ')', 0)
        print(e)
    return False


def get_serial_number():
    os_type = sys.platform.lower()
    if "win" in os_type:
        command = "wmic bios get serialnumber"
    elif "linux" in os_type:
        command = "hal-get-property --udi /org/freedesktop/Hal/devices/computer --key system.hardware.uuid"
    elif "darwin" in os_type:
        command = "ioreg -l | grep IOPlatformSerialNumber"
    return os.popen(command).read().replace("\n", "").replace("	", "").replace(" ", "").replace("SerialNumber", "")


def upload_file_to_blob(path_file, file_name, container_name, connect_string):
    ret = dict()
    try:
        blob_service_client = BlobServiceClient.from_connection_string(connect_string)
        blob_client = blob_service_client.get_blob_client(container=container_name, blob=file_name)
        with open(path_file, "rb") as data:
            blob_client.upload_blob(data, blob_type="BlockBlob", length=None, metadata=None, overwrite=True)
        ret['status'] = True
    except Exception as e:
        save_log_agent(e,
                       'upload_file_to_blob(' + path_file + ',' + file_name + ',' + container_name + ',' + connect_string + ')',
                       0)
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
        save_log_agent(e,
                       'move_blob_to_lastversion(' + connection_string + ',' + source_container_name + ',' + blob_name + ')',
                       0)
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
        save_log_agent(e, 'get_list_file_cloud(' + connect_string + ',' + container_name + ')', 0)
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
        save_log_agent(e, 'get_list_file_cloud(' + connect_string + ',' + container_name + ')', 0)
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
        save_log_agent(e, 'get_list_file_cloud(' + connect_string + ',' + container_name + ')', 0)
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
        save_log_agent(e, 'get_list_name_file_cloud(' + list_cloud + ')', 0)
        ret['status'] = True
        ret['data'] = []
    return ret


def delete_file_blob(connection_string, source_container_name, blob_name):
    try:
        # Create client
        blob_service_client = BlobServiceClient.from_connection_string(connection_string)
        # # delete blob
        container_client = blob_service_client.get_container_client(source_container_name)
        # await container_client.delete_blobs(blob_name)
        container_client.delete_blobs(blob_name)
    except Exception as e:
        save_log_agent(e, 'delete_file_blob(' + connection_string + ',' + source_container_name + ',' + blob_name + ')',
                       0)
    return


def rename_file(path_file, file_name):
    try:
        path_file = path_file + file_name
        file_name = file_name.split('\\')[len(file_name.split('\\')) - 1]
        os.rename(path_file, os.path.dirname(os.path.abspath(path_file)) + '\\[DelCloud]_' + file_name)
    except Exception as e:
        return
    return True


def get_serial_number():
    os_type = sys.platform.lower()
    if "win" in os_type:
        command = "wmic bios get serialnumber"
    elif "linux" in os_type:
        command = "hal-get-property --udi /org/freedesktop/Hal/devices/computer --key system.hardware.uuid"
    elif "darwin" in os_type:
        command = "ioreg -l | grep IOPlatformSerialNumber"
    return os.popen(command).read().replace("\n", "").replace("	", "").replace(" ", "").replace("SerialNumber", "")


def get_time_ex(data):
    try:
        if int(data['bytesave_expiration_date']) > int(datetime.now().timestamp()):
            return True
    except Exception as e:
        return False
    return False

def save_log_agent(e, function, type):
    try:
        requests.get(
            BYTESAVE_API_SAVE_LOG + 'them-log-agent/' + str(get_serial_number()) + '/' + str(email_loggin) + '/' + str(
                e).replace('/', '\\') + '/[Service]' + str(
                function) + ' /0/' + str(datetime.now().timestamp()) + '/' + str(type))
    except Exception as e:
        print(e)
    return


import base64
decoded_data = str(base64.b64decode(""),'utf-8')


def write_log(function, status, time_log, log_content):
    with open(file_path_local_history) as json_file:
        data = json.load(json_file)

    data['history']['history_bytesave'].append({
        'function': function,
        'status': status,
        'time_log': time_log,
        'log_content': log_content,
    })
    with open(file_path_local_history, 'w') as outfile:
        json.dump(data, outfile)


def write_logs_service(log_content):
    try:
        file_object = open(file_path_local_logs, 'a')
        # Append 'hello' at the end of file
        file_object.write(str(log_content.encode("utf-8")))
        # Close the file
        file_object.close()
    except Exception as e:
        return


now = datetime.now()


def send_mail(is_folder, name_backup, count_file, path_file, list_file_upload, email_receiver,sender_email,password,host,port,subject):
    sender_email = sender_email
    password = str(password,'utf-8')
    host = host
    port = int(port) if port != '' else ''
    message = MIMEMultipart("alternative")
    message["Subject"] = subject
    message["From"] = sender_email

    text1 = 'Tác vụ sao lưu {name_backup} đã thực hiện sao lưu file: {path_file}'
    text_folder = 'Tác vụ sao lưu {name_backup} đã thực hiện sao lưu {count_file_backup} trong folder {path_file} : {list_file_upload}'
    write_log(function='Sao lưu', status=1, time_log=datetime.now().timestamp(),
              log_content=(text_folder if is_folder == 1 else text1).replace('{name_backup}', name_backup).replace(
                  '{count_file_backup}', str(count_file))
              .replace('{list_file_upload}', str(list_file_upload)).replace('{path_file}', path_file))

    try:
        # Create the plain-text and HTML version of your message
        html = """\
        <html>
          <body>
            <p>ByteSave thông báo hoạt động [{time_run}]<br>
               Tác vụ sao lưu {name_backup} đã thực hiện sao lưu file: {path_file}
              <br><br><br> Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
            </p>
          </body>
        </html>
        """
        html_folder = """\
                <html>
                  <body>
                    <p>ByteSave thông báo hoạt động [{time_run}]<br>
                       Tác vụ sao lưu {name_backup} đã thực hiện sao lưu {count_file_backup} trong folder {path_file} : {list_file_upload}
                       <br> <br> <br>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
                    </p>
                  </body>
                </html>
                """
        time_run = now.strftime("%d/%m/%Y %H:%M")
        part = MIMEText(
            (html_folder if is_folder == 1 else html).replace('{name_backup}', name_backup).replace('{time_run}', str(
                time_run)).replace('{count_file_backup}', str(count_file))
                .replace('{list_file_upload}', str(list_file_upload)).replace('{path_file}', path_file), "html")

        # message = MIMEMultipart("alternative")
        # message["Subject"] = subject
        # message["From"] = sender_email
        message.attach(part)
        # Create secure connection with server and send email
        context = ssl.create_default_context()
        for item in email_receiver.split(','):
            with smtplib.SMTP(host, port) as server:
                server.ehlo()  # Can be omitted
                server.starttls(context=context)
                server.ehlo()  # Can be omitted
                server.login(sender_email, password)
                server.sendmail(sender_email, item, message.as_string())

    except Exception as e:
        print(e)

def process(item,data,sender_email,password,host,port,subject):
    detail_connection = None
    for item_connect in data['Settings']['connect_bytesaves']:
        if item_connect['id'] == item['id_connect_bytesave']:
            detail_connection = item_connect
            continue
    list_file_upload = []
    # detail_connection = get_detail_connecttion(item['id_connect_bytesave'])
    list_file_cloud = get_list_file_cloud(
        detail_connection['metric_service_information_connect'],
        item['container_name'])
    list_name_file_cloud = get_list_name_file_cloud(list_file_cloud['data'])
    if item['is_folder'] == 1:  # local path là folder
        list_file_local = get_list_file_local(item['local_path'])
        for item_file_local in list_file_local['data']:
            write_logs_service('item_file_local[name] -> ' + str(item_file_local['name']))
            if len(item_file_local['name']) > 10 and item_file_local['name'].split('\\')[len(
                    item_file_local['name'].split('\\')) - 1][0:10] == '[DelCloud]':
                continue
            else:
                name_folder_upload = item['local_path'].split('\\')[
                    len(item['local_path'].split('\\')) - 1]
                item_file_local_after = str(name_folder_upload) + str(item_file_local['name'])
                # Kiểm tra xem file đã có trên Blob hay chưa?
                is_exist_file_in_cloud = item_file_local_after in list_name_file_cloud['data']
                if is_exist_file_in_cloud == True:  # Đã có file trên Blob
                    enumber_rate = list_name_file_cloud['data'].index(
                        item_file_local_after)
                    file_cloud = list_file_cloud['data'][enumber_rate]
                    # Kiểm tra xem có sự thay đổi của file mới hay không?
                    if file_cloud['size'] != item_file_local['size']:
                        # Chuyển file cũ sang folder .LastVersion khi file backup lên đã có trên Blob
                        move_blob_to_lastversion(
                            detail_connection['metric_service_information_connect'],
                            item['container_name'], item_file_local_after)
                        upload_file_to_blob(
                            item['local_path'] + item_file_local['name'],
                            item_file_local_after,
                            item['container_name'],
                            detail_connection['metric_service_information_connect'])

                        list_file_upload.append(item_file_local_after)
                    else:
                        if item['time_delete'] > 0:
                            if file_cloud['last_modified'] + (
                                    item['time_delete'] * 86400) < datetime.now().timestamp():
                                rename_file(path_file=item['local_path'],
                                            file_name=item_file_local[
                                                'name'])  # rename file local when delete file in cloud
                                delete_file_blob(
                                    detail_connection['metric_service_information_connect'],
                                    item['container_name'],
                                    item_file_local['name'])
                            else:
                                a = 0
                        else:
                            a = 0
                else:  # Chưa có file trên Blob thì Upload lên Blob luôn
                    upload_file_to_blob(
                        item['local_path'] + item_file_local['name'],
                        item_file_local_after,
                        item['container_name'],
                        detail_connection['metric_service_information_connect'])
                    list_file_upload.append(item_file_local['name'])
        if list_file_upload.__len__() > 0:
            send_mail(item['is_folder'], item['name'], list_file_upload.__len__(),
                      item['local_path'], list_file_upload, item['email'], sender_email, password, host,
                      port, subject)
    else:  # local path is file
        file_name = item['local_path'].split('\\')[len(item['local_path'].split('\\')) - 1]
        # Kiểm tra xem file đã có trên Blob hay chưa?
        is_exist_file_in_cloud = file_name in list_name_file_cloud['data']
        size_file_local = os.path.getsize(item['local_path'])
        if is_exist_file_in_cloud == True:  # FIle đã có trên Blob
            enumber_rate = list_name_file_cloud['data'].index(
                file_name)
            file_cloud = list_file_cloud['data'][enumber_rate]
            if file_cloud['size'] != size_file_local:
                # Chuyển file cũ sang folder .LastVersion khi file backup lên đã có trên Blob
                move_blob_to_lastversion(detail_connection[
                                             'metric_service_information_connect'],
                                         item['container_name'], file_name)

                upload_file_to_blob(item['local_path'],
                                    file_name,
                                    item['container_name'],
                                    detail_connection['metric_service_information_connect'])
                send_mail(item['is_folder'], item['name'], '1', item['local_path'], [],
                          item['email'], sender_email, password, host, port, subject)
            else:
                if item['time_delete'] > 0:
                    if file_cloud['last_modified'] + (
                            item['time_delete'] * 86400) < datetime.now().timestamp():
                        rename_file(path_file=item['local_path'],
                                    file_name=file_name),  # rename file local when delete file in cloud
                        delete_file_blob(
                            detail_connection['metric_service_information_connect'],
                            item['container_name'], item['local_path'])

        else:
            upload_file_to_blob(path_file=item['local_path'],
                                file_name=file_name,
                                container_name=item['container_name'],
                                connect_string=detail_connection[
                                    'metric_service_information_connect'])
            send_mail(item['is_folder'], item['name'], '1', item['local_path'], [],
                      item['email'], sender_email, password, host, port, subject)
    list_file_cloud_in_LastVersion = get_list_file_cloud_in_LastVersion(
        connect_string=detail_connection[
            'metric_service_information_connect'], container_name=item['container_name'])
    if item['time_delete_file_in_LastVersion'] > 0:
        for item_file in list_file_cloud_in_LastVersion['data']:
            if (item_file['last_modified'] + (
                    item['time_delete_file_in_LastVersion'] * 86400)) < datetime.now().timestamp():
                delete_file_blob(detail_connection['metric_service_information_connect'],
                                 item['container_name'], item_file['name'])


class WorkerThread(threading.Thread):
    def __init__(self,item,data,sender_email,password,host,port,subject):
        super().__init__()
        self.item = item
        self.data = data
        self.sender_email = sender_email
        self.password = password
        self.host = host
        self.port = port
        self.subject = subject

    def run(self) -> None:
        process(self.item,self.data,self.sender_email,self.password,self.host,self.port,self.subject)


def main_process():
    print('start service')
    try:
        with open(file_path_local_appsettings) as json_file:
            data = json.load(json_file)
        sender_email = data['Settings']['bytesave_setting']['set_bytesave']['mail_send'] if \
        data['Settings']['bytesave_setting']['set_bytesave']['mail_send'] != None else ''
        password = base64.b64decode(data['Settings']['bytesave_setting']['set_bytesave']['mail_send_pwd'] if data['Settings']['bytesave_setting']['set_bytesave']['mail_send_pwd'] != None else '')
        host = data['Settings']['bytesave_setting']['set_bytesave']['server_mail'] if \
        data['Settings']['bytesave_setting']['set_bytesave']['server_mail'] != None else ''
        port = data['Settings']['bytesave_setting']['set_bytesave']['port'] if \
        data['Settings']['bytesave_setting']['set_bytesave']['port'] != None else ''
        subject = data['Settings']['bytesave_setting']['set_bytesave']['subject'] if \
        data['Settings']['bytesave_setting']['set_bytesave']['subject'] != None else ''

        check_time_ex = get_time_ex(data['Settings']['bytesave_info']['information'])
        print('check_time_ex'+'---->'+ str(check_time_ex))
        if check_time_ex == False:
            a = 0
            return
        list_backup = data['Settings']['backup_bytesaves']
        if len(list_backup) > 0:
            for item in list_backup:
                rets = is_allow(item['time'])
                print(threading.currentThread().getName(), 'Starting')
                write_logs_service('rets --> ' + str(rets))
                # rets = True
                if rets == True:
                    print('start job')
                    thread = WorkerThread(item,data,sender_email,password,host,port,subject)
                    thread.start()
                    thread.join()
        else:
            print('Job chưa đến thời gian thực hiện!')
    except Exception as e:
        save_log_agent(e, 'main()', 0)
        return
class MyService:
    """Silly little application stub"""
    def stop(self):
        """Stop the service"""
        self.running = False

    def run(self):
        """Main service loop. This is where work is done!"""
        self.running = True
        while self.running:
            # threa1 = my_thread('Thread 1')
            # threa1.start()
            main_process()
            #time.sleep(10)  # Important work
            servicemanager.LogInfoMsg("Service running...")



class MyServiceFramework(win32serviceutil.ServiceFramework):

    _svc_name_ = "bytesave_service_backup"
    _svc_display_name_ = "bytesave_service_backup"
    _svc_description_ = "service backup"

    def SvcStop(self):
        """Stop the service"""
        self.ReportServiceStatus(win32service.SERVICE_STOP_PENDING)
        self.service_impl.stop()
        self.ReportServiceStatus(win32service.SERVICE_STOPPED)

    def SvcDoRun(self):
      """Start the service; does not return until stopped"""
      self.ReportServiceStatus(win32service.SERVICE_START_PENDING)
      self.service_impl = MyService()
      self.ReportServiceStatus(win32service.SERVICE_RUNNING)
      # Run the service
      self.service_impl.run()


def init():
    if len(sys.argv) == 1:
        servicemanager.Initialize()
        servicemanager.PrepareToHostSingle(MyServiceFramework)
        servicemanager.StartServiceCtrlDispatcher()
    else:
        win32serviceutil.HandleCommandLine(MyServiceFramework)

if __name__ == '__main__':
    init()
