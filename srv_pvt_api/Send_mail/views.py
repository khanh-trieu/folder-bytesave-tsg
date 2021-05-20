import smtplib
import ssl
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

from django.core.mail import send_mail
from django.http import JsonResponse
from django.shortcuts import render
from django.template import TemplateDoesNotExist, Context, loader
from django.template.loader import get_template, render_to_string
from django.core.mail import EmailMultiAlternatives
sender_email = "khanh.trieu@tsg.net.vn"
password = 'Tsg@1234'
host = 'smtp.office365.com'
port = 587
message = MIMEMultipart("alternative")
message["Subject"] = "[ByteSave Backup]"
message["From"] = sender_email

# Create your views here.
from ByteSave_SRV_PVT import settings

# plaintext = render_to_string('send_mail/templatesendmail.txt')
# template = get_template('send_mail/templatesendmail.html')


def send_mails(request):
    try:
        send_mail('[ByteSave Test]', 'Test send mail tsg', 'khanh.trieu@tsg.net.vn', ['khanh.trieu@tsg.net.vn', ])
        return JsonResponse(
            {'status': 'true', 'msg': 'Gửi thông báo thành công cho email: ' + 'khanh.trieu@tsg.net.vn' + '!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Gửi email không thành công!', 'string_error': e})
    return

def send_mailsss(request):
    try:
        # msg = EmailMultiAlternatives(subject='seet', from_email="khanh.trieu@tsg.net.vn",
        #                              to=["khanh.trieu@tsg.net.vn"], body='123456')
        # html_template = get_template('templatesendmail.html').render()
        # # html_content = htmly.render(d)
        # msg.attach_alternative(html_template, "text/html")
        # msg.send()
        html = """\
               <html>
                 <body>
                   <p>ByteSave <br>
                      Tác vụ sao lưu {name_backup} đã thực hiện sao lưu file: {path_file}
                     <br><br><br> Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
                   </p>
                 </body>
               </html>
               """
        send_mail('[ByteSave Test]', 'Test send mail tsg', 'khanh.trieu@tsg.net.vn', ['khanh.trieu@tsg.net.vn', ])
        return JsonResponse(
            {'status': 'true', 'msg': 'Gửi thông báo thành công cho email: ' + 'khanh.trieu@tsg.net.vn' + '!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Gửi email không thành công!', 'string_error': e})
    return


def notification_customer_bytesave(subject,email,pwd, email_to):
    try:
        html = f"""\
                       <html>
                         <body>
                           <p>ByteSave <br>
                              Thông tin đăng nhập ByteSave:
                             <br>
                             Email : {email}
                             <br>
                             Mật khẩu : {pwd}
                             <br><br><br> Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
                           </p>
                         </body>
                       </html>
                       """

        merge_data = {
            'ORDERNO': "12345", 'TRACKINGNO': "1Z987"
        }

        part = MIMEText(html, "html")

        message.attach(part)
        # Create secure connection with server and send email
        context = ssl.create_default_context()
        with smtplib.SMTP(host, port) as server:
                server.starttls(context=context)
                server.login(sender_email, password)
                server.sendmail(sender_email, email_to, message.as_string())
        return JsonResponse(
            {'status': 'true', 'msg': 'Gửi thông báo thành công cho email: ' + email_to + '!'})
    except Exception as e:
        return JsonResponse({'status': 'false', 'msg': 'Gửi email không thành công!', 'string_error': e})
    return
