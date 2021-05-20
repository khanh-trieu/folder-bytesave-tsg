"C:\Program Files (x86)\ByteSave\ByteSave\setup_service.exe" install
sc config bytesave_service_backup start=auto
sc start  bytesave_service_backup
pip install -r requirements.txt 
icacls "C:\Program Files (x86)\ByteSave" /grant Users:(OI)(CI)F /T

