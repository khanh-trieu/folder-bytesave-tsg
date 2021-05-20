FROM ubuntu:20.04

RUN apt-get update
RUN apt-get install -y python3-pip
RUN apt-get install -y python3-dev
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

