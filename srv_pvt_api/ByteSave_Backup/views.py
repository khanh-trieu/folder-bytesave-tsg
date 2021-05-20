import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer
from Core.models import backup_bytesave, Agents, connect_bytesave, Metric_Services, Customer_ByteSave


def load_data_bytesave_backup(request, serial_number,email):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            item_customer = Customer_ByteSave.objects.filter((~Q(is_del=1))).get(bytesave_email=email)
            item_agent= Agents.objects.filter((~Q(is_del=1))).filter(serial_number=serial_number).get(id_customer=item_customer.id_customer)
            for item in backup_bytesave.objects.filter(id_agent=item_agent.id).filter(~Q(is_del=1)):
                countdata = 1
                data.append({
                    'id': item.id,
                    'id_agent': item.id_agent,
                    'id_connect_bytesave': item.id_connect_bytesave,
                    'connect_bytesave_name': connect_bytesave.objects.filter(~Q(is_del=1)).get(id=item.id_connect_bytesave).name ,
                    'connect_bytesave_username_account': Metric_Services.objects.filter(~Q(is_del=1)).get(id=connect_bytesave.objects.filter(~Q(is_del=1)).get(id=item.id_connect_bytesave).id_metric_service).username_account  ,
                    'name': item.name,
                    'local_path': item.local_path,
                    'container_name': item.container_name,
                    'time': item.time,
                    'time_delete': item.time_delete,
                    'time_delete_file_in_LastVersion': item.time_delete_file_in_LastVersion,
                    'email': item.email,
                    'is_folder': item.is_folder,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
                countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [],'error':str(e)})
        return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        item_customer = Customer_ByteSave.objects.filter((~Q(is_del=1))).get(bytesave_email=email)
        if form.get('id') == '0':# thêm mới
            try:
                backup_bytesave.create(**{
                    'id_agent': Agents.objects.filter(~Q(is_del=1)).filter(is_logged=1).get(id_customer=item_customer.id_customer).id,
                    'id_connect_bytesave': form.get('id_connect_bytesave'),
                    'name': form.get('name'),
                    'local_path': form.get('local_path').replace('-','\\'),
                    'container_name': form.get('container_name'),
                    'email': form.get('email') if form.get('email') != '0' else '',
                    'time_delete': form.get('time_delete'),
                    'time_delete_file_in_LastVersion': form.get('time_delete_file_in_LastVersion'),
                    'time': form.get('time'),
                    'is_folder': form.get('is_folder'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'error': str(e)})
        else: # chỉnh sửa
            try:
                item = backup_bytesave.objects.filter((~Q(is_del=1))).get(id=int(form.get('id')))
                item.id_connect_bytesave = form.get('id_connect_bytesave')
                item.name = form.get('name')
                item.local_path = str(form.get('local_path').replace('-','\\'))
                item.container_name = form.get('container_name')
                item.email = form.get('email')
                item.time_delete = form.get('time_delete')
                item.time_delete_file_in_LastVersion = form.get('time_delete_file_in_LastVersion')
                item.time = form.get('time')
                item.is_folder = form.get('is_folder')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'error': str(e)})
            return None;


def check_name_backup(request,  serial_number,name):
    if request.method == 'GET':
        try:
            check = backup_bytesave.objects.filter(id_agent=Agents.objects.filter((~Q(is_del=1))).filter(is_logged=1).get(serial_number=serial_number).id).filter(name=name)
            if check.count() > 0:
                return JsonResponse({'status': 'false', 'msg': 'Tên tác vụ sao lưu đã tồn tại!','error':''})
            return JsonResponse({'status': 'true', 'msg': 'Tên tác vụ sao lưu hợp lệ!','error':''})
        except Exception as e:
            return JsonResponse({'status': 'error', 'msg': 'Lỗi hệ thống','error':str(e)})

def del_backup_bytesave(request, id):
    try:
        item = backup_bytesave.objects.filter(~Q(is_del=1)).get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!','error':str(e)})

def load_data_bytesave_backup_in_web(request,email):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            item_bytesave = Customer_ByteSave.objects.filter((~Q(is_del=1))).get(bytesave_email=email)
            item_agent= Agents.objects.filter((~Q(is_del=1))).filter(is_logged=1).get(id_customer=item_bytesave.id_customer)
            for item in backup_bytesave.objects.filter(id_agent=item_agent.id).filter(~Q(is_del=1)):
                countdata = 1
                data.append({
                    'id': item.id,
                    'id_agent': item.id_agent,
                    'id_connect_bytesave': item.id_connect_bytesave,
                    'connect_bytesave_name': connect_bytesave.objects.filter(~Q(is_del=1)).get(id=item.id_connect_bytesave).name ,
                    'connect_bytesave_username_account': Metric_Services.objects.filter(~Q(is_del=1)).get(id=connect_bytesave.objects.filter(~Q(is_del=1)).get(id=item.id_connect_bytesave).id_metric_service).username_account  ,
                    'name': item.name,
                    'local_path': item.local_path,
                    'container_name': item.container_name,
                    'time': item.time,
                    'time_delete': item.time_delete,
                    'time_delete_file_in_LastVersion': item.time_delete_file_in_LastVersion,
                    'email': item.email,
                    'is_folder': item.is_folder,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
                countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [],'error':str(e)})
        return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})