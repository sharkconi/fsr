#!/usr/bin/env python
# -*- coding: utf-8 -*-

from flask import Flask, render_template, redirect, url_for
import MySQLdb
from decimal import *
import datetime
import time

app = Flask(__name__)

def handle_ribao_data(res):
    ds = []
    all_in = Decimal(0.0)
    for line in res:
        if Decimal(line[5]) == 0.0 or Decimal(line[6]) == 0.0:
            continue
        d = (line[2], '{:10,.2f}'.format(Decimal(line[6])), '{:10,.2f}'.format(Decimal(line[5])), '{:10,.2f}'.format(Decimal(line[6])/Decimal(line[5])))
        all_in = all_in + Decimal(line[6])
        ds.append(d)
    return (ds, all_in)

@app.route('/m')
def fsr_mobile_page():
    date_str = (datetime.datetime.now() - datetime.timedelta(days=1)).strftime("%Y-%m-%d")
    conn = MySQLdb.connect(
        host = 'rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com',
        port = 3306,
        user = 'fsradmin',
        passwd = 'fsradmin',
        db = 'fsr',
        charset = 'utf8',
    )
    cur = conn.cursor()
    
    result = cur.execute("select * from ribao where date = \'" + date_str + "\'")
    ds, all_in = handle_ribao_data(cur.fetchmany(result))
    cur.close()
    conn.close()
 
    return render_template('fsr_mobile.html', ds = ds, d=date_str, all_in='{:6,.2f}'.format(all_in/10000))

@app.route('/web/<date_str>')
def fsr_web_page_date(date_str):
    date_split=date_str.split('-')
    date_now = datetime.datetime(year=int(date_split[0]), month=int(date_split[1]), day=int(date_split[2]))

    date_str_list = [(date_now).strftime("%Y-%m-%d"),
           (date_now - datetime.timedelta(days=1)).strftime("%Y-%m-%d"),
           (date_now - datetime.timedelta(days=2)).strftime("%Y-%m-%d"),
           (date_now - datetime.timedelta(days=3)).strftime("%Y-%m-%d"),
           (date_now - datetime.timedelta(days=4)).strftime("%Y-%m-%d"),
           (date_now - datetime.timedelta(days=5)).strftime("%Y-%m-%d"),]
    conn = MySQLdb.connect(
        host = 'rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com',
        port = 3306,
        user = 'fsradmin',
        passwd = 'fsradmin',
        db = 'fsr',
        charset = 'utf8',
    )
    cur = conn.cursor()
    
    result = cur.execute("select * from ribao where date = \'" + date_str_list[0] + "\'")
    ds, all_in = handle_ribao_data(cur.fetchmany(result))
    cur.close()
    conn.close()
 
    return render_template('fsr_web.html', ds = ds, d=date_str_list, all_in='{:6,.2f}'.format(all_in/10000))

@app.route('/web')
def fsr_web_page_main():
    date_now = (datetime.datetime.now() - datetime.timedelta(days=1)).strftime("%Y-%m-%d")
    return redirect(url_for("fsr_web_page_date", date_str=date_now))

@app.route("/")
def fsr_main():
    return redirect(url_for("fsr_web_page_main"))

if __name__ == "__main__":
    app.run(host = '0.0.0.0', debug = True)
