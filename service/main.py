import asyncio
import os
from datetime import datetime

from SMWinservice import SMWinservice
from backup_blob import is_allow, get_list_file_cloud, get_list_file_local, \
    upload_file_to_blob, delete_file_blob, rename_file, move_blob_to_lastversion, \
    get_list_name_file_cloud, get_list_file_cloud_in_LastVersion
from data_api import get_list_backup, get_detail_connecttion, save_log_agent, get_time_ex
from send_mail import send_mail
import logging
logging.basicConfig(filename="std.log",
					format='%(asctime)s %(message)s',
					filemode='w')
#Let us Create an object
logger=logging.getLogger()

#Now we are going to Set the threshold of logger to DEBUG
logger.setLevel(logging.DEBUG)
# DDaonj này cho main chạy
# class PythonCornerExample(SMWinservice):
#     _svc_name_ = "bytesave_service_backup"
#     _svc_display_name_ = "bytesave_service_backup"
#     _svc_description_ = "service backup"
#
#     def start(self):
#         self.isrunning = True
#
#     def stop(self):
#         self.isrunning = False
#
#     def main(self):
#         print('start service')
#         loop = asyncio.get_event_loop()
#         tasks = [
#             main_process()
#         ]
#         loop.run_until_complete(asyncio.wait(tasks))
#         loop.close()
# if __name__ == '__main__':
#     PythonCornerExample.parse_command_line()

def main_process():
    print('start service')
    logger.info("main_process")
    while True:
        # rets = is_allow('1,2,3,4,5,6,0|17:28,17:29,17:30,17:31')
        try:
            check_time_ex = get_time_ex()
            if check_time_ex =='false':
                continue
            list_backup = get_list_backup()
            # loop = asyncio.get_event_loop()
            # tasks = [
            if list_backup['status'] == True and list_backup['data']['countdata'] > 0:
                print('start job')
                for item in list_backup['data']['data']:
                    rets = is_allow(item['time'])
                    rets = True
                    if rets == True:
                        list_file_upload = []
                        detail_connection = get_detail_connecttion(item['id_connect_bytesave'])
                        list_file_cloud = get_list_file_cloud(
                            detail_connection['data']['data'][0]['metric_service_information_connect'],
                            item['container_name'])
                        list_name_file_cloud = get_list_name_file_cloud(list_file_cloud['data'])
                        if item['is_folder'] == 1:  # local path là folder
                            list_file_local = get_list_file_local(item['local_path'])
                            for item_file_local in list_file_local['data']:
                                if len(item_file_local['name']) > 10 and item_file_local['name'].split('\\')[len(
                                        item_file_local['name'].split('\\')) - 1][0:10] == '[DelCloud]':
                                    continue
                                else:
                                    # Kiểm tra xem file đã có trên Blob hay chưa?
                                    is_exist_file_in_cloud = item_file_local['name'] in list_name_file_cloud['data']
                                    if is_exist_file_in_cloud == True:  # Đã có file trên Blob
                                        enumber_rate = list_name_file_cloud['data'].index(
                                            item_file_local['name'])
                                        file_cloud = list_file_cloud['data'][enumber_rate]
                                        # Kiểm tra xem có sự thay đổi của file mới hay không?
                                        if file_cloud['size'] != item_file_local['size']:
                                            # loop2 = asyncio.get_event_loop()
                                            # tasks2 = [
                                            # Chuyển file cũ sang folder .LastVersion khi file backup lên đã có trên Blob
                                            move_blob_to_lastversion(
                                                detail_connection['data']['data'][0][
                                                    'metric_service_information_connect'],
                                                item['container_name'], item_file_local['name']),
                                            # Backup file lên Blob
                                            upload_file_to_blob(
                                                item['local_path'] + item_file_local['name'],
                                                item_file_local['name'],
                                                item['container_name'],
                                                detail_connection['data']['data'][0][
                                                    'metric_service_information_connect'])
                                            # ]
                                            # loop2.run_until_complete(asyncio.wait(tasks2))
                                            # loop2.close()
                                            list_file_upload.append(item_file_local['name'])
                                        else:
                                            if item['time_delete'] > 0:
                                                if file_cloud['last_modified'] + (
                                                        item['time_delete'] * 86400) < datetime.now().timestamp():
                                                    rename_file(path_file=item['local_path'],
                                                                file_name=item_file_local[
                                                                    'name'])  # rename file local when delete file in cloud
                                                    delete_file_blob(detail_connection['data']['data'][0][
                                                                         'metric_service_information_connect'],
                                                                     item['container_name'],
                                                                     item_file_local['name'])
                                                else:
                                                    a = 0
                                            else:
                                                a = 0
                                    else:  # Chưa có file trên Blob thì Upload lên Blob luôn
                                        # loop2 = asyncio.get_event_loop()
                                        # tasks2 = [
                                        upload_file_to_blob(
                                            item['local_path'] + item_file_local['name'],
                                            item_file_local['name'],
                                            item['container_name'],
                                            detail_connection['data']['data'][0][
                                                'metric_service_information_connect'])
                                        # ]
                                        # loop2.run_until_complete(asyncio.wait(tasks2))
                                        # loop2.close()
                                        list_file_upload.append(item_file_local['name'])
                            if list_file_upload.__len__() > 0:
                                send_mail(item['is_folder'], item['name'], list_file_upload.__len__(),
                                          item['local_path'], list_file_upload,item['email'])

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
                                    # loop2 = asyncio.get_event_loop()
                                    # tasks2 = [
                                    # Chuyển file cũ sang folder .LastVersion khi file backup lên đã có trên Blob
                                    move_blob_to_lastversion(detail_connection['data']['data'][0][
                                                                 'metric_service_information_connect'],
                                                             item['container_name'], file_name),
                                    upload_file_to_blob(item['local_path'],
                                                        file_name,
                                                        item['container_name'],
                                                        detail_connection['data']['data'][0][
                                                            'metric_service_information_connect']),
                                    send_mail(item['is_folder'], item['name'], '1', item['local_path'], [],item['email'])
                                # ]
                                # loop2.run_until_complete(asyncio.wait(tasks2))
                                # loop2.close()

                                else:
                                    if item['time_delete'] > 0:
                                        if file_cloud['last_modified'] + (
                                                item['time_delete'] * 86400) < datetime.now().timestamp():
                                            # loop2 = asyncio.get_event_loop()
                                            # tasks2 = [
                                            rename_file(path_file=item['local_path'], file_name=item_file_local[
                                                'name']),  # rename file local when delete file in cloud
                                            delete_file_blob(detail_connection['data']['data'][0][
                                                                 'metric_service_information_connect'],
                                                             item['container_name'], item['local_path'])
                                        # ]
                                        # loop2.run_until_complete(asyncio.wait(tasks2))
                                        # loop2.close()

                            else:
                                # loop2 = asyncio.get_event_loop()
                                # tasks2 = [
                                upload_file_to_blob(path_file=item['local_path'],
                                                    file_name=file_name,
                                                    container_name=item['container_name'],
                                                    connect_string=detail_connection['data']['data'][0][
                                                        'metric_service_information_connect']),
                                send_mail(item['is_folder'], item['name'], '1', item['local_path'], [],item['email'])
                            # ]
                            # loop2.run_until_complete(asyncio.wait(tasks2))
                            # loop2.close()

                            # if item['time_delete'] > 0:
                            #     if item['time_create_at'] + (
                            #             item['time_delete'] * 86400) < datetime.now().timestamp():
                            #         rename_file(path_file=item['local_path'], file_name=item_file_local[
                            #             'name'])  # rename file local when delete file in cloud
                            #         delete_file_blob(detail_connection['data']['data'][0][
                            #                              'metric_service_information_connect'],
                            #                          item['container_name'], item['local_path'])
                        list_file_cloud_in_LastVersion=get_list_file_cloud_in_LastVersion(connect_string=detail_connection['data']['data'][0][
                                                        'metric_service_information_connect'],container_name=item['container_name'])
                        if item['time_delete_file_in_LastVersion'] > 0:
                            for item_file in list_file_cloud_in_LastVersion['data']:
                                if (item_file['last_modified'] + (item['time_delete_file_in_LastVersion'] * 86400)) < datetime.now().timestamp():
                                    delete_file_blob(detail_connection['data']['data'][0][
                                                         'metric_service_information_connect'],
                                                     item['container_name'], item_file['name'])
                    else:
                        print('Job chưa đến thời gian thực hiện!')
        except Exception as e:
            save_log_agent(e, 'main()', 0)
            continue

    # ]
    # loop.run_until_complete(asyncio.wait(tasks))
    # loop.close()

    print('khong phai luc nay!')

main_process()
