## Pull python 3 image
#FROM python:3
## Create a work dir
#
## copy requirements.txt into workdir created above
#COPY requirements.txt ./
## Install all requirements
#RUN python3 -m pip install --user --no-cache-dir -r requirements.txt
## Copy entire project into workdir
#COPY . .
#WORKDIR .
## Run our app without output
#CMD ["python", "manage.py"]


FROM ubuntu:20.04

RUN apt-get update
RUN apt-get install -y python3-pip
RUN apt-get install -y python3-dev
RUN apt-get install -y g++
RUN apt-get install -y unixodbc-dev
#RUN apt install unixodbc-bin -y
#RUN apt-get clean -
#RUN python3 -m pip install pyodbc==4.0.30

RUN mkdir /code
WORKDIR /code
COPY requirements.txt /code/
#RUN pip install -r requirements.txt
#COPY requirements.txt /.
RUN python3 -m  pip install  -r requirements.txt

COPY . .
WORKDIR .
