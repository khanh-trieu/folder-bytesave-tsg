import hashlib
import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import encryption, Timer
from Core.models import Loggin


def loggin_web(request):
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        email = form.get('email')
        pwd = form.get('pwd')
        password = encryption(pwd)
        item = Loggin.objects.filter(email=email)
        if item.count() == 0:
            return JsonResponse({'status': 'false', 'msg': 'Email nhập không chính xác!'})
        item = Loggin.objects.filter(email=email).filter(pwd=password)
        if item.count() == 0:
            return JsonResponse({'status': 'false', 'msg': 'Mật khẩu nhập không chính xác!'})
        return JsonResponse({'status': 'true', 'msg': 'Đăng nhập thành công!'})


def load_data(request):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            for item in Loggin.objects.all().filter((~Q(is_del=1))):
                countdata = 1
                data.append({
                    'id': item.id,
                    'name': item.name,
                    'email': item.email,
                    'type': item.type,
                })
                countdata += 1
            return JsonResponse({'status': 'true', 'countdata': countdata, 'data': data})
        except Exception as e:
            return JsonResponse({'status': 'false', 'countdata': 0, 'data': [], 'string_error': e})
    if request.method == 'POST':
        data = json.dumps(request.POST)
        form = json.loads(data)
        id = form.get('id', 0)
        if id == '0' or id == '':  # thêm mới
            try:
                Loggin.create(**{
                    'email': form.get('email'),
                    'name': form.get('name'),
                    'pwd': encryption(form.get('pwd')),
                    'type': form.get('type'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:  # Chỉnh sửa
            try:
                item = Loggin.objects.get(id=int(id))
                item.email = form.get('email')
                item.name = form.get('name')
                item.pwd = encryption(form.get('pwd'))
                item.type = form.get('type')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()
                return JsonResponse({'status': 'true', 'msg': 'Chính sửa thành công!'})

            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

        return None;