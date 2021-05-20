import calendar
from datetime import datetime
import json
from importlib.resources import Resource

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import genpwd, Timer, encryption, Convert_timestamp
from Core.models import Customers, Customer_ByteSave, Customer_Represent, Metric_Services, Versions, bytesave_cycle, \
    connect_bytesave
from Send_mail.views import notification_customer_bytesave


def Load_data(request, id):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            countdata_represent = 0
            i=0
            if id == 0:
                for item in Customers.objects.all().filter((~Q(is_del=1))).order_by('-id'):
                    data_represent = []
                    i = i + 1
                    item_customer_bytesave = Customer_ByteSave.objects.filter(id_customer=item.id)[0]
                    status=0
                    # if item_customer_bytesave.bytesave_expiration_date != '' and item_customer_bytesave.bytesave_expiration_date != None:
                    #     if float(item_customer_bytesave.bytesave_expiration_date) < Timer.get_timestamp_now():
                    #         status = 2
                    #     else:status = 1
                    item_customer_represent = Customer_Represent.objects.filter(id_customer=item.id).filter(
                        (~Q(is_del=1)))
                    if item_customer_represent.count() > 0:
                        stt = 0
                        for item_represent in item_customer_represent:
                            data_represent.append({
                                'stt': stt + 1,
                                'id': item_represent.id,
                                'id_customer': item_represent.id_customer,
                                'name': item_represent.name,
                                'phone_number': item_represent.phone_number,
                                'position': item_represent.position,
                                'email': item_represent.email,
                                'type': item_represent.type,
                                'is_del': item_represent.is_del,
                                'time_create_at': item_represent.time_create_at,
                                'time_update_at': item_represent.time_update_at,
                            })
                            countdata_represent += 1
                            stt += 1
                    data.append({
                        'id': item.id,
                        'stt': i,
                        'id_loggin': item.id_loggin,
                        'id_version': item.id_version,
                        'customer_bytesave': {
                            'id': item_customer_bytesave.id,
                            'bytesave_email': item_customer_bytesave.bytesave_email if item_customer_bytesave.bytesave_email != None else '',
                            'bytesave_pwd': item_customer_bytesave.bytesave_pwd,
                            # 'bytesave_amount_used': item_customer_bytesave.bytesave_amount_used,
                            # 'bytesave_duration': item_customer_bytesave.bytesave_duration,
                            'bytesave_use_start_date': item_customer_bytesave.bytesave_use_start_date,
                            # 'bytesave_time_type': item_customer_bytesave.bytesave_time_type,
                            #'bytesave_expiration_date': Convert_timestamp( item_customer_bytesave.bytesave_expiration_date),
                        },
                        'name': item.name,
                        'email': item.email,
                        'type': item.type,
                        'phone_number': item.phone_number,
                        'city': item.city,
                        'address': item.address,
                        'tax_code': item.tax_code,
                        'website': item.website,
                        'fax': item.fax,
                        'legal_representative': item.legal_representative,
                        'scale': item.scale,
                        'field': item.field,
                        'is_upgrade_version': item.is_upgrade_version,
                        'time_create_at': item.time_create_at,
                        'time_update_at': item.time_update_at,
                        'customer_represent': data_represent,
                        'countdata_represent': countdata_represent,
                        'status': status,
                    })
            else:
                item = Customers.objects.get(id=id)
                data_represent = []
                item_customer_bytesave = Customer_ByteSave.objects.filter(id_customer=item.id)[0]
                status = 0
                # if item_customer_bytesave.bytesave_expiration_date != '' and item_customer_bytesave.bytesave_expiration_date != None:
                #     a =Timer.get_timestamp_now
                #     if float(item_customer_bytesave.bytesave_expiration_date) < Timer.get_timestamp_now():
                #         status = 2
                #     else:
                #         status = 1
                item_customer_represent = Customer_Represent.objects.filter(id_customer=item.id).filter((~Q(is_del=1)))
                if item_customer_represent.count() > 0:
                    stt = 0
                    for item_represent in item_customer_represent:
                        data_represent.append({
                            'stt': stt + 1,
                            'id': item_represent.id,
                            'id_customer': item_represent.id_customer,
                            'name': item_represent.name,
                            'phone_number': item_represent.phone_number,
                            'position': item_represent.position,
                            'email': item_represent.email,
                            'type': item_represent.type,
                            'is_del': item_represent.is_del,
                            'time_create_at': item_represent.time_create_at,
                            'time_update_at': item_represent.time_update_at,
                        })
                        countdata_represent += 1
                        stt += 1
                item_bytesave_cycle = bytesave_cycle.objects.filter(id_customer=item.id).filter((~Q(is_del=1)))
                data_cycle=[]
                countdata_cycle = 0
                if item_bytesave_cycle.count() > 0:
                    stt = 0
                    for item_cycle in item_bytesave_cycle:
                        data_cycle.append({
                            'stt': stt + 1,
                            'id': item_cycle.id,
                            'id_customer': item_cycle.id_customer,
                            'bytesave_amount_used': item_cycle.bytesave_amount_used,
                            'bytesave_duration': str(item_cycle.bytesave_duration) + (' tháng' if item_cycle.bytesave_time_type == 0 else ' năm'),
                            'bytesave_time_type': item_cycle.bytesave_time_type,
                            'bytesave_start_date': Convert_timestamp(item_cycle.bytesave_start_date),
                            'bytesave_expiration_date': Convert_timestamp(item_cycle.bytesave_expiration_date),
                        })
                        countdata_cycle += 1
                        stt += 1
                date_start = Convert_timestamp(item_customer_bytesave.bytesave_use_start_date)
                date_start = datetime.strptime(date_start, "%d/%m/%Y")
                date_start = convert_string_date_format(date_start)
                date_start = datetime.strptime(date_start, "%Y-%m-%d")
                data.append({
                    'stt': i,
                    'id': item.id,
                    'id_loggin': item.id_loggin,
                    'id_version': item.id_version,
                    'customer_bytesave': {
                        'id': item_customer_bytesave.id,
                        'bytesave_email': item_customer_bytesave.bytesave_email if item_customer_bytesave.bytesave_email != None else '',
                        'bytesave_pwd': item_customer_bytesave.bytesave_pwd,
                        #'bytesave_amount_used': item_customer_bytesave.bytesave_amount_used,
                        #'bytesave_duration': item_customer_bytesave.bytesave_duration,
                        'bytesave_use_start_date': date_start,
                        #'bytesave_time_type': item_customer_bytesave.bytesave_time_type,
                        #'bytesave_expiration_date': Convert_timestamp( item_customer_bytesave.bytesave_expiration_date),
                    },
                    'name': item.name,
                    'email': item.email,
                    'type': item.type,
                    'phone_number': item.phone_number,
                    'city': item.city,
                    'address': item.address,
                    'tax_code': item.tax_code,
                    'website': item.website,
                    'fax': item.fax,
                    'legal_representative': item.legal_representative,
                    'scale': item.scale,
                    'field': item.field,
                    'is_upgrade_version': item.is_upgrade_version,
                    'time_create_at': item.time_create_at,
                    'time_update_at': item.time_update_at,
                    'customer_represent': data_represent,
                    'countdata_represent': countdata_represent,
                    'data_cycle': data_cycle,
                    'countdata_cycle': countdata_cycle,
                    'status': status,
                })
            countdata = Customers.objects.all().count()

            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})

        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        is_upgrade_version = 0
        if form.get('is_upgrade_version') == 'on':
            is_upgrade_version = 1
        id = form.get('id', 0)
        if id == '0':  # thêm mới
            try:
                Customers.create(**{
                    'id_loggin': 0,
                    'id_version': Versions.objects.filter(~Q(is_del=1)).first().id,
                    'name': form.get('name'),
                    'email': form.get('email'),
                    'type': form.get('type'),
                    'phone_number': form.get('phone_number'),
                    'city': form.get('city'),
                    'address': form.get('address'),
                    'tax_code': form.get('tax_code'),
                    'website': form.get('website'),
                    'fax': form.get('fax'),
                    'legal_representative': form.get('legal_representative'),
                    'scale': form.get('scale'),
                    'field': form.get('field'),
                    'is_upgrade_version': is_upgrade_version,
                })
                id_customer_max = Customers.objects.all().order_by("-id")[0].id
                # str_time_start = form.get('date_start_used').replace('-', '/')
                # date = datetime.strptime(str_time_start, "%Y/%m/%d")
                pwd = genpwd(12)
                Customer_ByteSave.create(**{
                    'id_customer': id_customer_max,
                    'bytesave_email': form.get('email'),
                    'bytesave_pwd': encryption(pwd),
                    'bytesave_use_start_date': Timer.get_timestamp_now(),
                    # 'bytesave_amount_used': form.get('bytesave_amount_used'),
                    # 'bytesave_duration': form.get('bytesave_duration'),
                    # 'bytesave_time_type': form.get('bytesave_time_type'),
                    # 'bytesave_expiration_date': form.get('bytesave_expiration_date'),
                })
                notification_customer_bytesave('[ByteSave]',form.get('email'),pwd,form.get('email'))

                if int(form.get('CountRowNDD')) > 0:
                    for i in range(1, int(form.get('CountRowNDD')) + 1):
                        Customer_Represent.create(**{
                            'id_customer': id_customer_max,
                            'name': form.get('represent_name' + str(i)),
                            'phone_number': form.get('represent_phone_number' + str(i)),
                            'position': form.get('represent_position' + str(i)),
                            'email': form.get('represent_email' + str(i)),
                            'type': form.get('represent_type' + str(i)),
                        })
                if int(form.get('CountRowService')) > 0:
                    for i in range(1, int(form.get('CountRowService')) + 1):
                        id_metric =Metric_Services.create(**{
                            'id_customer': id_customer_max,
                            'id_service': form.get('id_service' + str(i)),
                            'information_connect': form.get('information_connect' + str(i)),
                            'max_storage': form.get('max_storage' + str(i)),
                            'username_account': form.get('username_account' + str(i)),
                            'type': form.get('represent_type' + str(i)),
                        })
                        connect_bytesave.create(**{
                            'name': 'Connection ' + form.get('max_storage' + str(i)),
                            'id_customer': id_customer_max,
                            'id_agent': 0,
                            'id_metric_service': id_metric,
                            'type': 0,
                        })
                if int(form.get('CountRowChuki')) > 0:
                    for i in range(1, int(form.get('CountRowChuki')) + 1):
                        # date_end = add_months(date, int(form.get('bytesave_duration' + str(i))) if form.get(
                        #     'bytesave_time_type' + str(i)) == '0' else (int(form.get('bytesave_duration' + str(i))) * 12))

                        date_start  = datetime.strptime((form.get('date_start_used' + str(i))).replace('-','/'), '%d/%m/%Y')
                        date_end  = datetime.strptime((form.get('date_end_used' + str(i))).replace('-','/'), '%d/%m/%Y')

                        bytesave_cycle.create(**{
                            'id_customer': id_customer_max,
                            'bytesave_amount_used': form.get('bytesave_amount_used' + str(i)),
                            'bytesave_duration': form.get('bytesave_duration' + str(i)),
                            'bytesave_time_type': form.get('bytesave_time_type' + str(i)),
                            'bytesave_start_date': date_start.timestamp(),
                            'bytesave_expiration_date': date_end.timestamp(),
                        })

                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!', 'id': id_customer_max })
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:
            try:
                item = Customers.objects.get(id=id)
                item.name = form.get('name')
                item.email = form.get('email')
                item.scale = form.get('scale')
                item.field = form.get('field')
                item.city = form.get('city')
                item.time_update_at = Timer.get_timestamp_now()
                item.is_upgrade_version = is_upgrade_version
                item.type = form.get('type')
                item.phone_number = form.get('phone_number')
                item.address = form.get('address')
                item.tax_code = form.get('tax_code')
                item.website = form.get('website')
                item.fax = form.get('fax')
                item.legal_representative = form.get('legal_representative')
                item.save()

                item_bytesave = Customer_ByteSave.objects.get(id_customer=id)
                item_bytesave.bytesave_email = form.get('email')
                item_bytesave.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

        return None;


def save_cycle_bytesave(request,id):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        id_customer = form.get('id_customer', 0)
        if id == 0:  # thêm mới
            try:
                date_start = datetime.strptime((form.get('date_start_used')).replace('-', '/'), '%Y/%m/%d')
                date_end = add_months(date_start, int(form.get('bytesave_duration') if form.get(
                            'bytesave_time_type') == '0' else (int(form.get('bytesave_duration')) * 12)))
                bytesave_cycle.create(**{
                    'id_customer': form.get('id_customer'),
                    'bytesave_amount_used': form.get('bytesave_amount_used'),
                    'bytesave_duration': form.get('bytesave_duration'),
                    'bytesave_time_type': form.get('bytesave_time_type'),
                    'bytesave_start_date': date_start.timestamp(),
                    'bytesave_expiration_date': date_end,
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:  # Chỉnh sửa
            try:
                date_start = datetime.strptime((form.get('date_start_used')).replace('-', '/'), '%Y/%m/%d')
                date_end = add_months(date_start, int(form.get('bytesave_duration') if form.get(
                    'bytesave_time_type') == '0' else (int(form.get('bytesave_duration')) * 12)))
                date_start = date_start.timestamp()
                item = bytesave_cycle.objects.get(id=id)
                print(item.bytesave_start_date)
                item.id_customer = form.get('id_customer')
                item.bytesave_amount_used = form.get('bytesave_amount_used')
                item.bytesave_duration = form.get('bytesave_duration')
                item.bytesave_time_type = form.get('bytesave_time_type')
                item.bytesave_start_date = int(date_start)
                item.bytesave_expiration_date = date_end
                item.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

    return None

def Del_cycle(request, id):
    try:
        item = bytesave_cycle.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!', 'string_error': e})

def add_months(sourcedate, months):
    month = sourcedate.month - 1 + months
    year = sourcedate.year + month // 12
    month = month % 12 + 1
    day = min(sourcedate.day, calendar.monthrange(year,month)[1])
    datetime_str =str(day)+ '/'+ str(month) + '/'+ str(year)
    #datetime_str = '19/09/18 13:55:26'

    datetime_object = datetime.strptime(datetime_str, '%d/%m/%Y')

    return datetime_object.timestamp()

def convert_string_date_format(date):
    month = date.month
    year = date.year
    day = date.day
    return str(year)+'-'+str(month)+'-'+str(day)

def Save_Customer_ByteSave(request, id):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        try:
            Customer_ByteSave.create(**{
                'id_customer': id,
                'bytesave_email': form.get('bytesave_email'),
                'bytesave_pwd': form.get('bytesave_pwd'),
                # 'bytesave_amount_used': form.get('bytesave_amount_used'),
                # 'bytesave_duration': form.get('bytesave_duration'),
                # 'bytesave_time_type': form.get('bytesave_time_type'),
                # 'bytesave_expiration_date': form.get('bytesave_expiration_date'),
            })
            return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
        except Exception as e:
            return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})




# def Save_Customer_ByteSave(request, id):
#     if request.method == 'POST':
#         data = json.dumps(request.POST)
#         form = json.loads(data)
#         try:
#             Customer_ByteSave.create(**{
#                 'id_customer': id,
#                 'bytesave_email': form.get('bytesave_email'),
#                 'bytesave_pwd': form.get('bytesave_pwd'),
#                 'bytesave_amount_used': form.get('bytesave_amount_used'),
#                 'bytesave_duration': form.get('bytesave_duration'),
#                 'bytesave_time_type': form.get('bytesave_time_type'),
#                 'bytesave_expiration_date': form.get('bytesave_expiration_date'),
#             })
#             return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
#         except Exception as e:
#             return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})





def Check_Customer(request):
    data = json.dumps(request.POST)
    form = json.loads(data)

    item = Customers.objects.filter(email=form.get('email')).filter(~Q(is_del=1)).filter(~Q(id=(form.get('id')) if form.get('id') != '' else 0)).filter((~Q(is_del=1)))
    if (item.count() > 0):
        return JsonResponse({'status': 'false', 'msg': 'Email đã tồn tại!'})
    item = Customers.objects.filter(tax_code=form.get('tax_code')).filter(~Q(is_del=1)).filter(~Q(id=(form.get('id')) if form.get('id') != '' else 0)).filter((~Q(is_del=1)))
    if (item.count() > 0):
        return JsonResponse({'status': 'false', 'msg': 'Mã số thuế đã tồn tại!'})
    return JsonResponse({'status': 'true', 'msg': 'Thành công!'})


def Del_Customer(request, id):
    try:
        item = Customers.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            item_customer_bytesave = Customer_ByteSave.objects.get(id_customer=item.id)
            item_customer_bytesave.is_del = 1
            item_customer_bytesave.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!'})

def Reset_Pwd_Customer_ByteSave(request, id):
    try:
        item = Customer_ByteSave.objects.get(id=id)
        if item != None:
            pwd=genpwd(12)
            item.bytesave_pwd = encryption(pwd)
            item.save()
            notification_customer_bytesave('[ByteSave]', item.bytesave_email, pwd, item.bytesave_email)
            return JsonResponse({'status': 'true', 'msg': 'Reset mật khẩu thành công và thông tin đã được gửi tới '+item.bytesave_email})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Reset mật khẩu không thành công!'})


def Check_Represent(request):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        item = Customer_Represent.objects.filter(id_customer=form.get('id_customer')).filter(phone_number=form.get('represent_phone_number'))
        if item.count() > 0:
            return JsonResponse({'status': 'false', 'msg': 'Người đại diện đã tồn tại!'})
        return JsonResponse({'status': 'true', 'msg': 'Thành công!'})




def Save_Customer_Represent(request, id):
    a = 0
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        id_customer = form.get('id_customer', 0)
        if id == 0:  # thêm mới
            try:
                Customer_Represent.create(**{
                    'id_customer': form.get('id_customer'),
                    'name': form.get('represent_name'),
                    'phone_number': form.get('represent_phone_number'),
                    'position': form.get('represent_position'),
                    'email': form.get('represent_email'),
                    'type': form.get('represent_type'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:  # Chỉnh sửa
            try:
                item = Customer_Represent.objects.get(id=id)

                item.id_customer = form.get('id_customer')
                item.name = form.get('represent_name')
                item.phone_number = form.get('represent_phone_number')
                item.position = form.get('represent_position')
                item.email = form.get('represent_email')
                item.type = form.get('represent_type')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()
                return JsonResponse({'status': 'true', 'msg': 'Chỉnh sửa thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

    return None



def Del_Customer_Represent(request, id):
    try:
        item = Customer_Represent.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!', 'string_error': e})

def get_ex_date(request, id):
    try:
        item = Customer_Represent.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!', 'string_error': e})
