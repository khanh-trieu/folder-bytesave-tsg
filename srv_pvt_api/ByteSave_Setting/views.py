import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer, Convert_timestamp
from Core.models import setting_bytesave, Agents, Customers, Versions, Customer_ByteSave, bytesave_cycle


def load_data_setting_bytesave(request, id, serial_number,email_loggin):
    if request.method == 'GET':
        try:
            data = []
            item_customer = Customer_ByteSave.objects.get(bytesave_email=email_loggin)
            countdata = 0
            for item in setting_bytesave.objects.filter(
                    id_agent=Agents.objects.filter(~Q(is_del=1)).filter(serial_number=serial_number).get(id_customer=item_customer.id_customer).id).filter(
                    ~Q(is_del=1)):
                countdata = 1
                data.append({
                    'id': item.id,
                    'id_agent': item.id_agent,
                    'server_mail': item.server_mail,
                    'port': item.port,
                    'mail_send': item.mail_send,
                    'mail_send_pwd': item.mail_send_pwd,
                    'subject': item.subject,
                    'is_ssl': item.is_ssl,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                })
                countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'error': str(e)})
        return JsonResponse({'status': 'false', 'countdata': countdata, 'data': data})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)

        if id == 0:  # thêm mới
            try:
                item_customer = Customer_ByteSave.objects.get(bytesave_email=email_loggin)
                id_agent = Agents.objects.filter(~Q(is_del=1)).filter(serial_number=serial_number).get(
                    id_customer=item_customer.id_customer).id
                setting_bytesave.create(**{
                    'id_agent': id_agent,
                    'server_mail': form.get('server_mail'),
                    'port': form.get('port'),
                    'mail_send': form.get('mail_send'),
                    'mail_send_pwd': form.get('mail_send_pwd'),
                    'subject': form.get('subject'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'error': str(e)})
        else:  # chỉnh sửa
            try:
                item = setting_bytesave.objects.filter((~Q(is_del=1))).get(id=id)
                item.id_agent = form.get('id_agent')
                item.server_mail = form.get('server_mail')
                item.port = form.get('port')
                item.mail_send = form.get('mail_send')
                item.mail_send_pwd = form.get('mail_send_pwd')
                item.subject = form.get('subject')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'error': str(e)})
            return None;


def information_bytesave(request, serial_number,email_loggin):
    if request.method == 'GET':
        try:
            data = []
            item = Customers.objects.filter(~Q(is_del=1)).get(email=email_loggin)
            name_version = Versions.objects.get(id = item.id_version).name if item.id_version != 0 else Versions.objects.all()[0].name
            #item_cycle = bytesave_cycle.objects.filter(id_customer=item.id).order_by('-id')[0]
            bytesave_expiration_date = 0
            # Kiểm tra chu kì:
            # get list chu kì có time hết hạn > time.now(): nếu count() = 0 thì return tài khaorn đã hết hạn
            item_cycles = bytesave_cycle.objects.filter(~Q(is_del=1)).filter(id_customer=item.id).filter(
                bytesave_expiration_date__gte=Timer.get_timestamp_now())
            if item_cycles.count() > 0:
                bytesave_expiration_date = item_cycles.first().bytesave_expiration_date
            # Tìm tổng số máy được sử dụng
            number_used = 0
            for item_cycle in item_cycles:
                if item_cycle.bytesave_start_date > Timer.get_timestamp_now():
                    continue
                if item_cycle.bytesave_start_date <= Timer.get_timestamp_now():
                    number_used += item_cycle.bytesave_amount_used
            items_agents_logged = Agents.objects.filter(id_customer=item.id).filter(
                is_logged=1).filter(~Q(is_del=1))
            if number_used < items_agents_logged.count():
                bytesave_expiration_date = 0

            data.append({
                'name_version': name_version,
                'bytesave_expiration_date': bytesave_expiration_date,
            })
            return JsonResponse({'status': 'true','countdata':1, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false','countdata':0, 'data': [], 'error': str(e)})
        return JsonResponse({'status': 'false','countdata':0, 'data': data})
