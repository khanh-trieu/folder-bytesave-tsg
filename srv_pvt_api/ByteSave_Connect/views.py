import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer
from Core.models import connect_bytesave, Metric_Services, Service, backup_bytesave, Agents, Customer_ByteSave, \
    Customers
from MetricService.views import Del_Metric_Service


def load_data_bytesave_connect(request, serial_number,email_loggin):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            item_customer = Customers.objects.filter((~Q(is_del=1))).get(email=email_loggin)
            # item_agent= Agents.objects.filter((~Q(is_del=1))).filter(is_logged=1).get(serial_number=serial_number)
            for item in connect_bytesave.objects.filter(id_customer=item_customer.id).filter(~Q(is_del=1)):
                item_metric = Metric_Services.objects.filter((~Q(is_del=1))).get(id=item.id_metric_service) \
                    if Metric_Services.objects.filter(~Q(is_del=1)).filter(id=item.id_metric_service).count() > 0 else None
                data.append({
                    'id': item.id,
                    'id_agent': item.id_agent,
                    'id_metric_service': item.id_metric_service,
                    'metric_service_max_storage': item_metric.max_storage
                    if item_metric != None else 0,
                    'metric_service_information_connect': item_metric.information_connect
                    if item_metric != None else '',
                    'name': item.name,
                    'metric_service_username_account': item_metric.username_account
                    if item_metric != None else '',
                    'time_check_at': item.time_check_at,
                    'type_text': Service.objects.get(
                        id=item_metric.id_service).name if item_metric != None else '' ,
                    'type': item_metric.type if item_metric != None else 0,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
                countdata += 1
            # item_connection_id_agent = connect_bytesave.objects.filter(id_agent=item_agent.id).filter((~Q(is_del=1))).filter(id_customer=0)
            # if item_connection_id_agent.count() > 0:
            #     for item in item_connection_id_agent:
            #         countdata = 1
            #         item_metric = Metric_Services.objects.filter((~Q(is_del=1))).get(id=item.id_metric_service) \
            #             if Metric_Services.objects.filter(~Q(is_del=1)).filter(
            #             id=item.id_metric_service).count() > 0 else None
            #         data.append({
            #             'id': item.id,
            #             'id_agent': item.id_agent,
            #             'id_metric_service': item.id_metric_service,
            #             'metric_service_max_storage': item_metric.max_storage
            #             if item_metric != None else 0,
            #             'metric_service_information_connect': item_metric.information_connect
            #             if item_metric != None else '',
            #             'name': item.name,
            #             'metric_service_username_account': item_metric.username_account
            #             if item_metric != None else '',
            #             'time_check_at': item.time_check_at,
            #             'type_text': Service.objects.get(
            #                 id=item_metric.id_service).name if item_metric != None else '',
            #             'type': item_metric.type if item_metric != None else 0,
            #             'time_create_at': item.time_create_at,
            #             'time_update_at': item.time_update_at,
            #         })
            #         countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [],'error':str(e)})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        if form.get('id') == '0':# thêm mới
            try:
                id_metric_service = Metric_Services.create(**{
                    'id_customer':Agents.objects.filter((~Q(is_del=1))).get(serial_number=serial_number).id_customer,
                    'id_service':form.get('id_service'),
                    'information_connect':form.get('information_connect'),
                    'max_storage':form.get('max_storage'),
                    'username_account':form.get('username_account'),
                    'type':1,
                })

                connect_bytesave.create(**{
                    'id_agent': Agents.objects.filter((~Q(is_del=1))).get(serial_number=serial_number).id,
                    'id_metric_service': id_metric_service,
                    'id_customer': Agents.objects.filter((~Q(is_del=1))).get(serial_number=serial_number).id_customer,
                    'name': form.get('name'),
                    'type': form.get('type'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'error': str(e)})
        else: # chỉnh sửa
            try:
                item = connect_bytesave.objects.filter((~Q(is_del=1))).get(id=int(form.get('id')))
                item.name = form.get('name')
                item.save()
                # item_metric_serivce= Metric_Services.objects.filter((~Q(is_del=1))).get(id=item.id_metric_service)
                # item_metric_serivce.id_service = form.get('id_service')
                # item_metric_serivce.information_connect = form.get('information_connect')
                # item_metric_serivce.max_storage = form.get('max_storage')
                # item_metric_serivce.username_account = form.get('username_account')
                # item_metric_serivce.type = form.get('type')
                # item.time_update_at = Timer.get_timestamp_now()
                # item_metric_serivce.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'error': str(e)})
            return None;

def load_detail_bytesave_connect(request, id):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            item = connect_bytesave.objects.get(id=id)
            data.append({
                'id': item.id,
                'id_agent': item.id_agent,
                'id_metric_service': item.id_metric_service,
                'metric_service_max_storage': Metric_Services.objects.get(id=item.id_metric_service).max_storage,
                'metric_service_information_connect': Metric_Services.objects.get(
                    id=item.id_metric_service).information_connect,
                'name': item.name,
                'type_text': Service.objects.get(
                    id=Metric_Services.objects.get(id=item.id_metric_service).id_service).name,
                'type': item.type,
                'time_create_at': item.time_create_at,
                'time_update_at': item.time_update_at,
            })
        except Exception as e:
            data = []
        return JsonResponse({'status': 'true','data': data})

def check_account_name(request,  serial_number,name):
    if request.method == 'GET':
        try:
            check = connect_bytesave.objects.filter(id_agent=Agents.objects.filter((~Q(is_del=1))).filter(is_logged=1).get(serial_number=serial_number).id).filter(name=name)
            if check.count() > 0:
                return JsonResponse({'status': 'false', 'msg': 'Tên kết nối đã tồn tại!'})
            return JsonResponse({'status': 'true', 'msg': 'Tên kết nối hợp lệ!'})
        except Exception as e:
            return JsonResponse({'status': 'error', 'msg': str(e)})

def del_connect_bytesave(request, id):
    try:
        item = connect_bytesave.objects.get(id=id)
        lst_backup = backup_bytesave.objects.filter(id_connect_bytesave=item.id).filter((~Q(is_del=1)))
        if lst_backup.count() > 0:
            return JsonResponse({'status': 'false', 'msg': 'Kết nối có tác vụ sao lưu sử dụng, bạn không thể xóa kết nối này!!'})
        if item != None:
            item.is_del = 1
            item.save()
            Del_Metric_Service(request,item.id_metric_service)
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'error', 'msg': 'Xóa không thành công!','error':str(e)})

def load_data_bytesave_connect_in_web(request, email):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            item_bytesave = Customer_ByteSave.objects.filter((~Q(is_del=1))).get(bytesave_email=email)
            item_agent= Agents.objects.filter((~Q(is_del=1))).filter(is_logged=1).get(id_customer=item_bytesave.id_customer)
            for item in connect_bytesave.objects.filter(id_customer=item_bytesave.id_customer).filter((~Q(is_del=1))):
                item_metric = Metric_Services.objects.filter((~Q(is_del=1))).get(id=item.id_metric_service) \
                    if Metric_Services.objects.filter(~Q(is_del=1)).filter(id=item.id_metric_service).count() > 0 else None
                data.append({
                    'id': item.id,
                    'id_agent': item.id_agent,
                    'id_metric_service': item.id_metric_service,
                    'metric_service_max_storage': item_metric.max_storage
                    if item_metric != None else 0,
                    'metric_service_information_connect': item_metric.information_connect
                    if item_metric != None else '',
                    'name': item.name,
                    'metric_service_username_account': item_metric.username_account
                    if item_metric != None else '',
                    'time_check_at': item.time_check_at,
                    'type_text': Service.objects.get(
                        id=item_metric.id_service).name if item_metric != None else '' ,
                    'type': item_metric.type if item_metric != None else 0,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
                countdata += 1
            item_connection_id_agent = connect_bytesave.objects.filter(id_agent=item_agent.id).filter((~Q(is_del=1))).filter(id_customer=0)
            if item_connection_id_agent.count() > 0:
                for item in item_connection_id_agent:
                    countdata = 1
                    item_metric = Metric_Services.objects.filter((~Q(is_del=1))).get(id=item.id_metric_service) \
                        if Metric_Services.objects.filter(~Q(is_del=1)).filter(
                        id=item.id_metric_service).count() > 0 else None
                    data.append({
                        'id': item.id,
                        'id_agent': item.id_agent,
                        'id_metric_service': item.id_metric_service,
                        'metric_service_max_storage': item_metric.max_storage
                        if item_metric != None else 0,
                        'metric_service_information_connect': item_metric.information_connect
                        if item_metric != None else '',
                        'name': item.name,
                        'metric_service_username_account': item_metric.username_account
                        if item_metric != None else '',
                        'time_check_at': item.time_check_at,
                        'type_text': Service.objects.get(
                            id=item_metric.id_service).name if item_metric != None else '',
                        'type': item_metric.type if item_metric != None else 0,
                        'time_create_at': item.time_create_at,
                        'time_update_at': item.time_update_at,
                    })
                    countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [],'error':str(e)})
