import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer
from ByteSave_Connect.views import load_data_bytesave_connect
from Core.models import Agents, Customers, Versions, Customer_ByteSave, connect_bytesave, Metric_Services


def load_version(request):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            for item in Versions.objects.all().filter((~Q(is_del=1))):
                data.append({
                    'id': item.id,
                    'name': item.name,
                    'description': item.description,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
            countdata = Versions.objects.all().filter((~Q(is_del=1))).count()
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})


def Load_data(request, id):
    if request.method == 'GET':
        try:
            data = []
            data_agent_bytesave_connect = []
            countdata = 0
            countdata_agent_bytesave_connect = 0
            if id == 0:
                i = 0
                for item in Customers.objects.all().filter((~Q(is_del=1))):
                    if Agents.objects.filter(id_customer=item.id).filter((~Q(is_del=1))):
                        countdata_agent_bytesave_connect = 0
                        data_agent = []
                        for item_agent in Agents.objects.filter(id_customer=item.id).filter(is_logged=1).filter((~Q(is_del=1))):
                            data_agent_bytesave_connect = []
                            # if connect_bytesave.objects.filter(id_agent=item_agent.id).filter((~Q(is_del=1))):
                            #     data_agent_bytesave_connect = load_data_bytesave_connect(item_agent.id)['data']
                            #     countdata_agent_bytesave_connect = load_data_bytesave_connect(item_agent.id)[
                            #         'countdata']
                            data_agent.append({
                                'id': item_agent.id,
                                'os': item_agent.os,
                                'serial_number': item_agent.serial_number,
                                'ip_public': item_agent.ip_public,
                                'ip_private': item_agent.ip_private,
                                'name_computer': item_agent.name_computer,
                                'time_create_at': item_agent.time_create_at,
                                'time_update_at': item_agent.time_update_at,
                                # 'countdata_agent_bytesave_connect': countdata_agent_bytesave_connect,
                                # 'data_agent_bytesave_connect': data_agent_bytesave_connect,
                            })
                        data.append({
                            'stt':i+1,
                            'id': item.id,
                            'name_customer': item.name,
                            'id_customer': item.id,
                            'email': item.email,
                            'name_version': Versions.objects.get(id=item.id_version).name,
                            'description_version': Versions.objects.get(id=item.id_version).description,
                            'bytesave_use_start_date': Customer_ByteSave.objects.get(
                                id_customer=item.id).bytesave_use_start_date,
                            'data_agent': data_agent,
                        })
                        i+=1
                        countdata += 1

            else:  # id != 0
                for item in Customers.objects.filter(id=id).filter((~Q(is_del=1))):
                    if Agents.objects.filter(id_customer=item.id).filter((~Q(is_del=1))):
                        countdata_agent_bytesave_connect = 0
                        data_agent = []
                        for item_agent in Agents.objects.filter(id_customer=item.id).filter((~Q(is_del=1))):
                            data_agent_bytesave_connect = []
                            # if connect_bytesave.objects.filter(id_agent=item_agent.id).filter((~Q(is_del=1))):
                            #     data_agent_bytesave_connect = load_data_bytesave_connect(item_agent.id)['data']
                            #     countdata_agent_bytesave_connect = load_data_bytesave_connect(item_agent.id)[
                            #         'countdata']
                            data_agent.append({
                                'id': item_agent.id,
                                'os': item_agent.os,
                                'serial_number': item_agent.serial_number,
                                'ip_public': item_agent.ip_public,
                                'ip_private': item_agent.ip_private,
                                'name_computer': item_agent.name_computer,
                                'time_create_at': item_agent.time_create_at,
                                'time_update_at': item_agent.time_update_at,
                                # 'countdata_agent_bytesave_connect': countdata_agent_bytesave_connect,
                                # 'data_agent_bytesave_connect': data_agent_bytesave_connect,
                            })
                        data.append({
                            'id': item.id,
                            'id_customer': item.id,
                            'name_customer': item.name,
                            'name_version': Versions.objects.get(id=item.id_version).name,
                            'description_version': Versions.objects.get(id=item.id_version).description,
                            'bytesave_use_start_date': Customer_ByteSave.objects.get(
                                id_customer=item.id).bytesave_use_start_date,
                            'data_agent': data_agent,
                        })
                        countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})

def create_metric_service_agent(request):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        id_metric_service = form.get('id_ser')
        serial_number = form.get('serial_number')
        if id_metric_service == '0' or id_metric_service == '':  # thêm mới
            try:
                Metric_Services.create(**{
                    'id_customer': Agents.objects.get(serial_number=serial_number).id_customer,
                    'id_service': form.get('id_service_s'),
                    'information_connect': form.get('information_connect'),
                    'max_storage': form.get('max_storage'),
                    'username_account': form.get('username_account'),
                    'type': 1,
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

                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})

            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

        return None;