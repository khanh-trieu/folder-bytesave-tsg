import json

from django.db.models import Q
from django.http import JsonResponse
from django.shortcuts import render

# Create your views here.
from App_Common import Timer
from Core.models import Versions


def load_data_verrsion(request):
    if request.method == 'GET':
        try:
            data = []
            countdata = 0
            for item in Versions.objects.all().filter((~Q(is_del=1))):
                countdata = 1
                data.append({
                    'id': item.id,
                    'name': item.name,
                    'description': item.description,
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
        id_version = form.get('id_version', 0)
        if id_version == '0' or id_version == '':  # thêm mới
            try:
                Versions.create(**{
                    'name': form.get('name'),
                    'description': form.get('description'),
                })
                return JsonResponse({'status': 'true', 'msg': 'Thêm mới thành công!'})
            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Thêm mới không thành công!', 'string_error': e})
        else:  # Chỉnh sửa
            try:
                item = Versions.objects.get(id=id_version)
                item.name = form.get('name')
                item.description = form.get('description')
                item.time_update_at = Timer.get_timestamp_now()
                item.save()

                return JsonResponse({'status': 'true', 'msg': 'Chính sửa thành công!'})

            except Exception as e:
                return JsonResponse({'status': 'false', 'msg': 'Chỉnh sửa không thành công!', 'string_error': e})

        return None;

def Del_Version(request, id):
    try:
        item = Versions.objects.get(id=id)
        if item != None:
            item.is_del = 1
            item.save()
            return JsonResponse({'status': 'true', 'msg': 'Xóa thành công!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Xóa không thành công!'})


