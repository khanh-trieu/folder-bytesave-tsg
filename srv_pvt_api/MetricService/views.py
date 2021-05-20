import asyncio
import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer, GetCapacity
from Core.models import Service, Customer_ByteSave, Metric_Services, Customers, Agents, connect_bytesave
from Send_mail.views import notification_customer_bytesave

def Load_Service(request):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            for item in Service.objects.all().filter((~Q(is_del=1))):
                data.append({
                    'id': item.id,
                    'name': item.name,
                    'description': item.description,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
            countdata = Service.objects.all().filter((~Q(is_del=1))).count()
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})


def Load_data(request, id):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            if id == 0:
                for item in Metric_Services.objects.filter(type=1).filter((~Q(is_del=1))):
                    countdata = 1
                    data.append({
                        'id': item.id,
                        'id_customer': item.id_customer,
                        'name_customer': Customers.objects.get(id=item.id_customer).name,
                        'id_service': item.id_service,
                        'name_service': Service.objects.get(id=item.id_service).name,
                        'information_connect': item.information_connect,
                        'max_storage': item.max_storage,
                        'username_account': item.username_account,
                        'status': item.status,
                        'time_create_at': item.time_create_at,
                        'time_update_at': item.time_update_at,
                    })
                    countdata += 1
            else:  # id != 0: là lấy ra các metric_service của khách hàng có id đó
                for item in Metric_Services.objects.filter(id_customer=id).filter(~Q(is_del=1)).filter(type=1):
                    countdata = 1
                    data.append({
                        'id': item.id,
                        'id_customer': item.id_customer,
                        'name_customer': Customers.objects.get(id=item.id_customer).name,
                        'id_service': item.id_service,
                        'name_service': Service.objects.get(id=item.id_service).name,
                        'information_connect': item.information_connect,
                        'max_storage': item.max_storage,
                        'username_account': item.username_account,
                        'status': item.status,
                        'time_create_at': item.time_create_at,
                        'time_update_at': item.time_update_at,
                    })
                    countdata += 1

            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        is_upgrade_version = 0
        id_metric_service = form.get('id_ser', 0)
        if id_metric_service == '0' or id_metric_service == '':  # thêm mới
            try:
                id_metric = Metric_Services.create(**{
                    'id_customer': id,
                    'id_service': form.get('id_service_s'),
                    'information_connect': form.get('information_connect'),
                    'max_storage': form.get('max_storage'),
                    'username_account': form.get('username_account'),
                    'type': 1,
                })
                connect_bytesave.create(**{
                    'name': 'Connection '+form.get('max_storage'),
                    'id_customer': id,
                    'id_agent': 0,
                    'id_metric_service': id_metric,
                    'type': 0,
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:  # Chỉnh sửa
            try:
                item = Metric_Services.objects.get(id=id_metric_service)
                item.id_customer = id
                item.id_service = form.get('id_service_s')
                item.information_connect = form.get('information_connect')
                item.max_storage = form.get('max_storage')
                item.username_account = form.get('username_account')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()

                return JsonResponse({'status': 'true', 'msg': 'Chính sửa thành công!'})

            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

        return None;


def Check_Metric_Service(request):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)

        item = Metric_Services.objects.filter(information_connect=form.get('information_connect')).filter(~Q(is_del=1)).filter(id_customer=form.get('ID_KH')).filter(id=(form.get('id_ser')) if form.get('id_ser') != '' else 0)
        if item.count() > 0:
            return JsonResponse({'status': 'false', 'msg': 'Chuỗi kết nối đã tồn tại!'})
        return JsonResponse({'status': 'true', 'msg': 'Thành công!'})


def Del_Metric_Service(request, id):
    try:
        item = Metric_Services.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            connect_bytesave.objects.filter(id_metric_service=id).delete()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!', 'string_error': e})


def Load_used_capacity(request, id):
    try:
        dataService = []
        i = 0
        acountServiceQuaHan = 0
        countdata = 0
        if id == 0:
            if Metric_Services.objects.filter((~Q(is_del=1))):
                for item in Metric_Services.objects.filter((~Q(is_del=1))):
                    i = i + 1
                    full_size = GetCapacity(item.information_connect)
                    used_capacity = "{:.2f}".format(full_size * 0.000000000931)
                    if (full_size * 0.000000000931) > int(item.max_storage):
                        acountServiceQuaHan += 1
                    loadding = "{:.2f}".format(((full_size * 0.000000000931) / item.max_storage) * 100)
                    dataService.append({
                        'stt': i,
                        'id': item.id,
                        'max_storage': item.max_storage,
                        'username_account': item.username_account,
                        'loadding': loadding,
                        'used_capacity': used_capacity,
                    })
                    countdata += 1
        else:
            if Metric_Services.objects.filter(id_customer=id).filter((~Q(is_del=1))):
                for item in Metric_Services.objects.filter(id_customer=id).filter((~Q(is_del=1))):
                    i = i + 1
                    full_size = GetCapacity(item.information_connect)

                    used_capacity = "{:.2f}".format(full_size * 0.000000000931)
                    if (full_size * 0.000000000931) > int(item.max_storage):
                        acountServiceQuaHan += 1
                    loadding = "{:.2f}".format(((full_size * 0.000000000931) / item.max_storage) * 100)
                    dataService.append({
                        'stt': i,
                        'id': item.id,
                        'max_storage': item.max_storage,
                        'username_account': item.username_account,
                        'loadding': loadding,
                        'used_capacity': used_capacity,
                    })
                    countdata += 1
        return JsonResponse({'status': 'true', 'msg': 'Thành công!', 'data': dataService, 'countdata': countdata,
                             'acountServiceQuaHan': acountServiceQuaHan}, status=200)
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Không thành công!', 'data': dataService, 'countdata': countdata,
                             'acountServiceQuaHan': acountServiceQuaHan}, status=200)


def Load_used_capacity_of_agent(request, id):
    try:
        dataService = []
        i = 0
        acountServiceQuaHan = 0
        countdata = 0

        if Metric_Services.objects.filter().filter((~Q(is_del=1))):
            item = Metric_Services.objects.get(id=id)
            full_size = GetCapacity(item.information_connect)
            used_capacity = "{:.2f}".format(full_size * 0.000000000931)
            if (full_size * 0.000000000931) > int(item.max_storage):
                acountServiceQuaHan += 1
            loadding = "{:.2f}".format(((full_size * 0.000000000931) / item.max_storage) * 100)
            dataService.append({
                'stt': i,
                'id': item.id,
                'max_storage': item.max_storage,
                'username_account': item.username_account,
                'loadding': loadding,
                'used_capacity': used_capacity,
            })
            countdata += 1
        return JsonResponse({'status': 'true', 'msg': 'Thành công!', 'data': dataService }, status=200)
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Không thành công!', 'data': dataService}, status=200)

