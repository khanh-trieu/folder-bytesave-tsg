import os

BYTESAVE_API_PVT = os.getenv('BYTESAVE_API_PVT') if os.getenv('BYTESAVE_API_PVT') != None else 'http://13.212.85.136:8000/'
#BYTESAVE_API_PVT = 'http://13.212.85.136:8000/'
# BYTESAVE_API_PVT = os.getenv('BYTESAVE_API_PVT') if os.getenv('BYTESAVE_API_PVT') != None else 'http://127.0.0.1:8080/'

API_SRV_PVT = BYTESAVE_API_PVT
API_LOGGIN = API_SRV_PVT+ 'dang-nhap-bytesave/'
API_LOGGED = API_SRV_PVT+ 'dang-nhap-bytesave/kiem-tra-logged/'
API_LOGOUT = API_SRV_PVT+ 'dang-xuat-bytesave/'


API_EX_DATE = API_SRV_PVT+ 'bytesave-ngay-het-han/'

API_INFO = API_SRV_PVT+ 'bytesave-thong-tin/'

API_SETTING = API_SRV_PVT+ 'bytesave-cai-dat-chung/'

API_LIST_BACKUP = API_SRV_PVT+ 'bytesave-sao-luu/'
API_CHECK_BACKUP = API_SRV_PVT+ 'bytesave-sao-luu/kiem-tra/'
API_DEL_BACKUP = API_SRV_PVT+ 'bytesave-sao-luu/xoa/'

API_LIST_CONNECT = API_SRV_PVT+ 'bytesave-ket-noi/'
API_DETAIL_CONNECT = API_SRV_PVT+ 'bytesave-ket-noi/chi-tiet/'
API_CHECK_CONNECT = API_SRV_PVT+ 'bytesave-ket-noi/kiem-tra/'
API_DEL_CONNECT = API_SRV_PVT+ 'bytesave-ket-noi/xoa/'


API_AGENT_CREATE_UPDATE_METRIC_SERVICE_CHECK = API_SRV_PVT+ 'dich-vu-luu-tru/kiem-tra/'
API_AGENT_CREATE_UPDATE_METRIC_SERVICE = API_SRV_PVT+ 'agent/them-moi-dich-vu-luu-tru/'