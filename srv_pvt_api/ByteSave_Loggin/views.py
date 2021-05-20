import calendar
import hashlib
import json
from datetime import datetime, timedelta

from django.db.models import Q
from django.http import JsonResponse

# Create your views here.
from App_Common import encryption, Timer
from Core.models import Customer_ByteSave, Agents, Customers, bytesave_cycle


def loggin_bytesave(request):
    if request.method == 'POST':
        try:
            data = json.dumps(request.POST)
            form = json.loads(data)
            email = form.get('email')
            pwd = encryption(form.get('pwd'))
            serial_number = form.get('serial_number')
            item = Customer_ByteSave.objects.filter(bytesave_email=email).filter(~Q(is_del=1))
            if item.count() == 0:
                return JsonResponse({'status': 'false', 'msg': 'Email nhập không chính xác!'})
            item = Customer_ByteSave.objects.filter(bytesave_email=email).filter(bytesave_pwd=pwd).filter(~Q(is_del=1))
            if item.count() == 0:
                return JsonResponse({'status': 'false', 'msg': 'Mật khẩu nhập không chính xác!'})
            #item = Agents.objects.filter(serial_number=serial_number).filter(~Q(is_del=1))
            #if item.count() > 0:
            #    item = Agents.objects.filter(~Q(is_del=1)).get(serial_number=serial_number)
            #    item.is_logged = 1
            #    item.save()
            #   return JsonResponse({'status': 'true', 'msg': 'Đăng nhập thành công!'})
            item = Customer_ByteSave.objects.filter(~Q(is_del=1)).get(bytesave_email=email)

            #Kiểm tra chu kì:
            #get list chu kì có time hết hạn > time.now(): nếu count() = 0 thì return tài khaorn đã hết hạn
            item_cycles = bytesave_cycle.objects.filter(~Q(is_del=1)).filter(id_customer=item.id_customer).filter(
                bytesave_expiration_date__gte=Timer.get_timestamp_now())
            if item_cycles.count() == 0:
                return JsonResponse({'status': 'false', 'msg': 'Tài khoản đã hết hạn sử dụng!', 'email_loggin': email})
            #Tìm tổng số máy được sử dụng
            number_used = 0
            for item_cycle in item_cycles:
                if item_cycle.bytesave_start_date > Timer.get_timestamp_now():
                    continue
                if item_cycle.bytesave_start_date <= Timer.get_timestamp_now():
                    number_used += item_cycle.bytesave_amount_used
            # nếu số mày =0  là tài khoản chưa đến ngày bắt đầu sử dụng
            if number_used == 0:
                return JsonResponse(
                    {'status': 'false', 'msg': 'Tài khoản chưa được kích hoạt!', 'email_loggin': email})
            items_agents_logged = Agents.objects.filter(id_customer=item.id_customer).filter(
                is_logged=1).filter(~Q(is_del=1))
            if number_used <= items_agents_logged.count():
                return JsonResponse(
                    {'status': 'false', 'msg': 'Tài khoản đã hết phiên đăng nhập!', 'email_loggin': email})
            items_agents = Agents.objects.filter(id_customer=item.id_customer).filter(
                serial_number=serial_number).filter(~Q(is_del=1))
            # Kiểm tra đã có agent đăng nhập nào chưa.nếu chưa thì đi thêm mới agnet :
            if items_agents.count() == 0:
                os = form.get('os')
                ip_public = form.get('ip_public')
                ip_private = form.get('ip_private')
                name_computer = form.get('name_computer')
                create_agent_when_loggin(item.id, item.id_customer, os, serial_number, ip_public, ip_private,
                                         name_computer)
                return JsonResponse({'status': 'true', 'msg': 'Đăng nhập thành công!', 'email_loggin': email})
           #item_cycles = bytesave_cycle.objects.filter(~Q(is_del=1)).filter(bytesave_amount_used) < float(Timer.get_timestamp_now()))
            check_serial_number_in_agents = Agents.objects.filter(serial_number=serial_number).filter(
                id_customer=item.id_customer).filter(~Q(is_del=1)).filter(is_logged=0).first()
            if check_serial_number_in_agents != None:
                # item_serial_number = Agents.objects.filter(~Q(is_del=1)).filter(is_logged=0).get(serial_number=serial_number)
                check_serial_number_in_agents.is_logged = 1
                check_serial_number_in_agents.save()
                return JsonResponse({'status': 'true', 'msg': 'Đăng nhập thành công!', 'email_loggin': email})


            # if items_agent.count() > item.bytesave_amount_used:
            #     return JsonResponse({'status': 'false', 'msg': 'Tài khoản đã được sử dụng hết phiên đăng nhập!'})
            # if item.bytesave_expiration_date != None:
            #     if float(item.bytesave_expiration_date) < Timer.get_timestamp_now():
            #         return JsonResponse({'status': 'false', 'msg': 'Tài khoản đã hết hạn sử dụng!'})



            # today = datetime.today()
            # if items_agent.count() ==0:
            #     if item.bytesave_time_type == 0:
            #         item.bytesave_expiration_date = str(datetime(today.year , today.month + item.bytesave_duration, 1).timestamp())
            #     else:
            #         item.bytesave_expiration_date = str(datetime(today.year + item.bytesave_duration , today.month , 1).timestamp())
            #     item.bytesave_use_start_date = today.timestamp()
            # item.save()
            # os = form.get('os')
            # ip_public = form.get('ip_public')
            # ip_private = form.get('ip_private')
            # name_computer = form.get('name_computer')
            # create_agent_when_loggin(item.id,item.id_customer,os,serial_number,ip_public,ip_private,name_computer)
            return JsonResponse({'status': 'true', 'msg': 'Đăng nhập thành công!','email_loggin':email})
        except Exception as e:
            return JsonResponse({'status': 'false', 'msg': 'Đăng nhập không thành công!'})



def create_agent_when_loggin(id_customer_bytesave,id_customer,os,serial_number,ip_public,ip_private,name_computer):
    try:
        id_agent = Agents.create(**{
            'id_customer_bytesave':id_customer_bytesave,
            'id_customer':id_customer,
            'os':os,
            'serial_number':serial_number,
            'ip_public':ip_public,
            'ip_private':ip_private,
            'name_computer':name_computer,
        })

        return True
    except Exception as e:
        return False
    return


def check_logged(request):
    if request.method == 'POST':
        try:
            data = json.dumps(request.POST)
            form = json.loads(data)
            serial_number = form.get('serial_number')
            item = Agents.objects.filter(~Q(is_del=1)).filter(is_logged=1).filter(serial_number=serial_number)
            if item.count() == 0:
                return JsonResponse({'status': 'none', 'msg': 'Agent chưa được tạo!'})
            else:
                is_logged = item.first().is_logged
                if is_logged == 0:
                    return JsonResponse({'status': 'false', 'msg': 'Không duy trì đăng nhập!'})
            return JsonResponse({'status': 'true', 'msg': 'Duy trì đăng nhập!',})
        except Exception as e:
            return JsonResponse({'status': 'error','msg': 'Không duy trì đăng nhập!', 'error': str(e)})

def logout_bytesave(request):
    if request.method == 'POST':
        try:
            data = json.dumps(request.POST)
            form = json.loads(data)
            serial_number = form.get('serial_number')
            item = Agents.objects.filter(serial_number=serial_number)
            if item.count() == 0:
                return JsonResponse({'status': 'false', 'msg': 'Đăng xuất không thành công!'})
            else:
                item =Agents.objects.filter(~Q(is_del=1)).filter(is_logged=1).filter(serial_number=serial_number).first()
                item.is_logged = 0
                item.save()
            return JsonResponse({'status': 'true', 'msg': 'Đăng xuất thành công!'})
        except Exception as e:
            return JsonResponse({'status': 'error','msg': 'Đăng xuất không thành công!', 'error': str(e)})


def check_ex_date(request,serial_number):
    if request.method == 'GET':
        try:
            item = Agents.objects.filter(~Q(is_del=1)).filter(is_logged=1).filter(serial_number=serial_number).first()
            item_bytesave_customer = Customer_ByteSave.objects.filter(~Q(is_del=1)).get(id_customer=item.id_customer)

            if float(item_bytesave_customer.bytesave_expiration_date) > Timer.get_timestamp_now():
                return JsonResponse({'status': 'true', 'msg': 'thành công!'})
            else:
                return JsonResponse({'status': 'false', 'msg': 'không thành công!'})
        except Exception as e:
            return JsonResponse({'status': 'false', 'msg': str(e)})

