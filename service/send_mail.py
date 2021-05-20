import smtplib, ssl
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from datetime import datetime

now = datetime.now()


sender_email = "khanh.trieu@tsg.net.vn"
password = 'Tsg@1234'
host = 'smtp.office365.com'
port = 587
message = MIMEMultipart("alternative")
message["Subject"] = "[ByteSave Backup]"
message["From"] = sender_email


def send_mail(is_folder,name_backup,count_file,path_file,list_file_upload,email_receiver):
    try:
        # Create the plain-text and HTML version of your message
        html = """\
        <html>
          <body>
            <p>ByteSave thông báo hoạt động [{time_run}]<br>
               Tác vụ sao lưu {name_backup} đã thực hiện sao lưu file: {path_file}
              <br><br><br> Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
            </p>
          </body>
        </html>
        """
        html_folder = """\
                <html>
                  <body>
                    <p>ByteSave thông báo hoạt động [{time_run}]<br>
                       Tác vụ sao lưu {name_backup} đã thực hiện sao lưu {count_file_backup} trong folder {path_file} : {list_file_upload}
                       <br> <br> <br>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
                    </p>
                  </body>
                </html>
                """


        time_run = now.strftime("%d/%m/%Y %H:%M")
        part = MIMEText((html_folder if is_folder == 1 else html).replace('{name_backup}',name_backup).replace('{time_run}',str(time_run)).replace('{count_file_backup}',str(count_file))
                        .replace('{list_file_upload}',str(list_file_upload)).replace('{path_file}',path_file), "html")
        message.attach(part)
        # Create secure connection with server and send email
        context = ssl.create_default_context()
        for item in email_receiver.split(','):
            with smtplib.SMTP(host, port) as server:
                server.ehlo()  # Can be omitted
                server.starttls(context=context)
                server.ehlo()  # Can be omitted
                server.login(sender_email, password)
                server.sendmail(sender_email, item, message.as_string())
    except Exception as e:
        print(e)
