#!/usr/bin/env python
# -*- coding: utf-8 -*-

from flask import Flask, render_template, redirect, url_for
import MySQLdb
from decimal import *
import datetime
import time
import json

app = Flask(__name__)
area_def=["S02","S06", "S07", "S14", "S23", "F02", "S30", "F09", "F03", u"一区小计",
        "S03", "S20", "S27", "S28", "S33", "F07", "F04", u"二区小计",
        "S04", "S08", "S15", "S18", "S31", "S36", "S37", "S39", u"三区小计",
        "S09", "S17", "S21", "S24", "S25", "S32", "S35", "F10", u"四区小计",
         "S10", "S12", "S19", "S22", "S26", "S38", "S40", "G01", u"五区小计",
         "F01", "F06", "Y01", "Y02", "L01", u"六区小计",
         "F08", "F05", "L02", u"北京区域小计",
         "005", "006", u"点沁系列小计"]

area_def_v2 =[{"name":u"一区小计", "depart":["S02","S06", "S07", "S14", "S23", "F02", "S30", "F09", "F03"], "category":"一区"},
        {"name":u"二区小计", "depart":["S03", "S20", "S27", "S28", "S33", "F07", "F04"], "category":"二区"},
        {"name":u"三区小计", "depart":[ "S04", "S08", "S15", "S18", "S31", "S36", "S37", "S39"], "category":"三区"},
        {"name":u"四区小计", "depart":["S09", "S17", "S21", "S24", "S25", "S32", "S35", "F10"], "category":"四区"},
        {"name":u"五区小计", "depart":["S10", "S12", "S19", "S22", "S26", "S38", "S40", "G01"], "category":"五区"},
        {"name":u"六区小计", "depart":["F01", "F06", "Y01", "Y02", "L01"], "category":"六区"},
        {"name":u"北京区小计", "depart":["F08", "F05", "L02"], "category":"北京区"},
        {"name":u"点沁系列小计", "depart":["005", "006"], "category":"点沁系列"}]

def handle_ribao_data(res):
    ds = []
    all_in = Decimal(0.0)
    for line in res:
        if Decimal(line[5]) == 0.0 or Decimal(line[6]) == 0.0:
            d = (line[1], line[2], Decimal(0.0), Decimal(0.0), Decimal(0.0))
            ds.append(d)
            continue
        d = (line[1], line[2], Decimal(line[10])-Decimal(line[11]), Decimal(line[5]), (Decimal(line[10])-Decimal(line[11]))/Decimal(line[5]))
        all_in = all_in + Decimal(line[10]) - Decimal(line[11])
        ds.append(d)

    da = []
    sum = [Decimal(0.0), Decimal(0.0), Decimal(0.0)]
    for area in area_def:
        if len(area) > 3:
            da.append([area, '{:6,.2f}'.format(sum[0]), '{:5}'.format(sum[1]), '{:3,.2f}'.format(sum[0]/sum[1]), True])
            sum = [Decimal(0.0), Decimal(0.0), Decimal(0.0)]
        for d in ds:
            if d[0] == area:
                da.append([d[1], '{:6,.2f}'.format(d[2]), '{:5}'.format(d[3]), '{:3,.2f}'.format(d[4]), False])
                sum[0] = sum[0] + d[2]
                sum[1] = sum[1] + d[3]
    return (da, all_in)

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
    return redirect(url_for("fsr_main_v2"))

def fsr_get_mysql_data_v2(date_str):
    ds = []
    all_in = Decimal(0.0)
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
    for line in cur.fetchmany(result):
        if Decimal(line[5]) == 0.0 or Decimal(line[6]) == 0.0:
            d = (line[1], line[2], Decimal(0.0), Decimal(0.0), Decimal(0.0))
            ds.append(d)
            continue
        d = (line[1], line[2], Decimal(line[10])-Decimal(line[11]), Decimal(line[5]), (Decimal(line[10])-Decimal(line[11]))/Decimal(line[5]))
        ds.append(d)

    cur.close()
    conn.close()
    return ds

def fsr_get_category_data_v2(ds, area, date_str=None):
    if area == "sum":
         da = []
         all_in = Decimal(0.0)
         chart_category = []
         serial_data = []
         idx = 0
         for area in area_def_v2:
            idx = idx + 1
            sum = [Decimal(0.0), Decimal(0.0)]
            for dep in area["depart"]:
                for d in ds:
                    if d[0] == dep:
                        sum[0] = sum[0] + d[2]
                        sum[1] = sum[1] + d[3]
            all_in = all_in + sum[0]
            da.append([area["name"], '{:6,.2f}'.format(sum[0]), '{:5}'.format(sum[1]), '{:3,.2f}'.format(sum[0]/sum[1]), "/v2/web/"+date_str+"_" + str(idx)])
            chart_category.append(area["category"])
            serial_data.append(float('{:6,.2f}'.format(sum[0]/10000)))
            name = u"丰收日"
    elif area == "1" or  area == "2" or  area == "3" or  area == "4" or  area == "5" or  area == "6" or  area == "7" or  area == "8":
        da = []
        all_in = Decimal(0.0)
        chart_category = []
        serial_data = []
        name = area_def_v2[int(area)-1]["name"]
        for dep in area_def_v2[int(area)-1]["depart"]:
            for d in ds:
                if d[0] == dep:
                    all_in = all_in + d[2]
                    da.append([d[1], '{:6,.2f}'.format(d[2]), '{:5}'.format(d[3]), '{:3,.2f}'.format(d[2]/d[3]), ""])
                    chart_category.append(d[1])
                    serial_data.append(float('{:6,.2f}'.format(d[2]/10000)))
    else:
        pass

    return (da, all_in, json.dumps(chart_category), json.dumps(serial_data), name)

@app.route("/v2/web/<date_str>")
def fsr_web_page_date_v2(date_str):
    area_str = date_str[-1:]
    date_str = date_str[:len(date_str)-2]
    isBack = True
    date_now = (datetime.datetime.now() - datetime.timedelta(days=1)).strftime("%Y-%m-%d")

    date_split=date_str.split('-')
    date_today = datetime.datetime(year=int(date_split[0]), month=int(date_split[1]), day=int(date_split[2]))
    date_before = (date_today - datetime.timedelta(days=1)).strftime("%Y-%m-%d")
    if date_now == date_str:
        date_after=""
    else:
        date_after = (date_today + datetime.timedelta(days=1)).strftime("%Y-%m-%d")

    if area_str == "0":
       area_str = "sum"
       isBack = False

    raw_ds = fsr_get_mysql_data_v2(date_str)
    if len(raw_ds) == 0:
        return render_template("fsr_web_v2.html", all_in=0.0, ds=[], d=[date_str, date_before, date_after], name = "",
                chart_category=None, serial_data=None, isBack=isBack)
    else:
        ds, all_in, chart_category, serial_data, name = fsr_get_category_data_v2(raw_ds, area_str, date_str)
        return render_template("fsr_web_v2.html", all_in='{:6,.2f}'.format(all_in/10000), ds=ds, d=[date_str, date_before, date_after], name = name,
                chart_category=chart_category, serial_data=serial_data, isBack=isBack)

@app.route("/v2/")
def fsr_main_v2():
    date_now = (datetime.datetime.now() - datetime.timedelta(days=1)).strftime("%Y-%m-%d")
    return redirect(url_for("fsr_web_page_date_v2", date_str= (date_now + "_0")))

if __name__ == "__main__":
    app.run(host = '0.0.0.0', debug = True)
