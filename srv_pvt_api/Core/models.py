from django.db import models

# Create your models here.
from App_Common import Timer


class Loggin(models.Model):
    email = models.CharField(max_length=250)
    name = models.CharField(max_length=250,null=True)
    pwd = models.CharField(max_length=250,null=True)
    # 0: admin; 1: nhân viên
    type = models.IntegerField(default=1,null=True)
    # 0: không khóa; 1: khóa
    is_lock = models.IntegerField(default=0)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        Loggin.objects.create(
            email=kwargs['email'],
            name=kwargs['name'],
            pwd=kwargs['pwd'],
            type=kwargs['type'],
        )



class Customer_ByteSave(models.Model):
    id_customer = models.IntegerField()
    bytesave_email = models.CharField(max_length=250, null=True, blank=True)
    bytesave_pwd = models.CharField(max_length=250, null=True, blank=True)
    # is_logged = models.IntegerField(default=1)
    is_del = models.IntegerField(default=0)
    bytesave_use_start_date = models.CharField(max_length=250, blank=True, null=True)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        Customer_ByteSave.objects.create(
            id_customer=kwargs['id_customer'],
            bytesave_email=kwargs['bytesave_email'],
            bytesave_pwd=kwargs['bytesave_pwd'],
            bytesave_use_start_date=kwargs['bytesave_use_start_date'],
            #bytesave_amount_used=kwargs['bytesave_amount_used'],
            # bytesave_duration=kwargs['bytesave_duration'],
            # bytesave_time_type=kwargs['bytesave_time_type'],
            # bytesave_expiration_date=kwargs['bytesave_expiration_date'],
        )

class bytesave_cycle(models.Model):
    id_customer = models.IntegerField()
    #Tổng số máy sử dụng
    bytesave_amount_used = models.IntegerField(default=1)
    # Thời hạn sừ dụng
    bytesave_duration = models.IntegerField(null=True, blank=True)
    # 0: tháng; 1: năm
    bytesave_time_type = models.IntegerField(default=1)
    bytesave_start_date = models.IntegerField(null=True,default=Timer.get_timestamp_now())
    bytesave_expiration_date = models.IntegerField(null=True,default=Timer.get_timestamp_now())
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        bytesave_cycle.objects.create(
            id_customer=kwargs['id_customer'],
            bytesave_amount_used=kwargs['bytesave_amount_used'],
            bytesave_duration=kwargs['bytesave_duration'],
            bytesave_time_type=kwargs['bytesave_time_type'],
            bytesave_start_date=kwargs['bytesave_start_date'],
            bytesave_expiration_date=kwargs['bytesave_expiration_date'],
        )


class Customers(models.Model):
    id_loggin = models.IntegerField(default=0)
    id_version = models.IntegerField(default=1)
    name = models.CharField(max_length=250)
    email = models.CharField(max_length=250)
    # 0: cá nhân; 1: doanh nghiệp
    type = models.IntegerField(default=1)
    phone_number = models.CharField(max_length=250, blank=True, null=True)
    # thành phố lưu theo int
    city = models.IntegerField(default=1)
    address = models.CharField(max_length=250, blank=True, null=True)
    tax_code = models.CharField(max_length=250, blank=True, null=True)
    website = models.CharField(max_length=250, blank=True, null=True)
    fax = models.CharField(max_length=250, blank=True, null=True)
    # người đại diện pháp luật
    legal_representative = models.CharField(max_length=250, blank=True, null=True)
    scale = models.IntegerField(default=1)
    # lĩnh vức
    field = models.IntegerField(default=1)
    # 0: không ký hợp đồng nâng cấp; 1: ký hợp đồng nâng cấp
    is_upgrade_version = models.IntegerField(default=0)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        Customers.objects.create(
            id_loggin=kwargs['id_loggin'],
            id_version=kwargs['id_version'],
            name=kwargs['name'],
            email=kwargs['email'],
            type=kwargs['type'],
            phone_number=kwargs['phone_number'],
            city=kwargs['city'],
            address=kwargs['address'],
            tax_code=kwargs['tax_code'],
            website=kwargs['website'],
            fax=kwargs['fax'],
            legal_representative=kwargs['legal_representative'],
            scale=kwargs['scale'],
            field=kwargs['field'],
            is_upgrade_version=kwargs['is_upgrade_version'],
        )



class Customer_Represent(models.Model):
    id_customer = models.IntegerField()
    name = models.CharField(max_length=250, blank=True)
    phone_number = models.CharField(max_length=250, blank=True)
    # chức vụ người đại diện
    position = models.CharField(max_length=250, blank=True)
    email = models.CharField(max_length=250, blank=True)
    # 0: đầu mối liên hệ; 1: còn liên hệ; 2: dừng liên hệ
    type = models.IntegerField(null=True)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        Customer_Represent.objects.create(
            id_customer=kwargs['id_customer'],
            name=kwargs['name'],
            phone_number=kwargs['phone_number'],
            position=kwargs['position'],
            email=kwargs['email'],
            type=kwargs['type'],
        )


class Metric_Services(models.Model):
    id_customer = models.IntegerField()
    id_service = models.IntegerField()
    information_connect = models.CharField(max_length=250, blank=True)
    max_storage = models.IntegerField()
    username_account = models.CharField(max_length=250, blank=True)
    # 0: Azure Blob Storage; 1: đang cập nhật
    is_service = models.IntegerField()
    # 0: Chưa sử dụng; 1: Đã sử dụng
    status = models.IntegerField(default=0)
    #0: tạo trên agent: 1: tạo tạo web server
    type = models.IntegerField(default=0)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        new = Metric_Services.objects.create(
            id_customer=kwargs['id_customer'],
            id_service=kwargs['id_service'],
            information_connect=kwargs['information_connect'],
            max_storage=kwargs['max_storage'],
            username_account=kwargs['username_account'],
            type=1,
            is_service=0,
            status = 0,
        )
        return new.id


class Agents(models.Model):
    id_customer = models.IntegerField()
    id_customer_bytesave = models.IntegerField(default=0)
    os = models.CharField(max_length=250, blank=True,null=True)
    serial_number = models.CharField(max_length=250, blank=True,null=True)
    ip_public = models.CharField(max_length=250, blank=True,null=True)
    ip_private = models.CharField(max_length=250, blank=True,null=True)
    name_computer = models.CharField(max_length=250, blank=True,null=True)
    #agent duy trì đăng nhập
    #0: khong
    #1: co duy trì
    is_logged = models.IntegerField(default=1)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        new = Agents.objects.create(
            id_customer=kwargs['id_customer'],
            id_customer_bytesave=kwargs['id_customer_bytesave'],
            os=kwargs['os'],
            serial_number=kwargs['serial_number'],
            ip_public=kwargs['ip_public'],
            ip_private=kwargs['ip_private'],
            name_computer=kwargs['name_computer'],
        )
        return new.id


class Service(models.Model):
    name = models.CharField(max_length=100, blank=True)
    description = models.CharField(max_length=250, blank=True)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())


class Versions(models.Model):
    name = models.CharField(max_length=100, blank=True)
    description = models.CharField(max_length=250, blank=True)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        Versions.objects.create(
            name=kwargs['name'],
            description=kwargs['description'],
            is_del=0,
        )


class connect_bytesave(models.Model):
    id_agent = models.IntegerField()
    id_customer = models.IntegerField(default=0)
    id_metric_service = models.IntegerField()
    name = models.CharField(max_length=250, blank=True)
    #type:#Chưa sử dụng để làm gì hết :D
    type = models.IntegerField()
    is_del = models.IntegerField(default=0)
    time_check_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        connect_bytesave.objects.create(
            id_agent=kwargs['id_agent'],
            id_customer=kwargs['id_customer'],
            id_metric_service=kwargs['id_metric_service'],
            name=kwargs['name'],
            type=kwargs['type'],
            is_del=0,
        )


class backup_bytesave(models.Model):
    id_agent = models.IntegerField()
    id_connect_bytesave = models.IntegerField()
    name = models.CharField(max_length=250, blank=True)
    local_path = models.CharField(max_length=250, blank=True)
    container_name = models.CharField(max_length=250, blank=True)
    email = models.CharField(max_length=250, blank=True,null=True)
    #Định dạng chuỗi time:
    #dayofweek|hour:minute
    #dayofweek: 0,1,2,3,4,5,6 tương tự từ thứ 2 đến chủ nhật
    #hour:minute: ví dụ: 13:20,15:12
    time_delete = models.IntegerField()
    time_delete_file_in_LastVersion = models.IntegerField(default=30)
    time = models.CharField(max_length=250, blank=True,null=True)
    # Giới hạn hệ thống xử lí luồng max
    max_concurrency = models.IntegerField(default=10)
    #0:file; 1:folder
    is_folder = models.IntegerField()
    time_last_run = models.IntegerField(null=True)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        backup_bytesave.objects.create(
            id_agent=kwargs['id_agent'],
            id_connect_bytesave=kwargs['id_connect_bytesave'],
            name=kwargs['name'],
            local_path=kwargs['local_path'],
            container_name=kwargs['container_name'],
            email=kwargs['email'],
            time_delete=kwargs['time_delete'],
            time_delete_file_in_LastVersion=kwargs['time_delete_file_in_LastVersion'],
            time=kwargs['time'],
            is_folder=kwargs['is_folder'],
            time_last_run=0,
            is_del=0,
        )

class setting_bytesave(models.Model):
    id_agent = models.IntegerField()
    server_mail = models.CharField(max_length=250, blank=True)
    port = models.CharField(max_length=250, blank=True)
    mail_send = models.CharField(max_length=250, blank=True)
    mail_send_pwd = models.CharField(max_length=250, blank=True,null=True)
    subject = models.CharField(max_length=250, blank=True,null=True)
    is_ssl = models.IntegerField(default=1)
    is_del = models.IntegerField(default=0)
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        setting_bytesave.objects.create(
            id_agent=kwargs['id_agent'],
            server_mail=kwargs['server_mail'],
            port=kwargs['port'],
            mail_send=kwargs['mail_send'],
            mail_send_pwd=kwargs['mail_send_pwd'],
            subject=kwargs['subject'],
        )
class log_contents(models.Model):
    log_content = models.CharField(max_length=500, blank=True)
    function = models.CharField(max_length=250, blank=True)
    status = models.CharField(max_length=250, blank=True)
    time_log = models.IntegerField(default=Timer.get_timestamp_now())
    time_create_at = models.IntegerField(default=Timer.get_timestamp_now())
    time_update_at = models.IntegerField(default=Timer.get_timestamp_now())

    def create(**kwargs):
        log_contents.objects.create(
            log_content=kwargs['log_content'],
            function=kwargs['function'],
            status=kwargs['status'],
        )
