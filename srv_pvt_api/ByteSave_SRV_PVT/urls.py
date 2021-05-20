"""ByteSave_SRV_PVT URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/3.1/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import path

from ByteSave_Backup.views import load_data_bytesave_backup, check_name_backup, del_backup_bytesave, \
    load_data_bytesave_backup_in_web
from ByteSave_Connect.views import load_data_bytesave_connect, load_detail_bytesave_connect, check_account_name, \
    del_connect_bytesave, load_data_bytesave_connect_in_web
from ByteSave_Loggin.views import loggin_bytesave, check_logged, logout_bytesave, check_ex_date
from ByteSave_Setting.views import load_data_setting_bytesave, information_bytesave
from Customer.views import Load_data as data_customer, Save_Customer_ByteSave, Check_Customer, Del_Customer, \
    Save_Customer_Represent, \
    Del_Customer_Represent, Check_Represent, Reset_Pwd_Customer_ByteSave, save_cycle_bytesave, Del_cycle
from Loggin_Web.views import loggin_web, load_data
from MetricService.views import Load_Service, Load_data, Check_Metric_Service, Del_Metric_Service, Load_used_capacity, \
    Load_used_capacity_of_agent
from Agent.views import Load_data as load_data_agent, load_version, create_metric_service_agent
from django.views.decorators.csrf import csrf_exempt

from Send_mail.views import send_mails
from Version.views import load_data_verrsion, Del_Version

urlpatterns = [
    # Customer
    path('khach-hang/<int:id>', csrf_exempt(data_customer)),
    # id: là id của khách hàng
    path('khach-hang/dang-nhap-bytesave/<int:id>', csrf_exempt(Save_Customer_ByteSave)),
    path('khach-hang/kiem-tra/', csrf_exempt(Check_Customer)),
    path('khach-hang/xoa/<int:id>', csrf_exempt(Del_Customer)),
    # id: là id của bytesave customer
    path('khach-hang/reset-mat-khau/<int:id>', csrf_exempt(Reset_Pwd_Customer_ByteSave)),

    # id: là id của người đại diện
    path('khach-hang/them-nguoi-dai-dien/<int:id>', csrf_exempt(Save_Customer_Represent)),
    path('khach-hang/kiem-tra-nguoi-dai-dien/', csrf_exempt(Check_Represent)),
    path('khach-hang/xoa-nguoi-dai-dien/<int:id>', csrf_exempt(Del_Customer_Represent)),

    # Load metric_service
    # id: là id của khách hàng sử dụng dịch vụ này
    path('dich-vu-luu-tru/<int:id>', csrf_exempt(Load_data)),
    path('dich-vu-luu-tru/kiem-tra/', csrf_exempt(Check_Metric_Service)),
    path('dich-vu-luu-tru/xoa/<int:id>', csrf_exempt(Del_Metric_Service)),

    # id: là id của khách hàng
    path('dich-vu-luu-tru/load_dsd/<int:id>', csrf_exempt(Load_used_capacity)),
    # id: là id của metric service
    path('dich-vu-luu-tru/load_dsd_agent/<int:id>', csrf_exempt(Load_used_capacity_of_agent)),

    # Load service
    path('dich-vu/', csrf_exempt(Load_Service)),
    # Load version
    path('phien-ban/', csrf_exempt(load_data_verrsion)),
    path('phien-ban/xoa/<int:id>', csrf_exempt(Del_Version)),

    # Load Agnet
    # id: laf id của khách hàng
    path('agent/<int:id>', csrf_exempt(load_data_agent)),

    path('agent/them-moi-dich-vu-luu-tru/', csrf_exempt(create_metric_service_agent)),


    # Load Connect Bytesave
    path('bytesave-ket-noi/<str:serial_number>/<str:email_loggin>', csrf_exempt(load_data_bytesave_connect)),
    # id: là id của connect_bytesave
    path('bytesave-ket-noi/chi-tiet/<int:id>', csrf_exempt(load_detail_bytesave_connect)),
    path('bytesave-ket-noi/kiem-tra/<str:serial_number>/<str:name>', csrf_exempt(check_account_name)),
    path('bytesave-ket-noi/xoa/<int:id>', csrf_exempt(del_connect_bytesave)),

    path('bytesave-ket-noi-web/<str:email>', csrf_exempt(load_data_bytesave_connect_in_web)),


    # Load Backup Bytesave
    # serial_number: serial_number của agent
    path('bytesave-sao-luu/<str:serial_number>/<str:email>', csrf_exempt(load_data_bytesave_backup)),
    path('bytesave-sao-luu/kiem-tra/<str:serial_number>/<str:name>', csrf_exempt(check_name_backup)),
    path('bytesave-sao-luu/xoa/<int:id>', csrf_exempt(del_backup_bytesave)),
    # load backups in web
    path('bytesave-sao-luu-web/<str:email>', csrf_exempt(load_data_bytesave_backup_in_web)),

    # load setting bytesave
    path('bytesave-cai-dat-chung/<int:id>/<str:serial_number>/<str:email_loggin>', csrf_exempt(load_data_setting_bytesave)),

    # load setting bytesave
    path('bytesave-thong-tin/<str:serial_number>/<str:email_loggin>', csrf_exempt(information_bytesave)),

    # đăng nhập website server
    path('dang-nhap/', csrf_exempt(loggin_web)),

    #check thời  gian hết hạn
    path('bytesave-ngay-het-han/<str:serial_number>', csrf_exempt(check_ex_date)),

    #Thêm mới chu kì sử dụng bytesave
    #id: id của cycle
    path('bytesave-chu-ki/them-moi/<int:id>', csrf_exempt(save_cycle_bytesave)),
    path('bytesave-chu-ki/xoa/<int:id>', csrf_exempt(Del_cycle)),


    # đăng nhập website server
    path('dang-nhap-bytesave/', csrf_exempt(loggin_bytesave)),
    path('dang-nhap-bytesave/kiem-tra-logged/', csrf_exempt(check_logged)),
    path('dang-xuat-bytesave/', csrf_exempt(logout_bytesave)),

    # load tài khoản đăng nhập
    path('tai-khoan/', csrf_exempt(load_data)),

    # Gửi mail cho khách hàng thông tin đăng nhập
    path('gui-mail/', csrf_exempt(send_mails)),

    path('admin/', admin.site.urls),
]
